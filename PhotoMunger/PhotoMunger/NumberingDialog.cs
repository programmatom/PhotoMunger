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
using System.Text;
using System.Windows.Forms;

namespace AdaptiveImageSizeReducer
{
    public partial class NumberingDialog : Form
    {
        private NumberingOptions options;

        public NumberingDialog(NumberingOptions source)
        {
            this.options = source;

            InitializeComponent();

            numberingOptionsBindingSource.Add(this.options);

            this.textBoxFormat.TextChanged += TextBoxFormat_TextChanged;
            this.textBoxFormat.Validated += TextBoxFormat_TextChanged;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public NumberingOptions Options { get { return options; } }

        private void TextBoxFormat_TextChanged(object sender, EventArgs e)
        {
            this.labelExample.Text = NumberingOptions.FormatValue(1, this.textBoxFormat.Text);
        }
    }

    public class NumberingOptions
    {
        private int start = 1;
        private int increment = 1;
        private string format = "Page <G>";
        private int? count;
        private int stride = 1;

        public NumberingOptions()
        {
        }

        public NumberingOptions(NumberingOptions source)
        {
            this.start = source.start;
            this.increment = source.increment;
            this.format = source.format;
            this.count = source.count;
            this.stride = source.stride;
        }

        [Bindable(true)]
        public int Start { get { return start; } set { start = value; } }

        [Bindable(true)]
        public int Increment { get { return increment; } set { increment = value; } }

        [Bindable(true)]
        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                try
                {
                    string s = FormatValue(0, value); // validate format string
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }
                format = value;
            }
        }

        [Bindable(true)]
        public string CountAsString
        {
            get
            {
                return count.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    count = null;
                }
                else
                {
                    try
                    {
                        count = Int32.Parse(value); // throws if invalid
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        [Bindable(false)]
        public int? Count { get { return count; } }

        [Bindable(true)]
        public int Stride
        {
            get
            {
                return stride;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException();
                }
                stride = value;
            }
        }

        public static string FormatValue(int value, string format)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            while (i < format.Length)
            {
                int ii = format.IndexOf('<', i);
                if (ii < 0)
                {
                    sb.Append(format.Substring(i));
                    break;
                }
                int j = format.IndexOf('>', ii);
                if (j < 0)
                {
                    sb.Append(format.Substring(i));
                    break;
                }
                sb.Append(format.Substring(i, ii - i));
                string v = value.ToString(format.Substring(ii + 1, j - (ii + 1)));
                sb.Append(v);
                i = j + 1;
            }

            return sb.ToString();
        }
    }
}
