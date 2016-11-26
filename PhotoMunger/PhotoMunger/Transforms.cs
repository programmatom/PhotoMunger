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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;

namespace AdaptiveImageSizeReducer
{
    public static class Transforms
    {
        public const int InternalJpegQuality = 95;

        private const int OrientationPropertyId = 0x112;

        [Flags]
        public enum Channel { Invalid = 0, R = 1 << 0, G = 1 << 1, B = 1 << 2, Composite = R | G | B };

        private const bool EnableVectors = true;
#if DEBUG
        private const bool VectorHardwareAcceleratedOverride = true;
#else
        private const bool VectorHardwareAcceleratedOverride = false;
#endif

        public delegate void AddMessageMethod(string text);

        public static MemoryStream ReadAllBytes(string path)
        {
            using (Stream source = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[(int)source.Length];
                int o = 0;
                int c;
                while ((c = source.Read(buffer, o, buffer.Length - o)) != 0)
                {
                    o += c;
                }
                Debug.Assert(o == buffer.Length);
                return new MemoryStream(buffer);
            }
        }

        public static void CopyProperties(Image original, Image copy)
        {
            PropertyItem[] propertyItems;
            try
            {
                propertyItems = original.PropertyItems;
            }
            catch (ArgumentException)
            {
                return; // properties not supported
            }
            foreach (PropertyItem propertyItem in propertyItems)
            {
                copy.SetPropertyItem(propertyItem);
            }
        }

        // Check that the file is one of the supported image formats for GDI+. This is just a sanity check and
        // may produce false positives, in which case the file will be rejected when an attempt is made to load
        // it. The purpose is primarily to avoid trying to load huge MP4/AVI video files which tend to blow up
        // by causing GDI+ to use huge amounts of memory.
        public static bool SanityCheckValidImageFormatFile(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] b = new byte[4];
                if (b.Length != stream.Read(b, 0, b.Length)) // too short!
                {
                    return false;
                }

