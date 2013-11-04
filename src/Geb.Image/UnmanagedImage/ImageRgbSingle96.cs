/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public partial struct RgbSingle96
    {
        public Single Blue,Red,Green;

        public RgbSingle96(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public RgbSingle96(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    public partial class ImageRgbSingle96 : IDisposable
    {
        public const int ChannelCount = 3;

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

        private System.Drawing.Imaging.PixelFormat GetOutputBitmapPixelFormat()
        {
            throw new NotImplementedException();
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
