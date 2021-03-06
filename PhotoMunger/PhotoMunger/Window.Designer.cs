﻿/*
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
namespace AdaptiveImageSizeReducer
{
    partial class Window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.dataGridViewFiles = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shrinkDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonApply = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonShowOriginalInsteadOfProcessed = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowAnnotationsPrimary = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowAnnotationsDetail = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowShrunkExpandedPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUseGDIResize = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRotateLeft90 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotateRight90 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClearCrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCropKeepAspectRatio = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInvertAspectRatioForCrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCustomAspectRatio = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonNormalizeGeometryCorners = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonResetNormalizedGeometryAspectRatio = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonResetAllNormalizedGeometry = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSetNormalizedGeometryToIdentity = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGlobalOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButtonMarkColor = new System.Windows.Forms.ToolStripDropDownButton();
            this.blackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yellowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.greenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.purpleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cyanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonCatchAllMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.markToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.sortByNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unsortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.renumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.diagnosticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLogWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autocropGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unbiasGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxDetail1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxDetail2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelStats = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanelDimensions = new System.Windows.Forms.FlowLayoutPanel();
            this.labelDimensionsOriginal = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelDimensionsShrunk = new System.Windows.Forms.Label();
            this.flowLayoutPanelSize = new System.Windows.Forms.FlowLayoutPanel();
            this.labelSizeOriginal = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelSizeShrunk = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.flowLayoutPanelCropRect = new System.Windows.Forms.FlowLayoutPanel();
            this.labelCropRect = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxBrightAdjust = new System.Windows.Forms.CheckBox();
            this.checkBoxPolyUnbias = new System.Windows.Forms.CheckBox();
            this.checkBoxNormalizeGeometry = new System.Windows.Forms.CheckBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.statusHue = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.statusSaturation = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.statusValue = new System.Windows.Forms.Label();
            this.panelMainPictureContainer = new System.Windows.Forms.Panel();
            this.pictureBoxMain = new AdaptiveImageSizeReducer.ImageBox();
            this.toolTipDetail2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipDetail1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.flowLayoutPanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetail1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetail2)).BeginInit();
            this.tableLayoutPanelStats.SuspendLayout();
            this.flowLayoutPanelDimensions.SuspendLayout();
            this.flowLayoutPanelSize.SuspendLayout();
            this.flowLayoutPanelCropRect.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panelMainPictureContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewFiles
            // 
            this.dataGridViewFiles.AllowUserToAddRows = false;
            this.dataGridViewFiles.AllowUserToDeleteRows = false;
            this.dataGridViewFiles.AllowUserToResizeRows = false;
            this.dataGridViewFiles.AutoGenerateColumns = false;
            this.dataGridViewFiles.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.shrinkDataGridViewCheckBoxColumn,
            this.Delete,
            this.FileSize});
            this.dataGridViewFiles.DataSource = this.dataBindingSource;
            this.dataGridViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewFiles.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewFiles.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridViewFiles.Name = "dataGridViewFiles";
            this.dataGridViewFiles.RowHeadersVisible = false;
            this.dataGridViewFiles.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewFiles.Size = new System.Drawing.Size(120, 605);
            this.dataGridViewFiles.TabIndex = 0;
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FileName.DataPropertyName = "RenamedFileName";
            this.FileName.HeaderText = "Name";
            this.FileName.Name = "FileName";
            // 
            // shrinkDataGridViewCheckBoxColumn
            // 
            this.shrinkDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.shrinkDataGridViewCheckBoxColumn.DataPropertyName = "Shrink";
            this.shrinkDataGridViewCheckBoxColumn.HeaderText = "Shrink";
            this.shrinkDataGridViewCheckBoxColumn.Name = "shrinkDataGridViewCheckBoxColumn";
            this.shrinkDataGridViewCheckBoxColumn.Width = 23;
            // 
            // Delete
            // 
            this.Delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Delete.DataPropertyName = "Delete";
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Width = 23;
            // 
            // FileSize
            // 
            this.FileSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FileSize.DataPropertyName = "FileSize";
            this.FileSize.HeaderText = "Size";
            this.FileSize.Name = "FileSize";
            this.FileSize.ReadOnly = true;
            this.FileSize.Width = 52;
            // 
            // dataBindingSource
            // 
            this.dataBindingSource.DataSource = typeof(AdaptiveImageSizeReducer.Item);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dataGridViewFiles);
            this.splitContainer.Panel1MinSize = 100;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanel);
            this.splitContainer.Size = new System.Drawing.Size(928, 605);
            this.splitContainer.SplitterDistance = 120;
            this.splitContainer.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.panelMainPictureContainer, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(804, 605);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.Controls.Add(this.toolStrip);
            this.flowLayoutPanel.Controls.Add(this.pictureBoxDetail1);
            this.flowLayoutPanel.Controls.Add(this.pictureBoxDetail2);
            this.flowLayoutPanel.Controls.Add(this.tableLayoutPanelStats);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(577, 0);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(227, 605);
            this.flowLayoutPanel.TabIndex = 0;
            this.flowLayoutPanel.WrapContents = false;
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonApply,
            this.toolStripSeparator1,
            this.toolStripButtonShowOriginalInsteadOfProcessed,
            this.toolStripButtonShowAnnotationsPrimary,
            this.toolStripButtonShowAnnotationsDetail,
            this.toolStripButtonShowShrunkExpandedPreview,
            this.toolStripButtonUseGDIResize,
            this.toolStripSeparator2,
            this.toolStripButtonRotateLeft90,
            this.toolStripButtonRotateRight90,
            this.toolStripSeparator6,
            this.toolStripButtonCrop,
            this.toolStripButtonClearCrop,
            this.toolStripButtonCropKeepAspectRatio,
            this.toolStripButtonInvertAspectRatioForCrop,
            this.toolStripButtonCustomAspectRatio,
            this.toolStripSeparator4,
            this.toolStripButtonNormalizeGeometryCorners,
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly,
            this.toolStripButtonResetNormalizedGeometryAspectRatio,
            this.toolStripButtonResetAllNormalizedGeometry,
            this.toolStripButtonSetNormalizedGeometryToIdentity,
            this.toolStripSeparator3,
            this.toolStripButtonOptions,
            this.toolStripButtonGlobalOptions,
            this.toolStripSeparator5,
            this.toolStripDropDownButtonMarkColor,
            this.toolStripDropDownButtonCatchAllMenu});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(227, 69);
            this.toolStrip.TabIndex = 3;
            // 
            // toolStripButtonApply
            // 
            this.toolStripButtonApply.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonApply.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonApply.Image")));
            this.toolStripButtonApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonApply.Name = "toolStripButtonApply";
            this.toolStripButtonApply.Size = new System.Drawing.Size(42, 19);
            this.toolStripButtonApply.Text = "Apply";
            this.toolStripButtonApply.ToolTipText = "Apply transforms to image collection";
            this.toolStripButtonApply.Click += new System.EventHandler(this.toolStripButtonApply_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonShowOriginalInsteadOfProcessed
            // 
            this.toolStripButtonShowOriginalInsteadOfProcessed.CheckOnClick = true;
            this.toolStripButtonShowOriginalInsteadOfProcessed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowOriginalInsteadOfProcessed.Image = global::AdaptiveImageSizeReducer.Properties.Resources.ShowOriginalOrTransformed;
            this.toolStripButtonShowOriginalInsteadOfProcessed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowOriginalInsteadOfProcessed.Name = "toolStripButtonShowOriginalInsteadOfProcessed";
            this.toolStripButtonShowOriginalInsteadOfProcessed.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonShowOriginalInsteadOfProcessed.ToolTipText = "Show original image instead of processed image";
            // 
            // toolStripButtonShowAnnotationsPrimary
            // 
            this.toolStripButtonShowAnnotationsPrimary.Checked = true;
            this.toolStripButtonShowAnnotationsPrimary.CheckOnClick = true;
            this.toolStripButtonShowAnnotationsPrimary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonShowAnnotationsPrimary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowAnnotationsPrimary.Image = global::AdaptiveImageSizeReducer.Properties.Resources.AnnotationButton;
            this.toolStripButtonShowAnnotationsPrimary.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowAnnotationsPrimary.Name = "toolStripButtonShowAnnotationsPrimary";
            this.toolStripButtonShowAnnotationsPrimary.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonShowAnnotationsPrimary.Text = "Show/Hide Annotations";
            this.toolStripButtonShowAnnotationsPrimary.ToolTipText = "Show/hide annotations in primary view";
            // 
            // toolStripButtonShowAnnotationsDetail
            // 
            this.toolStripButtonShowAnnotationsDetail.CheckOnClick = true;
            this.toolStripButtonShowAnnotationsDetail.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowAnnotationsDetail.Image = global::AdaptiveImageSizeReducer.Properties.Resources.AnnotationDetailButton;
            this.toolStripButtonShowAnnotationsDetail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowAnnotationsDetail.Name = "toolStripButtonShowAnnotationsDetail";
            this.toolStripButtonShowAnnotationsDetail.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonShowAnnotationsDetail.Text = "Show/Hide Annotations in Detail";
            this.toolStripButtonShowAnnotationsDetail.ToolTipText = "Show/hide annotations in detail view";
            // 
            // toolStripButtonShowShrunkExpandedPreview
            // 
            this.toolStripButtonShowShrunkExpandedPreview.Checked = true;
            this.toolStripButtonShowShrunkExpandedPreview.CheckOnClick = true;
            this.toolStripButtonShowShrunkExpandedPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonShowShrunkExpandedPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowShrunkExpandedPreview.Image = global::AdaptiveImageSizeReducer.Properties.Resources.ShowShrunkExpandedPreview;
            this.toolStripButtonShowShrunkExpandedPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowShrunkExpandedPreview.Name = "toolStripButtonShowShrunkExpandedPreview";
            this.toolStripButtonShowShrunkExpandedPreview.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonShowShrunkExpandedPreview.ToolTipText = "Show shrunk-expanded image in output preview";
            // 
            // toolStripButtonUseGDIResize
            // 
            this.toolStripButtonUseGDIResize.CheckOnClick = true;
            this.toolStripButtonUseGDIResize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUseGDIResize.Image = global::AdaptiveImageSizeReducer.Properties.Resources.UseGDIResize;
            this.toolStripButtonUseGDIResize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUseGDIResize.Name = "toolStripButtonUseGDIResize";
            this.toolStripButtonUseGDIResize.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonUseGDIResize.ToolTipText = "Use GDI resize instead of WPF (previews Bicubic resize, but very slow)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonRotateLeft90
            // 
            this.toolStripButtonRotateLeft90.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRotateLeft90.Image = global::AdaptiveImageSizeReducer.Properties.Resources.RotateLeft90;
            this.toolStripButtonRotateLeft90.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotateLeft90.Name = "toolStripButtonRotateLeft90";
            this.toolStripButtonRotateLeft90.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonRotateLeft90.ToolTipText = "Rotate counter-clockwise 90 degrees";
            this.toolStripButtonRotateLeft90.Click += new System.EventHandler(this.toolStripButtonRotateLeft90_Click);
            // 
            // toolStripButtonRotateRight90
            // 
            this.toolStripButtonRotateRight90.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRotateRight90.Image = global::AdaptiveImageSizeReducer.Properties.Resources.RotateRight90;
            this.toolStripButtonRotateRight90.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotateRight90.Name = "toolStripButtonRotateRight90";
            this.toolStripButtonRotateRight90.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonRotateRight90.ToolTipText = "Rotate clockwise 90 degrees";
            this.toolStripButtonRotateRight90.Click += new System.EventHandler(this.toolStripButtonRotateRight90_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonCrop
            // 
            this.toolStripButtonCrop.CheckOnClick = true;
            this.toolStripButtonCrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCrop.Image = global::AdaptiveImageSizeReducer.Properties.Resources.CropButton;
            this.toolStripButtonCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCrop.Name = "toolStripButtonCrop";
            this.toolStripButtonCrop.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonCrop.Text = "Set Crop";
            this.toolStripButtonCrop.ToolTipText = "Select crop area";
            // 
            // toolStripButtonClearCrop
            // 
            this.toolStripButtonClearCrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClearCrop.Image = global::AdaptiveImageSizeReducer.Properties.Resources.ClearCropButton;
            this.toolStripButtonClearCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClearCrop.Name = "toolStripButtonClearCrop";
            this.toolStripButtonClearCrop.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonClearCrop.Text = "Clear Crop";
            this.toolStripButtonClearCrop.ToolTipText = "Clear crop selection";
            // 
            // toolStripButtonCropKeepAspectRatio
            // 
            this.toolStripButtonCropKeepAspectRatio.CheckOnClick = true;
            this.toolStripButtonCropKeepAspectRatio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCropKeepAspectRatio.Image = global::AdaptiveImageSizeReducer.Properties.Resources.CropRetainAspectRatioButton;
            this.toolStripButtonCropKeepAspectRatio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCropKeepAspectRatio.Name = "toolStripButtonCropKeepAspectRatio";
            this.toolStripButtonCropKeepAspectRatio.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonCropKeepAspectRatio.Text = "Preserve Aspect Ratio";
            this.toolStripButtonCropKeepAspectRatio.ToolTipText = "Preserve aspect ratio when selecting crop area";
            // 
            // toolStripButtonInvertAspectRatioForCrop
            // 
            this.toolStripButtonInvertAspectRatioForCrop.CheckOnClick = true;
            this.toolStripButtonInvertAspectRatioForCrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInvertAspectRatioForCrop.Image = global::AdaptiveImageSizeReducer.Properties.Resources.InvertAspectRatioForCrop;
            this.toolStripButtonInvertAspectRatioForCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInvertAspectRatioForCrop.Name = "toolStripButtonInvertAspectRatioForCrop";
            this.toolStripButtonInvertAspectRatioForCrop.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonInvertAspectRatioForCrop.ToolTipText = "Flip aspect ratio constraint for crop";
            // 
            // toolStripButtonCustomAspectRatio
            // 
            this.toolStripButtonCustomAspectRatio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCustomAspectRatio.Image = global::AdaptiveImageSizeReducer.Properties.Resources.CustomCropAspectRatio;
            this.toolStripButtonCustomAspectRatio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCustomAspectRatio.Name = "toolStripButtonCustomAspectRatio";
            this.toolStripButtonCustomAspectRatio.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonCustomAspectRatio.ToolTipText = "Select preset or custom aspect ratio for crop";
            this.toolStripButtonCustomAspectRatio.Click += new System.EventHandler(this.toolStripButtonCustomAspectRatio_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonNormalizeGeometryCorners
            // 
            this.toolStripButtonNormalizeGeometryCorners.CheckOnClick = true;
            this.toolStripButtonNormalizeGeometryCorners.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNormalizeGeometryCorners.Image = global::AdaptiveImageSizeReducer.Properties.Resources.GeometryCorners;
            this.toolStripButtonNormalizeGeometryCorners.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNormalizeGeometryCorners.Name = "toolStripButtonNormalizeGeometryCorners";
            this.toolStripButtonNormalizeGeometryCorners.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonNormalizeGeometryCorners.ToolTipText = "Edit corners for normalize geometry";
            // 
            // toolStripButtonSetNormalizedGeometryAspectRatioExplicitly
            // 
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.Image = global::AdaptiveImageSizeReducer.Properties.Resources.SetNormalizeGeometryAspect;
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.Name = "toolStripButtonSetNormalizedGeometryAspectRatioExplicitly";
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.ToolTipText = "Set normalized geometry forced aspect ratio explicitly";
            this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly.Click += new System.EventHandler(this.toolStripButtonSetNormalizedGeometryAspectRatioExplicitly_Click);
            // 
            // toolStripButtonResetNormalizedGeometryAspectRatio
            // 
            this.toolStripButtonResetNormalizedGeometryAspectRatio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonResetNormalizedGeometryAspectRatio.Image = global::AdaptiveImageSizeReducer.Properties.Resources.ResetNormalizeGeometryAspect;
            this.toolStripButtonResetNormalizedGeometryAspectRatio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonResetNormalizedGeometryAspectRatio.Name = "toolStripButtonResetNormalizedGeometryAspectRatio";
            this.toolStripButtonResetNormalizedGeometryAspectRatio.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonResetNormalizedGeometryAspectRatio.ToolTipText = "Reset normalized geometry aspect ratio to globally specified value";
            this.toolStripButtonResetNormalizedGeometryAspectRatio.Click += new System.EventHandler(this.toolStripButtonResetNormalizedGeometryAspectRatio_Click);
            // 
            // toolStripButtonResetAllNormalizedGeometry
            // 
            this.toolStripButtonResetAllNormalizedGeometry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonResetAllNormalizedGeometry.Image = global::AdaptiveImageSizeReducer.Properties.Resources.ResetNormalizeGeometryToGlobal;
            this.toolStripButtonResetAllNormalizedGeometry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonResetAllNormalizedGeometry.Name = "toolStripButtonResetAllNormalizedGeometry";
            this.toolStripButtonResetAllNormalizedGeometry.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonResetAllNormalizedGeometry.ToolTipText = "Reset normalized geometry to automatically detected values";
            this.toolStripButtonResetAllNormalizedGeometry.Click += new System.EventHandler(this.toolStripButtonResetAllNormalizedGeometry_Click);
            // 
            // toolStripButtonSetNormalizedGeometryToIdentity
            // 
            this.toolStripButtonSetNormalizedGeometryToIdentity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetNormalizedGeometryToIdentity.Image = global::AdaptiveImageSizeReducer.Properties.Resources.SetNormalizeGeometryRectToIdentity;
            this.toolStripButtonSetNormalizedGeometryToIdentity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetNormalizedGeometryToIdentity.Name = "toolStripButtonSetNormalizedGeometryToIdentity";
            this.toolStripButtonSetNormalizedGeometryToIdentity.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonSetNormalizedGeometryToIdentity.ToolTipText = "Set normalized geometry rectangle to identity transform";
            this.toolStripButtonSetNormalizedGeometryToIdentity.Click += new System.EventHandler(this.toolStripButtonResetNormalizedGeometryToIdentity_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonOptions
            // 
            this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOptions.Image = global::AdaptiveImageSizeReducer.Properties.Resources.OpenOptionsDialog;
            this.toolStripButtonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOptions.Name = "toolStripButtonOptions";
            this.toolStripButtonOptions.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonOptions.ToolTipText = "Change processing options for this image";
            this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStripButtonOptions_Click);
            // 
            // toolStripButtonGlobalOptions
            // 
            this.toolStripButtonGlobalOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGlobalOptions.Image = global::AdaptiveImageSizeReducer.Properties.Resources.OpenGlobalOptionsDialog;
            this.toolStripButtonGlobalOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGlobalOptions.Name = "toolStripButtonGlobalOptions";
            this.toolStripButtonGlobalOptions.Size = new System.Drawing.Size(23, 20);
            this.toolStripButtonGlobalOptions.ToolTipText = "Change global processing options and analyze again";
            this.toolStripButtonGlobalOptions.Click += new System.EventHandler(this.toolStripButtonGlobalOptions_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripDropDownButtonMarkColor
            // 
            this.toolStripDropDownButtonMarkColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonMarkColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blackToolStripMenuItem,
            this.blueToolStripMenuItem,
            this.redToolStripMenuItem,
            this.yellowToolStripMenuItem,
            this.orangeToolStripMenuItem,
            this.greenToolStripMenuItem,
            this.purpleToolStripMenuItem,
            this.cyanToolStripMenuItem,
            this.pinkToolStripMenuItem});
            this.toolStripDropDownButtonMarkColor.Image = global::AdaptiveImageSizeReducer.Properties.Resources.TagColor;
            this.toolStripDropDownButtonMarkColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonMarkColor.Name = "toolStripDropDownButtonMarkColor";
            this.toolStripDropDownButtonMarkColor.Size = new System.Drawing.Size(29, 20);
            this.toolStripDropDownButtonMarkColor.ToolTipText = "Mark item name with color";
            // 
            // blackToolStripMenuItem
            // 
            this.blackToolStripMenuItem.Name = "blackToolStripMenuItem";
            this.blackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D1)));
            this.blackToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.blackToolStripMenuItem.Text = "Black";
            // 
            // blueToolStripMenuItem
            // 
            this.blueToolStripMenuItem.Name = "blueToolStripMenuItem";
            this.blueToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D2)));
            this.blueToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.blueToolStripMenuItem.Text = "Blue";
            // 
            // redToolStripMenuItem
            // 
            this.redToolStripMenuItem.Name = "redToolStripMenuItem";
            this.redToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D3)));
            this.redToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.redToolStripMenuItem.Text = "Red";
            // 
            // yellowToolStripMenuItem
            // 
            this.yellowToolStripMenuItem.Name = "yellowToolStripMenuItem";
            this.yellowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D4)));
            this.yellowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.yellowToolStripMenuItem.Text = "Yellow";
            // 
            // orangeToolStripMenuItem
            // 
            this.orangeToolStripMenuItem.Name = "orangeToolStripMenuItem";
            this.orangeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D5)));
            this.orangeToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.orangeToolStripMenuItem.Text = "Orange";
            // 
            // greenToolStripMenuItem
            // 
            this.greenToolStripMenuItem.Name = "greenToolStripMenuItem";
            this.greenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D6)));
            this.greenToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.greenToolStripMenuItem.Text = "Green";
            // 
            // purpleToolStripMenuItem
            // 
            this.purpleToolStripMenuItem.Name = "purpleToolStripMenuItem";
            this.purpleToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D7)));
            this.purpleToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.purpleToolStripMenuItem.Text = "Purple";
            // 
            // cyanToolStripMenuItem
            // 
            this.cyanToolStripMenuItem.Name = "cyanToolStripMenuItem";
            this.cyanToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D8)));
            this.cyanToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.cyanToolStripMenuItem.Text = "Cyan";
            // 
            // pinkToolStripMenuItem
            // 
            this.pinkToolStripMenuItem.Name = "pinkToolStripMenuItem";
            this.pinkToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D9)));
            this.pinkToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.pinkToolStripMenuItem.Text = "Pink";
            // 
            // toolStripDropDownButtonCatchAllMenu
            // 
            this.toolStripDropDownButtonCatchAllMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonCatchAllMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markToolStripMenuItem,
            this.swapToolStripMenuItem,
            this.toolStripMenuItem4,
            this.duplicateItemToolStripMenuItem,
            this.toolStripMenuItem1,
            this.sortByNameToolStripMenuItem,
            this.unsortToolStripMenuItem,
            this.toolStripMenuItem2,
            this.renumberToolStripMenuItem,
            this.toolStripMenuItem3,
            this.diagnosticsToolStripMenuItem});
            this.toolStripDropDownButtonCatchAllMenu.Image = global::AdaptiveImageSizeReducer.Properties.Resources.CatchAllMenu;
            this.toolStripDropDownButtonCatchAllMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonCatchAllMenu.Name = "toolStripDropDownButtonCatchAllMenu";
            this.toolStripDropDownButtonCatchAllMenu.Size = new System.Drawing.Size(29, 20);
            // 
            // markToolStripMenuItem
            // 
            this.markToolStripMenuItem.Name = "markToolStripMenuItem";
            this.markToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.M)));
            this.markToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.markToolStripMenuItem.Text = "Mark";
            this.markToolStripMenuItem.Click += new System.EventHandler(this.markToolStripMenuItem_Click);
            // 
            // swapToolStripMenuItem
            // 
            this.swapToolStripMenuItem.Name = "swapToolStripMenuItem";
            this.swapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.swapToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.swapToolStripMenuItem.Text = "Swap";
            this.swapToolStripMenuItem.Click += new System.EventHandler(this.swapToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(211, 6);
            // 
            // sortByNameToolStripMenuItem
            // 
            this.sortByNameToolStripMenuItem.Name = "sortByNameToolStripMenuItem";
            this.sortByNameToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.sortByNameToolStripMenuItem.Text = "Sort Items By Target Name";
            this.sortByNameToolStripMenuItem.Click += new System.EventHandler(this.sortByNameToolStripMenuItem_Click);
            // 
            // unsortToolStripMenuItem
            // 
            this.unsortToolStripMenuItem.Name = "unsortToolStripMenuItem";
            this.unsortToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.unsortToolStripMenuItem.Text = "Reset To Original Order";
            this.unsortToolStripMenuItem.Click += new System.EventHandler(this.unsortToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(211, 6);
            // 
            // renumberToolStripMenuItem
            // 
            this.renumberToolStripMenuItem.Name = "renumberToolStripMenuItem";
            this.renumberToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.renumberToolStripMenuItem.Text = "Renumber...";
            this.renumberToolStripMenuItem.Click += new System.EventHandler(this.renumberToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(211, 6);
            // 
            // diagnosticsToolStripMenuItem
            // 
            this.diagnosticsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showCacheToolStripMenuItem,
            this.refreshImageToolStripMenuItem,
            this.showLogWindowToolStripMenuItem,
            this.autocropGridToolStripMenuItem,
            this.unbiasGridToolStripMenuItem});
            this.diagnosticsToolStripMenuItem.Name = "diagnosticsToolStripMenuItem";
            this.diagnosticsToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.diagnosticsToolStripMenuItem.Text = "Diagnostics";
            // 
            // showCacheToolStripMenuItem
            // 
            this.showCacheToolStripMenuItem.Name = "showCacheToolStripMenuItem";
            this.showCacheToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.showCacheToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.showCacheToolStripMenuItem.Text = "Show Cache";
            this.showCacheToolStripMenuItem.Click += new System.EventHandler(this.showCacheToolStripMenuItem_Click);
            // 
            // refreshImageToolStripMenuItem
            // 
            this.refreshImageToolStripMenuItem.Name = "refreshImageToolStripMenuItem";
            this.refreshImageToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshImageToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.refreshImageToolStripMenuItem.Text = "Refresh Image";
            this.refreshImageToolStripMenuItem.Click += new System.EventHandler(this.refreshImageToolStripMenuItem_Click);
            // 
            // showLogWindowToolStripMenuItem
            // 
            this.showLogWindowToolStripMenuItem.Name = "showLogWindowToolStripMenuItem";
            this.showLogWindowToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.showLogWindowToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.showLogWindowToolStripMenuItem.Text = "Show Log Window";
            this.showLogWindowToolStripMenuItem.Click += new System.EventHandler(this.showLogWindowToolStripMenuItem_Click);
            // 
            // autocropGridToolStripMenuItem
            // 
            this.autocropGridToolStripMenuItem.Name = "autocropGridToolStripMenuItem";
            this.autocropGridToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.autocropGridToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.autocropGridToolStripMenuItem.Text = "Autocrop Grid";
            this.autocropGridToolStripMenuItem.Click += new System.EventHandler(this.autocropGridToolStripMenuItem_Click);
            // 
            // unbiasGridToolStripMenuItem
            // 
            this.unbiasGridToolStripMenuItem.Name = "unbiasGridToolStripMenuItem";
            this.unbiasGridToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.unbiasGridToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.unbiasGridToolStripMenuItem.Text = "Unbias Grid";
            this.unbiasGridToolStripMenuItem.Click += new System.EventHandler(this.unbiasGridToolStripMenuItem_Click);
            // 
            // pictureBoxDetail1
            // 
            this.pictureBoxDetail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxDetail1.Location = new System.Drawing.Point(1, 69);
            this.pictureBoxDetail1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.pictureBoxDetail1.MinimumSize = new System.Drawing.Size(225, 225);
            this.pictureBoxDetail1.Name = "pictureBoxDetail1";
            this.pictureBoxDetail1.Size = new System.Drawing.Size(225, 225);
            this.pictureBoxDetail1.TabIndex = 0;
            this.pictureBoxDetail1.TabStop = false;
            // 
            // pictureBoxDetail2
            // 
            this.pictureBoxDetail2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxDetail2.Location = new System.Drawing.Point(1, 295);
            this.pictureBoxDetail2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.pictureBoxDetail2.MinimumSize = new System.Drawing.Size(225, 225);
            this.pictureBoxDetail2.Name = "pictureBoxDetail2";
            this.pictureBoxDetail2.Size = new System.Drawing.Size(225, 225);
            this.pictureBoxDetail2.TabIndex = 1;
            this.pictureBoxDetail2.TabStop = false;
            // 
            // tableLayoutPanelStats
            // 
            this.tableLayoutPanelStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelStats.AutoSize = true;
            this.tableLayoutPanelStats.ColumnCount = 3;
            this.tableLayoutPanelStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelStats.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelStats.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanelStats.Controls.Add(this.flowLayoutPanelDimensions, 2, 0);
            this.tableLayoutPanelStats.Controls.Add(this.flowLayoutPanelSize, 2, 1);
            this.tableLayoutPanelStats.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanelStats.Controls.Add(this.flowLayoutPanelCropRect, 2, 2);
            this.tableLayoutPanelStats.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanelStats.Controls.Add(this.labelMessage, 0, 5);
            this.tableLayoutPanelStats.Controls.Add(this.flowLayoutPanel2, 0, 3);
            this.tableLayoutPanelStats.Location = new System.Drawing.Point(3, 524);
            this.tableLayoutPanelStats.Name = "tableLayoutPanelStats";
            this.tableLayoutPanelStats.RowCount = 6;
            this.tableLayoutPanelStats.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelStats.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelStats.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelStats.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelStats.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelStats.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelStats.Size = new System.Drawing.Size(221, 109);
            this.tableLayoutPanelStats.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dimensions:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Size:";
            // 
            // flowLayoutPanelDimensions
            // 
            this.flowLayoutPanelDimensions.AutoSize = true;
            this.flowLayoutPanelDimensions.Controls.Add(this.labelDimensionsOriginal);
            this.flowLayoutPanelDimensions.Controls.Add(this.label3);
            this.flowLayoutPanelDimensions.Controls.Add(this.labelDimensionsShrunk);
            this.flowLayoutPanelDimensions.Location = new System.Drawing.Point(79, 3);
            this.flowLayoutPanelDimensions.Name = "flowLayoutPanelDimensions";
            this.flowLayoutPanelDimensions.Size = new System.Drawing.Size(44, 13);
            this.flowLayoutPanelDimensions.TabIndex = 2;
            // 
            // labelDimensionsOriginal
            // 
            this.labelDimensionsOriginal.AutoSize = true;
            this.labelDimensionsOriginal.Location = new System.Drawing.Point(0, 0);
            this.labelDimensionsOriginal.Margin = new System.Windows.Forms.Padding(0);
            this.labelDimensionsOriginal.Name = "labelDimensionsOriginal";
            this.labelDimensionsOriginal.Size = new System.Drawing.Size(13, 13);
            this.labelDimensionsOriginal.TabIndex = 0;
            this.labelDimensionsOriginal.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "/";
            // 
            // labelDimensionsShrunk
            // 
            this.labelDimensionsShrunk.AutoSize = true;
            this.labelDimensionsShrunk.Location = new System.Drawing.Point(31, 0);
            this.labelDimensionsShrunk.Margin = new System.Windows.Forms.Padding(0);
            this.labelDimensionsShrunk.Name = "labelDimensionsShrunk";
            this.labelDimensionsShrunk.Size = new System.Drawing.Size(13, 13);
            this.labelDimensionsShrunk.TabIndex = 2;
            this.labelDimensionsShrunk.Text = "0";
            // 
            // flowLayoutPanelSize
            // 
            this.flowLayoutPanelSize.AutoSize = true;
            this.flowLayoutPanelSize.Controls.Add(this.labelSizeOriginal);
            this.flowLayoutPanelSize.Controls.Add(this.label4);
            this.flowLayoutPanelSize.Controls.Add(this.labelSizeShrunk);
            this.flowLayoutPanelSize.Controls.Add(this.label6);
            this.flowLayoutPanelSize.Location = new System.Drawing.Point(79, 22);
            this.flowLayoutPanelSize.Name = "flowLayoutPanelSize";
            this.flowLayoutPanelSize.Size = new System.Drawing.Size(74, 13);
            this.flowLayoutPanelSize.TabIndex = 3;
            // 
            // labelSizeOriginal
            // 
            this.labelSizeOriginal.AutoSize = true;
            this.labelSizeOriginal.Location = new System.Drawing.Point(0, 0);
            this.labelSizeOriginal.Margin = new System.Windows.Forms.Padding(0);
            this.labelSizeOriginal.Name = "labelSizeOriginal";
            this.labelSizeOriginal.Size = new System.Drawing.Size(13, 13);
            this.labelSizeOriginal.TabIndex = 0;
            this.labelSizeOriginal.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "/";
            // 
            // labelSizeShrunk
            // 
            this.labelSizeShrunk.AutoSize = true;
            this.labelSizeShrunk.Location = new System.Drawing.Point(31, 0);
            this.labelSizeShrunk.Margin = new System.Windows.Forms.Padding(0);
            this.labelSizeShrunk.Name = "labelSizeShrunk";
            this.labelSizeShrunk.Size = new System.Drawing.Size(13, 13);
            this.labelSizeShrunk.TabIndex = 2;
            this.labelSizeShrunk.Text = "0";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "(est.)";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 41);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Crop Rect:";
            // 
            // flowLayoutPanelCropRect
            // 
            this.flowLayoutPanelCropRect.AutoSize = true;
            this.flowLayoutPanelCropRect.Controls.Add(this.labelCropRect);
            this.flowLayoutPanelCropRect.Location = new System.Drawing.Point(79, 41);
            this.flowLayoutPanelCropRect.Name = "flowLayoutPanelCropRect";
            this.flowLayoutPanelCropRect.Size = new System.Drawing.Size(33, 13);
            this.flowLayoutPanelCropRect.TabIndex = 5;
            // 
            // labelCropRect
            // 
            this.labelCropRect.AutoSize = true;
            this.labelCropRect.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "CropRectText", true));
            this.labelCropRect.Location = new System.Drawing.Point(0, 0);
            this.labelCropRect.Margin = new System.Windows.Forms.Padding(0);
            this.labelCropRect.Name = "labelCropRect";
            this.labelCropRect.Size = new System.Drawing.Size(33, 13);
            this.labelCropRect.TabIndex = 0;
            this.labelCropRect.Text = "None";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelStats.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxBrightAdjust);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxPolyUnbias);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxNormalizeGeometry);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 76);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(221, 20);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // checkBoxBrightAdjust
            // 
            this.checkBoxBrightAdjust.AutoSize = true;
            this.checkBoxBrightAdjust.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "BrightAdjust", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBrightAdjust.Location = new System.Drawing.Point(3, 3);
            this.checkBoxBrightAdjust.Name = "checkBoxBrightAdjust";
            this.checkBoxBrightAdjust.Size = new System.Drawing.Size(85, 17);
            this.checkBoxBrightAdjust.TabIndex = 0;
            this.checkBoxBrightAdjust.Text = "Bright Adjust";
            this.checkBoxBrightAdjust.UseVisualStyleBackColor = true;
            // 
            // checkBoxPolyUnbias
            // 
            this.checkBoxPolyUnbias.AutoSize = true;
            this.checkBoxPolyUnbias.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "Unbias", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxPolyUnbias.Location = new System.Drawing.Point(94, 3);
            this.checkBoxPolyUnbias.Name = "checkBoxPolyUnbias";
            this.checkBoxPolyUnbias.Size = new System.Drawing.Size(59, 17);
            this.checkBoxPolyUnbias.TabIndex = 1;
            this.checkBoxPolyUnbias.Text = "Unbias";
            this.checkBoxPolyUnbias.UseVisualStyleBackColor = true;
            // 
            // checkBoxNormalizeGeometry
            // 
            this.checkBoxNormalizeGeometry.AutoSize = true;
            this.checkBoxNormalizeGeometry.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "NormalizeGeometry", true));
            this.checkBoxNormalizeGeometry.Location = new System.Drawing.Point(159, 3);
            this.checkBoxNormalizeGeometry.Name = "checkBoxNormalizeGeometry";
            this.checkBoxNormalizeGeometry.Size = new System.Drawing.Size(46, 17);
            this.checkBoxNormalizeGeometry.TabIndex = 2;
            this.checkBoxNormalizeGeometry.Text = "Geo";
            this.checkBoxNormalizeGeometry.UseVisualStyleBackColor = true;
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.AutoSize = true;
            this.tableLayoutPanelStats.SetColumnSpan(this.labelMessage, 3);
            this.labelMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "Message", true));
            this.labelMessage.Location = new System.Drawing.Point(0, 96);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(221, 13);
            this.labelMessage.TabIndex = 7;
            this.labelMessage.UseMnemonic = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.tableLayoutPanelStats.SetColumnSpan(this.flowLayoutPanel2, 3);
            this.flowLayoutPanel2.Controls.Add(this.label7);
            this.flowLayoutPanel2.Controls.Add(this.statusHue);
            this.flowLayoutPanel2.Controls.Add(this.label9);
            this.flowLayoutPanel2.Controls.Add(this.statusSaturation);
            this.flowLayoutPanel2.Controls.Add(this.label12);
            this.flowLayoutPanel2.Controls.Add(this.statusValue);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 60);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(221, 13);
            this.flowLayoutPanel2.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "H:";
            // 
            // statusHue
            // 
            this.statusHue.AutoSize = true;
            this.statusHue.Location = new System.Drawing.Point(18, 0);
            this.statusHue.Margin = new System.Windows.Forms.Padding(0);
            this.statusHue.Name = "statusHue";
            this.statusHue.Size = new System.Drawing.Size(0, 13);
            this.statusHue.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 0);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "S:";
            // 
            // statusSaturation
            // 
            this.statusSaturation.AutoSize = true;
            this.statusSaturation.Location = new System.Drawing.Point(35, 0);
            this.statusSaturation.Margin = new System.Windows.Forms.Padding(0);
            this.statusSaturation.Name = "statusSaturation";
            this.statusSaturation.Size = new System.Drawing.Size(0, 13);
            this.statusSaturation.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(35, 0);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "V:";
            // 
            // statusValue
            // 
            this.statusValue.AutoSize = true;
            this.statusValue.Location = new System.Drawing.Point(52, 0);
            this.statusValue.Margin = new System.Windows.Forms.Padding(0);
            this.statusValue.Name = "statusValue";
            this.statusValue.Size = new System.Drawing.Size(0, 13);
            this.statusValue.TabIndex = 4;
            // 
            // panelMainPictureContainer
            // 
            this.panelMainPictureContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMainPictureContainer.Controls.Add(this.pictureBoxMain);
            this.panelMainPictureContainer.Location = new System.Drawing.Point(0, 1);
            this.panelMainPictureContainer.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.panelMainPictureContainer.Name = "panelMainPictureContainer";
            this.panelMainPictureContainer.Size = new System.Drawing.Size(577, 603);
            this.panelMainPictureContainer.TabIndex = 1;
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.CrosshairColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxMain.CrosshairX = 0;
            this.pictureBoxMain.CrosshairY = 0;
            this.pictureBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxMain.Image = null;
            this.pictureBoxMain.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxMain.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.ShowCrosshair = false;
            this.pictureBoxMain.Size = new System.Drawing.Size(577, 603);
            this.pictureBoxMain.TabIndex = 1;
            this.pictureBoxMain.TabStop = false;
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(211, 6);
            // 
            // duplicateItemToolStripMenuItem
            // 
            this.duplicateItemToolStripMenuItem.Name = "duplicateItemToolStripMenuItem";
            this.duplicateItemToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.duplicateItemToolStripMenuItem.Text = "Duplicate Item";
            this.duplicateItemToolStripMenuItem.Click += new System.EventHandler(this.duplicateItemToolStripMenuItem_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 605);
            this.Controls.Add(this.splitContainer);
            this.Name = "Window";
            this.Text = "Images";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetail1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetail2)).EndInit();
            this.tableLayoutPanelStats.ResumeLayout(false);
            this.tableLayoutPanelStats.PerformLayout();
            this.flowLayoutPanelDimensions.ResumeLayout(false);
            this.flowLayoutPanelDimensions.PerformLayout();
            this.flowLayoutPanelSize.ResumeLayout(false);
            this.flowLayoutPanelSize.PerformLayout();
            this.flowLayoutPanelCropRect.ResumeLayout(false);
            this.flowLayoutPanelCropRect.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panelMainPictureContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource dataBindingSource;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.PictureBox pictureBoxDetail1;
        private System.Windows.Forms.PictureBox pictureBoxDetail2;
        private ImageBox pictureBoxMain;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonApply;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelDimensions;
        private System.Windows.Forms.Label labelDimensionsOriginal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelDimensionsShrunk;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelSize;
        private System.Windows.Forms.Label labelSizeOriginal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelSizeShrunk;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowAnnotationsPrimary;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonCrop;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearCrop;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowAnnotationsDetail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCropRect;
        private System.Windows.Forms.Label labelCropRect;
        private System.Windows.Forms.ToolStripButton toolStripButtonCropKeepAspectRatio;
        private System.Windows.Forms.ToolTip toolTipDetail2;
        private System.Windows.Forms.ToolTip toolTipDetail1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowShrunkExpandedPreview;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxBrightAdjust;
        private System.Windows.Forms.CheckBox checkBoxPolyUnbias;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotateRight90;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowOriginalInsteadOfProcessed;
        private System.Windows.Forms.ToolStripButton toolStripButtonInvertAspectRatioForCrop;
        private System.Windows.Forms.ToolStripButton toolStripButtonGlobalOptions;
        private System.Windows.Forms.ToolStripButton toolStripButtonCustomAspectRatio;
        public System.Windows.Forms.DataGridView dataGridViewFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonUseGDIResize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.CheckBox checkBoxNormalizeGeometry;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetNormalizedGeometryAspectRatioExplicitly;
        private System.Windows.Forms.ToolStripButton toolStripButtonResetNormalizedGeometryAspectRatio;
        private System.Windows.Forms.ToolStripButton toolStripButtonResetAllNormalizedGeometry;
        private System.Windows.Forms.ToolStripButton toolStripButtonNormalizeGeometryCorners;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotateLeft90;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonMarkColor;
        private System.Windows.Forms.ToolStripMenuItem blackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yellowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem greenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem purpleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cyanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pinkToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn shrinkDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSize;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonCatchAllMenu;
        private System.Windows.Forms.ToolStripMenuItem markToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem swapToolStripMenuItem;
        private System.Windows.Forms.Panel panelMainPictureContainer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sortByNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unsortToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label statusHue;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label statusSaturation;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label statusValue;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem renumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem diagnosticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLogWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autocropGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unbiasGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetNormalizedGeometryToIdentity;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem duplicateItemToolStripMenuItem;
    }
}

