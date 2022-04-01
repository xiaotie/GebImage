/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct Bgr24
    {
        public static Bgr24 WHITE = new Bgr24 { Red = 255, Green = 255, Blue = 255 };
        public static Bgr24 BLACK = new Bgr24();
        public static Bgr24 RED = new Bgr24 { Red = 255 };
        public static Bgr24 BLUE = new Bgr24 { Blue = 255 };
        public static Bgr24 GREEN = new Bgr24 { Green = 255 };
        public static Bgr24 YELLOW = new Bgr24 { Red = 255, Green = 255 };
        public static Bgr24 PINK = new Bgr24 { Red = 253, Green = 215, Blue = 228 };

        /// <summary>
        /// 紫红色
        /// </summary>
        public static Bgr24 FUCHISIA = new Bgr24 { Red = 253, Green = 00, Blue = 255 };

        public static Bgr24 CYAN = new Bgr24 { Green = 255, Blue = 255 };

        public Bgr24(int red, int green, int blue)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
        }

        public Bgr24(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Bgr24(int value)
            : this((value >> 16) & 0xFF, (value >> 8) & 0xFF, value & 0xFF)
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

        public double GetDistance(Bgr24 other)
        {
            int deltaR = this.Red - other.Red;
            int deltaG = this.Green - other.Green;
            int deltaB = this.Blue - other.Blue;
            int distance = deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
            return Math.Sqrt(distance);
        }

        public int GetDistanceSquare(Bgr24 other)
        {
            int deltaR = this.Red - other.Red;
            int deltaG = this.Green - other.Green;
            int deltaB = this.Blue - other.Blue;
            return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
        }

        public int GetVisualDistanceSquare(Bgr24 other, int redCoeff = 1, int greenCoeff = 2, int blueCoeff = 1)
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

    public partial class ImageBgr24 : IImage, IDisposable
    {
        public const int ChannelCount = 3;

        public int BytesPerPixel { get; } = 3;

        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy((byte*)from, (byte*)to, 3 * length);
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToBgr24(from, (Bgr24*)to, length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToBgr24(from, (Bgr24*)to, length);
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format24bppBgr;
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.Copy(src, dst, width * sizeof(Bgr24));
        }

        #endregion

        #region 转换为编码图像
        public void SaveBmp(String imagePath)
        {
            using(var imageBgr32=this.ToImageBgr32())
                new Formats.Bmp.BmpEncoder().Encode(imageBgr32, imagePath);
        }

        public void SaveJpeg(String imagePath, int quality = 70, Formats.Jpeg.JpegPixelFormats fmt = Formats.Jpeg.JpegPixelFormats.YCbCr)
        {
            Formats.Jpeg.JpegEncoder.Encode(this, imagePath, quality, fmt);
        }

        public Byte[] ToJpegData(int quality = 70, Formats.Jpeg.JpegPixelFormats fmt = Formats.Jpeg.JpegPixelFormats.YCbCr)
        {
            return Formats.Jpeg.JpegEncoder.Encode(this, quality, fmt);
        }

        public void SavePng(String imagePath, Formats.Png.PngEncoderOptions options = null)
        {
            Formats.Png.PngEncoder.Encode(this, imagePath, options);
        }

        public Byte[] ToPngData(Formats.Png.PngEncoderOptions options = null)
        {
            return Formats.Png.PngEncoder.Encode(this, options);
        }
        #endregion

        public unsafe ImageBgr24(int width, int height, Bgr24* p0, int stride) 
            : this(width,height)
        {
            for(int h = 0; h < height; h++)
            {
                Bgr24* pDst = Start + h * width;
                Bgr24* pSrc = (Bgr24*)((Byte*)p0 + stride * width);
                Bgr24* pDstEnd = pDst + width;
                while(pDst < pDstEnd)
                {
                    *pDst = *pSrc;
                    pDst++;
                    pSrc++;
                }
            }
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
            Byte* start = (Byte*)this.Start;
            int size = sizeof(Bgr24);
            Byte* end = start + sizeof(Bgr24) * length;
            ImageU8 imgU8 = new ImageU8(this.Width, this.Height);
            Byte* dst = imgU8.Start;
            while (start != end)
            {
                *dst = start[channel];
                start += size;
                dst++;
            }
            return imgU8;
        }

        public unsafe ImageU8 ToGrayscaleImage(double rCoeff, double gCoeff, double bCoeff)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Bgr24* p = Start;
            Byte* to = img.Start;
            Bgr24* end = p + Length;

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

        public unsafe ImageInt32 ToGrayscaleImageInt32(double rCoeff, double gCoeff, double bCoeff)
        {
            ImageInt32 img = new ImageInt32(this.Width, this.Height);
            Bgr24* p = Start;
            Int32* to = img.Start;
            Bgr24* end = p + Length;

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

        /// <summary>
        /// 将当前图像转换为 ImageBgra32 格式的图像
        /// </summary>
        /// <returns>转换后的图像</returns>
        public unsafe ImageBgra32 ToImageBgr32()
        {
            ImageBgra32 image = new ImageBgra32(this.Width, this.Height);
            Bgr24* pSrc = this.Start;
            Bgra32* pDst = image.Start;
            Bgr24* pSrcEnd = this.Start + this.Length;
            while (pSrc < pSrcEnd)
            {
                pDst->Red = pSrc->Red;
                pDst->Green = pSrc->Green;
                pDst->Blue = pSrc->Blue;
                pDst->Alpha = 255;
                pSrc++; pDst++;
            }
            return image;
        }


        /// <summary>
        /// 对图像进行 Alpha 混合
        /// </summary>
        /// <param name="background"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public unsafe ImageBgr24 AlphaMix(ImageBgr24 background, ImageU8 mask)
        {
            var size = this.Size;
            if (size != background.Size || size != mask.Size)
                throw new ArgumentException("All the images must be same size.");

            ImageBgr24 imgMixed = background.Clone();
            Bgr24* pSrc = this.Start;
            Bgr24* pDst = imgMixed.Start;
            Byte* pMask = mask.Start;
            for (int i = 0; i < imgMixed.Length; i++)
            {
                int alpha = pMask[i];
                if (alpha == 255) pDst[i] = pSrc[i];
                else if (alpha > 0)
                {
                    int beta = 255 - alpha;
                    Bgr24 a = pSrc[i];
                    Bgr24 b = pDst[i];
                    a.Red = (Byte)((a.Red * alpha + b.Red * beta) >> 8);
                    a.Green = (Byte)((a.Green * alpha + b.Green * beta) >> 8);
                    a.Blue = (Byte)((a.Blue * alpha + b.Blue * beta) >> 8);
                    pDst[i] = a;
                }
            }

            return imgMixed;
        }

        public unsafe void ApplyMedianFilter(int medianRadius)
        {
            if (medianRadius > 0)
            {
                // 进行中值滤波
                using (ImageBgr24 copy = this.Clone() as ImageBgr24)
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
                                        Bgr24 c = copy[hh, ww];
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
                            Bgr24 median = new Bgr24 { Red = r[m], Green = g[m], Blue = b[m] };
                            this[y, x] = median;
                        }
                    }
                }
            }
        }

        public unsafe void ApplyMedianFilter(int medianRadius, IList<Geb.Image.Point> points)
        {
            if (medianRadius <= 0 || points == null) return;
            Bgr24[] vals = new Bgr24[points.Count];
            int size = medianRadius * 2 + 1;
            int count = 0;
            byte[] r = new byte[size * size];
            byte[] g = new byte[size * size];
            byte[] b = new byte[size * size];
            int height = this.Height;
            int width = this.Width;
            
            for (int i = 0; i < points.Count; i++)
            {
                Geb.Image.Point p = points[i];
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
                            Bgr24 c = this[hh, ww];
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
                Bgr24 median = new Bgr24 { Red = r[m], Green = g[m], Blue = b[m] };
                vals[i] = median;
            }

            for (int i = 0; i < points.Count; i++)
            {
                Geb.Image.Point p = points[i];
                this[p] = vals[i];
            }
        }
    }
}
