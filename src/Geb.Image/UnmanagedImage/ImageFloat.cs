/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public struct FloatConverter : IColorConverter
    {
        public unsafe void Copy(Rgb24* from, void* to, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe void Copy(Argb32* from, void* to, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe void Copy(byte* from, void* to, int length)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ImageFloat : UnmanagedImage<float>
    {
        public unsafe ImageFloat(Int32 width, Int32 height)
            : base(width, height)
        {
        }

        protected override IColorConverter CreateByteConverter()
        {
            return new FloatConverter();
        }

        public override IImage Clone()
        {
            ImageFloat img = new ImageFloat(this.Width, this.Height);
            img.CloneFrom(this);
            return img;
        }

        protected override PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format8bppIndexed;
        }

        protected override unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            throw new NotImplementedException();
        }
    }
}
