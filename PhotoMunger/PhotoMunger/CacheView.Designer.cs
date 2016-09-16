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
    partial class CacheView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CacheView));
            this.dataGridViewEntries = new System.Windows.Forms.DataGridView();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalBytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.holdersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oldBitmapsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.canDisposeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.entryInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonEnableSwap = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entryInfoBindingSource)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewEntries
            // 
            this.dataGridViewEntries.AllowUserToAddRows = false;
            this.dataGridViewEntries.AllowUserToDeleteRows = false;
            this.dataGridViewEntries.AllowUserToResizeRows = false;
            this.dataGridViewEntries.AutoGenerateColumns = false;
            this.dataGridViewEntries.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewEntries.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewEntries.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEntries.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEntries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.Priority,
            this.statusDataGridViewTextBoxColumn,
            this.TotalBytes,
            this.holdersDataGridViewTextBoxColumn,
            this.oldBitmapsDataGridViewTextBoxColumn,
            this.canDisposeDataGridViewCheckBoxColumn});
            this.dataGridViewEntries.DataSource = this.entryInfoBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEntries.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewEntries.Location = new System.Drawing.Point(3, 28);
            this.dataGridViewEntries.Name = "dataGridViewEntries";
            this.dataGridViewEntries.ReadOnly = true;
            this.dataGridViewEntries.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewEntries.RowHeadersVisible = false;
            this.dataGridViewEntries.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dataGridViewEntries.RowTemplate.Height = 15;
            this.dataGridViewEntries.Size = new System.Drawing.Size(585, 506);
            this.dataGridViewEntries.TabIndex = 1;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Width = 300;
            // 
            // Priority
            // 
            this.Priority.DataPropertyName = "Priority";
            this.Priority.HeaderText = "MRU";
            this.Priority.Name = "Priority";
            this.Priority.ReadOnly = true;
            this.Priority.Width = 35;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusDataGridViewTextBoxColumn.Width = 50;
            // 
            // TotalBytes
            // 
            this.TotalBytes.DataPropertyName = "TotalBytesString";
            this.TotalBytes.HeaderText = "Size";
            this.TotalBytes.Name = "TotalBytes";
            this.TotalBytes.ReadOnly = true;
            this.TotalBytes.Width = 50;
            // 
            // holdersDataGridViewTextBoxColumn
            // 
            this.holdersDataGridViewTextBoxColumn.DataPropertyName = "Holders";
            this.holdersDataGridViewTextBoxColumn.HeaderText = "Refs";
            this.holdersDataGridViewTextBoxColumn.Name = "holdersDataGridViewTextBoxColumn";
            this.holdersDataGridViewTextBoxColumn.ReadOnly = true;
            this.holdersDataGridViewTextBoxColumn.Width = 35;
            // 
            // oldBitmapsDataGridViewTextBoxColumn
            // 
            this.oldBitmapsDataGridViewTextBoxColumn.DataPropertyName = "OldBitmaps";
            this.oldBitmapsDataGridViewTextBoxColumn.HeaderText = "Old";
            this.oldBitmapsDataGridViewTextBoxColumn.Name = "oldBitmapsDataGridViewTextBoxColumn";
            this.oldBitmapsDataGridViewTextBoxColumn.ReadOnly = true;
            this.oldBitmapsDataGridViewTextBoxColumn.Width = 35;
            // 
            // canDisposeDataGridViewCheckBoxColumn
            // 
            this.canDisposeDataGridViewCheckBoxColumn.DataPropertyName = "CanDispose";
            this.canDisposeDataGridViewCheckBoxColumn.HeaderText = "Disposable";
            this.canDisposeDataGridViewCheckBoxColumn.Name = "canDisposeDataGridViewCheckBoxColumn";
            this.canDisposeDataGridViewCheckBoxColumn.ReadOnly = true;
            this.canDisposeDataGridViewCheckBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.canDisposeDataGridViewCheckBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.canDisposeDataGridViewCheckBoxColumn.Width = 60;
            // 
            // entryInfoBindingSource
            // 
            this.entryInfoBindingSource.DataSource = typeof(AdaptiveImageSizeReducer.ImageCache.EntryInfo);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 1000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.dataGridViewEntries, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(591, 537);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEnableSwap,
            this.toolStripButtonClear});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(591, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonEnableSwap
            // 
            this.toolStripButtonEnableSwap.CheckOnClick = true;
            this.toolStripButtonEnableSwap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonEnableSwap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEnableSwap.Image")));
            this.toolStripButtonEnableSwap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEnableSwap.Name = "toolStripButtonEnableSwap";
            this.toolStripButtonEnableSwap.Size = new System.Drawing.Size(77, 22);
            this.toolStripButtonEnableSwap.Text = "Enable Swap";
            this.toolStripButtonEnableSwap.ToolTipText = "Permit cached images to be swapped to disk";
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClear.Image")));
            this.toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Size = new System.Drawing.Size(38, 22);
            this.toolStripButtonClear.Text = "Clear";
            this.toolStripButtonClear.ToolTipText = "Clear entries from cache";
            this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
            // 
            // CacheView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 537);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "CacheView";
            this.Text = "Cache View";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entryInfoBindingSource)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewEntries;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.BindingSource entryInfoBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Priority;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalBytes;
        private System.Windows.Forms.DataGridViewTextBoxColumn holdersDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn oldBitmapsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn canDisposeDataGridViewCheckBoxColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonEnableSwap;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
    }
}