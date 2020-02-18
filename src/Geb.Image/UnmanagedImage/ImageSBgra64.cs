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
    [StructLayout(LayoutKind.Explicit)]
    public partial struct SBgra64
    {
        public static SBgra64 WHITE = new SBgra64 { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static SBgra64 BLACK = new SBgra64 { Alpha = 255 };
        public static SBgra64 RED = new SBgra64 { Red = 255, Alpha = 255 };
        public static SBgra64 BLUE = new SBgra64 { Blue = 255, Alpha = 255 };
        public static SBgra64 GREEN = new SBgra64 { Green = 255, Alpha = 255 };
        public static SBgra64 EMPTY = new SBgra64 { };

        [FieldOffset(0)]
        public short Blue;
        [FieldOffset(2)]
        public short Green;
        [FieldOffset(4)]
        public short Red;
        [FieldOffset(6)]
        public short Alpha;

        public short NormMax()
        {
            return Math.Max(Math.Max(Math.Abs(Blue), Math.Abs(Red)), Math.Abs(Green));
        }

        public double Norm()
        {
            return Math.Sqrt(Red * Red + Green * Green + Blue * Blue);
        }

        public SBgra64(int red, int green, int blue, int alpha = 255)
        {
            Red = (short)red;
            Green = (short)green;
            Blue = (short)blue;
            Alpha = (short)alpha;
        }

        public SBgra64(short red, short green, short blue, short alpha = 255)
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
            return "SignedArgb64 [A=" + Alpha + ", R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }

    public partial class ImageSBgra64 : IImage, IDisposable
    {
        public const int ChannelCount = 4;

        public int BytesPerPixel { get; } = 8;

        #region 静态方法

        public unsafe static ImageSBgra64 CreateFrom(ImageBgra32 img)
        {
            ImageSBgra64 imgS = new ImageSBgra64(img.Width, img.Height);
            SBgra64* pS = imgS.Start;
            SBgra64* pSEnd = imgS.Start + imgS.Length;
            Bgra32* p = img.Start;
            while (pS < pSEnd)
            {
                pS->Blue = p->Blue;
                pS->Green = p->Green;
                pS->Red = p->Red;
                pS->Alpha = p->Alpha;
                pS++;
                p++;
            }
            return imgS;
        }

	    #endregion 
        
        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToSignedArgb64(from, (SBgra64*)to, length);
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToSignedArgb64(from, (SBgra64*)to, length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToSignedArgb64(from, (SBgra64*)to, length);
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format32bppBgra;
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.ToBgra32((SBgra64*)src, (Bgra32*)dst, width);
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
            SBgra64* p = Start;
            Byte* to = img.Start;
            SBgra64* end = p + Length;

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
            SBgra64* p = Start;
            Byte* to = img.Start;
            SBgra64* end = p + Length;

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

        public unsafe ImageBgra32 ToImageArgb32()
        {
            ImageBgra32 img = new ImageBgra32(this.Width, this.Height);
            SBgra64* p = Start;
            Bgra32* to = img.Start;
            SBgra64* end = p + Length;

            while (p != end)
            {
                to->Blue = (byte)p->Blue;
                to->Green = (byte)p->Green;
                to->Red = (byte)p->Red;
                to->Alpha = (byte)p->Alpha;
                p++;
                to++;
            }

            return img;
        }

        public unsafe void SetAlpha(byte alpha)
        {
            SBgra64* start = (SBgra64*)this.Start;
            SBgra64* end = start + this.Length;
            while (start != end)
            {
                start->Alpha = alpha;
                start++;
            }
        }

        public unsafe ImageSBgra64 Subtract(ImageSBgra64 other)
        {
            if ((other.Width != this.Width) || (other.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            ImageSBgra64 img = new ImageSBgra64(this.Width, this.Height);
            SBgra64* p = img.Start;
            SBgra64* p0 = this.Start;
            SBgra64* p1 = other.Start;
            SBgra64* end = p + this.Length;

            while (p != end)
            {
                p->Blue = (short)(p0->Blue - p1->Blue);
                p->Green = (short)(p0->Green - p1->Green);
                p->Red = (short)(p0->Red - p1->Red);
                p->Alpha = (short)(p0->Alpha - p1->Alpha);
                p++;
                p0++;
                p1++;
            }

            return img;
        }

        public unsafe ImageSBgra64 Add(ImageSBgra64 other)
        {
            if ((other.Width != this.Width) || (other.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            ImageSBgra64 img = new ImageSBgra64(this.Width, this.Height);
            SBgra64* p = img.Start;
            SBgra64* p0 = this.Start;
            SBgra64* p1 = other.Start;
            SBgra64* end = p + this.Length;

            while (p != end)
            {
                p->Blue = (short)(p0->Blue + p1->Blue);
                p->Green = (short)(p0->Green + p1->Green);
                p->Red = (short)(p0->Red + p1->Red);
                p->Alpha = (short)(p0->Alpha + p1->Alpha);
                p++;
                p0++;
                p1++;
            }

            return img;
        }

        public unsafe ImageSBgra64 Mix(ImageSBgra64 other)
        {
            if ((other.Width != this.Width) || (other.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            ImageSBgra64 img = new ImageSBgra64(this.Width, this.Height);
            SBgra64* p = img.Start;
            SBgra64* p0 = this.Start;
            SBgra64* p1 = other.Start;
            SBgra64* end = p + this.Length;

            while (p != end)
            {
                p->Blue = (short)((p0->Blue + p1->Blue)/2);
                p->Green = (short)((p0->Green + p1->Green) / 2);
                p->Red = (short)((p0->Red + p1->Red)/2);
                p->Alpha = (short)((p0->Alpha + p1->Alpha)/2);
                p++;
                p0++;
                p1++;
            }

            return img;
        }

        public unsafe void MixInPlace(ImageSBgra64 bg)
        {
            if ((bg.Width != this.Width) || (bg.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            SBgra64* p0 = this.Start;
            SBgra64* p1 = bg.Start;
            SBgra64* end = p0 + this.Length;

            while (p0 != end)
            {
                if (Math.Abs(p1->Blue) > Math.Abs(p0->Blue))
                {
                    p0->Blue = p1->Blue;
                }

                if (Math.Abs(p1->Green) > Math.Abs(p0->Green))
                {
                    p0->Green = p1->Green;
                }

                if (Math.Abs(p1->Red) > Math.Abs(p0->Red))
                {
                    p0->Red = p1->Red;
                }

                if (Math.Abs(p1->Alpha) > Math.Abs(p0->Alpha))
                {
                    p0->Alpha = p1->Alpha;
                }

                //p0->Blue = (short)((p0->Blue + p1->Blue)/2);
                //p0->Green = (short)((p0->Green + p1->Green) / 2);
                //p0->Red = (short)((p0->Red + p1->Red)/2);
                //p0->Alpha = (short)((p0->Alpha + p1->Alpha)/2);
                p0++;
                p1++;
            }
        }

        public unsafe void MixInPlace(ImageSBgra64 bg, ImageInt32 mask)
        {
            if ((bg.Width != this.Width) || (bg.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            SBgra64* p0 = this.Start;
            SBgra64* p1 = bg.Start;
            SBgra64* end = p0 + this.Length;
            Int32* pMask = mask.Start;

            while (p0 != end)
            {
                int coeff0 = *pMask;
                int coeff1 = 256 - coeff0;

                p0->Blue = (short)((p0->Blue * coeff0 + p1->Blue * coeff1) / 256);
                p0->Green = (short)((p0->Green * coeff0 + p1->Green * coeff1) / 256);
                p0->Red = (short)((p0->Red * coeff0 + p1->Red * coeff1) / 256);

                p0++;
                p1++;
                pMask++;
            }
        }

        public unsafe void MixInPlace(ImageSBgra64 bg, Byte frontRatio)
        {
            if ((bg.Width != this.Width) || (bg.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }
            int coeff0 = frontRatio;
            int coeff1 = 256 - frontRatio;
            SBgra64* p0 = this.Start;
            SBgra64* p1 = bg.Start;
            SBgra64* end = p0 + this.Length;
            while (p0 != end)
            {
                p0->Blue = (short)((p0->Blue * coeff0 + p1->Blue * coeff1) / 256);
                p0->Green = (short)((p0->Green * coeff0 + p1->Green * coeff1) / 256);
                p0->Red = (short)((p0->Red * coeff0 + p1->Red * coeff1) / 256);
                p0++;
                p1++;
            }
        }

        public unsafe void UpdatePixel(int add, int shift)
        {
            SBgra64* start = (SBgra64*)this.Start;
            SBgra64* end = start + this.Length;
            while (start != end)
            {
                start->Blue = (short)((start->Blue + add) >> shift);
                start->Green = (short)((start->Green + add) >> shift);
                start->Red = (short)((start->Red + add) >> shift);
                start->Alpha = (short)((start->Alpha + add) >> shift);
                start++;
            }
        }

        public unsafe void Multiply(int val)
        {
            SBgra64* start = (SBgra64*)this.Start;
            SBgra64* end = start + this.Length;
            while (start != end)
            {
                start->Blue = (short)(start->Blue * val);
                start->Green = (short)(start->Green * val);
                start->Red = (short)(start->Red * val);
                start->Alpha = (short)(start->Alpha * val);
                start++;
            }
        }

        public unsafe void Abs()
        {
            SBgra64* start = (SBgra64*)this.Start;
            SBgra64* end = start + this.Length;
            while (start != end)
            {
                start->Blue = Math.Abs(start->Blue);
                start->Green = Math.Abs(start->Green);
                start->Red = Math.Abs(start->Red);
                start->Alpha = Math.Abs(start->Alpha);
                start++;
            }
        }


    }
}
