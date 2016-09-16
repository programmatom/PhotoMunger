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
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class LoggingWindow : Form
    {
        public LoggingWindow()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;

            this.toolStripButtonProfile.Checked = Program.ProfileMode;
            this.toolStripButtonProfile.CheckedChanged += ToolStripButtonProfile_CheckedChanged;
        }

        private void ToolStripButtonProfile_CheckedChanged(object sender, EventArgs e)
        {
            Program.ProfileMode = this.toolStripButtonProfile.Checked;
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            this.textBox.Clear();
        }

        public delegate void AppendDelegate(LogCat cat, string text);
        public void Append(LogCat cat, string text)
        {
            if (Disposing)
            {
                return;
            }

            switch (cat)
            {
                default:
                case LogCat.All:
                    break;
                case LogCat.Cache:
                    if (!this.toolStripButtonShowCache.Checked)
                    {
                        return;
                    }
                    break;
                case LogCat.Perf:
                    if (!this.toolStripButtonShowPerf.Checked)
                    {
                        return;
                    }
                    break;
                case LogCat.Xform:
                    if (!this.toolStripButtonShowXform.Checked)
                    {
                        return;
                    }
                    break;
            }

            int start = this.textBox.SelectionStart;
            int length = this.textBox.SelectionLength;
            bool atEnd = start == this.textBox.TextLength;

            this.textBox.AppendText(text);
            if (atEnd)
            {
                this.textBox.Select(this.textBox.TextLength, 0);
            }
            else
            {
                this.textBox.Select(start, length);
            }
            this.textBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Program.logWindow = null;
            base.OnFormClosing(e);
        }
    }

    public enum LogCat
    {
        All = 0,
        Cache = 1,
        Perf = 2,
        Xform = 3,
    }
}
