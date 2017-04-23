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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace AdaptiveImageSizeReducer
{
    // TODO: swap to disk vs. discard & regenerate based on tracking regeneration cost and doing what's faster

    // TODO: swap performance is disappointing all around - may not be worth having at all


    // Lock ordering: acquire ImageCache lock, before Entry lock, before SerializationManager lock


    //
    // Entry
    //

    public class Entry : IDisposable
    {
        private readonly string id;
        public Task<ManagedBitmap> bitmap;
        private readonly List<KeyValuePair<int, BitmapHolder>> holders = new List<KeyValuePair<int, BitmapHolder>>();
        private string swapFilePath;
        private Stream swapFileStream;
        private Task<bool> outTask;
        private readonly List<ManagedBitmap> oldBitmaps = new List<ManagedBitmap>();

        public delegate void BitmapCompletedDelegate(object sender, EventArgs args);
        public event BitmapCompletedDelegate BitmapCompleted;

        public Entry(string path, Task<ManagedBitmap> bitmap)
        {
            this.id = path;
            this.bitmap = bitmap;
            StartBitmapCompleteWaiter();
        }

        private void StartBitmapCompleteWaiter()
        {
            Task<bool> onCompleted = new Task<bool>(
                delegate ()
                {
                    BitmapCompletedDelegate localBitmapCompleted = this.BitmapCompleted;
                    Task<ManagedBitmap> bitmap = this.bitmap;

                    if (bitmap == null) // object disposed before task started
                    {
                        return false;
                    }

                    bitmap.Wait();

                    if (localBitmapCompleted != null)
                    {
                        localBitmapCompleted.Invoke(this, EventArgs.Empty);
                    }

                    return false;
                });
            onCompleted.Start();
        }

        public string Id { get { return this.id; } }

        public KeyValuePair<int, BitmapHolder>[] Holders
        {
            get
            {
                lock (this)
                {
                    return this.holders.ToArray();
                }
            }
        }

        public int HoldersCount
        {
            get
            {
                lock (this)
                {
                    return this.holders.Count;
                }
            }
        }

        public void EnsureBitmapTaskStarted()
        {
            lock (this)
            {
                Debug.Assert(this.holders.Count != 0);
                if (this.bitmap.Status == TaskStatus.Created)
                {
                    try
                    {
                        this.bitmap.Start();
                    }
                    catch (InvalidOperationException)
                    {
                        // race condition: other request may have started it between our checking and our starting
                    }
                }
            }
        }

        public ManagedBitmap GetBitmap(CancellationToken cancel)
        {
            Task<ManagedBitmap> bitmap;
            lock (this)
            {
                EnsureBitmapTaskStarted();
                bitmap = this.bitmap;
            }
            bitmap.Wait(cancel); // throws OperationCanceledException; may take CancellationToken.None
            return bitmap.Result;
        }

        public void AttachHolder(KeyValuePair<int, BitmapHolder> holderEntry)
        {
            lock (this)
            {
                this.holders.Add(holderEntry);
            }
        }

        public void DetachHolder(int key)
        {
            lock (this)
            {
                int i = this.holders.FindIndex(delegate (KeyValuePair<int, BitmapHolder> candidate) { return candidate.Key == key; });
                Debug.Assert(i >= 0);
                this.holders.RemoveAt(i);
            }
        }

        private enum FILE_INFO_BY_HANDLE_CLASS
        {
            FileBasicInfo = 0,
            FileStandardInfo = 1,
            FileNameInfo = 2,
            FileRenameInfo = 3,
            FileDispositionInfo = 4,
            FileAllocationInfo = 5,
            FileEndOfFileInfo = 6,
            FileStreamInfo = 7,
            FileCompressionInfo = 8,
            FileAttributeTagInfo = 9,
            FileIdBothDirectoryInfo = 10, // 0xA
            FileIdBothDirectoryRestartInfo = 11, // 0xB
            FileIoPriorityHintInfo = 12, // 0xC
            FileRemoteProtocolInfo = 13, // 0xD
            FileFullDirectoryInfo = 14, // 0xE
            FileFullDirectoryRestartInfo = 15, // 0xF
            FileStorageInfo = 16, // 0x10
            FileAlignmentInfo = 17, // 0x11
            FileIdInfo = 18, // 0x12
            FileIdExtdDirectoryInfo = 19, // 0x13
            FileIdExtdDirectoryRestartInfo = 20, // 0x14
            MaximumFileInfoByHandlesClass
        };

        private enum PRIORITY_HINT
        {
            IoPriorityHintVeryLow = 0,
            IoPriorityHintLow,
            IoPriorityHintNormal,
            MaximumIoPriorityHintType
        };

        private struct FILE_IO_PRIORITY_HINT_INFO
        {
            public PRIORITY_HINT PriorityHint;
        }

        [DllImport("Kernel32.dll")]
        private static extern bool SetFileInformationByHandle(
            IntPtr hFile,
            FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
            [MarshalAs(UnmanagedType.Struct)] ref FILE_IO_PRIORITY_HINT_INFO lpFileInformation,
            int dwBufferSize);

        private static void SetIOPriority(Stream stream, PRIORITY_HINT priority)
        {
            FileStream fileStream = (FileStream)stream;
            FieldInfo field = fileStream.GetType().GetField("_handle", BindingFlags.Instance | BindingFlags.NonPublic);
            SafeHandle fileHandle = (SafeHandle)field.GetValue(fileStream);

            FILE_IO_PRIORITY_HINT_INFO fi = new FILE_IO_PRIORITY_HINT_INFO();
            fi.PriorityHint = priority;

            bool reffed = false;
            bool f = false;
            int hrForLastWin32Error = 0;
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    fileHandle.DangerousAddRef(ref reffed);
                }

                f = SetFileInformationByHandle(fileHandle.DangerousGetHandle(), FILE_INFO_BY_HANDLE_CLASS.FileIoPriorityHintInfo, ref fi, Marshal.SizeOf(fi));
                hrForLastWin32Error = Marshal.GetHRForLastWin32Error();
            }
            finally
            {
                if (reffed)
                {
                    fileHandle.DangerousRelease();
                }
            }

            if (!f)
            {
                Marshal.ThrowExceptionForHR(hrForLastWin32Error);
            }
        }

        public void StartSwapOut(ImageCache cache)
        {
            lock (this)
            {
                Debug.Assert(Program.EnableSwap);

                if (this.outTask != null)
                {
                    return;
                }
                if (this.holders.Count != 0)
                {
                    return;
                }

                Stopwatch elapsed = Stopwatch.StartNew();

#if true // TODO: remove hack
                EventWaitHandle swapOutDelay = new EventWaitHandle(false, EventResetMode.AutoReset);
#endif
                Task<bool> serializationTask = null;

                Task<ManagedBitmap> oldBitmap = this.bitmap;

                // 'this' is not locked at the time 'cache' is used below in the task delegates

                this.outTask = new Task<bool>(
                    delegate ()
                    {
                        Profile profile = new Profile("SwapOut {0}", this.id);

#if true // TODO: remove hack
                        profile.Push("Hack delay for swapins");
                        // HACK: wait a little to allow swapins to start before swapouts
                        EventWaitHandle localSwapOutDelay = Interlocked.Exchange(ref swapOutDelay, null);
                        if (localSwapOutDelay != null) // race: swapin can grab and clear this before we get here
                        {
                            localSwapOutDelay.WaitOne(100);
                        }
                        profile.Pop();
                        //
#endif
                        profile.Push("WaitSwapOutGate");
                        cache.WaitSwapOutGate(); // defer to in-flight swapins
                        profile.Pop();

                        lock (this)
                        {
                            Debug.Assert(this.swapFilePath == null);
                            Debug.Assert(this.swapFileStream == null);
                            this.swapFilePath = Path.GetTempFileName();
                            this.swapFileStream = new FileStream(this.swapFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
                        }

                        profile.Push("oldBitmap.Wait()");
                        oldBitmap.Wait(); // wait for an old in-flight creation to complete
                        profile.Pop();

                        profile.Push("Serialize");
                        serializationTask = new Task<bool>(
                            delegate ()
                            {
                                SetIOPriority(this.swapFileStream, PRIORITY_HINT.IoPriorityHintLow);
                                ManagedBitmap.Serialize(this.swapFileStream, oldBitmap.Result);
                                SetIOPriority(this.swapFileStream, PRIORITY_HINT.IoPriorityHintNormal);
                                return false;
                            });
                        SerializationManager.Manager.EnqueueWriteTask(serializationTask);
                        serializationTask.Wait();
                        profile.Pop();

                        profile.Push("Enqueue oldBitmap");
                        lock (this)
                        {
                            this.oldBitmaps.Add(oldBitmap.Result); // remove current bitmap and enqueue for destruction upon zero refs
                        }
                        profile.Pop();

                        profile.Push("Epilog");
                        cache.Trace("swapout", this, elapsed);
                        cache.PurgeDisposeList();
                        profile.Pop();

                        profile.End();
                        //Program.Log(LogCat.Perf, profile.ToString());

                        return false;
                    });

                this.bitmap = new Task<ManagedBitmap>(
                    delegate ()
                    {
                        Profile profile = new Profile("SwapIn {0}", this.id);

                        profile.Push("outTask.Wait()");
#if true // TODO: remove hack
                        // HACK: release delay immediately if swapin is requested
                        EventWaitHandle localSwapOutDelay = Interlocked.Exchange(ref swapOutDelay, null);
                        if (localSwapOutDelay != null)
                        {
                            localSwapOutDelay.Set();
                        }
                        //
#endif
                        SerializationManager.Manager.Prioritize(serializationTask);
                        Debug.Assert(this.outTask != null);
                        this.outTask.Wait(); // ensure in-progress swapout finishes
                        profile.Pop();

                        profile.Push("cache.BeginSwapIn()");
                        cache.BeginSwapIn();
                        profile.Pop();
                        try
                        {
                            Stopwatch elapsed2 = Stopwatch.StartNew();

                            Debug.Assert(this.swapFilePath != null);
                            Debug.Assert(this.swapFileStream != null);

                            profile.Push("Deserialize");
                            ManagedBitmap bitmap = null;
                            Task<bool> deserializationTask = new Task<bool>(
                                delegate ()
                                {
                                    bitmap = ManagedBitmap.Deserialize(this.swapFileStream);
                                    return false;
                                });
                            SerializationManager.Manager.EnqueueReadTask(deserializationTask);
                            deserializationTask.Wait();
                            this.swapFilePath = null;
                            Stream localSwapFileStream = this.swapFileStream;
                            this.swapFileStream = null;
                            localSwapFileStream.Dispose();
                            this.outTask = null;
                            profile.Pop();

                            profile.Push("Epilog");
                            cache.Trace("swapin", this, elapsed2);
                            cache.PurgeDisposeList();

                            StartBitmapCompleteWaiter();

                            return bitmap;
                        }
                        finally
                        {
                            cache.EndSwapIn();
#if true // TODO: remove hack
                            if (localSwapOutDelay != null)
                            {
                                localSwapOutDelay.Dispose();
                            }
#endif
                            profile.Pop(); // Epilog - here to include cache.EndSwapIn()

                            profile.End();
                            //Program.Log(LogCat.Perf, profile.ToString());
                        }
                    });

                this.outTask.Start();
            }
        }

        public bool SwappedOut { get { return this.outTask != null; } }

        public bool PurgeOldBitmaps()
        {
            ManagedBitmap[] oldBitmaps;
            lock (this)
            {
                if (this.holders.Count != 0)
                {
                    return false;
                }
                oldBitmaps = this.oldBitmaps.ToArray();
                this.oldBitmaps.Clear();
            }
            foreach (ManagedBitmap bitmap in oldBitmaps)
            {
                bitmap.Dispose();
            }
            return oldBitmaps.Length != 0;
        }

        public int OldBitmapsCount
        {
            get
            {
                lock (this)
                {
                    return this.oldBitmaps.Count;
                }
            }
        }

        public long TotalBytes
        {
            get
            {
                long total = 0;
                lock (this)
                {
                    if (this.bitmap.IsCompleted)
                    {
                        total += this.bitmap.Result.TotalBytes;
                    }
                    for (int i = 0; i < this.oldBitmaps.Count; i++)
                    {
                        total += this.oldBitmaps[i].TotalBytes;
                    }
                }
                return total;
            }
        }

        public long TotalSwappedBytes
        {
            get
            {
                long total = 0;
                lock (this)
                {
                    if (this.swapFileStream != null)
                    {
                        total += this.swapFileStream.Length;
                    }
                }
                return total;
            }
        }

        public bool CanDispose
        {
            get
            {
                lock (this)
                {
                    return ((this.bitmap.Status == TaskStatus.Created) || this.bitmap.IsCompleted)
                        && (this.holders.Count == 0)
                        && ((this.outTask == null) || this.outTask.IsCompleted);
                }
            }
        }

        public bool TryDispose()
        {
            bool canDispose;
            lock (this)
            {
                canDispose = this.CanDispose;
            }
            if (canDispose)
            {
                this.Dispose();
            }
            return canDispose;
        }

        public void Dispose()
        {
            Debug.Assert(this.CanDispose);

            if (this.outTask != null)
            {
                this.outTask.Wait();
                this.outTask = null;
            }

            Task<ManagedBitmap> bitmap;
            lock (this)
            {
                bitmap = this.bitmap;
                if (bitmap.Status == TaskStatus.Created)
                {
                    bitmap = null;
                }
                this.bitmap = null;
            }
            if (bitmap != null)
            {
                bitmap.Wait();
                bitmap.Result.Dispose();
            }

            Debug.Assert((this.swapFilePath != null) == (this.swapFileStream != null));
            if (this.swapFilePath != null)
            {
                this.swapFilePath = null;
            }
            if (this.swapFileStream != null)
            {
                Stream localSwapFileStream = this.swapFileStream;
                this.swapFileStream = null;
                localSwapFileStream.Dispose();
            }

            PurgeOldBitmaps();

            GC.SuppressFinalize(this);
        }

        ~Entry()
        {
#if DEBUG
            Debug.Assert(false, "Entry: should have been Dispose()ed? " + allocatedFrom.ToString());
#endif
            Dispose();
        }
#if DEBUG
        private readonly StackTrace allocatedFrom = new StackTrace(1, true/*fNeedFileInfo*/);
#endif
    }


    //
    // BitmapHolder
    //

    public class BitmapHolder : IDisposable
    {
        private readonly ImageCache cache;
        private readonly Entry entry;
        private readonly StackTrace stack;
        private readonly int id;

        private static int idGen;

        internal BitmapHolder(ImageCache cache, Entry entry)
        {
            this.id = Interlocked.Increment(ref idGen);
            entry.AttachHolder(new KeyValuePair<int, BitmapHolder>(this.id, this));

            this.cache = cache;
            this.entry = entry;
            this.stack = new StackTrace(1, true);
        }

        public ManagedBitmap Bitmap
        {
            get
            {
                ManagedBitmap bitmap = this.entry.GetBitmap(CancellationToken.None);
                this.cache.Touch(this.entry);
                return bitmap;
            }
        }

        public void Wait()
        {
            this.entry.GetBitmap(CancellationToken.None);
        }

        public void Wait(CancellationToken cancel)
        {
            try
            {
                this.entry.GetBitmap(cancel);
            }
            catch (OperationCanceledException)
            {
                // client checks bitmap.IsCompleted property
            }
        }

        public bool IsCompleted { get { return entry.bitmap.IsCompleted; } }

        public StackTrace Stack { get { return stack; } }

        public BitmapHolder Clone()
        {
            return new BitmapHolder(this.cache, entry);
        }

        public void Dispose()
        {
            this.entry.DetachHolder(this.id);
            GC.SuppressFinalize(this);
        }

        ~BitmapHolder()
        {
#if DEBUG
            Debug.Assert(false, "BitmapHolder: Did you forget to Dispose()? " + allocatedFrom.ToString());
#endif
            Dispose();
        }
#if DEBUG
        private readonly StackTrace allocatedFrom = new StackTrace(1, true/*fNeedFileInfo*/);
#endif
    }


    //
    // ImageCache
    //

    public class ImageCache : IDisposable
    {
        private const bool EnableTrace = true;
        private const int MinInMemoryCount = 10;

        private readonly List<Entry> entries = new List<Entry>();
        private readonly List<Entry> disposeList = new List<Entry>();
        private uint hits;
        private int inFlightSwapIns;
        private readonly EventWaitHandle swapOutGate;

        private readonly static long SystemMemory = (long)new ComputerInfo().TotalPhysicalMemory;
        private const long ReservedMemory = 3072L * 1024 * 1024;
        private const long ReservedMemorySmall = 2560L * 1024 * 1024;
        private const long SmallMemoryCutoff = 4096L * 1024 * 1024;
        private const long MinimumMemory = 1024L * 1024 * 1024;
        public readonly static long MemoryLimit = Math.Max(MinimumMemory, SystemMemory - (SystemMemory > SmallMemoryCutoff ? ReservedMemory : ReservedMemorySmall));
        private const int OverheadFactor = 110, OverheadDenominator = 100;

        private const long MaxDiskUsage = 10L * 1024 * 1024 * 1024;
        private const long MinDiskAvailabilitty = 1L * 1024 * 1024 * 1024;
        private const int DiskFractionFactor = 1;
        private const int DiskFractionDenominator = 4;

        public delegate ManagedBitmap BitmapProvider();


        public ImageCache()
        {
            this.swapOutGate = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        public void Trace(string operation, Entry entry, Stopwatch elapsed = null)
        {
            if (EnableTrace)
            {
                lock (this)
                {
                    if (hits != 0)
                    {
                        Program.Log(LogCat.Cache, String.Format("{0} ImageCache: hits since last update: {1}" + Environment.NewLine, DateTime.Now, hits));
                        hits = 0;
                    }
                    int disposeListInUse = 0;
                    for (int i = 0; i < disposeList.Count; i++)
                    {
                        if (!disposeList[i].CanDispose)
                        {
                            disposeListInUse++;
                        }
                    }
                    Program.Log(LogCat.Cache, String.Format(
                        "{0} ImageCache: {2,6} {3} disposeList={4} disposeList-NotDisposable={5}{6}{1}",
                        DateTime.Now,
                        Environment.NewLine,
                        operation,
                        entry != null ? entry.Id : null,
                        disposeList.Count,
                        disposeListInUse,
                        elapsed != null ? String.Concat(" elapsed=", (elapsed.ElapsedMilliseconds * .001).ToString("N3")) : null));
                    for (int i = 0; i < disposeList.Count; i++)
                    {
                        foreach (KeyValuePair<int, BitmapHolder> holder in disposeList[i].Holders)
                        {
                            if (holder.Value.Stack != null)
                            {
                                Program.Log(LogCat.Cache, String.Format("In Use [{1}]:{0}{2}{0}", Environment.NewLine, holder.Key, holder.Value.Stack));
                            }
                        }
                    }
                }
            }
        }

        public class EntryInfo
        {
            public string Id { get; private set; }
            public int Priority { get; private set; }
            public string Status { get; private set; }
            public long TotalBytes { get; private set; }
            public string TotalBytesString { get { return FileSizeText.GetFileSizeString(this.TotalBytes); } }
            public int Holders { get; private set; }
            public int OldBitmaps { get; private set; }
            public bool CanDispose { get; private set; }

            public EntryInfo(string id, int priority, string status, long totalBytes, int holders, int oldBitmaps, bool canDispose)
            {
                this.Id = id;
                this.Priority = priority;
                this.Status = status;
                this.TotalBytes = totalBytes;
                this.Holders = holders;
                this.OldBitmaps = oldBitmaps;
                this.CanDispose = canDispose;
            }

            public override bool Equals(object obj)
            {
                EntryInfo other = (EntryInfo)obj;
                return String.Equals(this.Id, other.Id)
                    && (this.Priority == other.Priority)
                    && String.Equals(this.Status, other.Status)
                    && (this.TotalBytes == other.TotalBytes)
                    && (this.Holders == other.Holders)
                    && (this.OldBitmaps == other.OldBitmaps)
                    && (this.CanDispose == other.CanDispose);
            }

            public override int GetHashCode()
            {
                // need a reasonable initial value
                int hashCode = 0L.GetHashCode();
                // implementation derived from Roslyn compiler implementation for anonymous types:
                // Microsoft.CodeAnalysis.CSharp.Symbols.AnonymousTypeManager.AnonymousTypeGetHashCodeMethodSymbol 
                const int HASH_FACTOR = -1521134295;
                unchecked
                {
                    hashCode = hashCode * HASH_FACTOR + this.Id.GetHashCode();
                    hashCode = hashCode * HASH_FACTOR + this.Priority.GetHashCode();
                    hashCode = hashCode * HASH_FACTOR + this.Status.GetHashCode();
                    hashCode = hashCode * HASH_FACTOR + this.TotalBytes.GetHashCode();
                    hashCode = hashCode * HASH_FACTOR + this.Holders.GetHashCode();
                    hashCode = hashCode * HASH_FACTOR + this.OldBitmaps.GetHashCode();
                    hashCode = hashCode * HASH_FACTOR + this.CanDispose.GetHashCode();
                }
                return hashCode;
            }
        }

        public EntryInfo[] GetCurrentEntryInfo()
        {
            lock (this)
            {
                EntryInfo[] result = new EntryInfo[entries.Count];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new EntryInfo(
                        entries[i].Id,
                        i,
                        entries[i].SwappedOut ? "Swapped" : "Resident",
                        entries[i].TotalBytes,
                        entries[i].HoldersCount,
                        entries[i].OldBitmapsCount,
                        entries[i].CanDispose);
                }

                return result;
            }
        }


        // operations

        public BitmapHolder Query(string id, BitmapProvider provider)
        {
            BitmapHolder holder;

            lock (this)
            {
                Entry entry;
                {
                    bool added = false;

                    int i = entries.FindIndex(delegate (Entry candidate) { return String.Equals(id, candidate.Id, StringComparison.OrdinalIgnoreCase); });
                    if (i < 0)
                    {
                        i = Add(id, provider);
                        added = true;
                    }
                    entry = entries[i];
                    holder = new BitmapHolder(this, entry);
                    if (added || entry.SwappedOut)
                    {
                        entry.EnsureBitmapTaskStarted(); // start creation, or initiate swap-in before any swap-out is started below (swap-in gets disk access priority)
                    }

                    entries.RemoveAt(i);
                    entries.Insert(0, entry);

                    hits = unchecked(hits + 1);
                }

                PurgeDisposeList();
            }

            return holder;
        }

        private int Add(string id, BitmapProvider provider)
        {
            lock (this)
            {
                Task<ManagedBitmap> bitmap = new Task<ManagedBitmap>(
                    delegate ()
                    {
                        try
                        {
                            return provider();
                        }
                        catch (Exception exception)
                        {
                            if (EnableTrace)
                            {
                                Program.Log(LogCat.Cache, String.Format(
                                    "{0} ImageCache: provider threw exception: {2}{1}",
                                    DateTime.Now,
                                    Environment.NewLine,
                                    exception));
                            }
                        }
                        return ManagedBitmap.CreateFromGDI(new Bitmap(Properties.Resources.InvalidPlaceHolder));
                    });

                Entry entry = new Entry(id, bitmap);
                entry.BitmapCompleted += delegate (object sender, EventArgs args) { SwapOutOverLimit(); };
                entries.Insert(0, entry);
                Trace("add", entry);

                PurgeDisposeList();
                return 0;
            }
        }

        private void SwapOutOverLimit()
        {
            lock (this)
            {
                long estimatedInUseMemory = MemoryLimit;
                if (Debugger.IsAttached)
                {
                    estimatedInUseMemory = MinimumMemory;
                }
                long availableDiskSpace = this.SwappedBytesLimit;
                for (int i = 0; i < entries.Count; i++)
                {
                    Entry entry = entries[i];

                    estimatedInUseMemory -= entry.TotalBytes * OverheadFactor / OverheadDenominator;
                    if (estimatedInUseMemory < 0)
                    {
                        if (i >= MinInMemoryCount)
                        {
                            availableDiskSpace -= entry.TotalSwappedBytes;
                            if (Program.EnableSwap && (availableDiskSpace >= 0))
                            {
                                entry.StartSwapOut(this);
                            }
                            else
                            {
                                Entry oldest = entries[i];
                                Trace("expire", oldest);
                                disposeList.Add(oldest);
                                entries.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetDiskFreeSpaceEx(
            string lpDirectoryName,
            out long lpFreeBytesAvailable, // use this (accounts for user quotas)
            out long lpTotalNumberOfBytes,
            out long lpTotalNumberOfFreeBytes);

        private long GetTempFolderFreeSpace()
        {
            long free, unused;
            if (0 == GetDiskFreeSpaceEx(Path.GetTempPath(), out free, out unused, out unused))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            return free;
        }

        public long SwappedBytesLimit
        {
            get
            {
                long availableDiskSpace;
                lock (this)
                {
                    long freeDiskSpace = GetTempFolderFreeSpace();
                    availableDiskSpace = Math.Min(freeDiskSpace * DiskFractionFactor / DiskFractionDenominator, MaxDiskUsage);
                    if (availableDiskSpace < MinDiskAvailabilitty)
                    {
                        availableDiskSpace = -1;
                    }
                }
                return availableDiskSpace;
            }
        }

        public long TotalSwappedBytes
        {
            get
            {
                long total = 0;
                lock (this)
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        total += entries[i].TotalSwappedBytes;
                    }
                    for (int i = 0; i < disposeList.Count; i++)
                    {
                        total += disposeList[i].TotalSwappedBytes;
                    }
                }
                return total;
            }
        }

        public void Invalidate(string id)
        {
            lock (this)
            {
                int i = entries.FindIndex(delegate (Entry candidate) { return String.Equals(id, candidate.Id); });
                if (i < 0)
                {
                    return;
                }

                Entry entry = entries[i];
                entries.RemoveAt(i);
                Trace("remove", entry);
                disposeList.Add(entry);

                PurgeDisposeList();
            }
        }

        public void InvalidatePrefixed(string id)
        {
            lock (this)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    Entry entry = entries[i];
                    if (entry.Id.StartsWith(id))
                    {
                        entries.RemoveAt(i);
                        i--;
                        Trace("remove", entry);
                        disposeList.Add(entry);
                    }
                }

                PurgeDisposeList();
            }
        }

        public void Touch(Entry entry)
        {
            lock (this)
            {
                int i = entries.FindIndex(delegate (Entry candidate) { return String.Equals(entry.Id, candidate.Id, StringComparison.OrdinalIgnoreCase); });
                if (i < 0)
                {
                    return;
                }
                entry = entries[i];

                entries.RemoveAt(i);
                entries.Insert(0, entry);

                hits = unchecked(hits + 1);
            }
        }

        public bool TryClear()
        {
            while (true)
            {
                bool progress = false;

                lock (this)
                {
                    this.disposeList.AddRange(this.entries);
                    this.entries.Clear();

                    int c = this.disposeList.Count;

                    if (c == 0)
                    {
                        return true;
                    }

                    PurgeDisposeList();
                    progress = c > this.disposeList.Count;
                }

                if (!progress)
                {
                    return false;
                }
            }
        }

        public void PurgeDisposeList()
        {
            lock (this)
            {
                bool somethingDone = false;

                for (int i = 0; i < disposeList.Count; i++)
                {
                    Entry entry = disposeList[i];
                    if (entry.TryDispose())
                    {
                        disposeList.RemoveAt(i);
                        i--;
                        somethingDone = true;
                    }
                }

                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].PurgeOldBitmaps())
                    {
                        somethingDone = true;
                    }
                }

                if (somethingDone)
                {
                    GC.Collect();
                }
            }
        }

        public void BeginSwapIn()
        {
            lock (this)
            {
                this.inFlightSwapIns++;
                this.swapOutGate.Reset();
            }
        }

        public void EndSwapIn()
        {
            lock (this)
            {
                this.inFlightSwapIns--;
                Debug.Assert(this.inFlightSwapIns >= 0);
                if (this.inFlightSwapIns == 0)
                {
                    this.swapOutGate.Set();
                }
            }
        }

        public void WaitSwapOutGate()
        {
            this.swapOutGate.WaitOne();
        }

        public void Dispose()
        {
            while (!TryClear())
            {
                Thread.CurrentThread.Join(100);
            }

            this.swapOutGate.Dispose();
        }
    }


    //
    // SerializationManager
    //

    public class SerializationManager : IDisposable
    {
        public readonly static SerializationManager Manager = new SerializationManager();

        private readonly List<Task<bool>> readTasks = new List<Task<bool>>();
        private readonly List<Task<bool>> writeTasks = new List<Task<bool>>();
        private readonly EventWaitHandle available;
        private readonly EventWaitHandle exited;
        private Thread taskThread;
        private int terminate;

        public SerializationManager()
        {
            this.available = new EventWaitHandle(false, EventResetMode.ManualReset);
            this.exited = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        private void EnsureStarted()
        {
            lock (this)
            {
                if (taskThread == null)
                {
                    this.exited.Reset();
                    taskThread = new Thread(new ThreadStart(delegate () { this.TaskThreadMain(); }));
                    taskThread.Start();
                }
            }
        }

        public void EnqueueWriteTask(Task<bool> task)
        {
            lock (this)
            {
                writeTasks.Add(task);
                available.Set();

                EnsureStarted();
            }
        }

        public void EnqueueReadTask(Task<bool> task)
        {
            lock (this)
            {
                readTasks.Add(task);
                available.Set();

                EnsureStarted();
            }
        }

        public void Prioritize(Task<bool> task)
        {
            lock (this)
            {
                int i;
                if ((i = readTasks.IndexOf(task)) >= 0)
                {
                    readTasks.RemoveAt(i);
                    readTasks.Insert(0, task);
                }
                else if ((i = writeTasks.IndexOf(task)) >= 0)
                {
                    writeTasks.RemoveAt(i);
                    readTasks.Insert(0, task); // move to read queue
                }
                // else - may have already been dequeued
            }
        }

        private void TaskThreadMain()
        {
            try
            {
                while (true)
                {
                    Task<bool> task;

                    this.available.WaitOne();
                    if (Thread.VolatileRead(ref terminate) != 0)
                    {
                        break;
                    }

                    lock (this)
                    {
                        if (readTasks.Count != 0)
                        {
                            task = readTasks[0];
                            readTasks.RemoveAt(0);
                        }
                        else if (writeTasks.Count != 0)
                        {
                            task = writeTasks[0];
                            writeTasks.RemoveAt(0);
                        }
                        else
                        {
                            const string Message = "if 'available' is signalled, there must be at least one task queued";
                            Debug.Assert(false, Message);
                            throw new InvalidOperationException(Message);
                        }

                        if ((readTasks.Count == 0) && (writeTasks.Count == 0))
                        {
                            this.available.Reset();
                        }
                    }

                    task.Start();
                    task.Wait();
                }
            }
            catch (Exception exception)
            {
                Program.Log(LogCat.Cache, String.Concat("SerializationManager task thread exited with exception: ", exception.ToString(), Environment.NewLine));
                MessageBox.Show(exception.ToString());
            }
            finally
            {
                exited.Set();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                Thread.VolatileWrite(ref terminate, 1);
                this.available.Set();
            }
            this.exited.WaitOne();

            this.available.Dispose();
            this.exited.Dispose();
        }
    }
}
