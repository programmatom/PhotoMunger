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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace AdaptiveImageSizeReducer
{
    public enum Status { Pending, Valid, Invalid };

    public class Item : INotifyPropertyChanged
    {
        // USE CAUTIOUSLY: always the path to the user's folder, i.e. to which file modifications will be written, and
        // sometimes the source/original
        private readonly string targetPath;
        private readonly GlobalOptions options;
        private readonly ImageCache cache;

        private string rename;

        private int width, height;

        private RotateFlipType exifOrientation;

        private int? jpegQuality;
        private bool? jpegUseGdi;

        private bool? shrink;
        private float? shrinkFactor;

        private bool delete;

        private int? rightRotations;

        private bool? brightAdjust;
        private float? brightAdjustMinClusterFrac;
        private bool? brightAdjustWhiteCorrect;

        private bool? unbias;
        private int? unbiasMaxDegree;
        private float? unbiasMaxChisq;

        private bool? oneBit;
        private Transforms.Channel? oneBitChannel;
        private float? oneBitThreshhold;

        private bool? staticSaturate;
        private float? staticSaturateWhiteThreshhold;
        private float? staticSaturateBlackThreshhold;
        private float? staticSaturateExponent;

        private bool normalizeGeometry;
        private bool normalizeGeometryExplicitlySet;
        private Point cornerTL, cornerTR, cornerBL, cornerBR;
        private int cornerRotation;
        private float? normalizedGeometryAspectRatio;

        private double fineRotateDegrees;

        private Status status;
        private Task<bool> validateTask;
        private CancellationTokenSource validateTaskCancel;
        private bool permitCurrentViewDuringReanalysis;

        private bool[,] hf;
        private bool[,] autoCropGrid;
        private Transforms.PolyUnbiasDiagnostics polyUnbiasDiagnostics;
        private Rectangle cropRect;
        private bool cropRectExplicitlySet;
        private int cropRectRotation;

        private string analysisMessage;
        private string message;

        private Color tagColor = Color.Black;

        private XPathNavigator settingsNav;
        private string hash;

        public const int AnnotationDivisor = 4;

        public event PropertyChangedEventHandler PropertyChanged;
        private delegate void FirePropertyChangedDelegate(string name);
        private readonly FirePropertyChangedDelegate propertyChangedDelegate;


        public override string ToString()
        {
            return this.RenamedFileName;
        }

        // property accessors

        private void FirePropertyChanged(string name)
        {
            if (Thread.CurrentThread.ManagedThreadId == Program.MainThreadId)
            {
                FirePropertyChanged_MainThread(name);
            }
            else
            {
                FirePropertyChangedDelegate d = this.FirePropertyChanged;
                Program.MainThreadDispatcher.BeginInvoke(d, new object[] { name });
            }
        }

        private void FirePropertyChanged_MainThread(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        // always the path to the user's folder, i.e. to which file modifications will be written
        [Bindable(true)]
        public string TargetPath { get { return targetPath; } }

        // the path to the user's folder, unless the "-original" source/backup exists, in which case it is to the file in "-original"
        [Bindable(true)]
        public string SourcePath { get { return GetActualSourcePath(this.targetPath); } }

        public static string GetSourcePath(string path)
        {
            string file = Path.GetFileName(path);
            string directory = Path.GetDirectoryName(path);
            return Path.Combine(directory + Window.SourceDirectorySuffix, file);
        }

        private static string GetActualSourcePath(string path)
        {
            string sourcePath = GetSourcePath(path);
            return File.Exists(sourcePath) ? sourcePath : path;
        }

        [Bindable(true)]
        public string SourceFileName { get { return Path.GetFileName(this.targetPath); } }

        [Bindable(true)]
        public string RenamedFileName
        {
            get
            {
                return this.rename != null ? this.rename : this.SourceFileName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    this.rename = null;
                    return;
                }
                this.rename = value;

                FirePropertyChanged("RenamedFileName");
            }
        }

        [Bindable(true)]
        public string FileSize
        {
            get
            {
                if (!File.Exists(SourcePath))
                {
                    return "missing";
                }
                return FileSizeText.GetFileSizeString(SourcePath);
            }
        }

        [Bindable(true)]
        public int JpegQuality
        {
            get
            {
                return jpegQuality.HasValue ? jpegQuality.Value : options.JpegQuality;
            }
            set
            {
                jpegQuality = value;

                FirePropertyChanged("JpegQuality");
            }
        }

        [Bindable(true)]
        public bool JpegUseGdi
        {
            get
            {
                return jpegUseGdi.HasValue ? jpegUseGdi.Value : options.JpegUseGDI;
            }
            set
            {
                jpegUseGdi = value;

                FirePropertyChanged("JpegUseGdi");
            }
        }

        [Bindable(true)]
        public bool Shrink
        {
            get
            {
                return shrink.HasValue ? shrink.Value : options.Shrink;
            }
            set
            {
                shrink = value;

                FirePropertyChanged("Shrink");
            }
        }

        [Bindable(true)]
        public float ShrinkFactor
        {
            get
            {
                return shrinkFactor.HasValue ? shrinkFactor.Value : options.ShrinkFactor;
            }
            set
            {
                shrinkFactor = value;

                FirePropertyChanged("ShrinkFactor");
            }
        }

        [Bindable(true)]
        public bool Delete
        {
            get
            {
                return delete;
            }
            set
            {
                delete = value;

                FirePropertyChanged("Delete");
            }
        }

        [Bindable(true)]
        public RotateFlipType OriginalExifOrientation { get { return exifOrientation; } }

        [Bindable(true)]
        public int RightRotations
        {
            get
            {
                return rightRotations.HasValue ? rightRotations.Value : options.RightRotations;
            }
            set
            {
                bool flipAspect = Math.Abs(RightRotations - value) % 2 != 0;

                rightRotations = value;
                cache.InvalidatePrefixed(SourceId);

                if (flipAspect)
                {
                    int t = this.width;
                    this.width = this.height;
                    this.height = t;
                }

                ResetAnalyzeTask(true/*invalidateCurrentView*/);
                StartInit();

                FirePropertyChanged("RightRotations");
            }
        }

        [Bindable(true)]
        public bool Valid
        {
            get
            {
                return (this.Status == Status.Valid) || (this.permitCurrentViewDuringReanalysis && (this.Status == Status.Pending));
            }
        }

        [Bindable(true)]
        public Status Status { get { return this.status; } }

        [Bindable(true)]
        public int Width { get { return width; } }

        [Bindable(true)]
        public int Height { get { return height; } }

        private void SetCropRect(Rectangle rect, bool explicitlySet)
        {
            Debug.Assert((rect.Left == Transforms.SnapLossless(rect.Left)) && (rect.Top == Transforms.SnapLossless(rect.Top))
                && (rect.Right == Transforms.SnapLossless(rect.Right)) && (rect.Bottom == Transforms.SnapLossless(rect.Bottom)));
            rect.Intersect(new Rectangle(0, 0, this.width, this.height));
            cropRect = rect;
            cropRectRotation = RightRotations;
            this.cropRectExplicitlySet = this.cropRectExplicitlySet || explicitlySet;

            cache.InvalidatePrefixed(SourceId + ":Post:");

            FirePropertyChanged("CropRect");
            FirePropertyChanged("CropRectText");
        }

        [Bindable(true)]
        public Rectangle CropRect
        {
            get
            {
                if (!this.cropRect.IsEmpty && (this.cropRectRotation != this.RightRotations))
                {
                    if (this.cropRectRotation > this.RightRotations)
                    {
                        this.cropRectRotation -= 4;
                    }
                    int oldWidth = (this.RightRotations - this.cropRectRotation) % 2 == 0 ? this.width : this.height;
                    int oldHeight = (this.RightRotations - this.cropRectRotation) % 2 == 0 ? this.height : this.width;
                    this.cropRect = Transforms.RotateRect(oldWidth, oldHeight, this.RightRotations - this.cropRectRotation, this.cropRect);
                    this.cropRectRotation = this.RightRotations;
                }
                return cropRect;
            }
            set
            {
                SetCropRect(value, true/*explicitlySet*/);
            }
        }

        [Bindable(false)]
        public Rectangle CropRectForAutoCrop { set { SetCropRect(value, false/*explicitlySet*/); } }

        [Bindable(false)]
        public bool CropRectExplicitlySet { get { return this.cropRectExplicitlySet; } }

        [Bindable(true)]
        public string CropRectText
        {
            get
            {
                return !cropRect.IsEmpty
                    ? String.Format("{0}, {1}, {2}, {3}", cropRect.X, cropRect.Y, cropRect.Width, cropRect.Height)
                    : "None";
            }
        }

        [Bindable(true)]
        public bool BrightAdjust
        {
            get
            {
                return brightAdjust.HasValue ? brightAdjust.Value : options.BrightAdjust;
            }
            set
            {
                brightAdjust = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("BrightAdjust");
            }
        }

        [Bindable(true)]
        public bool BrightAdjustWhiteCorrect
        {
            get
            {
                return brightAdjustWhiteCorrect.HasValue ? brightAdjustWhiteCorrect.Value : options.BrightAdjustWhiteCorrect;
            }
            set
            {
                brightAdjustWhiteCorrect = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("BrightAdjustWhiteCorrect");
            }
        }

        [Bindable(true)]
        public float BrightAdjustMinClusterFrac
        {
            get
            {
                return brightAdjustMinClusterFrac.HasValue ? brightAdjustMinClusterFrac.Value : options.BrightAdjustMinClusterFrac;
            }
            set
            {
                brightAdjustMinClusterFrac = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("BrightAdjustMinClusterFrac");
            }
        }

        [Bindable(true)]
        public bool Unbias
        {
            get
            {
                return unbias.HasValue ? unbias.Value : options.Unbias;
            }
            set
            {
                unbias = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("Unbias");
            }
        }

        [Bindable(true)]
        public int UnbiasMaxDegree
        {
            get
            {
                return unbiasMaxDegree.HasValue ? unbiasMaxDegree.Value : options.UnbiasMaxDegree;
            }
            set
            {
                unbiasMaxDegree = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("UnbiasMaxDegree");
            }
        }

        [Bindable(true)]
        public float UnbiasMaxChisq
        {
            get
            {
                return unbiasMaxChisq.HasValue ? unbiasMaxChisq.Value : options.UnbiasMaxChisq;
            }
            set
            {
                unbiasMaxChisq = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("UnbiasMaxChisq");
            }
        }

        [Bindable(true)]
        public bool OneBit
        {
            get
            {
                return oneBit.HasValue ? oneBit.Value : options.OneBit;
            }
            set
            {
                oneBit = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("OneBit");
            }
        }

        [Bindable(true)]
        public Transforms.Channel OneBitChannel
        {
            get
            {
                return oneBitChannel.HasValue ? oneBitChannel.Value : options.OneBitChannel;
            }
            set
            {
                oneBitChannel = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("OneBitChannel");
            }
        }

        [Bindable(true)]
        public float OneBitThreshhold
        {
            get
            {
                return oneBitThreshhold.HasValue ? oneBitThreshhold.Value : options.OneBitThreshhold;
            }
            set
            {
                oneBitThreshhold = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("OneBitThreshhold");
            }
        }

        [Bindable(true)]
        public OutputTransforms.OutputFormat OutputFormat
        {
            get
            {
                if (!OneBit)
                {
                    return OutputTransforms.OutputFormat.Jpeg;
                }
                else
                {
                    return options.OneBitOutputFormat;
                }
            }
        }

        [Bindable(true)]
        public bool OneBitScaleUp { get { return options.OneBitScaleUp; } }

        [Bindable(true)]
        public bool StaticSaturate
        {
            get
            {
                return this.staticSaturate.HasValue ? this.staticSaturate.Value : options.StaticSaturate;
            }
            set
            {
                this.staticSaturate = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("StaticSaturate");
            }
        }

        [Bindable(true)]
        public float StaticSaturateWhiteThreshhold
        {
            get
            {
                return this.staticSaturateWhiteThreshhold.HasValue ? this.staticSaturateWhiteThreshhold.Value : options.StaticSaturateWhiteThreshhold;
            }
            set
            {
                this.staticSaturateWhiteThreshhold = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("StaticSaturateWhiteThreshhold");
            }
        }

        [Bindable(true)]
        public float StaticSaturateBlackThreshhold
        {
            get
            {
                return this.staticSaturateBlackThreshhold.HasValue ? this.staticSaturateBlackThreshhold.Value : options.StaticSaturateBlackThreshhold;
            }
            set
            {
                this.staticSaturateBlackThreshhold = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("StaticSaturateBlackThreshhold");
            }
        }

        [Bindable(true)]
        public float StaticSaturateExponent
        {
            get
            {
                return this.staticSaturateExponent.HasValue ? this.staticSaturateExponent.Value : options.StaticSaturateExponent;
            }
            set
            {
                this.staticSaturateExponent = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");

                FirePropertyChanged("StaticSaturateExponent");
            }
        }

        [Bindable(true)]
        public bool NormalizeGeometry
        {
            get
            {
                return normalizeGeometry;
            }
            set
            {
                if (this.normalizeGeometry != value)
                {
                    this.normalizeGeometry = value;
                    this.normalizeGeometryExplicitlySet = true;

                    ResetAnalyzeTask(false/*invalidateCurrentView*/);
                    StartInit();

                    FirePropertyChanged("NormalizeGeometry");
                }
            }
        }

        private void EnsureCornerOrientation()
        {
            if (this.cornerRotation != this.RightRotations)
            {
                if (this.cornerRotation > this.RightRotations)
                {
                    this.cornerRotation -= 4;
                }
                Debug.Assert(!(this.cornerRotation > this.RightRotations));
                int oldWidth, oldHeight;
                if ((this.RightRotations - this.cornerRotation) % 2 == 0)
                {
                    oldWidth = this.width;
                    oldHeight = this.height;
                }
                else
                {
                    oldWidth = this.height;
                    oldHeight = this.width;
                }
                Point[] points = new Point[] { this.cornerTL, this.cornerTR, this.cornerBR, this.cornerBL };
                Transforms.RotatePoints(oldWidth, oldHeight, this.RightRotations - this.cornerRotation, points);
                int o = this.RightRotations - this.cornerRotation;
                this.cornerTL = points[o];
                o = (o + 1) % 4;
                this.cornerTR = points[o];
                o = (o + 1) % 4;
                this.cornerBR = points[o];
                o = (o + 1) % 4;
                this.cornerBL = points[o];
                this.cornerRotation = this.RightRotations;
            }
        }

        [Bindable(true)]
        public Point CornerTL
        {
            get
            {
                EnsureCornerOrientation();
                return cornerTL;
            }
        }

        public void SetCornerTL(Point value, bool setByUser)
        {
            EnsureCornerOrientation();

            cornerTL = value;

            cache.InvalidatePrefixed(SourceId + ":Post:"); // invalidate old annotated image

            if (setByUser)
            {
                normalizeGeometryExplicitlySet = true;

                ResetAnalyzeTask(false/*invalidateCurrentView*/);
                StartInit();
            }

            FirePropertyChanged("CornerTL");
        }

        [Bindable(true)]
        public Point CornerTR
        {
            get
            {
                EnsureCornerOrientation();
                return cornerTR;
            }
        }

        public void SetCornerTR(Point value, bool setByUser)
        {
            EnsureCornerOrientation();

            cornerTR = value;

            cache.InvalidatePrefixed(SourceId + ":Post:"); // invalidate old annotated image

            if (setByUser)
            {
                normalizeGeometryExplicitlySet = true;

                ResetAnalyzeTask(false/*invalidateCurrentView*/);
                StartInit();
            }

            FirePropertyChanged("CornerTR");
        }

        [Bindable(true)]
        public Point CornerBL
        {
            get
            {
                EnsureCornerOrientation();
                return cornerBL;
            }
        }

        public void SetCornerBL(Point value, bool setByUser)
        {
            EnsureCornerOrientation();

            cornerBL = value;

            cache.InvalidatePrefixed(SourceId + ":Post:"); // invalidate old annotated image

            if (setByUser)
            {
                normalizeGeometryExplicitlySet = true;

                ResetAnalyzeTask(false/*invalidateCurrentView*/);
                StartInit();
            }

            FirePropertyChanged("CornerBL");
        }

        [Bindable(true)]
        public Point CornerBR
        {
            get
            {
                EnsureCornerOrientation();
                return cornerBR;
            }
        }

        public void SetCornerBR(Point value, bool setByUser)
        {
            EnsureCornerOrientation();

            cornerBR = value;

            cache.InvalidatePrefixed(SourceId + ":Post:"); // invalidate old annotated image

            if (setByUser)
            {
                normalizeGeometryExplicitlySet = true;

                ResetAnalyzeTask(false/*invalidateCurrentView*/);
                StartInit();
            }

            FirePropertyChanged("CornerBR");
        }

        [Bindable(true)]
        public bool NormalizeGeometryExplicitlySet
        {
            get
            {
                return normalizeGeometryExplicitlySet;
            }
            set
            {
                if (this.normalizeGeometryExplicitlySet != value)
                {
                    this.normalizeGeometryExplicitlySet = value;

                    if (!value)
                    {
                        ResetAnalyzeTask(false/*invalidateCurrentView*/);
                        StartInit();
                    }

                    FirePropertyChanged("NormalizeGeometryExplicitlySet");
                }
            }
        }

        [Bindable(true)]
        public float? NormalizeGeometryForcedAspectRatio
        {
            get
            {
                return this.normalizedGeometryAspectRatio.HasValue ? this.normalizedGeometryAspectRatio.Value : options.AutoNormalizeGeometryAspectRatio;
            }
            set
            {
                this.normalizedGeometryAspectRatio = value;

                ResetAnalyzeTask(false/*invalidateCurrentView*/);
                StartInit();

                FirePropertyChanged("NormalizeGeometryForcedAspectRatio");
            }
        }

        [Bindable(true)]
        public Transforms.InterpMethod NormalizeGeometryPreviewInterp { get { return options.NormalizeGeometryPreviewInterp; } }

        [Bindable(true)]
        public Transforms.InterpMethod NormalizeGeometryFinalInterp { get { return options.NormalizeGeometryFinalInterp; } }

        [Bindable(true)]
        public double FineRotateDegrees
        {
            get
            {
                return fineRotateDegrees;
            }
            set
            {
                fineRotateDegrees = value;

                ResetAnalyzeTask(true/*invalidateCurrentView*/);
                StartInit();

                FirePropertyChanged("FineRotationDegrees");
            }
        }

        [Bindable(true)]
        public string AnalysisMessage
        {
            get
            {
                return analysisMessage;
            }
            private set
            {
                analysisMessage = value;

                FirePropertyChanged("Message"); // not "AnalysisMessage"
            }
        }

        [Bindable(true)]
        public string Message
        {
            get
            {
                return String.Join(" ", analysisMessage, message);
            }
            private set
            {
                message = value;

                FirePropertyChanged("Message");
            }
        }

        public Color TagColor
        {
            get
            {
                return tagColor;
            }
            set
            {
                Debug.Assert(value.IsNamedColor);
                tagColor = value;

                FirePropertyChanged("TagColor");
            }
        }


        // providers

        public string SourceId { get { return SourceFileName; } }

        private ManagedBitmap GetSourceBitmap(Profile profile)
        {
            if (profile == null)
            {
                profile = new Profile();
            }

            profile.Push("Item.GetSourceBitmap");
            ManagedBitmap result = ImageClient.LoadAndOrientGDI(profile, SourcePath, RightRotations, out exifOrientation);
            profile.Pop();

            return result;
        }

        private ManagedBitmap GetSourceBitmap()
        {
            return GetSourceBitmap(null);
        }

        public BitmapHolder GetSourceBitmapHolder()
        {
            return cache.Query(
                SourceId,
                GetSourceBitmap);
        }

        private string GetSourceBitmapHolderWithNormalizeGeometryId()
        {
            return (NormalizeGeometry || (FineRotateDegrees != 0)) ? String.Format("{0}:NormalizeGeometry", SourceId) : SourceId;
        }

        public BitmapHolder GetSourceBitmapHolderWithNormalizeGeometry()
        {
            if (!NormalizeGeometry && (this.FineRotateDegrees == 0))
            {
                Debug.Assert(String.Equals(GetSourceBitmapHolderWithNormalizeGeometryId(), SourceId));
                return GetSourceBitmapHolder();
            }
            else
            {
                Debug.Assert(!String.Equals(GetSourceBitmapHolderWithNormalizeGeometryId(), SourceId));
                return cache.Query(
                    GetSourceBitmapHolderWithNormalizeGeometryId(),
                    delegate ()
                    {
                        ManagedBitmap result;

                        using (BitmapHolder source = GetSourceBitmapHolder())
                        {
                            result = source.Bitmap.Clone();
                            Transforms.ApplyNormalizeGeometry(
                                new Profile(),
                                this.SourceId,
                                result,
                                1,
                                new Transforms.NormalizeGeometryParameters(this.CornerTL, this.CornerTR, this.CornerBL, this.CornerBR, this.NormalizeGeometryForcedAspectRatio, this.FineRotateDegrees, this.NormalizeGeometryPreviewInterp),
                                null,
                                new CancellationTokenSource());
                        }

                        return result;
                    });
            }
        }

        private string GetPreviewId(float shrinkExpandFactor, bool inDrag, bool showNormalizedGeometry)
        {
            return String.Format("{0}:Post:{1}(shrinkExpand={2}, inDrag={3}, normalizeGeometry={4})", SourceFileName, "Preview", shrinkExpandFactor, inDrag, showNormalizedGeometry);
        }

        public BitmapHolder GetPreviewBitmapHolder(bool shrinkExpand, bool inDrag, bool showNormalizedGeometry)
        {
            float effectiveShrinkExpandFactor = shrinkExpand && !inDrag ? this.ShrinkFactor : 1;
            bool effectiveShowNormalizedGeometry = (this.NormalizeGeometry || (this.FineRotateDegrees != 0)) && showNormalizedGeometry;

            string previewId = GetPreviewId(effectiveShrinkExpandFactor, inDrag, effectiveShowNormalizedGeometry);

            return cache.Query(
                previewId,
                delegate ()
                {
                    ManagedBitmap result;

                    Profile profile = new Profile("Item.GetPreviewBitmapHolder {0}", this.SourceId);

                    profile.Push("GetSourceBitmapHolder");
                    using (BitmapHolder sourceHolder = this.GetSourceBitmapHolder())
                    {
                        profile.Pop();

                        string message = String.Empty;
                        Transforms.AddMessageMethod addMessage = delegate (string text) { message = String.Concat(message, !String.IsNullOrEmpty(message) ? " " : null, text); };

                        Transforms.PolyUnbiasDiagnostics polyUnbiasDiagnostics;
                        SmartBitmap bitmap = Transforms.GetPreviewImageCore(
                            profile,
                            cache,
                            this.SourceId,
                            sourceHolder,
                            effectiveShrinkExpandFactor,
                            inDrag ? AnnotationDivisor : 1,
                            hf,
                            this.CropRect,
                            effectiveShowNormalizedGeometry,
                            new Transforms.NormalizeGeometryParameters(this.CornerTL, this.CornerTR, this.CornerBL, this.CornerBR, this.NormalizeGeometryForcedAspectRatio, this.FineRotateDegrees, this.NormalizeGeometryPreviewInterp),
                            !inDrag && this.BrightAdjust,
                            new Transforms.BrightAdjustParameters(this.BrightAdjustMinClusterFrac, this.BrightAdjustWhiteCorrect),
                            !inDrag && this.Unbias,
                            new Transforms.PolyUnbiasParameters(this.UnbiasMaxDegree, this.UnbiasMaxChisq),
                            !inDrag && this.StaticSaturate,
                            new Transforms.StaticSaturateParameters(this.StaticSaturateWhiteThreshhold, this.StaticSaturateBlackThreshhold, this.StaticSaturateExponent),
                            !inDrag && this.OneBit,
                            new Transforms.OneBitParameters(this.OneBitChannel, this.OneBitThreshhold, this.OneBitScaleUp),
                            addMessage,
                            out polyUnbiasDiagnostics);

                        this.Message = message;
                        this.polyUnbiasDiagnostics = polyUnbiasDiagnostics;
                        result = bitmap.AsManagedDetach(profile);
                    }

                    profile.End();
                    Program.Log(LogCat.Perf, profile.Report());

                    return result;
                });
        }

        private string GetAnnotatedId(bool showHF, bool inDrag, bool showNormalizedGeometry, bool showAutoCropGrid, bool showPolyUnbiasGrid, bool showCropDragPads)
        {
            return String.Format("{0}:Post:{1}(showHF={2}, inDrag={3}, normalizedGeometry={4}, autoCropGrid={5}, polyUnbiasGrid={6}, showCropDragPads={7})", SourceFileName, "Annotated", showHF, inDrag, showNormalizedGeometry, showAutoCropGrid, showPolyUnbiasGrid, showCropDragPads);
        }

        public BitmapHolder GetAnnotatedBitmapHolder(bool shrinkExpand, bool inDrag, bool showHF, bool showNormalizedGeometry, bool showAutoCropGrid, bool showPolyUnbiasGrid, bool showCropDragPads)
        {
            bool effectiveShowNormalizedGeometry = (this.NormalizeGeometry || (this.FineRotateDegrees != 0)) && showNormalizedGeometry;

            string annotatedId = GetAnnotatedId(showHF, inDrag, effectiveShowNormalizedGeometry, showAutoCropGrid, showPolyUnbiasGrid, showCropDragPads);

            return cache.Query(
                annotatedId,
                delegate ()
                {
                    ManagedBitmap result;

                    Profile profile = new Profile("GetAnnotatedBitmapHolder {0}", this.SourceId);

                    profile.Push("Item.GetSourceBitmapHolder");
                    using (BitmapHolder sourceHolder = this.GetSourceBitmapHolder())
                    {
                        profile.Pop();
                        profile.Push("Item.GetPreviewBitmapHolder");
                        using (BitmapHolder previewHolder = this.GetPreviewBitmapHolder(
                            shrinkExpand,
                            inDrag,
                            effectiveShowNormalizedGeometry))
                        {
                            profile.Pop();
                            SmartBitmap bitmap = Transforms.GetAnnotatedImage(
                                profile,
                                this.SourceId,
                                sourceHolder,
                                previewHolder,
                                this.ShrinkFactor,
                                inDrag ? AnnotationDivisor : 1,
                                showHF ? this.hf : null,
                                showHF && showAutoCropGrid ? this.autoCropGrid : null,
                                showHF && showPolyUnbiasGrid ? this.polyUnbiasDiagnostics : null,
                                this.CropRect,
                                !effectiveShowNormalizedGeometry && this.NormalizeGeometry
                                    ? new Point[] { this.CornerTL, this.CornerTR, this.CornerBL, this.CornerBR }
                                    : null,
                                showCropDragPads);
                            result = bitmap.AsManagedDetach(profile);
                        }
                    }

                    profile.End();
                    Program.Log(LogCat.Perf, profile.Report());

                    return result;
                });
        }


        // construction

        public Item(string targetPath, GlobalOptions options, ImageCache cache)
        {
            this.targetPath = targetPath;
            this.options = options;
            this.cache = cache;

            this.propertyChangedDelegate = this.FirePropertyChanged_MainThread;

            ResetAnalyzeTask(false/*invalidateCurrentView*/);
        }

        public void ResetAnalyzeTask(bool invalidateCurrentView)
        {
            this.hf = null;
            this.autoCropGrid = null;
            this.polyUnbiasDiagnostics = null;

            if (this.validateTask != null)
            {
                this.validateTaskCancel.Cancel();

                this.validateTask = null;
                this.validateTaskCancel = null;
            }

            this.permitCurrentViewDuringReanalysis = !invalidateCurrentView;

            CancellationTokenSource localValidateTaskCancel = this.validateTaskCancel = new CancellationTokenSource();
            this.validateTask = new Task<bool>(
                delegate ()
                {
                    bool valid = false;

                    ThreadPriority savedPriority = Thread.CurrentThread.Priority;
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                    try
                    {
                        if (invalidateCurrentView)
                        {
                            cache.InvalidatePrefixed(SourceId + ":"); // invalidate all but original source image
                        }

                        using (SmartBitmap source = this.ValidateAndGetSourceImage())
                        {
                            if (source != null)
                            {
                                if ((cornerTL == Point.Empty) && (cornerTR == Point.Empty) && (cornerBL == Point.Empty) && (cornerBR == Point.Empty))
                                {
                                    //cornerTL = Point.Empty;
                                    cornerTR = new Point(source.Width, 0);
                                    cornerBL = new Point(0, source.Height);
                                    cornerBR = new Point(source.Width, source.Height);
                                }

                                string message = String.Empty;
                                Transforms.AddMessageMethod addMessage = delegate (string text) { message = String.Concat(message, !String.IsNullOrEmpty(message) ? " " : null, text); };

                                Transforms.Analyze(this, source, options, addMessage, localValidateTaskCancel);
                                valid = true;

                                this.AnalysisMessage = message;
                            }
                        }

                        cache.InvalidatePrefixed(SourceId + ":");
                    }
                    catch (OperationCanceledException)
                    {
                        // parameters changed - someone cancelled this analysis task and started a new one
                    }
                    finally
                    {
                        Thread.CurrentThread.Priority = savedPriority;
                    }

                    this.status = valid ? Status.Valid : Status.Invalid;

                    this.permitCurrentViewDuringReanalysis = false;
                    FirePropertyChanged("Status");

                    return false;
                });

            this.status = Status.Pending;

            // not invalidating first == permit current view to remain until new analysis is complete
            if (!invalidateCurrentView)
            {
                FirePropertyChanged("Status");
            }
        }

        public void StartInit()
        {
            if (this.validateTask.Status == TaskStatus.Created)
            {
                this.validateTask.Start();
            }
        }

        public void WaitInit()
        {
            if (this.validateTask.Status == TaskStatus.Created)
            {
                this.validateTask.Start();
            }
            this.validateTask.Wait();
        }

        public SmartBitmap ValidateAndGetSourceImage()
        {
            try
            {
                // avoid trying to load obviously inappropriate files
                Transforms.SanityCheckJpegFormatFileThrow(this.SourcePath);

                // task to compute hash signature for finding identical files
                Task<string> hashTask = new Task<string>(
                    delegate ()
                    {
                        byte[] hashBytes;
                        using (Stream stream = new FileStream(this.SourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            hashBytes = SHA512.Create().ComputeHash(stream);
                        }
                        return Convert.ToBase64String(hashBytes);
                    });
                hashTask.Start();

                // ensure image is loadable
                SmartBitmap bitmap = new SmartBitmap(GetSourceBitmap()); // could throw if file invalid, despite above sanity check
                this.width = bitmap.Width;
                this.height = bitmap.Height;

                hashTask.Wait();
                this.hash = hashTask.Result;

                // see if we must read settings now (file was renamed, but we now have the hash to find old settings)
                if (this.settingsNav != null)
                {
                    XPathNavigator nav = settingsNav.SelectSingleNode(String.Format("/*/items/item[hash=\"{0}\"]", this.hash));
                    if (nav != null)
                    {
                        ReadXml(nav);
                    }
                    settingsNav = null;
                }

                return bitmap;
            }
            catch (Exception)
            {
            }
            return null;
        }

        [Bindable(false)]
        public bool[,] HF
        {
            get
            {
                return hf;
            }
            set
            {
                hf = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");
            }
        }

        [Bindable(false)]
        public bool[,] AutoCropGrid
        {
            get
            {
                return autoCropGrid;
            }
            set
            {
                autoCropGrid = value;
                cache.InvalidatePrefixed(SourceId + ":Post:");
            }
        }

        [Bindable(false)]
        public Transforms.PolyUnbiasDiagnostics PolyUnbiasDiagnostics
        {
            get
            {
                return polyUnbiasDiagnostics;
            }
            set
            {
                polyUnbiasDiagnostics = value;
                //cache.InvalidatePrefixed(SourceId + ":Post:");
            }
        }


        // serialization

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("item");

            writer.WriteStartElement("file");
            writer.WriteValue(Path.GetFileName(this.targetPath));
            writer.WriteEndElement(); // file

            if (!String.IsNullOrEmpty(this.rename))
            {
                writer.WriteStartElement("rename");
                writer.WriteValue(this.rename);
                writer.WriteEndElement(); // rename
            }
            if (this.tagColor != Color.Black)
            {
                writer.WriteStartElement("tagColor");
                writer.WriteValue((string)this.tagColor.Name);
                writer.WriteEndElement(); // tagColor
            }
            if (!String.IsNullOrEmpty(this.hash))
            {
                writer.WriteStartElement("hash");
                writer.WriteValue(this.hash);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("actions");
            {
                if (this.jpegQuality.HasValue)
                {
                    writer.WriteStartElement("jpegQuality");
                    writer.WriteValue(this.jpegQuality.Value);
                    writer.WriteEndElement(); // jpegQuality
                }
                if (this.jpegUseGdi.HasValue)
                {
                    writer.WriteStartElement("jpegUseGdi");
                    writer.WriteValue(this.jpegUseGdi.Value);
                    writer.WriteEndElement(); // jpegUseGdi
                }

                if (this.shrink.HasValue)
                {
                    writer.WriteStartElement("shrink");
                    writer.WriteValue(this.shrink.Value);
                    writer.WriteEndElement(); // shrink
                }
                if (this.shrinkFactor.HasValue)
                {
                    writer.WriteStartElement("shrinkFactor");
                    writer.WriteValue(this.shrinkFactor.Value);
                    writer.WriteEndElement(); // shrinkFactor
                }

                writer.WriteStartElement("delete");
                writer.WriteValue(this.delete);
                writer.WriteEndElement(); // delete

                if (!this.cropRect.IsEmpty && (CropRect != new Rectangle(Point.Empty, new Size(this.width, this.height))))
                {
                    KeyValuePair<string, int>[] cropFields = new KeyValuePair<string, int>[]
                    {
                        new KeyValuePair<string, int>("left", this.cropRect.Left),
                        new KeyValuePair<string, int>("top", this.cropRect.Top),
                        new KeyValuePair<string, int>("width", this.cropRect.Width),
                        new KeyValuePair<string, int>("height", this.cropRect.Height),
                        new KeyValuePair<string, int>("rotation", this.cropRectRotation),
                    };
                    writer.WriteStartElement("crop");
                    foreach (KeyValuePair<string, int> field in cropFields)
                    {
                        writer.WriteStartElement(field.Key);
                        writer.WriteValue(field.Value);
                        writer.WriteEndElement();
                    }
                    if (this.cropRectExplicitlySet)
                    {
                        writer.WriteStartElement("setByUser");
                        writer.WriteValue(this.cropRectExplicitlySet);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); // crop
                }

                if (rightRotations.HasValue)
                {
                    writer.WriteStartElement("rotate");
                    writer.WriteValue(this.rightRotations.Value);
                    writer.WriteEndElement(); // rotate
                }

                if (brightAdjust.HasValue || brightAdjustMinClusterFrac.HasValue || brightAdjustWhiteCorrect.HasValue)
                {
                    writer.WriteStartElement("brightAdjust");
                    if (brightAdjust.HasValue)
                    {
                        writer.WriteStartElement("enable");
                        writer.WriteValue(this.brightAdjust.Value);
                        writer.WriteEndElement(); // enable
                    }
                    if (brightAdjustMinClusterFrac.HasValue)
                    {
                        writer.WriteStartElement("minClusterFrac");
                        writer.WriteValue(this.brightAdjustMinClusterFrac.Value);
                        writer.WriteEndElement(); // minClusterFrac
                    }
                    if (brightAdjustWhiteCorrect.HasValue)
                    {
                        writer.WriteStartElement("whiteCorrect");
                        writer.WriteValue(this.brightAdjustWhiteCorrect.Value);
                        writer.WriteEndElement(); // whiteCorrect
                    }
                    writer.WriteEndElement(); // brightAdjust
                }

                if (oneBit.HasValue || oneBitChannel.HasValue || oneBitThreshhold.HasValue)
                {
                    writer.WriteStartElement("oneBit");
                    if (oneBit.HasValue)
                    {
                        writer.WriteStartElement("enable");
                        writer.WriteValue(this.oneBit.Value);
                        writer.WriteEndElement(); // enable
                    }
                    if (oneBitChannel.HasValue)
                    {
                        writer.WriteStartElement("channel");
                        writer.WriteValue((int)this.oneBitChannel.Value);
                        writer.WriteEndElement(); // channel
                    }
                    if (oneBitThreshhold.HasValue)
                    {
                        writer.WriteStartElement("threshhold");
                        writer.WriteValue(this.oneBitThreshhold.Value);
                        writer.WriteEndElement(); // threshhold
                    }
                    writer.WriteEndElement(); // oneBit
                }

                if (this.staticSaturate.HasValue || this.staticSaturateWhiteThreshhold.HasValue
                    || this.staticSaturateBlackThreshhold.HasValue || this.staticSaturateExponent.HasValue)
                {
                    writer.WriteStartElement("staticSaturate");
                    if (staticSaturate.HasValue)
                    {
                        writer.WriteStartElement("enable");
                        writer.WriteValue(this.staticSaturate.Value);
                        writer.WriteEndElement(); // enable
                    }
                    if (staticSaturateWhiteThreshhold.HasValue)
                    {
                        writer.WriteStartElement("whiteThreshhold");
                        writer.WriteValue((int)this.staticSaturateWhiteThreshhold.Value);
                        writer.WriteEndElement(); // whiteThreshhold
                    }
                    if (staticSaturateBlackThreshhold.HasValue)
                    {
                        writer.WriteStartElement("blackThreshhold");
                        writer.WriteValue(this.staticSaturateBlackThreshhold.Value);
                        writer.WriteEndElement(); // blackThreshhold
                    }
                    if (staticSaturateExponent.HasValue)
                    {
                        writer.WriteStartElement("exponent");
                        writer.WriteValue(this.staticSaturateExponent.Value);
                        writer.WriteEndElement(); // exponent
                    }
                    writer.WriteEndElement(); // staticSaturate
                }

                writer.WriteStartElement("normalizeGeometry");
                //
                writer.WriteStartElement("enable");
                writer.WriteValue(this.normalizeGeometry);
                writer.WriteEndElement(); // enable
                //
                writer.WriteStartElement("setByUser");
                writer.WriteValue(this.normalizeGeometryExplicitlySet);
                writer.WriteEndElement(); // setByUser
                //
                writer.WriteStartElement("cornerTL");
                writer.WriteStartElement("x");
                writer.WriteValue(this.cornerTL.X);
                writer.WriteEndElement(); // x
                writer.WriteStartElement("y");
                writer.WriteValue(this.cornerTL.Y);
                writer.WriteEndElement(); // y
                writer.WriteEndElement(); // cornerTL
                //
                writer.WriteStartElement("cornerTR");
                writer.WriteStartElement("x");
                writer.WriteValue(this.cornerTR.X);
                writer.WriteEndElement(); // x
                writer.WriteStartElement("y");
                writer.WriteValue(this.cornerTR.Y);
                writer.WriteEndElement(); // y
                writer.WriteEndElement(); // cornerTR
                //
                writer.WriteStartElement("cornerBL");
                writer.WriteStartElement("x");
                writer.WriteValue(this.cornerBL.X);
                writer.WriteEndElement(); // x
                writer.WriteStartElement("y");
                writer.WriteValue(this.cornerBL.Y);
                writer.WriteEndElement(); // y
                writer.WriteEndElement(); // cornerBL
                //
                writer.WriteStartElement("cornerBR");
                writer.WriteStartElement("x");
                writer.WriteValue(this.cornerBR.X);
                writer.WriteEndElement(); // x
                writer.WriteStartElement("y");
                writer.WriteValue(this.cornerBR.Y);
                writer.WriteEndElement(); // y
                writer.WriteEndElement(); // cornerBR
                //
                writer.WriteStartElement("rotation");
                writer.WriteValue(this.cornerRotation);
                writer.WriteEndElement(); // rotation
                //
                writer.WriteEndElement(); // normalizeGeometry

                writer.WriteStartElement("fineRotate");
                //
                writer.WriteStartElement("fineRotateDegrees");
                writer.WriteValue(this.fineRotateDegrees);
                writer.WriteEndElement(); // fineRotateDegrees
                //
                writer.WriteEndElement(); // fineRotate
            }
            writer.WriteEndElement(); // actions

            writer.WriteEndElement(); // item
        }

        public static bool ReadValue<T>(XPathNavigator nav, string xpath, out T value)
        {
            nav = nav.SelectSingleNode(xpath);
            if (nav != null)
            {
                value = (T)nav.ValueAs(typeof(T));
                return true;
            }
            value = default(T);
            return false;
        }

        public void ReadXml(XPathNavigator item)
        {
            bool b;
            int i;
            float f;
            string s;
            double d;

            if (ReadValue(item, "rename", out s))
            {
                this.rename = s;
            }
            if (ReadValue(item, "tagColor", out s))
            {
                this.tagColor = Color.FromName(s);
            }
            if (ReadValue(item, "hash", out s))
            {
                this.hash = s;
            }

            if (ReadValue(item, "actions/jpegQuality", out i))
            {
                this.jpegQuality = i;
            }
            if (ReadValue(item, "actions/jpegUseGdi", out b))
            {
                this.jpegUseGdi = b;
            }

            if (ReadValue(item, "actions/shrink", out b))
            {
                this.shrink = b;
            }
            if (ReadValue(item, "actions/shrinkFactor", out f))
            {
                this.shrinkFactor = f;
            }

            if (ReadValue(item, "actions/delete", out b))
            {
                this.delete = b;
            }

            int left, top, width, height, cropRotation;
            if (ReadValue(item, "actions/crop/left", out left)
                && ReadValue(item, "actions/crop/top", out top)
                && ReadValue(item, "actions/crop/width", out width)
                && ReadValue(item, "actions/crop/height", out height)
                && ReadValue(item, "actions/crop/rotation", out cropRotation))
            {
                this.cropRect = new Rectangle(left, top, width, height);
                this.cropRectRotation = cropRotation;
                if (ReadValue(item, "actions/crop/setByUser", out b))
                {
                    this.cropRectExplicitlySet = b;
                }
            }

            if (ReadValue(item, "actions/rotate", out i))
            {
                this.rightRotations = i;
            }

            if (ReadValue(item, "actions/brightAdjust/enable", out b))
            {
                this.brightAdjust = b;
            }
            if (ReadValue(item, "actions/brightAdjust/minClusterFrac", out f))
            {
                this.brightAdjustMinClusterFrac = f;
            }
            if (ReadValue(item, "actions/brightAdjust/whiteCorrect", out b))
            {
                this.brightAdjustWhiteCorrect = b;
            }

            if (ReadValue(item, "actions/oneBit/enable", out b))
            {
                this.oneBit = b;
            }
            if (ReadValue(item, "actions/oneBit/channel", out i))
            {
                this.oneBitChannel = (Transforms.Channel)i;
            }
            if (ReadValue(item, "actions/oneBit/threshhold", out f))
            {
                this.oneBitThreshhold = f;
            }

            if (ReadValue(item, "actions/staticSaturate/enable", out b))
            {
                this.staticSaturate = b;
            }
            if (ReadValue(item, "actions/staticSaturate/whiteThreshhold", out f))
            {
                this.staticSaturateWhiteThreshhold = f;
            }
            if (ReadValue(item, "actions/staticSaturate/blackThreshhold", out f))
            {
                this.staticSaturateBlackThreshhold = f;
            }
            if (ReadValue(item, "actions/staticSaturate/exponent", out f))
            {
                this.staticSaturateExponent = f;
            }

            if (ReadValue(item, "actions/normalizeGeometry/enable", out b))
            {
                this.normalizeGeometry = b;
            }
            if (ReadValue(item, "actions/normalizeGeometry/setByUser", out b))
            {
                this.normalizeGeometryExplicitlySet = b;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerTL/x", out i))
            {
                this.cornerTL.X = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerTL/y", out i))
            {
                this.cornerTL.Y = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerTR/x", out i))
            {
                this.cornerTR.X = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerTR/y", out i))
            {
                this.cornerTR.Y = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerBL/x", out i))
            {
                this.cornerBL.X = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerBL/y", out i))
            {
                this.cornerBL.Y = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerBR/x", out i))
            {
                this.cornerBR.X = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/cornerBR/y", out i))
            {
                this.cornerBR.Y = i;
            }
            if (ReadValue(item, "actions/normalizeGeometry/rotation", out i))
            {
                this.cornerRotation = i;
            }

            if (ReadValue(item, "actions/fineRotate/fineRotateDegrees", out d))
            {
                this.fineRotateDegrees = d;
            }
        }

        public XPathNavigator SettingsNav { set { this.settingsNav = value; } }
    }
}
