/*************************************************************************
 *  Copyright (c) 2019 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct RgbS
    {
        public static RgbS WHITE = new RgbS { Red = 1, Green = 1, Blue = 1};
        public static RgbS BLACK = new RgbS { };
        public static RgbS RED = new RgbS { Red = 255};
        public static RgbS BLUE = new RgbS { Blue = 255};
        public static RgbS GREEN = new RgbS { Green = 255};
        public static RgbS EMPTY = new RgbS { };

        [FieldOffset(0)]
        public float Red;
        [FieldOffset(4)]
        public float Green;
        [FieldOffset(8)]
        public float Blue;

        public float NormMax()
        {
            return Math.Max(Math.Max(Math.Abs(Blue), Math.Abs(Red)), Math.Abs(Green));
        }

        public float Norm()
        {
            return (float)Math.Sqrt(Red * Red + Green * Green + Blue * Blue);
        }

        public RgbS(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Byte ToGray()
        {
            return (Byte)(0.299 * Red + 0.587 * Green + 0.114 * Blue);
        }

        public override string ToString()
        {
            return "BgrS [R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }

    public partial class ImageRgbS : IImage, IDisposable
    {
        public const int ChannelCount = 1;

        public int BytesPerPixel { get; } = 12;

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            throw new NotImplementedException();
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            throw new NotImplementedException();
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            throw new NotImplementedException();
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            throw new NotImplementedException();
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            throw new NotImplementedException();
        }
    }
}
