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
    public partial struct SignedArgb64
    {
        public static SignedArgb64 WHITE = new SignedArgb64 { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static SignedArgb64 BLACK = new SignedArgb64 { Alpha = 255 };
        public static SignedArgb64 RED = new SignedArgb64 { Red = 255, Alpha = 255 };
        public static SignedArgb64 BLUE = new SignedArgb64 { Blue = 255, Alpha = 255 };
        public static SignedArgb64 GREEN = new SignedArgb64 { Green = 255, Alpha = 255 };
        public static SignedArgb64 EMPTY = new SignedArgb64 { };

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

        public SignedArgb64(int red, int green, int blue, int alpha = 255)
        {
            Red = (short)red;
            Green = (short)green;
            Blue = (short)blue;
            Alpha = (short)alpha;
        }

        public SignedArgb64(short red, short green, short blue, short alpha = 255)
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

    public struct SignedArgb64Converter : IColorConverter
    {
        public unsafe void Copy(Rgb24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToSignedArgb64(from, (SignedArgb64*)to, length);
        }

        public unsafe void Copy(Argb32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToSignedArgb64(from, (SignedArgb64*)to, length);
        }

        public unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToSignedArgb64(from, (SignedArgb64*)to, length);
        }
    }

    public partial class ImageSignedArgb64 : UnmanagedImage<SignedArgb64>
    {
        #region 静态方法

        public unsafe static ImageSignedArgb64 CreateFrom(ImageArgb32 img)
        {
            ImageSignedArgb64 imgS = new ImageSignedArgb64(img.Width, img.Height);
            SignedArgb64* pS = imgS.Start;
            SignedArgb64* pSEnd = imgS.Start + imgS.Length;
            Argb32* p = img.Start;
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
        
        public unsafe ImageSignedArgb64(Int32 width, Int32 height)
            : base(width, height)
        {
        }

        public unsafe ImageSignedArgb64(Int32 width, Int32 height, void* data)
            : base(width, height,data)
        {
        }

        public ImageSignedArgb64(Bitmap map)
            : base(map)
        {
        }

        public ImageSignedArgb64(String path)
            : base(path)
        {
        }

        protected override IColorConverter CreateByteConverter()
        {
            return new SignedArgb64Converter();
        }

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
            SignedArgb64* p = Start;
            Byte* to = img.Start;
            SignedArgb64* end = p + Length;

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
            SignedArgb64* p = Start;
            Byte* to = img.Start;
            SignedArgb64* end = p + Length;

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

        public unsafe ImageArgb32 ToImageArgb32()
        {
            ImageArgb32 img = new ImageArgb32(this.Width, this.Height);
            SignedArgb64* p = Start;
            Argb32* to = img.Start;
            SignedArgb64* end = p + Length;

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

        protected override System.Drawing.Imaging.PixelFormat GetOutputBitmapPixelFormat()
        {
            return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
        }

        protected override unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.ToArgb32((SignedArgb64*)src, (Argb32*)dst, width);
        }

        public unsafe void SetAlpha(byte alpha)
        {
            SignedArgb64* start = (SignedArgb64*)this.Start;
            SignedArgb64* end = start + this.Length;
            while (start != end)
            {
                start->Alpha = alpha;
                start++;
            }
        }

        public unsafe ImageSignedArgb64 Subtract(ImageSignedArgb64 other)
        {
            if ((other.Width != this.Width) || (other.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            ImageSignedArgb64 img = new ImageSignedArgb64(this.Width, this.Height);
            SignedArgb64* p = img.Start;
            SignedArgb64* p0 = this.Start;
            SignedArgb64* p1 = other.Start;
            SignedArgb64* end = p + this.Length;

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

        public unsafe ImageSignedArgb64 Add(ImageSignedArgb64 other)
        {
            if ((other.Width != this.Width) || (other.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            ImageSignedArgb64 img = new ImageSignedArgb64(this.Width, this.Height);
            SignedArgb64* p = img.Start;
            SignedArgb64* p0 = this.Start;
            SignedArgb64* p1 = other.Start;
            SignedArgb64* end = p + this.Length;

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

        public unsafe ImageSignedArgb64 Mix(ImageSignedArgb64 other)
        {
            if ((other.Width != this.Width) || (other.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            ImageSignedArgb64 img = new ImageSignedArgb64(this.Width, this.Height);
            SignedArgb64* p = img.Start;
            SignedArgb64* p0 = this.Start;
            SignedArgb64* p1 = other.Start;
            SignedArgb64* end = p + this.Length;

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

        public unsafe void MixInPlace(ImageSignedArgb64 bg)
        {
            if ((bg.Width != this.Width) || (bg.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            SignedArgb64* p0 = this.Start;
            SignedArgb64* p1 = bg.Start;
            SignedArgb64* end = p0 + this.Length;

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

        public unsafe void MixInPlace(ImageSignedArgb64 bg, ImageInt32 mask)
        {
            if ((bg.Width != this.Width) || (bg.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }

            SignedArgb64* p0 = this.Start;
            SignedArgb64* p1 = bg.Start;
            SignedArgb64* end = p0 + this.Length;
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

        public unsafe void MixInPlace(ImageSignedArgb64 bg, Byte frontRatio)
        {
            if ((bg.Width != this.Width) || (bg.Height != this.Height))
            {
                throw new ArgumentException("Images not the same size.");
            }
            int coeff0 = frontRatio;
            int coeff1 = 256 - frontRatio;
            SignedArgb64* p0 = this.Start;
            SignedArgb64* p1 = bg.Start;
            SignedArgb64* end = p0 + this.Length;
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
            SignedArgb64* start = (SignedArgb64*)this.Start;
            SignedArgb64* end = start + this.Length;
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
            SignedArgb64* start = (SignedArgb64*)this.Start;
            SignedArgb64* end = start + this.Length;
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
            SignedArgb64* start = (SignedArgb64*)this.Start;
            SignedArgb64* end = start + this.Length;
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
