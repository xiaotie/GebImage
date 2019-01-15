/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using Geb.Utils;

    public partial struct Hsl 
    {
        public float H;
        public float S;
        public float L;

        public Hsl(float h, float s, float l)
        {
            H = h;
            S = s;
            L = l;
        }
    }

    public partial class ImageHsl : IImage, IDisposable
    {
        public const int ChannelCount = 3;

        public int BytesPerPixel { get; } = 12;

        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToHsl(from, (Hsl*)to, length);
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToHsl(from, (Hsl*)to, length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToHsl(from, (Hsl*)to, length);
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.ToRgb24((Hsl*)src, (Bgr24*)dst, width);
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format24bppBgr;
        }

        #endregion

        public unsafe ImageHsl(ImageBgr24 img)
            : this(img.Width, img.Height)
        {
            int length = img.Length;
            UnmanagedImageConverter.ToLab24(img.Start, (Lab24*)this.Start, length);
        }

        public unsafe ImageBgr24 ToImageRgb24()
        {
            ImageBgr24 img = new ImageBgr24(this.Width, this.Height);
            UnmanagedImageConverter.ToBgr24((Lab24*)this.Start, img.Start, img.Length);
            return img;
        }
    }
}
