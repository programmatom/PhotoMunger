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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class ImageBox : Control, ISupportInitialize
    {
        private Bitmap image;
        private int offsetX;
        private int offsetY;
        private Bitmap backing;

        private bool showCrosshair;
        private Color crosshairColor = SystemColors.ControlDark;
        private int crosshairX;
        private int crosshairY;

        public ImageBox()
        {
            InitializeComponent();
        }

        protected void Dispose2()
        {
            if (backing != null)
            {
                backing.Dispose();
                backing = null;
            }
        }


        // Properties

        [Category("Appearance")]
        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                ReinstallImage();
            }
        }

        [Category("Appearance")]
        public Color CrosshairColor
        {
            get
            {
                return crosshairColor;
            }
            set
            {
                crosshairColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public bool ShowCrosshair
        {
            get
            {
                return showCrosshair;
            }
            set
            {
                showCrosshair = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public int CrosshairX
        {
            get
            {
                return crosshairX;
            }
            set
            {
                crosshairX = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public int CrosshairY
        {
            get
            {
                return crosshairY;
            }
            set
            {
                crosshairY = value;
                Invalidate();
            }
        }


        // Drawing machinery

        protected override void OnPaint(PaintEventArgs e)
        {
            if (backing != null)
            {
                Rectangle imageRect = new Rectangle(offsetX, offsetY, backing.Width, backing.Height);

                if (showCrosshair)
                {
                    using (Brush brush = new SolidBrush(crosshairColor))
                    {
                        int crosshairScreenX = offsetX + crosshairX * backing.Width / image.Width;
                        int crosshairScreenY = offsetY + crosshairY * backing.Height / image.Height;

                        Rectangle horiz = new Rectangle(0, crosshairScreenY, ClientSize.Width, 1);
                        Rectangle vert = new Rectangle(crosshairScreenX, 0, 1, ClientSize.Height);

                        e.Graphics.FillRectangle(brush, horiz);
                        e.Graphics.FillRectangle(brush, vert);

                        e.Graphics.SetClip(horiz, CombineMode.Exclude);
                        e.Graphics.SetClip(vert, CombineMode.Exclude);
                    }
                }

                e.Graphics.DrawImage(backing, imageRect);

                e.Graphics.SetClip(imageRect, CombineMode.Exclude);
            }
            using (Brush background = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(background, this.ClientRectangle);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ReinstallImage();
            base.OnSizeChanged(e);
        }

        private void ReinstallImage()
        {
            if (image != null)
            {
                // This is equivalent to PictureBoxSizeMode.Zoom for PictureBox

                Size areaSize = this.ClientSize;

                Size imageSize = image.Size;
                float ratio = Math.Min((float)areaSize.Width / (float)imageSize.Width, (float)areaSize.Height / (float)imageSize.Height);

                if (backing != null)
                {
                    backing.Dispose();
                }
                backing = new Bitmap(image, (int)(imageSize.Width * ratio), (int)(imageSize.Height * ratio));

                offsetX = (ClientRectangle.Width - backing.Width) / 2;
                offsetY = (ClientRectangle.Height - backing.Height) / 2;
            }

            Invalidate();
        }


        // ISupportInitialize

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }
    }
}
