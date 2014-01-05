/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using TPixel = System.Byte;
using TCache = System.Int32;
using TImage = Geb.Image.ImageU8;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Hidden
{
    public class ImageFilter_Template : TImage
    {
        public ImageFilter_Template()
            :base(1,1)
        {
        }

        #region mixin

        /// <summary>
        /// 使用卷积。
        /// </summary>
        /// <param name="k">卷积核</param>
        /// <returns>直接在原图像上使用卷积，返回为卷积后的原图像</returns>
        public unsafe TImage ApplyConvolution(ConvolutionKernel k)
        {
            int kernelHeight = k.Width;
            int kernelWidth = k.Height;
            int scale = k.Scale;
            int valShift = k.ValueShift;
            int[,] kernel = k.Kernel;
            int extend = Math.Max(kernelWidth, kernelHeight) / 2;
            int width = this.Width;
            int height = this.Height;
            TPixel* start = this.Start;
            TImage maskImage = CreatePaddingImage(extend);

            if (scale == 1)
            {
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        TCache val = 0;
                        for (int kw = 0; kw < kernelWidth; kw++)
                        {
                            for (int kh = 0; kh < kernelHeight; kh++)
                            {
                                val += maskImage[h + kh, w + kw] * kernel[kh, kw];
                            }
                        }
                        start[h * width + w] = (TPixel)(val + valShift);
                    }
                }
            }
            else
            {
                double factor = 1.0 / scale;
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        TCache val = 0;
                        for (int kw = 0; kw < kernelWidth; kw++)
                        {
                            for (int kh = 0; kh < kernelHeight; kh++)
                            {
                                val += maskImage[h + kh, w + kw] * kernel[kh, kw];
                            }
                        }
                        start[h * width + w] = (TPixel)(val * factor + valShift);
                    }
                }
            }
            maskImage.Dispose();
            return this;
        }

        #endregion
    }
}
