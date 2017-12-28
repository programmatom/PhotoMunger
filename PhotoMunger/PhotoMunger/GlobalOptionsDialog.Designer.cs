/*
 *  Copyright © 2010-2018 Thomas R. Lawrence
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
    partial class GlobalOptionsDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel15 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonShrinkFixed = new System.Windows.Forms.RadioButton();
            this.radioButtonShrinkTargeted = new System.Windows.Forms.RadioButton();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxShrinkFactor = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxShrinkTargetSize = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAutoCropLeftMax = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAutoCropTopMax = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAutoCropRightMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAutoCropBottomMax = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxMinClusterFrac = new System.Windows.Forms.TextBox();
            this.checkBoxWhiteCorrection = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAutoCropMinMedianBrightness = new System.Windows.Forms.TextBox();
            this.checkBoxUseEdgeColor = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.label17 = new System.Windows.Forms.Label();
            this.radioButtonOneBitChannelAll = new System.Windows.Forms.RadioButton();
            this.radioButtonOneBitChannelR = new System.Windows.Forms.RadioButton();
            this.radioButtonOneBitChannelG = new System.Windows.Forms.RadioButton();
            this.radioButtonOneBitChannelB = new System.Windows.Forms.RadioButton();
            this.label18 = new System.Windows.Forms.Label();
            this.textBoxOneBitThreshhold = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.label20 = new System.Windows.Forms.Label();
            this.radioButtonOneBitOutputBmp = new System.Windows.Forms.RadioButton();
            this.radioButtonOneBitOutputPng = new System.Windows.Forms.RadioButton();
            this.checkBoxOneBitScaleUp = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxStaticSaturationWhiteThreshhold = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBoxStaticSaturationBlackThreshhold = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBoxStaticSaturateExponent = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel10 = new System.Windows.Forms.FlowLayoutPanel();
            this.label29 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.textBoxNormalizeGeometryAspectWidth = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.textBoxNormalizeGeometryAspectHeight = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel11 = new System.Windows.Forms.FlowLayoutPanel();
            this.label28 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.comboBoxNormalizeGeometryPreviewResizeMethod = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.comboBoxNormalizeGeometryFinalResizeMethod = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxUnbiasMaxDegree = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxUnbiasMaxChiSq = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMaxS = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxMinV = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel12 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxJpegQuality = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.radioButtonJpegEncoderGDI = new System.Windows.Forms.RadioButton();
            this.radioButtonJpegEncoderWPF = new System.Windows.Forms.RadioButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.checkBoxShrink = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoNormalizeGeometry = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoCrop = new System.Windows.Forms.CheckBox();
            this.checkBoxUnbias = new System.Windows.Forms.CheckBox();
            this.checkBoxBrightAdjust = new System.Windows.Forms.CheckBox();
            this.checkBoxStaticSaturation = new System.Windows.Forms.CheckBox();
            this.checkBoxOneBit = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonRotation0 = new System.Windows.Forms.RadioButton();
            this.radioButtonRotation90 = new System.Windows.Forms.RadioButton();
            this.radioButtonRotation180 = new System.Windows.Forms.RadioButton();
            this.radioButtonRotation270 = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.flowLayoutPanel13 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonTimestampDoNothing = new System.Windows.Forms.RadioButton();
            this.radioButtonTimestampAdd = new System.Windows.Forms.RadioButton();
            this.radioButtonTimestampRemove = new System.Windows.Forms.RadioButton();
            this.checkBoxTimestampOverwriteExisting = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel14 = new System.Windows.Forms.FlowLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.radioButtonTimestampFileCreated = new System.Windows.Forms.RadioButton();
            this.radioButtonTimestampFileLastModified = new System.Windows.Forms.RadioButton();
            this.globalOptionsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel15.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.flowLayoutPanel10.SuspendLayout();
            this.flowLayoutPanel11.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel13.SuspendLayout();
            this.flowLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.globalOptionsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 15);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel7, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 4, 5);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 4, 9);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel5, 4, 6);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel6, 4, 11);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel8, 4, 12);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel9, 4, 10);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel10, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel11, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 4, 7);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel12, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelVersion, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxShrink, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxAutoNormalizeGeometry, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxAutoCrop, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxUnbias, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxBrightAdjust, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxStaticSaturation, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxOneBit, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.label9, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel4, 4, 8);
            this.tableLayoutPanel1.Controls.Add(this.label12, 2, 13);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel13, 4, 13);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel14, 4, 14);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 16;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(727, 375);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 5);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.21191F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.57619F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.2119F));
            this.tableLayoutPanel2.Controls.Add(this.buttonOK, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonCancel, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(11, 343);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(713, 29);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.Location = new System.Drawing.Point(172, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(125, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(414, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(125, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.AutoSize = true;
            this.flowLayoutPanel7.Controls.Add(this.flowLayoutPanel15);
            this.flowLayoutPanel7.Controls.Add(this.label19);
            this.flowLayoutPanel7.Controls.Add(this.textBoxShrinkFactor);
            this.flowLayoutPanel7.Controls.Add(this.label16);
            this.flowLayoutPanel7.Controls.Add(this.textBoxShrinkTargetSize);
            this.flowLayoutPanel7.Location = new System.Drawing.Point(212, 33);
            this.flowLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(349, 23);
            this.flowLayoutPanel7.TabIndex = 8;
            // 
            // flowLayoutPanel15
            // 
            this.flowLayoutPanel15.AutoSize = true;
            this.flowLayoutPanel15.Controls.Add(this.radioButtonShrinkFixed);
            this.flowLayoutPanel15.Controls.Add(this.radioButtonShrinkTargeted);
            this.flowLayoutPanel15.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel15.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel15.Name = "flowLayoutPanel15";
            this.flowLayoutPanel15.Size = new System.Drawing.Size(130, 23);
            this.flowLayoutPanel15.TabIndex = 7;
            // 
            // radioButtonShrinkFixed
            // 
            this.radioButtonShrinkFixed.AutoSize = true;
            this.radioButtonShrinkFixed.Location = new System.Drawing.Point(3, 3);
            this.radioButtonShrinkFixed.Name = "radioButtonShrinkFixed";
            this.radioButtonShrinkFixed.Size = new System.Drawing.Size(50, 17);
            this.radioButtonShrinkFixed.TabIndex = 7;
            this.radioButtonShrinkFixed.TabStop = true;
            this.radioButtonShrinkFixed.Text = "Fixed";
            this.radioButtonShrinkFixed.UseVisualStyleBackColor = true;
            this.radioButtonShrinkFixed.CheckedChanged += new System.EventHandler(this.radioButtonShrinkFixed_CheckedChanged);
            // 
            // radioButtonShrinkTargeted
            // 
            this.radioButtonShrinkTargeted.AutoSize = true;
            this.radioButtonShrinkTargeted.Location = new System.Drawing.Point(59, 3);
            this.radioButtonShrinkTargeted.Name = "radioButtonShrinkTargeted";
            this.radioButtonShrinkTargeted.Size = new System.Drawing.Size(68, 17);
            this.radioButtonShrinkTargeted.TabIndex = 7;
            this.radioButtonShrinkTargeted.TabStop = true;
            this.radioButtonShrinkTargeted.Text = "Targeted";
            this.radioButtonShrinkTargeted.UseVisualStyleBackColor = true;
            this.radioButtonShrinkTargeted.CheckedChanged += new System.EventHandler(this.radioButtonShrinkTargeted_CheckedChanged);
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(133, 5);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(40, 13);
            this.label19.TabIndex = 8;
            this.label19.Text = "Factor:";
            // 
            // textBoxShrinkFactor
            // 
            this.textBoxShrinkFactor.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "ShrinkFactor", true));
            this.textBoxShrinkFactor.Location = new System.Drawing.Point(176, 0);
            this.textBoxShrinkFactor.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxShrinkFactor.Name = "textBoxShrinkFactor";
            this.textBoxShrinkFactor.Size = new System.Drawing.Size(40, 20);
            this.textBoxShrinkFactor.TabIndex = 9;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(219, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(87, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Target Size (KB):";
            // 
            // textBoxShrinkTargetSize
            // 
            this.textBoxShrinkTargetSize.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "ShrinkTargetKB", true));
            this.textBoxShrinkTargetSize.Location = new System.Drawing.Point(309, 0);
            this.textBoxShrinkTargetSize.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxShrinkTargetSize.Name = "textBoxShrinkTargetSize";
            this.textBoxShrinkTargetSize.Size = new System.Drawing.Size(40, 20);
            this.textBoxShrinkTargetSize.TabIndex = 9;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.textBoxAutoCropLeftMax);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.textBoxAutoCropTopMax);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.textBoxAutoCropRightMax);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.textBoxAutoCropBottomMax);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(212, 103);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(387, 20);
            this.flowLayoutPanel1.TabIndex = 22;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "LeftMax:";
            // 
            // textBoxAutoCropLeftMax
            // 
            this.textBoxAutoCropLeftMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxAutoCropLeftMax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoCropLeftMax", true));
            this.textBoxAutoCropLeftMax.Location = new System.Drawing.Point(51, 0);
            this.textBoxAutoCropLeftMax.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxAutoCropLeftMax.Name = "textBoxAutoCropLeftMax";
            this.textBoxAutoCropLeftMax.Size = new System.Drawing.Size(40, 20);
            this.textBoxAutoCropLeftMax.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(94, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "TopMax:";
            // 
            // textBoxAutoCropTopMax
            // 
            this.textBoxAutoCropTopMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxAutoCropTopMax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoCropTopMax", true));
            this.textBoxAutoCropTopMax.Location = new System.Drawing.Point(143, 0);
            this.textBoxAutoCropTopMax.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxAutoCropTopMax.Name = "textBoxAutoCropTopMax";
            this.textBoxAutoCropTopMax.Size = new System.Drawing.Size(40, 20);
            this.textBoxAutoCropTopMax.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(186, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "RightMax:";
            // 
            // textBoxAutoCropRightMax
            // 
            this.textBoxAutoCropRightMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxAutoCropRightMax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoCropRightMax", true));
            this.textBoxAutoCropRightMax.Location = new System.Drawing.Point(241, 0);
            this.textBoxAutoCropRightMax.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxAutoCropRightMax.Name = "textBoxAutoCropRightMax";
            this.textBoxAutoCropRightMax.Size = new System.Drawing.Size(40, 20);
            this.textBoxAutoCropRightMax.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(284, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "BottomMax:";
            // 
            // textBoxAutoCropBottomMax
            // 
            this.textBoxAutoCropBottomMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxAutoCropBottomMax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoCropBottomMax", true));
            this.textBoxAutoCropBottomMax.Location = new System.Drawing.Point(347, 0);
            this.textBoxAutoCropBottomMax.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxAutoCropBottomMax.Name = "textBoxAutoCropBottomMax";
            this.textBoxAutoCropBottomMax.Size = new System.Drawing.Size(40, 20);
            this.textBoxAutoCropBottomMax.TabIndex = 29;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.label10);
            this.flowLayoutPanel2.Controls.Add(this.textBoxMinClusterFrac);
            this.flowLayoutPanel2.Controls.Add(this.checkBoxWhiteCorrection);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(212, 194);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(294, 23);
            this.flowLayoutPanel2.TabIndex = 45;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 5);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 13);
            this.label10.TabIndex = 45;
            this.label10.Text = "Minimum Cluster Fraction:";
            // 
            // textBoxMinClusterFrac
            // 
            this.textBoxMinClusterFrac.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "BrightAdjustMinClusterFrac", true));
            this.textBoxMinClusterFrac.Location = new System.Drawing.Point(133, 0);
            this.textBoxMinClusterFrac.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxMinClusterFrac.Name = "textBoxMinClusterFrac";
            this.textBoxMinClusterFrac.Size = new System.Drawing.Size(40, 20);
            this.textBoxMinClusterFrac.TabIndex = 46;
            // 
            // checkBoxWhiteCorrection
            // 
            this.checkBoxWhiteCorrection.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxWhiteCorrection.AutoSize = true;
            this.checkBoxWhiteCorrection.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "BrightAdjustWhiteCorrect", true));
            this.checkBoxWhiteCorrection.Location = new System.Drawing.Point(176, 3);
            this.checkBoxWhiteCorrection.Name = "checkBoxWhiteCorrection";
            this.checkBoxWhiteCorrection.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.checkBoxWhiteCorrection.Size = new System.Drawing.Size(115, 17);
            this.checkBoxWhiteCorrection.TabIndex = 48;
            this.checkBoxWhiteCorrection.Text = "White Correction";
            this.checkBoxWhiteCorrection.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.Controls.Add(this.label6);
            this.flowLayoutPanel5.Controls.Add(this.textBoxAutoCropMinMedianBrightness);
            this.flowLayoutPanel5.Controls.Add(this.checkBoxUseEdgeColor);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(212, 125);
            this.flowLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(279, 23);
            this.flowLayoutPanel5.TabIndex = 30;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Min Median Brightness:";
            // 
            // textBoxAutoCropMinMedianBrightness
            // 
            this.textBoxAutoCropMinMedianBrightness.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoCropMinMedianBrightness", true));
            this.textBoxAutoCropMinMedianBrightness.Location = new System.Drawing.Point(123, 0);
            this.textBoxAutoCropMinMedianBrightness.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxAutoCropMinMedianBrightness.Name = "textBoxAutoCropMinMedianBrightness";
            this.textBoxAutoCropMinMedianBrightness.Size = new System.Drawing.Size(40, 20);
            this.textBoxAutoCropMinMedianBrightness.TabIndex = 31;
            // 
            // checkBoxUseEdgeColor
            // 
            this.checkBoxUseEdgeColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxUseEdgeColor.AutoSize = true;
            this.checkBoxUseEdgeColor.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "AutoCropUseEdgeColor", true));
            this.checkBoxUseEdgeColor.Location = new System.Drawing.Point(166, 3);
            this.checkBoxUseEdgeColor.Name = "checkBoxUseEdgeColor";
            this.checkBoxUseEdgeColor.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.checkBoxUseEdgeColor.Size = new System.Drawing.Size(110, 17);
            this.checkBoxUseEdgeColor.TabIndex = 31;
            this.checkBoxUseEdgeColor.Text = "Use Edge Color";
            this.checkBoxUseEdgeColor.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.Controls.Add(this.label17);
            this.flowLayoutPanel6.Controls.Add(this.radioButtonOneBitChannelAll);
            this.flowLayoutPanel6.Controls.Add(this.radioButtonOneBitChannelR);
            this.flowLayoutPanel6.Controls.Add(this.radioButtonOneBitChannelG);
            this.flowLayoutPanel6.Controls.Add(this.radioButtonOneBitChannelB);
            this.flowLayoutPanel6.Controls.Add(this.label18);
            this.flowLayoutPanel6.Controls.Add(this.textBoxOneBitThreshhold);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(212, 240);
            this.flowLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(343, 20);
            this.flowLayoutPanel6.TabIndex = 59;
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 3);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 59;
            this.label17.Text = "Key Channel:";
            // 
            // radioButtonOneBitChannelAll
            // 
            this.radioButtonOneBitChannelAll.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonOneBitChannelAll.AutoSize = true;
            this.radioButtonOneBitChannelAll.Location = new System.Drawing.Point(79, 1);
            this.radioButtonOneBitChannelAll.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonOneBitChannelAll.Name = "radioButtonOneBitChannelAll";
            this.radioButtonOneBitChannelAll.Size = new System.Drawing.Size(36, 17);
            this.radioButtonOneBitChannelAll.TabIndex = 59;
            this.radioButtonOneBitChannelAll.TabStop = true;
            this.radioButtonOneBitChannelAll.Text = "All";
            this.radioButtonOneBitChannelAll.UseVisualStyleBackColor = true;
            this.radioButtonOneBitChannelAll.CheckedChanged += new System.EventHandler(this.radioButtonOneBitChannelAll_CheckedChanged);
            // 
            // radioButtonOneBitChannelR
            // 
            this.radioButtonOneBitChannelR.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonOneBitChannelR.AutoSize = true;
            this.radioButtonOneBitChannelR.Location = new System.Drawing.Point(121, 1);
            this.radioButtonOneBitChannelR.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonOneBitChannelR.Name = "radioButtonOneBitChannelR";
            this.radioButtonOneBitChannelR.Size = new System.Drawing.Size(33, 17);
            this.radioButtonOneBitChannelR.TabIndex = 60;
            this.radioButtonOneBitChannelR.TabStop = true;
            this.radioButtonOneBitChannelR.Text = "R";
            this.radioButtonOneBitChannelR.UseVisualStyleBackColor = true;
            this.radioButtonOneBitChannelR.CheckedChanged += new System.EventHandler(this.radioButtonOneBitChannelR_CheckedChanged);
            // 
            // radioButtonOneBitChannelG
            // 
            this.radioButtonOneBitChannelG.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonOneBitChannelG.AutoSize = true;
            this.radioButtonOneBitChannelG.Location = new System.Drawing.Point(160, 1);
            this.radioButtonOneBitChannelG.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonOneBitChannelG.Name = "radioButtonOneBitChannelG";
            this.radioButtonOneBitChannelG.Size = new System.Drawing.Size(33, 17);
            this.radioButtonOneBitChannelG.TabIndex = 61;
            this.radioButtonOneBitChannelG.TabStop = true;
            this.radioButtonOneBitChannelG.Text = "G";
            this.radioButtonOneBitChannelG.UseVisualStyleBackColor = true;
            this.radioButtonOneBitChannelG.CheckedChanged += new System.EventHandler(this.radioButtonOneBitChannelG_CheckedChanged);
            // 
            // radioButtonOneBitChannelB
            // 
            this.radioButtonOneBitChannelB.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonOneBitChannelB.AutoSize = true;
            this.radioButtonOneBitChannelB.Location = new System.Drawing.Point(199, 1);
            this.radioButtonOneBitChannelB.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonOneBitChannelB.Name = "radioButtonOneBitChannelB";
            this.radioButtonOneBitChannelB.Size = new System.Drawing.Size(32, 17);
            this.radioButtonOneBitChannelB.TabIndex = 62;
            this.radioButtonOneBitChannelB.TabStop = true;
            this.radioButtonOneBitChannelB.Text = "B";
            this.radioButtonOneBitChannelB.UseVisualStyleBackColor = true;
            this.radioButtonOneBitChannelB.CheckedChanged += new System.EventHandler(this.radioButtonOneBitChannelB_CheckedChanged);
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(237, 3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 13);
            this.label18.TabIndex = 63;
            this.label18.Text = "Threshhold:";
            // 
            // textBoxOneBitThreshhold
            // 
            this.textBoxOneBitThreshhold.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxOneBitThreshhold.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "OneBitThreshhold", true));
            this.textBoxOneBitThreshhold.Location = new System.Drawing.Point(303, 0);
            this.textBoxOneBitThreshhold.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxOneBitThreshhold.Name = "textBoxOneBitThreshhold";
            this.textBoxOneBitThreshhold.Size = new System.Drawing.Size(40, 20);
            this.textBoxOneBitThreshhold.TabIndex = 64;
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel8.AutoSize = true;
            this.flowLayoutPanel8.Controls.Add(this.label20);
            this.flowLayoutPanel8.Controls.Add(this.radioButtonOneBitOutputBmp);
            this.flowLayoutPanel8.Controls.Add(this.radioButtonOneBitOutputPng);
            this.flowLayoutPanel8.Controls.Add(this.checkBoxOneBitScaleUp);
            this.flowLayoutPanel8.Location = new System.Drawing.Point(212, 263);
            this.flowLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(345, 23);
            this.flowLayoutPanel8.TabIndex = 65;
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 5);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(77, 13);
            this.label20.TabIndex = 65;
            this.label20.Text = "Output Format:";
            // 
            // radioButtonOneBitOutputBmp
            // 
            this.radioButtonOneBitOutputBmp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonOneBitOutputBmp.AutoSize = true;
            this.radioButtonOneBitOutputBmp.Location = new System.Drawing.Point(86, 3);
            this.radioButtonOneBitOutputBmp.Name = "radioButtonOneBitOutputBmp";
            this.radioButtonOneBitOutputBmp.Size = new System.Drawing.Size(46, 17);
            this.radioButtonOneBitOutputBmp.TabIndex = 65;
            this.radioButtonOneBitOutputBmp.TabStop = true;
            this.radioButtonOneBitOutputBmp.Text = "Bmp";
            this.radioButtonOneBitOutputBmp.UseVisualStyleBackColor = true;
            this.radioButtonOneBitOutputBmp.CheckedChanged += new System.EventHandler(this.radioButtonOneBitOutputBmp_CheckedChanged);
            // 
            // radioButtonOneBitOutputPng
            // 
            this.radioButtonOneBitOutputPng.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonOneBitOutputPng.AutoSize = true;
            this.radioButtonOneBitOutputPng.Location = new System.Drawing.Point(138, 3);
            this.radioButtonOneBitOutputPng.Name = "radioButtonOneBitOutputPng";
            this.radioButtonOneBitOutputPng.Size = new System.Drawing.Size(44, 17);
            this.radioButtonOneBitOutputPng.TabIndex = 66;
            this.radioButtonOneBitOutputPng.TabStop = true;
            this.radioButtonOneBitOutputPng.Text = "Png";
            this.radioButtonOneBitOutputPng.UseVisualStyleBackColor = true;
            this.radioButtonOneBitOutputPng.CheckedChanged += new System.EventHandler(this.radioButtonOneBitOutputPng_CheckedChanged);
            // 
            // checkBoxOneBitScaleUp
            // 
            this.checkBoxOneBitScaleUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxOneBitScaleUp.AutoSize = true;
            this.checkBoxOneBitScaleUp.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "OneBitScaleUp", true));
            this.checkBoxOneBitScaleUp.Location = new System.Drawing.Point(188, 3);
            this.checkBoxOneBitScaleUp.Name = "checkBoxOneBitScaleUp";
            this.checkBoxOneBitScaleUp.Size = new System.Drawing.Size(154, 17);
            this.checkBoxOneBitScaleUp.TabIndex = 67;
            this.checkBoxOneBitScaleUp.Text = "Scale 2x Before Quantizing";
            this.checkBoxOneBitScaleUp.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.AutoSize = true;
            this.flowLayoutPanel9.Controls.Add(this.label22);
            this.flowLayoutPanel9.Controls.Add(this.textBoxStaticSaturationWhiteThreshhold);
            this.flowLayoutPanel9.Controls.Add(this.label23);
            this.flowLayoutPanel9.Controls.Add(this.textBoxStaticSaturationBlackThreshhold);
            this.flowLayoutPanel9.Controls.Add(this.label24);
            this.flowLayoutPanel9.Controls.Add(this.textBoxStaticSaturateExponent);
            this.flowLayoutPanel9.Location = new System.Drawing.Point(212, 217);
            this.flowLayoutPanel9.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Size = new System.Drawing.Size(380, 20);
            this.flowLayoutPanel9.TabIndex = 51;
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 3);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(94, 13);
            this.label22.TabIndex = 51;
            this.label22.Text = "White Threshhold:";
            // 
            // textBoxStaticSaturationWhiteThreshhold
            // 
            this.textBoxStaticSaturationWhiteThreshhold.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "StaticSaturateWhiteThreshhold", true));
            this.textBoxStaticSaturationWhiteThreshhold.Location = new System.Drawing.Point(100, 0);
            this.textBoxStaticSaturationWhiteThreshhold.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxStaticSaturationWhiteThreshhold.Name = "textBoxStaticSaturationWhiteThreshhold";
            this.textBoxStaticSaturationWhiteThreshhold.Size = new System.Drawing.Size(40, 20);
            this.textBoxStaticSaturationWhiteThreshhold.TabIndex = 52;
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(143, 3);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(93, 13);
            this.label23.TabIndex = 53;
            this.label23.Text = "Black Threshhold:";
            // 
            // textBoxStaticSaturationBlackThreshhold
            // 
            this.textBoxStaticSaturationBlackThreshhold.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "StaticSaturateBlackThreshhold", true));
            this.textBoxStaticSaturationBlackThreshhold.Location = new System.Drawing.Point(239, 0);
            this.textBoxStaticSaturationBlackThreshhold.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxStaticSaturationBlackThreshhold.Name = "textBoxStaticSaturationBlackThreshhold";
            this.textBoxStaticSaturationBlackThreshhold.Size = new System.Drawing.Size(40, 20);
            this.textBoxStaticSaturationBlackThreshhold.TabIndex = 54;
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(282, 3);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(55, 13);
            this.label24.TabIndex = 55;
            this.label24.Text = "Exponent:";
            // 
            // textBoxStaticSaturateExponent
            // 
            this.textBoxStaticSaturateExponent.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "StaticSaturateExponent", true));
            this.textBoxStaticSaturateExponent.Location = new System.Drawing.Point(340, 0);
            this.textBoxStaticSaturateExponent.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxStaticSaturateExponent.Name = "textBoxStaticSaturateExponent";
            this.textBoxStaticSaturateExponent.Size = new System.Drawing.Size(40, 20);
            this.textBoxStaticSaturateExponent.TabIndex = 56;
            // 
            // flowLayoutPanel10
            // 
            this.flowLayoutPanel10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel10.AutoSize = true;
            this.flowLayoutPanel10.Controls.Add(this.label29);
            this.flowLayoutPanel10.Controls.Add(this.label26);
            this.flowLayoutPanel10.Controls.Add(this.textBoxNormalizeGeometryAspectWidth);
            this.flowLayoutPanel10.Controls.Add(this.label27);
            this.flowLayoutPanel10.Controls.Add(this.textBoxNormalizeGeometryAspectHeight);
            this.flowLayoutPanel10.Location = new System.Drawing.Point(212, 57);
            this.flowLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel10.Name = "flowLayoutPanel10";
            this.flowLayoutPanel10.Size = new System.Drawing.Size(269, 20);
            this.flowLayoutPanel10.TabIndex = 12;
            this.flowLayoutPanel10.WrapContents = false;
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 3);
            this.label29.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(101, 13);
            this.label29.TabIndex = 12;
            this.label29.Text = "Force Aspect Ratio:";
            // 
            // label26
            // 
            this.label26.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(107, 3);
            this.label26.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(38, 13);
            this.label26.TabIndex = 12;
            this.label26.Text = "Width:";
            // 
            // textBoxNormalizeGeometryAspectWidth
            // 
            this.textBoxNormalizeGeometryAspectWidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxNormalizeGeometryAspectWidth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoNormalizeGeometryAspectWidthAsString", true));
            this.textBoxNormalizeGeometryAspectWidth.Location = new System.Drawing.Point(145, 0);
            this.textBoxNormalizeGeometryAspectWidth.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxNormalizeGeometryAspectWidth.Name = "textBoxNormalizeGeometryAspectWidth";
            this.textBoxNormalizeGeometryAspectWidth.Size = new System.Drawing.Size(40, 20);
            this.textBoxNormalizeGeometryAspectWidth.TabIndex = 13;
            // 
            // label27
            // 
            this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(188, 3);
            this.label27.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(41, 13);
            this.label27.TabIndex = 14;
            this.label27.Text = "Height:";
            // 
            // textBoxNormalizeGeometryAspectHeight
            // 
            this.textBoxNormalizeGeometryAspectHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxNormalizeGeometryAspectHeight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "AutoNormalizeGeometryAspectHeightAsString", true));
            this.textBoxNormalizeGeometryAspectHeight.Location = new System.Drawing.Point(229, 0);
            this.textBoxNormalizeGeometryAspectHeight.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxNormalizeGeometryAspectHeight.Name = "textBoxNormalizeGeometryAspectHeight";
            this.textBoxNormalizeGeometryAspectHeight.Size = new System.Drawing.Size(40, 20);
            this.textBoxNormalizeGeometryAspectHeight.TabIndex = 15;
            // 
            // flowLayoutPanel11
            // 
            this.flowLayoutPanel11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel11.AutoSize = true;
            this.flowLayoutPanel11.Controls.Add(this.label28);
            this.flowLayoutPanel11.Controls.Add(this.label30);
            this.flowLayoutPanel11.Controls.Add(this.comboBoxNormalizeGeometryPreviewResizeMethod);
            this.flowLayoutPanel11.Controls.Add(this.label31);
            this.flowLayoutPanel11.Controls.Add(this.comboBoxNormalizeGeometryFinalResizeMethod);
            this.flowLayoutPanel11.Location = new System.Drawing.Point(212, 80);
            this.flowLayoutPanel11.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel11.Name = "flowLayoutPanel11";
            this.flowLayoutPanel11.Size = new System.Drawing.Size(330, 21);
            this.flowLayoutPanel11.TabIndex = 16;
            this.flowLayoutPanel11.WrapContents = false;
            // 
            // label28
            // 
            this.label28.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(3, 4);
            this.label28.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(81, 13);
            this.label28.TabIndex = 16;
            this.label28.Text = "Resize Method:";
            // 
            // label30
            // 
            this.label30.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(87, 4);
            this.label30.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(48, 13);
            this.label30.TabIndex = 16;
            this.label30.Text = "Preview:";
            // 
            // comboBoxNormalizeGeometryPreviewResizeMethod
            // 
            this.comboBoxNormalizeGeometryPreviewResizeMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNormalizeGeometryPreviewResizeMethod.DropDownWidth = 120;
            this.comboBoxNormalizeGeometryPreviewResizeMethod.FormattingEnabled = true;
            this.comboBoxNormalizeGeometryPreviewResizeMethod.Items.AddRange(new object[] {
            "Bicubic",
            "Bilinear",
            "Nearest Neighbor"});
            this.comboBoxNormalizeGeometryPreviewResizeMethod.Location = new System.Drawing.Point(135, 0);
            this.comboBoxNormalizeGeometryPreviewResizeMethod.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxNormalizeGeometryPreviewResizeMethod.Name = "comboBoxNormalizeGeometryPreviewResizeMethod";
            this.comboBoxNormalizeGeometryPreviewResizeMethod.Size = new System.Drawing.Size(80, 21);
            this.comboBoxNormalizeGeometryPreviewResizeMethod.TabIndex = 17;
            // 
            // label31
            // 
            this.label31.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(218, 4);
            this.label31.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(32, 13);
            this.label31.TabIndex = 18;
            this.label31.Text = "Final:";
            // 
            // comboBoxNormalizeGeometryFinalResizeMethod
            // 
            this.comboBoxNormalizeGeometryFinalResizeMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNormalizeGeometryFinalResizeMethod.DropDownWidth = 120;
            this.comboBoxNormalizeGeometryFinalResizeMethod.FormattingEnabled = true;
            this.comboBoxNormalizeGeometryFinalResizeMethod.Items.AddRange(new object[] {
            "Bicubic",
            "Bilinear",
            "Nearest Neighbor"});
            this.comboBoxNormalizeGeometryFinalResizeMethod.Location = new System.Drawing.Point(250, 0);
            this.comboBoxNormalizeGeometryFinalResizeMethod.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxNormalizeGeometryFinalResizeMethod.Name = "comboBoxNormalizeGeometryFinalResizeMethod";
            this.comboBoxNormalizeGeometryFinalResizeMethod.Size = new System.Drawing.Size(80, 21);
            this.comboBoxNormalizeGeometryFinalResizeMethod.TabIndex = 19;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.label14);
            this.flowLayoutPanel3.Controls.Add(this.textBoxUnbiasMaxDegree);
            this.flowLayoutPanel3.Controls.Add(this.label15);
            this.flowLayoutPanel3.Controls.Add(this.textBoxUnbiasMaxChiSq);
            this.flowLayoutPanel3.Controls.Add(this.label8);
            this.flowLayoutPanel3.Controls.Add(this.textBoxMaxS);
            this.flowLayoutPanel3.Controls.Add(this.label11);
            this.flowLayoutPanel3.Controls.Add(this.textBoxMinV);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(212, 148);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(404, 20);
            this.flowLayoutPanel3.TabIndex = 34;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 3);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(68, 13);
            this.label14.TabIndex = 34;
            this.label14.Text = "Max Degree:";
            // 
            // textBoxUnbiasMaxDegree
            // 
            this.textBoxUnbiasMaxDegree.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "UnbiasMaxDegree", true));
            this.textBoxUnbiasMaxDegree.Location = new System.Drawing.Point(74, 0);
            this.textBoxUnbiasMaxDegree.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxUnbiasMaxDegree.Name = "textBoxUnbiasMaxDegree";
            this.textBoxUnbiasMaxDegree.Size = new System.Drawing.Size(40, 20);
            this.textBoxUnbiasMaxDegree.TabIndex = 35;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(117, 3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "Max ChiSq:";
            // 
            // textBoxUnbiasMaxChiSq
            // 
            this.textBoxUnbiasMaxChiSq.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "UnbiasMaxChisq", true));
            this.textBoxUnbiasMaxChiSq.Location = new System.Drawing.Point(181, 0);
            this.textBoxUnbiasMaxChiSq.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxUnbiasMaxChiSq.Name = "textBoxUnbiasMaxChiSq";
            this.textBoxUnbiasMaxChiSq.Size = new System.Drawing.Size(60, 20);
            this.textBoxUnbiasMaxChiSq.TabIndex = 37;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(244, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "MaxS:";
            // 
            // textBoxMaxS
            // 
            this.textBoxMaxS.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "UnbiasMaxS", true));
            this.textBoxMaxS.Location = new System.Drawing.Point(284, 0);
            this.textBoxMaxS.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxMaxS.Name = "textBoxMaxS";
            this.textBoxMaxS.Size = new System.Drawing.Size(40, 20);
            this.textBoxMaxS.TabIndex = 37;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(327, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "MinV:";
            // 
            // textBoxMinV
            // 
            this.textBoxMinV.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "UnbiasMinV", true));
            this.textBoxMinV.Location = new System.Drawing.Point(364, 0);
            this.textBoxMinV.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxMinV.Name = "textBoxMinV";
            this.textBoxMinV.Size = new System.Drawing.Size(40, 20);
            this.textBoxMinV.TabIndex = 37;
            // 
            // flowLayoutPanel12
            // 
            this.flowLayoutPanel12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel12.AutoSize = true;
            this.flowLayoutPanel12.Controls.Add(this.label1);
            this.flowLayoutPanel12.Controls.Add(this.textBoxJpegQuality);
            this.flowLayoutPanel12.Controls.Add(this.label33);
            this.flowLayoutPanel12.Controls.Add(this.radioButtonJpegEncoderGDI);
            this.flowLayoutPanel12.Controls.Add(this.radioButtonJpegEncoderWPF);
            this.flowLayoutPanel12.Location = new System.Drawing.Point(212, 11);
            this.flowLayoutPanel12.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel12.Name = "flowLayoutPanel12";
            this.flowLayoutPanel12.Size = new System.Drawing.Size(391, 20);
            this.flowLayoutPanel12.TabIndex = 4;
            this.flowLayoutPanel12.WrapContents = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Quality:";
            // 
            // textBoxJpegQuality
            // 
            this.textBoxJpegQuality.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxJpegQuality.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.globalOptionsBindingSource, "JpegQuality", true));
            this.textBoxJpegQuality.Location = new System.Drawing.Point(48, 0);
            this.textBoxJpegQuality.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxJpegQuality.Name = "textBoxJpegQuality";
            this.textBoxJpegQuality.Size = new System.Drawing.Size(40, 20);
            this.textBoxJpegQuality.TabIndex = 3;
            // 
            // label33
            // 
            this.label33.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(91, 3);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(50, 13);
            this.label33.TabIndex = 0;
            this.label33.Text = "Encoder:";
            // 
            // radioButtonJpegEncoderGDI
            // 
            this.radioButtonJpegEncoderGDI.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonJpegEncoderGDI.AutoSize = true;
            this.radioButtonJpegEncoderGDI.Location = new System.Drawing.Point(147, 1);
            this.radioButtonJpegEncoderGDI.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonJpegEncoderGDI.Name = "radioButtonJpegEncoderGDI";
            this.radioButtonJpegEncoderGDI.Size = new System.Drawing.Size(50, 17);
            this.radioButtonJpegEncoderGDI.TabIndex = 4;
            this.radioButtonJpegEncoderGDI.TabStop = true;
            this.radioButtonJpegEncoderGDI.Text = "GDI+";
            this.radioButtonJpegEncoderGDI.UseVisualStyleBackColor = true;
            this.radioButtonJpegEncoderGDI.CheckedChanged += new System.EventHandler(this.radioButtonJpegEncoderGDI_CheckedChanged);
            // 
            // radioButtonJpegEncoderWPF
            // 
            this.radioButtonJpegEncoderWPF.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonJpegEncoderWPF.AutoSize = true;
            this.radioButtonJpegEncoderWPF.Location = new System.Drawing.Point(203, 1);
            this.radioButtonJpegEncoderWPF.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonJpegEncoderWPF.Name = "radioButtonJpegEncoderWPF";
            this.radioButtonJpegEncoderWPF.Size = new System.Drawing.Size(185, 17);
            this.radioButtonJpegEncoderWPF.TabIndex = 5;
            this.radioButtonJpegEncoderWPF.TabStop = true;
            this.radioButtonJpegEncoderWPF.Text = "WPF (better for high-color images)";
            this.radioButtonJpegEncoderWPF.UseVisualStyleBackColor = true;
            this.radioButtonJpegEncoderWPF.CheckedChanged += new System.EventHandler(this.radioButtonJpegEncoderWPF_CheckedChanged);
            // 
            // pictureBox
            // 
            this.pictureBox.ErrorImage = null;
            this.pictureBox.InitialImage = null;
            this.pictureBox.Location = new System.Drawing.Point(619, 36);
            this.pictureBox.Name = "pictureBox";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox, 11);
            this.pictureBox.Size = new System.Drawing.Size(100, 100);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 20;
            this.pictureBox.TabStop = false;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(677, 15);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.labelVersion.Size = new System.Drawing.Size(47, 13);
            this.labelVersion.TabIndex = 14;
            this.labelVersion.Text = "Version";
            // 
            // checkBoxShrink
            // 
            this.checkBoxShrink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShrink.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxShrink, 2);
            this.checkBoxShrink.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "Shrink", true));
            this.checkBoxShrink.Location = new System.Drawing.Point(11, 36);
            this.checkBoxShrink.Name = "checkBoxShrink";
            this.checkBoxShrink.Size = new System.Drawing.Size(158, 17);
            this.checkBoxShrink.TabIndex = 7;
            this.checkBoxShrink.Text = "Shrink";
            this.checkBoxShrink.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoNormalizeGeometry
            // 
            this.checkBoxAutoNormalizeGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoNormalizeGeometry.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxAutoNormalizeGeometry, 2);
            this.checkBoxAutoNormalizeGeometry.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.globalOptionsBindingSource, "EnableAutoNormalizeGeometry", true));
            this.checkBoxAutoNormalizeGeometry.Location = new System.Drawing.Point(11, 59);
            this.checkBoxAutoNormalizeGeometry.Name = "checkBoxAutoNormalizeGeometry";
            this.checkBoxAutoNormalizeGeometry.Size = new System.Drawing.Size(158, 17);
            this.checkBoxAutoNormalizeGeometry.TabIndex = 11;
            this.checkBoxAutoNormalizeGeometry.Text = "Auto-normalize Geometry";
            this.checkBoxAutoNormalizeGeometry.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoCrop
            // 
            this.checkBoxAutoCrop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxAutoCrop.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxAutoCrop, 2);
            this.checkBoxAutoCrop.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "AutoCrop", true));
            this.checkBoxAutoCrop.Location = new System.Drawing.Point(11, 105);
            this.checkBoxAutoCrop.Name = "checkBoxAutoCrop";
            this.checkBoxAutoCrop.Size = new System.Drawing.Size(158, 17);
            this.checkBoxAutoCrop.TabIndex = 21;
            this.checkBoxAutoCrop.Text = "Autocrop";
            this.checkBoxAutoCrop.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnbias
            // 
            this.checkBoxUnbias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUnbias.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxUnbias, 2);
            this.checkBoxUnbias.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "Unbias", true));
            this.checkBoxUnbias.Location = new System.Drawing.Point(11, 151);
            this.checkBoxUnbias.Name = "checkBoxUnbias";
            this.checkBoxUnbias.Size = new System.Drawing.Size(158, 17);
            this.checkBoxUnbias.TabIndex = 33;
            this.checkBoxUnbias.Text = "Polynomial Unbias";
            this.checkBoxUnbias.UseVisualStyleBackColor = true;
            // 
            // checkBoxBrightAdjust
            // 
            this.checkBoxBrightAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxBrightAdjust.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxBrightAdjust, 2);
            this.checkBoxBrightAdjust.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "BrightAdjust", true));
            this.checkBoxBrightAdjust.Location = new System.Drawing.Point(11, 197);
            this.checkBoxBrightAdjust.Name = "checkBoxBrightAdjust";
            this.checkBoxBrightAdjust.Size = new System.Drawing.Size(158, 17);
            this.checkBoxBrightAdjust.TabIndex = 44;
            this.checkBoxBrightAdjust.Text = "Adjust Brightness";
            this.checkBoxBrightAdjust.UseVisualStyleBackColor = true;
            // 
            // checkBoxStaticSaturation
            // 
            this.checkBoxStaticSaturation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxStaticSaturation.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxStaticSaturation, 2);
            this.checkBoxStaticSaturation.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "StaticSaturate", true));
            this.checkBoxStaticSaturation.Location = new System.Drawing.Point(11, 220);
            this.checkBoxStaticSaturation.Name = "checkBoxStaticSaturation";
            this.checkBoxStaticSaturation.Size = new System.Drawing.Size(158, 17);
            this.checkBoxStaticSaturation.TabIndex = 50;
            this.checkBoxStaticSaturation.Text = "Static Saturation";
            this.checkBoxStaticSaturation.UseVisualStyleBackColor = true;
            // 
            // checkBoxOneBit
            // 
            this.checkBoxOneBit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxOneBit.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxOneBit, 2);
            this.checkBoxOneBit.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "OneBit", true));
            this.checkBoxOneBit.Location = new System.Drawing.Point(11, 243);
            this.checkBoxOneBit.Name = "checkBoxOneBit";
            this.checkBoxOneBit.Size = new System.Drawing.Size(158, 17);
            this.checkBoxOneBit.TabIndex = 58;
            this.checkBoxOneBit.Text = "Monochrome Bitmap Output";
            this.checkBoxOneBit.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 176);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "Bulk Rotation:";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(27, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(142, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Jpeg";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.Controls.Add(this.radioButtonRotation0);
            this.flowLayoutPanel4.Controls.Add(this.radioButtonRotation90);
            this.flowLayoutPanel4.Controls.Add(this.radioButtonRotation180);
            this.flowLayoutPanel4.Controls.Add(this.radioButtonRotation270);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(212, 174);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(198, 17);
            this.flowLayoutPanel4.TabIndex = 40;
            this.flowLayoutPanel4.WrapContents = false;
            // 
            // radioButtonRotation0
            // 
            this.radioButtonRotation0.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonRotation0.AutoSize = true;
            this.radioButtonRotation0.Location = new System.Drawing.Point(3, 0);
            this.radioButtonRotation0.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonRotation0.Name = "radioButtonRotation0";
            this.radioButtonRotation0.Size = new System.Drawing.Size(51, 17);
            this.radioButtonRotation0.TabIndex = 39;
            this.radioButtonRotation0.TabStop = true;
            this.radioButtonRotation0.Text = "None";
            this.radioButtonRotation0.UseVisualStyleBackColor = true;
            this.radioButtonRotation0.CheckedChanged += new System.EventHandler(this.radioButtonRotation0_CheckedChanged);
            // 
            // radioButtonRotation90
            // 
            this.radioButtonRotation90.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonRotation90.AutoSize = true;
            this.radioButtonRotation90.Location = new System.Drawing.Point(60, 0);
            this.radioButtonRotation90.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonRotation90.Name = "radioButtonRotation90";
            this.radioButtonRotation90.Size = new System.Drawing.Size(37, 17);
            this.radioButtonRotation90.TabIndex = 40;
            this.radioButtonRotation90.TabStop = true;
            this.radioButtonRotation90.Text = "90";
            this.radioButtonRotation90.UseVisualStyleBackColor = true;
            this.radioButtonRotation90.CheckedChanged += new System.EventHandler(this.radioButtonRotation90_CheckedChanged);
            // 
            // radioButtonRotation180
            // 
            this.radioButtonRotation180.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonRotation180.AutoSize = true;
            this.radioButtonRotation180.Location = new System.Drawing.Point(103, 0);
            this.radioButtonRotation180.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonRotation180.Name = "radioButtonRotation180";
            this.radioButtonRotation180.Size = new System.Drawing.Size(43, 17);
            this.radioButtonRotation180.TabIndex = 41;
            this.radioButtonRotation180.TabStop = true;
            this.radioButtonRotation180.Text = "180";
            this.radioButtonRotation180.UseVisualStyleBackColor = true;
            this.radioButtonRotation180.CheckedChanged += new System.EventHandler(this.radioButtonRotation180_CheckedChanged);
            // 
            // radioButtonRotation270
            // 
            this.radioButtonRotation270.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonRotation270.AutoSize = true;
            this.radioButtonRotation270.Location = new System.Drawing.Point(152, 0);
            this.radioButtonRotation270.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonRotation270.Name = "radioButtonRotation270";
            this.radioButtonRotation270.Size = new System.Drawing.Size(43, 17);
            this.radioButtonRotation270.TabIndex = 42;
            this.radioButtonRotation270.TabStop = true;
            this.radioButtonRotation270.Text = "270";
            this.radioButtonRotation270.UseVisualStyleBackColor = true;
            this.radioButtonRotation270.CheckedChanged += new System.EventHandler(this.radioButtonRotation270_CheckedChanged);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(27, 291);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(142, 13);
            this.label12.TabIndex = 68;
            this.label12.Text = "Timestamp in File Name";
            // 
            // flowLayoutPanel13
            // 
            this.flowLayoutPanel13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel13.AutoSize = true;
            this.flowLayoutPanel13.Controls.Add(this.radioButtonTimestampDoNothing);
            this.flowLayoutPanel13.Controls.Add(this.radioButtonTimestampAdd);
            this.flowLayoutPanel13.Controls.Add(this.radioButtonTimestampRemove);
            this.flowLayoutPanel13.Controls.Add(this.checkBoxTimestampOverwriteExisting);
            this.flowLayoutPanel13.Location = new System.Drawing.Point(212, 286);
            this.flowLayoutPanel13.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel13.Name = "flowLayoutPanel13";
            this.flowLayoutPanel13.Size = new System.Drawing.Size(322, 23);
            this.flowLayoutPanel13.TabIndex = 69;
            this.flowLayoutPanel13.WrapContents = false;
            // 
            // radioButtonTimestampDoNothing
            // 
            this.radioButtonTimestampDoNothing.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonTimestampDoNothing.AutoSize = true;
            this.radioButtonTimestampDoNothing.Location = new System.Drawing.Point(3, 3);
            this.radioButtonTimestampDoNothing.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonTimestampDoNothing.Name = "radioButtonTimestampDoNothing";
            this.radioButtonTimestampDoNothing.Size = new System.Drawing.Size(79, 17);
            this.radioButtonTimestampDoNothing.TabIndex = 69;
            this.radioButtonTimestampDoNothing.TabStop = true;
            this.radioButtonTimestampDoNothing.Text = "Do Nothing";
            this.radioButtonTimestampDoNothing.UseVisualStyleBackColor = true;
            this.radioButtonTimestampDoNothing.CheckedChanged += new System.EventHandler(this.radioButtonTimestampDoNothing_CheckedChanged);
            // 
            // radioButtonTimestampAdd
            // 
            this.radioButtonTimestampAdd.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonTimestampAdd.AutoSize = true;
            this.radioButtonTimestampAdd.Location = new System.Drawing.Point(88, 3);
            this.radioButtonTimestampAdd.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonTimestampAdd.Name = "radioButtonTimestampAdd";
            this.radioButtonTimestampAdd.Size = new System.Drawing.Size(44, 17);
            this.radioButtonTimestampAdd.TabIndex = 70;
            this.radioButtonTimestampAdd.TabStop = true;
            this.radioButtonTimestampAdd.Text = "Add";
            this.radioButtonTimestampAdd.UseVisualStyleBackColor = true;
            this.radioButtonTimestampAdd.CheckedChanged += new System.EventHandler(this.radioButtonTimestampAdd_CheckedChanged);
            // 
            // radioButtonTimestampRemove
            // 
            this.radioButtonTimestampRemove.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonTimestampRemove.AutoSize = true;
            this.radioButtonTimestampRemove.Location = new System.Drawing.Point(138, 3);
            this.radioButtonTimestampRemove.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioButtonTimestampRemove.Name = "radioButtonTimestampRemove";
            this.radioButtonTimestampRemove.Size = new System.Drawing.Size(65, 17);
            this.radioButtonTimestampRemove.TabIndex = 71;
            this.radioButtonTimestampRemove.TabStop = true;
            this.radioButtonTimestampRemove.Text = "Remove";
            this.radioButtonTimestampRemove.UseVisualStyleBackColor = true;
            this.radioButtonTimestampRemove.CheckedChanged += new System.EventHandler(this.radioButtonTimestampRemove_CheckedChanged);
            // 
            // checkBoxTimestampOverwriteExisting
            // 
            this.checkBoxTimestampOverwriteExisting.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxTimestampOverwriteExisting.AutoSize = true;
            this.checkBoxTimestampOverwriteExisting.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.globalOptionsBindingSource, "TimestampsOverwriteExisting", true));
            this.checkBoxTimestampOverwriteExisting.Location = new System.Drawing.Point(209, 3);
            this.checkBoxTimestampOverwriteExisting.Name = "checkBoxTimestampOverwriteExisting";
            this.checkBoxTimestampOverwriteExisting.Size = new System.Drawing.Size(110, 17);
            this.checkBoxTimestampOverwriteExisting.TabIndex = 72;
            this.checkBoxTimestampOverwriteExisting.Text = "Overwrite Existing";
            this.checkBoxTimestampOverwriteExisting.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel14
            // 
            this.flowLayoutPanel14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel14.AutoSize = true;
            this.flowLayoutPanel14.Controls.Add(this.label13);
            this.flowLayoutPanel14.Controls.Add(this.radioButtonTimestampFileCreated);
            this.flowLayoutPanel14.Controls.Add(this.radioButtonTimestampFileLastModified);
            this.flowLayoutPanel14.Location = new System.Drawing.Point(212, 309);
            this.flowLayoutPanel14.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel14.Name = "flowLayoutPanel14";
            this.flowLayoutPanel14.Size = new System.Drawing.Size(362, 23);
            this.flowLayoutPanel14.TabIndex = 73;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(156, 13);
            this.label13.TabIndex = 73;
            this.label13.Text = "If Exif timestamp is missing, use:";
            // 
            // radioButtonTimestampFileCreated
            // 
            this.radioButtonTimestampFileCreated.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonTimestampFileCreated.AutoSize = true;
            this.radioButtonTimestampFileCreated.Location = new System.Drawing.Point(165, 3);
            this.radioButtonTimestampFileCreated.Name = "radioButtonTimestampFileCreated";
            this.radioButtonTimestampFileCreated.Size = new System.Drawing.Size(81, 17);
            this.radioButtonTimestampFileCreated.TabIndex = 74;
            this.radioButtonTimestampFileCreated.TabStop = true;
            this.radioButtonTimestampFileCreated.Text = "File Created";
            this.radioButtonTimestampFileCreated.UseVisualStyleBackColor = true;
            this.radioButtonTimestampFileCreated.CheckedChanged += new System.EventHandler(this.radioButtonTimestampFileCreated_CheckedChanged);
            // 
            // radioButtonTimestampFileLastModified
            // 
            this.radioButtonTimestampFileLastModified.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonTimestampFileLastModified.AutoSize = true;
            this.radioButtonTimestampFileLastModified.Location = new System.Drawing.Point(252, 3);
            this.radioButtonTimestampFileLastModified.Name = "radioButtonTimestampFileLastModified";
            this.radioButtonTimestampFileLastModified.Size = new System.Drawing.Size(107, 17);
            this.radioButtonTimestampFileLastModified.TabIndex = 75;
            this.radioButtonTimestampFileLastModified.TabStop = true;
            this.radioButtonTimestampFileLastModified.Text = "File Last Modified";
            this.radioButtonTimestampFileLastModified.UseVisualStyleBackColor = true;
            this.radioButtonTimestampFileLastModified.CheckedChanged += new System.EventHandler(this.radioButtonTimestampFileLastModified_CheckedChanged);
            // 
            // globalOptionsBindingSource
            // 
            this.globalOptionsBindingSource.DataSource = typeof(AdaptiveImageSizeReducer.GlobalOptions);
            // 
            // GlobalOptionsDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(727, 375);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalOptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Global Options";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.flowLayoutPanel15.ResumeLayout(false);
            this.flowLayoutPanel15.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            this.flowLayoutPanel10.ResumeLayout(false);
            this.flowLayoutPanel10.PerformLayout();
            this.flowLayoutPanel11.ResumeLayout(false);
            this.flowLayoutPanel11.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel12.ResumeLayout(false);
            this.flowLayoutPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.flowLayoutPanel13.ResumeLayout(false);
            this.flowLayoutPanel13.PerformLayout();
            this.flowLayoutPanel14.ResumeLayout(false);
            this.flowLayoutPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.globalOptionsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxAutoCrop;
        private System.Windows.Forms.BindingSource globalOptionsBindingSource;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAutoCropLeftMax;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAutoCropTopMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAutoCropRightMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAutoCropBottomMax;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxBrightAdjust;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxMinClusterFrac;
        private System.Windows.Forms.CheckBox checkBoxWhiteCorrection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox checkBoxUnbias;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxUnbiasMaxDegree;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxUnbiasMaxChiSq;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.RadioButton radioButtonRotation0;
        private System.Windows.Forms.RadioButton radioButtonRotation90;
        private System.Windows.Forms.RadioButton radioButtonRotation180;
        private System.Windows.Forms.RadioButton radioButtonRotation270;
        private System.Windows.Forms.CheckBox checkBoxShrink;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxAutoCropMinMedianBrightness;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxJpegQuality;
        private System.Windows.Forms.CheckBox checkBoxOneBit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBoxOneBitThreshhold;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxShrinkFactor;
        private System.Windows.Forms.RadioButton radioButtonOneBitChannelAll;
        private System.Windows.Forms.RadioButton radioButtonOneBitChannelR;
        private System.Windows.Forms.RadioButton radioButtonOneBitChannelG;
        private System.Windows.Forms.RadioButton radioButtonOneBitChannelB;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.RadioButton radioButtonOneBitOutputBmp;
        private System.Windows.Forms.RadioButton radioButtonOneBitOutputPng;
        private System.Windows.Forms.CheckBox checkBoxOneBitScaleUp;
        private System.Windows.Forms.CheckBox checkBoxStaticSaturation;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textBoxStaticSaturationWhiteThreshhold;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBoxStaticSaturationBlackThreshhold;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox textBoxStaticSaturateExponent;
        private System.Windows.Forms.CheckBox checkBoxAutoNormalizeGeometry;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel10;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox textBoxNormalizeGeometryAspectWidth;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox textBoxNormalizeGeometryAspectHeight;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel11;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ComboBox comboBoxNormalizeGeometryPreviewResizeMethod;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ComboBox comboBoxNormalizeGeometryFinalResizeMethod;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel12;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.RadioButton radioButtonJpegEncoderGDI;
        private System.Windows.Forms.RadioButton radioButtonJpegEncoderWPF;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxUseEdgeColor;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMaxS;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxMinV;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel13;
        private System.Windows.Forms.RadioButton radioButtonTimestampDoNothing;
        private System.Windows.Forms.RadioButton radioButtonTimestampAdd;
        private System.Windows.Forms.RadioButton radioButtonTimestampRemove;
        private System.Windows.Forms.CheckBox checkBoxTimestampOverwriteExisting;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton radioButtonTimestampFileCreated;
        private System.Windows.Forms.RadioButton radioButtonTimestampFileLastModified;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel15;
        private System.Windows.Forms.RadioButton radioButtonShrinkFixed;
        private System.Windows.Forms.RadioButton radioButtonShrinkTargeted;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxShrinkTargetSize;
    }
}