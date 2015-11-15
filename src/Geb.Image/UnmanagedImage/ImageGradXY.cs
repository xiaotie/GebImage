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
    /// <summary>
    /// 2D梯度场像素
    /// </summary>
    public partial struct GradXY
    {
        public Single DX;
        public Single DY;
    }

    /// <summary>
    /// 2D梯度场图像
    /// </summary>
    public partial class ImageGradXY : IDisposable
    {
        public const int ChannelCount = 2;

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
        
    }
}
