/*
 *  Copyright © 2010-2017 Thomas R. Lawrence
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
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace AdaptiveImageSizeReducer
{
    public partial class GlobalOptionsDialog : Form
    {
        private readonly GlobalOptions originalOptions;
        private readonly GlobalOptions options;
        private readonly Bitmap samplePreview;

        public GlobalOptionsDialog(GlobalOptions originalOptions, string directory)
        {
            this.originalOptions = originalOptions;
            this.options = new GlobalOptions(originalOptions);

            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;

            AssemblyFileVersionAttribute versionAttribute = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
            string version = versionAttribute.Version;
            version = version.Substring(0, version.LastIndexOf('.'));
            labelVersion.Text = String.Format("{0} {1}", labelVersion.Text, version);

            globalOptionsBindingSource.Add(this.options);

            SelectRightRotation(this.options.RightRotations, false/*updateSource*/);
            SelectOneBitChannel(this.options.OneBitChannel, false/*updateSource*/);
            SelectOneBitFormat(this.options.OneBitOutputFormat, false/*updateSource*/);
            SelectJpegEncoder(this.options.JpegUseGDI, false/*updateSource*/);
            SelectTimestamps(this.options.Timestamps, false/*updateSource*/);
            SelectTimestampsCreatedModified(this.options.TimestampsExifMissingModifiedInsteadOfCreated, false/*updateSource*/);

            this.comboBoxNormalizeGeometryPreviewResizeMethod.SelectedIndex = (int)this.options.NormalizeGeometryPreviewInterp;
            this.comboBoxNormalizeGeometryFinalResizeMethod.SelectedIndex = (int)this.options.NormalizeGeometryFinalInterp;

            string[] files = Directory.GetFiles(Program.GetScanDirectoryFromTargetDirectory(directory));
            string sampleFile = Array.Find(
                files,
                delegate (string candidate)
                {
                    return !String.Equals(candidate, Program.SettingsFile) && Transforms.SanityCheckValidImageFormatFile(candidate);
                });
            if (!String.IsNullOrEmpty(sampleFile) && File.Exists(sampleFile))
            {
                try
                {
                    samplePreview = Transforms.LoadAndOrientGDI(sampleFile);
                    pictureBox.Image = samplePreview;
                }
                catch (Exception)
                {
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (samplePreview != null)
            {
                samplePreview.Dispose();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.options.NormalizeGeometryPreviewInterp = (Transforms.InterpMethod)this.comboBoxNormalizeGeometryPreviewResizeMethod.SelectedIndex;
            this.options.NormalizeGeometryFinalInterp = (Transforms.InterpMethod)this.comboBoxNormalizeGeometryFinalResizeMethod.SelectedIndex;

            this.originalOptions.CopyFrom(this.options);
            DialogResult = DialogResult.OK;
            Close();
        }

        //

        private int lockOutRightRotation;
        private void SelectRightRotation(int rotation, bool updateSource)
        {
            if (lockOutRightRotation != 0)
            {
                return;
            }

            try
            {
                lockOutRightRotation++;

                radioButtonRotation0.Checked = false;
                radioButtonRotation90.Checked = false;
                radioButtonRotation180.Checked = false;
                radioButtonRotation270.Checked = false;

                if (updateSource)
                {
                    options.RightRotations = rotation;
                }
                switch (rotation)
                {
                    default:
                        Debug.Assert(false);
                        break;
                    case 0:
                        radioButtonRotation0.Checked = true;
                        break;
                    case 1:
                        radioButtonRotation90.Checked = true;
                        break;
                    case 2:
                        radioButtonRotation180.Checked = true;
                        break;
                    case 3:
                        radioButtonRotation270.Checked = true;
                        break;
                }
            }
            finally
            {
                lockOutRightRotation--;
            }
        }

        private void radioButtonRotation0_CheckedChanged(object sender, EventArgs e)
        {
            SelectRightRotation(0, true/*updateSource*/);
        }

        private void radioButtonRotation90_CheckedChanged(object sender, EventArgs e)
        {
            SelectRightRotation(1, true/*updateSource*/);
        }

        private void radioButtonRotation180_CheckedChanged(object sender, EventArgs e)
        {
            SelectRightRotation(2, true/*updateSource*/);
        }

        private void radioButtonRotation270_CheckedChanged(object sender, EventArgs e)
        {
            SelectRightRotation(3, true/*updateSource*/);
        }

        //

        private int lockOutOneBitChannel;
        private void SelectOneBitChannel(Transforms.Channel channel, bool updateSource)
        {
            if (lockOutOneBitChannel != 0)
            {
                return;
            }

            try
            {
                lockOutOneBitChannel++;

                radioButtonOneBitChannelAll.Checked = false;
                radioButtonOneBitChannelR.Checked = false;
                radioButtonOneBitChannelG.Checked = false;
                radioButtonOneBitChannelB.Checked = false;

                if (updateSource)
                {
                    options.OneBitChannel = channel;
                }
                switch (channel)
                {
                    default:
                        Debug.Assert(false);
                        break;
                    case Transforms.Channel.Composite:
                        radioButtonOneBitChannelAll.Checked = true;
                        break;
                    case Transforms.Channel.R:
                        radioButtonOneBitChannelR.Checked = true;
                        break;
                    case Transforms.Channel.G:
                        radioButtonOneBitChannelG.Checked = true;
                        break;
                    case Transforms.Channel.B:
                        radioButtonOneBitChannelB.Checked = true;
                        break;
                }
            }
            finally
            {
                lockOutOneBitChannel--;
            }
        }

        private void radioButtonOneBitChannelB_CheckedChanged(object sender, EventArgs e)
        {
            SelectOneBitChannel(Transforms.Channel.B, true/*updateSource*/);
        }

        private void radioButtonOneBitChannelG_CheckedChanged(object sender, EventArgs e)
        {
            SelectOneBitChannel(Transforms.Channel.G, true/*updateSource*/);
        }

        private void radioButtonOneBitChannelR_CheckedChanged(object sender, EventArgs e)
        {
            SelectOneBitChannel(Transforms.Channel.R, true/*updateSource*/);
        }

        private void radioButtonOneBitChannelAll_CheckedChanged(object sender, EventArgs e)
        {
            SelectOneBitChannel(Transforms.Channel.Composite, true/*updateSource*/);
        }

        //

        private int lockOutOneBitFormat;
        private void SelectOneBitFormat(OutputTransforms.OutputFormat format, bool updateSource)
        {
            if (lockOutOneBitFormat != 0)
            {
                return;
            }

            try
            {
                lockOutOneBitFormat++;

                radioButtonOneBitOutputPng.Checked = false;
                radioButtonOneBitOutputBmp.Checked = false;

                if (updateSource)
                {
                    options.OneBitOutputFormat = format;
                }
                switch (format)
                {
                    default:
                        Debug.Assert(false);
                        break;
                    case OutputTransforms.OutputFormat.Bmp:
                        radioButtonOneBitOutputBmp.Checked = true;
                        break;
                    case OutputTransforms.OutputFormat.Png:
                        radioButtonOneBitOutputPng.Checked = true;
                        break;
                }
            }
            finally
            {
                lockOutOneBitFormat--;
            }
        }

        private void radioButtonOneBitOutputBmp_CheckedChanged(object sender, EventArgs e)
        {
            SelectOneBitFormat(OutputTransforms.OutputFormat.Bmp, true/*updateSource*/);
        }

        private void radioButtonOneBitOutputPng_CheckedChanged(object sender, EventArgs e)
        {
            SelectOneBitFormat(OutputTransforms.OutputFormat.Png, true/*updateSource*/);
        }

        //

        private int lockOutJpegEncoder;
        private void SelectJpegEncoder(bool useGdiEncoder, bool updateSource)
        {
            if (lockOutJpegEncoder != 0)
            {
                return;
            }

            try
            {
                lockOutJpegEncoder++;

                radioButtonJpegEncoderGDI.Checked = useGdiEncoder;
                radioButtonJpegEncoderWPF.Checked = !useGdiEncoder;

                if (updateSource)
                {
                    options.JpegUseGDI = useGdiEncoder;
                }
            }
            finally
            {
                lockOutJpegEncoder--;
            }
        }

        private void radioButtonJpegEncoderGDI_CheckedChanged(object sender, EventArgs e)
        {
            SelectJpegEncoder(true/*useGdiEncoder*/, true/*updateSource*/);
        }

        private void radioButtonJpegEncoderWPF_CheckedChanged(object sender, EventArgs e)
        {
            SelectJpegEncoder(false/*useGdiEncoder*/, true/*updateSource*/);
        }

        private int lockoutTimestamps;
        private void SelectTimestamps(bool? timestamps, bool updateSource)
        {
            if (lockoutTimestamps != 0)
            {
                return;
            }

            try
            {
                lockoutTimestamps++;

                radioButtonTimestampDoNothing.Checked = !timestamps.HasValue;
                radioButtonTimestampAdd.Checked = timestamps.HasValue && timestamps.Value;
                radioButtonTimestampRemove.Checked = timestamps.HasValue && !timestamps.Value;

                if (updateSource)
                {
                    options.Timestamps = timestamps;
                }
            }
            finally
            {
                lockoutTimestamps--;
            }
        }

        private void radioButtonTimestampDoNothing_CheckedChanged(object sender, EventArgs e)
        {
            SelectTimestamps(null/*timestamps*/, true/*updateSource*/);
        }

        private void radioButtonTimestampAdd_CheckedChanged(object sender, EventArgs e)
        {
            SelectTimestamps(true/*timestamps*/, true/*updateSource*/);
        }

        private void radioButtonTimestampRemove_CheckedChanged(object sender, EventArgs e)
        {
            SelectTimestamps(false/*timestamps*/, true/*updateSource*/);
        }

        private int lockoutTimestampsCreatedModified;
        private void SelectTimestampsCreatedModified(bool timestampsExifMissingModifiedInsteadOfCreated, bool updateSource)
        {
            if (lockoutTimestampsCreatedModified != 0)
            {
                return;
            }

            try
            {
                lockoutTimestampsCreatedModified++;

                radioButtonTimestampFileCreated.Checked = !timestampsExifMissingModifiedInsteadOfCreated;
                radioButtonTimestampFileLastModified.Checked = timestampsExifMissingModifiedInsteadOfCreated;

                if (updateSource)
                {
                    options.TimestampsExifMissingModifiedInsteadOfCreated = timestampsExifMissingModifiedInsteadOfCreated;
                }
            }
            finally
            {
                lockoutTimestampsCreatedModified--;
            }
        }

        private void radioButtonTimestampFileCreated_CheckedChanged(object sender, EventArgs e)
        {
            SelectTimestampsCreatedModified(false/*timestampsExifMissingModifiedInsteadOfCreated*/ , true/*updateSource*/);
        }

        private void radioButtonTimestampFileLastModified_CheckedChanged(object sender, EventArgs e)
        {
            SelectTimestampsCreatedModified(true/*timestampsExifMissingModifiedInsteadOfCreated*/ , true/*updateSource*/);
        }
    }

    public class GlobalOptions
    {
        private int jpegQuality = 95;
        private bool jpegUseGDI = false;

        private bool shrink;
        private float shrinkFactor = 2f;

        private bool autoCrop;
        private float autoCropLeftLimit = .25f, autoCropTopLimit = .25f, autoCropRightLimit = .25f, autoCropBottomLimit = .25f;
        private float autoCropMinMedianBrightness = .45f;
        private bool autoCropUseEdgeColor;

        private int rightRotations;

        private bool brightAdjust;
        private float brightAdjustMinClusterFrac = .5f;
        private bool brightAdjustWhiteCorrect;

        private bool unbias;
        private int unbiasMaxDegree = 6;
        private float unbiasMaxChisq = 200;
        private float unbiasMaxS = .10f;
        private float unbiasMinV = .35f;

        private bool oneBit;
        private Transforms.Channel oneBitChannel = Transforms.Channel.Composite;
        private float oneBitThreshhold = .5f;
        private OutputTransforms.OutputFormat oneBitOutputFormat = OutputTransforms.OutputFormat.Png;
        private bool oneBitScaleUp;

        private bool staticSaturate;
        private float staticSaturateWhiteThreshhold = .9f;
        private float staticSaturateBlackThreshhold = .1f;
        private float staticSaturateExponent = 1;

        private bool enableAutoNormalizeGeometry;
        private float? autoNormalizeGeometryAspectHeight;
        private float? autoNormalizeGeometryAspectWidth;
        private Transforms.InterpMethod normalizeGeometryPreviewInterp = Transforms.InterpMethod.NearestNeighbor;
        private Transforms.InterpMethod normalizeGeometryFinalInterp = Transforms.InterpMethod.Bicubic;

        private bool? timestamps;
        private bool timestampsOverwriteExisting;
        private bool timestampsExifMissingModifiedInsteadOfCreated;

        // UI options
        private bool showOriginalInsteadOfProcessed = false;
        private bool showAnnotationsPrimary = true;
        private bool showAnnotationsDetail = false;
        private bool showShrinkExpandDetail = true;
        private bool showAutoCropGrid = false;
        private bool showPolyUnbiasGrid = false;
        private int[] lastSelected = new int[] { 0 };
        private int lastSelectedSwap = -1;

        [Bindable(true)]
        public int JpegQuality { get { return jpegQuality; } set { jpegQuality = value; } }
        [Bindable(true)]
        public bool JpegUseGDI { get { return jpegUseGDI; } set { jpegUseGDI = value; } }

        [Bindable(true)]
        public bool Shrink { get { return shrink; } set { shrink = value; } }
        [Bindable(true)]
        public float ShrinkFactor { get { return shrinkFactor; } set { shrinkFactor = value; } }

        [Bindable(true)]
        public bool AutoCrop { get { return autoCrop; } set { autoCrop = value; } }

        [Bindable(true)]
        public float AutoCropLeftMax { get { return autoCropLeftLimit; } set { autoCropLeftLimit = value; } }
        [Bindable(true)]
        public float AutoCropTopMax { get { return autoCropTopLimit; } set { autoCropTopLimit = value; } }
        [Bindable(true)]
        public float AutoCropRightMax { get { return autoCropRightLimit; } set { autoCropRightLimit = value; } }
        [Bindable(true)]
        public float AutoCropBottomMax { get { return autoCropBottomLimit; } set { autoCropBottomLimit = value; } }
        [Bindable(true)]
        public float AutoCropMinMedianBrightness { get { return autoCropMinMedianBrightness; } set { autoCropMinMedianBrightness = value; } }
        [Bindable(true)]
        public bool AutoCropUseEdgeColor { get { return autoCropUseEdgeColor; } set { autoCropUseEdgeColor = value; } }

        [Bindable(true)]
        public int RightRotations { get { return rightRotations; } set { rightRotations = value; } }

        [Bindable(true)]
        public bool BrightAdjust { get { return brightAdjust; } set { brightAdjust = value; } }
        [Bindable(true)]
        public float BrightAdjustMinClusterFrac { get { return brightAdjustMinClusterFrac; } set { brightAdjustMinClusterFrac = value; } }
        [Bindable(true)]
        public bool BrightAdjustWhiteCorrect { get { return brightAdjustWhiteCorrect; } set { brightAdjustWhiteCorrect = value; } }

        [Bindable(true)]
        public bool Unbias { get { return unbias; } set { unbias = value; } }
        [Bindable(true)]
        public int UnbiasMaxDegree { get { return unbiasMaxDegree; } set { unbiasMaxDegree = value; } }
        [Bindable(true)]
        public float UnbiasMaxChisq { get { return unbiasMaxChisq; } set { unbiasMaxChisq = value; } }
        [Bindable(true)]
        public float UnbiasMaxS { get { return unbiasMaxS; } set { unbiasMaxS = value; } }
        [Bindable(true)]
        public float UnbiasMinV { get { return unbiasMinV; } set { unbiasMinV = value; } }

        [Bindable(true)]
        public bool OneBit { get { return oneBit; } set { oneBit = value; } }
        [Bindable(true)]
        public Transforms.Channel OneBitChannel { get { return oneBitChannel; } set { oneBitChannel = value; } }
        [Bindable(true)]
        public float OneBitThreshhold { get { return oneBitThreshhold; } set { oneBitThreshhold = value; } }
        [Bindable(true)]
        public OutputTransforms.OutputFormat OneBitOutputFormat { get { return oneBitOutputFormat; } set { oneBitOutputFormat = value; } }
        [Bindable(true)]
        public bool OneBitScaleUp { get { return oneBitScaleUp; } set { oneBitScaleUp = value; } }

        [Bindable(true)]
        public bool StaticSaturate { get { return staticSaturate; } set { staticSaturate = value; } }
        [Bindable(true)]
        public float StaticSaturateWhiteThreshhold { get { return staticSaturateWhiteThreshhold; } set { staticSaturateWhiteThreshhold = value; } }
        [Bindable(true)]
        public float StaticSaturateBlackThreshhold { get { return staticSaturateBlackThreshhold; } set { staticSaturateBlackThreshhold = value; } }
        [Bindable(true)]
        public float StaticSaturateExponent { get { return staticSaturateExponent; } set { staticSaturateExponent = value; } }

        [Bindable(true)]
        public bool EnableAutoNormalizeGeometry { get { return enableAutoNormalizeGeometry; } set { enableAutoNormalizeGeometry = value; } }
        [Bindable(true)]
        public float? AutoNormalizeGeometryAspectHeight { get { return autoNormalizeGeometryAspectHeight; } set { autoNormalizeGeometryAspectHeight = value; } }
        [Bindable(true)]
        public string AutoNormalizeGeometryAspectHeightAsString { get { return autoNormalizeGeometryAspectHeight.ToString(); } set { autoNormalizeGeometryAspectHeight = null; float f; if (Single.TryParse(value, out f)) { autoNormalizeGeometryAspectHeight = f; } } }
        [Bindable(true)]
        public float? AutoNormalizeGeometryAspectWidth { get { return autoNormalizeGeometryAspectWidth; } set { autoNormalizeGeometryAspectWidth = value; } }
        [Bindable(true)]
        public string AutoNormalizeGeometryAspectWidthAsString { get { return autoNormalizeGeometryAspectWidth.ToString(); } set { autoNormalizeGeometryAspectWidth = null; float f; if (Single.TryParse(value, out f)) { autoNormalizeGeometryAspectWidth = f; } } }
        [Bindable(true)]
        public float? AutoNormalizeGeometryAspectRatio { get { return autoNormalizeGeometryAspectHeight.HasValue && autoNormalizeGeometryAspectWidth.HasValue ? autoNormalizeGeometryAspectWidth.Value / autoNormalizeGeometryAspectHeight.Value : (float?)null; } }
        [Bindable(true)]
        public Transforms.InterpMethod NormalizeGeometryPreviewInterp { get { return normalizeGeometryPreviewInterp; } set { normalizeGeometryPreviewInterp = value; } }
        [Bindable(true)]
        public Transforms.InterpMethod NormalizeGeometryFinalInterp { get { return normalizeGeometryFinalInterp; } set { normalizeGeometryFinalInterp = value; } }

        [Bindable(true)]
        public bool? Timestamps { get { return timestamps; } set { timestamps = value; } }
        [Bindable(true)]
        public bool TimestampsOverwriteExisting { get { return timestampsOverwriteExisting; } set { timestampsOverwriteExisting = value; } }
        [Bindable(true)]
        public bool TimestampsExifMissingModifiedInsteadOfCreated { get { return timestampsExifMissingModifiedInsteadOfCreated; } set { timestampsExifMissingModifiedInsteadOfCreated = value; } }

        // UI options
        [Bindable(true)]
        public bool ShowOriginalInsteadOfProcessed { get { return showOriginalInsteadOfProcessed; } set { showOriginalInsteadOfProcessed = value; } }
        [Bindable(true)]
        public bool ShowAnnotationsPrimary { get { return showAnnotationsPrimary; } set { showAnnotationsPrimary = value; } }
        [Bindable(true)]
        public bool ShowAnnotationsDetail { get { return showAnnotationsDetail; } set { showAnnotationsDetail = value; } }
        [Bindable(true)]
        public bool ShowShrinkExpandDetail { get { return showShrinkExpandDetail; } set { showShrinkExpandDetail = value; } }
        [Bindable(true)]
        public bool ShowAutoCropGrid { get { return showAutoCropGrid; } set { showAutoCropGrid = value; } }
        [Bindable(true)]
        public bool ShowPolyUnbiasGrid { get { return showPolyUnbiasGrid; } set { showPolyUnbiasGrid = value; } }
        [Bindable(true)]
        public int[] LastSelected { get { return lastSelected; } set { lastSelected = value; } }
        [Bindable(true)]
        public int LastSelectedSwap { get { return lastSelectedSwap; } set { lastSelectedSwap = value; } }

        public void CopyFrom(GlobalOptions source)
        {
            this.showOriginalInsteadOfProcessed = source.showOriginalInsteadOfProcessed;
            this.showAnnotationsPrimary = source.showAnnotationsPrimary;
            this.showAnnotationsDetail = source.showAnnotationsDetail;
            this.showShrinkExpandDetail = source.showShrinkExpandDetail;
            this.showAutoCropGrid = source.showAutoCropGrid;
            this.showPolyUnbiasGrid = source.showPolyUnbiasGrid;
            this.lastSelected = (int[])source.lastSelected.Clone();
            this.lastSelectedSwap = source.lastSelectedSwap;

            this.jpegQuality = source.jpegQuality;
            this.jpegUseGDI = source.jpegUseGDI;

            this.shrink = source.shrink;
            this.shrinkFactor = source.shrinkFactor;

            this.autoCrop = source.autoCrop;
            this.autoCropLeftLimit = source.autoCropLeftLimit;
            this.autoCropTopLimit = source.autoCropTopLimit;
            this.autoCropRightLimit = source.autoCropRightLimit;
            this.autoCropBottomLimit = source.autoCropBottomLimit;
            this.autoCropMinMedianBrightness = source.autoCropMinMedianBrightness;
            this.autoCropUseEdgeColor = source.autoCropUseEdgeColor;

            this.rightRotations = source.rightRotations;

            this.brightAdjust = source.brightAdjust;
            this.brightAdjustMinClusterFrac = source.brightAdjustMinClusterFrac;
            this.brightAdjustWhiteCorrect = source.brightAdjustWhiteCorrect;

            this.unbias = source.unbias;
            this.unbiasMaxDegree = source.unbiasMaxDegree;
            this.unbiasMaxChisq = source.unbiasMaxChisq;
            this.unbiasMaxS = source.unbiasMaxS;
            this.unbiasMinV = source.unbiasMinV;

            this.oneBit = source.oneBit;
            this.oneBitChannel = source.oneBitChannel;
            this.oneBitThreshhold = source.oneBitThreshhold;
            this.oneBitOutputFormat = source.oneBitOutputFormat;
            this.oneBitScaleUp = source.oneBitScaleUp;

            this.staticSaturate = source.staticSaturate;
            this.staticSaturateWhiteThreshhold = source.staticSaturateWhiteThreshhold;
            this.staticSaturateBlackThreshhold = source.staticSaturateBlackThreshhold;
            this.staticSaturateExponent = source.staticSaturateExponent;

            this.enableAutoNormalizeGeometry = source.enableAutoNormalizeGeometry;
            this.autoNormalizeGeometryAspectHeight = source.autoNormalizeGeometryAspectHeight;
            this.autoNormalizeGeometryAspectWidth = source.autoNormalizeGeometryAspectWidth;
            this.normalizeGeometryPreviewInterp = source.normalizeGeometryPreviewInterp;
            this.normalizeGeometryFinalInterp = source.normalizeGeometryFinalInterp;

            this.timestamps = source.timestamps;
            this.timestampsOverwriteExisting = source.timestampsOverwriteExisting;
            this.timestampsExifMissingModifiedInsteadOfCreated = source.timestampsExifMissingModifiedInsteadOfCreated;
        }

        public GlobalOptions(GlobalOptions source)
        {
            this.CopyFrom(source);
        }

        public GlobalOptions(XPathNavigator nav)
        {
            if (nav != null)
            {
                this.showOriginalInsteadOfProcessed = nav.SelectSingleNode("ui/showOriginalInsteadOfProcessed").ValueAsBoolean;
                this.showAnnotationsPrimary = nav.SelectSingleNode("ui/showAnnotationsPrimary").ValueAsBoolean;
                this.showAnnotationsDetail = nav.SelectSingleNode("ui/showAnnotationsDetail").ValueAsBoolean;
                this.showShrinkExpandDetail = nav.SelectSingleNode("ui/showShrinkExpandDetail").ValueAsBoolean;
                this.showAutoCropGrid = nav.SelectSingleNode("ui/showAutoCropGrid").ValueAsBoolean;
                this.showPolyUnbiasGrid = nav.SelectSingleNode("ui/showPolyUnbiasGrid").ValueAsBoolean;
                {
                    List<int> items = new List<int>();
                    foreach (XPathNavigator index in nav.Select("ui/lastSelected/index"))
                    {
                        items.Add(index.ValueAsInt);
                    }
                    this.lastSelected = items.ToArray();
                }
                this.lastSelectedSwap = nav.SelectSingleNode("ui/lastSelectedSwap").ValueAsInt;

                this.jpegQuality = nav.SelectSingleNode("jpegQuality").ValueAsInt;
                this.jpegUseGDI = nav.SelectSingleNode("jpegUseGDI").ValueAsBoolean;

                this.shrink = nav.SelectSingleNode("shrink/enable").ValueAsBoolean;
                this.shrinkFactor = nav.SelectSingleNode("shrink/shrinkFactor").ValueAsInt;

                this.autoCrop = nav.SelectSingleNode("autoCrop/enable").ValueAsBoolean;
                this.autoCropLeftLimit = (float)nav.SelectSingleNode("autoCrop/leftLimit").ValueAsDouble;
                this.autoCropTopLimit = (float)nav.SelectSingleNode("autoCrop/topLimit").ValueAsDouble;
                this.autoCropRightLimit = (float)nav.SelectSingleNode("autoCrop/rightLimit").ValueAsDouble;
                this.autoCropBottomLimit = (float)nav.SelectSingleNode("autoCrop/bottomLimit").ValueAsDouble;
                this.autoCropMinMedianBrightness = (float)nav.SelectSingleNode("autoCrop/minMedianBrightness").ValueAsDouble;
                this.autoCropUseEdgeColor = nav.SelectSingleNode("autoCrop/useEdgeColor").ValueAsBoolean;

                this.rightRotations = nav.SelectSingleNode("rightRotations").ValueAsInt;

                this.brightAdjust = nav.SelectSingleNode("brightAdjust/enable").ValueAsBoolean;
                this.brightAdjustMinClusterFrac = (float)nav.SelectSingleNode("brightAdjust/minClusterFrac").ValueAsDouble;
                this.brightAdjustWhiteCorrect = nav.SelectSingleNode("brightAdjust/whiteCorrect").ValueAsBoolean;

                this.unbias = nav.SelectSingleNode("polyUnbias/enable").ValueAsBoolean;
                this.unbiasMaxDegree = nav.SelectSingleNode("polyUnbias/maxDegree").ValueAsInt;
                this.unbiasMaxChisq = (float)nav.SelectSingleNode("polyUnbias/maxChisq").ValueAsDouble;
                this.unbiasMaxS = (float)nav.SelectSingleNode("polyUnbias/maxS").ValueAsDouble;
                this.unbiasMinV = (float)nav.SelectSingleNode("polyUnbias/minV").ValueAsDouble;

                this.oneBit = nav.SelectSingleNode("oneBit/enable").ValueAsBoolean;
                this.oneBitChannel = (Transforms.Channel)nav.SelectSingleNode("oneBit/channel").ValueAsInt;
                this.oneBitThreshhold = (float)nav.SelectSingleNode("oneBit/threshhold").ValueAsDouble;
                this.oneBitOutputFormat = (OutputTransforms.OutputFormat)nav.SelectSingleNode("oneBit/outputFormat").ValueAsInt;
                this.oneBitScaleUp = nav.SelectSingleNode("oneBit/scaleUp").ValueAsBoolean;

                this.staticSaturate = nav.SelectSingleNode("staticSaturate/enable").ValueAsBoolean;
                this.staticSaturateWhiteThreshhold = (float)nav.SelectSingleNode("staticSaturate/whiteThreshhold").ValueAsDouble;
                this.staticSaturateBlackThreshhold = (float)nav.SelectSingleNode("staticSaturate/blackThreshhold").ValueAsDouble;
                this.staticSaturateExponent = (float)nav.SelectSingleNode("staticSaturate/exponent").ValueAsDouble;

                this.enableAutoNormalizeGeometry = nav.SelectSingleNode("normalizeGeometry/enable").ValueAsBoolean;
                this.autoNormalizeGeometryAspectHeight = (float?)nav.SelectSingleNode("normalizeGeometry/autoNormalizeGeometryAspectHeight")?.ValueAsDouble;
                this.autoNormalizeGeometryAspectWidth = (float?)nav.SelectSingleNode("normalizeGeometry/autoNormalizeGeometryAspectWidth")?.ValueAsDouble;
                this.normalizeGeometryPreviewInterp = (Transforms.InterpMethod)Enum.Parse(typeof(Transforms.InterpMethod), nav.SelectSingleNode("normalizeGeometry/normalizeGeometryPreviewInterp").Value);
                this.normalizeGeometryFinalInterp = (Transforms.InterpMethod)Enum.Parse(typeof(Transforms.InterpMethod), nav.SelectSingleNode("normalizeGeometry/normalizeGeometryFinalInterp").Value);

                this.timestamps = nav.SelectSingleNode("timestamps/enable")?.ValueAsBoolean;
                {
                    XPathNavigator nav2 = nav.SelectSingleNode("timestamps/overwriteExisting");
                    if (nav2 != null)
                    {
                        this.timestampsOverwriteExisting = nav2.ValueAsBoolean;
                    }
                    nav2 = nav.SelectSingleNode("timestamps/timestampsExifMissingModifiedInsteadOfCreated");
                    if (nav2 != null)
                    {
                        this.timestampsExifMissingModifiedInsteadOfCreated = nav2.ValueAsBoolean;
                    }
                }
            }
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("options");

            writer.WriteStartElement("ui");
            //
            writer.WriteStartElement("showOriginalInsteadOfProcessed");
            writer.WriteValue(this.showOriginalInsteadOfProcessed);
            writer.WriteEndElement(); // showOriginalInsteadOfProcessed
            //
            writer.WriteStartElement("showAnnotationsPrimary");
            writer.WriteValue(this.showAnnotationsPrimary);
            writer.WriteEndElement(); // showAnnotationsPrimary
            //
            writer.WriteStartElement("showAnnotationsDetail");
            writer.WriteValue(this.showAnnotationsDetail);
            writer.WriteEndElement(); // showAnnotationsDetail
            //
            writer.WriteStartElement("showShrinkExpandDetail");
            writer.WriteValue(this.showShrinkExpandDetail);
            writer.WriteEndElement(); // showShrinkExpandDetail
            //
            writer.WriteStartElement("showAutoCropGrid");
            writer.WriteValue(this.showAutoCropGrid);
            writer.WriteEndElement(); // showAutoCropGrid
            //
            writer.WriteStartElement("showPolyUnbiasGrid");
            writer.WriteValue(this.showPolyUnbiasGrid);
            writer.WriteEndElement(); // showPolyUnbiasGrid
            //
            writer.WriteStartElement("lastSelected");
            foreach (int index in this.lastSelected)
            {
                writer.WriteStartElement("index");
                writer.WriteValue(index);
                writer.WriteEndElement(); // index
            }
            writer.WriteEndElement(); // lastSelected
            //
            writer.WriteStartElement("lastSelectedSwap");
            writer.WriteValue(this.lastSelectedSwap);
            writer.WriteEndElement(); // lastSelectedSwap
            //
            writer.WriteEndElement();

            writer.WriteStartElement("jpegQuality");
            writer.WriteValue(this.jpegQuality);
            writer.WriteEndElement(); // jpegQuality
            //
            writer.WriteStartElement("jpegUseGDI");
            writer.WriteValue(this.jpegUseGDI);
            writer.WriteEndElement(); // jpegUseGDI

            writer.WriteStartElement("shrink");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.shrink);
            writer.WriteEndElement(); // shrink
            //
            writer.WriteStartElement("shrinkFactor");
            writer.WriteValue(this.shrinkFactor);
            writer.WriteEndElement(); // shrinkFactor
            //
            writer.WriteEndElement(); // shrink

            writer.WriteStartElement("autoCrop");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.autoCrop);
            writer.WriteEndElement(); // enable
            //
            writer.WriteStartElement("leftLimit");
            writer.WriteValue(this.autoCropLeftLimit);
            writer.WriteEndElement(); // leftLimit
            //
            writer.WriteStartElement("topLimit");
            writer.WriteValue(this.autoCropTopLimit);
            writer.WriteEndElement(); // topLimit
            //
            writer.WriteStartElement("rightLimit");
            writer.WriteValue(this.autoCropRightLimit);
            writer.WriteEndElement(); // rightLimit
            //
            writer.WriteStartElement("bottomLimit");
            writer.WriteValue(this.autoCropBottomLimit);
            writer.WriteEndElement(); // bottomLimit
            //
            writer.WriteStartElement("minMedianBrightness");
            writer.WriteValue(this.autoCropMinMedianBrightness);
            writer.WriteEndElement(); // minMedianBrightness
            //
            writer.WriteStartElement("useEdgeColor");
            writer.WriteValue(this.autoCropUseEdgeColor);
            writer.WriteEndElement(); // useEdgeColor
            //
            writer.WriteEndElement(); // autoCrop

            writer.WriteStartElement("rightRotations");
            writer.WriteValue(this.rightRotations);
            writer.WriteEndElement(); // rightRotations

            writer.WriteStartElement("brightAdjust");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.brightAdjust);
            writer.WriteEndElement(); // enable
            //
            writer.WriteStartElement("minClusterFrac");
            writer.WriteValue(this.brightAdjustMinClusterFrac);
            writer.WriteEndElement(); // minClusterFrac
            //
            writer.WriteStartElement("whiteCorrect");
            writer.WriteValue(this.brightAdjustWhiteCorrect);
            writer.WriteEndElement(); // whiteCorrect
            //
            writer.WriteEndElement(); // brightAdjust

            writer.WriteStartElement("polyUnbias");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.unbias);
            writer.WriteEndElement(); // enable
            //
            writer.WriteStartElement("maxDegree");
            writer.WriteValue(this.unbiasMaxDegree);
            writer.WriteEndElement(); // maxDegree
            //
            writer.WriteStartElement("maxChisq");
            writer.WriteValue(this.unbiasMaxChisq);
            writer.WriteEndElement(); // maxChisq
            //
            writer.WriteStartElement("maxS");
            writer.WriteValue(this.unbiasMaxS);
            writer.WriteEndElement(); // maxS
            //
            writer.WriteStartElement("minV");
            writer.WriteValue(this.unbiasMinV);
            writer.WriteEndElement(); // minV
            //
            writer.WriteEndElement(); // polyUnbias

            writer.WriteStartElement("oneBit");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.oneBit);
            writer.WriteEndElement(); // enable
            //
            writer.WriteStartElement("channel");
            writer.WriteValue((int)this.oneBitChannel);
            writer.WriteEndElement(); // oneBitChannel
            //
            writer.WriteStartElement("threshhold");
            writer.WriteValue(this.oneBitThreshhold);
            writer.WriteEndElement(); // oneBitThreshhold
            //
            writer.WriteStartElement("outputFormat");
            writer.WriteValue((int)this.oneBitOutputFormat);
            writer.WriteEndElement(); // outputFormat
            //
            writer.WriteStartElement("scaleUp");
            writer.WriteValue(this.oneBitScaleUp);
            writer.WriteEndElement(); // scaleUp
            //
            writer.WriteEndElement(); // oneBit

            writer.WriteStartElement("staticSaturate");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.staticSaturate);
            writer.WriteEndElement(); // enable
            //
            writer.WriteStartElement("whiteThreshhold");
            writer.WriteValue(this.staticSaturateWhiteThreshhold);
            writer.WriteEndElement(); // whiteThreshhold
            //
            writer.WriteStartElement("blackThreshhold");
            writer.WriteValue(this.staticSaturateBlackThreshhold);
            writer.WriteEndElement(); // blackThreshhold
            //
            writer.WriteStartElement("exponent");
            writer.WriteValue(this.staticSaturateExponent);
            writer.WriteEndElement(); // exponent
            //
            writer.WriteEndElement(); // staticSaturate

            writer.WriteStartElement("normalizeGeometry");
            //
            writer.WriteStartElement("enable");
            writer.WriteValue(this.enableAutoNormalizeGeometry);
            writer.WriteEndElement(); // enable
            //
            if (this.autoNormalizeGeometryAspectWidth.HasValue)
            {
                writer.WriteStartElement("autoNormalizeGeometryAspectWidth");
                writer.WriteValue(this.autoNormalizeGeometryAspectWidth.Value);
                writer.WriteEndElement(); // autoNormalizeGeometryAspectWidth
            }
            //
            if (this.autoNormalizeGeometryAspectHeight.HasValue)
            {
                writer.WriteStartElement("autoNormalizeGeometryAspectHeight");
                writer.WriteValue(this.autoNormalizeGeometryAspectHeight.Value);
                writer.WriteEndElement(); // autoNormalizeGeometryAspectHeight
            }
            //
            writer.WriteStartElement("normalizeGeometryPreviewInterp");
            writer.WriteValue(this.normalizeGeometryPreviewInterp.ToString());
            writer.WriteEndElement(); // normalizeGeometryPreviewInterp
            //
            writer.WriteStartElement("normalizeGeometryFinalInterp");
            writer.WriteValue(this.normalizeGeometryFinalInterp.ToString());
            writer.WriteEndElement(); // normalizeGeometryFinalInterp
            //
            writer.WriteEndElement(); // normalizeGeometry

            writer.WriteStartElement("timestamps");
            if (this.timestamps.HasValue)
            {
                writer.WriteStartElement("enable");
                writer.WriteValue(this.timestamps.Value);
                writer.WriteEndElement(); // enable
                //
                writer.WriteStartElement("timestampsOverwriteExisting");
                writer.WriteValue(this.timestampsOverwriteExisting);
                writer.WriteEndElement(); // timestampsOverwriteExisting
                //
                writer.WriteStartElement("timestampsExifMissingModifiedInsteadOfCreated");
                writer.WriteValue(this.timestampsExifMissingModifiedInsteadOfCreated);
                writer.WriteEndElement(); // timestampsExifMissingModifiedInsteadOfCreated
            }
            writer.WriteEndElement(); // timestamps

            writer.WriteEndElement(); // options
        }
    }
}