                // try to recognize any of the formats supported by GDI+
                if ((b[0] == 0xFF) && (b[1] == 0xD8)) // JPEG
                {
                    return true;
                }
                if ((b[0] == 0x42) && (b[1] == 0x4D)) // BMP
                {
                    return true;
                }
                if ((b[0] == 0x49) && (b[1] == 0x49)) // TIFF - little-endian
                {
                    return true;
                }
                if ((b[0] == 0x4D) && (b[1] == 0x4D)) // TIFF - big-endian
                {
                    return true;
                }
                if ((b[1] == 0x50) && (b[2] == 0x4E) && (b[3] == 0x47)) // PNG
                {
                    return true;
                }
                if ((b[0] == 0x47) && (b[1] == 0x49) && (b[2] == 0x46)) // GIF
                {
                    return true;
                }
            }
            return false;
        }

        public class NotJpegFormatFileException : Exception
        {
            public NotJpegFormatFileException()
                : base()
            {
            }

            public NotJpegFormatFileException(string message)
                : base(message)
            {
            }
        }

        public static void SanityCheckValidImageFormatFileThrow(string path)
        {
            if (!SanityCheckValidImageFormatFile(path))
            {
                throw new NotJpegFormatFileException(String.Format("File header is not a Jpeg/Exif header ({0})", Path.GetFileName(path)));
            }
        }

        public struct ColorRGB
        {
            public float R;
            public float G;
            public float B;

            public ColorRGB(int rgb32)
            {
                int r = (rgb32 >> 16) & 0xFF;
                R = (float)r / 255;
                int g = (rgb32 >> 8) & 0xFF;
                G = (float)g / 255;
                int b = (rgb32 >> 0) & 0xFF;
                B = (float)b / 255;
            }

            public ColorRGB(float r, float g, float b)
            {
                this.R = r;
                this.G = g;
                this.B = b;
            }

            public int ToRGB32()
            {
                int r = Math.Min(Math.Max((int)(R * 255 + .5f), 0), 255);
                int g = Math.Min(Math.Max((int)(G * 255 + .5f), 0), 255);
                int b = Math.Min(Math.Max((int)(B * 255 + .5f), 0), 255);
                return (r << 16) | (g << 8) | (b << 0);
            }

            public static ColorRGB operator *(float l, ColorRGB r)
            {
                return new ColorRGB(l * r.R, l * r.G, l * r.B);
            }

            public static ColorRGB operator +(ColorRGB l, ColorRGB r)
            {
                return new ColorRGB(l.R + r.R, l.G + r.G, l.B + r.B);
            }

            public static ColorRGB operator -(ColorRGB l, ColorRGB r)
            {
                return new ColorRGB(l.R - r.R, l.G - r.G, l.B - r.B);
            }

            public static ColorRGB operator -(ColorRGB x)
            {
                return new ColorRGB(-x.R, -x.G, -x.B);
            }

            public override string ToString()
            {
                return String.Format("({0:N2}, {1:N2}, {2:N2})", R, G, B);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float RgbMaxF(int rgb32)
            {
                int r = (rgb32 >> 16) & 0xFF;
                int g = (rgb32 >> 8) & 0xFF;
                int b = (rgb32 >> 0) & 0xFF;
                int vi = Math.Max(Math.Max(r, g), b);
                float v = vi / 255f;
                return v;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int RgbMaxI(int rgb32)
            {
                int r = (rgb32 >> 16) & 0xFF;
                int g = (rgb32 >> 8) & 0xFF;
                int b = (rgb32 >> 0) & 0xFF;
                int vi = Math.Max(Math.Max(r, g), b);
                return vi;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int RgbScale(int rgb32, float scale)
            {
                int r = (rgb32 >> 16) & 0xFF;
                int g = (rgb32 >> 8) & 0xFF;
                int b = (rgb32 >> 0) & 0xFF;
                r = Math.Min(Math.Max((int)(r * scale + .5f), 0), 255);
                g = Math.Min(Math.Max((int)(g * scale + .5f), 0), 255);
                b = Math.Min(Math.Max((int)(b * scale + .5f), 0), 255);
                return (r << 16) | (g << 8) | (b << 0);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int GetR(int rgb32)
            {
                return (rgb32 >> 16) & 0xFF;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int GetG(int rgb32)
            {
                return (rgb32 >> 8) & 0xFF;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int GetB(int rgb32)
            {
                return (rgb32 >> 0) & 0xFF;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GetRGB(int rgb32, out int r, out int g, out int b)
            {
                r = (rgb32 >> 16) & 0xFF;
                g = (rgb32 >> 8) & 0xFF;
                b = (rgb32 >> 0) & 0xFF;
            }

            public static int MakeRGBMask(bool r, bool g, bool b)
            {
                return MakeRGB(r ? 255 : 0, b ? 255 : 0, b ? 255 : 0);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int MakeRGB(int r, int g, int b)
            {
                return (r << 16) | (g << 8) | (b << 0);
            }

            public static Vector3 VectorFromRGB(int rgb32)
            {
                return new Vector3(/*R*/(rgb32 >> 16) & 0xFF, /*G*/(rgb32 >> 8) & 0xFF, /*B*/(rgb32 >> 0) & 0xFF);
            }

            public static int RGBFromVector(Vector3 v)
            {
                int r = Math.Min(Math.Max((int)(v.X + .5f), 0), 255);
                int g = Math.Min(Math.Max((int)(v.Y + .5f), 0), 255);
                int b = Math.Min(Math.Max((int)(v.Z + .5f), 0), 255);
                return (r << 16) | (g << 8) | (b << 0);
            }
        }

        // HSB (and other) color conversion from:
        // http://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part
        public struct ColorHSV
        {
            public float H;
            public float S;
            public float V;

            public ColorHSV(float h, float s, float v)
            {
                this.H = h;
                this.S = s;
                this.V = v;
            }

            public ColorHSV(ColorRGB rgb)
            {
                float max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
                float min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));

                H = 0;
                if (max == rgb.R && rgb.G >= rgb.B)
                {
                    H = 60 * (rgb.G - rgb.B) / (max - min);
                }
                else if (max == rgb.R && rgb.G < rgb.B)
                {
                    H = 60 * (rgb.G - rgb.B) / (max - min) + 360;
                }
                else if (max == rgb.G)
                {
                    H = 60 * (rgb.B - rgb.R) / (max - min) + 120;
                }
                else if (max == rgb.B)
                {
                    H = 60 * (rgb.R - rgb.G) / (max - min) + 240;
                }

                S = (max == 0) ? 0 : (1 - (min / max));
                V = max;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GetSV(int rgb32, out float s, out int v)
            {
                int r = (rgb32 >> 16) & 0xFF;
                int g = (rgb32 >> 8) & 0xFF;
                int b = (rgb32 >> 0) & 0xFF;

                int max = Math.Max(r, Math.Max(g, b));
                float min = Math.Min(r, Math.Min(g, b));

                s = (max == 0) ? 0 : (1 - (min / max));
                v = max;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GetSV(int rgb32, out float s, out float v)
            {
                int vi;
                GetSV(rgb32, out s, out vi);
                v = vi;
            }

#if false // TODO: remove - slower
            private readonly static Vector<uint> AlphaAndMask = new Vector<uint>(0x00FFFFFF);
            private readonly static Vector<uint> AlphaOrMask = new Vector<uint>(0xFF000000);
            private readonly static Vector<uint> FloatHackBase = new Vector<uint>(0x4B000000);
            private readonly static Vector<uint> LowByteMask = new Vector<uint>(0x000000FF);

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe static void GetSV(int* rgb32, out Vector<float> s, out Vector<float> v, byte* hackWorkspace)
            {
                Vector<byte> p4;
                Vector<byte> p4a = Unsafe.Read<Vector<byte>>(rgb32);

                p4 = p4a;
                p4 = Vector.BitwiseAnd(p4, Vector.AsVectorByte(AlphaAndMask));
                // HACK: neither multiply nor divide are implemented as instructions but as calls, and no shift is provided
                //p4 = Vector.Max(p4, Vector.AsVectorByte(Vector.Divide(Vector.AsVectorUInt32(p4), new Vector<uint>(0x100))));
                //p4 = Vector.Max(p4, Vector.AsVectorByte(Vector.Divide(Vector.AsVectorUInt32(p4), new Vector<uint>(0x10000))));
                Unsafe.Write(hackWorkspace, p4);
                p4 = Vector.Max(Vector.Max(p4, Unsafe.Read<Vector<byte>>(hackWorkspace + 1)), Unsafe.Read<Vector<byte>>(hackWorkspace + 2));
                p4 = p4 & Vector.AsVectorByte(LowByteMask);
                // HACK to convert byte to float (need https://github.com/dotnet/corefx/issues/1605)
                Vector<float> max = Vector.AsVectorSingle(Vector.Subtract(Vector.AsVectorSingle(Vector.Add(Vector.AsVectorUInt32(p4), FloatHackBase)), Vector.AsVectorSingle(FloatHackBase)));

                p4 = p4a;
                p4 = Vector.BitwiseOr(p4, Vector.AsVectorByte(AlphaOrMask));
                // HACK: neither multiply nor divide are implemented as instructions but as calls, and no shift is provided
                //p4 = Vector.Min(p4, Vector.AsVectorByte(Vector.Divide(Vector.AsVectorUInt32(p4), new Vector<uint>(0x100))));
                //p4 = Vector.Min(p4, Vector.AsVectorByte(Vector.Divide(Vector.AsVectorUInt32(p4), new Vector<uint>(0x10000))));
                Unsafe.Write(hackWorkspace, p4);
                p4 = Vector.Min(Vector.Min(p4, Unsafe.Read<Vector<byte>>(hackWorkspace + 1)), Unsafe.Read<Vector<byte>>(hackWorkspace + 2));
                p4 = p4 & Vector.AsVectorByte(LowByteMask);
                // HACK to convert byte to float (need https://github.com/dotnet/corefx/issues/1605)
                Vector<float> min = Vector.AsVectorSingle(Vector.Subtract(Vector.AsVectorSingle(Vector.Add(Vector.AsVectorUInt32(p4), FloatHackBase)), Vector.AsVectorSingle(FloatHackBase)));

                s = Vector.ConditionalSelect(
                    Vector.Equals(max, Vector<float>.Zero),
                    Vector<float>.Zero,
                    Vector<float>.One - (min / max));
                v = max;
            }
#endif

            public ColorRGB ToRGB()
            {
                float r = 0;
                float g = 0;
                float b = 0;

                if (S == 0)
                {
                    r = g = b = V;
                }
                else
                {
                    // the color wheel consists of 6 sectors. Figure out which sector you're in.
                    float sectorPos = H / 60;
                    int sectorNumber = (int)(Math.Floor(sectorPos));
                    // get the fractional part of the sector
                    float fractionalSector = sectorPos - sectorNumber;

                    // calculate values for the three axes of the color.
                    float p = V * (1 - S);
                    float q = V * (1 - (S * fractionalSector));
                    float t = V * (1 - (S * (1 - fractionalSector)));

                    // assign the fractional colors to r, g, and b based on the sector the angle is in.
                    switch (sectorNumber)
                    {
                        case 0:
                            r = V;
                            g = t;
                            b = p;
                            break;
                        case 1:
                            r = q;
                            g = V;
                            b = p;
                            break;
                        case 2:
                            r = p;
                            g = V;
                            b = t;
                            break;
                        case 3:
                            r = p;
                            g = q;
                            b = V;
                            break;
                        case 4:
                            r = t;
                            g = p;
                            b = V;
                            break;
                        case 5:
                            r = V;
                            g = p;
                            b = q;
                            break;
                    }
                }

                ColorRGB rgb = new ColorRGB(r, g, b);
                return rgb;
            }

            public override string ToString()
            {
                return String.Format("({0:N0}, {1:N2}, {2:N2})", H, S, V);
            }
        }

        public static unsafe void Analyze(Item item, SmartBitmap bitmap, GlobalOptions options, AddMessageMethod addMessage, CancellationTokenSource cancel)
        {
            Profile profile = new Profile("Transforms.Analyze {0}", item.RenamedFileName);

            using (bitmap)
            {
                ManagedBitmap managedBitmap = bitmap.AsManaged(profile);

                if (options.EnableAutoNormalizeGeometry && !item.NormalizeGeometryExplicitlySet)
                {
                    AnalyzeAutoNormalizeGeometry(
                        profile,
                        item,
                        managedBitmap,
                        options.AutoCropLeftMax,
                        options.AutoCropTopMax,
                        options.AutoCropRightMax,
                        options.AutoCropBottomMax,
                        options.AutoCropMinMedianBrightness,
                        addMessage,
                        cancel);
                }

                if (item.NormalizeGeometry || (item.FineRotateDegrees != 0))
                {
                    ApplyNormalizeGeometry(
                        profile,
                        item.SourceFileName,
                        managedBitmap,
                        1,
                        new NormalizeGeometryParameters(
                            item.CornerTL,
                            item.CornerTR,
                            item.CornerBL,
                            item.CornerBR,
                            item.NormalizeGeometryForcedAspectRatio,
                            item.FineRotateDegrees,
                            item.NormalizeGeometryPreviewInterp),
                        addMessage,
                        cancel);
                }

                item.HF = AnalyzeHF(
                    profile,
                    item,
                    managedBitmap,
                    cancel);

                if (options.AutoCrop && (item.CropRect.IsEmpty || !item.CropRectExplicitlySet))
                {
                    item.CropRectForAutoCrop = AnalyzeAutoCrop(
                        profile,
                        item,
                        managedBitmap,
                        options.AutoCropLeftMax,
                        options.AutoCropTopMax,
                        options.AutoCropRightMax,
                        options.AutoCropBottomMax,
                        options.AutoCropMinMedianBrightness,
                        addMessage,
                        cancel);
                }
            }

            profile.End();
            Program.Log(LogCat.Perf, profile.Report());
        }

        public const int HighFrequencyBlockSize = 64;

        private unsafe static bool[,] AnalyzeHF(Profile profile, Item item, ManagedBitmap bitmap, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.AnalyzeHF");

            int* scan0 = (int*)bitmap.Scan0;
            int stride = bitmap.Stride / 4;

            int imageWidth = bitmap.Width;
            int imageHeight = bitmap.Height;

            bool[,] hf = new bool[(imageWidth + HighFrequencyBlockSize - 1) / HighFrequencyBlockSize, (imageHeight + HighFrequencyBlockSize - 1) / HighFrequencyBlockSize];

            float[][] vBuf = new float[HighFrequencyBlockSize + 1][];
            for (int i = 0; i < HighFrequencyBlockSize + 1; i++)
            {
                vBuf[i] = new float[HighFrequencyBlockSize + 1];
            }

            for (int j = 0; j < imageHeight; j += HighFrequencyBlockSize)
            {
                cancel.Token.ThrowIfCancellationRequested();
                for (int i = 0; i < imageWidth; i += HighFrequencyBlockSize)
                {
                    //const float HSMin = (float)(Math.PI / 2);
                    const float VMin = .07f;
                    const float Fraction = .1f;

                    int c = 0, o = 0;

                    int jjl = Math.Min(j + HighFrequencyBlockSize, imageHeight - 1);
                    int iil = Math.Min(i + HighFrequencyBlockSize, imageWidth - 1);

                    for (int jj = j; jj <= jjl; jj++)
                    {
                        int* prow = scan0 + jj * stride;
                        for (int ii = i; ii <= iil; ii++)
                        {
                            int* pp = prow + ii;
                            //ColorRGB rgb = new ColorRGB(*pp);
                            //ColorHSV hsv = new ColorHSV(rgb);
                            //vBuf[jj - j][ii - i] = hsv.v;
                            float v = ColorRGB.RgbMaxF(*pp);
                            vBuf[jj - j][ii - i] = v;
                        }
                    }

                    for (int jj = j; jj < jjl; jj++)
                    {
                        float v10 = vBuf[jj - j][0];
                        for (int ii = i; ii < iil; ii++)
                        {
                            //float v00 = vBuf[jj - j][ii - i];
                            float v00 = v10;
                            float v01 = vBuf[jj - j + 1][ii - i];
                            v10 = vBuf[jj - j][ii - i + 1];

                            float vd = Math.Max(Math.Abs(v00 - v01), Math.Abs(v00 - v10));

                            c++;
                            if (vd >= VMin)
                            {
                                o++;
                            }
                        }
                    }

                    hf[i / HighFrequencyBlockSize, j / HighFrequencyBlockSize] = (float)o / c >= Fraction;
                }
            }

            profile.Pop();

            return hf;
        }

        private static void AnalyzeAutoNormalizeGeometry(Profile profile, Item item, ManagedBitmap bitmap, float leftMax, float topMax, float rightMax, float bottomMax, float minMedianBrightness, AddMessageMethod addMessage, CancellationTokenSource cancel)
        {
            profile.Push("AnalyzeAutoNormalizeGeometry");

            Debug.Assert(!item.NormalizeGeometryExplicitlySet);

            int cWidth = (bitmap.Width + AutoCropBlockSize - 1) / AutoCropBlockSize;
            int cHeight = (bitmap.Height + AutoCropBlockSize - 1) / AutoCropBlockSize;


            profile.Push("GenerateAutoCropGrid");
            bool[,] g = GenerateAutoCropGrid(profile, bitmap, minMedianBrightness, cancel);
            profile.Pop();


            profile.Push("Find lines");

            float leftA, leftB, leftC;
            if (!FitLineToPageEdge(g, FitSide.Left, leftMax, out leftA, out leftB, out leftC))
            {
                string message = "AutoNormalizeGeometry failed to find left edge.";
                addMessage?.Invoke(message);
                Program.Log(LogCat.Xform, String.Format(
                    "{1}: {2}){0}",
                    Environment.NewLine,
                    item.RenamedFileName,
                    message));
            }

            float rightA, rightB, rightC;
            if (!FitLineToPageEdge(g, FitSide.Right, rightMax, out rightA, out rightB, out rightC))
            {
                string message = "AutoNormalizeGeometry failed to find right edge.";
                addMessage?.Invoke(message);
                Program.Log(LogCat.Xform, String.Format(
                    "{1}: {2}){0}",
                    Environment.NewLine,
                    item.RenamedFileName,
                    message));
            }
            float topA, topB, topC;
            if (!FitLineToPageEdge(g, FitSide.Top, topMax, out topA, out topB, out topC))
            {
                string message = "AutoNormalizeGeometry failed to find top edge.";
                addMessage?.Invoke(message);
                Program.Log(LogCat.Xform, String.Format(
                    "{1}: {2}){0}",
                    Environment.NewLine,
                    item.RenamedFileName,
                    message));
            }

            float bottomA, bottomB, bottomC;
            if (!FitLineToPageEdge(g, FitSide.Bottom, leftMax, out bottomA, out bottomB, out bottomC))
            {
                string message = "AutoNormalizeGeometry failed to find bottom edge.";
                addMessage?.Invoke(message);
                Program.Log(LogCat.Xform, String.Format(
                    "{1}: {2}){0}",
                    Environment.NewLine,
                    item.RenamedFileName,
                    message));
            }

            item.SetCornerTL(IntersectH(leftA, leftB, leftC, topA, topB, topC), false/*setByUser*/);
            item.SetCornerTR(IntersectH(topA, topB, topC, rightA, rightB, rightC), false/*setByUser*/);
            item.SetCornerBL(IntersectH(leftA, leftB, leftC, bottomA, bottomB, bottomC), false/*setByUser*/);
            item.SetCornerBR(IntersectH(bottomA, bottomB, bottomC, rightA, rightB, rightC), false/*setByUser*/);
            item.NormalizeGeometry = true;
            item.NormalizeGeometryExplicitlySet = false;

            profile.Pop();


            profile.Pop();
        }

        // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Using_homogeneous_coordinates
        public static Point IntersectH(float a1, float b1, float c1, float a2, float b2, float c2)
        {
            float x = b1 * c2 - b2 * c1;
            float y = a2 * c1 - a1 * c2;
            float w = a1 * b2 - a2 * b1;
            x /= w;
            y /= w;
            return new Point((int)(x + .5f), (int)(y + .5f));
        }

        // from y=mx+b to Ax+By+C=0
        public static void MBtoABC(float m, float b, bool reflect, out float A, out float B, out float C)
        {
            if (!reflect)
            {
                A = -m;
                B = 1;
            }
            else
            {
                A = 1;
                B = -m;
            }
            C = -b;
        }

        public static void LineFromPoints(Point one, Point two, out float m, out float b, out bool reflect)
        {
            float dx = one.X - two.X;
            float dy = one.Y - two.Y;
            if (Math.Abs(dy) < Math.Abs(dx))
            {
                reflect = false;
                m = dy / dx;
                b = one.Y - m * one.X;
            }
            else
            {
                reflect = true;
                m = dx / dy;
                b = one.X - m * one.Y;
            }
        }

        public static void LineFromPoints(Point one, Point two, out float A, out float B, out float C)
        {
            float m, b;
            bool reflect;
            LineFromPoints(one, two, out m, out b, out reflect);
            MBtoABC(m, b, reflect, out A, out B, out C);
        }

        // Obtain coefficients for fitted line, a*x + b*y + c = 0
        private enum FitSide { Left, Top, Right, Bottom };
        private static bool FitLineToPageEdge(bool[,] g, FitSide side, float inset, out float coefA, out float coefB, out float coefC)
        {
            coefA = coefB = coefC = 0;

            int cWidth = g.GetLength(1);
            int cHeight = g.GetLength(0);

            bool reflect = (side == FitSide.Left) || (side == FitSide.Right);

            List<Point> points = new List<Point>();
            {
                bool[] stops;
                switch (side)
                {
                    default:
                        Debug.Assert(false);
                        throw new ArgumentException();

                    case FitSide.Left:
                        stops = new bool[cHeight];
                        for (int j = 0; j < (int)(inset * cWidth); j++)
                        {
                            for (int i = 0; i < cHeight; i++)
                            {
                                if (!stops[i] && g[i, j])
                                {
                                    stops[i] = true;
                                    points.Add(new Point(j * AutoCropBlockSize, i * AutoCropBlockSize + AutoCropBlockSize / 2));
                                }
                            }
                        }
                        break;

                    case FitSide.Right:
                        stops = new bool[cHeight];
                        for (int j = cWidth - 1; j > (int)(cWidth - inset * cWidth); j--)
                        {
                            for (int i = 0; i < cHeight; i++)
                            {
                                if (!stops[i] && g[i, j])
                                {
                                    stops[i] = true;
                                    points.Add(new Point((j + 1) * AutoCropBlockSize, i * AutoCropBlockSize + AutoCropBlockSize / 2));
                                }
                            }
                        }
                        break;

                    case FitSide.Top:
                        stops = new bool[cWidth];
                        for (int i = 0; i < (int)(inset * cHeight); i++)
                        {
                            for (int j = 0; j < cWidth; j++)
                            {
                                if (!stops[j] && g[i, j])
                                {
                                    stops[j] = true;
                                    points.Add(new Point(j * AutoCropBlockSize + AutoCropBlockSize / 2, i * AutoCropBlockSize));
                                }
                            }
                        }
                        break;

                    case FitSide.Bottom:
                        stops = new bool[cWidth];
                        for (int i = cHeight - 1; i > (int)(cHeight - inset * cHeight); i--)
                        {
                            for (int j = 0; j < cWidth; j++)
                            {
                                if (!stops[j] && g[i, j])
                                {
                                    stops[j] = true;
                                    points.Add(new Point(j * AutoCropBlockSize + AutoCropBlockSize / 2, (i + 1) * AutoCropBlockSize));
                                }
                            }
                        }
                        break;
                }
            }

            if (points.Count < 2)
            {
                return false;
            }

            points.Sort(delegate (Point l, Point r) { return (reflect ? l.Y : l.X).CompareTo(reflect ? r.Y : r.X); });


            // Handling missing corners and other aberrations:
            // Least squares is the best overall fit quality metric in terms of tolerating noisy data, but it
            // allows points far off the ideal line to exert excessive influence. The following finds many lines
            // that fit various subsections of the data and selects the fit that has the most points from the
            // complete set lying "very close" to the fitted line.

            List<Tuple<float, float, float, float>> sections = new List<Tuple<float, float, float, float>>();
            for (int h = 1; h <= 5; h++)
            {
                for (int hh = 0; hh < h; hh++)
                {
                    int start = (int)((float)hh / h * points.Count + .5f);
                    int endp1 = (int)((float)(hh + 1) / h * points.Count + .5f);
                    List<Point> subPoints = points.GetRange(start, endp1 - start);
                    if (subPoints.Count >= 2)
                    {
                        double[] X = subPoints.ConvertAll(delegate (Point p) { return (double)p.X; }).ToArray();
                        double[] Y = subPoints.ConvertAll(delegate (Point p) { return (double)p.Y; }).ToArray();

                        Tuple<double, double> t = SimpleRegression.Fit(!reflect ? X : Y, !reflect ? Y : X);
                        float b = (float)t.Item1; // y = m*x + b;
                        float m = (float)t.Item2;

                        coefA = -m;
                        coefB = 1;
                        coefC = -b;

                        // see https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_an_equation
                        const float MaxDistance = AutoCropBlockSize;
                        int closeCount = 0;
                        float denom = coefA * coefA + coefB * coefB;
                        for (int i = 0; i < points.Count; i++)
                        {
                            float x0 = !reflect ? points[i].X : points[i].Y;
                            float y0 = !reflect ? points[i].Y : points[i].X;

                            float d = Math.Abs(coefA * x0 + coefB * y0 + coefC) / denom;

                            if (d <= MaxDistance)
                            {
                                closeCount++;
                            }
                        }
                        float closeFrac = (float)closeCount / points.Count;
                        sections.Add(new Tuple<float, float, float, float>(coefA, coefB, coefC, closeFrac));
                    }
                }
            }

            // select max closeFrac
            float closestFrac = -1;
            for (int i = 0; i < sections.Count; i++)
            {
                if (closestFrac < sections[i].Item4)
                {
                    closestFrac = sections[i].Item4;
                    coefA = sections[i].Item1;
                    coefB = sections[i].Item2;
                    coefC = sections[i].Item3;
                }
            }

            if (reflect)
            {
                float a2 = coefB;
                float b2 = coefA * coefB;
                float c2 = coefC;
                coefA = a2;
                coefB = b2;
                coefC = c2;
            }

            return true;
        }

        private const int AutoCropBlockSize = LosslessCropGranularity;

        private static Rectangle AnalyzeAutoCrop(Profile profile, Item item, ManagedBitmap bitmap, float leftMax, float topMax, float rightMax, float bottomMax, float minMedianBrightness, AddMessageMethod addMessage, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.AnalyzeAutoCrop");

            int cWidth = (bitmap.Width + AutoCropBlockSize - 1) / AutoCropBlockSize;
            int cHeight = (bitmap.Height + AutoCropBlockSize - 1) / AutoCropBlockSize;

            Rectangle full = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            Rectangle crop = full;

            bool[,] g = GenerateAutoCropGrid(profile, bitmap, minMedianBrightness, cancel);

            const float thresh = .125f;
            for (int j = 0; j < cWidth / 2; j++)
            {
                int o = j;
                int n = cHeight / 2 - o;
                int c = 0;
                for (int i = o; i < n + o; i++)
                {
                    if (g[i, j])
                    {
                        c++;
                    }
                }
                if (c >= n * thresh)
                {
                    break;
                }
                crop.X += AutoCropBlockSize;
                crop.Width -= AutoCropBlockSize;
            }
            for (int j = cWidth - 1; j > cWidth / 2; j--)
            {
                int o = cWidth - 1 - j;
                int n = cHeight / 2 - o;
                int c = 0;
                for (int i = o; i < n + o; i++)
                {
                    if (g[i, j])
                    {
                        c++;
                    }
                }
                if (c >= n * thresh)
                {
                    break;
                }
                if (crop.Width % AutoCropBlockSize != 0)
                {
                    crop.Width = crop.Width / AutoCropBlockSize * AutoCropBlockSize;
                }
                else
                {
                    crop.Width -= AutoCropBlockSize;
                }
            }
            for (int i = 0; i < cHeight / 2; i++)
            {
                int o = i;
                int n = cWidth / 2 - i;
                int c = 0;
                for (int j = o; j < n + o; j++)
                {
                    if (g[i, j])
                    {
                        c++;
                    }
                }
                if (c >= n * thresh)
                {
                    break;
                }
                crop.Y += AutoCropBlockSize;
                crop.Height -= AutoCropBlockSize;
            }
            for (int i = cHeight - 1; i > cHeight / 2; i--)
            {
                int o = cHeight - 1 - i;
                int n = cWidth / 2 - o;
                int c = 0;
                for (int j = o; j < n + o; j++)
                {
                    if (g[i, j])
                    {
                        c++;
                    }
                }
                if (c >= n * thresh)
                {
                    break;
                }
                if (crop.Height % AutoCropBlockSize != 0)
                {
                    crop.Height = crop.Height / AutoCropBlockSize * AutoCropBlockSize;
                }
                else
                {
                    crop.Height -= AutoCropBlockSize;
                }
            }

            item.AutoCropGrid = g;

            if (((float)crop.X / bitmap.Width > leftMax)
                || (((float)bitmap.Width - crop.Right) / bitmap.Width > rightMax)
                || ((float)crop.Y / bitmap.Height > topMax)
                || (((float)bitmap.Height - crop.Bottom) / bitmap.Height > bottomMax))
            {
                string message = "AutoCrop not done due to excessive trim.";
                addMessage?.Invoke(message);
                Program.Log(LogCat.Xform, String.Format(
                    "{1}: {2}){0}",
                    Environment.NewLine,
                    item.RenamedFileName,
                    message));
                return new Rectangle();
            }
            if (crop == full)
            {
                crop = new Rectangle();
            }

            profile.Pop();

            return crop;
        }

        private unsafe static bool[,] GenerateAutoCropGrid(Profile profile, ManagedBitmap bitmap, float minMedianBrightness, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.GenerateAutoCropGrid");

            int* scan0 = (int*)bitmap.Scan0;
            int stride = bitmap.Stride / 4;

            const float maxS = .1f;
            float minMV = minMedianBrightness; // was hardcoded .5f

            int cWidth = (bitmap.Width + AutoCropBlockSize - 1) / AutoCropBlockSize;
            int cHeight = (bitmap.Height + AutoCropBlockSize - 1) / AutoCropBlockSize;
            bool[,] g = new bool[cHeight, cWidth];
            for (int i = 0; i < cHeight; i++)
            {
                cancel.Token.ThrowIfCancellationRequested();
                for (int j = 0; j < cWidth; j++)
                {
                    const int NumBins = 100;
                    Histogram hist = new Histogram(NumBins, NumBins);

                    double aS1 = 0;
                    int lii = Math.Min((i + 1) * AutoCropBlockSize, bitmap.Height);
                    int ljj = Math.Min((j + 1) * AutoCropBlockSize, bitmap.Width);
                    int n = (lii - i * AutoCropBlockSize) * (ljj - j * AutoCropBlockSize);
#if false // TODO: remove
                    if (EnableVectors && (Vector.IsHardwareAccelerated || VectorHardwareAcceleratedOverride) && (lii == AutoCropBlockSize))
                    {
                        // vector
                        for (int ii = i * AutoCropBlockSize; ii < lii; ii++)
                        {
                            for (int jj = j * AutoCropBlockSize; jj < ljj; jj += Vector<float>.Count)
                            {
                                int* pp = scan0 + ii * stride + jj;
                                Vector<float> v, s;
                                ColorHSV.GetSV(pp, out s, out v, pHackWorkspace);
                                switch (Vector<float>.Count)
                                {
                                    default:
                                        for (int vi = 0; vi < Vector<float>.Count; vi++)
                                        {
                                            pMV[cmv++] = v[vi];
                                            aS1 += s[vi];
                                        }
                                        break;
                                    case 4:
                                        Unsafe.Write(pHackWorkspace, v);
                                        pMV[cmv++] = ((float*)pHackWorkspace)[0];
                                        pMV[cmv++] = ((float*)pHackWorkspace)[1];
                                        pMV[cmv++] = ((float*)pHackWorkspace)[2];
                                        pMV[cmv++] = ((float*)pHackWorkspace)[3];
                                        Unsafe.Write(pHackWorkspace, s);
                                        aS1 += ((float*)pHackWorkspace)[0];
                                        aS1 += ((float*)pHackWorkspace)[1];
                                        aS1 += ((float*)pHackWorkspace)[2];
                                        aS1 += ((float*)pHackWorkspace)[3];
                                        break;
                                }
                            }
                        }
                    }
                    else
#endif
                    {
                        // non-vector
                        for (int ii = i * AutoCropBlockSize; ii < lii; ii++)
                        {
                            for (int jj = j * AutoCropBlockSize; jj < ljj; jj++)
                            {
                                int* pp = scan0 + ii * stride + jj;
                                //ColorRGB rgb = new ColorRGB(*pp);
                                //ColorHSV hsv = new ColorHSV(rgb);
                                //float v = hsv.v;
                                //float s = hsv.s;
                                int v;
                                float s;
                                ColorHSV.GetSV(*pp, out s, out v);
                                aS1 += s;
                                hist.Add(v * NumBins / 255);
                            }
                        }
                    }

                    // average saturation
                    aS1 /= n;

                    // estimate background white level by computing mode, with compactness check to attempt to exclude
                    // regions of a page that may contain photos or dark diagrams.
                    int mode, modeC;
                    hist.GetMode(out mode, out modeC);
                    // compactness check
                    const int CompactnessWindowRadius = 8;
                    const float MinClusterFrac = .5f;
                    int binSum = hist.SumAroundRadius(mode, CompactnessWindowRadius);
                    bool compact = (float)binSum / n > MinClusterFrac;
                    float modef = (float)mode / NumBins;

                    if ((aS1 < maxS) && (modef >= minMV) && compact)
                    {
                        // mark if saturation is very low (whitish), and mode brightness is over cutoff, and mode peak is compact
                        g[i, j] = true;
                    }
                }
            }

            profile.Pop();

            return g;
        }

        public struct NormalizeGeometryParameters
        {
            public readonly Point cornerTL, cornerTR, cornerBL, cornerBR;
            public readonly float? normalizedGeometryAspectRatio;
            public readonly InterpMethod interpolation;
            public readonly double fineRotationDegrees;

            public NormalizeGeometryParameters(Point cornerTL, Point cornerTR, Point cornerBL, Point cornerBR, float? normalizedGeometryAspectRatio, double fineRotationDegrees, InterpMethod interpolation)
            {
                this.cornerTL = cornerTL;
                this.cornerTR = cornerTR;
                this.cornerBL = cornerBL;
                this.cornerBR = cornerBR;
                this.normalizedGeometryAspectRatio = normalizedGeometryAspectRatio;
                this.fineRotationDegrees = fineRotationDegrees;
                this.interpolation = interpolation;
            }
        }

        private struct Matrix3x3
        {
            public readonly float _00;
            public readonly float _01;
            public readonly float _02;
            public readonly float _10;
            public readonly float _11;
            public readonly float _12;
            public readonly float _20;
            public readonly float _21;
            public readonly float _22;

            public Matrix3x3(float _00, float _01, float _02, float _10, float _11, float _12, float _20, float _21, float _22)
            {
                this._00 = _00;
                this._01 = _01;
                this._02 = _02;
                this._10 = _10;
                this._11 = _11;
                this._12 = _12;
                this._20 = _20;
                this._21 = _21;
                this._22 = _22;
            }

            public void VMul(float x0, float x1, float x2, out float y0, out float y1, out float y2)
            {
                y0 = x0 * _00 + x1 * _10 + x2 * _20;
                y1 = x0 * _01 + x1 * _11 + x2 * _21;
                y2 = x0 * _02 + x1 * _12 + x2 * _22;
            }

            public Matrix3x3 Scale(float factor)
            {
                return new Matrix3x3(
                    factor * _00,
                    factor * _01,
                    factor * _02,
                    factor * _10,
                    factor * _11,
                    factor * _12,
                    factor * _00,
                    factor * _21,
                    factor * _22);
            }

            public bool IsIdentity { get { return (_00 == 1) && (_01 == 0) && (_02 == 0) && (_10 == 0) && (_11 == 1) && (_12 == 0) && (_20 == 0) && (_21 == 0) && (_22 == 1); } }

            public Matrix<float> ToMatrix()
            {
                return Matrix<float>.Build.DenseOfArray(
                    new float[3, 3]
                    {
                        { _00, _01, _02, },
                        { _10, _11, _12, },
                        { _20, _21, _22, }
                    });
            }

            public static Matrix3x3 FromMatrix(Matrix<float> m)
            {
                return new AdaptiveImageSizeReducer.Transforms.Matrix3x3(m[0, 0], m[0, 1], m[0, 2], m[1, 0], m[1, 1], m[1, 2], m[2, 0], m[2, 1], m[2, 2]);
            }
        }

        // from http://www.ams.org/samplings/feature-column/fc-2013-03 - "Using Projective Geometry to Correct a Camera"
        public static void ApplyNormalizeGeometry(Profile profile, string tag, ManagedBitmap bitmap, int previewScaleDivisor, NormalizeGeometryParameters normalizeParameters, AddMessageMethod addMessage, CancellationTokenSource cancel)
        {
            profile.Push("ApplyNormalizeGeometry");

            const bool debug = false;

            // Since we're not using WPF to do a forward transform to the normalized geometry, but rather doing a
            // back projection to the original image, the source and target points are inverted.

            float x0, y0, x1, y1, x2, y2, x3, y3;
            if (!normalizeParameters.normalizedGeometryAspectRatio.HasValue)
            {
                x0 = (normalizeParameters.cornerTL.X + normalizeParameters.cornerBL.X) / 2 / previewScaleDivisor;
                y0 = (normalizeParameters.cornerTL.Y + normalizeParameters.cornerTR.Y) / 2 / previewScaleDivisor;
                x1 = (normalizeParameters.cornerTR.X + normalizeParameters.cornerBR.X) / 2 / previewScaleDivisor;
                y1 = y0;
                x2 = x0;
                y2 = (normalizeParameters.cornerBL.Y + normalizeParameters.cornerBR.Y) / 2 / previewScaleDivisor;
                x3 = x1;
                y3 = y2;
            }
            else
            {
                const float MinInset = .025f;

                float width, height;
                float xoffset, yoffset;

                float ratio = normalizeParameters.normalizedGeometryAspectRatio.Value;
                float imageAspect = (float)bitmap.Width / (float)bitmap.Height;
                if (ratio > imageAspect)
                {
                    // maximized margin on left/right
                    width = (1 - 2 * MinInset) * bitmap.Width;
                    height = width * ratio;
                    xoffset = MinInset * bitmap.Width;
                    yoffset = (bitmap.Height - height) / 2;
                }
                else
                {
                    // maximized margin on top/bottom
                    height = (1 - 2 * MinInset) * bitmap.Height;
                    width = height * ratio;
                    yoffset = MinInset * bitmap.Height;
                    xoffset = (bitmap.Width - width) / 2;
                }

                x0 = xoffset;
                y0 = yoffset;
                x1 = xoffset + width;
                y1 = yoffset;
                x2 = xoffset;
                y2 = yoffset + height;
                x3 = xoffset + width;
                y3 = yoffset + height;
            }

            float u0 = normalizeParameters.cornerTL.X / previewScaleDivisor, v0 = normalizeParameters.cornerTL.Y / previewScaleDivisor;
            float u1 = normalizeParameters.cornerTR.X / previewScaleDivisor, v1 = normalizeParameters.cornerTR.Y / previewScaleDivisor;
            float u2 = normalizeParameters.cornerBL.X / previewScaleDivisor, v2 = normalizeParameters.cornerBL.Y / previewScaleDivisor;
            float u3 = normalizeParameters.cornerBR.X / previewScaleDivisor, v3 = normalizeParameters.cornerBR.Y / previewScaleDivisor;

            Matrix<double> A = Matrix<double>.Build.DenseOfArray(
                new double[9, 9]
                {
                    { x0, y0, 1, 0,  0,  0, -x0 * u0, -y0 * u0, -u0 },
                    { 0,  0,  0, x0, y0, 1, -x0 * v0, -y0 * v0, -v0 },
                    { x1, y1, 1, 0,  0,  0, -x1 * u1, -y1 * u1, -u1 },
                    { 0,  0,  0, x1, y1, 1, -x1 * v1, -y1 * v1, -v1 },
                    { x2, y2, 1, 0,  0,  0, -x2 * u2, -y2 * u2, -u2 },
                    { 0,  0,  0, x2, y2, 1, -x2 * v2, -y2 * v2, -v2 },
                    { x3, y3, 1, 0,  0,  0, -x3 * u3, -y3 * u3, -u3 },
                    { 0,  0,  0, x3, y3, 1, -x3 * v3, -y3 * v3, -v3 },
                    { 0,  0,  0, 0,  0,  0,        0,        0,   1 } // arbitrary scalar multiple constraint
                });
            MathNet.Numerics.LinearAlgebra.Vector<double> B = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(
                new double[9] { 0, 0, 0, 0, 0, 0, 0, 0, 1 });
            MathNet.Numerics.LinearAlgebra.Vector<double> X = A.Solve(B);

            Matrix3x3 Mst = new Matrix3x3((float)X[0], (float)X[3], (float)X[6], (float)X[1], (float)X[4], (float)X[7], (float)X[2], (float)X[5], (float)X[8]);

            bool edgeExtend = !Mst.IsIdentity;

            if (normalizeParameters.fineRotationDegrees != 0)
            {
                float translateX = bitmap.Width / 2f;
                float translateY = bitmap.Height / 2f;

                Matrix<float> m = Mst.ToMatrix();

                m = m.Transpose();

                Matrix<float> translate = Matrix<float>.Build.DenseOfArray(
                    new float[3, 3]
                    {
                        { 1, 0, translateX, },
                        { 0, 1, translateY, },
                        { 0, 0, 1, }
                    });
                Matrix<float> untranslate = Matrix<float>.Build.DenseOfArray(
                    new float[3, 3]
                    {
                        { 1, 0, -translateX, },
                        { 0, 1, -translateY, },
                        { 0, 0, 1, }
                    });

                // translate center to origin
                m = m.Multiply(translate);

                double r = normalizeParameters.fineRotationDegrees / 180 * Math.PI;
                float cos = (float)Math.Cos(r);
                float sin = (float)Math.Sin(r);
                Matrix<float> rotate = Matrix<float>.Build.DenseOfArray(
                    new float[3, 3]
                    {
                        { cos, sin, 0, },
                        { -sin, cos, 0, },
                        { 0, 0, 1, }
                    });
                m = m.Multiply(rotate);

                // scale to fit
                float ufMax = 0, vfMax = 0;
                foreach (Point p in new Point[] { new Point(0, 0), new Point(0, bitmap.Height - 1), new Point(bitmap.Width - 1, 0), new Point(bitmap.Width - 1, bitmap.Height - 1) })
                {
                    float uf, vf, wf;
                    MathNet.Numerics.LinearAlgebra.Vector<float> v = rotate.Multiply(MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(new float[] { p.X - bitmap.Width / 2, p.Y - bitmap.Height / 2, 1 }));
                    uf = v[0];
                    vf = v[1];
                    wf = v[2];
                    uf = uf / wf;
                    vf = vf / wf;
                    ufMax = Math.Max(ufMax, uf);
                    vfMax = Math.Max(vfMax, vf);
                }
                float factor = Math.Max(ufMax / (bitmap.Width - 1) * 2, vfMax / (bitmap.Height - 1) * 2);
                Matrix<float> scale = Matrix<float>.Build.DenseOfArray(
                    new float[3, 3]
                    {
                        { factor, 0, 0, },
                        { 0, factor, 0, },
                        { 0, 0, 1, }
                    });
                m = m.Multiply(scale);

                // restore origin
                m = m.Multiply(untranslate);

                m = m.Transpose();

                Mst = Matrix3x3.FromMatrix(m);
            }

            using (ManagedBitmap32 result = new ManagedBitmap32(bitmap.Width, bitmap.Height))
            {
                ProjectiveTransform(bitmap, Mst, normalizeParameters.interpolation, edgeExtend, result, cancel);

#pragma warning disable CS0162 // unreachable
                if (debug)
                {
                    using (Bitmap overlay = result.GetSectionEnslaved(new Rectangle(Point.Empty, result.Size)))
                    {
                        using (Graphics graphics = Graphics.FromImage(overlay))
                        {
                            List<Tuple<System.Drawing.Brush, float, float>> points = new List<Tuple<System.Drawing.Brush, float, float>>();

                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Orange, x0, y0));
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Orange, x1, y1));
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Orange, x3, y3));
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Orange, x2, y2));

                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.LightPink, u0, v0));
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.LightPink, u1, v1));
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.LightPink, u3, v3));
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.LightPink, u2, v2));

                            float u, v, w;
                            Mst.VMul(x0, y0, 1, out u, out v, out w);
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Chartreuse, u / w, v / w));
                            Mst.VMul(x1, y1, 1, out u, out v, out w);
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Chartreuse, u / w, v / w));
                            Mst.VMul(x3, y3, 1, out u, out v, out w);
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Chartreuse, u / w, v / w));
                            Mst.VMul(x2, y2, 1, out u, out v, out w);
                            points.Add(new Tuple<System.Drawing.Brush, float, float>(System.Drawing.Brushes.Chartreuse, u / w, v / w));

                            System.Drawing.Brush lastBrush = null;
                            List<Tuple<float, float>> lastPoints = new List<Tuple<float, float>>();
                            foreach (Tuple<System.Drawing.Brush, float, float> point in points)
                            {
                                graphics.FillRectangle(point.Item1, point.Item2 - 10, point.Item3 - 10, 20, 20);
                                lastBrush = point.Item1;
                                lastPoints.Add(new Tuple<float, float>(point.Item2, point.Item3));
                                if (lastPoints.Count == 4)
                                {
                                    lastPoints.Add(lastPoints[0]);
                                    graphics.DrawLines(
                                        new System.Drawing.Pen(lastBrush, 2),
                                        lastPoints.ConvertAll(delegate (Tuple<float, float> item) { return new PointF(item.Item1, item.Item2); }).ToArray());
                                    lastPoints.Clear();
                                }
                            }
                        }
                    }
                }
