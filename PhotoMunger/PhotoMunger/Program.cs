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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;

namespace AdaptiveImageSizeReducer
{
    public static class Program
    {
        public const string SettingsFile = "settings.xml";

        public static bool ProfileMode; // true == single-threaded for stable timing
        public readonly static int MainThreadId = Thread.CurrentThread.ManagedThreadId;
        public static bool UseGDIResize = false;
        public static bool EnableSwap = false;
        public readonly static Dispatcher MainThreadDispatcher = Dispatcher.CurrentDispatcher;

        public static ParallelOptions GetProcessorConstrainedParallelOptions(CancellationToken cancel)
        {
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = !Program.ProfileMode ? Environment.ProcessorCount : 1;
            options.CancellationToken = cancel;
            return options;
        }

        public static ParallelOptions GetProcessorConstrainedParallelOptions2(CancellationToken cancel)
        {
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            options.CancellationToken = cancel;
            return options;
        }

        public static void Log(LogCat cat, string text)
        {
            Debugger.Log(0, null, text);

            LoggingWindow window = logWindow;
            if (window != null)
            {
                if (Thread.CurrentThread.ManagedThreadId == MainThreadId)
                {
                    window.Append(cat, text);
                }
                else
                {
                    LoggingWindow.AppendDelegate d = window.Append;
                    MainThreadDispatcher.BeginInvoke(d, new object[] { cat, text });
                }
            }
        }
        public static LoggingWindow logWindow;

        public static string GetScanDirectoryFromTargetDirectory(string directory)
        {
            string sourceDirectory = directory + Window.SourceDirectorySuffix;
            string scanDirectory = Directory.Exists(sourceDirectory) ? sourceDirectory : directory;
            return scanDirectory;
        }


        private static void ShiftArgs(ref string[] args, int count)
        {
            Array.Copy(args, count, args, 0, args.Length - count);
            Array.Resize(ref args, args.Length - count);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if ((args.Length >= 1) && String.Equals(args[0], "-waitdebugger"))
                {
                    while (!Debugger.IsAttached)
                    {
                        Thread.Sleep(250);
                    }
                    ShiftArgs(ref args, 1);
                }

                if ((args.Length == 2) && String.Equals(args[0], "-server"))
                {
                    Environment.ExitCode = ImageServer.RunServer(args[1]);
                    return;
                }


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);


                bool showLogOnStart = false;
                while (args.Length >= 1)
                {
                    switch (args[0])
                    {
                        default:
                            goto Break;

                        case "-profile":
                            ProfileMode = true;
                            ShiftArgs(ref args, 1);
                            break;

                        case "-showlog":
                            showLogOnStart = true;
                            ShiftArgs(ref args, 1);
                            break;

                        case "-swap":
                            EnableSwap = true;
                            ShiftArgs(ref args, 1);
                            break;

                        case "-noswap":
                            EnableSwap = false;
                            ShiftArgs(ref args, 1);
                            break;
                    }
                }
            Break:

                if (args.Length == 0)
                {
                    // HACK: based on what WinMerge does - usability is suspect but still much better than FolderBrowserDialog.
                    // See: http://www.codeproject.com/Articles/44914/Select-file-or-folder-from-the-same-dialog
                    using (OpenFileDialog dialog = new OpenFileDialog())
                    {
                        dialog.ValidateNames = false;
                        dialog.CheckFileExists = false;
                        dialog.CheckPathExists = false;
                        dialog.Title = "Select a Folder";
                        dialog.FileName = "Select Folder.";
                        if (dialog.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                        args = new string[] { Path.GetDirectoryName(dialog.FileName) };
                    }
                }

                if (args.Length != 1)
                {
                    MessageBox.Show("Program must take one argument which is the path to a directory containing images to adjust.");
                    return;
                }

                Window window = null;
                ImageCache cache = new ImageCache();

                string directory = Path.GetFullPath(args[0]);
                string scanDirectory = GetScanDirectoryFromTargetDirectory(directory);

                if (!Directory.Exists(directory))
                {
                    MessageBox.Show(String.Format("Specified directory does not exist: \"{0}\"", directory));
                    return;
                }

                XPathNavigator settingsNav;
                Dictionary<string, int> sourceFileNameToSequenceNumber = new Dictionary<string, int>();
                {
                    XmlDocument settings = new XmlDocument();
                    string settingsPath = Path.Combine(scanDirectory, SettingsFile);
                    if (File.Exists(settingsPath))
                    {
                        settings.Load(settingsPath);
                    }
                    settingsNav = settings.CreateNavigator();

                    int i = 0;
                    foreach (XPathNavigator nav in settingsNav.Select("/*/items/item/file"))
                    {
                        sourceFileNameToSequenceNumber.Add(nav.Value, i++);
                    }
                }

                GlobalOptions options = new GlobalOptions(settingsNav.SelectSingleNode("/*/options"));
                {
                    using (GlobalOptionsDialog dialog = new GlobalOptionsDialog(options, directory))
                    {
                        Application.Run(dialog);
                        if (dialog.DialogResult != DialogResult.OK)
                        {
                            return;
                        }
                    }
                }

                if (showLogOnStart)
                {
                    logWindow = new LoggingWindow();
                    logWindow.Show();
                }

                // scan items
                List<Item> items = new List<Item>();
                foreach (string filePath in Directory.GetFiles(scanDirectory))
                {
                    string fileName = Path.GetFileName(filePath);
                    if (String.Equals(fileName, SettingsFile, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    bool extUpper = String.Equals(Path.GetExtension(fileName), Path.GetExtension(fileName).ToUpper());

                    Item item = new Item(Path.Combine(directory, fileName), options, cache);
                    items.Add(item);

                    XPathNavigator itemNav = settingsNav.SelectSingleNode(String.Format("/*/items/item[file=\"{0}\"]", fileName));
                    if (itemNav != null)
                    {
                        item.ReadXml(itemNav);
                        itemNav.DeleteSelf();
                    }
                    else
                    {
                        item.SettingsNav = settingsNav; // no match; try after hash has been computed
                    }
                }
                items.Sort(
                    delegate (Item l, Item r)
                    {
                        int c;
                        int li, ri;
                        if (!sourceFileNameToSequenceNumber.TryGetValue(l.SourceFileName, out li))
                        {
                            li = Int32.MaxValue;
                        }
                        if (!sourceFileNameToSequenceNumber.TryGetValue(r.SourceFileName, out ri))
                        {
                            ri = Int32.MaxValue;
                        }
                        c = li.CompareTo(ri);
                        if (c == 0)
                        {
                            c = String.Compare(l.SourceFileName, r.SourceFileName, StringComparison.CurrentCultureIgnoreCase);
                        }
                        return c;
                    });

                if (items.Count != 0)
                {
                    window = new Window(directory, items, cache, options);
                    window.Show();

                    window.LastAnalysisTask = BatchAnalyzerQueue.BeginAnalyzeBatch(items);

                    Application.Run(window);
                }
                else
                {
                    MessageBox.Show(String.Format("The specified folder \"{0}\" contains no images.", scanDirectory));
                }

                LoggingWindow logw = logWindow;
                if (logw != null)
                {
                    logw.Close();
                }

                if (window != null)
                {
                    window.LastAnalysisTask.Wait(); // allow any unfinished actions to cancel and dispose state
                }

                cache.Dispose();
                SerializationManager.Manager.Dispose();
                ImageClient.Close();
            }
            catch (Exception exception)
            {
                Debugger.Log(0, null, exception.ToString());
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
