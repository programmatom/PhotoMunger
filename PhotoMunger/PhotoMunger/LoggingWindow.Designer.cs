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
namespace AdaptiveImageSizeReducer
{
    partial class LoggingWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoggingWindow));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonProfile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonShowCache = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowPerf = new System.Windows.Forms.ToolStripButton();
            this.textBox = new System.Windows.Forms.TextBox();
            this.toolStripButtonShowXform = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1169, 596);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonProfile,
            this.toolStripSeparator2,
            this.toolStripButtonClear,
            this.toolStripSeparator1,
            this.toolStripButtonShowCache,
            this.toolStripButtonShowPerf,
            this.toolStripButtonShowXform});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1169, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonProfile
            // 
            this.toolStripButtonProfile.CheckOnClick = true;
            this.toolStripButtonProfile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonProfile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonProfile.Image")));
            this.toolStripButtonProfile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonProfile.Name = "toolStripButtonProfile";
            this.toolStripButtonProfile.Size = new System.Drawing.Size(176, 22);
            this.toolStripButtonProfile.Text = "Profile Mode (Single Processor)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClear.Image")));
            this.toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Size = new System.Drawing.Size(38, 22);
            this.toolStripButtonClear.Text = "Clear";
            this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonShowCache
            // 
            this.toolStripButtonShowCache.Checked = true;
            this.toolStripButtonShowCache.CheckOnClick = true;
            this.toolStripButtonShowCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonShowCache.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonShowCache.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowCache.Image")));
            this.toolStripButtonShowCache.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowCache.Name = "toolStripButtonShowCache";
            this.toolStripButtonShowCache.Size = new System.Drawing.Size(44, 22);
            this.toolStripButtonShowCache.Text = "Cache";
            this.toolStripButtonShowCache.ToolTipText = "Log cache messages";
            // 
            // toolStripButtonShowPerf
            // 
            this.toolStripButtonShowPerf.Checked = true;
            this.toolStripButtonShowPerf.CheckOnClick = true;
            this.toolStripButtonShowPerf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonShowPerf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonShowPerf.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowPerf.Image")));
            this.toolStripButtonShowPerf.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowPerf.Name = "toolStripButtonShowPerf";
            this.toolStripButtonShowPerf.Size = new System.Drawing.Size(32, 22);
            this.toolStripButtonShowPerf.Text = "Perf";
            this.toolStripButtonShowPerf.ToolTipText = "Log performance messages";
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(3, 28);
            this.textBox.MaxLength = 2147483647;
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox.Size = new System.Drawing.Size(1163, 565);
            this.textBox.TabIndex = 1;
            this.textBox.WordWrap = false;
            // 
            // toolStripButtonShowXform
            // 
            this.toolStripButtonShowXform.Checked = true;
            this.toolStripButtonShowXform.CheckOnClick = true;
            this.toolStripButtonShowXform.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonShowXform.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonShowXform.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowXform.Image")));
            this.toolStripButtonShowXform.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowXform.Name = "toolStripButtonShowXform";
            this.toolStripButtonShowXform.Size = new System.Drawing.Size(44, 22);
            this.toolStripButtonShowXform.Text = "Xform";
            this.toolStripButtonShowXform.ToolTipText = "Log messages from image transforms";
            // 
            // Logging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 596);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Logging";
            this.Text = "Logging";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonProfile;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowCache;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowPerf;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowXform;
    }
}