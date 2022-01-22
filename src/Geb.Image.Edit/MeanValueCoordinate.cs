/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Geb.Image.Edit
{
    using Geb.Image;

    public class MeanValueCoordinate
    {
        private struct PointCoeff
        {
            public PointS Point;
            public Int16 RedDiff;
            public Int16 GreenDiff;
            public Int16 BlueDiff;
        }

        /// <summary>
        /// 使用均值坐标，将imgMask中的图像合成入imgBg中。
        /// </summary>
        /// <param name="imgBg">背景图像</param>
        /// <param name="imgMask">前景图像。Alpha通道为0或255，为255的图像区域为待合成区域</param>
        public unsafe void Blend(ImageBgra32 imgBg, ImageBgra32 imgMask, int xOffset = 0, int yOffset = 0)
        {
            // 对alpha通道进行距离变换，以求得边缘像素的位置
            ApplyDistanceTransformFast(imgMask);

            List<PointCoeff> edgePoints = new List<PointCoeff>();
            int height = imgMask.Height;
            int width = imgMask.Width;
            int wMin = Math.Max(0, -xOffset);
            int hMin = Math.Max(0, -yOffset);
            int wMax = Math.Min(imgMask.Width, imgBg.Width - xOffset);
            int hMax = Math.Min(imgMask.Height, imgBg.Height - yOffset);

            for (int h = 0; h < height; h++)
            {
                Bgra32* p0 = imgMask.Start + h * width;
                Bgra32* p = p0;
                Bgra32* pEnd = p + width;
                while (p < pEnd)
                {
                    if (p->Alpha == 1)
                    {
                        PointCoeff item = new PointCoeff();
                        item.Point = new PointS((UInt16)(p - p0), (UInt16)h);
                        edgePoints.Add(item);
                    }
                    p++;
                }
            }

            if (edgePoints.Count == 0) return;

            // 计算边界点的像素的差值
            for (int i = 0; i < edgePoints.Count; i++)
            {
                PointCoeff item = edgePoints[i];
                Bgra32 pMask = imgMask[item.Point];
                Bgra32 pBg = imgBg[item.Point];
                item.RedDiff = (Int16)(pBg.Red - pMask.Red);
                item.GreenDiff = (Int16)(pBg.Green - pMask.Green);
                item.BlueDiff = (Int16)(pBg.Blue - pMask.Blue);
                edgePoints[i] = item;
            }

            // 对于每一个alpha大于1的像素进行处理
            float sumRed = 0;
            float sumGreen = 0;
            float sumBlue = 0;
            float coeff = 0;
            for (int h = 0; h < hMax; h++)
            {
                for(int w = 0; w < wMax; w++)
                {
                    Bgra32 p = imgMask[h,w];
                    if (p.Alpha > 1)
                    {
                        sumRed = 0;
                        sumGreen = 0;
                        sumBlue = 0;
                        coeff = 0;
                        foreach (PointCoeff item in edgePoints)
                        {
                            int divX = item.Point.X - w;
                            int divY = item.Point.Y - h;
                            float tmp = 1.0f / (divX * divX + divY * divY);
                            tmp = tmp * tmp * tmp * tmp;
                            coeff += tmp;
                            sumRed += item.RedDiff * tmp ;
                            sumGreen += item.GreenDiff * tmp;
                            sumBlue += item.BlueDiff * tmp;
                        }
                        sumRed = sumRed / coeff;
                        sumGreen = sumGreen / coeff;
                        sumBlue = sumBlue / coeff;

                        Bgra32 pBlend = p;
                        Bgra32 pSrc = pBlend;

                        pBlend.Alpha = 255;
                        pBlend.Red = (Byte)Math.Max(0, Math.Min(255, (int)(pBlend.Red + sumRed)));
                        pBlend.Green = (Byte)Math.Max(0, Math.Min(255, (int)(pBlend.Green + sumGreen)));
                        pBlend.Blue = (Byte)Math.Max(0, Math.Min(255, (int)(pBlend.Blue + sumBlue)));
                        imgBg[h, w] = pBlend;
                    }
                }
            }
        }

        /// <summary>
        /// 均值坐标抠图。
        /// </summary>
        /// <param name="img">原图像</param>
        /// <param name="imgMask">图像mask。其中，前景为白色，背景为黑色，未定区域为灰色</param>
        public unsafe void Mate(ImageBgra32 img, ImageBgra32 imgMask)
        {
            if (img.Width != imgMask.Width || img.Height != imgMask.Height)
            {
                throw new ArgumentException("Images are not the same size.");
            }

            List<PointCoeff> frontEdgePoints = new List<PointCoeff>(imgMask.Width + imgMask.Height);
            List<PointCoeff> bgEdgePoints = new List<PointCoeff>(imgMask.Width + imgMask.Height);
            int width = imgMask.Width;
            int height = imgMask.Height;
            Bgra32* pMask0 = imgMask.Start;
            for (int h = 1; h < height - 1; h++)
            {
                Bgra32* p = pMask0 + width * h + 1;
                
                for (int w = 1; w < width - 1; w++)
                {
                    if (p->Red == 255)  // 前景
                    {
                        if (p[-1].Red < 255 || p[1].Red < 255
                            || p[width].Red < 255
                            || p[-width].Red < 255)
                        {
                            Bgra32 pRaw = img[h,w];
                            PointCoeff item = new PointCoeff { Point = new PointS(w,h) };
                            item.RedDiff = pRaw.Red;
                            item.BlueDiff = pRaw.Blue;
                            item.GreenDiff = pRaw.Green;
                            frontEdgePoints.Add(item);
                        }
                    }
                    else if (p->Red == 0)  // 背景
                    {
                        if (p[-1].Red > 0 || p[1].Red > 0
                            || p[width].Red > 0
                            || p[-width].Red > 0)
                        {
                            Bgra32 pRaw = img[h, w];
                            PointCoeff item = new PointCoeff { Point = new PointS(w, h) };
                            item.RedDiff = pRaw.Red;
                            item.BlueDiff = pRaw.Blue;
                            item.GreenDiff = pRaw.Green;
                            bgEdgePoints.Add(item);
                        }
                    }
                    p++;
                }
            }

            if (frontEdgePoints.Count == 0 || bgEdgePoints.Count == 0) return;

            // 对灰色区域进行处理，通过均值坐标计算 alpha

            float sumRedF = 0;
            float sumGreenF = 0;
            float sumBlueF = 0;
            float coeffF = 0;

            float sumRedB = 0;
            float sumGreenB = 0;
            float sumBlueB = 0;
            float coeffB = 0;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Bgra32 p = imgMask[h,w];
                    Bgra32 pRaw = img[h, w];
                    if (p.Red == 0)
                    {
                        pRaw.Alpha = 0;
                    }
                    else if (p.Red < 255)
                    {
                        sumRedF = 0;
                        sumGreenF = 0;
                        sumBlueF = 0;
                        coeffF = 0;

                        foreach (PointCoeff item in frontEdgePoints)
                        {
                            int divX = item.Point.X - w;
                            int divY = item.Point.Y - h;
                            float tmp = 1.0f / (divX * divX + divY * divY);
                            //tmp = tmp * tmp * tmp * tmp;
                            //tmp = tmp * tmp ;
                            coeffF += tmp;
                            sumRedF += item.RedDiff * tmp;
                            sumGreenF += item.GreenDiff * tmp;
                            sumBlueF += item.BlueDiff * tmp;
                        }
                        sumRedF = sumRedF / coeffF;
                        sumGreenF = sumGreenF / coeffF;
                        sumBlueF = sumBlueF / coeffF;

                        sumRedB = 0;
                        sumGreenB = 0;
                        sumBlueB = 0;
                        coeffB = 0;
                        foreach (PointCoeff item in bgEdgePoints)
                        {
                            int divX = item.Point.X - w;
                            int divY = item.Point.Y - h;
                            float tmp = 1.0f / (divX * divX + divY * divY);
                            //tmp = tmp * tmp * tmp * tmp;
                            //tmp = tmp * tmp;
                            coeffB += tmp;
                            sumRedB += item.RedDiff * tmp;
                            sumGreenB += item.GreenDiff * tmp;
                            sumBlueB += item.BlueDiff * tmp;
                        }

                        sumRedB = sumRedB / coeffB;
                        sumGreenB = sumGreenB / coeffB;
                        sumBlueB = sumBlueB / coeffB;

                        // alpha = (I - B)/(F - B) ; I 即为 pRaw
                        double divFB = (sumRedB - sumRedF) * (sumRedB - sumRedF)
                            + (sumGreenB - sumGreenF) * (sumGreenB - sumGreenF)
                            + (sumBlueB - sumBlueF) * (sumBlueB - sumBlueF);
                        divFB = Math.Sqrt(divFB);
                        double divIB = (pRaw.Red - sumRedB) * (pRaw.Red - sumRedB)
                            + (pRaw.Green - sumGreenB) * (pRaw.Green - sumGreenB)
                            + (pRaw.Blue - sumBlueB) * (pRaw.Blue - sumBlueB);
                        divIB = Math.Sqrt(divIB);
                        byte alpha = 255;
                        if (divFB > 0)
                        {
                            alpha = (byte) Math.Min(255, 255 * divIB / divFB);
                        }
                        pRaw.Alpha = alpha;
                    }
                    img[h, w] = pRaw;
                }
            }
        }

        private unsafe ImageBgra32 ApplyDistanceTransformFast(ImageBgra32 img)
        {
            Bgra32* start = img.Start;
            int width = img.Width;
            int height = img.Height;

            Int32 val;

            // 从上向下，从左向右扫描
            for (int h = 1; h < height - 1; h++)
            {
                // 位于每行的头部
                Bgra32* line0 = start + (h - 1) * width;
                Bgra32* line1 = start + (h) * width;
                Bgra32* line2 = start + (h + 1) * width;
                for (int w = 1; w < width; w++)
                {
                    if (line1[1].Alpha > 0) // 当前像素
                    {
                        val = Math.Min(line0[0].Alpha, line0[1].Alpha);
                        val = Math.Min(val, line1[0].Alpha);
                        val = Math.Min(val, line2[0].Alpha);
                        val = Math.Min(val + 1, line1[1].Alpha);
                        line1[1].Alpha = (byte)(Math.Min(val, 255));
                    }

                    line0++;
                    line1++;
                    line2++;
                }
            }

            // 从下向上，从右向左扫描
            for (int h = height - 2; h > 0; h--)
            {
                Bgra32* line0 = start + (h - 1) * width;
                Bgra32* line1 = start + (h) * width;
                Bgra32* line2 = start + (h + 1) * width;

                for (int w = width - 2; w >= 0; w--)
                {
                    if (line1[w].Alpha > 0)
                    {
                        val = Math.Min(line0[w + 1].Alpha, line1[w + 1].Alpha);
                        val = Math.Min(val, line2[w + 1].Alpha);
                        val = Math.Min(val, line2[w].Alpha);
                        val = Math.Min(val + 1, line1[w].Alpha);
                        line1[w].Alpha = (byte)(Math.Min(val, 255)); ;
                    }
                }
            }
            return img;
        }
    }
}
