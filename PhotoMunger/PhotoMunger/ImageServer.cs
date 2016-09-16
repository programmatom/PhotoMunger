/*
 *  Copyright © 2010-2016 Thomas R. Lawrence
 * 
 *  GNU General Public License
 * 
 *  This file is part of PhotoMunger
 * 
 *  PhotoMunger is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 
*/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace AdaptiveImageSizeReducer
{
    //
    // ClientServerCommunication
    //

    public static class ClientServerCommunication
    {
        public enum Commands
        {
            Quit = 0,
            Done = 1,
            Pending = 2,
            Exception = 3,
            Clear = 4,
            LoadAndOrientGDI = 5,
            ResizeGDI = 6,
            CreateTileGDI = 7,
            ShrinkExpandGDI = 8,
            ShrinkExpandWPF = 9,
        };

        private static byte[] ReadCommandPacket(PipeStream channel)
        {
            int length;
            {
                byte[] lengthBuffer = new byte[4];
                int o = 0;
                while (o != lengthBuffer.Length)
                {
                    int r = lengthBuffer.Length - o;
                    int c = channel.Read(lengthBuffer, o, r);
                    if (!channel.IsConnected)
                    {
                        throw new ChannelDisconnectedException("Channel is not connected");
                    }
                    if ((c == 0) && (r != 0)) // TODO: why isn't this blocking?
                    {
                        Thread.Sleep(20);
                    }
                    o += c;
                }
                length = lengthBuffer[0] | ((int)lengthBuffer[1] << 8) | ((int)lengthBuffer[2] << 16) | ((int)lengthBuffer[3] << 24);
            }

            byte[] buffer;
            {
                buffer = new byte[length];
                int o = 0;
                while (o != buffer.Length)
                {
                    int r = buffer.Length - o;
                    int c = channel.Read(buffer, o, r);
                    if (!channel.IsConnected)
                    {
                        throw new ChannelDisconnectedException("Channel is not connected");
                    }
                    if ((c == 0) && (r != 0)) // TODO: why isn't this blocking?
                    {
                        Thread.Sleep(20);
                    }
                    o += c;
                }
            }

            return buffer;
        }

        private static void WriteCommandPacket(PipeStream channel, byte[] buffer)
        {
            int l = buffer.Length;
            byte[] lengthBuffer = new byte[4] { (byte)l, (byte)(l >> 8), (byte)(l >> 16), (byte)(l >> 24) };
            channel.Write(lengthBuffer, 0, lengthBuffer.Length);
            channel.Write(buffer, 0, buffer.Length);
            channel.FlushAsync();
        }

        public static void SendMessage(TextWriter log, PipeStream channel, Commands command, object[] payload, object[] extra = null)
        {
            if (log != null)
            {
                log.WriteLine("  SendMessage {0}", command);
                foreach (object[] outs in new object[] { payload, extra })
                {
                    if (outs != null)
                    {
                        for (int i = 0; i < outs.Length; i++)
                        {
                            if (!(outs[i] is byte[]))
                            {
                                log.WriteLine("    args[{0}] = ({1}){2}", i, outs[i].GetType().Name, outs[i]);
                            }
                            else
                            {
                                log.WriteLine("    args[{0}] = new byte[{1}] {{ {2} }}", i, ((byte[])outs[i]).Length, FormatByteArray((byte[])outs[i]));
                            }
                        }
                    }
                }
                log.Flush();
            }

            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true/*leaveOpen*/))
            {
                writer.Write((byte)command);
                writer.Write((int)((payload != null ? payload.Length : 0) + (extra != null ? extra.Length : 0)));
                foreach (object[] outs in new object[][] { payload, extra })
                {
                    if (outs != null)
                    {
                        foreach (object o in outs)
                        {
                            if (o is byte)
                            {
                                writer.Write((byte)1);
                                writer.Write((byte)o);
                            }
                            else if (o is bool)
                            {
                                writer.Write((byte)2);
                                writer.Write((bool)o);
                            }
                            else if (o is int)
                            {
                                writer.Write((byte)3);
                                writer.Write((int)o);
                            }
                            else if (o is long)
                            {
                                writer.Write((byte)4);
                                writer.Write((long)o);
                            }
                            else if (o is string)
                            {
                                writer.Write((byte)5);
                                writer.Write((string)o);
                            }
                            else if (o is float)
                            {
                                writer.Write((byte)6);
                                writer.Write((float)o);
                            }
                            else if (o is double)
                            {
                                writer.Write((byte)7);
                                writer.Write((double)o);
                            }
                            else if (o is byte[])
                            {
                                writer.Write((byte)8);
                                writer.Write((int)((byte[])o).Length);
                                writer.Write((byte[])o);
                            }
                            else
                            {
                                throw new NotSupportedException(o.GetType().Name);
                            }
                        }
                    }
                }
            }
            WriteCommandPacket(channel, stream.ToArray());
            channel.WaitForPipeDrain();
        }

        public static void SendMessage(TextWriter log, PipeStream channel, Commands command)
        {
            SendMessage(log, channel, command, null);
        }

        private static string FormatByteArray(byte[] data)
        {
            return String.Join(", ", Array.ConvertAll(data, delegate (byte b) { return String.Concat("0x", b.ToString("X2")); }));
        }

        public static void ReceiveMessage(TextWriter log, PipeStream channel, out Commands command, out object[] payload)
        {
            byte[] package = ReadCommandPacket(channel);
            if (log != null)
            {
                log.WriteLine("  ReceiveMessage: new byte[{0}] {{ {1} }}", package.Length, FormatByteArray(package));
                log.Flush();
            }

            MemoryStream stream = new MemoryStream(package);
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true/*leaveOpen*/))
            {
                command = (Commands)reader.ReadByte();
                if (log != null)
                {
                    log.WriteLine("    Command {0}", command);
                    log.Flush();
                }
                int c = reader.ReadInt32();
                payload = new object[c];
                for (int i = 0; i < payload.Length; i++)
                {
                    switch (reader.ReadByte())
                    {
                        default:
                            throw new ArgumentException();
                        case 1:
                            payload[i] = reader.ReadByte();
                            break;
                        case 2:
                            payload[i] = reader.ReadBoolean();
                            break;
                        case 3:
                            payload[i] = reader.ReadInt32();
                            break;
                        case 4:
                            payload[i] = reader.ReadInt64();
                            break;
                        case 5:
                            payload[i] = reader.ReadString();
                            break;
                        case 6:
                            payload[i] = reader.ReadSingle();
                            break;
                        case 7:
                            payload[i] = reader.ReadDouble();
                            break;
                        case 8:
                            int l = reader.ReadInt32();
                            payload[i] = reader.ReadBytes(l);
                            break;
                    }
                    if (log != null)
                    {
                        if (!(payload[i] is byte[]))
                        {
                            log.WriteLine("    args[{0}] = ({1}){2}", i, payload[i].GetType().Name, payload[i]);
                        }
                        else
                        {
                            log.WriteLine("    args[{0}] = new byte[{1}] {{ {2} }}", i, ((byte[])payload[i]).Length, FormatByteArray((byte[])payload[i]));
                        }
                        log.Flush();
                    }
                }
            }
        }

        public class RemoteException : Exception
        {
            public RemoteException(string message)
                : base(message)
            {
            }
        }

        public class ChannelDisconnectedException : Exception
        {
            public ChannelDisconnectedException(string message)
                : base(message)
            {
            }
        }

        public static void WaitForDone(PipeStream channel, out Profile profile)
        {
            Commands command;
            object[] args;
            ReceiveMessage(null/*log*/, channel, out command, out args);

            switch (command)
            {
                default:
                    throw new NotSupportedException("Unexpected response from server " + command.ToString());

                case Commands.Done:
                    Debug.Assert(args.Length == 1);
                    byte[] d = (byte[])args[0];
                    using (MemoryStream stream = new MemoryStream(d))
                    {
                        profile = (Profile)new BinaryFormatter().Deserialize(stream);
                    }
                    return;

                case Commands.Exception:
                    Debug.Assert(args.Length == 1);
                    string s = (string)args[0];
                    throw new RemoteException(s);
            }
        }

        public static object[] WaitForPending(PipeStream channel, out Profile profile, out string mapId, out int width, out int height)
        {
            Commands command;
            object[] args;
            ReceiveMessage(null/*log*/, channel, out command, out args);

            switch (command)
            {
                default:
                    throw new NotSupportedException("Unexpected response from server " + command.ToString());

                case Commands.Pending:
                    Debug.Assert(args.Length >= 4);
                    mapId = (string)args[0];
                    width = (int)args[1];
                    height = (int)args[2];
                    byte[] d = (byte[])args[3];
                    using (MemoryStream stream = new MemoryStream(d))
                    {
                        profile = (Profile)new BinaryFormatter().Deserialize(stream);
                    }
                    object[] extra = new object[args.Length - 4];
                    Array.Copy(args, 4, extra, 0, extra.Length);
                    return extra;

                case Commands.Exception:
                    Debug.Assert(args.Length == 1);
                    string s = (string)args[0];
                    throw new RemoteException(s);
            }
        }
    }


    //
    // ImageServer
    //

    public static class ImageServer
    {
        private const bool Logging = true; // TODO: turn off

        public static int RunServer(string pipeName)
        {
            TextWriter log;
            if (Logging)
            {
                int i = 0;
                while (true)
                {
                    try
                    {
                        log = new StreamWriter(Path.Combine(Path.GetTempPath(), String.Concat("ImageServerLog-", i, ".log")));
                    }
                    catch (Exception)
                    {
                        i++;
                        continue;
                    }
                    break;
                }
                log = TextWriter.Synchronized(log);
            }

            ManagedBitmap currentBitmap = null;

            try
            {
                ThreadPriority savedThreadPriority = Thread.CurrentThread.Priority;

                if (log != null)
                {
                    log.WriteLine("Connecting to pipe {0}", pipeName);
                    log.Flush();
                }
                using (NamedPipeClientStream channel = new NamedPipeClientStream(".", pipeName))
                {
                    channel.Connect();

                    Thread disconnectMonitorThread = new Thread(new ThreadStart(
                        delegate ()
                        {
                            TimeoutThread(channel);
                            if (log != null)
                            {
                                log.WriteLine("TimeoutThread terminated - connection broken, exiting");
                                log.Flush();
                            }
                        }));
                    disconnectMonitorThread.Start();

                    while (true)
                    {
                        if (log != null)
                        {
                            log.WriteLine("Waiting");
                            log.Flush();
                        }

                        ClientServerCommunication.Commands command;
                        object[] args;
                        ClientServerCommunication.ReceiveMessage(log, channel, out command, out args);

                        object[] extraResults = null;

                        if (log != null)
                        {
                            log.WriteLine("Processing command");
                            log.Flush();
                        }
                        Profile profile;
                        try
                        {
                            profile = new Profile("ImageServer process command");

                            if (log != null)
                            {
                                log.WriteLine("Command: {0}", command);
                                log.Flush();
                            }
                            switch (command)
                            {
                                default:
                                    Debug.Assert(false);
                                    throw new ArgumentException(command.ToString());

                                case ClientServerCommunication.Commands.Quit:
                                    if (args.Length != 0)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    if (log != null)
                                    {
                                        log.WriteLine("Main thread exiting");
                                        log.Flush();
                                        log.Close();
                                    }
                                    return 0;

                                case ClientServerCommunication.Commands.Clear:
                                    if (args.Length != 0)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    if (currentBitmap == null)
                                    {
                                        const string Message = "Current bitmap is null";
                                        Debug.Assert(false, Message);
                                        throw new InvalidOperationException(Message);
                                    }
                                    currentBitmap.Dispose();
                                    currentBitmap = null;
                                    break;

                                case ClientServerCommunication.Commands.LoadAndOrientGDI:
                                    if (args.Length != 3)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    if (currentBitmap != null)
                                    {
                                        const string Message = "Current bitmap hasn't been cleared";
                                        Debug.Assert(false, Message);
                                        throw new InvalidOperationException(Message);
                                    }
                                    Thread.CurrentThread.Priority = (ThreadPriority)args[0];
                                    RotateFlipType exifOrientation;
                                    LoadAndOrientGDI(
                                        profile,
                                        (string)args[1],
                                        (int)args[2],
                                        out currentBitmap,
                                        out exifOrientation);
                                    extraResults = new object[] { (int)exifOrientation };
                                    break;

                                case ClientServerCommunication.Commands.ResizeGDI:
                                    if (args.Length != 6)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    if (currentBitmap != null)
                                    {
                                        const string Message = "Current bitmap hasn't been cleared";
                                        Debug.Assert(false, Message);
                                        throw new InvalidOperationException(Message);
                                    }
                                    Thread.CurrentThread.Priority = (ThreadPriority)args[0];
                                    ResizeGDI(
                                        profile,
                                        (string)args[1],
                                        (int)args[2],
                                        (int)args[3],
                                        (int)args[4],
                                        (int)args[5],
                                        out currentBitmap);
                                    break;

                                case ClientServerCommunication.Commands.CreateTileGDI:
                                    if (args.Length != 10)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    if (currentBitmap != null)
                                    {
                                        const string Message = "Current bitmap hasn't been cleared";
                                        Debug.Assert(false, Message);
                                        throw new InvalidOperationException(Message);
                                    }
                                    Thread.CurrentThread.Priority = (ThreadPriority)args[0];
                                    CreateTileGDI(
                                        profile,
                                        (string)args[1],
                                        (int)args[2],
                                        (int)args[3],
                                        (int)args[4],
                                        (int)args[5],
                                        (int)args[6],
                                        (int)args[7],
                                        (int)args[8],
                                        (int)args[9],
                                        out currentBitmap);
                                    break;

                                case ClientServerCommunication.Commands.ShrinkExpandGDI:
                                    if (args.Length != 6)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    Thread.CurrentThread.Priority = (ThreadPriority)args[0];
                                    ShrinkExpandGDI(
                                        profile,
                                        (int)args[1],
                                        (int)args[2],
                                        (string)args[3],
                                        (string)args[4],
                                        (double)args[5]);
                                    break;

                                case ClientServerCommunication.Commands.ShrinkExpandWPF:
                                    if (args.Length != 6)
                                    {
                                        const string Message = "remote command: wrong number of arguments";
                                        Debug.Assert(false, Message);
                                        throw new ArgumentException(Message);
                                    }
                                    Thread.CurrentThread.Priority = (ThreadPriority)args[0];
                                    ShrinkExpandWPF(
                                        profile,
                                        (int)args[1],
                                        (int)args[2],
                                        (string)args[3],
                                        (string)args[4],
                                        (double)args[5]);
                                    break;
                            }

                            profile.End();
                        }
                        catch (Exception exception)
                        {
                            if (log != null)
                            {
                                log.WriteLine("Recoverable exception: {0}", exception);
                                log.Flush();
                            }
                            ClientServerCommunication.SendMessage(
                                log,
                                channel,
                                ClientServerCommunication.Commands.Exception,
                                new object[] { exception.ToString() });
                            if (log != null)
                            {
                                log.WriteLine("Exception response sent");
                                log.Flush();
                            }
                            continue;
                        }
                        finally
                        {
                            Thread.CurrentThread.Priority = savedThreadPriority;
                        }

                        if (log != null)
                        {
                            log.WriteLine("Forming response");
                            log.Flush();
                        }
                        using (MemoryStream serializedProfileStream = new MemoryStream())
                        {
                            if (log != null)
                            {
                                log.WriteLine("Serializing profile");
                                log.Flush();
                            }
                            new BinaryFormatter().Serialize(serializedProfileStream, profile);

                            if (currentBitmap == null)
                            {
                                if (log != null)
                                {
                                    log.WriteLine("Sending 'Done'");
                                    log.Flush();
                                }
                                ClientServerCommunication.SendMessage(
                                    log,
                                    channel,
                                    ClientServerCommunication.Commands.Done,
                                    new object[] { serializedProfileStream.ToArray() });
                            }
                            else
                            {
                                if (log != null)
                                {
                                    log.WriteLine("Sending 'Pending'");
                                    log.Flush();
                                }
                                ClientServerCommunication.SendMessage(
                                    log,
                                    channel,
                                    ClientServerCommunication.Commands.Pending,
                                    new object[]
                                    {
                                        (string)currentBitmap.BackingName,
                                        (int)currentBitmap.Width,
                                        (int)currentBitmap.Height,
                                        (byte[])serializedProfileStream.ToArray()
                                    },
                                    extraResults);
                            }
                        }
                        if (log != null)
                        {
                            log.WriteLine("Sent");
                            log.Flush();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (log != null)
                {
                    log.WriteLine("Unrecoverable exception: {0}", exception);
                    log.Flush();
                }
                if (currentBitmap != null)
                {
                    currentBitmap.Dispose();
                }
                if (!(exception is ClientServerCommunication.ChannelDisconnectedException))
                {
                    MessageBox.Show(exception.ToString());
                }
                if (log != null)
                {
                    log.WriteLine("Terminating abnormally");
                    log.Flush();
                    log.Close();
                }
                return 1;
            }

#pragma warning disable CS0162 // unreachable
            if (log != null)
            {
                log.WriteLine("Terminating normally");
                log.Flush();
                log.Close();
            }
#pragma warning restore CS0162
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
            return 0;
        }

        private static void TimeoutThread(NamedPipeClientStream channel)
        {
            while (channel.IsConnected)
            {
                Thread.Sleep(1000);
            }
        }

        private static void LoadAndOrientGDI(Profile profile, string sourceFilePath, int rightRotations, out ManagedBitmap bitmap, out RotateFlipType exifOrientation)
        {
            profile.Push("LoadAndOrient [GDI]");

            PropertyItem[] properties;
            bitmap = Transforms.LoadAndOrientManaged(profile, sourceFilePath, rightRotations, out properties, out exifOrientation);

            profile.Pop();
        }

        private static void ResizeGDI(Profile profile, string sourceMapId, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight, out ManagedBitmap targetBitmap)
        {
            profile.Push("Resize [GDI]");

            using (ManagedBitmap source = new ManagedBitmap32(sourceWidth, sourceHeight, sourceMapId))
            {
                profile.Push("Allocate");
                targetBitmap = new ManagedBitmap32(targetWidth, targetHeight);
                profile.Pop();

                Transforms.Resize(profile, source, targetBitmap);
            }

            profile.Pop();
        }

        // TODO: untested
        private unsafe static void CreateTileGDI(Profile profile, string sourceMapId, int sourceWidth, int sourceHeight, int offsetX, int offsetY, int drawWidth, int drawHeight, int targetWidth, int targetHeight, out ManagedBitmap targetBitmap)
        {
            profile.Push("CreateTileGDI");

            using (ManagedBitmap sourceBitmap = new ManagedBitmap32(sourceWidth, sourceHeight, sourceMapId))
            {
                using (Bitmap sourceBitmapGDI = new Bitmap(sourceWidth, sourceHeight, sourceBitmap.Stride, sourceBitmap.ImageFormat, new IntPtr(sourceBitmap.Scan0)))
                {
                    targetBitmap = new ManagedBitmap32(targetWidth, targetHeight);
                    using (Bitmap targetBitmapGDI = new Bitmap(targetWidth, targetHeight, targetBitmap.Stride, targetBitmap.ImageFormat, new IntPtr(targetBitmap.Scan0)))
                    {
                        using (Graphics graphics = Graphics.FromImage(targetBitmapGDI))
                        {
                            graphics.FillRectangle(System.Drawing.Brushes.Black, 0, 0, targetWidth, targetHeight);
                            graphics.DrawImage(sourceBitmapGDI, offsetX, offsetY, drawWidth, drawHeight);
                        }
                    }
                }
            }

            profile.Pop();
        }

        private static void ShrinkExpandGDI(Profile profile, int width, int height, string sourceMapName, string targetMapName, double factor)
        {
            profile.Push("ShrinkExpand [GDI]");

            using (ManagedBitmap source = new ManagedBitmap32(width, height, sourceMapName))
            {
                using (ManagedBitmap target = new ManagedBitmap32(width, height, targetMapName))
                {
                    Transforms.ShrinkExpand(profile, source, factor, target);
                }
            }

            profile.Pop();
        }

        private static void ShrinkExpandWPF(Profile profile, int width, int height, string sourceMapName, string targetMapName, double factor)
        {
            profile.Push("ShrinkExpand [WPF]");

            using (SmartBitmap source = new SmartBitmap(new ManagedBitmap32(width, height, sourceMapName)))
            {
                profile.Push("Convert to WPF");
                BitmapFrame wpfSource = source.AsWPF();
                profile.Pop();

                BitmapFrame wpfShrunkExpanded = Transforms.ShrinkExpand(profile, wpfSource, factor);

                profile.Push("Convert to Managed");
                using (ManagedBitmap target = new ManagedBitmap32(wpfShrunkExpanded, targetMapName)) // copy into shared memory
                {
                }
                profile.Pop();
            }

            profile.Pop();
        }
    }


    //
    // ImageServerProxy
    //

    public class ImageServerProxy : IDisposable
    {
        private bool started;
        private NamedPipeServerStream channel;
        private Process remote;
        // for debugging
        private const bool InProc = false;
        private Thread simulatedRemote;

        public ImageServerProxy()
        {
        }

        private void EnsureStarted(Profile profile)
        {
            if (!started)
            {
                profile.Push("EnsureStarted");

                string pipeName = Guid.NewGuid().ToString();

                // TODO: security - restrict access to child process

                channel = new NamedPipeServerStream(pipeName);

                if (!InProc)
                {
                    remote = new Process();
                    remote.StartInfo.Arguments = String.Format("-server {0}", pipeName);
                    remote.StartInfo.CreateNoWindow = true;
                    remote.StartInfo.FileName = Assembly.GetExecutingAssembly().Location;
                    remote.StartInfo.UseShellExecute = false;
                    remote.StartInfo.WorkingDirectory = Path.GetTempPath();
                    remote.Start();
                }
                else
                {
#pragma warning disable CS0162 // unreachable
                    simulatedRemote = new Thread(new ThreadStart(
                        delegate ()
                        {
                            ImageServer.RunServer(pipeName);
                        }));
                    simulatedRemote.Name = "ImageServer - InProc";
                    simulatedRemote.Start();
#pragma warning restore CS0162
                }

                channel.WaitForConnection();

                started = true;

                profile.Pop();
            }
        }

        public void Close()
        {
            if (started)
            {
                ClientServerCommunication.SendMessage(null/*log*/, channel, ClientServerCommunication.Commands.Quit);
                if (remote != null)
                {
                    const int Timeout = 500;
                    remote.WaitForExit(Timeout); // A modest amount of decorum around shutdown, but don't waste time waiting too long
                    remote.Dispose();
                    remote = null;
                }
                if (simulatedRemote != null)
                {
                    simulatedRemote = null;
                }

                channel.Close();
                channel = null;

                started = false;
            }
        }

        public void Dispose()
        {
            Close();
        }


        public ManagedBitmap LoadAndOrientGDI(string path, int rightRotations, Profile profile, out RotateFlipType exifOrientation)
        {
            EnsureStarted(profile);

            exifOrientation = RotateFlipType.RotateNoneFlipNone;

            ManagedBitmap result;
            string mapId;
            int width, height;
            Profile serverProfile;
            bool succeeded = false;

            profile.Push("Remote call");
            ClientServerCommunication.SendMessage(
                null/*log*/,
                channel,
                ClientServerCommunication.Commands.LoadAndOrientGDI,
                new object[]
                {
                    (int)Thread.CurrentThread.Priority,
                    (string)path,
                    (int)rightRotations,
                });
            try
            {
                object[] extra = ClientServerCommunication.WaitForPending(channel, out serverProfile, out mapId, out width, out height);
                exifOrientation = (RotateFlipType)(int)extra[0];
                profile.Add(serverProfile);
                result = new ManagedBitmap32(width, height, mapId);
                succeeded = true;
            }
            catch (ClientServerCommunication.RemoteException)
            {
                result = ManagedBitmap.CreateFromGDI(new Bitmap(Properties.Resources.InvalidPlaceHolder));
            }
            profile.Pop();

            if (succeeded)
            {
                profile.Push("Remote call");
                ClientServerCommunication.SendMessage(
                    null/*log*/,
                    channel,
                    ClientServerCommunication.Commands.Clear,
                    null);
                ClientServerCommunication.WaitForDone(channel, out serverProfile);
                profile.Add(serverProfile);
                profile.Pop();
            }

            return result;
        }

        public ManagedBitmap ResizeGDI(ManagedBitmap source, int targetWidth, int targetHeight, Profile profile)
        {
            EnsureStarted(profile);

            ManagedBitmap result;
            string mapId;
            int width, height;
            Profile serverProfile;
            bool succeeded = false;

            profile.Push("Remote call");
            ClientServerCommunication.SendMessage(
                null/*log*/,
                channel,
                ClientServerCommunication.Commands.ResizeGDI,
                new object[]
                {
                    (int)Thread.CurrentThread.Priority,
                    (string)source.BackingName,
                    (int)source.Width,
                    (int)source.Height,
                    (int)targetWidth,
                    (int)targetHeight
                });
            try
            {
                ClientServerCommunication.WaitForPending(channel, out serverProfile, out mapId, out width, out height);
                profile.Add(serverProfile);
                result = new ManagedBitmap32(width, height, mapId);
                succeeded = true;
            }
            catch (ClientServerCommunication.RemoteException)
            {
                result = ManagedBitmap.CreateFromGDI(new Bitmap(Properties.Resources.InvalidPlaceHolder));
            }
            profile.Pop();

            if (succeeded)
            {
                profile.Push("Remote call");
                ClientServerCommunication.SendMessage(
                    null/*log*/,
                    channel,
                    ClientServerCommunication.Commands.Clear,
                    null);
                ClientServerCommunication.WaitForDone(channel, out serverProfile);
                profile.Add(serverProfile);
                profile.Pop();
            }

            return result;
        }

        public ManagedBitmap CreateTile(ManagedBitmap source, int offsetX, int offsetY, int drawWidth, int drawHeight, int targetWidth, int targetHeight, Profile profile)
        {
            EnsureStarted(profile);

            ManagedBitmap result;
            string mapId;
            int width, height;
            Profile serverProfile;
            bool succeeded = false;

            profile.Push("Remote call");
            ClientServerCommunication.SendMessage(
                null/*log*/,
                channel,
                ClientServerCommunication.Commands.CreateTileGDI,
                new object[]
                {
                    (int)Thread.CurrentThread.Priority,
                    (string)source.BackingName,
                    (int)source.Width,
                    (int)source.Height,
                    (int)offsetX,
                    (int)offsetY,
                    (int)drawWidth,
                    (int)drawHeight,
                    (int)targetWidth,
                    (int)targetHeight
                });
            try
            {
                ClientServerCommunication.WaitForPending(channel, out serverProfile, out mapId, out width, out height);
                profile.Add(serverProfile);
                result = new ManagedBitmap32(width, height, mapId);
                succeeded = true;
            }
            catch (ClientServerCommunication.RemoteException)
            {
                result = ManagedBitmap.CreateFromGDI(new Bitmap(Properties.Resources.InvalidPlaceHolder));
            }
            profile.Pop();

            if (succeeded)
            {
                profile.Push("Remote call");
                ClientServerCommunication.SendMessage(
                    null/*log*/,
                    channel,
                    ClientServerCommunication.Commands.Clear,
                    null);
                ClientServerCommunication.WaitForDone(channel, out serverProfile);
                profile.Add(serverProfile);
                profile.Pop();
            }

            return result;
        }

        public ManagedBitmap ShrinkExpandGDI(ManagedBitmap source, double factor, Profile profile)
        {
            EnsureStarted(profile);

            ManagedBitmap result = new ManagedBitmap32(source.Width, source.Height);

            profile.Push("Remote call");
            ClientServerCommunication.SendMessage(
                null/*log*/,
                channel,
                ClientServerCommunication.Commands.ShrinkExpandGDI,
                new object[]
                {
                    (int)Thread.CurrentThread.Priority,
                    (int)source.Width,
                    (int)source.Height,
                    (string)source.BackingName,
                    (string)result.BackingName,
                    (double)factor,
                });
            Profile serverProfile;
            ClientServerCommunication.WaitForDone(channel, out serverProfile);
            profile.Add(serverProfile);
            profile.Pop();

            return result;
        }

        public ManagedBitmap ShrinkExpandWPF(ManagedBitmap source, double factor, Profile profile)
        {
            EnsureStarted(profile);

            ManagedBitmap result = new ManagedBitmap32(source.Width, source.Height);

            profile.Push("Remote call");
            ClientServerCommunication.SendMessage(
                null/*log*/,
                channel,
                ClientServerCommunication.Commands.ShrinkExpandWPF,
                new object[]
                {
                    (int)Thread.CurrentThread.Priority,
                    (int)source.Width,
                    (int)source.Height,
                    (string)source.BackingName,
                    (string)result.BackingName,
                    (double)factor,
                });
            Profile serverProfile;
            ClientServerCommunication.WaitForDone(channel, out serverProfile);
            profile.Add(serverProfile);
            profile.Pop();

            return result;
        }
    }


    //
    // ImageServerPool
    //

    public class ImageServerPool : IDisposable
    {
        private readonly ImageServerProxy[] proxies;
        private readonly EventWaitHandle[] proxyGates;

        public ImageServerPool()
        {
            int count = !Program.ProfileMode ? Environment.ProcessorCount : 1;
            proxies = new ImageServerProxy[count];
            proxyGates = new EventWaitHandle[count];
            for (int i = 0; i < count; i++)
            {
                proxies[i] = new ImageServerProxy();
                proxyGates[i] = new EventWaitHandle(true/*initially available*/, EventResetMode.AutoReset);
            }
        }

        public ImageServerProxy ObtainProxy(Profile profile)
        {
            profile.Push("ObtainProxy");
            int which = WaitHandle.WaitAny(proxyGates);
            ImageServerProxy result = proxies[which];
            profile.Pop();
            return result;
        }

        public void ReleaseProxy(ImageServerProxy proxy)
        {
            int which = Array.IndexOf(proxies, proxy);
            proxyGates[which].Set();
        }

        public void Dispose()
        {
            Task<bool>[] disposes = new Task<bool>[proxies.Length];
            for (int i = 0; i < proxies.Length; i++)
            {
                ImageServerProxy proxy = proxies[i];
                proxies[i] = null;
                EventWaitHandle proxyGate = proxyGates[i];
                proxyGates[i] = null;

                disposes[i] = new Task<bool>(
                    delegate ()
                    {
                        proxy.Dispose();
                        proxyGate.Dispose();
                        return false;
                    });
                disposes[i].Start();
            }
            Task.WaitAll(disposes);
        }
    }


    //
    // ImageClient
    //

    public static class ImageClient
    {
        private readonly static ImageServerPool serverPool = new ImageServerPool();


        public static void Close()
        {
            serverPool.Dispose();
        }


        public static ManagedBitmap LoadAndOrientGDI(Profile profile, string path, int rightRotations, out RotateFlipType exifOrientation)
        {
            profile.Push("ImageClient.LoadAndOrientGDI");
            ImageServerProxy server = serverPool.ObtainProxy(profile);
            try
            {
                ManagedBitmap result = server.LoadAndOrientGDI(path, rightRotations, profile, out exifOrientation);
                return result;
            }
            finally
            {
                serverPool.ReleaseProxy(server);
                profile.Pop();
            }
        }

        public static ManagedBitmap ResizeGDI(Profile profile, ManagedBitmap source, int targetWidth, int targetHeight)
        {
            profile.Push("ImageClient.ResizeGDI");
            ImageServerProxy server = serverPool.ObtainProxy(profile);
            try
            {
                ManagedBitmap result = server.ResizeGDI(source, targetWidth, targetHeight, profile);
                return result;
            }
            finally
            {
                serverPool.ReleaseProxy(server);
                profile.Pop();
            }
        }

        public static ManagedBitmap CreateTileGDI(Profile profile, ManagedBitmap source, int offsetX, int offsetY, int drawWidth, int drawHeight, int targetWidth, int targetHeight)
        {
            profile.Push("ImageClient.CreateTileGDI");
            ImageServerProxy server = serverPool.ObtainProxy(profile);
            try
            {
                ManagedBitmap result = server.CreateTile(source, offsetX, offsetY, drawWidth, drawHeight, targetWidth, targetHeight, profile);
                return result;
            }
            finally
            {
                serverPool.ReleaseProxy(server);
                profile.Pop();
            }
        }

        public static ManagedBitmap ShrinkExpandGDI(Profile profile, ManagedBitmap source, double factor)
        {
            profile.Push("ImageClient.ShrinkExpandGDI");
            ImageServerProxy server = serverPool.ObtainProxy(profile);
            try
            {
                ManagedBitmap result = server.ShrinkExpandGDI(source, factor, profile);
                return result;
            }
            finally
            {
                serverPool.ReleaseProxy(server);
                profile.Pop();
            }
        }

        public static ManagedBitmap ShrinkExpandWPF(Profile profile, ManagedBitmap source, double factor)
        {
            profile.Push("ImageClient.ShrinkExpandWPF");
            ImageServerProxy server = serverPool.ObtainProxy(profile);
            try
            {
                ManagedBitmap result = server.ShrinkExpandWPF(source, factor, profile);
                return result;
            }
            finally
            {
                serverPool.ReleaseProxy(server);
                profile.Pop();
            }
        }
    }
}
