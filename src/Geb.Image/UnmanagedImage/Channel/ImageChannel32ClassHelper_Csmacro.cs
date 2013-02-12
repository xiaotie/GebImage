/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Int32;
    using TValue = System.Int32;

    public partial class ImageChannel32 : ImageChannel<Int32>
    {
        public static unsafe void FillImage(ImageChannel<TPixel> img, TPixel value)
        {
            int step = img.Step;
            TPixel* start = (TPixel*)img.StartIntPtr;
            TPixel* end = start += img.PixelCount * img.Step;
            while (start != end)
            {
                *start = value;
                start += step;
            }
        }

        public unsafe TPixel* Start { get { return (TPixel*)this.StartIntPtr; } }

        public static unsafe void CopyChannel(ImageChannel<TPixel> src, ImageChannel<TPixel> dst)
        {
            if (src.PixelCount != dst.PixelCount) throw new InvalidOperationException("The two channels are not the same length.");

            int length = src.PixelCount;
            int srcStep = src.Step;
            int dstStep = dst.Step;
            TPixel* srcStart = (TPixel*)src.StartIntPtr;
            TPixel* srcEnd = srcStart + length * srcStep;
            TPixel* dstStart = (TPixel*)dst.StartIntPtr;
            while (srcStart != srcEnd)
            {
                *dstStart = *srcStart;
                srcStart += srcStep;
                dstStart += dstStep;
            }
        }

        public static unsafe void CopyToIntChannel(ImageChannel<TPixel> src, ImageChannel<Int32> dst)
        {
            if (src.PixelCount != dst.PixelCount) throw new InvalidOperationException("The two channels are not the same length.");

            int length = src.PixelCount;
            int srcStep = src.Step;
            int dstStep = dst.Step;
            TPixel* srcStart = (TPixel*)src.StartIntPtr;
            TPixel* srcEnd = srcStart + length * srcStep;
            Int32* dstStart = (Int32*)dst.StartIntPtr;
            while (srcStart != srcEnd)
            {
                *dstStart = (Int32)(*srcStart);
                srcStart += srcStep;
                dstStart += dstStep;
            }
        }

        public static unsafe void CopyChannel(ImageChannel<TPixel> src, ImageChannel<TPixel> dst, System.Drawing.Point start, System.Drawing.Rectangle region, System.Drawing.Point destAnchor)
        {
            if (start.X >= src.Width || start.Y >= src.Height) return;
            int startSrcX = Math.Max(0, start.X);
            int startSrcY = Math.Max(0, start.Y);
            int endSrcX = Math.Min(start.X + region.Width, src.Width);
            int endSrcY = Math.Min(start.Y + region.Height, src.Height);
            int offsetX = start.X < 0 ? -start.X : 0;
            int offsetY = start.Y < 0 ? -start.Y : 0;
            offsetX = destAnchor.X + offsetX;
            offsetY = destAnchor.Y + offsetY;
            int startDstX = Math.Max(0, offsetX);
            int startDstY = Math.Max(0, offsetY);
            offsetX = offsetX < 0 ? -offsetX : 0;
            offsetY = offsetY < 0 ? -offsetY : 0;
            startSrcX += offsetX;
            startSrcY += offsetY;
            int step = dst.Step;
            int endDstX = Math.Min(destAnchor.X + region.Width, dst.Width);
            int endDstY = Math.Min(destAnchor.Y + region.Height, dst.Height);
            int copyWidth = Math.Min(endSrcX - startSrcX, endDstX - startDstX) * step;
            int copyHeight = Math.Min(endSrcY - startSrcY, endDstY - startDstY);
            if (copyWidth <= 0 || copyHeight <= 0) return;
            int srcWidth = src.Width * step;
            int dstWidth = dst.Width * step;

            TPixel* srcLine = (TPixel*)(src.StartIntPtr) + srcWidth * startSrcY + startSrcX;
            TPixel* dstLine = (TPixel*)dst.StartIntPtr + dstWidth * step * startDstY + startDstX;
            TPixel* endSrcLine = srcLine + srcWidth * copyHeight;
            while (srcLine < endSrcLine)
            {
                TPixel* pSrc = srcLine;
                TPixel* endPSrc = pSrc + copyWidth;
                TPixel* pDst = dstLine;
                while (pSrc < endPSrc)
                {
                    *pDst = *pSrc;
                    pSrc += step;
                    pDst += step;
                }
                srcLine += srcWidth;
                dstLine += dstWidth;
            }
        }

        public void ApplyConvolution(ConvolutionKernel k)
        {
            ApplyConvolution(this, k);
        }

        public static unsafe void ApplyConvolution(ImageChannel<TPixel> img, ConvolutionKernel k)
        {
            if (img.Width < 2 || img.Height < 2) return;

            int widthTypeScaled = img.Width * img.Step;

            int kernelHeight = k.Width;
            int kernelWidth = k.Height;
            int scale = k.Scale;
            int[,] kernel = k.Kernel;
            int extend = Math.Max(kernelWidth, kernelHeight) / 2;
            ImageChannel<TPixel> maskImage = new ImageChannel<TPixel>(img.Width + extend * 2, img.Height + extend * 2);
            FillImage(maskImage, (TPixel)0); //这里效率不高。原本只需要填充四周扩大的部分即可
            CopyChannel(img, maskImage, new System.Drawing.Point(0, 0), new System.Drawing.Rectangle(0, 0, img.Width, img.Height), new System.Drawing.Point(extend, extend));

            int width = img.Width;
            int height = img.Height;
            TPixel* start = (TPixel*)img.StartIntPtr;
            TPixel* maskStart = (TPixel*)maskImage.StartIntPtr;

            // 复制边界像素
            TPixel* dstStart = maskStart + extend;
            int maskWidth = img.Width + extend * 2;
            int maskHeight = img.Height + extend * 2;
            TPixel* dstContentStart = maskStart + extend + maskWidth * extend;

            // 复制上方的像素
            for (int y = 0; y < extend; y++)
            {
                TPixel* lineStart = dstStart + y * maskWidth;
                TPixel* lineEnd = lineStart + width;
                TPixel* copyStart = dstContentStart;
                while (lineStart != lineEnd)
                {
                    *lineStart = *copyStart;
                    lineStart++;
                    copyStart++;
                }
            }

            // 复制下方的像素
            for (int y = height + extend; y < maskHeight; y++)
            {
                TPixel* lineStart = dstStart + y * maskWidth;
                TPixel* lineEnd = lineStart + width;
                TPixel* copyStart = dstContentStart + height * maskWidth - maskWidth;
                while (lineStart != lineEnd)
                {
                    *lineStart = *copyStart;
                    lineStart++;
                    copyStart++;
                }
            }

            // 复制左右两侧的像素
            TPixel* dstLine = maskStart + maskWidth * extend;
            TPixel p = default(TPixel);
            for (int y = extend; y < height + extend; y++)
            {
                p = dstLine[extend];
                for (int x = 0; x < extend; x++)
                {
                    dstLine[x] = p;
                }

                p = dstLine[extend + width - 1];
                for (int x = width + extend; x < maskWidth; x++)
                {
                    dstLine[x] = p;
                }
                dstLine += maskWidth;
            }

            // 复制四个角落的像素

            // 左上
            p = dstContentStart[0];
            for (int y = 0; y < extend; y++)
            {
                for (int x = 0; x < extend; x++)
                {
                    maskStart[y * maskWidth + x] = p;
                }
            }

            // 右上
            p = dstContentStart[width - 1];
            for (int y = 0; y < extend; y++)
            {
                for (int x = width + extend; x < maskWidth; x++)
                {
                    maskStart[y * maskWidth + x] = p;
                }
            }

            // 左下
            p = dstContentStart[(height - 1) * maskWidth];
            for (int y = height + extend; y < maskHeight; y++)
            {
                for (int x = 0; x < extend; x++)
                {
                    maskStart[y * maskWidth + x] = p;
                }
            }

            // 右下
            p = dstContentStart[(height-1) * maskWidth + width - 1];
            for (int y = height + extend; y < maskHeight; y++)
            {
                for (int x = width + extend; x < maskWidth; x++)
                {
                    maskStart[y * maskWidth + x] = p;
                }
            }

            if (scale == 1)
            {
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        TValue val = 0;
                        for (int kw = 0; kw < kernelWidth; kw++)
                        {
                            for (int kh = 0; kh < kernelHeight; kh++)
                            {
                                val += maskStart[(h + kh) * maskWidth + (w + kw)] * kernel[kh, kw];
                            }
                        }
                        start[h * widthTypeScaled + w] = (TPixel)val;
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
                        TValue val = 0;
                        for (int kw = 0; kw < kernelWidth; kw++)
                        {
                            for (int kh = 0; kh < kernelHeight; kh++)
                            {
                                val += maskStart[(h + kh) * maskWidth + (w + kw)] * kernel[kh, kw];
                            }
                        }
                        start[h * widthTypeScaled + w] = (TPixel)(val * factor);
                    }
                }
            }
            maskImage.Dispose();
        }

        public unsafe ImageGrad CreateGradImage(Boolean computeDirection = true)
        {
            int step = this.Step;
            using (ImageChannel32 gradX = new ImageChannel32(this.Width, this.Height))
            using (ImageChannel32 gradY = new ImageChannel32(this.Width, this.Height))
            {
                CopyToIntChannel(this, gradX);
                CopyToIntChannel(this, gradY);
                gradX.ApplyConvolution(ConvolutionKernel.SobelX);
                gradY.ApplyConvolution(ConvolutionKernel.SobelY);
                ImageGrad grad = new ImageGrad(this.Width, this.Height);
                Int32* xStart = (Int32*)gradX.StartIntPtr;
                Int32* yStart = (Int32*)gradY.StartIntPtr;
                Grad* g = grad.Start;
                Grad* gEnd = g + grad.Length;
                if (computeDirection == true)
                {
                    while (g != gEnd)
                    {
                        Int32 x = *xStart;
                        Int32 y = *yStart;
                        float f = (float)Math.Sqrt(x * x + y * y);
                        float theta = (float)Math.Atan2(x, y);
                        g->Value = f;
                        g->Theta = theta;
                        xStart++;
                        yStart++;
                        g++;
                    }
                }
                else
                {
                    while (g != gEnd)
                    {
                        Int32 x = *xStart;
                        Int32 y = *yStart;
                        float f = (float)Math.Sqrt(x * x + y * y);
                        g->Value = f;
                        g->Theta = 0;
                        xStart++;
                        yStart++;
                        g++;
                    }
                }

                return grad;
            }
        }
    }
}

