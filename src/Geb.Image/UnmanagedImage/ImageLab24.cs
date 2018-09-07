/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    public partial struct Lab24
    {
        public byte L;
        public byte A;
        public byte B;

        public Lab24(byte l,byte a,byte b)
        {
            L = l;
            A = a;
            B = b;
        }

        public Lab24(int l,int a,int b)
        {
            L = (byte)l;
            A = (byte)a;
            B = (byte)b;
        }

        public int GetDistanceSquare(Lab24 other)
        {
            int deltaL = this.L - other.L;
            int deltaA = this.A - other.A;
            int deltaB = this.B - other.B;
            return (int)(0.16 * deltaL * deltaL) + deltaA * deltaA + deltaB * deltaB;
        }

        public double GetDistance(Lab24 other)
        {
            return Math.Sqrt(GetDistanceSquare(other));
        }

        #region RGB 与 Lab 互相转换 ，从 OpenCV 项目中借鉴得到

        
        public static unsafe Lab24 CreateFrom(Bgr24 color)
        {
            Lab24 dst = new Lab24();
            UnmanagedImageConverter.ToLab24(&color, &dst, 1);
            return dst;
        }

        public static unsafe Lab24 CreateFrom(Byte red,Byte green,Byte blue)
        {
            return CreateFrom(new Bgr24(red,green,blue));
        }

        public unsafe Bgr24 ToRgb24()
        {
            Bgr24 dst = new Bgr24();
            fixed (Lab24* l = &this)
            {
                UnmanagedImageConverter.ToRgb24(l, &dst, 1);
            }
            return dst;
        }

        #endregion

        #region Test

        public static void ConvertTest()
        {
            Boolean error = false;

            for (int r = 0; r < 256; r += 2)
            {
                for (int g = 0; g < 256; g += 2)
                {
                    for (int b = 0; b < 256; b += 2)
                    {
                        Bgr24 src = new Bgr24(r, g, b);
                        Lab24 dst = Lab24.CreateFrom(src);
                        Bgr24 rvt = dst.ToRgb24();
                        if (Math.Abs(src.Red - rvt.Red) + Math.Abs(src.Green - rvt.Green) + Math.Abs(src.Blue - rvt.Blue) > 16)
                        {
                            error = true;
                        }

                    }
                }
            }

            Console.WriteLine(error);
        }

        #endregion
    }

    public partial class ImageLab24 : IDisposable
    {
        public const int ChannelCount = 3;

        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToLab24(from, (Lab24*)to, length);
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToLab24(from, (Lab24*)to, length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToLab24(from, (Lab24*)to, length);
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.ToRgb24((Lab24*)src, (Bgr24*)dst, width);
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format24bppRgb;
        }

        #endregion

        public unsafe ImageLab24(ImageBgr24 img)
            : this(img.Width, img.Height)
        {
            int length = img.Length;
            UnmanagedImageConverter.ToLab24(img.Start, (Lab24*)this.Start, length);
        }

        public unsafe ImageBgr24 ToImageRgb24()
        {
            ImageBgr24 img = new ImageBgr24(this.Width, this.Height);
            UnmanagedImageConverter.ToRgb24((Lab24*)this.Start, img.Start, img.Length);
            return img;
        }
    }
}
