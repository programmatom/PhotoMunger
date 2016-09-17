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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AdaptiveImageSizeReducer
{
    public partial class Window : Form, ISuspendResumeFormatting
    {
        private readonly string directory;
        private readonly BindingList<Item> items = new BindingList<Item>();
        private readonly ImageCache cache;
        private readonly GlobalOptions options;

        private BitmapHolder pictureBoxMainHolder;
        private Point detailOffset;
        private Bitmap detail1, detail2;

        private Item lastItem;

        private bool inDragForCrop;
        private Point dragPivot;
        private int? dragFloatOverrideX;
        private int? dragFloatOverrideY;
        private bool moveBox;
        private Rectangle originalBox;

        private GeoCorner geoDrag;
        private Point[] originalCorners;

        private Rectangle savedCropRect;
        private float? aspectRatioOverride;

        private int swapIndex = -1;

        private bool showAutoCropGrid;
        private bool showPolyUnbiasGrid;

        private bool fullScreenMode;
        private FormWindowState savedWindowStateForFullScreenMode;

        private bool closing;

        private BatchAnalyzerQueue lastAnalysisTask;

        private CacheView cacheView;

        public Window(string directory, IList<Item> items, ImageCache cache, GlobalOptions options)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon1;

            this.directory = directory;
            this.cache = cache;
            this.options = options;
            foreach (Item item in items)
            {
                item.PropertyChanged += Item_PropertyChanged;
                this.items.Add(item);
            }

            UpdateWindowTitle();

            this.toolStripButtonShowOriginalInsteadOfProcessed.Checked = options.ShowOriginalInsteadOfProcessed;
            this.toolStripButtonShowAnnotationsPrimary.Checked = options.ShowAnnotationsPrimary;
            this.toolStripButtonShowAnnotationsDetail.Checked = options.ShowAnnotationsDetail;
            this.toolStripButtonShowShrunkExpandedPreview.Checked = options.ShowShrinkExpandDetail;
            this.showAutoCropGrid = options.ShowAutoCropGrid;
            this.showPolyUnbiasGrid = options.ShowPolyUnbiasGrid;
            this.swapIndex = Math.Min(Math.Max(options.LastSelectedSwap, -1), items.Count - 1);

            this.dataGridViewFiles.Enabled = false;
            this.dataBindingSource.DataSource = new BindingSource(this.items, null);
            this.dataGridViewFiles.CellFormatting += DataGridViewFiles_CellFormatting;
            this.dataGridViewFiles.CellValidating += DataGridViewFiles_CellValidating;
            this.dataGridViewFiles.EditingControlShowing += DataGridViewFiles_EditingControlShowing;

            this.pictureBoxMain.MouseMove += PictureBoxMain_MouseMove;
            this.pictureBoxMain.MouseDown += PictureBoxMain_MouseDown;
            this.pictureBoxMain.MouseUp += PictureBoxMain_MouseUp;
            this.pictureBoxMain.MouseLeave += PictureBoxMain_MouseLeave;

            this.toolStripButtonShowAnnotationsPrimary.CheckedChanged += ToolStripButtonShowAnnotationsPrimary_CheckedChanged;
            this.toolStripButtonShowAnnotationsDetail.CheckedChanged += ToolStripButtonShowAnnotationsDetail_CheckedChanged;
            this.toolStripButtonCrop.CheckedChanged += ToolStripButtonCrop_CheckedChanged;
            this.toolStripButtonClearCrop.Click += ToolStripButtonClearCrop_Click;
            this.toolStripButtonShowShrunkExpandedPreview.CheckedChanged += ToolStripButtonShowShrunkExpandedPreview_CheckedChanged;
            this.tableLayoutPanelStats.SizeChanged += TableLayoutPanelStats_SizeChanged;
            this.toolStripButtonShowOriginalInsteadOfProcessed.CheckedChanged += ToolStripButtonShowOriginalInsteadOfProcessed_CheckedChanged;
            this.toolStripButtonUseGDIResize.CheckedChanged += ToolStripButtonUseGDIResize_CheckedChanged;
            this.toolStripButtonNormalizeGeometryCorners.CheckedChanged += ToolStripButtonNormalizeGeometryCorners_CheckedChanged;

            this.checkBoxBrightAdjust.Click += UIElementRestoreFocus_Click;
            this.checkBoxPolyUnbias.Click += UIElementRestoreFocus_Click;
            this.checkBoxNormalizeGeometry.Click += UIElementRestoreFocus_Click;

            this.pictureBoxDetail1.MouseEnter += PictureBoxDetail1_MouseEnter;
            this.pictureBoxDetail1.MouseLeave += PictureBoxDetail1_MouseLeave;
            this.pictureBoxDetail2.MouseEnter += PictureBoxDetail2_MouseEnter;
            this.pictureBoxDetail2.MouseLeave += PictureBoxDetail2_MouseLeave;

            Application.Idle += Idle_RestoreDataGridViewSelectedCells;
        }

        private void UpdateWindowTitle()
        {
            this.Text = String.Format("Loaded {1}: {0}{2}", directory, items.Count, lastItem != null ? String.Concat(" - ", lastItem.RenamedFileName) : null);
        }

        public BatchAnalyzerQueue LastAnalysisTask
        {
            get
            {
                return this.lastAnalysisTask;
            }
            set
            {
                if ((this.lastAnalysisTask != null) && !this.lastAnalysisTask.IsCompleted)
                {
                    Debug.Assert(false);
                    throw new InvalidOperationException();
                }
                this.lastAnalysisTask = value;
            }
        }

        private delegate string InvalidateIdGetter(Item item);
        private void InvalidateCacheAll(InvalidateIdGetter getter)
        {
            foreach (Item item in items)
            {
                cache.Invalidate(getter(item));
            }
        }

        public Item CurrentItem
        {
            get
            {
                if ((dataGridViewFiles.CurrentCell == null) || ((uint)dataGridViewFiles.CurrentCell.RowIndex >= items.Count))
                {
                    return null;
                }
                return items[dataGridViewFiles.CurrentCell.RowIndex];
            }
        }

        public int CurrentItemRow
        {
            get
            {
                return dataGridViewFiles.CurrentCell.RowIndex;
            }
        }

        private static int BinarySearch(IList<Item> items, Item item)
        {
            int lower = 0;
            int upper = items.Count - 1;
            while (lower <= upper)
            {
                int middle = (lower + upper) / 2;

                int c = String.Compare(items[middle].SourcePath, item.SourcePath);
                if (c == 0)
                {
                    return middle;
                }
                else if (c < 0)
                {
                    lower = middle + 1;
                }
                else
                {
                    upper = middle - 1;
                }
            }
            return ~lower;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            closing = true;
            base.OnFormClosing(e);
            this.Visible = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            SaveSettings();

            if (this.cacheView != null)
            {
                this.cacheView.Dispose();
                this.cacheView = null;
            }

            this.lastAnalysisTask.Cancel();

            SetPictureBoxImage(this.pictureBoxMain, null);
            if (pictureBoxMainHolder != null)
            {
                pictureBoxMainHolder.Dispose();
                pictureBoxMainHolder = null;
            }

            if (lastUpdatePrimaryCancellation != null)
            {
                lastUpdatePrimaryCancellation.Cancel();
            }
            if (lastUpdateDetailCancellation != null)
            {
                lastUpdateDetailCancellation.Cancel();
            }

            if (detail1 != null)
            {
                detail1.Dispose();
            }
            if (detail2 != null)
            {
                detail2.Dispose();
            }
        }

        private void TableLayoutPanelStats_SizeChanged(object sender, EventArgs e)
        {
            const int FudgeFactor = 10;
            labelMessage.MaximumSize = new Size(tableLayoutPanelStats.ClientSize.Width - tableLayoutPanelStats.Margin.Left - tableLayoutPanelStats.Margin.Right - FudgeFactor, 0);
        }

        private void PictureBoxDetail1_MouseEnter(object sender, EventArgs e)
        {
            toolTipDetail1.Show("Original image preview detail", pictureBoxDetail1);
        }

        private void PictureBoxDetail1_MouseLeave(object sender, EventArgs e)
        {
            toolTipDetail1.Hide(pictureBoxDetail1);
        }

        private void PictureBoxDetail2_MouseEnter(object sender, EventArgs e)
        {
            toolTipDetail2.Show("Shrunk image preview detail", pictureBoxDetail2);
        }

        private void PictureBoxDetail2_MouseLeave(object sender, EventArgs e)
        {
            toolTipDetail2.Hide(pictureBoxDetail2);
        }

        private void DataGridViewFiles_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dataGridViewFiles.CurrentCell.ColumnIndex != 0) // Don't immediately commit for filename edits
            {
                this.dataGridViewFiles.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            UpdatePrimary();
        }

        private void FormatRow(int rowIndex)
        {
            foreach (DataGridViewCell cell in this.dataGridViewFiles.Rows[rowIndex].Cells)
            {
                FormatCell(cell.RowIndex, cell.ColumnIndex, cell.Style);
            }
        }

        private void FormatCell(int rowIndex, int columnIndex, DataGridViewCellStyle cellStyle)
        {
            Item item = items[rowIndex];

            DataGridViewCellStyle defaultCellStyle = this.dataGridViewFiles.DefaultCellStyle;
            DataGridViewCell cell = this.dataGridViewFiles.Rows[rowIndex].Cells[columnIndex];

            cellStyle.BackColor = rowIndex == this.swapIndex ? SystemColors.ControlLight : defaultCellStyle.BackColor;

            switch (item.Status)
            {
                case Status.Valid:
                    cellStyle.ForeColor = defaultCellStyle.ForeColor;
                    if (item.TagColor != Color.Black)
                    {
                        cellStyle.ForeColor = item.TagColor;
                    }
                    break;
                case Status.Invalid:
                    cellStyle.ForeColor = Color.DarkRed;
                    break;
                case Status.Pending:
                    cellStyle.ForeColor = SystemColors.ControlDark;
                    break;
            }

            if (String.Equals(this.dataGridViewFiles.Columns[columnIndex].Name, "shrinkDataGridViewCheckBoxColumn"))
            {
                cell.ReadOnly = item.Status != Status.Valid;
            }

            if (item.Status != Status.Invalid)
            {
                cellStyle.Font = defaultCellStyle.Font;
            }
            else
            {
                Font defaultFont = this.dataGridViewFiles.DefaultCellStyle.Font;
                if (this.strikeOutFont == null)
                {
                    this.strikeOutFont = new Font(defaultFont, (FontStyle)((int)defaultFont.Style ^ (int)FontStyle.Strikeout));
                }
                cellStyle.Font = this.strikeOutFont;
            }
        }
        private Font strikeOutFont;

        private void DataGridViewFiles_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            FormatCell(e.RowIndex, e.ColumnIndex, e.CellStyle);
        }

        // HACKS: DataGridView will call DataGridViewFiles_CellFormatting on ALL cells in a column for a format change
        // to any of them when AutoSizeMode is auto anything. We know our format doesn't change width, so allow suspend
        // during bulk changes.
        private DataGridViewAutoSizeColumnMode[] savedColumnAutoSizes;
        public void SuspendDataGridViewReformatting()
        {
            this.savedColumnAutoSizes = new DataGridViewAutoSizeColumnMode[this.dataGridViewFiles.ColumnCount];
            for (int i = 0; i < this.dataGridViewFiles.ColumnCount; i++)
            {
                this.savedColumnAutoSizes[i] = this.dataGridViewFiles.Columns[i].AutoSizeMode;
                this.dataGridViewFiles.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
        public void ResumeDataGridViewReformatting()
        {
            for (int i = 0; i < this.dataGridViewFiles.ColumnCount; i++)
            {
                this.dataGridViewFiles.Columns[i].AutoSizeMode = this.savedColumnAutoSizes[i];
            }
            this.savedColumnAutoSizes = null;
        }

        private void DataGridViewFiles_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0) // "RenamedFileName"
            {
                if (((string)e.FormattedValue).IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    e.Cancel = true;
                    MessageBox.Show("New name contains characters not permitted in a file name.");
                    return;
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (i != e.RowIndex)
                    {
                        if (String.Equals(items[i].SourceFileName, e.FormattedValue)
                            || String.Equals(items[i].RenamedFileName, e.FormattedValue))
                        {
                            e.Cancel = true;
                            MessageBox.Show("File cannot be renamed to the name of another file");
                            return;
                        }
                    }
                }
            }
        }

        private void DataGridViewFiles_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dataGridViewFiles.CurrentCell == null)
            {
                return;
            }

            int row = this.dataGridViewFiles.CurrentCell.RowIndex;
            Item item = items[row];
            if (item != lastItem)
            {
                lastItem = item;
                UpdatePrimary();

                PreloadNearbyCacheItems();
            }
            labelMessage.Text = item.Message; // not data-bound, so must update explicitly
        }

        private void DataGridViewFiles_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Application.Idle += Idle_ReselectDataGridViewCurrentTextEdit;
            e.Control.TextChanged += Control_TextChanged;
        }

        // Another annoying HACK to deal with DataGridView selecting text after BeginEdit() after any applicable event fires,
        // preventing us from doing a custom selection. Instead, wait until idle and do it.
        private void Idle_ReselectDataGridViewCurrentTextEdit(object sender, EventArgs e)
        {
            Application.Idle -= Idle_ReselectDataGridViewCurrentTextEdit;
            if (this.dataGridViewFiles.CurrentCell.ColumnIndex == 0) // filename
            {
                TextBox textBox = (TextBox)this.dataGridViewFiles.EditingControl;
                if (textBox != null)
                {
                    textBox.TextChanged -= Control_TextChanged;
                    int i = textBox.Text.LastIndexOf('.');
                    if (i < 0)
                    {
                        i = textBox.Text.Length;
                    }
                    textBox.Select(0, i);
                }
            }
        }

        // 2nd part of HACK to prevent wrecking selection if user has already typed before Idle occurs
        private void Control_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = ((TextBox)sender);

            textBox.TextChanged -= Control_TextChanged;
            Application.Idle -= Idle_ReselectDataGridViewCurrentTextEdit;

            // If entered by directly typing, append the file extension automatically. Heuristic of .Length != 1
            // to detect pasting, in which case do not append extension.
            if (textBox.Text.Length == 1)
            {
                textBox.Text = textBox.Text + Path.GetExtension(CurrentItem.RenamedFileName);
                textBox.Select(1, 0);
            }
        }

        // Annoying HACK to deal with DataGridView providing no "finished" event and always resetting the
        // selection after any seemingly useful event (such as DataBindingComplete)
        private void Idle_RestoreDataGridViewSelectedCells(object sender, EventArgs e)
        {
            Application.Idle -= Idle_RestoreDataGridViewSelectedCells;

            this.dataGridViewFiles.Enabled = true;

            this.lastItem = null;
            this.dataGridViewFiles.ClearSelection();
            foreach (int index in options.LastSelected)
            {
                if ((index >= 0) && (index < items.Count))
                {
                    if (this.lastItem == null)
                    {
                        this.lastItem = items[index];
                        this.dataGridViewFiles.CurrentCell = this.dataGridViewFiles.Rows[index].Cells[0];
                    }
                    this.dataGridViewFiles.Rows[index].Selected = true;
                }
            }
            if (this.lastItem == null)
            {
                this.dataGridViewFiles.Rows[0].Selected = true;
                this.dataGridViewFiles.CurrentCell = this.dataGridViewFiles.Rows[0].Cells[0];
                this.lastItem = items[0];
            }

            this.dataGridViewFiles.SelectionChanged += DataGridViewFiles_SelectionChanged;
            this.dataGridViewFiles.CurrentCellChanged += DataGridViewFiles_CurrentCellChanged;
            this.dataGridViewFiles.CurrentCellDirtyStateChanged += DataGridViewFiles_CurrentCellDirtyStateChanged;

            this.dataGridViewFiles.Focus();

            UpdatePrimary();
        }

        private void DataGridViewFiles_SelectionChanged(object sender, EventArgs e)
        {
            List<int> rows = new List<int>();
            for (int i = 0; i < items.Count; i++)
            {
                bool selected = false;
                for (int j = 0; j < this.dataGridViewFiles.ColumnCount; j++)
                {
                    if (selected = selected || this.dataGridViewFiles[j, i].Selected)
                    {
                        break;
                    }
                }
                if (selected)
                {
                    rows.Add(i);
                }
            }
            options.LastSelected = rows.ToArray();
        }

        private void SelectItem(int rowIndex)
        {
            SelectItem(this.items[rowIndex]);
        }

        private void SelectItem(Item item)
        {
            int index = this.items.IndexOf(item);
            Debug.Assert(index >= 0);
            this.dataGridViewFiles.CurrentCell = this.dataGridViewFiles.Rows[index].Cells[this.dataGridViewFiles.CurrentCell.ColumnIndex];
        }

        [Flags]
        private enum Preloads
        {
            Source = 1,
            ShrunkExpanded = 2,
            Annotated = 4,

            All = 127,
        };

        private readonly static KeyValuePair<int, Preloads>[] PreloadList = new KeyValuePair<int, Preloads>[]
        {
            new KeyValuePair<int, Preloads>(0, Preloads.All),
            new KeyValuePair<int, Preloads>(1, Preloads.Source | Preloads.ShrunkExpanded),
            new KeyValuePair<int, Preloads>(2, Preloads.Source),
            new KeyValuePair<int, Preloads>(-1, Preloads.Source | Preloads.ShrunkExpanded),
        };

        private void PreloadNearbyCacheItems()
        {
            if (Program.ProfileMode)
            {
                return;
            }
            if (this.dataGridViewFiles.CurrentCell == null)
            {
                return;
            }

            int row = this.dataGridViewFiles.CurrentCell.RowIndex;
            Item currentItem = items[row];

            bool showAnnotations = toolStripButtonShowAnnotationsPrimary.Checked;
            bool showAnnotationsDetail = toolStripButtonShowAnnotationsDetail.Checked;
            bool showAnnotationsHF = toolStripButtonShowAnnotationsPrimary.Checked;
            bool showShrunkExpandedPreview = toolStripButtonShowShrunkExpandedPreview.Checked;
            bool showAutoCropGrid = this.showAutoCropGrid;
            bool showPolyUnbiasGrid = this.showPolyUnbiasGrid;
            bool showCropRectPads = toolStripButtonCrop.Checked;

            Queue<Task<bool>> tasks = new Queue<Task<bool>>();

            // load queue in order of importance
            foreach (KeyValuePair<int, Preloads> o in PreloadList)
            {
                int i = row + o.Key;
                if ((i < 0) || (i >= items.Count))
                {
                    continue;
                }

                Item cacheItem = items[i];

                if (!showAnnotationsDetail && ((o.Value & Preloads.Source) != 0))
                {
                    tasks.Enqueue(new Task<bool>(
                        delegate ()
                        {
                            if (currentItem != this.CurrentItem)
                            {
                                return false; // stop preload if user changes current item
                            }

                            using (BitmapHolder holder = cacheItem.GetSourceBitmapHolder())
                            {
                                holder.Wait();
                            }
                            return false;
                        }));
                }

                if ((!inDragForCrop || !(geoDrag != GeoCorner.None) || (i != row)) && ((o.Value & Preloads.ShrunkExpanded) != 0))
                {
                    tasks.Enqueue(new Task<bool>(
                        delegate ()
                        {
                            if (currentItem != this.CurrentItem)
                            {
                                return false; // stop preload if user changes current item
                            }

                            using (BitmapHolder holder = cacheItem.GetPreviewBitmapHolder(
                                showShrunkExpandedPreview,
                                false/*inDrag*/,
                                currentItem.NormalizeGeometry))
                            {
                                holder.Wait();
                            }
                            return false;
                        }));

                    if (showAnnotations && ((o.Value & Preloads.Annotated) != 0))
                    {
                        tasks.Enqueue(new Task<bool>(
                            delegate ()
                            {
                                if (currentItem != this.CurrentItem)
                                {
                                    return false; // stop preload if user changes current item
                                }

                                using (BitmapHolder holder = cacheItem.GetAnnotatedBitmapHolder(
                                    showShrunkExpandedPreview,
                                    false/*inDrag*/,
                                    showAnnotationsHF,
                                    CurrentItem.NormalizeGeometry,
                                    showAutoCropGrid,
                                    showPolyUnbiasGrid,
                                    showCropRectPads))
                                {
                                    holder.Wait();
                                }
                                return false;
                            }));
                    }
                }
            }

            Task<bool> driver = new Task<bool>(
                delegate ()
                {
                    //int p = !Program.ProfileMode ? Math.Max(1, Environment.ProcessorCount - 1/*leave one for UI thread*/) : 1;
                    int p = tasks.Count; // some block on others - so just let 'em all rip
                    Task<bool>[] inFlight = new Task<bool>[p];

                    for (int i = 0; (i < inFlight.Length) && (tasks.Count != 0); i++)
                    {
                        inFlight[i] = tasks.Dequeue();
                        inFlight[i].Start();
                    }
                    while (tasks.Count != 0)
                    {
                        int i = Task.WaitAny(inFlight, CancellationToken.None);
                        inFlight[i] = tasks.Dequeue();
                        inFlight[i].Start();
                    }

                    //Task.WaitAll(inFlight, CancellationToken.None); -- not needed

                    return false;
                });
            driver.Start();
        }

        private void ToolStripButtonShowOriginalInsteadOfProcessed_CheckedChanged(object sender, EventArgs e)
        {
            options.ShowOriginalInsteadOfProcessed = this.toolStripButtonShowOriginalInsteadOfProcessed.Checked;
            UpdatePrimary();
        }

        private void ToolStripButtonShowAnnotationsPrimary_CheckedChanged(object sender, EventArgs e)
        {
            options.ShowAnnotationsPrimary = this.toolStripButtonShowAnnotationsPrimary.Checked;
            UpdatePrimary();
        }

        private void ToolStripButtonShowAnnotationsDetail_CheckedChanged(object sender, EventArgs e)
        {
            options.ShowAnnotationsDetail = this.toolStripButtonShowAnnotationsDetail.Checked;
            UpdatePrimary();
        }

        private void ToolStripButtonShowShrunkExpandedPreview_CheckedChanged(object sender, EventArgs e)
        {
            options.ShowShrinkExpandDetail = this.toolStripButtonShowShrunkExpandedPreview.Checked;
            UpdateDetail();
        }

        private void ToolStripButtonUseGDIResize_CheckedChanged(object sender, EventArgs e)
        {
            this.cache.TryClear();
            Program.UseGDIResize = this.toolStripButtonUseGDIResize.Checked;
            UpdatePrimary();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Item currentItem = CurrentItem;

            if (String.Equals(e.PropertyName, "Status"))
            {
                // DataGridView doesn't handle binding for formatting, so this simulates it
                int i = this.items.IndexOf((Item)sender);
                FormatRow(i);
                if (CurrentItem == sender)
                {
                    UpdatePrimary();
                }
                return;
            }

            PropertyFlags action = PropertyFlags.All;
            int j = Array.FindIndex(PropertyActions, delegate (KeyValuePair<string, PropertyFlags> candidate) { return String.Equals(e.PropertyName, candidate.Key); });
            if (j >= 0)
            {
                action = PropertyActions[j].Value;
            }
            if ((action & PropertyFlags.SaveSettings) != 0)
            {
                Application.Idle += Application_Idle_SaveSettings;
            }
            if ((action & PropertyFlags.UpdatePrimary) != 0)
            {
                if (sender == currentItem)
                {
                    UpdatePrimary();
                }
            }
        }

        [Flags]
        private enum PropertyFlags { None = 0, SaveSettings = 1, UpdatePrimary = 2, All = 127 };
        private readonly static KeyValuePair<string, PropertyFlags>[] PropertyActions = new KeyValuePair<string, PropertyFlags>[]
        {
            new KeyValuePair<string, PropertyFlags>("Delete", PropertyFlags.SaveSettings),
            new KeyValuePair<string, PropertyFlags>("Message", PropertyFlags.None),
        };

        private void Application_Idle_SaveSettings(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle_SaveSettings;
            SaveSettings();
        }

        private void UIElementRestoreFocus_Click(object sender, EventArgs e)
        {
            this.dataGridViewFiles.Focus();
        }

        private int reentranceSuppression;

        private void ToolStripButtonCrop_CheckedChanged(object sender, EventArgs e)
        {
            if (reentranceSuppression == 0)
            {
                reentranceSuppression++;
                this.toolStripButtonNormalizeGeometryCorners.Checked = false;
                reentranceSuppression--;
                UpdatePrimary();
            }
        }

        private void ToolStripButtonClearCrop_Click(object sender, EventArgs e)
        {
            if (CurrentItem != null)
            {
                CurrentItem.CropRect = new Rectangle();
                UpdatePrimary();
            }
        }

        private void ToolStripButtonNormalizeGeometryCorners_CheckedChanged(object sender, EventArgs e)
        {
            if (reentranceSuppression == 0)
            {
                reentranceSuppression++;
                this.toolStripButtonCrop.Checked = false;
                reentranceSuppression--;
                UpdatePrimary();
            }
        }

        private void toolStripButtonCustomAspectRatio_Click(object sender, EventArgs e)
        {
            using (AspectRatioDialog dialog = new AspectRatioDialog(CurrentItem != null ? new SizeF(CurrentItem.Width, CurrentItem.Height) : new SizeF(), String.Format("Set Cropping Aspect Ratio for {0}", CurrentItem.RenamedFileName)))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.UseCurrent)
                    {
                        aspectRatioOverride = null;
                    }
                    else
                    {
                        aspectRatioOverride = dialog.Aspect;
                    }
                    this.toolStripButtonCropKeepAspectRatio.Checked = true;
                    this.toolStripButtonCrop.Checked = true;
                }
            }
        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            List<Item> items = new List<Item>();
            Dictionary<int, bool> alreadyAddedRows = new Dictionary<int, bool>();
            foreach (DataGridViewCell cell in this.dataGridViewFiles.SelectedCells)
            {
                int row = cell.RowIndex;
                if (!alreadyAddedRows.ContainsKey(row))
                {
                    items.Add(this.items[row]);
                    alreadyAddedRows.Add(row, false);
                }
            }
            if (items.Count != 0)
            {
                OptionsDialog.DoDialog(items);
                UpdatePrimary();
            }
        }

        private void toolStripButtonGlobalOptions_Click(object sender, EventArgs e)
        {
            using (GlobalOptionsDialog dialog = new GlobalOptionsDialog(this.options, null))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            this.lastAnalysisTask.Cancel();

            this.cache.TryClear();

            this.lastAnalysisTask = BatchAnalyzerQueue.BeginAnalyzeBatch(this.items, this);

            UpdatePrimary();
            PreloadNearbyCacheItems();
        }

        private void toolStripButtonRotateLeft90_Click(object sender, EventArgs e)
        {
            Item item = CurrentItem;
            if (item != null)
            {
                item.RightRotations = (item.RightRotations - 1) & 3;
            }
        }

        private void toolStripButtonRotateRight90_Click(object sender, EventArgs e)
        {
            Item item = CurrentItem;
            if (item != null)
            {
                item.RightRotations = (item.RightRotations + 1) & 3;
            }
        }

        private void toolStripButtonSetNormalizedGeometryAspectRatioExplicitly_Click(object sender, EventArgs e)
        {
            Item currentItem = this.CurrentItem;
            if (currentItem != null)
            {
                SizeF currentAspectRatio = new SizeF(
                    ((currentItem.CornerTR.X - currentItem.CornerTL.X) + (currentItem.CornerBR.X - currentItem.CornerBL.X)) / 2,
                    ((currentItem.CornerBL.Y - currentItem.CornerTL.Y) + (currentItem.CornerBR.Y - currentItem.CornerTR.Y)) / 2);
                using (AspectRatioDialog dialog = new AspectRatioDialog(currentAspectRatio, String.Format("Set Normalize Geometry Aspect Ratio for {0}", currentItem.RenamedFileName)))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        currentItem.NormalizeGeometryForcedAspectRatio = dialog.Aspect;
                    }
                }
            }
        }

        private void toolStripButtonResetNormalizedGeometryAspectRatio_Click(object sender, EventArgs e)
        {
            Item currentItem = this.CurrentItem;
            if (currentItem != null)
            {
                currentItem.NormalizeGeometryForcedAspectRatio = null;
            }
        }

        private void toolStripButtonResetAllNormalizedGeometry_Click(object sender, EventArgs e)
        {
            Item currentItem = this.CurrentItem;
            if (currentItem != null)
            {
                currentItem.NormalizeGeometryExplicitlySet = false;
            }
        }

        private static void SetPictureBoxImage(PictureBox pictureBox, Image image)
        {
            Image old = pictureBox.Image;
            pictureBox.Image = image;
            if (old != null)
            {
                old.Dispose();
            }
        }

        private void EnsureDetails()
        {
            Debug.Assert(this.pictureBoxDetail1.ClientSize == this.pictureBoxDetail2.ClientSize);
            Size size = this.pictureBoxDetail1.ClientSize;

            if ((this.detail1 != null) && (size != this.detail1.Size))
            {
                this.pictureBoxDetail1.Image = null;
                this.pictureBoxDetail2.Image = null;
                this.detail1.Dispose();
                this.detail2.Dispose();
                this.detail1 = null;
                this.detail2 = null;
            }

            if (this.detail1 == null)
            {
                this.detail1 = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppRgb);
                this.detail2 = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppRgb);
                EraseDetails();
                this.pictureBoxDetail1.Image = this.detail1;
                this.pictureBoxDetail2.Image = this.detail2;
            }
        }

        private void EraseDetails()
        {
            using (Brush brush = new SolidBrush(Color.FromKnownColor(KnownColor.ControlLight)))
            {
                using (Graphics graphics = Graphics.FromImage(detail1))
                {
                    graphics.FillRectangle(brush, 0, 0, detail1.Width, detail1.Height);
                }

                using (Graphics graphics = Graphics.FromImage(detail2))
                {
                    graphics.FillRectangle(brush, 0, 0, detail2.Width, detail2.Height);
                }
            }
        }

        private delegate void UpdatePrimaryCompletionDelegate(Item item, BitmapHolder newPictureBoxMainHolder);
        private void UpdatePrimaryCompletion(Item item, BitmapHolder pictureBoxMainHolder)
        {
            BitmapHolder pictureBoxMainHolderOld = null;
            try
            {
                pictureBoxMainHolderOld = this.pictureBoxMainHolder; // must clear pictureBoxMain.Image first (via SetPictureBoxImage)
                this.pictureBoxMainHolder = null;

                if (closing)
                {
                    return;
                }

                UpdateWindowTitle();

                this.pictureBoxMainHolder = pictureBoxMainHolder;

                if (pictureBoxMain.Cursor == Cursors.WaitCursor)
                {
                    pictureBoxMain.Cursor = Cursors.Arrow;
                }

                if (item != null)
                {
                    int width, height;
                    if (item.Valid)
                    {
                        Debug.Assert(pictureBoxMainHolder.IsCompleted); // task should have blocked until completion
                        ManagedBitmap bitmap = pictureBoxMainHolder.Bitmap;
                        SetPictureBoxImage(pictureBoxMain, bitmap.GetSectionEnslaved(new Rectangle(new Point(), bitmap.Size)));
                        width = bitmap.Width;
                        height = bitmap.Height;
                    }
                    else
                    {
                        SetPictureBoxImage(pictureBoxMain, new Bitmap(Properties.Resources.InvalidPlaceHolder));
                        width = 0;
                        height = 0;
                    }

                    if (item.Valid)
                    {
                        labelDimensionsOriginal.Text = String.Format("{0} x {1}", width, height);
                        Size dim = item.CropRect.IsEmpty ? new Size(width, height) : new Size(item.CropRect.Width, item.CropRect.Height);
                        if (item.Shrink)
                        {
                            dim = new Size((int)(dim.Width / item.ShrinkFactor), (int)(dim.Height / item.ShrinkFactor));
                        }
                        labelDimensionsShrunk.Text = String.Format("{0} x {1}", dim.Width, dim.Height);
                        labelSizeOriginal.Text = FileSizeText.GetFileSizeString(item.SourcePath);
                        // TODO: estimate impact of JpegQuality; handle estimating one-bit sizing
                        int shrunkFileSize = (int)((float)FileSizeText.GetFileLength(item.SourcePath) / (item.Shrink ? item.ShrinkFactor * item.ShrinkFactor : 1)
                            * (!item.CropRect.IsEmpty ? ((float)item.CropRect.Width * item.CropRect.Height) / ((float)width * height) : 1f));
                        labelSizeShrunk.Text = FileSizeText.GetFileSizeString(shrunkFileSize);
                    }
                    else
                    {
                        labelDimensionsOriginal.Text = "-";
                        labelDimensionsShrunk.Text = "-";
                        labelSizeOriginal.Text = "-";
                        labelSizeShrunk.Text = "-";
                    }
                }
                else
                {
                    SetPictureBoxImage(pictureBoxMain, null);
                    EraseDetails();

                    labelDimensionsOriginal.Text = "-";
                    labelDimensionsShrunk.Text = "-";
                    labelSizeOriginal.Text = "-";
                    labelSizeShrunk.Text = "-";
                }
            }
            finally
            {
                if (pictureBoxMainHolderOld != null)
                {
                    pictureBoxMainHolderOld.Dispose();
                }
            }
        }

        private CancellationTokenSource lastUpdatePrimaryCancellation;
        private void UpdatePrimary()
        {
            bool inDrag = this.inDragForCrop || (this.geoDrag != GeoCorner.None);

            if (lastUpdatePrimaryCancellation != null)
            {
                if (inDrag)
                {
                    return; // do not generate scads of in-flight image regenerations by cancelling and restarting during MouseMove
                }

                lastUpdatePrimaryCancellation.Cancel();
                lastUpdatePrimaryCancellation = null;
            }

            Item item = CurrentItem;
            if (item == null)
            {
                UpdatePrimaryCompletion(null, null);
                return;
            }

            if (this.lastAnalysisTask != null)
            {
                this.lastAnalysisTask.Prioritize(item);
            }

            if (!inDrag && !this.toolStripButtonNormalizeGeometryCorners.Checked)
            {
                pictureBoxMain.Cursor = Cursors.WaitCursor;
            }

            bool showShrunkExpanded = toolStripButtonShowShrunkExpandedPreview.Checked;
            bool showAnnotations = toolStripButtonShowAnnotationsPrimary.Checked;
            bool showNormalizedGeometry = !this.toolStripButtonNormalizeGeometryCorners.Checked;
            bool showAutoCropGrid = this.showAutoCropGrid;
            bool showPolyUnbiasGrid = this.showPolyUnbiasGrid;
            bool showCropRectPads = toolStripButtonCrop.Checked;

            CancellationTokenSource currentCancellation = lastUpdatePrimaryCancellation = new CancellationTokenSource();
            Task<bool> task = new Task<bool>(
                delegate ()
                {
                    BitmapHolder newPictureBoxMainHolder = null;

                    try
                    {
                        newPictureBoxMainHolder = !toolStripButtonShowOriginalInsteadOfProcessed.Checked
                            ? item.GetAnnotatedBitmapHolder(
                                showShrunkExpanded,
                                inDrag,
                                showAnnotations,
                                showNormalizedGeometry,
                                showAutoCropGrid,
                                showPolyUnbiasGrid,
                                showCropRectPads)
                            : item.GetSourceBitmapHolder();
                        newPictureBoxMainHolder.Wait(currentCancellation.Token);

                        if (!currentCancellation.IsCancellationRequested)
                        {
                            UpdatePrimaryCompletionDelegate d = new UpdatePrimaryCompletionDelegate(this.UpdatePrimaryCompletion);
                            this.Invoke(d, new object[] { item, newPictureBoxMainHolder });
                            newPictureBoxMainHolder = null;
                        }
                    }
                    catch (Exception exception)
                    {
                        Program.Log(LogCat.All, exception.ToString() + Environment.NewLine);
                        throw;
                    }
                    finally
                    {
                        if (newPictureBoxMainHolder != null)
                        {
                            newPictureBoxMainHolder.Dispose();
                        }
                    }

                    if (lastUpdatePrimaryCancellation == currentCancellation)
                    {
                        lastUpdatePrimaryCancellation = null;
                    }
                    return false;
                },
                currentCancellation.Token);
            task.Start();


            UpdateDetail();
        }

        private delegate void UpdateDetailCompletionDelegate(Item item, BitmapHolder imageHolder1, BitmapHolder imageHolder2);
        private void UpdateDetailCompletion(Item item, BitmapHolder imageHolder1, BitmapHolder imageHolder2)
        {
            pictureBoxDetail1.Cursor = Cursors.Arrow;
            pictureBoxDetail2.Cursor = Cursors.Arrow;

            using (imageHolder1) // always dispose
            {
                using (imageHolder2) // always dispose
                {
                    if (closing)
                    {
                        return;
                    }

                    EnsureDetails();

                    if ((item != null) && item.Valid)
                    {
                        using (Graphics graphics = Graphics.FromImage(detail1))
                        {
                            int divisor = (inDragForCrop || (this.geoDrag != GeoCorner.None)) ? Item.AnnotationDivisor : 1; // for fast drag mode
                            graphics.FillRectangle(Brushes.Black, 0, 0, detail1.Width, detail1.Height);
                            using (Bitmap section = imageHolder1.Bitmap.GetSectionEnslaved(new Rectangle(detailOffset.X / divisor, detailOffset.Y / divisor, detail1.Width / divisor, detail1.Height / divisor)))
                            {
                                graphics.DrawImage(section, 0, 0, detail1.Width, detail1.Height);
                            }
                        }

                        using (Graphics graphics = Graphics.FromImage(detail2))
                        {
                            graphics.FillRectangle(Brushes.Black, 0, 0, detail2.Width, detail2.Height);
                            using (Bitmap section = imageHolder2.Bitmap.GetSectionEnslaved(new Rectangle(detailOffset.X, detailOffset.Y, detail1.Width, detail1.Height)))
                            {
                                graphics.DrawImage(section, 0, 0, detail1.Width, detail1.Height);
                            }
                        }
                    }
                    else
                    {
                        using (Graphics graphics = Graphics.FromImage(detail1))
                        {
                            graphics.DrawImage(Properties.Resources.InvalidPlaceHolder, new Rectangle(new Point(), detail1.Size));
                        }
                        using (Graphics graphics = Graphics.FromImage(detail2))
                        {
                            graphics.DrawImage(Properties.Resources.InvalidPlaceHolder, new Rectangle(new Point(), detail1.Size));
                        }
                    }
                }
            }

            pictureBoxDetail1.Invalidate();
            pictureBoxDetail1.Update();
            pictureBoxDetail2.Invalidate();
            pictureBoxDetail2.Update();
        }

        private CancellationTokenSource lastUpdateDetailCancellation;
        private void UpdateDetail()
        {
            bool inDrag = this.inDragForCrop || (this.geoDrag != GeoCorner.None);

            if (lastUpdateDetailCancellation != null)
            {
                if (inDrag)
                {
                    return; // do not generate scads of in-flight image regenerations by cancelling and restarting during MouseMove
                }

                lastUpdateDetailCancellation.Cancel();
                lastUpdateDetailCancellation = null;
            }

            Item item = CurrentItem;
            if (item == null)
            {
                UpdateDetailCompletion(null, null, null);
                return;
            }


            // UI settings
            bool showShrunkExpanded = toolStripButtonShowShrunkExpandedPreview.Checked;
            bool showAnnotations = toolStripButtonShowAnnotationsPrimary.Checked;
            bool showAnnotationsDetail = toolStripButtonShowAnnotationsDetail.Checked;
            bool showNormalizedGeometry = !this.toolStripButtonNormalizeGeometryCorners.Checked;
            bool showAutoCropGrid = this.showAutoCropGrid;
            bool showPolyUnbiasGrid = this.showPolyUnbiasGrid;
            bool showCropRectPads = toolStripButtonCrop.Checked;

            pictureBoxDetail1.Cursor = Cursors.WaitCursor;
            pictureBoxDetail2.Cursor = Cursors.WaitCursor;

            CancellationTokenSource currentCancellation = lastUpdateDetailCancellation = new CancellationTokenSource();
            Task<bool> task = new Task<bool>(
                delegate ()
                {
                    BitmapHolder imageHolder1 = null;
                    BitmapHolder imageHolder2 = null;

                    try
                    {
                        bool combinedNormalizeGeometry = item.NormalizeGeometry && showNormalizedGeometry;

                        imageHolder1 = !showAnnotationsDetail
                            ? (!combinedNormalizeGeometry
                                ? item.GetSourceBitmapHolder()
                                : item.GetSourceBitmapHolderWithNormalizeGeometry())
                            : item.GetAnnotatedBitmapHolder(
                                showShrunkExpanded,
                                inDrag,
                                showAnnotations,
                                combinedNormalizeGeometry,
                                showAutoCropGrid,
                                showPolyUnbiasGrid,
                                 showCropRectPads);

                        imageHolder2 = item.GetPreviewBitmapHolder(
                            showShrunkExpanded,
                            inDrag,
                            combinedNormalizeGeometry);

                        imageHolder1.Wait(currentCancellation.Token);
                        imageHolder2.Wait(currentCancellation.Token);

                        if (!currentCancellation.IsCancellationRequested)
                        {
                            UpdateDetailCompletionDelegate d = new UpdateDetailCompletionDelegate(this.UpdateDetailCompletion);
                            this.Invoke(d, new object[] { item, imageHolder1, imageHolder2 });
                            imageHolder1 = null;
                            imageHolder2 = null;
                        }
                    }
                    catch (Exception exception)
                    {
                        Program.Log(LogCat.All, exception.ToString() + Environment.NewLine);
                        throw;
                    }
                    finally
                    {
                        if (imageHolder1 != null)
                        {
                            imageHolder1.Dispose();
                        }
                        if (imageHolder2 != null)
                        {
                            imageHolder2.Dispose();
                        }
                    }

                    if (lastUpdateDetailCancellation == currentCancellation)
                    {
                        lastUpdateDetailCancellation = null;
                    }
                    return false;
                },
                currentCancellation.Token);
            task.Start();
        }

        public const string SourceDirectorySuffix = "-original";

        private void SaveSettings()
        {
            Debugger.Log(0, null, "SaveSettings()" + Environment.NewLine);

            string sourceDirectory = String.Concat(directory, SourceDirectorySuffix);

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.IndentChars = new string(' ', 4);

            string settingsPath = Path.Combine(Directory.Exists(sourceDirectory) ? sourceDirectory : directory, Program.SettingsFile);
            using (XmlWriter writer = XmlWriter.Create(settingsPath, writerSettings))
            {
                writer.WriteStartElement("root");

                options.Save(writer);

                writer.WriteStartElement("items");
                foreach (Item item in items)
                {
                    item.WriteXml(writer);
                }
                writer.WriteEndElement(); // items

                writer.WriteEndElement(); // root
            }
        }

        private bool inApply; // TODO: remove if this isn't a problem
        private void toolStripButtonApply_Click(object sender, EventArgs e)
        {
            SaveSettings();


            this.inApply = true;


            string sourceDirectory = String.Concat(directory, SourceDirectorySuffix);

            string sourceMessage = String.Format(
                Directory.Exists(sourceDirectory)
                    ? "Reading from source directory \"{0}\" and overwriting files in \"{1}\"."
                    : "Backing up to source directory \"{0}\" and overwriting files in \"{1}\".",
                Path.GetFileName(sourceDirectory),
                Path.GetFileName(directory));
            if (MessageBox.Show(String.Format("About to begin modification of files. {0} Continue?", sourceMessage), null, MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            // backup to source directory, if needed
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
                foreach (string source in Directory.GetFiles(directory))
                {
                    if (String.Equals(Path.GetFileName(source), Program.SettingsFile, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string target = Path.Combine(sourceDirectory, Path.GetFileName(source));
                    if (File.Exists(source) && !File.Exists(target))
                    {
                        File.Copy(source, target, true/*overwrite*/);
                        File.SetCreationTime(target, File.GetCreationTime(source));
                        File.SetLastWriteTime(target, File.GetLastWriteTime(source));
                    }
                }

                if (File.Exists(Path.Combine(directory, Program.SettingsFile)))
                {
                    File.Move(Path.Combine(directory, Program.SettingsFile), Path.Combine(sourceDirectory, Program.SettingsFile));
                }
            }

            this.cache.TryClear(); // free up memory for processing that follows


            // modify files
            OutputTransforms.Stats stats = new OutputTransforms.Stats();
            int maximum = 0;
            for (int i = 0; i < this.items.Count; i++)
            {
                if (!this.items[i].Delete)
                {
                    maximum++;
                }
            }
            CancellationTokenSource cancel = new CancellationTokenSource();
            using (ProgressDialog progressDialog = new ProgressDialog(maximum, this.items.Count, cancel))
            {
                // can't start until dialog is visible, but ShowDialog() blocks
                progressDialog.Shown += delegate (object _sender, EventArgs _e)
                {
                    OutputTransforms.StartFinalOutput(this.items, stats, progressDialog, cancel);
                };

                progressDialog.ShowDialog(); // exits when task completes
            }


            PreloadNearbyCacheItems();

            if (!cancel.IsCancellationRequested)
            {
                MessageBox.Show(String.Format("File update finished: {0} files deleted, {1} files shrunk, {3} files cropped, {2} files as original.", stats.deleted, stats.shrunk, stats.original, stats.cropped));
            }

            this.inApply = false;
        }


        // gestures

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if ((m.Msg == WM_KEYDOWN) || (m.Msg == WM_SYSKEYDOWN))
            {
                Keys key = (Keys)m.WParam & ~Keys.Modifiers;
                Item currentItem = this.CurrentItem;

                if ((ModifierKeys & Keys.Alt) == 0)
                {
                    switch (key)
                    {
                        default:
                            break;

                        case Keys.Escape:
                            if (!this.dataGridViewFiles.IsCurrentCellInEditMode
                                || (this.dataGridViewFiles.CurrentCell.ColumnIndex != 0))
                            {
                                ToolStripButtonClearCrop_Click(this, EventArgs.Empty);
                                return true;
                            }
                            break;

                        case Keys.F1:
                            Process.Start("https://programmatom.github.io/PhotoMunger/");
                            break;

                        case Keys.F4:
                            if (this.cacheView == null)
                            {
                                this.cacheView = new CacheView(this.cache);
                                this.cacheView.Show();
                                this.cacheView.FormClosed += CacheView_FormClosed;
                            }
                            else
                            {
                                this.cacheView.Activate();
                            }
                            return true;

                        case Keys.F5:
                            cache.InvalidatePrefixed(currentItem.SourceId);
                            UpdatePrimary();
                            return true;

                        case Keys.F6:
                            if (Program.logWindow == null)
                            {
                                Program.logWindow = new LoggingWindow();
                                Program.logWindow.Show();
                            }
                            else
                            {
                                Program.logWindow.Activate();
                            }
                            return true;

                        case Keys.F7:
                            showAutoCropGrid = !showAutoCropGrid;
                            options.ShowAutoCropGrid = showAutoCropGrid;
                            UpdatePrimary();
                            break;

                        case Keys.F8:
                            showPolyUnbiasGrid = !showPolyUnbiasGrid;
                            options.ShowPolyUnbiasGrid = showPolyUnbiasGrid;
                            UpdatePrimary();
                            break;

                        case Keys.F11:
                            if (!fullScreenMode)
                            {
                                fullScreenMode = true;
                                savedWindowStateForFullScreenMode = this.WindowState;

                                this.Hide();
                                this.WindowState = FormWindowState.Normal;
                                this.FormBorderStyle = FormBorderStyle.None;
                                this.WindowState = FormWindowState.Maximized;
                                this.Show();
                            }
                            else
                            {
                                fullScreenMode = false;

                                this.FormBorderStyle = FormBorderStyle.Sizable;
                                this.WindowState = savedWindowStateForFullScreenMode;
                            }
                            break;
                    }
                }

                if (((ModifierKeys & Keys.Alt) != 0) && (currentItem != null))
                {
                    switch (key)
                    {
                        default:
                            break;

                        case Keys.C:
                            this.savedCropRect = currentItem.CropRect;
                            return true;

                        case Keys.V:
                            currentItem.CropRect = this.savedCropRect;
                            UpdatePrimary();
                            return true;

                        case Keys.A:
                            this.toolStripButtonShowAnnotationsPrimary.Checked = !this.toolStripButtonShowAnnotationsPrimary.Checked;
                            return true;

                        case Keys.O:
                            this.toolStripButtonOptions_Click(this, EventArgs.Empty);
                            return true;

                        case Keys.M:
                            if (this.swapIndex >= 0)
                            {
                                this.dataGridViewFiles.InvalidateRow(this.swapIndex);
                            }
                            this.swapIndex = this.CurrentItemRow;
                            options.LastSelectedSwap = this.swapIndex;
                            this.dataGridViewFiles.InvalidateRow(this.swapIndex);
                            return true;

                        case Keys.S:
                            if (this.swapIndex >= 0)
                            {
                                this.dataGridViewFiles.InvalidateRow(this.swapIndex);
                                int savedIndex = this.CurrentItemRow;
                                this.SelectItem(this.swapIndex);
                                this.swapIndex = savedIndex;
                                options.LastSelectedSwap = this.swapIndex;
                                this.dataGridViewFiles.InvalidateRow(this.swapIndex);
                                return true;
                            }
                            break;

                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Up:
                        case Keys.Down:
                            int dx = 0;
                            int dy = 0;
                            switch ((Keys)key & ~Keys.Control)
                            {
                                default:
                                    Debug.Assert(false);
                                    break;
                                case Keys.Left:
                                    dx = -Transforms.LosslessCropGranularity;
                                    break;
                                case Keys.Right:
                                    dx = Transforms.LosslessCropGranularity;
                                    break;
                                case Keys.Up:
                                    dy = -Transforms.LosslessCropGranularity;
                                    break;
                                case Keys.Down:
                                    dy = Transforms.LosslessCropGranularity;
                                    break;
                            }
                            Rectangle rect;
                            switch (HitTestCropCorner(pictureBoxMain.PointToClient(Form.MousePosition)))
                            {
                                default:
                                    Debug.Assert(false);
                                    break;
                                case CropCorner.None:
                                    break;
                                case CropCorner.Center:
                                    rect = currentItem.CropRect;
                                    rect.Offset(new Point(dx, dy));
                                    currentItem.CropRect = rect;
                                    break;
                                case CropCorner.TopLeft:
                                    rect = currentItem.CropRect;
                                    currentItem.CropRect = new Rectangle(rect.X + dx, rect.Y + dy, rect.Width - dx, rect.Height - dy);
                                    break;
                                case CropCorner.TopRight:
                                    rect = currentItem.CropRect;
                                    currentItem.CropRect = new Rectangle(rect.X, rect.Y + dy, rect.Width + dx, rect.Height - dy);
                                    break;
                                case CropCorner.BottomLeft:
                                    rect = currentItem.CropRect;
                                    currentItem.CropRect = new Rectangle(rect.X + dx, rect.Y, rect.Width - dx, rect.Height + dy);
                                    break;
                                case CropCorner.BottomRight:
                                    rect = currentItem.CropRect;
                                    currentItem.CropRect = new Rectangle(rect.X, rect.Y, rect.Width + dx, rect.Height + dy);
                                    break;
                            }
                            UpdatePrimary();
                            return true;

                        case Keys.D1:
                        case Keys.D2:
                        case Keys.D3:
                        case Keys.D4:
                        case Keys.D5:
                        case Keys.D6:
                        case Keys.D7:
                        case Keys.D8:
                        case Keys.D9:
                            currentItem.TagColor = TagColors[(int)key - (int)Keys.D1];
                            this.dataGridViewFiles.InvalidateRow(this.dataGridViewFiles.CurrentRow.Index);
                            return true;
                    }
                }
            }

            return base.ProcessKeyPreview(ref m);
        }
        private readonly static Color[] TagColors = new Color[] { Color.Black, Color.Blue, Color.Red, Color.Yellow, Color.Orange, Color.Green, Color.Purple, Color.Cyan, Color.Pink };

        private void CacheView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.cacheView = null;
        }

        private enum CropCorner { None, TopLeft, TopRight, BottomLeft, BottomRight, Center, Top, Left, Right, Bottom };
        private CropCorner HitTestCropCorner(Point winLoc)
        {
            Item item = CurrentItem;
            if ((item != null) && !item.CropRect.IsEmpty)
            {
                const int Radius = 12;

                Point cropTL = ImageToWindowPosition(new Point(item.CropRect.Left, item.CropRect.Top), pictureBoxMain, item.Width, item.Height, null);
                Point cropBR = ImageToWindowPosition(new Point(item.CropRect.Right, item.CropRect.Bottom), pictureBoxMain, item.Width, item.Height, null);
                int cropRectMidX = (cropTL.X + cropBR.X) / 2;
                int cropRectMidY = (cropTL.Y + cropBR.Y) / 2;

                Rectangle quadUL = new Rectangle(0, 0, cropRectMidX, cropRectMidY);
                quadUL.Intersect(new Rectangle(cropTL.X - Radius, cropTL.Y - Radius, Radius * 2, Radius * 2));
                if (quadUL.Contains(winLoc))
                {
                    return CropCorner.TopLeft;
                }

                Rectangle quadLL = new Rectangle(0, cropRectMidY, cropRectMidX, pictureBoxMain.Height - cropRectMidY);
                quadLL.Intersect(new Rectangle(cropTL.X - Radius, cropBR.Y - Radius, Radius * 2, Radius * 2));
                if (quadLL.Contains(winLoc))
                {
                    return CropCorner.BottomLeft;
                }

                Rectangle quadUR = new Rectangle(cropRectMidX, 0, pictureBoxMain.Width - cropRectMidX, cropRectMidY);
                quadUR.Intersect(new Rectangle(cropBR.X - Radius, cropTL.Y - Radius, Radius * 2, Radius * 2));
                if (quadUR.Contains(winLoc))
                {
                    return CropCorner.TopRight;
                }

                Rectangle quadLR = new Rectangle(cropRectMidX, cropRectMidY, pictureBoxMain.Width - cropRectMidX, pictureBoxMain.Height - cropRectMidY);
                quadLR.Intersect(new Rectangle(cropBR.X - Radius, cropBR.Y - Radius, Radius * 2, Radius * 2));
                if (quadLR.Contains(winLoc))
                {
                    return CropCorner.BottomRight;
                }


                Rectangle quadC = new Rectangle(cropRectMidX - Radius, cropRectMidY - Radius, Radius * 2, Radius * 2);
                if (quadC.Contains(winLoc))
                {
                    return CropCorner.Center;
                }


                Rectangle quadL = new Rectangle(cropTL.X - Radius, cropRectMidY - Radius, Radius * 2, Radius * 2);
                if (quadL.Contains(winLoc))
                {
                    return CropCorner.Left;
                }

                Rectangle quadR = new Rectangle(cropBR.X - Radius, cropRectMidY - Radius, Radius * 2, Radius * 2);
                if (quadR.Contains(winLoc))
                {
                    return CropCorner.Right;
                }

                Rectangle quadT = new Rectangle(cropRectMidX - Radius, cropTL.Y - Radius, Radius * 2, Radius * 2);
                if (quadT.Contains(winLoc))
                {
                    return CropCorner.Top;
                }

                Rectangle quadB = new Rectangle(cropRectMidX - Radius, cropBR.Y - Radius, Radius * 2, Radius * 2);
                if (quadB.Contains(winLoc))
                {
                    return CropCorner.Bottom;
                }
            }

            return CropCorner.None;
        }

        private enum GeoCorner { None, TopLeft, TopRight, BottomRight, BottomLeft, TopI, RightI, BottomI, LeftI, TopR, RightR, BottomR, LeftR };
        private GeoCorner HitTestGeoCorner(Point winLoc)
        {
            Item item = CurrentItem;
            if (item != null)
            {
                const int Radius = 12;

                Point[] corners = new Point[] { item.CornerTL, item.CornerTR, item.CornerBR, item.CornerBL };
                for (int i = 0; i < corners.Length; i++)
                {
                    corners[i] = ImageToWindowPosition(corners[i], pictureBoxMain, item.Width, item.Height, null);
                }

                for (int i = 0; i < corners.Length; i++)
                {
                    if (new Rectangle(corners[i].X - Radius, corners[i].Y - Radius, Radius * 2, Radius * 2).Contains(winLoc))
                    {
                        return (GeoCorner)((int)GeoCorner.TopLeft + i);
                    }

                    Point one = corners[i];
                    Point two = corners[(i + 1) % corners.Length];

                    bool invert;
                    float m, b;
                    Point half, twoThirds;
                    Transforms.ComputeGeoCornerPadPositions(one, two, out m, out b, out invert, out half, out twoThirds);

                    if (new Rectangle(half.X - Radius, half.Y - Radius, Radius * 2, Radius * 2).Contains(winLoc))
                    {
                        return (GeoCorner)((int)GeoCorner.TopI + i);
                    }

                    if (new Rectangle(twoThirds.X - Radius, twoThirds.Y - Radius, Radius * 2, Radius * 2).Contains(winLoc))
                    {
                        return (GeoCorner)((int)GeoCorner.TopR + i);
                    }
                }
            }

            return GeoCorner.None;
        }

        private void PictureBoxMain_MouseMove(object sender, MouseEventArgs e)
        {
            Item item = CurrentItem;
            if ((item != null) && item.Valid)
            {
                if (inDragForCrop)
                {
                    Debug.Assert(toolStripButtonCrop.Checked);
                    UpdateCropOnDrag(e.Location);
                    pictureBoxMain.Update();
                }
                else if (geoDrag != GeoCorner.None)
                {
                    Debug.Assert(toolStripButtonNormalizeGeometryCorners.Checked);
                    UpdateGeoCornersOnDrag(e.Location, false/*final*/);
                    pictureBoxMain.Update();
                }
                else
                {
                    if (item.Status == Status.Pending)
                    {
                        pictureBoxMain.Cursor = Cursors.WaitCursor;
                    }
                    else if (this.toolStripButtonCrop.Checked && (Cursor != Cursors.WaitCursor))
                    {
                        pictureBoxMain.Cursor = CropCorner.None == HitTestCropCorner(e.Location) ? Cursors.Arrow : Cursors.Cross;
                    }
                    if (this.toolStripButtonNormalizeGeometryCorners.Checked && (Cursor != Cursors.WaitCursor))
                    {
                        pictureBoxMain.Cursor = GeoCorner.None == HitTestGeoCorner(e.Location) ? Cursors.Arrow : Cursors.Cross;
                    }
                }

                detailOffset = WindowToImagePosition(e.Location, pictureBoxMain, item.Width, item.Height, pictureBoxDetail1);

                UpdateDetail();
            }
        }

        private void PictureBoxMain_MouseDown(object sender, MouseEventArgs e)
        {
            Item item = CurrentItem;
            if ((item != null) && item.Valid)
            {
                if (this.toolStripButtonNormalizeGeometryCorners.Checked)
                {
                    Point corner = WindowToImagePosition(e.Location, pictureBoxMain, item.Width, item.Height, null);

                    dragPivot = e.Location;
                    geoDrag = HitTestGeoCorner(e.Location);
                    originalCorners = new Point[] { item.CornerTL, item.CornerTR, item.CornerBR, item.CornerBL };

                    UpdatePrimary();
                }
                else if (toolStripButtonCrop.Checked)
                {
                    moveBox = false;
                    CropCorner corner = HitTestCropCorner(e.Location);
                    if (corner == CropCorner.None)
                    {
                        dragPivot = e.Location;
                    }
                    else
                    {
                        Point cropRectTL = ImageToWindowPosition(item.CropRect.Location, pictureBoxMain, item.Width, item.Height, null);
                        Point cropRectBR = ImageToWindowPosition(item.CropRect.Location + item.CropRect.Size, pictureBoxMain, item.Width, item.Height, null);
                        dragFloatOverrideX = dragFloatOverrideY = null;
                        switch (corner)
                        {
                            default:
                                Debug.Assert(false);
                                throw new ArgumentException();
                            case CropCorner.TopLeft:
                                dragPivot = new Point(cropRectBR.X, cropRectBR.Y);
                                break;
                            case CropCorner.BottomLeft:
                                dragPivot = new Point(cropRectBR.X, cropRectTL.Y);
                                break;
                            case CropCorner.TopRight:
                                dragPivot = new Point(cropRectTL.X, cropRectBR.Y);
                                break;
                            case CropCorner.BottomRight:
                                dragPivot = new Point(cropRectTL.X, cropRectTL.Y);
                                break;
                            case CropCorner.Center:
                                dragPivot = e.Location;
                                originalBox = item.CropRect;
                                moveBox = true;
                                break;
                            case CropCorner.Left:
                                dragPivot = new Point(cropRectBR.X, cropRectBR.Y);
                                dragFloatOverrideY = cropRectTL.Y;
                                break;
                            case CropCorner.Right:
                                dragPivot = new Point(cropRectTL.X, cropRectTL.Y);
                                dragFloatOverrideY = cropRectBR.Y;
                                break;
                            case CropCorner.Top:
                                dragPivot = new Point(cropRectBR.X, cropRectBR.Y);
                                dragFloatOverrideX = cropRectTL.X;
                                break;
                            case CropCorner.Bottom:
                                dragPivot = new Point(cropRectTL.X, cropRectTL.Y);
                                dragFloatOverrideX = cropRectBR.X;
                                break;
                        }
                    }
                    inDragForCrop = true;
                }
            }
        }

        private void PictureBoxMain_MouseUp(object sender, MouseEventArgs e)
        {
            bool update = false;

            try
            {
                if ((CurrentItem != null) && CurrentItem.Valid)
                {
                    if (inDragForCrop)
                    {
                        Debug.Assert(toolStripButtonCrop.Checked);
                        UpdateCropOnDrag(e.Location);
                        update = true;
                    }
                    else if (geoDrag != GeoCorner.None)
                    {
                        Debug.Assert(toolStripButtonNormalizeGeometryCorners.Checked);
                        UpdateGeoCornersOnDrag(e.Location, true/*final*/);
                        update = true;
                    }
                }
            }
            finally
            {
                geoDrag = GeoCorner.None;
                inDragForCrop = false;
            }

            if (update)
            {
                UpdatePrimary();
            }
        }

        private void PictureBoxMain_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxMain.Cursor = Cursors.Arrow;
        }

        private static Point WindowImagePositionMapper(Point sourcePos, Control window, int imageWidth, int imageHeight, Control centerOn, bool reverse)
        {
            int insetX = 0, insetY = 0;
            double imageAspectRatio = (double)imageWidth / imageHeight;
            double boxAspectRatio = (double)window.Width / window.Height;
            if (imageAspectRatio > boxAspectRatio)
            {
                insetY = (int)(window.Height * (1 - boxAspectRatio / imageAspectRatio) / 2);
            }
            else
            {
                insetX = (int)(window.Width * (1 - imageAspectRatio / boxAspectRatio) / 2);
            }

            double scaleX = (double)imageWidth / (window.Width - 2 * insetX);
            int offsetX, offsetY;
            if (!reverse)
            {
                offsetX = (int)((sourcePos.X - insetX) * scaleX - (centerOn != null ? centerOn.Width : 0) / 2);
                offsetY = (int)((sourcePos.Y - insetY) * scaleX - (centerOn != null ? centerOn.Height : 0) / 2);
            }
            else
            {
                offsetX = (int)((sourcePos.X + (centerOn != null ? centerOn.Width : 0) / 2) / scaleX + insetX);
                offsetY = (int)((sourcePos.Y + (centerOn != null ? centerOn.Height : 0) / 2) / scaleX + insetY);
            }

            return new Point(offsetX, offsetY);
        }

        private static Point WindowToImagePosition(Point windowPos, Control window, int imageWidth, int imageHeight, Control centerOn)
        {
            return WindowImagePositionMapper(windowPos, window, imageWidth, imageHeight, centerOn, false/*reverse*/);
        }

        private static Point ImageToWindowPosition(Point imagePos, Control window, int imageWidth, int imageHeight, Control centerOn)
        {
            return WindowImagePositionMapper(imagePos, window, imageWidth, imageHeight, centerOn, true/*reverse*/);
        }

        private void UpdateCropOnDrag(Point loc)
        {
            Item item = CurrentItem;

            Point cropA, cropB;
            if (!moveBox)
            {
                if (toolStripButtonCropKeepAspectRatio.Checked && !dragFloatOverrideX.HasValue && !dragFloatOverrideY.HasValue)
                {
                    loc = WindowToImagePosition(loc, pictureBoxMain, item.Width, item.Height, null);
                    PreserveAspectRatio(
                        new Size(item.Width, item.Height),
                        WindowToImagePosition(dragPivot, pictureBoxMain, item.Width, item.Height, null),
                        this.toolStripButtonInvertAspectRatioForCrop.Checked,
                        ref loc);
                    loc = ImageToWindowPosition(loc, pictureBoxMain, item.Width, item.Height, null);
                }
                cropA = dragPivot;
                cropB = loc;
                if (dragFloatOverrideX.HasValue)
                {
                    if (toolStripButtonCropKeepAspectRatio.Checked)
                    {
                        double aspect = GetAspectRatio(
                            new Size(item.Width, item.Height),
                            this.toolStripButtonInvertAspectRatioForCrop.Checked);
                        double midpoint = (cropA.X + dragFloatOverrideX.Value) / 2d;
                        double halfHeight = (cropA.Y - cropB.Y) / 2d;
                        cropA.X = (int)Math.Round(midpoint + halfHeight * aspect);
                        cropB.X = (int)Math.Round(midpoint - halfHeight * aspect);
                    }
                    else
                    {
                        cropB.X = dragFloatOverrideX.Value;
                    }
                }
                if (dragFloatOverrideY.HasValue)
                {
                    if (toolStripButtonCropKeepAspectRatio.Checked)
                    {
                        double aspect = GetAspectRatio(
                            new Size(item.Width, item.Height),
                            this.toolStripButtonInvertAspectRatioForCrop.Checked);
                        double midpoint = (cropA.Y + dragFloatOverrideY.Value) / 2d;
                        double halfWidth = (cropA.X - cropB.X) / 2d;
                        cropA.Y = (int)Math.Round(midpoint + halfWidth / aspect);
                        cropB.Y = (int)Math.Round(midpoint - halfWidth / aspect);
                    }
                    else
                    {
                        cropB.Y = dragFloatOverrideY.Value;
                    }
                }
            }
            else
            {
                // TODO: pin in window
                cropA = ImageToWindowPosition(new Point(originalBox.Left, originalBox.Top), pictureBoxMain, item.Width, item.Height, null)
                    + new Size(loc) - new Size(dragPivot);
                cropB = ImageToWindowPosition(new Point(originalBox.Right, originalBox.Bottom), pictureBoxMain, item.Width, item.Height, null)
                    + new Size(loc) - new Size(dragPivot);
            }
            Rectangle cropRect = ComputeBounds(
               WindowToImagePosition(cropA, pictureBoxMain, item.Width, item.Height, null),
               WindowToImagePosition(cropB, pictureBoxMain, item.Width, item.Height, null));
            item.CropRect = cropRect;

            UpdatePrimary();
        }

        private delegate Point GeoCornerGet();
        private delegate void GeoCornerSet(Point p);
        private void UpdateGeoCornersOnDrag(Point loc, bool final)
        {
            Item item = CurrentItem;

            Tuple<GeoCornerGet, GeoCornerSet>[] corners = new Tuple<GeoCornerGet, GeoCornerSet>[]
            {
                new Tuple<GeoCornerGet, GeoCornerSet>(delegate() { return item.CornerTL; }, delegate(Point p) { item.SetCornerTL(p, final/*setByUser*/); }),
                new Tuple<GeoCornerGet, GeoCornerSet>(delegate() { return item.CornerTR; }, delegate(Point p) { item.SetCornerTR(p, final/*setByUser*/); }),
                new Tuple<GeoCornerGet, GeoCornerSet>(delegate() { return item.CornerBR; }, delegate(Point p) { item.SetCornerBR(p, final/*setByUser*/); }),
                new Tuple<GeoCornerGet, GeoCornerSet>(delegate() { return item.CornerBL; }, delegate(Point p) { item.SetCornerBL(p, final/*setByUser*/); }),
            };

            Point imagePivot = WindowToImagePosition(dragPivot, pictureBoxMain, item.Width, item.Height, null);
            Point imageLoc = WindowToImagePosition(loc, pictureBoxMain, item.Width, item.Height, null);

            int i;
            float a0, b0, c0;
            float a9, b9, c9;
            float a1, b1, c1;
            switch (geoDrag)
            {
                default:
                    Debug.Assert(false);
                    break;

                case GeoCorner.TopLeft:
                case GeoCorner.TopRight:
                case GeoCorner.BottomRight:
                case GeoCorner.BottomLeft:
                    i = (int)geoDrag - (int)GeoCorner.TopLeft;
                    corners[i].Item2(imageLoc);
                    break;

                case GeoCorner.TopI:
                case GeoCorner.RightI:
                case GeoCorner.BottomI:
                case GeoCorner.LeftI:
                    i = (int)geoDrag - (int)GeoCorner.TopI;
                    Point one = originalCorners[i];
                    Point two = originalCorners[(i + 1) & 3];
                    if ((geoDrag == GeoCorner.LeftI) || (geoDrag == GeoCorner.RightI))
                    {
                        int d = imageLoc.X - imagePivot.X;
                        one.X += d;
                        two.X += d;
                    }
                    else
                    {
                        int d = imageLoc.Y - imagePivot.Y;
                        one.Y += d;
                        two.Y += d;
                    }
                    Transforms.LineFromPoints(one, two, out a0, out b0, out c0);
                    Transforms.LineFromPoints(originalCorners[(i - 1) & 3], originalCorners[i], out a9, out b9, out c9);
                    Transforms.LineFromPoints(originalCorners[(i + 1) & 3], originalCorners[(i + 2) & 3], out a1, out b1, out c1);

                    one = Transforms.IntersectH(a9, b9, c9, a0, b0, c0);
                    two = Transforms.IntersectH(a0, b0, c0, a1, b1, c1);
                    corners[i].Item2(one);
                    corners[(i + 1) & 3].Item2(two);
                    break;

                case GeoCorner.TopR:
                case GeoCorner.RightR:
                case GeoCorner.BottomR:
                case GeoCorner.LeftR:
                    i = (int)geoDrag - (int)GeoCorner.TopR;
                    Point center = new Point(
                        (originalCorners[i].X + originalCorners[(i + 1) & 3].X) / 2,
                        (originalCorners[i].Y + originalCorners[(i + 1) & 3].Y) / 2);
                    Transforms.LineFromPoints(center, imageLoc, out a0, out b0, out c0);
                    Transforms.LineFromPoints(originalCorners[(i - 1) & 3], originalCorners[i], out a9, out b9, out c9);
                    Transforms.LineFromPoints(originalCorners[(i + 1) & 3], originalCorners[(i + 2) & 3], out a1, out b1, out c1);

                    one = Transforms.IntersectH(a9, b9, c9, a0, b0, c0);
                    two = Transforms.IntersectH(a0, b0, c0, a1, b1, c1);
                    corners[i].Item2(one);
                    corners[(i + 1) & 3].Item2(two);
                    break;
            }

            UpdatePrimary();
        }

        private static Rectangle ComputeBounds(Point pivot, Point loc)
        {
            int x = Transforms.SnapLossless(pivot.X);
            int y = Transforms.SnapLossless(pivot.Y);
            int xx = Transforms.SnapLossless(loc.X);
            int yy = Transforms.SnapLossless(loc.Y);
            if (x > xx)
            {
                int t = x;
                x = xx;
                xx = t;
            }
            if (y > yy)
            {
                int t = y;
                y = yy;
                yy = t;
            }
            return new Rectangle(x, y, xx - x, yy - y);
        }

        private double GetAspectRatio(Size size, bool invert)
        {
            double ratio;
            if (!aspectRatioOverride.HasValue)
            {
                ratio = !invert ? (float)size.Width / (float)size.Height : (float)size.Height / (float)size.Width;
            }
            else
            {
                ratio = !invert ? aspectRatioOverride.Value : 1f / aspectRatioOverride.Value;
            }
            return ratio;
        }

        private void PreserveAspectRatio(Size size, Point pivot, bool invert, ref Point loc)
        {
            double ratio = GetAspectRatio(size, invert);

            int w = Math.Abs(loc.X - pivot.X);
            int h = Math.Abs(loc.Y - pivot.Y);
            if ((w == 0) || (h == 0))
            {
                return;
            }

            double wr = 1, hr = 1;
            if (w < h * ratio)
            {
                wr = h * ratio / w;
            }
            else
            {
                hr = w / ratio / h;
            }

            Point loc2 = new Point(
                Math.Sign(loc.X - pivot.X) * (int)Math.Round(w * wr) + pivot.X,
                Math.Sign(loc.Y - pivot.Y) * (int)Math.Round(h * hr) + pivot.Y);
            double ox = 1, oy = 1;
            if (loc2.X < 0)
            {
                ox = (double)pivot.X / (double)(pivot.X - loc2.X);
            }
            else if (loc2.X >= size.Width)
            {
                ox = ((double)size.Width - (double)pivot.X) / ((double)loc2.X - (double)pivot.X);
            }
            if (loc2.Y < 0)
            {
                oy = (double)pivot.Y / (double)(pivot.Y - loc2.Y);
            }
            else if (loc2.Y >= size.Height)
            {
                oy = ((double)size.Height - (double)pivot.Y) / ((double)loc2.Y - (double)pivot.Y);
            }
            double o = Math.Min(ox, oy);
            wr *= o;
            hr *= o;
            loc = new Point(
                Math.Sign(loc.X - pivot.X) * (int)Math.Round(w * wr) + pivot.X,
                Math.Sign(loc.Y - pivot.Y) * (int)Math.Round(h * hr) + pivot.Y);
        }
    }

    public interface ISuspendResumeFormatting
    {
        void SuspendDataGridViewReformatting();
        void ResumeDataGridViewReformatting();
    }
}
