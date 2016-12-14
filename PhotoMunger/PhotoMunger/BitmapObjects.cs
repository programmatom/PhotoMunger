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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AdaptiveImageSizeReducer
{
    //
    // ManagedBitmap base class
    //

    public unsafe abstract class ManagedBitmap : IDisposable
    {
        public abstract Bitmap CloneToGDI();
        public abstract BitmapFrame CloneToWPF();

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract System.Drawing.Imaging.PixelFormat ImageFormat { get; }
        public abstract int TotalBytes { get; }
        public abstract string BackingName { get; }
        public Size Size { get { return new Size(this.Width, this.Height); } }
        public abstract byte* Scan0 { get; }
        public IntPtr Scan0I { get { return new IntPtr(this.Scan0); } }
        public abstract int Stride { get; }
        public abstract int this[int x, int y] { get; set; }
        public abstract byte* Row(int y);

        public abstract ManagedBitmap Clone();
        public abstract ManagedBitmap NewCompatible(int width, int height);
        protected abstract void Serialize(Stream stream);

        public abstract void Dispose();

        public static ManagedBitmap CreateFromGDI(Bitmap gdiBitmap)
        {
            switch (gdiBitmap.PixelFormat)
            {
                default:
                    Debug.Assert(false);
                    throw new ArgumentException();

                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    return new ManagedBitmap1(gdiBitmap);

                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return new ManagedBitmap32(gdiBitmap);
            }
        }

        public void DrawRect(System.Drawing.Color color, Rectangle rect)
        {
            int rgb = color.ToArgb();
            int height = this.Height;
            int width = this.Width;

            if ((rect.Top >= 0) && (rect.Top < height))
            {
                int s = Math.Max(rect.Left, 0);
                int e = Math.Min(rect.Right, width);
                for (int x = s; x < e; x++)
                {
                    this[x, rect.Top] = rgb;
                }
            }
            if ((rect.Bottom - 1 >= 0) && (rect.Bottom - 1 < height))
            {
                int s = Math.Max(rect.Left, 0);
                int e = Math.Min(rect.Right, width);
                for (int x = s; x < e; x++)
                {
                    this[x, rect.Bottom - 1] = rgb;
                }
            }

            if ((rect.Left >= 0) && (rect.Left < width))
            {
                int s = Math.Max(rect.Top, 0);
                int e = Math.Min(rect.Bottom, height);
                for (int y = s; y < e; y++)
                {
                    this[rect.Left, y] = rgb;
                }
            }
            if ((rect.Right - 1 >= 0) && (rect.Right - 1 < width))
            {
                int s = Math.Max(rect.Top, 0);
                int e = Math.Min(rect.Bottom, height);
                for (int y = s; y < e; y++)
                {
                    this[rect.Right - 1, y] = rgb;
                }
            }
        }

        // Simple general implementation of rotate/flip - override in subclasses to provide optimized versions
        public virtual ManagedBitmap RotateFlip(RotateFlipType action)
        {
            ManagedBitmap copy;
            switch ((int)action)
            {
                default:
                    Debug.Assert(false);
                    throw new ArgumentException();

                case 0: // RotateNoneFlipNone, Rotate180FlipXY
                case 4: // RotateNoneFlipX, Rotate180FlipY
                    copy = this.Clone();
                    break;

                case 1: // Rotate90FlipNone, Rotate270FlipXY
                case 5: // Rotate90FlipX, Rotate270FlipY
                    copy = this.NewCompatible(this.Height, this.Width);
                    for (int y = 0; y < this.Height; y++)
                    {
                        for (int x = 0; x < this.Width; x++)
                        {
                            copy[this.Height - 1 - y, x] = this[x, y];
                        }
                    }
                    break;

                case 2: // Rotate180FlipNone, RotateNoneFlipXY
                case 6: // Rotate180FlipX, RotateNoneFlipY
                    copy = this.NewCompatible(this.Width, this.Height);
                    for (int y = 0; y < this.Height; y++)
                    {
                        for (int x = 0; x < this.Width; x++)
                        {
                            copy[this.Width - 1 - x, this.Height - 1 - y] = this[x, y];
                        }
                    }
                    break;

                case 3: // Rotate270FlipNone, Rotate90FlipXY
                case 7: // Rotate270FlipX, Rotate90FlipY
                    copy = this.NewCompatible(this.Height, this.Width);
                    for (int y = 0; y < this.Height; y++)
                    {
                        for (int x = 0; x < this.Width; x++)
                        {
                            copy[y, this.Width - 1 - x] = this[x, y];
                        }
                    }
                    break;
            }

            if ((int)action >= 4)
            {
                for (int y = 0; y < copy.Height; y++)
                {
                    for (int x = 0; x < copy.Width / 2; x++)
                    {
                        int c = copy[x, y];
                        copy[x, y] = copy[copy.Width - 1 - x, y];
                        copy[copy.Width - 1 - x, y] = c;
                    }
                }
            }

            return copy;
        }

        // Return subsection of bitmap as new bitmap
        public virtual ManagedBitmap Crop(Rectangle rect)
        {
            // Generic implementation - override for optimized versions

            // constrain crop
            rect.Intersect(new Rectangle(Point.Empty, this.Size));

            ManagedBitmap result = this.NewCompatible(rect.Width, rect.Height);

            for (int i = 0; i < rect.Height; i++)
            {
                for (int j = 0; j < rect.Width; j++)
                {
                    result[j, i] = this[j + rect.X, i + rect.Y];
                }
            }

            return result;
        }

        // The Bitmap returned here can outlive the ManagedBitmap used to create it
        public virtual Bitmap GetSection(Rectangle rect)
        {
            // Generic implementation - override for optimized versions

            Bitmap gdiBitmapTarget = new Bitmap(rect.Width, rect.Height);

            BitmapData d = gdiBitmapTarget.LockBits(new Rectangle(0, 0, gdiBitmapTarget.Width, gdiBitmapTarget.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            int stride = d.Stride;
            Debug.Assert(stride > 0);

            int width = this.Width;
            int height = this.Height;
            for (int y = 0; y < gdiBitmapTarget.Height; y++)
            {
                for (int x = 0; x < gdiBitmapTarget.Width; x++)
                {
                    int ey = y + rect.Y;
                    int ex = x + rect.X;
                    if ((ey >= 0) && (ey < height) && (ex >= 0) && (ex < width))
                    {
                        int rgb = this[ex, ey];
                        *(int*)(scan0 + y * stride + x * 4) = rgb;
                    }
                }
            }

            gdiBitmapTarget.UnlockBits(d);

            return gdiBitmapTarget;
        }

        // The Bitmap returned here references the backing store for the ManagedBitmap and therefore cannot outlive it.
        public virtual Bitmap GetSectionEnslaved(Rectangle rect)
        {
            return GetSection(rect);
        }

        public virtual void CopyTo(ManagedBitmap target)
        {
            // Generic implementation - override in subclass to provide optimized version
            Debug.Assert((this.Width == target.Width) && (this.Height == target.Height));

            int width = this.Width;
            int height = this.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    target[x, y] = this[x, y];
                }
            }
        }

        private const long PageSize = 4096;

        public static ManagedBitmap Deserialize(Stream stream)
        {
            stream.Position = 0;

            int type;
            int width, height;
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true/*leaveOpen*/))
            {
                type = reader.ReadInt32();
                width = reader.ReadInt32();
                height = reader.ReadInt32();
            }
            stream.Position = (stream.Position + (PageSize - 1)) & ~(PageSize - 1);

            switch (type)
            {
                default:
                    Debug.Assert(false);
                    throw new ArgumentException();
                case 1:
                    return new ManagedBitmap1(width, height, stream);
                case 32:
                    return new ManagedBitmap32(width, height, stream);
            }
        }

        public static ManagedBitmap Deserialize(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Deserialize(stream);
            }
        }

        public static void Serialize(Stream stream, ManagedBitmap bitmap)
        {
            stream.Position = 0;
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true/*leaveOpen*/))
            {
                if (bitmap is ManagedBitmap32)
                {
                    writer.Write((int)32);
                }
                else if (bitmap is ManagedBitmap1)
                {
                    writer.Write((int)1);
                }
                else
                {
                    throw new ArgumentException();
                }
                writer.Write((int)bitmap.Width);
                writer.Write((int)bitmap.Height);
            }
            int padding = (int)((PageSize - stream.Position) & (PageSize - 1));
            stream.Write(new byte[padding], 0, padding);

            bitmap.Serialize(stream);
            stream.SetLength(stream.Position);
        }

        public static void Serialize(string path, ManagedBitmap bitmap)
        {
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Serialize(stream, bitmap);
            }
        }

        protected static long aggregatedTotalBytes;
        public static long AggregatedTotalBytes
        {
            get
            {
                return Interlocked.Read(ref aggregatedTotalBytes);
            }
        }
    }


    //
    // MemoryMappedBitmap
    //

    public unsafe abstract class MemoryMappedBitmap : ManagedBitmap
    {
        protected readonly int width;
        protected readonly int height;
        protected readonly int stride;
        protected readonly byte* scan0;

        protected bool disposed;
        protected readonly string backingName;
        protected readonly MemoryMappedFile backing;
        protected readonly UnmanagedMemoryAccessor accessor;
        protected readonly SafeBuffer buffer;

        protected MemoryMappedBitmap(int width, int height, int stride, string mapName)
        {
            this.width = width;
            this.height = height;
            this.stride = stride;

            int totalBytes = height * stride;
            Debug.Assert(totalBytes == this.TotalBytes);

            Interlocked.Add(ref aggregatedTotalBytes, totalBytes);

            // TODO: security - restrict access to only our process and child processes

            backingName = mapName;
            backing = MemoryMappedFile.CreateOrOpen(mapName, totalBytes);
            accessor = backing.CreateViewAccessor();
            FieldInfo field = typeof(UnmanagedMemoryAccessor).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
            buffer = (SafeBuffer)field.GetValue(accessor);
            buffer.AcquirePointer(ref scan0);
        }

        protected MemoryMappedBitmap(int width, int height, int stride)
            : this(width, height, stride, Guid.NewGuid().ToString()/*mapName*/)
        {
        }

        public override int TotalBytes
        {
            get
            {
                if (disposed)
                {
                    throw new InvalidOperationException();
                }

                return this.stride * this.height;
            }
        }

        public override int Width
        {
            get
            {
                if (disposed)
                {
                    throw new InvalidOperationException();
                }

                return this.width;
            }
        }

        public override int Height
        {
            get
            {
                if (disposed)
                {
                    throw new InvalidOperationException();
                }

                return this.height;
            }
        }

        public override byte* Scan0
        {
            get
            {
                if (disposed)
                {
                    throw new InvalidOperationException();
                }

                return this.scan0;
            }
        }

        public override int Stride
        {
            get
            {
                if (disposed)
                {
                    throw new InvalidOperationException();
                }

                return this.stride;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte* Row(int y)
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            return this.scan0 + y * this.stride;
        }

        public override string BackingName
        {
            get
            {
                if (disposed)
                {
                    throw new InvalidOperationException();
                }

                return this.backingName;
            }
        }

        protected override void Serialize(Stream stream)
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            using (UnmanagedMemoryStream target = new UnmanagedMemoryStream(this.scan0, TotalBytes))
            {
                target.CopyTo(stream);
            }
        }

        protected void LoadFromStream(Stream stream)
        {
            using (UnmanagedMemoryStream target = new UnmanagedMemoryStream(this.scan0, TotalBytes, TotalBytes, FileAccess.Write))
            {
                stream.CopyTo(target);
            }
        }

        public override void Dispose()
        {
            Debug.Assert(!disposed);

            GC.SuppressFinalize(this);

            Interlocked.Add(ref aggregatedTotalBytes, -TotalBytes);

            disposed = true;

            buffer.ReleasePointer();
            accessor.Dispose();
            backing.Dispose();
        }

        ~MemoryMappedBitmap()
        {
            string message = String.Format("{0}: Did you forget to Dispose()? {1}", this.GetType().Name, allocatedFrom.ToString());
            Debugger.Log(0, null, message + Environment.NewLine);
#if DEBUG
            Debug.Assert(false, message);
#else
#if true // TODO:remove
            System.Windows.Forms.MessageBox.Show(message);
#endif
#endif
            Dispose();
        }
#if DEBUG || true//TODO:remove
        private readonly StackTrace allocatedFrom = new StackTrace(1, true/*fNeedFileInfo*/);
#endif
    }


    //
    // ManagedBitmap32
    //

    public unsafe class ManagedBitmap32 : MemoryMappedBitmap
    {
        public const System.Drawing.Imaging.PixelFormat Format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;

        public ManagedBitmap32(int width, int height, string mapName)
            : base(width, height, width * 4, mapName)
        {
        }

        public ManagedBitmap32(int width, int height)
            : this(width, height, Guid.NewGuid().ToString()/*mapName*/)
        {
        }

        public ManagedBitmap32(Bitmap gdiBitmap)
            : this(gdiBitmap.Width, gdiBitmap.Height)
        {
            BitmapData d = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly, Format);
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            int stride = d.Stride;
            Debug.Assert(stride > 0);

            for (int y = 0; y < this.height; y++)
            {
                Buffer.MemoryCopy(scan0 + y * stride, this.scan0 + y * this.stride, this.width * 4, this.width * 4);
            }

            gdiBitmap.UnlockBits(d);
        }

        public ManagedBitmap32(BitmapFrame wpfBitmap, string mapName)
            : this(wpfBitmap.PixelWidth, wpfBitmap.PixelHeight, mapName)
        {
            wpfBitmap.CopyPixels(
                new System.Windows.Int32Rect(0, 0, wpfBitmap.PixelWidth, wpfBitmap.PixelHeight),
                new IntPtr(this.scan0),
                TotalBytes,
                this.stride);
        }

        public ManagedBitmap32(BitmapFrame wpfBitmap)
            : this(wpfBitmap.PixelWidth, wpfBitmap.PixelHeight)
        {
            wpfBitmap.CopyPixels(
                new System.Windows.Int32Rect(0, 0, wpfBitmap.PixelWidth, wpfBitmap.PixelHeight),
                new IntPtr(this.scan0),
                TotalBytes,
                this.stride);
        }

        public ManagedBitmap32(int width, int height, Stream stream)
            : this(width, height)
        {
            LoadFromStream(stream);
        }

        public override Bitmap CloneToGDI()
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            Bitmap gdiBitmap = new Bitmap(this.width, this.height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            BitmapData d = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.WriteOnly, Format);
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            int stride = d.Stride;
            Debug.Assert(stride > 0);

            for (int y = 0; y < this.height; y++)
            {
                Buffer.MemoryCopy(this.scan0 + y * this.stride, scan0 + y * stride, this.width * 4, this.width * 4);
            }

            gdiBitmap.UnlockBits(d);

            return gdiBitmap;
        }

        public override BitmapFrame CloneToWPF()
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            return BitmapFrame.Create(BitmapSource.Create(this.width, this.height, 96, 96, PixelFormats.Bgr32, null, new IntPtr(this.scan0), this.width * this.height * 4, this.stride));
        }

        public override System.Drawing.Imaging.PixelFormat ImageFormat
        {
            get
            {
                return Format;
            }
        }

        public override int this[int x, int y]
        {
            get
            {
                return *(int*)(this.scan0 + x * 4 + y * this.stride);
            }
            set
            {
                *(int*)(this.scan0 + x * 4 + y * this.stride) = value;
            }
        }

        // The Bitmap returned here can outlive the ManagedBitmap used to create it
        public override Bitmap GetSection(Rectangle rect)
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            Bitmap gdiBitmapTarget = new Bitmap(rect.Width, rect.Height);

            BitmapData d = gdiBitmapTarget.LockBits(new Rectangle(0, 0, gdiBitmapTarget.Width, gdiBitmapTarget.Height), ImageLockMode.WriteOnly, Format);
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            int stride = d.Stride;
            Debug.Assert(stride > 0);

            // should optimize this if it comes to matter
            for (int y = 0; y < gdiBitmapTarget.Height; y++)
            {
                for (int x = 0; x < gdiBitmapTarget.Width; x++)
                {
                    int ey = y + rect.Y;
                    int ex = x + rect.X;
                    int* pTarget = (int*)(scan0 + y * stride + x * 4);
                    *pTarget = 0;
                    if ((ey >= 0) && (ey < this.height) && (ex >= 0) && (ex < this.width))
                    {
                        *pTarget = *(int*)(this.scan0 + ey * this.stride + ex * 4);
                    }
                }
            }

            gdiBitmapTarget.UnlockBits(d);

            return gdiBitmapTarget;
        }

        // The Bitmap returned here references the backing store for the ManagedBitmap and therefore cannot outlive it.
        public override Bitmap GetSectionEnslaved(Rectangle rect)
        {
            Rectangle rect2 = rect;
            rect2.Intersect(new Rectangle(Point.Empty, this.Size));
            if (rect2 == rect)
            {
                return new Bitmap(rect.Width, rect.Height, this.stride, Format, new IntPtr(Row(rect.Top) + 4 * rect.Left));
            }
            else
            {
                // if requested rect includes area off edge of bitmap, it must be copied using the slow method
                return GetSection(rect);
            }
        }

        public override ManagedBitmap Clone()
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            ManagedBitmap32 copy = new ManagedBitmap32(this.width, this.height);
            Debug.Assert(this.stride == copy.stride);
            Buffer.MemoryCopy(this.scan0, copy.scan0, TotalBytes, TotalBytes);
            return copy;
        }

        public override ManagedBitmap NewCompatible(int width, int height)
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            return new ManagedBitmap32(width, height);
        }

        public unsafe override ManagedBitmap RotateFlip(RotateFlipType action)
        {
            ManagedBitmap32 copy;
            switch ((int)action)
            {
                default:
                    Debug.Assert(false);
                    throw new ArgumentException();

                case 0: // RotateNoneFlipNone, Rotate180FlipXY
                case 4: // RotateNoneFlipX, Rotate180FlipY
                    copy = (ManagedBitmap32)this.Clone();
                    break;

                case 1: // Rotate90FlipNone, Rotate270FlipXY
                case 5: // Rotate90FlipX, Rotate270FlipY
                    copy = new ManagedBitmap32(this.Height, this.Width);
                    for (int y = 0; y < this.Height; y++)
                    {
                        int* pThis = (int*)(this.scan0 + y * this.stride);
                        int* pCopy = (int*)(copy.Scan0 + (this.Height - 1 - y) * 4);
                        int copyStride = copy.stride / 4;
                        for (int x = 0; x < this.Width; x++)
                        {
                            *pCopy = *pThis;
                            pCopy += copyStride;
                            pThis++;
                        }
                    }
                    break;

                case 2: // Rotate180FlipNone, RotateNoneFlipXY
                case 6: // Rotate180FlipX, RotateNoneFlipY
                    copy = new ManagedBitmap32(this.Width, this.Height);
                    fixed (int* pBuf = &((new int[this.width])[0]))
                    {
                        for (int y = 0; y < this.Height; y++)
                        {
                            int* pThis = (int*)(this.scan0 + y * this.stride);
                            int* pCopy = pBuf + this.width;
                            for (int x = 0; x < this.Width; x++)
                            {
                                pCopy--;
                                *pCopy = *pThis;
                                pThis++;
                            }
                            Buffer.MemoryCopy(pBuf, copy.Scan0 + (this.height - 1 - y) * copy.stride, copy.stride, copy.stride);
                        }
                    }
                    break;

                case 3: // Rotate270FlipNone, Rotate90FlipXY
                case 7: // Rotate270FlipX, Rotate90FlipY
                    copy = new ManagedBitmap32(this.Height, this.Width);
                    for (int y = 0; y < this.Height; y++)
                    {
                        int* pThis = (int*)(this.scan0 + y * this.stride);
                        int* pCopy = (int*)(copy.Scan0 + y * 4 + this.width * copy.stride);
                        int copyStride = copy.stride / 4;
                        for (int x = 0; x < this.Width; x++)
                        {
                            pCopy -= copyStride;
                            *pCopy = *pThis;
                            pThis++;
                        }
                    }
                    break;
            }

            // This could be optimized, but not for now since it is generally unused
            if ((int)action >= 4)
            {
                for (int y = 0; y < copy.Height; y++)
                {
                    for (int x = 0; x < copy.Width / 2; x++)
                    {
                        int c = copy[x, y];
                        copy[x, y] = copy[copy.Width - 1 - x, y];
                        copy[copy.Width - 1 - x, y] = c;
                    }
                }
            }

            return copy;
        }

    }


    //
    // ManagedBitmap1
    //

    public unsafe class ManagedBitmap1 : MemoryMappedBitmap
    {
        public const System.Drawing.Imaging.PixelFormat Format = System.Drawing.Imaging.PixelFormat.Format1bppIndexed;

        public ManagedBitmap1(int width, int height, string mapName)
            : base(width, height, (width + 7) / 8, mapName)
        {
        }

        public ManagedBitmap1(int width, int height)
            : this(width, height, Guid.NewGuid().ToString()/*mapName*/)
        {
        }

        public ManagedBitmap1(Bitmap gdiBitmap)
            : this(gdiBitmap.Width, gdiBitmap.Height)
        {
            BitmapData d = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly, Format);
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            int stride = d.Stride;
            Debug.Assert(stride > 0);

            for (int y = 0; y < this.height; y++)
            {
                Buffer.MemoryCopy(scan0 + y * stride, this.scan0 + y * this.stride, (this.width + 7) / 8, (this.width + 7) / 8);
            }

            gdiBitmap.UnlockBits(d);
        }

        public ManagedBitmap1(int width, int height, Stream stream)
            : this(width, height)
        {
            LoadFromStream(stream);
        }

        public override Bitmap CloneToGDI()
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            Bitmap gdiBitmap = new Bitmap(this.width, this.height, Format);

            BitmapData d = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.WriteOnly, Format);
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            int stride = d.Stride;
            Debug.Assert(stride > 0);

            for (int y = 0; y < this.height; y++)
            {
                Buffer.MemoryCopy(this.scan0 + y * this.stride, scan0 + y * stride, (this.width + 7) / 8, (this.width + 7) / 8);
            }

            gdiBitmap.UnlockBits(d);

            return gdiBitmap;
        }

        public override BitmapFrame CloneToWPF()
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            throw new NotSupportedException();
        }

        public override System.Drawing.Imaging.PixelFormat ImageFormat { get { return Format; } }

        public override int this[int x, int y]
        {
            get
            {
                byte* pp = this.scan0 + y * this.stride + (x >> 3);
                byte b = *pp;
                byte mask = (byte)(0x80 >> (x & 0x7));
                return (b & mask) != 0 ? 0x00FFFFFF : 0;
            }
            set
            {
                byte* pp = this.scan0 + y * this.stride + (x >> 3);
                byte b = *pp;
                byte mask = (byte)(0x80 >> (x & 0x7));
                if (value != 0)
                {
                    b |= mask;
                }
                else
                {
                    b &= (byte)~mask;
                }
                *pp = b;
            }
        }

        public override ManagedBitmap Clone()
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            ManagedBitmap1 copy = new ManagedBitmap1(this.width, this.height);
            Debug.Assert(this.stride == copy.stride);
            Buffer.MemoryCopy(this.scan0, copy.scan0, TotalBytes, TotalBytes);
            return copy;
        }

        public override ManagedBitmap NewCompatible(int width, int height)
        {
            if (disposed)
            {
                throw new InvalidOperationException();
            }

            return new ManagedBitmap1(width, height);
        }
    }


    //
    // SmartBitmap
    //

    public unsafe class SmartBitmap : IDisposable
    {
        private Bitmap gdiBitmap;
        private BitmapFrame wpfBitmap;
        private ManagedBitmap managedBitmap;
        private readonly PropertyItem[] properties = new PropertyItem[0];

        public SmartBitmap(Bitmap gdiBitmap)
        {
            this.gdiBitmap = gdiBitmap;

            try
            {
                this.properties = gdiBitmap.PropertyItems;
            }
            catch (ArgumentException)
            {
                return; // properties not supported
            }
        }

        public SmartBitmap(BitmapFrame wpfBitmap)
        {
            this.wpfBitmap = wpfBitmap;
        }

        public SmartBitmap(ManagedBitmap managedBitmap)
        {
            this.managedBitmap = managedBitmap;
        }

        public int Width
        {
            get
            {
                if (gdiBitmap != null)
                {
                    return gdiBitmap.Width;
                }
                else if (wpfBitmap != null)
                {
                    return wpfBitmap.PixelWidth;
                }
                else if (managedBitmap != null)
                {
                    return managedBitmap.Width;
                }
                Debug.Assert(false);
                throw new InvalidOperationException();
            }
        }

        public int Height
        {
            get
            {
                if (gdiBitmap != null)
                {
                    return gdiBitmap.Height;
                }
                else if (wpfBitmap != null)
                {
                    return wpfBitmap.PixelHeight;
                }
                else if (managedBitmap != null)
                {
                    return managedBitmap.Height;
                }
                Debug.Assert(false);
                throw new InvalidOperationException();
            }
        }

        public System.Drawing.Imaging.PixelFormat ImageFormat
        {
            get
            {
                if (gdiBitmap != null)
                {
                    return gdiBitmap.PixelFormat;
                }
                else if (wpfBitmap != null)
                {
                    if (wpfBitmap.Format.Equals(PixelFormats.Bgr32))
                    {
                        return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
                    }
                    const string Message = "Unsupported WPF image format";
                    Debug.Assert(false, Message);
                    throw new NotSupportedException(Message);
                }
                else if (managedBitmap != null)
                {
                    return managedBitmap.ImageFormat;
                }
                Debug.Assert(false);
                throw new InvalidOperationException();
            }
        }

        public bool IsGDI { get { return this.gdiBitmap != null; } }

        public bool IsWPF { get { return this.wpfBitmap != null; } }

        public bool IsManaged { get { return this.managedBitmap != null; } }

        public Bitmap AsGDI(Profile profile = null)
        {
            bool pop = false;

            try
            {
                if (gdiBitmap != null)
                {
                    return gdiBitmap;
                }

                if (profile != null)
                {
                    profile.Push("SmartBitmap.AsGDI");
                    pop = true;
                }

                if (wpfBitmap != null)
                {
                    Bitmap gdiBitmap = new Bitmap(wpfBitmap.PixelWidth, wpfBitmap.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    ApplyProperties();

                    BitmapData d = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.WriteOnly, ManagedBitmap32.Format);
                    int* scan0 = (int*)d.Scan0.ToPointer();
                    int stride = d.Stride;
                    Debug.Assert(stride > 0);

                    wpfBitmap.CopyPixels(new System.Windows.Int32Rect(0, 0, gdiBitmap.Width, gdiBitmap.Height), new IntPtr(scan0), stride, 0/*offset*/);

                    gdiBitmap.UnlockBits(d);

                    wpfBitmap = null;

                    return gdiBitmap;
                }

                if (managedBitmap != null)
                {
                    gdiBitmap = managedBitmap.CloneToGDI();

                    managedBitmap.Dispose();
                    managedBitmap = null;

                    return gdiBitmap;
                }
            }
            finally
            {
                if ((profile != null) && pop)
                {
                    profile.Pop();
                }
            }

            Debug.Assert(false);
            throw new InvalidOperationException();
        }

        public BitmapFrame AsWPF(Profile profile = null)
        {
            bool pop = false;

            try
            {
                if (wpfBitmap != null)
                {
                    return wpfBitmap;
                }

                if (profile != null)
                {
                    profile.Push("SmartBitmap.AsWPF");
                    pop = true;
                }

                if (gdiBitmap != null)
                {
                    BitmapData d = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly, ManagedBitmap32.Format);
                    int* scan0 = (int*)d.Scan0.ToPointer();
                    int stride = d.Stride;
                    Debug.Assert(stride > 0);

                    wpfBitmap = (BitmapFrame)BitmapFrame.Create(gdiBitmap.Width, gdiBitmap.Height, 96, 96, PixelFormats.Bgr32, null, new IntPtr(scan0), gdiBitmap.Width * gdiBitmap.Height * 4, stride);

                    gdiBitmap.UnlockBits(d);

                    gdiBitmap.Dispose();
                    gdiBitmap = null;

                    return wpfBitmap;
                }

                if (managedBitmap != null)
                {
                    wpfBitmap = managedBitmap.CloneToWPF();

                    managedBitmap.Dispose();
                    managedBitmap = null;

                    return wpfBitmap;
                }
            }
            finally
            {
                if ((profile != null) && pop)
                {
                    profile.Pop();
                }
            }

            Debug.Assert(false);
            throw new InvalidOperationException();
        }

        public ManagedBitmap AsManaged(Profile profile = null)
        {
            bool pop = false;
            try
            {
                if (managedBitmap != null)
                {
                    return managedBitmap;
                }

                if (profile != null)
                {
                    profile.Push("SmartBitmap.AsManaged");
                    pop = true;
                }

                if (gdiBitmap != null)
                {
                    managedBitmap = ManagedBitmap.CreateFromGDI(gdiBitmap);

                    gdiBitmap.Dispose();
                    gdiBitmap = null;

                    return managedBitmap;
                }

                if (wpfBitmap != null)
                {
                    managedBitmap = new ManagedBitmap32(wpfBitmap);

                    wpfBitmap = null;

                    return managedBitmap;
                }
            }
            finally
            {
                if ((profile != null) && pop)
                {
                    profile.Pop();
                }
            }

            Debug.Assert(false);
            throw new InvalidOperationException();
        }

        public Bitmap AsGDIDetach(Profile profile = null)
        {
            Bitmap bitmap = AsGDI(profile);
            Debug.Assert(gdiBitmap != null);
            gdiBitmap = null;
            GC.SuppressFinalize(this);
            return bitmap;
        }

        public BitmapFrame AsWPFDetach()
        {
            BitmapFrame bitmap = AsWPF();
            Debug.Assert(wpfBitmap != null);
            wpfBitmap = null;
            GC.SuppressFinalize(this);
            return bitmap;
        }

        public ManagedBitmap AsManagedDetach(Profile profile = null)
        {
            ManagedBitmap bitmap = AsManaged(profile);
            Debug.Assert(managedBitmap != null);
            managedBitmap = null;
            GC.SuppressFinalize(this);
            return bitmap;
        }

        public void AbsorbGDI(Bitmap gdiBitmap)
        {
            Clear();
            this.gdiBitmap = gdiBitmap;
        }

        public void AbsorbWPF(BitmapFrame wpfBitmap)
        {
            Clear();
            this.wpfBitmap = wpfBitmap;
        }

        public void AbsorbManaged(ManagedBitmap managedBitmap)
        {
            Clear();
            this.managedBitmap = managedBitmap;
        }

        private void ApplyProperties()
        {
            Debug.Assert(gdiBitmap != null);
            foreach (PropertyItem property in properties)
            {
                gdiBitmap.SetPropertyItem(property);
            }
        }

        public void DrawRects(System.Drawing.Color color, IEnumerable<Rectangle> rects)
        {
            if (gdiBitmap != null)
            {
                using (Graphics graphics = Graphics.FromImage(gdiBitmap))
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(color))
                    {
                        foreach (Rectangle rect in rects)
                        {
                            graphics.DrawRectangle(pen, rect);
                        }
                    }
                }
                return;
            }

            if (wpfBitmap != null)
            {
                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();
                dc.DrawImage(wpfBitmap, new System.Windows.Rect(0, 0, wpfBitmap.PixelWidth, wpfBitmap.PixelHeight));
                int rgb = color.ToArgb();
                byte r = (byte)((rgb >> 16) & 0xFF);
                byte g = (byte)((rgb >> 8) & 0xFF);
                byte b = (byte)((rgb >> 0) & 0xFF);
                System.Windows.Media.Brush brush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(r, g, b));
                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(brush, 1);
                foreach (Rectangle rect in rects)
                {
                    dc.DrawRectangle(brush, pen, new System.Windows.Rect(rect.X, rect.Y, rect.Width, rect.Height));
                }
                dc.Close();
                RenderTargetBitmap bm = new RenderTargetBitmap(wpfBitmap.PixelWidth, wpfBitmap.PixelHeight, 96, 96, PixelFormats.Bgr32);
                bm.Render(dv);
                wpfBitmap = BitmapFrame.Create(bm);
                return;
            }

            if (managedBitmap != null)
            {
                foreach (Rectangle rect in rects)
                {
                    managedBitmap.DrawRect(color, rect);
                }
                return;
            }

            Debug.Assert(false);
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            this.wpfBitmap = null;
            if (this.gdiBitmap != null)
            {
                this.gdiBitmap.Dispose();
                this.gdiBitmap = null;
            }
            if (this.managedBitmap != null)
            {
                this.managedBitmap.Dispose();
                this.managedBitmap = null;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Clear();
        }

        ~SmartBitmap()
        {
            string message = String.Concat("SmartBitmap: Did you forget to Dispose()? ", allocatedFrom.ToString());
            Debugger.Log(0, null, message + Environment.NewLine);
#if DEBUG
            Debug.Assert(false, message);
#else
#if true // TODO:remove
            System.Windows.Forms.MessageBox.Show(message);
#endif
#endif
            Dispose();
        }
#if DEBUG || true//TODO:remove
        private readonly StackTrace allocatedFrom = new StackTrace(1, true/*fNeedFileInfo*/);
#endif
    }
}
