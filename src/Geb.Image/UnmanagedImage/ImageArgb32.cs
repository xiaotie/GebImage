/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;

namespace Geb.Image
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct Argb32
    {
        public static Argb32 WHITE = new Argb32 { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static Argb32 BLACK = new Argb32 { Alpha = 255 };
        public static Argb32 RED = new Argb32 { Red = 255, Alpha = 255 };
        public static Argb32 BLUE = new Argb32 { Blue = 255, Alpha = 255 };
        public static Argb32 GREEN = new Argb32 { Green = 255, Alpha = 255 };
        public static Argb32 EMPTY = new Argb32 { };

        [FieldOffset(0)]
        public Byte Blue;
        [FieldOffset(1)]
        public Byte Green;
        [FieldOffset(2)]
        public Byte Red;
        [FieldOffset(3)]
        public Byte Alpha;

        public Argb32(int red, int green, int blue, int alpha = 255)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
            Alpha = (byte)alpha;
        }

        public Argb32(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Byte ToGray()
        {
            return (Byte)(0.299 * Red + 0.587 * Green + 0.114 * Blue);
        }

        public override string ToString()
        {
            return "Argb32 [A="+ Alpha +", R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }

    public partial class ImageArgb32 : IDisposable
    {
        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Rgb24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToArgb32(from, (Argb32*)to, length);
        }

        private unsafe void Copy(Argb32* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy((byte*)from, (byte*)to, 4 * length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToArgb32(from, (Argb32*)to, length);
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format32bppArgb;
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.Copy(src, dst, width * 4);
        }
        
        #endregion

        public ImageU8 ToGrayscaleImage()
        {
            return ToGrayscaleImage(0.299, 0.587, 0.114);
        }

        public ImageU8 ToGrayscaleImage(byte transparentColor)
        {
            return ToGrayscaleImage(0.299, 0.587, 0.114, transparentColor);
        }

        public unsafe ImageU8 ToGrayscaleImage(double rCoeff, double gCoeff, double bCoeff, byte transparentColor)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Argb32* p = Start;
            Byte* to = img.Start;
            Argb32* end = p + Length;

            while (p != end)
            {
                if (p->Alpha == 0)
                {
                    *to = transparentColor;
                }
                else
                {
                    *to = (Byte)(p->Red * rCoeff + p->Green * gCoeff + p->Blue * bCoeff);
                }
                p++;
                to++;
            }

            return img;
        }

        public unsafe ImageU8 ToGrayscaleImage(double rCoeff, double gCoeff, double bCoeff)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Argb32* p = Start;
            Byte* to = img.Start;
            Argb32* end = p + Length;

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

        public unsafe void SetAlpha(byte alpha)
        {
            Argb32* start = (Argb32*)this.Start;
            Argb32* end = start + this.Length;
            while (start != end)
            {
                start->Alpha = alpha;
                start++;
            }
        }

        public unsafe void CombineAlpha(ImageArgb32 src, System.Drawing.Point start, System.Drawing.Rectangle region, System.Drawing.Point destAnchor)
        {
            if (start.X >= src.Width || start.Y >= src.Height) return;
            int startSrcX = Math.Max(0, start.X);
            int startSrcY = Math.Max(0, start.Y);
            int endSrcX = Math.Min(start.X + region.Width, src.Width);
            int endSrcY = Math.Min(start.Y + region.Height, src.Height);
            int offsetX = start.X < 0 ? -start.X : 0;
            int offsetY = start.Y < 0 ? -start.Y : 0;
            offsetX = destAnchor.X + offsetX;
            offsetY = destAnchor.Y + offsetY;
            int startDstX = Math.Max(0, offsetX);
            int startDstY = Math.Max(0, offsetY);
            offsetX = offsetX < 0 ? -offsetX : 0;
            offsetY = offsetY < 0 ? -offsetY : 0;
            startSrcX += offsetX;
            startSrcY += offsetY;
            int endDstX = Math.Min(destAnchor.X + region.Width, this.Width);
            int endDstY = Math.Min(destAnchor.Y + region.Height, this.Height);
            int copyWidth = Math.Min(endSrcX - startSrcX, endDstX - startDstX);
            int copyHeight = Math.Min(endSrcY - startSrcY, endDstY - startDstY);
            if (copyWidth <= 0 || copyHeight <= 0) return;

            int srcWidth = src.Width;
            int dstWidth = this.Width;

            Argb32* srcLine = (Argb32*)(src.Start) + srcWidth * startSrcY + startSrcX;
            Argb32* dstLine = this.Start + dstWidth * startDstY + startDstX;
            Argb32* endSrcLine = srcLine + srcWidth * copyHeight;
            while (srcLine < endSrcLine)
            {
                Argb32* pSrc = srcLine;
                Argb32* endPSrc = pSrc + copyWidth;
                Argb32* pDst = dstLine;
                while (pSrc < endPSrc)
                {
                    Argb32 p0 = *pSrc;
                    Argb32 p1 = *pDst;
                    switch (p0.Alpha)
                    {
                        case 255:
                            *pDst = p0;
                            break;
                        case 0:
                        default:
                            break;
                    }
                    pSrc++;
                    pDst++;
                }
                srcLine += srcWidth;
                dstLine += dstWidth;
            }
        }


        /// <summary>
        /// 应用双指数保边平滑算法，这是一种计算速度比较快的保边平滑算法。算法描述：
        /// Philippe Thévenaz, Daniel Sage, and Michael Unser. Bi-Exponential Edge-Preserving Smoother.
        /// IEEE TRANSACTIONS ON IMAGE PROCESSING, VOL. 21, NO. 9, SEPTEMBER 2012
        /// url：http://bigwww.epfl.ch/publications/thevenaz1202.pdf
        /// </summary>
        public unsafe void ApplyBiExponentialEdgePreservingSmoother(double photometricStandardDeviation = 30, double spatialDecay = 0.01)
        {
            Byte* p0 = (Byte*)this.Start;
            const int nChannel = 4;
            int length = this.Width * this.Height;

            // 对每个channel进行处理
            for (int idxChannel = 0; idxChannel < nChannel; idxChannel++)
            {
                float[] data1 = new float[Width * Height];
                float[] data2 = new float[Width * Height];

                for (int i = 0; i < length; i++)
                {
                    float val = p0[i * nChannel];
                    data1[i] = data2[i] = val;
                }

                BEEPSHorizontalVertical hv = new BEEPSHorizontalVertical(data1, Width, Height,  photometricStandardDeviation, spatialDecay);
                BEEPSVerticalHorizontal vh = new BEEPSVerticalHorizontal(data2, Width, Height, photometricStandardDeviation, spatialDecay);
                hv.run();
                vh.run();
                
                for (int i = 0; i < length; i++)
                {
                    float val = (data1[i] + data2[i]) * 0.5f;
                    val = Math.Min(255.0f, val);
                    p0[i * nChannel] = (Byte)val;
                }

               p0++;
            }
        }

        #region BiExponentialEdgePreservingSmooth 的具体实现类

        internal class BEEPSGain
        {
            private double[] data;
            private int length;
            private int startIndex;
            private static double mu;

            internal BEEPSGain(double[] data, int startIndex, int length)
            {
                this.data = data;
                this.startIndex = startIndex;
                this.length = length;
            }

            internal static void setup(double spatialContraDecay)
            {
                mu = (1.0 - spatialContraDecay) / (1.0 + spatialContraDecay);
            }

            public void run()
            {
                for (int k = startIndex, K = startIndex + length; (k < K); k++)
                {
                    data[k] *= mu;
                }
            }

        }

        internal class BEEPSHorizontalVertical
        {
            private double photometricStandardDeviation;
            private double spatialDecay;
            private float[] data;
            private int height;
            private int width;

            internal BEEPSHorizontalVertical(float[] data, int width, int height, double photometricStandardDeviation, double spatialDecay)
            {
                this.data = data;
                this.width = width;
                this.height = height;
                this.photometricStandardDeviation = photometricStandardDeviation;
                this.spatialDecay = spatialDecay;
            }

            public void run()
            {
                BEEPSProgressive.setup(photometricStandardDeviation,
                    1.0 - spatialDecay);
                BEEPSGain.setup(1.0 - spatialDecay);
                BEEPSRegressive.setup(photometricStandardDeviation,
                    1.0 - spatialDecay);
                double[] g = new double[width * height];
                for (int k = 0, K = data.Length; (k < K); k++)
                {
                    g[k] = (double)data[k];
                }

                double[] p = new double[height * width];
                double[] r = new double[height * width];

                Array.Copy(g, p, height * width);
                Array.Copy(g, r, height * width);

                for (int k2 = 0; (k2 < height); k2++)
                {
                    BEEPSProgressive progressive = new BEEPSProgressive(p, k2 * width, width);
                    BEEPSGain gain = new BEEPSGain(g, k2 * width, width);
                    BEEPSRegressive regressive = new BEEPSRegressive(r, k2 * width, width);
                    progressive.run();
                    gain.run();
                    regressive.run();
                }

                for (int k = 0, K = data.Length; (k < K); k++)
                {
                    r[k] += p[k] - g[k];
                }
                int m = 0;
                for (int k2 = 0; (k2 < height); k2++)
                {
                    int n = k2;
                    for (int k1 = 0; (k1 < width); k1++)
                    {
                        g[n] = r[m++];
                        n += height;
                    }
                }

                Array.Copy(g, p, height * width);
                Array.Copy(g, r, height * width);
                for (int k1 = 0; (k1 < width); k1++)
                {
                    BEEPSProgressive progressive = new BEEPSProgressive(p, k1 * height, height);
                    BEEPSGain gain = new BEEPSGain(g, k1 * height, height);
                    BEEPSRegressive regressive = new BEEPSRegressive(r, k1 * height, height);
                    progressive.run();
                    gain.run();
                    regressive.run();
                }

                for (int k = 0, K = data.Length; (k < K); k++)
                {
                    r[k] += p[k] - g[k];
                }
                m = 0;
                for (int k1 = 0; (k1 < width); k1++)
                {
                    int n = k1;
                    for (int k2 = 0; (k2 < height); k2++)
                    {
                        data[n] = (float)r[m++];
                        n += width;
                    }
                }
            }
        }

        const double BEEP_ZETA_3 = 1.2020569031595942853997381615114499907649862923404988817922715553418382057;
        internal class BEEPSProgressive
        {
            private double[] data;
            private int length;
            private int startIndex;
            private static double c;
            private static double rho;
            private static double spatialContraDecay;

            internal BEEPSProgressive(double[] data, int startIndex, int length)
            {
                this.data = data;
                this.startIndex = startIndex;
                this.length = length;
            }

            internal static void setup(double photometricStandardDeviation, double sharedSpatialContraDecay)
            {
                spatialContraDecay = sharedSpatialContraDecay;
                rho = 1.0 + spatialContraDecay;
                c = -0.5 / (photometricStandardDeviation * photometricStandardDeviation);
            }

            public void run()
            {
                double mu = 0.0;
                data[startIndex] /= rho;
                for (int k = startIndex + 1, K = startIndex + length;
                               (k < K); k++)
                {
                    mu = data[k] - rho * data[k - 1];
                    mu = spatialContraDecay * Math.Exp(c * mu * mu);
                    data[k] = data[k - 1] * mu + data[k] * (1.0 - mu) / rho;
                }
            }
        }

        internal class BEEPSRegressive
        {
            private double[] data;
            private int length;
            private int startIndex;
            private static double c;
            private static double rho;
            private static double spatialContraDecay;

            internal BEEPSRegressive(double[] data, int startIndex, int length)
            {
                this.data = data;
                this.startIndex = startIndex;
                this.length = length;
            }

            internal static void setup(double photometricStandardDeviation, double sharedSpatialContraDecay)
            {
                spatialContraDecay = sharedSpatialContraDecay;
                rho = 1.0 + spatialContraDecay;
                c = -0.5 / (photometricStandardDeviation * photometricStandardDeviation);
            }

            public void run()
            {
                double mu = 0.0;
                data[startIndex + length - 1] /= rho;
                for (int k = startIndex + length - 2; (startIndex <= k); k--)
                {
                    mu = data[k] - rho * data[k + 1];
                    mu = spatialContraDecay * Math.Exp(c * mu * mu);
                    data[k] = data[k + 1] * mu + data[k] * (1.0 - mu) / rho;
                }
            }
        }

        internal class BEEPSVerticalHorizontal
        {
            private double photometricStandardDeviation;
            private double spatialDecay;
            private float[] data;
            private int height;
            private int width;

            internal BEEPSVerticalHorizontal(float[] data, int width, int height, double photometricStandardDeviation, double spatialDecay)
            {
                this.data = data;
                this.width = width;
                this.height = height;
                this.photometricStandardDeviation = photometricStandardDeviation;
                this.spatialDecay = spatialDecay;
            }

            public void run()
            {
                BEEPSProgressive.setup(photometricStandardDeviation, 1.0 - spatialDecay);
                BEEPSGain.setup(1.0 - spatialDecay);
                BEEPSRegressive.setup(photometricStandardDeviation, 1.0 - spatialDecay);
                double[] g = new double[height * width];
                int m = 0;
                for (int k2 = 0; (k2 < height); k2++)
                {
                    int n = k2;
                    for (int k1 = 0; (k1 < width); k1++)
                    {
                        g[n] = (double)data[m++];
                        n += height;
                    }
                }

                double[] p = new double[height * width];
                double[] r = new double[height * width];

                Array.Copy(g, p, height * width);
                Array.Copy(g, r, height * width);

                for (int k1 = 0; (k1 < width); k1++)
                {
                    BEEPSProgressive progressive = new BEEPSProgressive(p, k1 * height, height);
                    BEEPSGain gain = new BEEPSGain(g, k1 * height, height);
                    BEEPSRegressive regressive = new BEEPSRegressive(r, k1 * height, height);
                    progressive.run();
                    gain.run();
                    regressive.run();
                }

                for (int k = 0, K = data.Length; (k < K); k++)
                {
                    r[k] += p[k] - g[k];
                }
                m = 0;
                for (int k1 = 0; (k1 < width); k1++)
                {
                    int n = k1;
                    for (int k2 = 0; (k2 < height); k2++)
                    {
                        g[n] = r[m++];
                        n += width;
                    }
                }

                Array.Copy(g, p, height * width);
                Array.Copy(g, r, height * width);

                for (int k2 = 0; (k2 < height); k2++)
                {
                    BEEPSProgressive progressive = new BEEPSProgressive(p, k2 * width, width);
                    BEEPSGain gain = new BEEPSGain(g, k2 * width, width);
                    BEEPSRegressive regressive = new BEEPSRegressive(r, k2 * width, width);
                    progressive.run();
                    gain.run();
                    regressive.run();
                }

                for (int k = 0, K = data.Length; (k < K); k++)
                {
                    data[k] = (float)(p[k] - g[k] + r[k]);
                }
            }
        }

        #endregion

    }
}
