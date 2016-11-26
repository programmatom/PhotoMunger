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
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class ItemOptionsDialog : Form
    {
        private readonly Options options;

        private ItemOptionsDialog(Options options, string name)
        {
            this.options = options;

            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;
            this.Text = String.Format("Options: {0}", name);

            optionsBindingSource.Add(options);

            SelectOneBitChannel(options.OneBitChannel, false/*updateSource*/);
            SelectJpegEncoder(options.JpegUseGdi, false/*updateSource*/);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
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
                    options.JpegUseGdi = useGdiEncoder;
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


        //

        public static void DoDialog(Item item)
        {
            Options options = new Options(item);
            using (ItemOptionsDialog dialog = new ItemOptionsDialog(options, item.RenamedFileName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    options.Save(item);
                }
            }
        }

        public static void DoDialog(IList<Item> items)
        {
            if (items.Count == 0)
            {
                Debug.Assert(false);
                //throw new ArgumentException();
                return;
            }

            if (items.Count == 1)
            {
                DoDialog(items[0]);
                return;
            }

            // TODO: indeterminate state UI
            Options options = new Options(items[0]);
            using (ItemOptionsDialog dialog = new ItemOptionsDialog(options, String.Format("[{0} Items]", items.Count)))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (Item item in items)
                    {
                        options.Save(item);
                    }
                }
            }
        }
    }

    public class Options : INotifyPropertyChanged
    {
        private int jpegQuality;
        private bool jpegQuality_dirty;
        private bool jpegUseGdi;
        private bool jpegUseGdi_dirty;

        private bool shrink;
        private bool shrink_dirty;
        private float shrinkFactor;
        private bool shrinkFactor_dirty;

        private bool brightAdjust;
        private bool brightAdjust_dirty;
        private float brightAdjustMinClusterFrac;
        private bool brightAdjustMinClusterFrac_dirty;
        private bool brightAdjustWhiteCorrect;
        private bool brightAdjustWhiteCorrect_dirty;

        private bool unbias;
        private bool unbias_dirty;
        private int unbiasMaxDegree;
        private bool unbiasMaxDegree_dirty;
        private float unbiasMaxChisq;
        private bool unbiasMaxChisq_dirty;

        private bool oneBit;
        private bool oneBit_dirty;
        private Transforms.Channel oneBitChannel;
        private bool oneBitChannel_dirty;
        private float oneBitThreshhold;
        private bool oneBitThreshhold_dirty;

        private bool staticSaturate;
        private bool staticSaturate_dirty;
        private float staticSaturateWhiteThreshhold = .9f;
        private bool staticSaturateWhiteThreshhold_dirty;
        private float staticSaturateBlackThreshhold = .1f;
        private bool staticSaturateBlackThreshhold_dirty;
        private float staticSaturateExponent = .1f;
        private bool staticSaturateExponent_dirty;

        private bool normalizeGeometry;
        private bool normalizeGeometry_dirty;

        private double fineRotateDegrees;
        private bool fineRotateDegrees_dirty;

        private Rectangle cropRect;
        private bool cropRect_dirty;

        public event PropertyChangedEventHandler PropertyChanged;

        [Bindable(true)]
        public int JpegQuality { get { return jpegQuality; } set { jpegQuality = value; jpegQuality_dirty = true; Notify("JpegQuality"); } }
        [Bindable(true)]
        public bool JpegUseGdi { get { return jpegUseGdi; } set { jpegUseGdi = value; jpegUseGdi_dirty = true; Notify("JpegUseGdi"); } }

        [Bindable(true)]
        public bool Shrink { get { return shrink; } set { shrink = value; shrink_dirty = true; Notify("Shrink"); } }
        [Bindable(true)]
        public float ShrinkFactor { get { return shrinkFactor; } set { shrinkFactor = value; shrinkFactor_dirty = true; Notify("ShrinkFactor"); } }

        [Bindable(true)]
        public bool BrightAdjust { get { return brightAdjust; } set { brightAdjust = value; brightAdjust_dirty = true; Notify("BrightAdjust"); } }
        [Bindable(true)]
        public float BrightAdjustMinClusterFrac { get { return brightAdjustMinClusterFrac; } set { brightAdjustMinClusterFrac = value; brightAdjustMinClusterFrac_dirty = true; Notify("BrightAdjustMinClusterFrac"); } }
        [Bindable(true)]
        public bool BrightAdjustWhiteCorrect { get { return brightAdjustWhiteCorrect; } set { brightAdjustWhiteCorrect = value; brightAdjustWhiteCorrect_dirty = true; Notify("BrightAdjustWhiteCorrect"); } }

        [Bindable(true)]
        public bool Unbias { get { return unbias; } set { unbias = value; unbias_dirty = true; Notify("Unbias"); } }
        [Bindable(true)]
        public int UnbiasMaxDegree { get { return unbiasMaxDegree; } set { unbiasMaxDegree = value; unbiasMaxDegree_dirty = true; Notify("UnbiasMaxDegree"); } }
        [Bindable(true)]
        public float UnbiasMaxChisq { get { return unbiasMaxChisq; } set { unbiasMaxChisq = value; unbiasMaxChisq_dirty = true; Notify("UnbiasMaxChisq"); } }

        [Bindable(true)]
        public bool OneBit { get { return oneBit; } set { oneBit = value; oneBit_dirty = true; Notify("OneBit"); } }
        [Bindable(true)]
        public Transforms.Channel OneBitChannel { get { return oneBitChannel; } set { oneBitChannel = value; oneBitChannel_dirty = true; Notify("OneBitChannel"); } }
        [Bindable(true)]
        public float OneBitThreshhold { get { return oneBitThreshhold; } set { oneBitThreshhold = value; oneBitThreshhold_dirty = true; Notify("OneBitThreshhold"); } }

        [Bindable(true)]
        public bool StaticSaturate { get { return staticSaturate; } set { staticSaturate = value; staticSaturate_dirty = true; Notify("StaticSaturate"); } }
        [Bindable(true)]
        public float StaticSaturateWhiteThreshhold { get { return staticSaturateWhiteThreshhold; } set { staticSaturateWhiteThreshhold = value; staticSaturateWhiteThreshhold_dirty = true; Notify("StaticSaturateWhiteThreshhold"); } }
        [Bindable(true)]
        public float StaticSaturateBlackThreshhold { get { return staticSaturateBlackThreshhold; } set { staticSaturateBlackThreshhold = value; staticSaturateBlackThreshhold_dirty = true; Notify("StaticSaturateBlackThreshhold"); } }
        [Bindable(true)]
        public float StaticSaturateExponent { get { return staticSaturateExponent; } set { staticSaturateExponent = value; staticSaturateExponent_dirty = true; Notify("StaticSaturateExponent"); } }

        [Bindable(true)]
        public bool NormalizeGeometry { get { return normalizeGeometry; } set { normalizeGeometry = value; normalizeGeometry_dirty = true; Notify("NormalizeGeometry"); } }

        [Bindable(true)]
        public double FineRotateDegrees { get { return fineRotateDegrees; } set { fineRotateDegrees = value; fineRotateDegrees_dirty = true; Notify("FineRotateDegrees"); } }

        [Bindable(true)]
        public int CropRectLeft
        {
            get
            {
                return cropRect.Left;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(value, cropRect.Y, cropRect.Width + cropRect.Left - value, cropRect.Height);
                cropRect_dirty = true;
                Notify("CropRectLeft");
                Notify("CropRectX");
                Notify("CropRectWidth");
            }
        }
        [Bindable(true)]
        public int CropRectTop
        {
            get
            {
                return cropRect.Top;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(cropRect.X, value, cropRect.Width, cropRect.Height + cropRect.Top - value);
                cropRect_dirty = true;
                Notify("CropRectTop");
                Notify("CropRectY");
                Notify("CropRectHeight");
            }
        }
        [Bindable(true)]
        public int CropRectRight
        {
            get
            {
                return cropRect.Right;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(cropRect.X, cropRect.Y, cropRect.Width + value - cropRect.Right, cropRect.Height);
                cropRect_dirty = true;
                Notify("CropRectRight");
                Notify("CropRectWidth");
            }
        }
        [Bindable(true)]
        public int CropRectBottom
        {
            get
            {
                return cropRect.Bottom;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(cropRect.X, cropRect.Y, cropRect.Width, cropRect.Height + value - cropRect.Bottom);
                cropRect_dirty = true;
                Notify("CropRectBottom");
                Notify("CropRectHeight");
            }
        }
        [Bindable(true)]
        public int CropRectX
        {
            get
            {
                return cropRect.X;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(value, cropRect.Y, cropRect.Width, cropRect.Height);
                cropRect_dirty = true;
                Notify("CropRectX");
                Notify("CropRectLeft");
                Notify("CropRectRight");
            }
        }
        [Bindable(true)]
        public int CropRectY
        {
            get
            {
                return cropRect.Y;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(cropRect.X, value, cropRect.Width, cropRect.Height);
                cropRect_dirty = true;
                Notify("CropRectY");
                Notify("CropRectTop");
                Notify("CropRectBottom");
            }
        }
        [Bindable(true)]
        public int CropRectWidth
        {
            get
            {
                return cropRect.Width;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(cropRect.X, cropRect.Y, value, cropRect.Height);
                cropRect_dirty = true;
                Notify("CropRectWidth");
                Notify("CropRectRight");
            }
        }
        [Bindable(true)]
        public int CropRectHeight
        {
            get
            {
                return cropRect.Height;
            }
            set
            {
                value = Transforms.SnapLossless(value);
                cropRect = new Rectangle(cropRect.X, cropRect.Y, cropRect.Width, value);
                cropRect_dirty = true;
                Notify("CropRectHeight");
                Notify("CropRectBottom");
            }
        }

        public Options(Item item)
        {
            this.jpegQuality = item.JpegQuality;
            this.jpegUseGdi = item.JpegUseGdi;

            this.shrink = item.Shrink;
            this.shrinkFactor = item.ShrinkFactor;

            this.brightAdjust = item.BrightAdjust;
            this.brightAdjustMinClusterFrac = item.BrightAdjustMinClusterFrac;
            this.brightAdjustWhiteCorrect = item.BrightAdjustWhiteCorrect;

            this.unbias = item.Unbias;
            this.unbiasMaxDegree = item.UnbiasMaxDegree;
            this.unbiasMaxChisq = item.UnbiasMaxChisq;

            this.oneBit = item.OneBit;
            this.oneBitChannel = item.OneBitChannel;
            this.oneBitThreshhold = item.OneBitThreshhold;

            this.staticSaturate = item.StaticSaturate;
            this.staticSaturateWhiteThreshhold = item.StaticSaturateWhiteThreshhold;
            this.staticSaturateBlackThreshhold = item.StaticSaturateBlackThreshhold;
            this.staticSaturateExponent = item.StaticSaturateExponent;

            this.normalizeGeometry = item.NormalizeGeometry;

            this.fineRotateDegrees = item.FineRotateDegrees;

            this.cropRect = item.CropRect;
        }

        private void Notify(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public void Save(Item item)
        {
            if (jpegQuality_dirty)
            {
                item.JpegQuality = this.jpegQuality;
            }
            if (jpegUseGdi_dirty)
            {
                item.JpegUseGdi = this.jpegUseGdi;
            }

            if (shrink_dirty)
            {
                item.Shrink = this.shrink;
            }
            if (shrinkFactor_dirty)
            {
                item.ShrinkFactor = this.shrinkFactor;
            }

            if (brightAdjust_dirty)
            {
                item.BrightAdjust = this.brightAdjust;
            }
            if (brightAdjustMinClusterFrac_dirty)
            {
                item.BrightAdjustMinClusterFrac = this.brightAdjustMinClusterFrac;
            }
            if (brightAdjustWhiteCorrect_dirty)
            {
                item.BrightAdjustWhiteCorrect = this.brightAdjustWhiteCorrect;
            }

            if (unbias_dirty)
            {
                item.Unbias = this.unbias;
            }
            if (unbiasMaxDegree_dirty)
            {
                item.UnbiasMaxDegree = this.unbiasMaxDegree;
            }
            if (unbiasMaxChisq_dirty)
            {
                item.UnbiasMaxChisq = this.unbiasMaxChisq;
            }

            if (oneBit_dirty)
            {
                item.OneBit = this.oneBit;
            }
            if (oneBitChannel_dirty)
            {
                item.OneBitChannel = this.oneBitChannel;
            }
            if (oneBitThreshhold_dirty)
            {
                item.OneBitThreshhold = this.oneBitThreshhold;
            }

            if (staticSaturate_dirty)
            {
                item.StaticSaturate = this.staticSaturate;
            }
            if (staticSaturateWhiteThreshhold_dirty)
            {
                item.StaticSaturateWhiteThreshhold = this.staticSaturateWhiteThreshhold;
            }
            if (staticSaturateBlackThreshhold_dirty)
            {
                item.StaticSaturateBlackThreshhold = this.staticSaturateBlackThreshhold;
            }
            if (staticSaturateExponent_dirty)
            {
                item.StaticSaturateExponent = this.staticSaturateExponent;
            }

            if (normalizeGeometry_dirty)
            {
                item.NormalizeGeometry = this.normalizeGeometry;
            }

            if (fineRotateDegrees_dirty)
            {
                item.FineRotateDegrees = this.fineRotateDegrees;
            }

            if (cropRect_dirty)
            {
                item.CropRect = this.cropRect;
            }
        }
    }
}
