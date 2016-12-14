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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;

namespace AdaptiveImageSizeReducer
{
    public class Histogram
    {
        private readonly int over;
        private readonly int numBins;
        private readonly int[] bins;

        public Histogram(int over, int numBins)
        {
            this.over = over;
            this.numBins = numBins;
            this.bins = new int[numBins + 1];
        }

        public Histogram CreateCompatibleHistogram()
        {
            return new Histogram(over, numBins);
        }

        public void Add(int value)
        {
            int i = GetBinIndex(value);
            this.bins[i]++;
        }

        // Threadsafe
        public void Incorporate(Histogram from)
        {
            Debug.Assert((this.over == from.over) && (this.numBins == from.numBins) && (this.bins.Length == from.bins.Length));
            for (int i = 0; i < this.bins.Length; i++)
            {
                Interlocked.Add(ref this.bins[i], from.bins[i]);
            }
        }

        public int[] Bins { get { return (int[])bins.Clone(); } }
        public int Over { get { return over; } }
        public int NumBins { get { return numBins; } }

        public int Total
        {
            get
            {
                int sum = 0;
                for (int i = 0; i <= numBins; i++)
                {
                    sum += bins[i];
                }
                return sum;
            }
        }

        /// <summary>
        /// Get mode of the distribution approximated by the histogram as defined by the index of the
        /// single bin with the highest count.
        /// </summary>
        /// <param name="mode">Out parameter receiving the mode</param>
        /// <param name="modeC">Out parameter receiving the count in the bin of the mode</param>
        /// <returns>true if the mode is unique, otherwise false</returns>
        public bool GetMode(out int mode, out int modeC)
        {
            return GetModeHelper(bins, numBins, out mode, out modeC);
        }

        /// <summary>
        /// Get mode of the distribution approximated by the histogram as defined by the center index
        /// of the region with the most density
        /// </summary>
        /// <param name="radius">The width of the section to compute densities over</param>
        /// <param name="wrap">for sections over end, continue on other end</param>
        /// <param name="mode">Out parameter receiving the mode</param>
        /// <param name="modeC">Out parameter receiving the count in the bin of the mode</param>
        /// <returns>true if the mode is unique, otherwise false</returns>
        public bool GetMode(int radius, bool wrap, out int mode, out int modeC)
        {
            if ((radius & 1) == 0)
            {
                throw new ArgumentException("radius must be odd");
            }

            int[] densities = new int[numBins + 1];
            int d = -radius / 2;
            for (int i = 0; i < numBins; i++) // do not include overs
            {
                int s = 0;
                for (int j = 0; j < radius; j++)
                {
                    int jj = i + j + d;
                    if ((jj >= 0) && (jj < numBins))
                    {
                        s += bins[jj];
                    }
                    else if (wrap)
                    {
                        jj = (jj + numBins) % numBins;
                        s += bins[jj];
                    }
                }
                densities[i] = s;
            }
            return GetModeHelper(densities, numBins, out mode, out modeC);
        }

        private static bool GetModeHelper(int[] bins, int numBins, out int mode, out int modeC)
        {
            bool unique = true;
            int ii = 0;
            for (int i = 1; i <= numBins; i++)
            {
                if (bins[i] >= bins[ii])
                {
                    unique = unique && (bins[i] != bins[ii]);
                    ii = i;
                }
            }
            mode = ii;
            modeC = bins[ii];
            return unique;
        }

        public int SumAroundRadius(int center, int radius)
        {
            int sum = 0;
            for (int k = Math.Max(center - radius, 0); k < Math.Min(center + radius - 1, this.numBins + 1); k++)
            {
                sum += this.bins[k];
            }
            return sum;
        }

        public int Median
        {
            get
            {
                int[] running = new int[numBins + 2];
                int t = 0;
                for (int i = 0; i <= numBins; i++)
                {
                    t += bins[i];
                    running[i] = t;
                }
                running[numBins + 1] = t;

                // TODO: should be weighted position within bin bounds based on past what fraction of the bin the median element occurs.

                //int j = Array.BinarySearch(running, (t - 1) / 2);
                //int k = Array.BinarySearch(running, (t - 1) / 2 + (t - 1) % 2);
                //int v = (bins[j] + bins[k]) / 2;
                int j = Array.BinarySearch(running, (t - 1) / 2);
                int v = bins[Math.Abs(j)];
                return v;
            }
        }

        public void GetBinBounds(int i, out int lb, out int hb) // inclusive
        {
            if (i < 0)
            {
                throw new ArgumentException();
            }
            if (i < numBins)
            {
                lb = ((i + 0) * over + numBins - 1) / numBins;
                hb = ((i + 1) * over + numBins - 1) / numBins - 1;
                Debug.Assert(GetBinIndex(lb) == GetBinIndex(hb));
                Debug.Assert((lb == 0) || (GetBinIndex(lb) == GetBinIndex(lb - 1) + 1));
                Debug.Assert(GetBinIndex(hb) == GetBinIndex(hb + 1) - 1);
            }
            else
            {
                lb = over;
                hb = Int32.MaxValue;
                Debug.Assert(GetBinIndex(lb) == GetBinIndex(hb));
                Debug.Assert(GetBinIndex(lb) == GetBinIndex(lb - 1) + 1);
            }
        }

