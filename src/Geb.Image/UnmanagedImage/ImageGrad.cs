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

    public partial class ImageGrad : UnmanagedImage<Grad>
    {
        public unsafe ImageGrad(Int32 width,Int32 height)
            : base(width,height)
        {
        }

        public unsafe ImageGrad(Int32 width, Int32 height, void* data)
            : base(width, height,data)
        {
        }

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

        public override unsafe Bitmap ToBitmap()
        {
            return ToImageU8().ToBitmap();
        }

        protected override unsafe void CreateFromBitmap(Bitmap map)
        {
            throw new NotImplementedException();
        }

        protected override PixelFormat GetOutputBitmapPixelFormat()
        {
            throw new NotImplementedException();
        }

        protected override unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            throw new NotImplementedException();
        }

        public override IImage Clone()
        {
            ImageGrad img = new ImageGrad(this.Width, this.Height);
            img.CloneFrom(this);
            return img;
        }

        protected override IColorConverter CreateByteConverter()
        {
            return null;
        }
    }
}
