/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace Geb.Image
{
    /// <summary>
    /// 2D梯度场像素
    /// </summary>
    public partial struct GradXY
    {
        public Int16 DX;
        public Int16 DY;
        public GradXY(Int16 dx, Int16 dy)
        {
            DX = dx;
            DY = dy;
        }
    }

    /// <summary>
    /// 2D梯度场图像
    /// </summary>
    public partial class ImageGradXY : IImage, IDisposable
    {
        public const int ChannelCount = 2;

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
        
        /// <summary>
        /// 返回 X 方向的梯度图像，红色代表正值，蓝色代表负值
        /// </summary>
        /// <returns></returns>
        public ImageBgr24 ToXGradImage(double scale = 1)
        {
            ImageBgr24 img = new ImageBgr24(this.Width, this.Height);
            for(int i = 0; i < Length; i++)
            {
                int val = this[i].DX;
                Bgr24 c = new Bgr24();
                if (val >= 0)
                    c.Red = (Byte)Math.Min(255, val * scale);
                else
                    c.Blue = (Byte)Math.Min(255, -val * scale);
                img[i] = c;
            }
            return img;
        }

        /// <summary>
        /// 返回 Y 方向的梯度图像，红色代表正值，蓝色代表负值
        /// </summary>
        /// <returns></returns>
        public ImageBgr24 ToYGradImage(double scale = 1)
        {
            ImageBgr24 img = new ImageBgr24(this.Width, this.Height);
            for (int i = 0; i < Length; i++)
            {
                int val = this[i].DY;
                Bgr24 c = new Bgr24();
                if (val >= 0)
                    c.Red = (Byte)Math.Min(255, val * scale);
                else
                    c.Blue = (Byte)Math.Min(255, -val * scale);
                img[i] = c;
            }
            return img;
        }
    }
}
