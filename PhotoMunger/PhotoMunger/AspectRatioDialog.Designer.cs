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
    partial class AspectRatioDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelAspectSelectors = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelSpacer = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonCustom = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCustomX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCustomY = new System.Windows.Forms.TextBox();
            this.radioButtonCurrent = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radioButtonPortrait = new System.Windows.Forms.RadioButton();
            this.radioButtonLandscape = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelAspectSelectors.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanelAspectSelectors, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel4, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(461, 321);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanelAspectSelectors
            // 
            this.flowLayoutPanelAspectSelectors.AutoSize = true;
            this.flowLayoutPanelAspectSelectors.Controls.Add(this.label1);
            this.flowLayoutPanelAspectSelectors.Controls.Add(this.panelSpacer);
            this.flowLayoutPanelAspectSelectors.Controls.Add(this.radioButtonCustom);
            this.flowLayoutPanelAspectSelectors.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanelAspectSelectors.Controls.Add(this.radioButtonCurrent);
            this.flowLayoutPanelAspectSelectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelAspectSelectors.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelAspectSelectors.Location = new System.Drawing.Point(10, 10);
            this.flowLayoutPanelAspectSelectors.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelAspectSelectors.Name = "flowLayoutPanelAspectSelectors";
            this.flowLayoutPanelAspectSelectors.Size = new System.Drawing.Size(215, 264);
            this.flowLayoutPanelAspectSelectors.TabIndex = 0;
            this.flowLayoutPanelAspectSelectors.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Crop Aspect Ratio:";
            // 
            // panelSpacer
            // 
            this.panelSpacer.Location = new System.Drawing.Point(0, 13);
            this.panelSpacer.Margin = new System.Windows.Forms.Padding(0);
            this.panelSpacer.Name = "panelSpacer";
            this.panelSpacer.Size = new System.Drawing.Size(50, 10);
            this.panelSpacer.TabIndex = 2;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.panel1);
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.textBoxCustomX);
            this.flowLayoutPanel2.Controls.Add(this.label3);
            this.flowLayoutPanel2.Controls.Add(this.textBoxCustomY);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 46);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(193, 20);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // radioButtonCustom
            // 
            this.radioButtonCustom.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonCustom.AutoSize = true;
            this.radioButtonCustom.Location = new System.Drawing.Point(3, 26);
            this.radioButtonCustom.Name = "radioButtonCustom";
            this.radioButtonCustom.Size = new System.Drawing.Size(60, 17);
            this.radioButtonCustom.TabIndex = 2;
            this.radioButtonCustom.TabStop = true;
            this.radioButtonCustom.Text = "Custom";
            this.radioButtonCustom.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "W:";
            // 
            // textBoxCustomX
            // 
            this.textBoxCustomX.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxCustomX.Location = new System.Drawing.Point(80, 0);
            this.textBoxCustomX.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.textBoxCustomX.Name = "textBoxCustomX";
            this.textBoxCustomX.Size = new System.Drawing.Size(40, 20);
            this.textBoxCustomX.TabIndex = 4;
            this.textBoxCustomX.TextChanged += new System.EventHandler(this.textBoxCustomX_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(126, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "H:";
            // 
            // textBoxCustomY
            // 
            this.textBoxCustomY.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxCustomY.Location = new System.Drawing.Point(150, 0);
            this.textBoxCustomY.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.textBoxCustomY.Name = "textBoxCustomY";
            this.textBoxCustomY.Size = new System.Drawing.Size(40, 20);
            this.textBoxCustomY.TabIndex = 6;
            this.textBoxCustomY.TextChanged += new System.EventHandler(this.textBoxCustomY_TextChanged);
            // 
            // radioButtonCurrent
            // 
            this.radioButtonCurrent.AutoSize = true;
            this.radioButtonCurrent.Location = new System.Drawing.Point(3, 69);
            this.radioButtonCurrent.Name = "radioButtonCurrent";
            this.radioButtonCurrent.Size = new System.Drawing.Size(59, 17);
            this.radioButtonCurrent.TabIndex = 7;
            this.radioButtonCurrent.TabStop = true;
            this.radioButtonCurrent.Text = "Current";
            this.radioButtonCurrent.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.flowLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel3, 3);
            this.flowLayoutPanel3.Controls.Add(this.buttonOK);
            this.flowLayoutPanel3.Controls.Add(this.label4);
            this.flowLayoutPanel3.Controls.Add(this.buttonCancel);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(61, 289);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(338, 29);
            this.flowLayoutPanel3.TabIndex = 0;
            this.flowLayoutPanel3.WrapContents = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(3, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(125, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(134, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 23);
            this.label4.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(210, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(125, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.Controls.Add(this.pictureBox1);
            this.flowLayoutPanel4.Controls.Add(this.radioButtonPortrait);
            this.flowLayoutPanel4.Controls.Add(this.radioButtonLandscape);
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(248, 13);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(200, 246);
            this.flowLayoutPanel4.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 200);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // radioButtonPortrait
            // 
            this.radioButtonPortrait.AutoSize = true;
            this.radioButtonPortrait.Location = new System.Drawing.Point(3, 203);
            this.radioButtonPortrait.Name = "radioButtonPortrait";
            this.radioButtonPortrait.Size = new System.Drawing.Size(58, 17);
            this.radioButtonPortrait.TabIndex = 200;
            this.radioButtonPortrait.TabStop = true;
            this.radioButtonPortrait.Text = "Portrait";
            this.radioButtonPortrait.UseVisualStyleBackColor = true;
            this.radioButtonPortrait.CheckedChanged += new System.EventHandler(this.radioButtonPortrait_CheckedChanged);
            // 
            // radioButtonLandscape
            // 
            this.radioButtonLandscape.AutoSize = true;
            this.radioButtonLandscape.Location = new System.Drawing.Point(3, 226);
            this.radioButtonLandscape.Name = "radioButtonLandscape";
            this.radioButtonLandscape.Size = new System.Drawing.Size(78, 17);
            this.radioButtonLandscape.TabIndex = 201;
            this.radioButtonLandscape.TabStop = true;
            this.radioButtonLandscape.Text = "Landscape";
            this.radioButtonLandscape.UseVisualStyleBackColor = true;
            this.radioButtonLandscape.CheckedChanged += new System.EventHandler(this.radioButtonLandscape_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(50, 10);
            this.panel1.TabIndex = 3;
            // 
            // AspectRatioDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(461, 321);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AspectRatioDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Aspect Ratio";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanelAspectSelectors.ResumeLayout(false);
            this.flowLayoutPanelAspectSelectors.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelAspectSelectors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.RadioButton radioButtonCustom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCustomX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCustomY;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.RadioButton radioButtonPortrait;
        private System.Windows.Forms.RadioButton radioButtonLandscape;
        private System.Windows.Forms.RadioButton radioButtonCurrent;
        private System.Windows.Forms.Panel panelSpacer;
        private System.Windows.Forms.Panel panel1;
    }
}