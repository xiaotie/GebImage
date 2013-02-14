/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    using Geb.Utils;

    [StructLayout(LayoutKind.Explicit)]
    public partial struct Rgb24 : IMetriable<Rgb24>
    {
        public static Rgb24 WHITE = new Rgb24 { Red = 255, Green = 255, Blue = 255 };
        public static Rgb24 BLACK = new Rgb24();
        public static Rgb24 RED = new Rgb24 { Red = 255 };
        public static Rgb24 BLUE = new Rgb24 { Blue = 255 };
        public static Rgb24 GREEN = new Rgb24 { Green = 255 };
        public static Rgb24 YELLOW = new Rgb24 { Red = 255, Green = 255 };
        public static Rgb24 PINK = new Rgb24 { Red = 253, Green = 215, Blue = 228 };

        /// <summary>
        /// 紫红色
        /// </summary>
        public static Rgb24 FUCHISIA = new Rgb24 { Red = 253, Green = 00, Blue = 255 };

        public static Rgb24 CYAN = new Rgb24 { Green = 255, Blue = 255 };

        public Rgb24(int red, int green, int blue)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
        }

        public Rgb24(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Rgb24(int value):this((value >> 16) & 0xFF, (value >> 8) & 0xFF, value & 0xFF)
        {
        }

        [FieldOffset(0)]
        public Byte Blue;
        [FieldOffset(1)]
        public Byte Green;
        [FieldOffset(2)]
        public Byte Red;

        public static readonly int BlueChennel = 0;
        public static readonly int GreenChannel = 1;
        public static readonly int RedChannel = 2;

        public override string ToString()
        {
            return "Rgb24 [R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }

        public void AdjustSaturation()
        {
            if (this.Red == this.Green && this.Red == this.Blue) return;

            int max = Math.Max(Math.Max(this.Red, this.Blue), this.Green);
            int min = Math.Min(Math.Min(this.Red, this.Blue), this.Green);
            float coeff = (float)max / (max - min);
            int red = (int)((this.Red - min) * coeff);
            int green = (int)((this.Green - min) * coeff);
            int blue = (int)((this.Blue - min) * coeff);
            if (red > 255) red = 255;
            if (green > 255) green = 255;
            if (blue > 255) blue = 255;
            this.Red = (byte)red;
            this.Green = (byte)green;
            this.Blue = (byte)blue;
        }

        public Byte ToGray()
        {
            return (Byte)(0.299 * Red + 0.587 * Green + 0.114 * Blue);
        }

        public Lab24 ToLab24()
        {
            return Lab24.CreateFrom(this);
        }

        public double GetDistance(Rgb24 other)
        {
            int deltaR = this.Red - other.Red;
            int deltaG = this.Green - other.Green;
            int deltaB = this.Blue - other.Blue;
            int distance = deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
            return Math.Sqrt(distance);
        }

        public int GetDistanceSquare(Rgb24 other)
        {
            int deltaR = this.Red - other.Red;
            int deltaG = this.Green - other.Green;
            int deltaB = this.Blue - other.Blue;
            return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
        }

        public int GetVisualDistanceSquare(Rgb24 other, int redCoeff = 1, int greenCoeff = 2, int blueCoeff = 1)
        {
            int deltaR = this.Red - other.Red;
            int deltaG = this.Green - other.Green;
            int deltaB = this.Blue - other.Blue;
            return redCoeff * deltaR * deltaR + greenCoeff * deltaG * deltaG + blueCoeff * deltaB * deltaB;
        }

        public override int GetHashCode()
        {
            return (Red << 16) + (Green << 8) + Blue;
        }
    }

    public struct Rgb24Converter : IColorConverter
    {
        public unsafe void Copy(Rgb24* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy((byte*)from, (byte*)to, 3* length);
        }

        public unsafe void Copy(Argb32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToRgb24(from, (Rgb24*)to, length);
        }

        public unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToRgb24(from, (Rgb24*)to, length);
        }
    }

    public partial class ImageRgb24 : UnmanagedImage<Rgb24>
    {
        public unsafe ImageRgb24(Int32 width, Int32 height)
            : base(width, height)
        {
        }

        public unsafe ImageRgb24(Int32 width, Int32 height, void* data)
            : base(width, height,data)
        {
        }

        public ImageRgb24(Bitmap map)
            : base(map)
        {
        }

        public ImageRgb24(String path)
            : base(path)
        {
        }

        protected override IColorConverter CreateByteConverter()
        {
            return new Rgb24Converter();
        }

        public ImageU8 ToGrayscaleImage()
        {
            return ToGrayscaleImage(0.299, 0.587, 0.114);
        }

        public ImageInt32 ToGrayscaleImageInt32()
        {
            return ToGrayscaleImageInt32(0.299, 0.587, 0.114);
        }

        public unsafe ImageU8 CopyChannel(int channel)
        {
            if (channel < 0 && channel > 2) throw new ArgumentOutOfRangeException("channel");
            int length = this.Length;
            Byte* start = (Byte*)this.StartIntPtr;
            int size = sizeof(Rgb24);
            Byte* end = start + sizeof(Rgb24) * length;
            ImageU8 imgU8 = new ImageU8(this.Width, this.Height);
            Byte* dst = imgU8.Start;
            while(start != end)
            {
                *dst = start[channel];
                start += size;
                dst ++;
            }
            return imgU8;
        }

        public unsafe ImageU8 ToGrayscaleImage(double rCoeff, double gCoeff, double bCoeff)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Rgb24* p = Start;
            Byte* to = img.Start;
            Rgb24* end = p + Length;

            if (Length < 1024)
            {
                while (p != end)
                {
                    *to = (Byte)(p->Red * rCoeff + p->Green * gCoeff + p->Blue * bCoeff);
                    p++;
                    to++;
                }
            }
            else
            {
                int* bCache = stackalloc int[256];
                int* gCache = stackalloc int[256];
                int* rCache = stackalloc int[256];

                const int shift = 1<<10;
                int rShift = (int)(rCoeff * shift);
                int gShift = (int)(gCoeff * shift);
                int bShift = shift - rShift - gShift;

                int r = 0, g = 0, b = 0;
                for (int i = 0; i < 256; i++)
                {
                    bCache[i] = b;
                    gCache[i] = g;
                    rCache[i] = r;
                    b += bShift;
                    g += gShift;
                    r += rShift;
                }

                while (p != end)
                {
                    *to = (Byte)(( bCache[p->Red] + gCache[p->Green] + rCache[p->Red] ) >> 10);
                    p++;
                    to++;
                }
            }
            return img;
        }

        public unsafe ImageInt32 ToGrayscaleImageInt32(double rCoeff, double gCoeff, double bCoeff)
        {
            ImageInt32 img = new ImageInt32(this.Width, this.Height);
            Rgb24* p = Start;
            Int32* to = img.Start;
            Rgb24* end = p + Length;

            if (Length < 1024)
            {
                while (p != end)
                {
                    *to = (Byte)(p->Red * rCoeff + p->Green * gCoeff + p->Blue * bCoeff);
                    p++;
                    to++;
                }
            }
            else
            {
                int* bCache = stackalloc int[256];
                int* gCache = stackalloc int[256];
                int* rCache = stackalloc int[256];

                const int shift = 1 << 10;
                int rShift = (int)(rCoeff * shift);
                int gShift = (int)(gCoeff * shift);
                int bShift = shift - rShift - gShift;

                int r = 0, g = 0, b = 0;
                for (int i = 0; i < 256; i++)
                {
                    bCache[i] = b;
                    gCache[i] = g;
                    rCache[i] = r;
                    b += bShift;
                    g += gShift;
                    r += rShift;
                }

                while (p != end)
                {
                    *to = (Byte)((bCache[p->Red] + gCache[p->Green] + rCache[p->Red]) >> 10);
                    p++;
                    to++;
                }
            }
            return img;
        }

        public unsafe void ApplyMedianFilter(int medianRadius)
        {
            if (medianRadius > 0)
            {
                // 进行中值滤波
                using (ImageRgb24 copy = this.Clone() as ImageRgb24)
                {
                    int size = medianRadius * 2 + 1;
                    int count = 0;
                    byte[] r = new byte[size * size];
                    byte[] g = new byte[size * size];
                    byte[] b = new byte[size * size];
                    int height = this.Height;
                    int width = this.Width;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            count = 0;
                            for (int h = -medianRadius; h <= medianRadius; h++)
                            {
                                for (int w = -medianRadius; w <= medianRadius; w++)
                                {
                                    int hh = y + h;
                                    int ww = x + w;
                                    if (hh >= 0 && hh < height && ww >= 0 && ww < width)
                                    {
                                        Rgb24 c = copy[hh, ww];
                                        r[count] = c.Red;
                                        g[count] = c.Green;
                                        b[count] = c.Blue;
                                        count++;
                                    }
                                }
                            }

                            Array.Sort(r, 0, count);
                            Array.Sort(g, 0, count);
                            Array.Sort(b, 0, count);
                            int m = count >> 1;
                            Rgb24 median = new Rgb24 { Red = r[m], Green = g[m], Blue = b[m] };
                            this[y, x] = median;
                        }
                    }
                }
            }
        }

        public unsafe void ApplyMedianFilter(int medianRadius, IList<Point> points)
        {
            if (medianRadius <= 0 || points == null) return;
            Rgb24[] vals = new Rgb24[points.Count];
            int size = medianRadius * 2 + 1;
            int count = 0;
            byte[] r = new byte[size * size];
            byte[] g = new byte[size * size];
            byte[] b = new byte[size * size];
            int height = this.Height;
            int width = this.Width;

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                int x = p.X;
                int y = p.Y;
                count = 0;
                for (int h = -medianRadius; h <= medianRadius; h++)
                {
                    for (int w = -medianRadius; w <= medianRadius; w++)
                    {
                        int hh = y + h;
                        int ww = x + w;
                        if (hh >= 0 && hh < height && ww >= 0 && ww < width)
                        {
                            Rgb24 c = this[hh, ww];
                            r[count] = c.Red;
                            g[count] = c.Green;
                            b[count] = c.Blue;
                            count++;
                        }
                    }
                }

                Array.Sort(r, 0, count);
                Array.Sort(g, 0, count);
                Array.Sort(b, 0, count);
                int m = count >> 1;
                Rgb24 median = new Rgb24 { Red = r[m], Green = g[m], Blue = b[m] };
                vals[i] = median;
            }

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                this[p] = vals[i];
            }
        }

        protected override System.Drawing.Imaging.PixelFormat GetOutputBitmapPixelFormat()
        {
            return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
        }

        protected override unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.Copy(src, dst, width * 3);
        }
    }
}