#pragma warning restore CS0162

                result.CopyTo(bitmap);
            }

            profile.Pop();
        }

        public enum InterpMethod { Bicubic = 0, Bilinear = 1, NearestNeighbor = 2 };

        private static void ProjectiveTransform(ManagedBitmap bitmap, Matrix3x3 Mst, InterpMethod interpolation, bool edgeExtend, ManagedBitmap32 result, CancellationTokenSource cancel)
        {
            Rectangle bounds = new Rectangle(Point.Empty, bitmap.Size);

            Parallel.For( // for (int y = 0; y < bitmap.Height; y++)
                0,
                bitmap.Height,
                Program.GetProcessorConstrainedParallelOptions2(cancel.Token),
                delegate (int y)
                {
                    switch (interpolation)
                    {
                        default:
                            Debug.Assert(false);
                            throw new ArgumentException();

                        case InterpMethod.NearestNeighbor:
                            {
                                float limitW = bitmap.Width - 1;
                                float limitH = bitmap.Height - 1;

                                for (int x = 0; x < bitmap.Width; x++)
                                {
                                    float uf, vf, wf;
                                    Mst.VMul(x, y, 1, out uf, out vf, out wf);

                                    uf = Math.Min(Math.Max(uf / wf + .5f, 0), limitW);
                                    vf = Math.Min(Math.Max(vf / wf + .5f, 0), limitH);

                                    int u = (int)uf;
                                    int v = (int)vf;

                                    result[x, y] = bitmap[u, v];
                                }
                            }
                            break;

                        case InterpMethod.Bilinear:
                            {
                                float limitW = bitmap.Width - 2;
                                float limitH = bitmap.Height - 2;

                                for (int x = 0; x < bitmap.Width; x++)
                                {
                                    float uf, vf, wf;
                                    Mst.VMul(x, y, 1, out uf, out vf, out wf);

                                    uf = Math.Min(Math.Max(uf / wf, 0), limitW);
                                    vf = Math.Min(Math.Max(vf / wf, 0), limitH);

                                    int u = (int)uf;
                                    int v = (int)vf;

                                    ColorRGB rgb00 = new ColorRGB(bitmap[u + 0, v + 0]); // note: rgbXX is named col,row order
                                    ColorRGB rgb10 = new ColorRGB(bitmap[u + 1, v + 0]);
                                    ColorRGB rgb01 = new ColorRGB(bitmap[u + 0, v + 1]);
                                    ColorRGB rgb11 = new ColorRGB(bitmap[u + 1, v + 1]);

                                    float ywr = vf - v;
                                    ColorRGB rgb0 = rgb00 + ywr * (rgb01 - rgb00);
                                    ColorRGB rgb1 = rgb10 + ywr * (rgb11 - rgb10);

                                    float xwr = uf - u;
                                    ColorRGB rgb = rgb0 + xwr * (rgb1 - rgb0);

                                    result[x, y] = rgb.ToRGB32();
                                }
                            }
                            break;

                        case InterpMethod.Bicubic:
                            {
                                float limitW = bitmap.Width;
                                float limitH = bitmap.Height;

                                for (int x = 0; x < bitmap.Width; x++)
                                {
                                    float uf, vf, wf;
                                    Mst.VMul(x, y, 1, out uf, out vf, out wf);

                                    uf = Math.Min(Math.Max(uf / wf, 0), limitW);
                                    vf = Math.Min(Math.Max(vf / wf, 0), limitH);

                                    int u = (int)uf;
                                    int v = (int)vf;

                                    // based on code from ImageMagick (http://www.imagemagick.org/script/index.php)

                                    float wum9, wup0, wup1, wup2;
                                    GetSplineWeights(uf - u, out wum9, out wup0, out wup1, out wup2);
                                    float wvm9, wvp0, wvp1, wvp2;
                                    GetSplineWeights(vf - v, out wvm9, out wvp0, out wvp1, out wvp2);

                                    int um1 = Math.Max(u - 1, 0);
                                    int up0 = Math.Min(u + 0, bitmap.Width - 1);
                                    int up1 = Math.Min(u + 1, bitmap.Width - 1);
                                    int up2 = Math.Min(u + 2, bitmap.Width - 1);
                                    int vm1 = Math.Max(v - 1, 0);
                                    int vp0 = Math.Min(v + 0, bitmap.Height - 1);
                                    int vp1 = Math.Min(v + 1, bitmap.Height - 1);
                                    int vp2 = Math.Min(v + 2, bitmap.Height - 1);

                                    Vector3 c0, c1, c2, c3;

                                    c0 = ColorRGB.VectorFromRGB(bitmap[um1, vm1]) * wum9;
                                    c1 = ColorRGB.VectorFromRGB(bitmap[um1, vp0]) * wum9;
                                    c2 = ColorRGB.VectorFromRGB(bitmap[um1, vp1]) * wum9;
                                    c3 = ColorRGB.VectorFromRGB(bitmap[um1, vp2]) * wum9;

                                    c0 += ColorRGB.VectorFromRGB(bitmap[up0, vm1]) * wup0;
                                    c1 += ColorRGB.VectorFromRGB(bitmap[up0, vp0]) * wup0;
                                    c2 += ColorRGB.VectorFromRGB(bitmap[up0, vp1]) * wup0;
                                    c3 += ColorRGB.VectorFromRGB(bitmap[up0, vp2]) * wup0;

                                    c0 += ColorRGB.VectorFromRGB(bitmap[up1, vm1]) * wup1;
                                    c1 += ColorRGB.VectorFromRGB(bitmap[up1, vp0]) * wup1;
                                    c2 += ColorRGB.VectorFromRGB(bitmap[up1, vp1]) * wup1;
                                    c3 += ColorRGB.VectorFromRGB(bitmap[up1, vp2]) * wup1;

                                    c0 += ColorRGB.VectorFromRGB(bitmap[up2, vm1]) * wup2;
                                    c1 += ColorRGB.VectorFromRGB(bitmap[up2, vp0]) * wup2;
                                    c2 += ColorRGB.VectorFromRGB(bitmap[up2, vp1]) * wup2;
                                    c3 += ColorRGB.VectorFromRGB(bitmap[up2, vp2]) * wup2;

                                    Vector3 p = c0 * wvm9 + c1 * wvp0 + c2 * wvp1 + c3 * wvp2;

                                    result[x, y] = ColorRGB.RGBFromVector(p);
                                }
                            }
                            break;
                    }

                    if (!edgeExtend)
                    {
                        int limitW = bitmap.Width - 1;
                        int limitH = bitmap.Height - 1;

                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            float uf, vf, wf;
                            Mst.VMul(x, y, 1, out uf, out vf, out wf);

                            int u = (int)(uf / wf + .5f);
                            int v = (int)(vf / wf + .5f);

                            if ((u < 0) || (v < 0) || (u > limitW) || (v > limitH))
                            {
                                result[x, y] = 0;
                            }
                        }
                    }
                });
        }

        // based on code from ImageMagick (http://www.imagemagick.org/script/index.php)
        private static void GetSplineWeights(float t, out float w0, out float w1, out float w2, out float w3)
        {
            // Nicolas Robidoux' 12 flops (6* + 5- + 1+) refactoring of the computation
            // of the standard four 1D cubic B-spline smoothing weights. The sampling
            // location is assumed between the second and third input pixel locations,
            // and x is the position relative to the second input pixel location.
            const float OneOverSix = 1f / 6f;
            float a = 1f - t;
            w3 = OneOverSix * t * t * t;
            w0 = OneOverSix * a * a * a;
            float b = w3 - w0;
            w1 = a - w0 + b;
            w2 = t - w3 - b;
        }

        public struct FineRotateParameters
        {
            public readonly double degrees;

            public FineRotateParameters(double degrees)
            {
                this.degrees = degrees;
            }
        }

        public static void ApplyFineRotation(Profile profile, string tag, ManagedBitmap bitmap, Rectangle rect, FineRotateParameters fineRotateParams, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.ApplyFineRotation {0}", tag);

            profile.Pop();
        }

        public struct BrightAdjustParameters
        {
            public readonly float MinClusterFrac;
            public readonly bool WhiteCorrect;

            public BrightAdjustParameters(float minClusterFrac, bool whiteCorrect)
            {
                this.MinClusterFrac = minClusterFrac;
                this.WhiteCorrect = whiteCorrect;
            }
        }

        public unsafe static void ApplyBrightAdjust(Profile profile, string tag, ManagedBitmap bitmap, Rectangle rect, BrightAdjustParameters brightAdjustParams, AddMessageMethod addMessage, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.ApplyBrightAdjust {0}", tag);

            if (rect.IsEmpty)
            {
                rect = new Rectangle(Point.Empty, bitmap.Size);
            }

            profile.Push("Build Histogram");
            int s;
            float top;
            int total;
            {
                const int NumBins = 100;
                const float NumBinsF = NumBins;
                Histogram hist = new Histogram(NumBins/*over*/, NumBins);

                const float insetFrac = .1f; // small inset to reduce influence of non-page borders (which we hope to trim later)
                int insetWidth = (int)(insetFrac * rect.Width);
                int insetHeight = (int)(insetFrac * rect.Height);
                Rectangle insetRect = new Rectangle(
                    rect.Left + insetWidth,
                    rect.Top + insetHeight,
                    rect.Width - 2 * insetWidth,
                    rect.Height - 2 * insetHeight);

                Parallel.For<Histogram>( // for (int y = insetRect.Top; y < insetRect.Bottom; y++)
                    insetRect.Top,
                    insetRect.Bottom,
                    Program.GetProcessorConstrainedParallelOptions2(cancel.Token),
                    delegate ()
                    {
                        return hist.CreateCompatibleHistogram();
                    },
                    delegate (int y, ParallelLoopState pls, Histogram localHist)
                    {
                        int* row = (int*)bitmap.Row(y);
                        int end = insetRect.Right;
                        for (int x = insetRect.Left; x < end; x++)
                        {
                            //ColorRGB rgb = new ColorRGB(row[x]);
                            //ColorHSV hsv = new ColorHSV(rgb);
                            //float v = hsv.v;
                            // HSV v is just Max(R, G, B)
                            //
                            //float v = ColorRGB.RgbMaxF(*pp);
                            //hist.Add((int)Math.Round(v * NumBinsF));
                            int vi = ColorRGB.RgbMaxI(row[x]);
                            int h = (vi * NumBins + 127) / 255;
                            localHist.Add(h);
                        }
                        return localHist;
                    },
                    delegate (Histogram localHist)
                    {
                        hist.Incorporate(localHist);
                    });

                total = hist.Total;
                int mi, mc;
                hist.GetMode(out mi, out mc);
                s = hist.SumAroundRadius(mi, 8);
                // too aggressive - makes light pencil illegible:
                //while ((mi >= 0) && hist.Bins[mi] >= mc / 2)
                //{
                //    mi--;
                //}
                top = (float)mi / NumBinsF;
                float cutoffPercentile = hist.BinPercentile(mi);
            }
            profile.Pop();

            if ((float)s / total > brightAdjustParams.MinClusterFrac)
            {
                profile.Push("Adjust Image");

                const float bottom = 0;
                // TODO: find closed form of this
                float pl = .01f, ph = 100f;
                float power = Single.NaN;
                while (ph - pl > .01f)
                {
                    power = (pl + ph) / 2;
                    float v1 = top / 2;
                    float v2 = (float)Math.Pow(.5, power);
                    if (v1 > v2)
                    {
                        ph = power;
                    }
                    else
                    {
                        pl = power;
                    }
                }

                const int PowerGranularity = 1024;
                float[] powers = new float[PowerGranularity + 1];
                for (int i = 0; i < powers.Length; i++)
                {
                    powers[i] = (float)Math.Pow((float)i / PowerGranularity, power);
                }

                Parallel.For( // for (int y = rect.Top; y < rect.Bottom; y++)
                    rect.Top,
                    rect.Bottom,
                    Program.GetProcessorConstrainedParallelOptions2(cancel.Token),
                    delegate (int y)
                    {
                        int* row = (int*)bitmap.Row(y);
                        for (int x = rect.Left; x < rect.Right; x++)
                        {
                            if (brightAdjustParams.WhiteCorrect)
                            {
                                // full [slow] version
                                ColorRGB rgb = new ColorRGB(row[x]);
                                ColorHSV hsv = new ColorHSV(rgb);
                                if (brightAdjustParams.WhiteCorrect)
                                {
                                    const float SaturationCutoff = .10f;
                                    const float TransitionWindow = .05f;
                                    float c;
                                    if (hsv.V >= top)
                                    {
                                        c = SaturationCutoff;
                                    }
                                    else if (hsv.V >= top - TransitionWindow)
                                    {
                                        c = SaturationCutoff * (hsv.V - (top - TransitionWindow)) / TransitionWindow;
                                    }
                                    else
                                    {
                                        c = 0;
                                    }
                                    hsv.S = Math.Max(hsv.S - c, 0);
                                }
                                // makes color ink too blury:
                                // lighten unsaturated (paper), keep saturated (pen) dark - potential cost is loss of some crispness
                                //float v = hsv.v;
                                //float v1 = Math.Min(Math.Max((hsv.v - bottom) / (top - bottom), 0), 1);
                                //hsv.v = v1 * (1 - hsv.s) + v * hsv.s;
                                hsv.V = Math.Min(Math.Max((hsv.V - bottom) / (top - bottom), 0), 1);
                                hsv.V = (float)Math.Pow(hsv.V, power);
                                rgb = hsv.ToRGB();
                                row[x] = rgb.ToRGB32();
                            }
                            else
                            {
                                // fast version
                                float vOrig = ColorRGB.RgbMaxF(row[x]);
                                if (vOrig != 0)
                                {
                                    float v = vOrig;
                                    v = Math.Min(Math.Max((v - bottom) / (top - bottom), 0), 1);
                                    //v = (float)Math.Pow(v, power);
                                    v = powers[(int)(v * PowerGranularity + .5f)];
                                    row[x] = ColorRGB.RgbScale(row[x], v / vOrig);
                                }
                            }
                        }
                    });

                profile.Pop();
            }
            else
            {
                string message = String.Format("Bright Adjust not done - histogram peak not compact (was {0:g3}); try reducing MinClusterFrac below value.", (float)s / total);
                addMessage?.Invoke(message);
                Program.Log(LogCat.Xform, String.Format(
                    "{1}: {2}){0}",
                    Environment.NewLine,
                    tag,
                    message));
            }

            profile.Pop();
        }

        public struct PolyUnbiasParameters
        {
            public readonly int MaxDegree;
            public readonly float MaxChiSq;

            public PolyUnbiasParameters(int maxDegree, float maxChiSq)
            {
                this.MaxDegree = maxDegree;
                this.MaxChiSq = maxChiSq;
            }
        }

        public class PolyUnbiasDiagnostics
        {
            public readonly bool[,] grid;
            public readonly Point gridOffset;
            public readonly double[,] weights;
            public readonly double chisq;

            public PolyUnbiasDiagnostics()
            {
            }

            public PolyUnbiasDiagnostics(bool[,] grid, Point gridOffset, double[,] weights, double chisq)
            {
                this.grid = grid;
                this.gridOffset = gridOffset;
                this.weights = weights;
                this.chisq = chisq;
            }

            public PolyUnbiasDiagnostics SetGrid(bool[,] grid, Point gridOffset)
            {
                return new PolyUnbiasDiagnostics(grid, gridOffset, this.weights, this.chisq);
            }

            public PolyUnbiasDiagnostics SetWeights(double[,] weights)
            {
                return new PolyUnbiasDiagnostics(this.grid, this.gridOffset, weights, this.chisq);
            }

            public PolyUnbiasDiagnostics SetChiSq(double chisq)
            {
                return new PolyUnbiasDiagnostics(this.grid, this.gridOffset, this.weights, chisq);
            }
        }

        private const int PolyFitBlockSize = 32; // cell size for initial brightness analysis
        private const int PolyFitDataGridSize = 16; // grid axis density (X & Y) for data points sent to least-squares fit routine

        public unsafe static void ApplyPolyUnbias(Profile profile, string tag, ManagedBitmap bitmap, Rectangle rect, PolyUnbiasParameters unbiasParams, AddMessageMethod addMessage, out PolyUnbiasDiagnostics diagnostics, CancellationTokenSource cancel)
        {
            try
            {
                profile.Push("Transforms.ApplyPolyUnbias {0}", tag);

                diagnostics = new PolyUnbiasDiagnostics();

                int* scan0 = (int*)bitmap.Scan0;
                int stride = bitmap.Stride / 4;

                int imageX = 0;
                int imageY = 0;
                int imageWidth = bitmap.Width;
                int imageHeight = bitmap.Height;
                if (!rect.IsEmpty)
                {
                    imageX = rect.X;
                    imageY = rect.Y;
                    imageWidth = rect.Width;
                    imageHeight = rect.Height;
                }

                int cWidth = (imageWidth + PolyFitBlockSize - 1) / PolyFitBlockSize;
                int cHeight = (imageHeight + PolyFitBlockSize - 1) / PolyFitBlockSize;
                float[,] modeV = new float[cHeight, cWidth]; // mode brightnesses
                float[,] aS = new float[cHeight, cWidth]; // average saturation
                bool[,] compact = new bool[cHeight, cWidth]; // include in dataset (mode peak has sufficient compactness)

                const int NumBins = 100;
                const int CompactnessWindowRadius = 8; // 16% window
                const float MinClusterFrac = .5f; // half of pixels must be in the window to be considered compact

                profile.Push("Cell medians");
                {
                    Parallel.For( // for (int i = 0; i < cHeight; i++)
                        0,
                        cHeight,
                        Program.GetProcessorConstrainedParallelOptions2(cancel.Token),
                        delegate (int i)
                        {
                            for (int j = 0; j < cWidth; j++)
                            {
                                Histogram hist = new Histogram(NumBins, NumBins);

                                double aS1 = 0;
                                int lii = Math.Min((i + 1) * PolyFitBlockSize, imageHeight);
                                int ljj = Math.Min((j + 1) * PolyFitBlockSize, imageWidth);
                                int n = (lii - i * PolyFitBlockSize) * (ljj - j * PolyFitBlockSize);
                                for (int ii = i * PolyFitBlockSize; ii < lii; ii++)
                                {
                                    for (int jj = j * PolyFitBlockSize; jj < ljj; jj++)
                                    {
                                        int* pp = scan0 + (imageY + ii) * stride + (imageX + jj);
                                        int v;
                                        float s;
                                        ColorHSV.GetSV(*pp, out s, out v); // v not normalized (0..255)
                                        aS1 += s;
                                        hist.Add(v * NumBins / 255);
                                    }
                                }
                                Debug.Assert(n == hist.Total);

                                // estimate background white level by computing mode, with compactness check to attempt to exclude
                                // regions of a page that may contain photos or dark diagrams.
                                int mode, modeC;
                                hist.GetMode(out mode, out modeC);
                                modeV[i, j] = mode * 255 / NumBins;
                                // compactness check
                                int binSum = hist.SumAroundRadius(mode, CompactnessWindowRadius);
                                if ((float)binSum / n > MinClusterFrac)
                                {
                                    compact[i, j] = true;
                                }

                                // saturation
                                aS[i, j] = (float)(aS1 / n);
                            }
                        });
                }
                diagnostics = diagnostics.SetGrid((bool[,])compact.Clone(), new Point(imageX, imageY));
                profile.Pop();

                // TODO: add these to options
                const float MaxS = .10f;
                const float MinModeV = .35f;
                const bool ApproximatelyPreserveOverallPageBrightness = true;

                profile.Push("Overall median");
                double[,] cw = new double[PolyFitDataGridSize, PolyFitDataGridSize];
                diagnostics = diagnostics.SetWeights(cw);
                PolyFit.XX[,] pos = new PolyFit.XX[PolyFitDataGridSize, PolyFitDataGridSize];
                float[] medcw = new float[PolyFitDataGridSize * PolyFitDataGridSize];
                int medcwi = 0;
                for (int i = 0; i < PolyFitDataGridSize; i++)
                {
                    for (int j = 0; j < PolyFitDataGridSize; j++)
                    {
                        double sumv = 0;
                        double sx = 0, sy = 0;
                        int n = 0;
                        for (int ii = i * cHeight / PolyFitDataGridSize; ii < (i + 1) * cHeight / PolyFitDataGridSize; ii++)
                        {
                            for (int jj = j * cWidth / PolyFitDataGridSize; jj < (j + 1) * cWidth / PolyFitDataGridSize; jj++)
                            {
                                // only ones with compact mode distribution and only use unsaturated values (to a threshhold)
                                if (compact[ii, jj] && (aS[ii, jj] <= MaxS) && (modeV[ii, jj] >= MinModeV))
                                {
                                    sumv += modeV[ii, jj];
                                    sx += jj;
                                    sy += ii;
                                    n++;
                                }
                                else
                                {
                                    diagnostics.grid[ii, jj] = false;
                                }
                            }
                        }
                        if (n > 0)
                        {
                            cw[i, j] = (float)(sumv / (255f * n));
                            medcw[medcwi++] = (float)cw[i, j];
                            pos[i, j] = new PolyFit.XX(sx / (cWidth * n), sy / (cHeight * n));
                        }
                        else
                        {
                            cw[i, j] = Double.NaN;
                        }
                    }
                }
                profile.Pop();

                const float MinDataPointFrac = .5f; // at least this fraction of points for fit must be defined
                if (medcwi < PolyFitDataGridSize * PolyFitDataGridSize * MinDataPointFrac)
                {
                    string message = "Unbias not done: unable to obtain minimum number of datapoints for fit.";
                    addMessage?.Invoke(message);
                    Program.Log(LogCat.Xform, String.Format(
                        "{1}: {2}{0}",
                        Environment.NewLine,
                        tag,
                        message));
                    return;
                }
                bool ul = false, ur = false, bl = false, br = false;
                for (int i = 0; i < PolyFitDataGridSize / 3; i++)
                {
                    for (int j = 0; j < PolyFitDataGridSize / 3; j++)
                    {
                        ul = ul || !Double.IsNaN(cw[i, j]);
                        ur = ur || !Double.IsNaN(cw[PolyFitDataGridSize - 1 - i, j]);
                        bl = bl || !Double.IsNaN(cw[i, PolyFitDataGridSize - 1 - j]);
                        br = br || !Double.IsNaN(cw[PolyFitDataGridSize - 1 - i, PolyFitDataGridSize - 1 - j]);
                    }
                }
                if (!ul || !ur || !bl || !br)
                {
                    string message = "Unbias not done: datapoints for fit not sufficiently distributed across image.";
                    addMessage?.Invoke(message);
                    Program.Log(LogCat.Xform, String.Format(
                        "{1}: {2}{0}",
                        Environment.NewLine,
                        tag,
                        message));
                    return;
                }

                Array.Sort(medcw, 0, medcwi);
                float medcwv = ((medcwi & 1) != 0) ? medcw[medcwi / 2] : (.5f * (medcw[medcwi / 2 - 1] + medcw[medcwi / 2]));

                profile.Push("Polyfit");
                PolyFit.Poly2D poly;
                PolyFit.Poly2D lastPoly;
                {
                    const int degree = 4;
                    PolyFit.Poly2DFactory polyfac = new PolyFit.Poly2DFactory(degree);
                    lastPoly = poly = PolyFit.PolyFit2D(polyfac, PolyFitDataGridSize, PolyFitDataGridSize, cw, pos);
                    if (poly.chiSq > unbiasParams.MaxChiSq)
                    {
                        poly = null;
                    }
                }
                if ((poly == null) && (unbiasParams.MaxDegree >= 6))
                {
                    const int degree = 6;
                    PolyFit.Poly2DFactory polyfac = new PolyFit.Poly2DFactory(degree);
                    lastPoly = poly = PolyFit.PolyFit2D(polyfac, PolyFitDataGridSize, PolyFitDataGridSize, cw, pos);
                    if (poly.chiSq > unbiasParams.MaxChiSq)
                    {
                        poly = null;
                    }
                }
                if ((poly == null) && (unbiasParams.MaxDegree >= 8))
                {
                    const int degree = 8;
                    PolyFit.Poly2DFactory polyfac = new PolyFit.Poly2DFactory(degree);
                    lastPoly = poly = PolyFit.PolyFit2D(polyfac, PolyFitDataGridSize, PolyFitDataGridSize, cw, pos);
                    if (poly.chiSq > unbiasParams.MaxChiSq)
                    {
                        poly = null;
                    }
                }
                diagnostics = diagnostics.SetChiSq(lastPoly.chiSq);
                profile.Pop();
                profile.Add(new Stopwatch(), "[Degree = {0}, ChiSq = {1}/{2}]", poly != null ? poly.factory.degree.ToString() : "failed", lastPoly.chiSq, unbiasParams.MaxChiSq);

                const float ModeToPeakTop = 1 + (float)CompactnessWindowRadius / NumBins;
                float baseline = ApproximatelyPreserveOverallPageBrightness ? medcwv : 1 / ModeToPeakTop;
                if (poly != null)
                {
                    profile.Push("Generate inverse bias");
                    float[,] points = new float[cHeight + 1, cWidth + 1];
                    Parallel.For( // for (int i = 0; i < cHeight + 1; i++)
                        0,
                        cHeight + 1,
                        Program.GetProcessorConstrainedParallelOptions2(cancel.Token),
                        delegate (int i)
                        {
                            float y = (i * PolyFitBlockSize) / (float)imageHeight;
                            float jf = 0;
                            float fImageWidthInv = 1f / imageWidth;
                            for (int j = 0; j <= cWidth; j++, jf++)
                            {
                                float x = (jf * PolyFitBlockSize) * fImageWidthInv;
                                points[i, j] = (float)poly.Eval(x, y);
                            }
                        });
                    profile.Pop();

                    profile.Push("Adjust image");
                    Parallel.For( // for (int i = 0; i < imageHeight; i += BlockSize)
                        0,
                        (imageHeight + PolyFitBlockSize - 1) / PolyFitBlockSize,
                        Program.GetProcessorConstrainedParallelOptions2(cancel.Token),
                        delegate (int id)
                        {
                            int i = id * PolyFitBlockSize;
                            float y = (float)i / imageHeight;
                            for (int j = 0; j < imageWidth; j += PolyFitBlockSize)
                            {
                                float v00 = points[i / PolyFitBlockSize + 0, j / PolyFitBlockSize + 0];
                                float v01 = points[i / PolyFitBlockSize + 1, j / PolyFitBlockSize + 0];
                                float v10 = points[i / PolyFitBlockSize + 0, j / PolyFitBlockSize + 1];
                                float v11 = points[i / PolyFitBlockSize + 1, j / PolyFitBlockSize + 1];
                                int il = Math.Min(i + PolyFitBlockSize, imageHeight);
                                int jl = Math.Min(j + PolyFitBlockSize, imageWidth);
                                for (int ii = i; ii < il; ii++)
                                {
                                    float ywr = (float)(ii - i) / PolyFitBlockSize;
                                    float v0 = v00 + ywr * (v01 - v00);
                                    float v1 = v10 + ywr * (v11 - v10);
                                    float v1m0 = v1 - v0;
                                    const float InvBlockSize = 1f / PolyFitBlockSize;
                                    for (int jj = j; jj < jl; jj++)
                                    {
                                        float xwr = (jj - j) * InvBlockSize;
                                        float v = v0 + xwr * v1m0;
                                        float f = baseline / v;

                                        int* pp = scan0 + (imageY + ii) * stride + (imageX + jj);
                                        *pp = ColorRGB.RgbScale(*pp, f); // shortcut to scale V by f
                                    }
                                }
                            }
                        });
                    profile.Pop();
                }
                else
                {
                    string message = String.Format("Unbias not done: chisq {0:g5} exceeds cutoff.", lastPoly.chiSq);
                    addMessage?.Invoke(message);
                    Program.Log(LogCat.Xform, String.Format(
                        "{1}: {2}{0}",
                        Environment.NewLine,
                        tag,
                        message));
                    return;
                }
            }
            finally
            {
                profile.Pop();
            }
        }

        public struct OneBitParameters
        {
            public readonly Channel Channel;
            public readonly float Threshhold;
            public readonly bool ScaleUp;

            public OneBitParameters(Channel channel, float threshhold, bool scaleUp)
            {
                this.Channel = channel;
                this.Threshhold = threshhold;
                this.ScaleUp = scaleUp;
            }
        }

        public unsafe static ManagedBitmap1 ApplyOneBit(Profile profile, string tag, SmartBitmap _bitmap, OneBitParameters oneBitParams, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.ApplyOneBit {0}", tag);

            ManagedBitmap1 monoImage = null;

            Stopwatch swDraw = new Stopwatch();
            Stopwatch swConvert = new Stopwatch();
            Stopwatch swTwiddle = new Stopwatch();

            int tileCount = 1;
            try
            {
                if (oneBitParams.ScaleUp)
                {
                    swConvert.Start();
                    Bitmap bitmap = _bitmap.AsGDI();
                    swConvert.Stop();

                    swDraw.Start();
                    const int ExpansionFactor = 2; // hq-interpolating to 2x and truncating gives improved smoothness
                    monoImage = new ManagedBitmap1(bitmap.Size.Width * ExpansionFactor, bitmap.Size.Height * ExpansionFactor);

                    const int Margin = 8;
                    const int MaxTileExtent = 5000 & ~7; // require byte multiple to permit optimizations
                    // minimize tile count without using excessive amounts of memory and without wasting space at the margins
                    int tileWidth;
                    int i = 1;
                    do
                    {
                        tileWidth = ((bitmap.Width * ExpansionFactor + i - 1) / i) & ~7;
                        i++;
                    }
                    while (tileWidth > MaxTileExtent);
                    i = 1;
                    int tileHeight;
                    do
                    {
                        tileHeight = ((bitmap.Height * ExpansionFactor + i - 1) / i) & ~7;
                        i++;
                    }
                    while (tileHeight > MaxTileExtent);

                    // TODO: switch to server (low pri since ScaleUp mode is seldom used)

                    using (Bitmap sourceImageTileGDI = new Bitmap(tileWidth + 2 * Margin, tileHeight + 2 * Margin))
                    {
                        swDraw.Stop();

                        tileCount = 0;
                        for (int yy = 0; yy < monoImage.Height; yy += tileHeight)
                        {
                            cancel.Token.ThrowIfCancellationRequested();

                            int yl = Math.Min(monoImage.Height, yy + tileHeight);
                            int yc = yl - yy;
                            for (int xx = 0; xx < monoImage.Width; xx += tileWidth)
                            {
                                tileCount++;

                                int xl = Math.Min(monoImage.Width, xx + tileWidth);
                                int xc = xl - xx;
                                swDraw.Start();
                                using (Graphics graphics = Graphics.FromImage(sourceImageTileGDI))
                                {
                                    //graphics.FillRectangle(System.Drawing.Brushes.Black, new Rectangle(Point.Empty, sourceImageTileGDI.Size));
                                    graphics.DrawImage(bitmap, new Rectangle(-xx, -yy, monoImage.Width, monoImage.Height));
                                }
                                swDraw.Stop();

                                swConvert.Start();
                                using (ManagedBitmap sourceImageTile = new ManagedBitmap32(sourceImageTileGDI))
                                {
                                    swConvert.Stop();

                                    swTwiddle.Start();
                                    ApplyOneBitTile(sourceImageTile, monoImage, xc, yc, xx, yy, oneBitParams, cancel);
                                    swTwiddle.Stop();
                                }
                            }
                        }

                        ManagedBitmap1 result = monoImage;
                        monoImage = null;
                        return result;
                    }
                }
                else
                {
                    swConvert.Start();
                    ManagedBitmap bitmap = _bitmap.AsManaged();
                    swConvert.Stop();

                    swDraw.Start();
                    monoImage = new ManagedBitmap1(bitmap.Width, bitmap.Height);
                    swDraw.Stop();

                    swTwiddle.Start();
                    ApplyOneBitTile(bitmap, monoImage, bitmap.Width, bitmap.Height, 0, 0, oneBitParams, cancel);
                    swTwiddle.Stop();

                    ManagedBitmap1 result = monoImage;
                    monoImage = null;
                    return result;
                }
            }
            finally
            {
                if (monoImage != null)
                {
                    monoImage.Dispose();
                }
                else
                {
                    profile.Add(swDraw, "Scale 2x (GDI)");
                    profile.Add(swConvert, "Convert GDI/Managed");
                    profile.Add(swTwiddle, "Set Bits");
                    profile.Add(new Stopwatch(), "(Tile Count: {0})", tileCount);
                }

                profile.Pop();
            }
        }

        private unsafe static void ApplyOneBitTile(ManagedBitmap sourceImageTile, ManagedBitmap monoImage, int xc, int yc, int xx, int yy, OneBitParameters oneBitParams, CancellationTokenSource cancel)
        {
            int threshhold = (int)Math.Round(oneBitParams.Threshhold * 255 * (oneBitParams.Channel == Channel.Composite ? 3 : 1));

            int rgbMask = ColorRGB.MakeRGBMask(
                (oneBitParams.Channel & Channel.R) != 0,
                (oneBitParams.Channel & Channel.G) != 0,
                (oneBitParams.Channel & Channel.B) != 0);

            for (int y = 0; y < yc; y++)
            {
                cancel.Token.ThrowIfCancellationRequested();

                int* p = (int*)sourceImageTile.Row(y);
                byte* row = monoImage.Row(y + yy);
                for (int xo = 0; xo < xc; xo += 8)
                {
                    int xil = Math.Min(xc - xo, 8);
                    int acc = 0;
                    int bit = 0x80;
                    for (int xi = 0; xi < xil; xi++)
                    {
                        int x = xo + xi;
                        int rgb = *(p++); // sourceImageTile[x, y]
                        int r, g, b;
                        ColorRGB.GetRGB(rgb & rgbMask, out r, out g, out b);
                        int sum = r + g + b;
                        bool w = sum > threshhold;
                        if (w)
                        {
                            acc |= bit;
                        }
                        bit >>= 1;
                    }
                    row[(xx + xo) / 8] = (byte)acc;
                }
            }
        }

        public struct StaticSaturateParameters
        {
            public readonly float WhiteThreshhold;
            public readonly float BlackThreshhold;
            public readonly float Exponent;

            public StaticSaturateParameters(float whiteThreshhold, float blackThreshhold, float exponent)
            {
                this.WhiteThreshhold = whiteThreshhold;
                this.BlackThreshhold = blackThreshhold;
                this.Exponent = exponent;
            }
        }

        public unsafe static void ApplyStaticSaturation(Profile profile, string tag, ManagedBitmap bitmap, Rectangle rect, StaticSaturateParameters staticSatParams, CancellationTokenSource cancel)
        {
            profile.Push("Transforms.ApplyStaticSaturation");

            if (!(bitmap is ManagedBitmap32))
            {
                Debug.Assert(false);
                throw new ArgumentException();
            }

            int xl, xh, yl, yh;
            if (rect.IsEmpty)
            {
                xl = 0;
                yl = 0;
                xh = bitmap.Width;
                yh = bitmap.Height;
            }
            else
            {
                xl = rect.Left;
                xh = rect.Right;
                yl = rect.Top;
                yh = rect.Bottom;
            }

            float l = staticSatParams.BlackThreshhold;
            float h = staticSatParams.WhiteThreshhold;
            float ir = 1 / (h - l);
            for (int y = yl; y < yh; y++)
            {
                cancel.Token.ThrowIfCancellationRequested();

                int* row = (int*)bitmap.Row(y);
                if (staticSatParams.Exponent == 1)
                {
                    for (int x = xl; x < xh; x++)
                    {
                        ColorRGB rgb = new ColorRGB(row[x]);
                        rgb = new ColorRGB(
                            (rgb.R - l) * ir,
                            (rgb.G - l) * ir,
                            (rgb.B - l) * ir);
                        row[x] = rgb.ToRGB32();
                    }
                }
                else
                {
                    double exponent = staticSatParams.Exponent;
                    for (int x = xl; x < xh; x++)
                    {
                        ColorRGB rgb = new ColorRGB(row[x]);
                        ColorHSV hsv = new ColorHSV(rgb);
                        hsv.V = (float)Math.Pow((hsv.V - l) * ir, exponent);
                        rgb = hsv.ToRGB();
                        row[x] = rgb.ToRGB32();
                    }
                }
            }

            profile.Pop();
        }

        public static void RotatePoints(int width, int height, int rightRotations, Point[] points)
        {
            for (int r = 0; r < rightRotations; r++)
            {
                // for one 90-degree clockwise rotation, given w x h, (x,y) ==> (h-y,x)
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = new Point(height - points[i].Y, points[i].X);
                }
                int t = width;
                width = height;
                height = t;
            }
        }

        public static Rectangle RotateRect(int width, int height, int rightRotations, Rectangle rect)
        {
            Debug.Assert(rightRotations >= 0);
            Point[] points = new Point[] { new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Bottom) };
            RotatePoints(width, height, rightRotations, points);
            Point tl = Point.Empty, br = Point.Empty;
            if (points[0].X < points[1].X)
            {
                tl.X = points[0].X;
                br.X = points[1].X;
            }
            else
            {
                tl.X = points[1].X;
                br.X = points[0].X;
            }
            if (points[0].Y < points[1].Y)
            {
                tl.Y = points[0].Y;
                br.Y = points[1].Y;
            }
            else
            {
                tl.Y = points[1].Y;
                br.Y = points[0].Y;
            }
            return new Rectangle(tl, new Size(br.X - tl.X, br.Y - tl.Y));
        }

        public const int LosslessCropGranularity = 16;

        public static int SnapLossless(int pos)
        {
            return ((pos + Math.Sign(pos) * Transforms.LosslessCropGranularity / 2) / Transforms.LosslessCropGranularity)
                * Transforms.LosslessCropGranularity;
        }

        public static bool LosslessCropFinal(string sourcePath, string targetPath, Rectangle cropRect, out string error)
        {
            error = null;

            bool success = false;
            using (Process cmd = new Process())
            {
                cmd.StartInfo.Arguments = String.Join(
                    " ",
                    "-optimize",
                    "-copy all",
                    String.Format("-crop {0}x{1}+{2}+{3}", cropRect.Width, cropRect.Height, cropRect.X, cropRect.Y),
                    String.Format("\"{0}\"", sourcePath),
                    String.Format("\"{0}\"", targetPath));
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "jpegtran.exe");
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = Path.GetTempPath();

                cmd.Start();
                cmd.WaitForExit();

                success = true;
                if (cmd.ExitCode != 0)
                {
                    error = String.Format("For {0}, jpegtran.exe returned {1}", Path.GetFileName(sourcePath), cmd.ExitCode);
                    success = false;
                }
            }
            return success;
        }

        public readonly static ImageCodecInfo JpegEncoder = GetEncoder(ImageFormat.Jpeg);
        public readonly static ImageCodecInfo BmpEncoder = GetEncoder(ImageFormat.Bmp);
        public readonly static ImageCodecInfo PngEncoder = GetEncoder(ImageFormat.Png);
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        // Save image using available/best encoder for requested parameters
        public static void SaveImage(Profile profile, SmartBitmap source, string path, int jpegQuality, bool jpegUseGdi, OutputTransforms.OutputFormat format)
        {
            profile.Push(String.Format("SaveImage jpegQ={0}, format={1}", jpegQuality, format));

            switch (source.ImageFormat)
            {
                default:
                    Debug.Assert(false);
                    throw new NotSupportedException(String.Format("Image format {0}", source.ImageFormat));

                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    if (jpegUseGdi)
                    {
                        // GDI+:
                        // (uses chroma subsampling which is worse for high-color images)
                        if (format != OutputTransforms.OutputFormat.Jpeg)
                        {
                            Debug.Assert(false);
                            throw new ArgumentException();
                        }
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)jpegQuality);
                        source.AsGDI().Save(path, JpegEncoder, encoderParameters);
                    }
                    else
                    {
                        // WPF:
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(source.AsWPF());
                        encoder.QualityLevel = jpegQuality;
                        using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        {
                            encoder.Save(stream);
                        }
                    }
                    break;

                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    switch (format)
                    {
                        default:
                            Debug.Assert(false);
                            throw new ArgumentException();
                        case OutputTransforms.OutputFormat.Bmp:
                            source.AsGDI().Save(path, BmpEncoder, null);
                            break;
                        case OutputTransforms.OutputFormat.Png:
                            source.AsGDI().Save(path, PngEncoder, null);
                            break;
                    }
                    break;
            }

            profile.Pop();
        }

        public static Bitmap LoadAndOrientGDI(string path, Profile profile = null) // GDI+
        {
            profile?.Push("Transforms.LoadAndOrientGDI {0}", Path.GetFileName(path));

            SanityCheckValidImageFormatFileThrow(path);

            profile?.Push("Load");
            Bitmap bitmap = new Bitmap(ReadAllBytes(path)); // could throw if file is invalid despite preceding sanity check
            profile?.Pop();

            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {
                profile?.Push("Ensure 32-bit depth");
                Bitmap bitmap2 = bitmap.Clone(new Rectangle(Point.Empty, bitmap.Size), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                bitmap.Dispose();
                bitmap = bitmap2;
                profile?.Pop();
            }

            profile?.Push("Orient");
            bitmap.RotateFlip(ReadOrientationProperty(bitmap));
            profile?.Pop();

            profile?.Pop();

            return bitmap;
        }

        public static ManagedBitmap LoadAndOrientManaged(Profile profile, string path, int? optionalRightRotations, out PropertyItem[] properties, out RotateFlipType exifOrientationProperty)
        {
            profile.Push("Transforms.LoadAndOrientManaged {0}", Path.GetFileName(path));

            ManagedBitmap managed;
            List<PropertyItem> propertiesList = new List<PropertyItem>();

            RotateFlipType orientationProperty;

            profile.Push("Load");
            SanityCheckValidImageFormatFileThrow(path);
            using (Bitmap bitmap = new Bitmap(ReadAllBytes(path))) // could throw if file is invalid despite preceding sanity check
            {
                orientationProperty = exifOrientationProperty = ReadOrientationProperty(bitmap);
                try
                {
                    foreach (PropertyItem property in bitmap.PropertyItems)
                    {
                        if (property.Id == OrientationPropertyId)
                        {
                            continue;
                        }
                        propertiesList.Add(property);
                    }
                }
                catch (ArgumentException)
                {
                    // properties not supported
                }
                profile.Pop();

                profile.Push("Convert to Managed");
                switch (bitmap.PixelFormat)
                {
                    default:
                        Debug.Assert(false);
                        throw new ArgumentException();
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                        managed = new ManagedBitmap32(bitmap);
                        break;
                    case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                        managed = new ManagedBitmap1(bitmap);
                        break;
                    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                        managed = new ManagedBitmap32(bitmap);
                        break;
                }
                profile.Pop();

                if (!(managed is ManagedBitmap32))
                {
                    profile.Push("Ensure 32-bit depth");
                    ManagedBitmap32 managed2 = new ManagedBitmap32(managed.Width, managed.Height);
                    managed.CopyTo(managed2);
                    managed.Dispose();
                    managed = managed2;
                    profile.Pop();
                }
            }

            if (optionalRightRotations.HasValue)
            {
                for (int i = 0; i < optionalRightRotations.Value; i++)
                {
                    orientationProperty = Rotation[(int)orientationProperty];
                }
            }

            if (orientationProperty != RotateFlipType.RotateNoneFlipNone)
            {
                profile.Push("Ensure Orientation");
                ManagedBitmap managed2;
                using (managed)
                {
                    managed2 = managed.RotateFlip(orientationProperty);
                }
                managed = managed2;
                profile.Pop();
            }

            profile.Pop();

            properties = propertiesList.ToArray();
            return managed;
        }

        private readonly static RotateFlipType[] Rotation = new RotateFlipType[8]
        {
            RotateFlipType.Rotate90FlipNone, // from [0] RotateNoneFlipNone
            RotateFlipType.Rotate180FlipNone, // from [1] Rotate90FlipNone
            RotateFlipType.Rotate270FlipNone, // from [2] Rotate180FlipNone
            RotateFlipType.RotateNoneFlipNone, // from [3] Rotate270FlipNone
            RotateFlipType.Rotate90FlipY, // from [4] RotateNoneFlipX
            RotateFlipType.Rotate180FlipY, // from [5] Rotate90FlipX
            RotateFlipType.Rotate270FlipY, // from [6] Rotate180FlipX
            RotateFlipType.RotateNoneFlipY, // from [7] Rotate270FlipX
        };

        public static RotateFlipType ReadOrientationProperty(Bitmap bitmap)
        {
            try
            {
                PropertyItem orientation = bitmap.GetPropertyItem(OrientationPropertyId);
                if (orientation != null)
                {
                    switch (orientation.Value[0])
                    {
                        case 1:
                            return RotateFlipType.RotateNoneFlipNone;
                        case 2:
                            return RotateFlipType.RotateNoneFlipX;
                        case 3:
                            return RotateFlipType.Rotate180FlipNone;
                        case 4:
                            return RotateFlipType.Rotate180FlipX;
                        case 5:
                            return RotateFlipType.Rotate90FlipX;
                        case 6:
                            return RotateFlipType.Rotate90FlipNone;
                        case 7:
                            return RotateFlipType.Rotate270FlipX;
                        case 8:
                            return RotateFlipType.Rotate270FlipNone;
                    }
                }
            }
            catch (ArgumentException)
            {
                // properties not supported in this image format
            }
            return RotateFlipType.RotateNoneFlipNone;
        }

        public static BitmapFrame LoadWPF(string path) // WPF
        {
            BitmapDecoder decoder = new JpegBitmapDecoder(
                ReadAllBytes(path),
                BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache | BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.None);
            return decoder.Frames[0];
        }

        public static void RotateRight(Bitmap image, int count) // GDI+
        {
            switch (count)
            {
                default:
                    Debug.Assert(false);
                    break;
                case 0:
                    break;
                case 1:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }

        public static void RotateRight(ref BitmapFrame image, int count) // WPF
        {
            if (count != 0)
            {
                TransformedBitmap rotatedTransform = new TransformedBitmap(image, new RotateTransform(90 * count));
                image = BitmapFrame.Create(rotatedTransform);
            }
        }

        public static RotateFlipType RotateFlipFromRightRotations(int rotations)
        {
            switch (rotations)
            {
                default:
                    Debug.Assert(false);
                    throw new ArgumentException(String.Format("rotations {0} is not supported", rotations));
                case 0:
                    return RotateFlipType.RotateNoneFlipNone;
                case 1:
                    return RotateFlipType.Rotate90FlipNone;
                case 2:
                    return RotateFlipType.Rotate180FlipNone;
                case 3:
                    return RotateFlipType.Rotate270FlipNone;
            }
        }

        public static int RightRotationsFromRotateFlipType(RotateFlipType rotations)
        {
            switch (rotations)
            {
                default:
                    Debug.Assert(false);
                    throw new ArgumentException(String.Format("RotateFlipType.{0} is not supported", rotations));
                case RotateFlipType.RotateNoneFlipNone:
                    return 0;
                case RotateFlipType.Rotate90FlipNone:
                    return 1;
                case RotateFlipType.Rotate180FlipNone:
                    return 2;
                case RotateFlipType.Rotate270FlipNone:
                    return 3;
            }
        }

        public static bool LosslessRotateRightFinal(string sourcePath, string targetPath, int rightRotations, out string error)
        {
            error = null;

            // always rotate even if combined rotation is 0, because we need to strip the EXIF orientation property.

            //if (rightRotations == 0)
            //{
            //    if (!String.Equals(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase))
            //    {
            //        File.Copy(sourcePath, targetPath, true/*overwrite*/);
            //    }
            //    return true;
            //}

            using (Process cmd = new Process())
            {
                cmd.StartInfo.Arguments = String.Join(
                    " ",
                    "-optimize",
                    // TODO: strip only Exif orientation after doing this - how? (currently strips all)
                    "-copy comments", // to remove orientation property we must blow away all properties, sorry about it
                    String.Format("-rotate {0}", rightRotations * 90),
                    String.Format("\"{0}\"", sourcePath),
                    String.Format("\"{0}\"", targetPath));
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "jpegtran.exe");
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = Path.GetTempPath();

                cmd.Start();
                cmd.WaitForExit();

                if (cmd.ExitCode != 0)
                {
                    error = String.Format("For {0}, jpegtran.exe returned {1}", Path.GetFileName(sourcePath), cmd.ExitCode);
                    return false;
                }
            }

            return true;
        }

        public static void Resize(Profile profile, ManagedBitmap source, ManagedBitmap target) // GDI
        {
            profile.Push("Resize [GDI]");

            Debug.Assert((source is ManagedBitmap32) && (target is ManagedBitmap32));

            profile.Push("Allocate");
            using (Bitmap gdiSource = new Bitmap(source.Width, source.Height, source.Stride, source.ImageFormat, source.Scan0I)) // borrow
            {
                using (Bitmap gdiTarget = new Bitmap(target.Width, target.Height, target.Stride, target.ImageFormat, target.Scan0I)) // borrow
                {
                    profile.Pop();

                    profile.Push("Resize");
                    using (Graphics graphics = Graphics.FromImage(gdiTarget))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.DrawImage(gdiSource, 0, 0, target.Width, target.Height);
                    }
                    profile.Pop();
                }
            }

            profile.Pop();
        }

        public static BitmapFrame Resize(Profile profile, BitmapFrame image, int resizedWidth, int resizedHeight) // WPF
        {
            profile.Push("Resize [WPF]");

            double shrinkX = (double)resizedWidth / image.PixelWidth;
            double shrinkY = (double)resizedHeight / image.PixelHeight;

            TransformedBitmap resizeTransform = new TransformedBitmap(image, new ScaleTransform(shrinkX, shrinkY));
            BitmapFrame result = BitmapFrame.Create(resizeTransform);

            profile.Pop();

            return result;
        }

        public static void ShrinkExpand(Profile profile, ManagedBitmap source, double factor, ManagedBitmap target) // GDI
        {
            profile.Push("ShrinkExpand [GDI]");

            Debug.Assert((source.Width == target.Width) && (source.Height == target.Height));
            Debug.Assert((source is ManagedBitmap32) && (target is ManagedBitmap32));
            int expandedWidth = source.Width;
            int expandedHeight = source.Height;

            profile.Push("Allocate intermediates");
            using (Bitmap gdiSource = new Bitmap(expandedWidth, expandedHeight, source.Stride, source.ImageFormat, source.Scan0I)) // borrow
            {
                using (Bitmap gdiTarget = new Bitmap(expandedWidth, expandedHeight, target.Stride, target.ImageFormat, target.Scan0I)) // borrow
                {
                    profile.Pop();

                    profile.Push("Shrink");
                    int shrunkWidth = (int)Math.Floor(expandedWidth / factor);
                    int shrunkHeight = (int)Math.Floor(expandedHeight / factor);
                    using (Bitmap gdiShrunk = new Bitmap(gdiSource, shrunkWidth, shrunkHeight))
                    {
                        profile.Pop();

                        profile.Push("Expand");
                        using (Graphics graphics = Graphics.FromImage(gdiTarget))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.DrawImage(gdiShrunk, 0, 0, expandedWidth, expandedHeight);
                        }
                        profile.Pop();
                    }
                }
            }

            profile.Pop();
        }

        public static BitmapFrame ShrinkExpand(Profile profile, BitmapFrame image, double factor) // WPF
        {
            profile.Push("ShrinkExpand [WPF]");

            double shrinkX = Math.Floor(image.PixelWidth / factor) / image.PixelWidth;
            double shrinkY = Math.Floor(image.PixelHeight / factor) / image.PixelHeight;

            profile.Push("Shrink [WPF]");
            TransformedBitmap shrinkTransform = new TransformedBitmap(image, new ScaleTransform(shrinkX, shrinkY));
            image = BitmapFrame.Create(shrinkTransform);
            profile.Pop();

            profile.Push("Expand [WPF]");
            TransformedBitmap expandedTransform = new TransformedBitmap(image, new ScaleTransform(1 / shrinkX, 1 / shrinkY));
            BitmapFrame result = BitmapFrame.Create(expandedTransform);
            profile.Pop();

            profile.Pop();

            return result;
        }

        private static int GetLineThickness(Size size)
        {
            return Math.Max(1, (int)Math.Round((float)Math.Max(size.Width, size.Height) / ScreenMax));
        }
        private readonly static int ScreenMax = Math.Max(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        public static SmartBitmap GetPreviewImageCore(
            Profile profile,
            ImageCache cache,
            string sourceId,
            BitmapHolder sourceHolder,
            float shrinkExpandFactor,
            int annotationDivisor,
            bool[,] hf,
            Rectangle cropRect,
            bool normalizeGeometry,
            NormalizeGeometryParameters normalizeGeometryParams,
            bool brightAdjust,
            BrightAdjustParameters brightAdjustParams,
            bool polyUnbias,
            PolyUnbiasParameters polyUnbiasParams,
            bool staticSaturate,
            StaticSaturateParameters staticSatParams,
            bool oneBit,
            OneBitParameters oneBitParams,
            AddMessageMethod addMessage,
            out PolyUnbiasDiagnostics polyUnbiasDiagnostics)
        {
            polyUnbiasDiagnostics = null;

            profile.Push("Transforms.GetPreviewImageCore {0}", sourceId);

            CancellationTokenSource cancel = new CancellationTokenSource(); // not used

            string times = String.Concat("Transforms.GetPreviewImageCore: ", sourceId, Environment.NewLine);
            Stopwatch sw = Stopwatch.StartNew();

            SmartBitmap preview = null;
            try
            {
                profile.Push("Load/Scale (AnnoDiv:{0}, ShrinkExpand:{1}, NormalizeGeometry:{2}) ", annotationDivisor, shrinkExpandFactor, normalizeGeometry);
                profile.Push("Query");
                using (BitmapHolder possiblyScaledSourceHolder = (annotationDivisor == 1)
                    ? sourceHolder.Clone()
                    : cache.Query(
                        String.Format("{0}:Pre:{1}", sourceId, "PreviewFastAnnoShrunk"),
                        delegate ()
                        {
                            ManagedBitmap fastAnnoShrunk;
                            if (Program.UseGDIResize)
                            {
                                fastAnnoShrunk = ImageClient.ResizeGDI(profile, sourceHolder.Bitmap, sourceHolder.Bitmap.Width / annotationDivisor, sourceHolder.Bitmap.Height / annotationDivisor);
                            }
                            else
                            {
                                using (SmartBitmap wpfSource = new SmartBitmap(sourceHolder.Bitmap.CloneToWPF()))
                                {
                                    using (SmartBitmap fastAnnoShrunkWPF = new SmartBitmap(Resize(profile, wpfSource.AsWPF(profile), sourceHolder.Bitmap.Width / annotationDivisor, sourceHolder.Bitmap.Height / annotationDivisor)))
                                    {
                                        fastAnnoShrunk = fastAnnoShrunkWPF.AsManagedDetach();
                                    }
                                }
                            }
                            return fastAnnoShrunk;
                        }))
                {
                    profile.Pop();

                    profile.Push("Wait for Load");
                    possiblyScaledSourceHolder.Wait();
                    profile.Pop();

                    profile.Push("Clone");
                    preview = new SmartBitmap(possiblyScaledSourceHolder.Bitmap.Clone());
                    profile.Pop();
                }
                profile.Pop();

                if (normalizeGeometry)
                {
                    ApplyNormalizeGeometry(profile, sourceId, preview.AsManaged(profile), annotationDivisor, normalizeGeometryParams, addMessage, cancel);
                }

                if (polyUnbias)
                {
                    ApplyPolyUnbias(profile, sourceId, preview.AsManaged(profile), cropRect, polyUnbiasParams, addMessage, out polyUnbiasDiagnostics, cancel);
                }

                if (brightAdjust)
                {
                    ApplyBrightAdjust(profile, sourceId, preview.AsManaged(profile), cropRect, brightAdjustParams, addMessage, cancel);
                }

                if (staticSaturate)
                {
                    ApplyStaticSaturation(profile, sourceId, preview.AsManaged(profile), cropRect, staticSatParams, cancel);
                }

                if ((shrinkExpandFactor != 1) && (annotationDivisor == 1))
                {
                    // Slight difference in quality, but WPF is way faster
                    if (Program.UseGDIResize)
                    {
                        // GDI's bicubic (slow)
                        profile.Push("ShrinkExpand [GDI]");
                        preview.AbsorbManaged(ImageClient.ShrinkExpandGDI(profile, preview.AsManaged(profile), shrinkExpandFactor));
                        profile.Pop();
                    }
                    else
                    {
                        // WPF's "Fant" (box filter?)
                        profile.Push("ShrinkExpand [WPF]");
                        preview.AbsorbManaged(ImageClient.ShrinkExpandWPF(profile, preview.AsManaged(profile), shrinkExpandFactor));
                        profile.Pop();
                    }
                }

                if (oneBit)
                {
                    using (SmartBitmap mono = new SmartBitmap(ApplyOneBit(profile, sourceId, preview, oneBitParams, cancel)))
                    {
                        profile.Push("OneBit - Render");

                        if (oneBitParams.ScaleUp)
                        {
                            // TODO: switch to server (low pri since ScaleUp mode is seldom used)

                            // 'mono' is doubled in both width and height - scale back to original size with interpolation
                            using (Graphics graphics = Graphics.FromImage(preview.AsGDI(profile)))
                            {
                                graphics.DrawImage(mono.AsGDI(profile), 0, 0, preview.Width, preview.Height);
                            }
                        }
                        else
                        {
                            Debug.Assert((preview.Width == mono.Width) && (preview.Height == mono.Height));
                            mono.AsManaged(profile).CopyTo(preview.AsManaged(profile));
                        }

                        profile.Pop();
                    }
                }

                preview.AsManaged(profile);

                profile.Pop();

                SmartBitmap result = preview;
                preview = null;
                return result;
            }
            catch (Exception exception)
            {
                Program.Log(LogCat.All, exception.ToString() + Environment.NewLine);
                throw;
            }
            finally
            {
                if (preview != null)
                {
                    preview.Dispose();
                }
            }
        }

        public static void ComputeGeoCornerPadPositions(Point one, Point two, out float m, out float b, out bool reflect, out Point half, out Point twoThirds)
        {
            LineFromPoints(one, two, out m, out b, out reflect);
            if (!reflect)
            {
                float x = (one.X + two.X) / 2;
                half = new Point((int)(x + .5f), (int)(m * x + b + .5f));
                x = (one.X + 2 * two.X) / 3;
                twoThirds = new Point((int)(x + .5f), (int)(m * x + b + .5f));
            }
            else
            {
                float y = (one.Y + two.Y) / 2;
                half = new Point((int)(m * y + b + .5f), (int)(y + .5f));
                y = (one.Y + 2 * two.Y) / 3;
                twoThirds = new Point((int)(m * y + b + .5f), (int)(y + .5f));
            }
        }

        public static SmartBitmap GetAnnotatedImage(
            Profile profile,
            string sourceId,
            BitmapHolder sourceHolder,
            BitmapHolder previewHolder,
            float shrinkExpandFactor,
            int annotationDivisor,
            bool[,] hf,
            bool[,] autoCropGrid,
            PolyUnbiasDiagnostics polyUnbiasDiagnostics,
            Rectangle cropRect,
            Point[] corners,
            bool showCropDragPads)
        {
            profile.Push("Transforms.GetAnnotatedImage: ", sourceId);

            CancellationTokenSource cancel = new CancellationTokenSource(); // not used

            SmartBitmap annotated = null;
            try
            {
                profile.Push("Acquire preview image");
                annotated = new SmartBitmap(previewHolder.Bitmap.Clone());
                profile.Pop();

                profile.Push("Draw overlay");

                int sourceWidth = sourceHolder.Bitmap.Width;
                int sourceHeight = sourceHolder.Bitmap.Height;

                int boxThickness = GetLineThickness(new Size(annotated.Width, annotated.Height));

                if (polyUnbiasDiagnostics != null)
                {
                    int localBoxThickness = Math.Max(1, boxThickness - 1);
                    List<Rectangle> rects = new List<Rectangle>(polyUnbiasDiagnostics.grid.GetLength(0) * polyUnbiasDiagnostics.grid.GetLength(1));
                    for (int ii = 0; ii < polyUnbiasDiagnostics.grid.GetLength(0); ii++)
                    {
                        int i = polyUnbiasDiagnostics.gridOffset.Y + ii * PolyFitBlockSize;
                        for (int jj = 0; jj < polyUnbiasDiagnostics.grid.GetLength(1); jj++)
                        {
                            int j = polyUnbiasDiagnostics.gridOffset.X + jj * PolyFitBlockSize;
                            if (polyUnbiasDiagnostics.grid[ii, jj])
                            {
                                int x = Math.Min(j + PolyFitBlockSize, sourceWidth - 1);
                                int y = Math.Min(i + PolyFitBlockSize, sourceHeight - 1);
                                for (int k = 0; k < localBoxThickness; k++)
                                {
                                    rects.Add(
                                        new Rectangle(
                                            j / annotationDivisor + k,
                                            i / annotationDivisor + k,
                                            (x - j) / annotationDivisor,
                                            (y - i) / annotationDivisor));
                                }
                            }
                        }
                    }
                    annotated.DrawRects(System.Drawing.Color.Chartreuse, rects.ToArray());
                    rects.Clear();
                    using (Graphics graphics = Graphics.FromImage(annotated.AsGDI()))
                    {
                        float totalHeight = polyUnbiasDiagnostics.grid.GetLength(0) * PolyFitBlockSize;
                        float totalWidth = polyUnbiasDiagnostics.grid.GetLength(1) * PolyFitBlockSize;
                        using (StringFormat format = new StringFormat(StringFormat.GenericDefault))
                        {
                            format.Alignment = StringAlignment.Center;
                            const int ChiSqFontHeight = 96;
                            using (Font font = new Font(System.Drawing.FontFamily.GenericSansSerif, ChiSqFontHeight * boxThickness, FontStyle.Bold))
                            {
                                System.Drawing.Color blendedColor = System.Drawing.Color.FromArgb(
                                    128,
                                    System.Drawing.Color.DarkBlue.R,
                                    System.Drawing.Color.DarkBlue.G,
                                    System.Drawing.Color.DarkBlue.B);
                                using (System.Drawing.Brush blendedBrush = new SolidBrush(blendedColor))
                                {
                                    graphics.DrawString(
                                        String.Format("chisq {0:G5}", polyUnbiasDiagnostics.chisq),
                                        font,
                                        blendedBrush,
                                        new RectangleF(
                                            polyUnbiasDiagnostics.gridOffset.X,
                                            polyUnbiasDiagnostics.gridOffset.Y + totalHeight / 2 - font.Height,
                                            totalWidth,
                                            totalHeight),
                                        format);
                                }
                            }
                            const int CellFontHeight = 14;
                            const string NumberFormat = "F2";
                            using (Font font = new Font(System.Drawing.FontFamily.GenericSansSerif, CellFontHeight * boxThickness, FontStyle.Bold))
                            {
                                int cellWidth = (int)(totalWidth / polyUnbiasDiagnostics.weights.GetLength(1));
                                int cellHeight = (int)(totalHeight / polyUnbiasDiagnostics.weights.GetLength(0));
                                int cellOffset = (cellHeight - font.Height) / 2;
                                Tuple<int, int, System.Drawing.Brush>[] strikes = new Tuple<int, int, System.Drawing.Brush>[]
                                {
                                    new Tuple<int, int, System.Drawing.Brush>(-1, -1, System.Drawing.Brushes.Black),
                                    new Tuple<int, int, System.Drawing.Brush>( 1, -1, System.Drawing.Brushes.Black),
                                    new Tuple<int, int, System.Drawing.Brush>(-1,  1, System.Drawing.Brushes.Black),
                                    new Tuple<int, int, System.Drawing.Brush>( 1,  1, System.Drawing.Brushes.Black),
                                    new Tuple<int, int, System.Drawing.Brush>( 0,  0, System.Drawing.Brushes.AliceBlue),
                                };
                                for (int i = 0; i < polyUnbiasDiagnostics.weights.GetLength(0); i++)
                                {
                                    for (int j = 0; j < polyUnbiasDiagnostics.weights.GetLength(1); j++)
                                    {
                                        int cellX = (int)((float)j / polyUnbiasDiagnostics.weights.GetLength(1) * totalWidth + polyUnbiasDiagnostics.gridOffset.X);
                                        int cellY = (int)((float)i / polyUnbiasDiagnostics.weights.GetLength(0) * totalHeight + polyUnbiasDiagnostics.gridOffset.Y);
                                        rects.Add(new Rectangle(cellX, cellY, cellWidth, cellHeight));
                                        foreach (Tuple<int, int, System.Drawing.Brush> strike in strikes)
                                        {
                                            string text = polyUnbiasDiagnostics.weights[i, j].ToString(NumberFormat);
                                            if (text.StartsWith("0."))
                                            {
                                                text = text.Substring(1);
                                            }
                                            graphics.DrawString(
                                                text,
                                                font,
                                                strike.Item3,
                                                new RectangleF(
                                                    cellX + strike.Item1 * boxThickness,
                                                    cellY + strike.Item2 * boxThickness,
                                                    cellWidth,
                                                    cellOffset),
                                                format);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    annotated.AsManaged();
                    annotated.DrawRects(System.Drawing.Color.AliceBlue, rects.ToArray());
                }

                if (autoCropGrid != null)
                {
                    int localBoxThickness = Math.Max(1, boxThickness - 1);
                    List<Rectangle> rects = new List<Rectangle>(autoCropGrid.GetLength(0) * autoCropGrid.GetLength(1));
                    for (int i = 0; i < sourceHeight; i += AutoCropBlockSize)
                    {
                        for (int j = 0; j < sourceWidth; j += AutoCropBlockSize)
                        {
                            if (autoCropGrid[i / AutoCropBlockSize, j / AutoCropBlockSize])
                            {
                                int x = Math.Min(j + AutoCropBlockSize, sourceWidth - 1);
                                int y = Math.Min(i + AutoCropBlockSize, sourceHeight - 1);
                                for (int k = 0; k < localBoxThickness; k++)
                                {
                                    rects.Add(
                                        new Rectangle(
                                            j / annotationDivisor + k,
                                            i / annotationDivisor + k,
                                            (x - j) / annotationDivisor,
                                            (y - i) / annotationDivisor));
                                }
                            }
                        }
                    }
                    annotated.DrawRects(System.Drawing.Color.MistyRose, rects.ToArray());
                }

                if (hf != null)
                {
                    List<Rectangle> rects = new List<Rectangle>(hf.GetLength(0) * hf.GetLength(1));
                    for (int i = 0; i < sourceWidth; i += HighFrequencyBlockSize)
                    {
                        for (int j = 0; j < sourceHeight; j += HighFrequencyBlockSize)
                        {
                            if (hf[i / HighFrequencyBlockSize, j / HighFrequencyBlockSize])
                            {
                                int x = Math.Min(i + HighFrequencyBlockSize, sourceWidth - 1);
                                int y = Math.Min(j + HighFrequencyBlockSize, sourceHeight - 1);
                                for (int k = 0; k < boxThickness; k++)
                                {
                                    rects.Add(
                                        new Rectangle(
                                            i / annotationDivisor + k,
                                            j / annotationDivisor + k,
                                            (x - i) / annotationDivisor,
                                            (y - j) / annotationDivisor));
                                }
                            }
                        }
                    }
                    annotated.DrawRects(System.Drawing.Color.Yellow, rects.ToArray());
                }

                if (!cropRect.IsEmpty)
                {
                    const int PadRadius = 8;
                    Tuple<Point, int>[] pads = new Tuple<Point, int>[]
                    {
                        new Tuple<Point, int>(new Point(cropRect.Left, cropRect.Top), 1),
                        new Tuple<Point, int>(new Point(cropRect.Right, cropRect.Top), 1),
                        new Tuple<Point, int>(new Point(cropRect.Left, cropRect.Bottom), 1),
                        new Tuple<Point, int>(new Point(cropRect.Right, cropRect.Bottom), 1),

                        new Tuple<Point, int>(new Point(cropRect.Left, (cropRect.Top + cropRect.Bottom) / 2), 1),
                        new Tuple<Point, int>(new Point(cropRect.Right, (cropRect.Top + cropRect.Bottom) / 2), 1),
                        new Tuple<Point, int>(new Point((cropRect.Left + cropRect.Right) / 2, cropRect.Top), 1),
                        new Tuple<Point, int>(new Point((cropRect.Left + cropRect.Right) / 2, cropRect.Bottom), 1),

                        new Tuple<Point, int>(new Point((cropRect.Left + cropRect.Right) / 2, (cropRect.Top + cropRect.Bottom) / 2), 1),
                        new Tuple<Point, int>(new Point((cropRect.Left + cropRect.Right) / 2, (cropRect.Top + cropRect.Bottom) / 2), 2),
                    };

                    List<Rectangle> rects = new List<Rectangle>();
                    for (int k = 0; k < boxThickness; k++)
                    {
                        rects.Add(
                            new Rectangle(
                                cropRect.X / annotationDivisor + k,
                                cropRect.Y / annotationDivisor + k,
                                cropRect.Width / annotationDivisor,
                                cropRect.Height / annotationDivisor));
                        if (showCropDragPads)
                        {
                            foreach (Tuple<Point, int> pad in pads)
                            {
                                rects.Add(
                                    new Rectangle(
                                        pad.Item1.X / annotationDivisor - PadRadius * boxThickness * pad.Item2 + k,
                                        pad.Item1.Y / annotationDivisor - PadRadius * boxThickness * pad.Item2 + k,
                                        2 * PadRadius * boxThickness * pad.Item2,
                                        2 * PadRadius * boxThickness * pad.Item2));
                            }
                        }
                    }
                    annotated.DrawRects(System.Drawing.Color.LightBlue, rects.ToArray());
                }

                if (corners != null)
                {
                    const int PadRadius = 8;

                    List<Rectangle> rects = new List<Rectangle>();
                    foreach (Point corner in corners)
                    {
                        for (int i = 0; i < boxThickness * PadRadius; i++)
                        {
                            int x = corner.X / annotationDivisor - boxThickness / 2;
                            int y = corner.Y / annotationDivisor - boxThickness / 2;
                            rects.Add(new Rectangle(x - i, y - i, boxThickness, boxThickness));
                            rects.Add(new Rectangle(x + i, y + i, boxThickness, boxThickness));
                            rects.Add(new Rectangle(x - i, y + i, boxThickness, boxThickness));
                            rects.Add(new Rectangle(x + i, y - i, boxThickness, boxThickness));
                        }
                    }
                    using (Graphics graphics = Graphics.FromImage(annotated.AsGDI()))
                    {
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.HotPink, boxThickness))
                        {
                            Point[] ordered = new Point[] { corners[0], corners[1], corners[3], corners[2] };
                            for (int i = 0; i < ordered.Length; i++)
                            {
                                Point one = ordered[i];
                                one.X /= annotationDivisor;
                                one.Y /= annotationDivisor;
                                Point two = ordered[(i + 1) % ordered.Length];
                                two.X /= annotationDivisor;
                                two.Y /= annotationDivisor;

                                float m, b;
                                bool reflect;
                                Point half, twoThirds;
                                ComputeGeoCornerPadPositions(one, two, out m, out b, out reflect, out half, out twoThirds);
                                if (!reflect)
                                {
                                    graphics.DrawLine(pen, 0, b, annotated.Width, m * annotated.Width + b);
                                }
                                else
                                {
                                    graphics.DrawLine(pen, b, 0, m * annotated.Height + b, annotated.Height);
                                }

                                graphics.DrawRectangle(
                                    pen,
                                    half.X - PadRadius * boxThickness,
                                    half.Y - PadRadius * boxThickness,
                                    PadRadius * 2 * boxThickness,
                                    PadRadius * 2 * boxThickness);
                                graphics.DrawEllipse(
                                    pen,
                                    twoThirds.X - PadRadius * boxThickness,
                                    twoThirds.Y - PadRadius * boxThickness,
                                    PadRadius * 2 * boxThickness,
                                    PadRadius * 2 * boxThickness);
                            }
                        }
                    }
                    annotated.AsManaged();
                    annotated.DrawRects(System.Drawing.Color.Red, rects);
                }

                profile.Pop();

                SmartBitmap result = annotated;
                annotated = null;
                return result;
            }
            catch (Exception exception)
            {
                Program.Log(LogCat.All, exception.ToString() + Environment.NewLine);
                throw;
            }
            finally
            {
                if (annotated != null)
                {
                    annotated.Dispose();
                }
            }
        }
    }
}
