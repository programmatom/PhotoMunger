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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class ProgressBar : UserControl
    {
        private int level;
        private int maximum = 1;
        private Color completedColor = Color.LimeGreen;

        public ProgressBar()
        {
            InitializeComponent();
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "Firebrick")]
        public Color CompletedColor { get { return completedColor; } set { completedColor = value; } }

        protected override void OnPaint(PaintEventArgs pe)
        {
            int pixel = level * Width / maximum;
            using (Brush brush = new SolidBrush(completedColor))
            {
                pe.Graphics.FillRectangle(brush, 0, 0, pixel, Height);
            }
            using (Brush brush = new SolidBrush(BackColor))
            {
                pe.Graphics.FillRectangle(brush, pixel, 0, Width - pixel, Height);
            }

            base.OnPaint(pe);
        }

        [Bindable(true), DefaultValue(1)]
        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                if (value < 1)
                {
                    Debug.Assert(false);
                    throw new ArgumentException();
                }
                maximum = value;
                Invalidate();
                Update();
            }
        }

        [Bindable(true), DefaultValue(0)]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                Invalidate();
                Update();
            }
        }
    }
}
