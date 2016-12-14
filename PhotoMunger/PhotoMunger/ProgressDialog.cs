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
using System.Threading;
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class ProgressDialog : Form
    {
        private int finished;
        private readonly int total;
        private readonly CancellationTokenSource cancel;

        public ProgressDialog(int maximum, int total, Form owner, CancellationTokenSource cancel)
        {
            this.total = total;
            this.cancel = cancel;

            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;
            this.Owner = owner;

            this.bumpDelegate = Bump;
            this.closeDelegate = Close;

            this.progressBar.Maximum = Math.Max(maximum, 1);
        }

        private delegate void BumpDelegate(bool significant);
        public void Bump(bool significant)
        {
            if (significant)
            {
                this.progressBar.Level++;
            }
            finished++;
            this.Text = String.Format("Progress ({0} of {1})", finished, total);
        }

        private readonly BumpDelegate bumpDelegate;
        public void BumpFromAnyThread(bool significant)
        {
            this.Invoke(bumpDelegate, new object[] { significant });
        }

        private readonly Action closeDelegate;
        public void CloseFromAnyThread()
        {
            this.Invoke(closeDelegate, null);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            cancel.Cancel();
        }
    }
}
