/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public partial struct Grad
    {
        public float Value;

        /// <summary>
        /// 弧度
        /// </summary>
        public float Theta;
    }

    public partial class ImageGrad : IDisposable
    {
        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Rgb24* from, void* to, int length)
        {
            throw new NotImplementedException();
        }

        private unsafe void Copy(Argb32* from, void* to, int length)
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
            return PixelFormat.Format32bppArgb;
        }

        #endregion

        public unsafe ImageU8 ToImageU8()
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Grad* start = this.Start;
            Grad* end = start + this.Length;
            float max = 0;

            // 找最大值
            Grad* src = start;
            while (src != end)
            {
                max = Math.Max(max, src->Value);
                src++;
            }

            float coeff = max > 255 ? 255f / max : 0;

            src = start;
            Byte* dst = img.Start;
            while (src != end)
            {
                *dst = (Byte)(coeff * src->Value);
                dst++;
                src++;
            }

            return img;
        }
    }
}
