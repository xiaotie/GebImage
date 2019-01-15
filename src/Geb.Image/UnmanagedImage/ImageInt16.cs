/*************************************************************************
 *  Copyright (c) 2019 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Geb.Image
{
    public partial class ImageInt16 : IImage, IDisposable
    {
        public const int ChannelCount = 1;

        public int BytesPerPixel { get; } = 2;

        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToBgra32(from, (Bgra32*)to, length);
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy((Byte*)from, (Byte*)to, length * 4);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            if (length < 1) return;
            Byte* end = from + length;
            Int32* dst = (Int32*)to;
            while (from != end)
            {
                *dst = *from;
                from++;
                dst++;
            }
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format8bpp;
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            Int32* start = (Int32*)src;
            Int32* end = start + width;
            while (start != end)
            {
                Int32 val = *start;
                val = val < 0 ? 0 : val > 255 ? 255 : val;
                *dst = (byte)val;
                start++;
                dst++;
            }
        }

        #endregion

    }
}