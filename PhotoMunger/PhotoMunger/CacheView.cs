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
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class CacheView : Form
    {
        private readonly ImageCache cache;
        private readonly BindingList<ImageCache.EntryInfo> entries = new BindingList<ImageCache.EntryInfo>();

        public CacheView(ImageCache cache)
        {
            this.cache = cache;

            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;

            this.entryInfoBindingSource.DataSource = new BindingSource(this.entries, null);

            this.toolStripButtonEnableSwap.Checked = Program.EnableSwap;
            this.toolStripButtonEnableSwap.CheckedChanged += ToolStripButtonEnableSwap_CheckedChanged;
        }

        private void ToolStripButtonEnableSwap_CheckedChanged(object sender, EventArgs e)
        {
            Program.EnableSwap = this.toolStripButtonEnableSwap.Checked;
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            timerRefresh.Start();

            ImageCache.EntryInfo[] current = this.cache.GetCurrentEntryInfo();
            Array.Sort(current, delegate (ImageCache.EntryInfo l, ImageCache.EntryInfo r) { return l.Id.CompareTo(r.Id); });
            if (this.entries.Count == current.Length)
            {
                for (int i = 0; i < current.Length; i++)
                {
                    if (!current[i].Equals(this.entries[i]))
                    {
                        goto NotEqual;
                    }
                }
                return;

            NotEqual:
                ;
            }

            for (int i = 0; i < current.Length; i++)
            {
                if (this.entries.Count > i)
                {
                    this.entries[i] = current[i];
                }
                else
                {
                    this.entries.Add(current[i]);
                }
            }
            while (this.entries.Count > current.Length)
            {
                this.entries.RemoveAt(this.entries.Count - 1);
            }

            this.Text = String.Format(
                "Cache View ({0}, {1}/{2}, {3}/{4})",
                this.entries.Count,
                FileSizeText.GetFileSizeString(ManagedBitmap.AggregatedTotalBytes),
                FileSizeText.GetFileSizeString(ImageCache.MemoryLimit),
                FileSizeText.GetFileSizeString(cache.TotalSwappedBytes),
                FileSizeText.GetFileSizeString(Math.Max(cache.SwappedBytesLimit, 0)));
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            this.cache.TryClear();
        }
    }
}
