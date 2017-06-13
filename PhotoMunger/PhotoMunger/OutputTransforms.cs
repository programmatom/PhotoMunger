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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public static class OutputTransforms
    {
        private const int RetryCount = 5;
        private const int SleepRetry = 250; // msec

        public class Stats
        {
            public int deleted;
            public int rotated;
            public int cropped;
            public int shrunk;
            public int original;
            public int brightAdjust;
            public int polyUnbias;
            public int oneBit;
        }

        public enum OutputFormat { Jpeg, Bmp, Png };

        // use highest quality transforms and no cached bitmaps
        public static void StartFinalOutput(IList<Item> items, Stats stats, ProgressDialog progressDialog, CancellationTokenSource cancel)
        {
            Task<bool> finalOutputTask = new Task<bool>(
                delegate ()
                {
                    try
                    {
                        Program.Log(LogCat.All, "++++ BEGIN OUTPUT PROCESSING ++++" + Environment.NewLine);

                        Dictionary<string, bool> createdFiles = new Dictionary<string, bool>();

                        ParallelOptions options = Program.GetProcessorConstrainedParallelOptions(CancellationToken.None);
                        options.CancellationToken = cancel.Token;
                        Parallel.ForEach(
                            items,
                            options,
                            delegate (Item item)
                            {
                                FinalOutputOne(item, stats, createdFiles, cancel);
                                progressDialog.BumpFromAnyThread(!item.Delete);
                            });

                        // remove all non-emitted files
                        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(items[0].TargetPath)))
                        {
                            if (!createdFiles.ContainsKey(Path.GetFileName(file).ToLowerInvariant()))
                            {
                                File.Delete(file);
                            }
                        }

                        // Maintain a list of all directories visited by program, ever. Forgot why I do this.
                        string recordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PhotoMunger", "visited.txt");
                        Directory.CreateDirectory(Path.GetDirectoryName(recordPath));
                        List<string> files = new List<string>();
                        if (File.Exists(recordPath))
                        {
                            files.AddRange(new List<string>(File.ReadAllLines(recordPath, Encoding.UTF8)));
                        }
                        string targetPath = Path.GetDirectoryName(items[0].TargetPath);
                        if (files.FindIndex(delegate (string candidate) { return String.Equals(candidate, targetPath, StringComparison.OrdinalIgnoreCase); }) < 0)
                        {
                            files.Add(targetPath);
                        }
                        files.Sort();
                        File.WriteAllLines(recordPath, files.ToArray(), Encoding.UTF8);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        Program.Log(LogCat.All, exception.ToString() + Environment.NewLine);
                        MessageBox.Show(exception.ToString());
                    }
                    finally
                    {
                        Program.Log(LogCat.All, "---- END OUTPUT PROCESSING ----" + Environment.NewLine);

                        progressDialog.CloseFromAnyThread();
                    }

                    return false;
                });
            finalOutputTask.Start();
        }

        // use highest quality transforms and no cached bitmaps
        public static void FinalOutputOne(Item item, Stats stats, Dictionary<string, bool> createdFiles, CancellationTokenSource cancel)
        {
            Profile profile = new Profile("FinalOutput {0}", item.RenamedFileName);

            profile.Push("Item.WaitInit");
            item.WaitInit();
            profile.Pop();

            DateTime originalCreationTime = File.GetCreationTime(item.SourcePath);
            DateTime originalLastWriteTime = File.GetLastWriteTime(item.SourcePath);

            string renamedTargetPath = Path.Combine(Path.GetDirectoryName(item.TargetPath), item.RenamedFileName);

            if (item.Delete)
            {
                // actual deletion occurs after end of run, by file being omitted from createdFiles
                Interlocked.Increment(ref stats.deleted);
            }
            else if (item.Valid)
            {
                // load source

                string tempFile = Path.GetTempFileName();
                File.Copy(item.SourcePath, tempFile, true/*overwrite*/);


                SmartBitmap bitmap = null;

                bool jpegTranRotationRequired = true;


                // initial lossy transforms

                if (item.NormalizeGeometry || (item.FineRotateDegrees != 0))
                {
                    if (bitmap == null)
                    {
                        bitmap = new SmartBitmap(Transforms.LoadAndOrientGDI(tempFile, profile));
                    }

                    jpegTranRotationRequired = false;
                    if (item.RightRotations != 0)
                    {
                        ManagedBitmap bitmap2 = bitmap.AsManaged().RotateFlip(Transforms.RotateFlipFromRightRotations(item.RightRotations));
                        bitmap.Dispose();
                        bitmap = new SmartBitmap(bitmap2);
                    }

                    Transforms.ApplyNormalizeGeometry(
                        profile,
                        item.SourceFileName,
                        bitmap.AsManaged(),
                        1,
                        new Transforms.NormalizeGeometryParameters(
                            item.CornerTL,
                            item.CornerTR,
                            item.CornerBL,
                            item.CornerBR,
                            item.NormalizeGeometryForcedAspectRatio,
                            item.FineRotateDegrees,
                            item.NormalizeGeometryFinalInterp),
                        delegate (string text) { },
                        cancel);
                }


                // lossless transform phase

                if (bitmap == null)
                {
                    List<string> jpegtranMessages = new List<string>();
                    string error;

                    if (jpegTranRotationRequired &&
                        ((item.RightRotations != 0) || (item.OriginalExifOrientation != RotateFlipType.RotateNoneFlipNone)))
                    {
                        int combinedRotations = (item.RightRotations + Transforms.RightRotationsFromRotateFlipType(item.OriginalExifOrientation)) % 4;

                        // TODO: strip only Exif orientation after doing this - how? (currently strips all)
                        if (Transforms.LosslessRotateRightFinal(
                            tempFile,
                            tempFile,
                            combinedRotations,
                            out error))
                        {
                            Interlocked.Increment(ref stats.rotated);
                        }
                        else
                        {
                            jpegtranMessages.Add(String.Format("Lossless rotate failed for \"{0}\" ({1}).", item.RenamedFileName, error));
                        }
                    }

                    if (!item.CropRect.IsEmpty)
                    {
                        if (Transforms.LosslessCropFinal(tempFile, tempFile, item.CropRect, out error))
                        {
                            Interlocked.Increment(ref stats.cropped);
                        }
                        else
                        {
                            jpegtranMessages.Add(String.Format("Lossless crop failed for \"{0}\" ({1}).", item.RenamedFileName, error));
                        }
                    }

                    if (jpegtranMessages.Count != 0)
                    {
                        MessageBox.Show(String.Join(" ", jpegtranMessages.ToArray()));
                    }
                }
                else
                {
                    // whoops- preceding transform prevents jpegtran from being used (recreating jpeg would be lossy)

                    if (!item.CropRect.IsEmpty)
                    {
                        ManagedBitmap bitmap2 = bitmap.AsManaged().Crop(item.CropRect);
                        bitmap.Dispose();
                        bitmap = new SmartBitmap(bitmap2);
                    }
                }


                // following lossy transforms phase

                if (item.Unbias)
                {
                    if (bitmap == null)
                    {
                        bitmap = new SmartBitmap(Transforms.LoadAndOrientGDI(tempFile, profile));
                    }

                    Transforms.PolyUnbiasDiagnostics unused;
                    Transforms.ApplyPolyUnbias(
                        profile,
                        item.RenamedFileName,
                        bitmap.AsManaged(profile),
                        Rectangle.Empty/*already cropped*/,
                        new Transforms.PolyUnbiasParameters(
                            item.UnbiasMaxDegree,
                            item.UnbiasMaxChisq,
                            item.UnbiasMaxS,
                            item.UnbiasMinV),
                        null,
                        out unused,
                        cancel);

                    Interlocked.Increment(ref stats.polyUnbias);
                }

                if (item.BrightAdjust)
                {
                    if (bitmap == null)
                    {
                        bitmap = new SmartBitmap(Transforms.LoadAndOrientGDI(tempFile, profile));
                    }

                    Transforms.ApplyBrightAdjust(
                        profile,
                        item.RenamedFileName,
                        bitmap.AsManaged(profile),
                        Rectangle.Empty/*already cropped*/,
                        new Transforms.BrightAdjustParameters(item.BrightAdjustMinClusterFrac, item.BrightAdjustWhiteCorrect),
                        null,
                        cancel);

                    Interlocked.Increment(ref stats.brightAdjust);
                }

                if (item.StaticSaturate)
                {
                    if (bitmap == null)
                    {
                        bitmap = new SmartBitmap(Transforms.LoadAndOrientGDI(tempFile, profile));
                    }

                    Transforms.ApplyStaticSaturation(
                        profile,
                        item.RenamedFileName,
                        bitmap.AsManaged(profile),
                        Rectangle.Empty/*already cropped*/,
                        new Transforms.StaticSaturateParameters(item.StaticSaturateWhiteThreshhold, item.StaticSaturateBlackThreshhold, item.StaticSaturateExponent),
                        cancel);
                }

                if (item.Shrink)
                {
                    if (bitmap == null)
                    {
                        bitmap = new SmartBitmap(Transforms.LoadAndOrientGDI(tempFile, profile));
                    }

                    //Bitmap shrunk = Transforms.Shrink(profile, bitmap.AsGDI(profile), item.ShrinkFactor);
                    int newWidth = (int)Math.Floor(bitmap.Width / item.ShrinkFactor);
                    int newHeight = (int)Math.Floor(bitmap.Height / item.ShrinkFactor);
                    ManagedBitmap bitmap2 = ImageClient.ResizeGDI(profile, bitmap.AsManaged(profile), newWidth, newHeight);
                    bitmap.Dispose();
                    bitmap = new SmartBitmap(bitmap2);

                    Interlocked.Increment(ref stats.shrunk);
                }

                // TODO: eliminate shrink and OneBit's expand if both are configured

                if (item.OneBit)
                {
                    if (bitmap == null)
                    {
                        bitmap = new SmartBitmap(Transforms.LoadAndOrientGDI(tempFile, profile));
                    }

                    SmartBitmap bitmap2 = new SmartBitmap(
                        Transforms.ApplyOneBit(
                            profile,
                            item.RenamedFileName,
                            bitmap,
                            new Transforms.OneBitParameters(item.OneBitChannel, item.OneBitThreshhold, item.OneBitScaleUp),
                            cancel));
                    bitmap.Dispose();
                    bitmap = bitmap2;

                    Interlocked.Increment(ref stats.oneBit);
                }

                if (bitmap != null)
                {
                    Transforms.SaveImage(profile, bitmap, tempFile, item.JpegQuality, item.JpegUseGdi, item.OutputFormat);
                    bitmap.Dispose();
                }


                // write target

                string targetPath;
                bool extUpper = String.Equals(Path.GetExtension(renamedTargetPath), Path.GetExtension(renamedTargetPath).ToUpper());
                switch (item.OutputFormat)
                {
                    default:
                        Debug.Assert(false);
                        throw new ArgumentException();
                    case OutputFormat.Jpeg:
                        targetPath = Path.ChangeExtension(renamedTargetPath, extUpper ? ".JPG" : ".jpg");
                        break;
                    case OutputFormat.Bmp:
                        targetPath = Path.ChangeExtension(renamedTargetPath, extUpper ? ".BMP" : ".bmp");
                        break;
                    case OutputFormat.Png:
                        targetPath = Path.ChangeExtension(renamedTargetPath, extUpper ? ".PNG" : ".png");
                        break;
                }
                for (int i = 0; i <= RetryCount; i++)
                {
                    try
                    {
                        File.Copy(tempFile, targetPath, true/*overwrite*/);
                        File.SetCreationTime(targetPath, originalCreationTime);
                        File.SetLastWriteTime(targetPath, DateTime.Now);
                    }
                    catch (IOException) when (i < RetryCount)
                    {
                        // HACK: If folder is open, Explorer may have file locked to refresh thumbnail
                        Thread.Sleep(SleepRetry);
                    }
                }

                lock (createdFiles)
                {
                    createdFiles.Add(Path.GetFileName(targetPath).ToLowerInvariant(), false);
                }

                File.Delete(tempFile);
            }
            else
            {
                lock (createdFiles)
                {
                    createdFiles.Add(Path.GetFileName(renamedTargetPath).ToLowerInvariant(), false);
                }

                File.Copy(item.SourcePath, renamedTargetPath, true/*overwrite*/);

            }

            profile.End();
            Program.Log(LogCat.Perf, profile.Report());
        }
    }
}