        // Threadsafe
        public int GetBinIndex(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException();
            }
            return value < over ? value * numBins / over : NumBins;
        }

        public float BinPercentile(int bin)
        {
            int s = 0;
            for (int i = 0; i < bin; i++)
            {
                s += bins[i];
            }
            return (float)s / Total;
        }

        public void Percentile(float p, out int vl, out int vh, out int bin, out float f)
        {
            int[] running = new int[numBins + 2];
            int t = 0;
            for (int i = 0; i <= numBins; i++)
            {
                t += bins[i];
                running[i] = t;
            }
            running[numBins + 1] = t;

            float fv = t * p;
            bin = Array.BinarySearch(running, (int)fv);
            if (bin < 0)
            {
                bin = ~bin;
            }
            GetBinBounds(bin, out vl, out vh);
            f = (float)(fv - (running[bin] - bins[bin])) / bins[bin];
            if (Single.IsNaN(f) || Single.IsInfinity(f))
            {
                f = .5f;
            }
        }

        private struct Field
        {
            public string lowerBound;
            public string upperBound;
            public string range;
            public string count;

            public Field(int lowerBound, int? upperBound, int count)
            {
                this.lowerBound = lowerBound.ToString();
                this.upperBound = upperBound.HasValue ? upperBound.ToString() : String.Empty;
                this.range = upperBound.HasValue ? (upperBound.Value - lowerBound + 1).ToString() : String.Empty;
                this.count = count.ToString();
            }
        }

        public override string ToString()
        {
            Field[] ranges = new Field[numBins + 1];
            int ll = 0, lu = 0, lr = 0, c = 0;
            int m = 0;
            for (int i = 0; i <= numBins; i++)
            {
                int l, u;
                GetBinBounds(i, out l, out u);
                Field r = new Field(l, i < numBins ? (int?)u : null, bins[i]);
                ll = Math.Max(ll, r.lowerBound.Length);
                lu = Math.Max(lu, r.upperBound.Length);
                lr = Math.Max(lr, r.range.Length);
                c = Math.Max(c, r.count.Length);
                ranges[i] = r;

                m = Math.Max(m, bins[i]);
            }
            c = Math.Max(c, Total.ToString().Length);

            const int Width = 80;

            StringBuilder sb = new StringBuilder();
            string format = String.Format("{{0,{0}}} - {{1,{1}}} ({{2,-{2}}}): {{3,-{3}}} ({{4,{4}}})", ll, lu, lr, Width, c);
            for (int i = 0; i <= numBins; i++)
            {
                int w = (int)((long)bins[i] * Width / m);
                sb.AppendLine(String.Format(format, ranges[i].lowerBound, ranges[i].upperBound, ranges[i].range, bins[i] > 0 ? (w > 0 ? new String('*', w) : ".") : null, ranges[i].count));
            }
            string last = String.Format(format, null, null, null, null, Total);
            int j = last.LastIndexOf('(');
            last = new String(' ', j) + last.Substring(j);
            sb.AppendLine(last);

            return sb.ToString();
        }

        public void Render(Graphics graphics, int width, int height, Brush fill, Pen border)
        {
            Render(graphics, width, height, fill, border, null, null);
        }

        public void Render(Graphics graphics, int width, int height, Brush fill, Pen border, float? percentile, Pen line)
        {
            int m = 0;
            for (int i = 0; i <= numBins; i++)
            {
                m = Math.Max(m, bins[i]);
            }

            for (int i = 0; i <= numBins; i++)
            {
                int y = i * height / (numBins + 1);
                int y1 = (i + 1) * height / (numBins + 1);
                int h = y1 - y;
                graphics.DrawLine(border, 0, y + 1, 0, y1 - 1);
                if (bins[i] > 0)
                {
                    int w = (int)((long)bins[i] * (width - 2) / m);
                    if (w > 0)
                    {
                        graphics.FillRectangle(fill, 2, y, w, h);
                        graphics.DrawRectangle(border, 2, y, w, h);
                    }
                    else
                    {
                        graphics.DrawLine(border, 2, y, 2, y1);
                    }
                }
            }

            if (percentile.HasValue)
            {
                int vl, vh, bin;
                float f;
                Percentile(percentile.Value, out vl, out vh, out bin, out f);
                int y = bin * height / (numBins + 1);
                int y1 = (bin + 1) * height / (numBins + 1);
                float yf = f * (y1 - y) + y;
                graphics.DrawLine(line, 2, (int)yf, width, (int)yf);
            }
        }
    }
}
