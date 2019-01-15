/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Geb.Image
{
    public partial class ImageFloat : IImage, IDisposable
    {
        public const int ChannelCount = 1;

        public int BytesPerPixel { get; } = 4;

        #region Image <-> Bitmap 所需的方法

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
            return PixelFormat.Format32bppBgra;
        }

        #endregion

        public unsafe ImageU8 ToImageU8(int coeff = 255)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            float* p = this.Start;
            float* pEnd = p + this.Length;
            byte* dst = img.Start;
            float val = 0;
            while (p < pEnd)
            {
                val = *p * coeff;
                val = Math.Min(255,Math.Max(val, 0));
                *dst = (Byte)val;
                p++;
                dst++;
            }
            return img;
        }
    }
}
