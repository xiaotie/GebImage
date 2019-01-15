/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    public partial struct FBgr96
    {
        public Single Blue,Red,Green;

        public FBgr96(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public FBgr96(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    public partial class ImageFBgr96 : IImage, IDisposable
    {
        public const int ChannelCount = 3;

        public int BytesPerPixel { get; } = 12;

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

        private PixelFormat GetOutputBitmapPixelFormat()
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
