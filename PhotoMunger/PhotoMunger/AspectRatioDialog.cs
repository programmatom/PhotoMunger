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
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class AspectRatioDialog : Form
    {
        private static SizeF custom;
        private static int index = 1;
        private static bool portrait;

        private readonly RadioButton[] buttons;
        private SizeF current;

        private const int SpecialCases = 2;

        public AspectRatioDialog(SizeF current, string title)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;

            this.current = current;

            if ((custom.Width != 0) && (custom.Height != 0))
            {
                this.textBoxCustomX.Text = custom.Width.ToString();
                this.textBoxCustomY.Text = custom.Height.ToString();
            }

            if (portrait)
            {
                this.radioButtonPortrait.Checked = true;
            }
            else
            {
                this.radioButtonLandscape.Checked = true;
            }


            this.buttons = new RadioButton[SpecialCases + Ratios.Length];

            this.buttons[0] = this.radioButtonCustom;
            if (index == 0)
            {
                this.radioButtonCustom.Checked = true;
            }
            this.radioButtonCustom.CheckedChanged += AspectButton_CheckedChanged;

            this.buttons[1] = this.radioButtonCurrent;
            if (index == 1)
            {
                this.radioButtonCurrent.Checked = true;
            }
            this.radioButtonCurrent.Text += String.Format(" {0}x{1}", current.Width, current.Height);
            this.radioButtonCurrent.CheckedChanged += AspectButton_CheckedChanged;

            for (int i = 0; i < Ratios.Length; i++)
            {
                RadioButton item = new RadioButton();
                string label = String.Format("{0}x{1}", Ratios[i].Ratio.Width, Ratios[i].Ratio.Height);
                List<string> label2 = new List<string>();
                foreach (SizeF alias in Ratios[i].Aliases)
                {
                    label2.Add(String.Format("{0}x{1}", alias.Width, alias.Height));
                }
                foreach (string alias in Ratios[i].NamedAliases)
                {
                    label2.Add(alias);
                }
                item.Text = String.Concat(label, label2.Count != 0 ? String.Concat(" (", String.Join(", ", label2), ")") : null);
                this.buttons[i + SpecialCases] = item;
                if (index == i + SpecialCases)
                {
                    item.Checked = true;
                }
                item.CheckedChanged += AspectButton_CheckedChanged;
                item.AutoSize = true;
                item.TabIndex = this.radioButtonCustom.TabIndex;
                flowLayoutPanelAspectSelectors.Controls.Add(item);
            }

            UpdatePreview();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            portrait = this.radioButtonPortrait.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private float CustomAspectUnoriented
        {
            get
            {
                SizeF custom = new SizeF();
                float r;

                if (Single.TryParse(this.textBoxCustomX.Text, out r))
                {
                    custom.Width = r;
                }
                if (Single.TryParse(this.textBoxCustomY.Text, out r))
                {
                    custom.Height = r;
                }

                r = custom.Width / custom.Height;
                if (Single.IsNaN(r) || Single.IsInfinity(r))
                {
                    r = 1;
                }

                return r;
            }
        }

        public float Aspect
        {
            get
            {
                float r;

                if (index == 0)
                {
                    r = this.CustomAspectUnoriented;
                }
                else if (index == 1)
                {
                    r = current.Width / current.Height;
                }
                else
                {
                    r = Ratios[index - SpecialCases].Ratio.Width / Ratios[index - SpecialCases].Ratio.Height;
                }

                if (portrait)
                {
                    r = 1f / r;
                }

                return r;
            }
        }

        public bool UseCurrent { get { return index == 1; } }


        private void UpdatePreview()
        {
            int clientExtent;
            int extent;
            {
                Size size = this.pictureBox1.ClientSize;
                Debug.Assert(size.Width == size.Height);
                clientExtent = size.Width;
                extent = clientExtent - 1;
            }

            Bitmap bitmap = new Bitmap(clientExtent, clientExtent);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(SystemBrushes.Control, new Rectangle(0, 0, extent, extent));

                float aspect = this.Aspect;
                float h = extent / aspect;
                Rectangle rect = new Rectangle(
                    0,
                    (int)Math.Round((extent - h) / 2),
                    extent,
                    (int)Math.Round(h));
                if (this.radioButtonPortrait.Checked)
                {
                    rect = new Rectangle(rect.Top, rect.Left, rect.Height, rect.Width);
                }

                graphics.FillRectangle(Brushes.White, rect);
                graphics.DrawRectangle(Pens.Black, rect);
            }

            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
            }
            this.pictureBox1.Image = bitmap;
        }


        private void textBoxCustomX_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void textBoxCustomY_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }


        private bool suspendOrientationChanged = false;
        private void radioButtonPortrait_CheckedChanged(object sender, EventArgs e)
        {
            if (!suspendOrientationChanged)
            {
                suspendOrientationChanged = true;
                radioButtonPortrait.Checked = true;
                radioButtonLandscape.Checked = false;
                suspendOrientationChanged = false;

                UpdatePreview();
            }
        }
        private void radioButtonLandscape_CheckedChanged(object sender, EventArgs e)
        {
            if (!suspendOrientationChanged)
            {
                suspendOrientationChanged = true;
                radioButtonLandscape.Checked = true;
                radioButtonPortrait.Checked = false;
                suspendOrientationChanged = false;

                UpdatePreview();
            }
        }


        private bool suspendAspectChanged;
        private void AspectButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!suspendAspectChanged)
            {
                suspendAspectChanged = true;
                this.buttons[index].Checked = false;
                index = Array.IndexOf(this.buttons, sender);
                this.buttons[index].Checked = true;
                suspendAspectChanged = false;

                UpdatePreview();
            }
        }


        private struct RatioItem
        {
            public readonly SizeF Ratio;
            public readonly SizeF[] Aliases;
            public readonly string[] NamedAliases;

            public RatioItem(SizeF ratio, SizeF[] aliases, string[] namedAliases)
            {
                this.Ratio = ratio;
                this.Aliases = aliases != null ? aliases : new SizeF[0];
                this.NamedAliases = namedAliases != null ? namedAliases : new string[0];
            }
        }

        // NOTE: ratio must be at least 1 - put larger value in X (values are assumed to be landscape)
        private readonly static RatioItem[] Ratios = new RatioItem[]
        {
            new RatioItem(new SizeF(1, 1), null, null),
            new RatioItem(new SizeF(3, 2), null, new string[] { "Classic 35mm" } ),
            new RatioItem(new SizeF(4, 3), null, null),
            new RatioItem(new SizeF(5, 3), null, null),
            new RatioItem(new SizeF(5, 4), new SizeF[] { new SizeF(10, 8) }, null),
            new RatioItem(new SizeF(3, 1), null, new string[] { "APS panorama" } ),
            new RatioItem(new SizeF(16, 9), null, new string[] { "HDTV" } ),
            new RatioItem(new SizeF(7, 5), null, null),
            new RatioItem(new SizeF(8, 5), null, null),
            new RatioItem(new SizeF(1.61803398874987f, 1), null, new string[] { "golden ratio" } ),
            new RatioItem(new SizeF(11, 8.5f), null, new string[] { "Page: Letter" } ),
            new RatioItem(new SizeF(14, 8.5f), null, new string[] { "Page: Legal" } ),
            new RatioItem(new SizeF(17, 11), null, new string[] { "Page: Tabloid" } ),
            new RatioItem(new SizeF(420, 297), null, new string[] { "Page: A3" } ),
            new RatioItem(new SizeF(297, 210), null, new string[] { "Page: A4" } ),
        };
    }
}
