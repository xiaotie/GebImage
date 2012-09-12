/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using TPixel = System.Byte;
using TCache = System.Int32;
using TKernel = System.Int32;
using TImage = Geb.Image.ImageU8;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image.Hidden
{
    public abstract class Image_Template : TImage
    {
        private Image_Template()
            : base(1,1)
        {
            throw new NotImplementedException();
        }

        #region mixin

        public unsafe TPixel* Start { get { return (TPixel*)this.StartIntPtr; } }

        public unsafe TPixel this[int index]
        {
            get
            {
                return Start[index];
            }
            set
            {
                Start[index] = value;
            }
        }

        public unsafe TPixel this[int row, int col]
        {
            get
            {
                return Start[row * this.Width + col];
            }
            set
            {
                Start[row * this.Width + col] = value;
            }
        }

        public unsafe TPixel this[System.Drawing.Point location]
        {
            get
            {
                return Start[location.Y * this.Width + location.X];
            }
            set
            {
                Start[location.Y * this.Width + location.X] = value;
            }
        }

        public unsafe TPixel this[PointS location]
        {
            get
            {
                return Start[location.Y * this.Width + location.X];
            }
            set
            {
                Start[location.Y * this.Width + location.X] = value;
            }
        }

        public unsafe TPixel* Row(Int32 row)
        {
            if (row < 0 || row >= this.Height) throw new ArgumentOutOfRangeException("row");
            return Start + row * this.Width;
        }

        public unsafe TImage CloneFrom(UnmanagedImage<TPixel> src)
        {
            if (src == null) throw new ArgumentNullException("src");
            if (src.ByteCount != this.ByteCount) throw new NotSupportedException("与src图像的像素数量不一致，无法复制.");

            TPixel* start = Start;
            TPixel* end = start + Length;
            TPixel* from = (TPixel*)(src.StartIntPtr);

            while (start != end)
            {
                *start = *from;
                ++start;
                ++from;
            }
            return this;
        }

        /// <summary>
        /// 对图像进行转置，行变成列，列变成行。转置结果直接存在当前的图像中。
        /// </summary>
        /// <returns></returns>
        public unsafe TImage ApplyTranspose()
        {
            TImage img = new TImage(Width, Height);
            img.CloneFrom(this);
            this.Width = img.Height;
            this.Height = img.Width;
            img.TransposeTo(this);
            img.Dispose();
            return this;
        }

        /// <summary>
        /// 将当前图像转置到另一幅图像中。也就是说，imgDst[i,j] = this[j,i]
        /// </summary>
        /// <param name="imgDst">转置标的图像，图像宽是本图像的高，图像的高是本图像的宽</param>
        /// <returns>转置标的图像</returns>
        public unsafe TImage TransposeTo(TImage imgDst)
        {
            if (this.Width != imgDst.Height || this.Height != imgDst.Width)
            {
                throw new ArgumentException("两幅图像的尺寸不匹配，无法进行转置操作.");
            }

            int width = this.Width;
            int height = this.Height;

            TPixel* src = this.Start;
            TPixel* dst = imgDst.Start;

            for (int y = 0; y < height; y++)
            {
                TPixel* srcLine = src + y * width;
                TPixel* dstLine = dst + y;
                for (int x = 0; x < width; x++)
                {
                    *dstLine = srcLine[x];
                    dstLine += height;
                }
            }

            return imgDst;
        }

        public unsafe TImage Fill(TPixel pixel)
        {
            TPixel* p = this.Start;
            TPixel* end = p + this.Length;
            while (p != end)
            {
                *p = pixel;
                p++;
            }
            return this;
        }

        public unsafe TImage Replace(TPixel pixel, TPixel replaced)
        {
            TPixel* p = this.Start;
            TPixel* end = p + this.Length;
            while (p != end)
            {
                if (*p == pixel)
                {
                    *p = replaced;
                }
                p++;
            }
            return this;
        }

        public unsafe TImage CopyFrom(UnmanagedImage<TPixel> src, System.Drawing.Point start, System.Drawing.Rectangle region, System.Drawing.Point destAnchor)
        {
            if (start.X >= src.Width || start.Y >= src.Height) return this;
            int startSrcX = Math.Max(0, start.X);
            int startSrcY = Math.Max(0, start.Y);
            int endSrcX = Math.Min(start.X + region.Width, src.Width);
            int endSrcY = Math.Min(start.Y + region.Height, src.Height);
            int offsetX = start.X < 0? -start.X : 0;
            int offsetY = start.Y < 0? -start.Y : 0;
            offsetX = destAnchor.X + offsetX;
            offsetY = destAnchor.Y + offsetY;
            int startDstX = Math.Max(0, offsetX);
            int startDstY = Math.Max(0, offsetY);
            offsetX = offsetX < 0 ? -offsetX : 0;
            offsetY = offsetY < 0 ? -offsetY : 0;
            startSrcX += offsetX;
            startSrcY += offsetY;
            int endDstX = Math.Min(destAnchor.X + region.Width, this.Width);
            int endDstY = Math.Min(destAnchor.Y + region.Height, this.Height);
            int copyWidth = Math.Min(endSrcX - startSrcX, endDstX - startDstX);
            int copyHeight = Math.Min(endSrcY - startSrcY, endDstY - startDstY);
            if (copyWidth <= 0 || copyHeight <= 0) return this;

            int srcWidth = src.Width;
            int dstWidth = this.Width;

            TPixel* srcLine = (TPixel*)(src.StartIntPtr) + srcWidth * startSrcY + startSrcX;
            TPixel* dstLine = this.Start + dstWidth * startDstY + startDstX;
            TPixel* endSrcLine = srcLine + srcWidth * copyHeight;
            while (srcLine < endSrcLine)
            {
                TPixel* pSrc = srcLine;
                TPixel* endPSrc = pSrc + copyWidth;
                TPixel* pDst = dstLine;
                while (pSrc < endPSrc)
                {
                    *pDst = *pSrc;
                    pSrc++;
                    pDst++;
                }
                srcLine += srcWidth;
                dstLine += dstWidth;
            }
            return this;
        }

        public TImage FloodFill(System.Drawing.Point location, TPixel anchorColor, TPixel replecedColor)
        {
            int width = this.Width;
            int height = this.Height;
            if (location.X < 0 || location.X >= width || location.Y < 0 || location.Y >= height) return this;

            if (anchorColor == replecedColor) return this;
            if (this[location.Y, location.X] != anchorColor) return this;

            Stack<System.Drawing.Point> points = new Stack<System.Drawing.Point>();
            points.Push(location);

            int ww = width - 1;
            int hh = height - 1;

            while (points.Count > 0)
            {
                System.Drawing.Point p = points.Pop();
                this[p.Y, p.X] = replecedColor;
                if (p.X > 0 && this[p.Y, p.X - 1] == anchorColor)
                {
                    this[p.Y, p.X - 1] = replecedColor;
                    points.Push(new System.Drawing.Point(p.X - 1, p.Y));
                }

                if (p.X < ww && this[p.Y, p.X + 1] == anchorColor)
                {
                    this[p.Y, p.X + 1] = replecedColor;
                    points.Push(new System.Drawing.Point(p.X + 1, p.Y));
                }

                if (p.Y > 0 && this[p.Y - 1, p.X] == anchorColor)
                {
                    this[p.Y - 1, p.X] = replecedColor;
                    points.Push(new System.Drawing.Point(p.X, p.Y - 1));
                }

                if (p.Y < hh && this[p.Y + 1, p.X] == anchorColor)
                {
                    this[p.Y + 1, p.X] = replecedColor;
                    points.Push(new System.Drawing.Point(p.X, p.Y + 1));
                }
            }
            return this;
        }

        /// <summary>
        /// 使用众值滤波
        /// </summary>
        public unsafe TImage ApplyModeFilter(int size)
        {
            if (size <= 1) throw new ArgumentOutOfRangeException("size 必须大于1.");
            else if (size > 127) throw new ArgumentOutOfRangeException("size 最大为127.");
            else if (size % 2 == 0) throw new ArgumentException("size 应该是奇数.");

            int* vals = stackalloc int[size * size + 1];
            TPixel* keys = stackalloc TPixel[size * size + 1];

            UnmanagedImage<TPixel> mask = this.Clone() as UnmanagedImage<TPixel>;
            int height = this.Height;
            int width = this.Width;

            TPixel* pMask = (TPixel*)mask.StartIntPtr;
            TPixel* pThis = (TPixel*)this.StartIntPtr;

            int radius = size / 2;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int count = 0;

                    // 建立直方图
                    for (int y = -radius; y <= radius; y++)
                    {
                        for (int x = -radius; x <= radius; x++)
                        {
                            int yy = y + h;
                            int xx = x + w;
                            if (xx >= 0 && xx < width && yy >= 0 && yy < height)
                            {
                                TPixel color = pMask[yy * width + xx];

                                bool find = false;
                                for (int i = 0; i < count; i++)
                                {
                                    if (keys[i] == color)
                                    {
                                        vals[i]++;
                                        find = true;
                                        break;
                                    }
                                }

                                if (find == false)
                                {
                                    keys[count] = color;
                                    vals[count] = 1;
                                    count++;
                                }
                            }
                        }
                    }

                    if (count > 0)
                    {
                        // 求众数
                        int index = -1;
                        int max = int.MinValue;
                        for (int i = 0; i < count; i++)
                        {
                            if (vals[i] > max)
                            {
                                index = i;
                                max = vals[i];
                            }
                        }

                        if (max > 1)
                        {
                            pThis[h * width + w] = keys[index];
                        }
                    }
                }
            }

            mask.Dispose();

            return this;
        }

        public void DrawRect(RectF rect, TPixel color, int radius)
        {
            DrawLine(new PointF(rect.X, rect.Y), new PointF(rect.X + rect.Width, rect.Y), color, radius);
            DrawLine(new PointF(rect.X, rect.Y), new PointF(rect.X, rect.Y + rect.Height), color, radius);
            DrawLine(new PointF(rect.X + rect.Width, rect.Y), new PointF(rect.X + rect.Width, rect.Y + rect.Height), color, radius);
            DrawLine(new PointF(rect.X, rect.Y + rect.Height), new PointF(rect.X + rect.Width, rect.Y + rect.Height), color, radius);
        }

        public void DrawLine(PointF start, PointF end, TPixel color, int radius)
        {
            float deltaX = end.X - start.X;
            float deltaY = end.Y - start.Y;
            int ww = this.Width - 1;
            int hh = this.Height - 1;

            if (Math.Abs(deltaX) < 0.0001)
            {
                if (Math.Abs(deltaY) < 0.0001)
                {
                    SetColor(start.X, start.Y, color, radius, ww, hh);
                    return;
                };

                float yStart = start.Y;
                float yEnd = end.Y;
                float x = start.X;

                if (yEnd < yStart)
                {
                    float tmp = yEnd;
                    yEnd = yStart;
                    yStart = tmp;
                }

                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(hh, yEnd);

                for (float y = yStart; y <= yEnd; y++)
                {
                    SetColor(x, y, color, radius, ww, hh);
                }
            }
            else
            {
                float xStart = start.X;
                float xEnd = end.X;
                if (xEnd < xStart)
                {
                    float tmp = xEnd;
                    xEnd = xStart;
                    xStart = tmp;
                }

                float step = 1;
                float grad = Math.Abs(deltaY / deltaX);
                if (grad > 1)
                {
                    step = 1 / grad;
                }


                for (float x = xStart; x <= xEnd; x += step)
                {
                    float deltaXX = start.X - x;
                    float deltaYY = deltaY * (deltaXX / deltaX);
                    float y = start.Y - deltaYY;

                    SetColor(x, y, color, radius, ww, hh);
                }
            }
        }

        public void Draw(float x, float y, TPixel color, int radius)
        {
            SetColor(x, y, color, radius, Width - 1, Height - 1);
        }

        private void SetColor(float x, float y, TPixel color, int radius, int ww, int hh)
        {
            int xStart = (int)(x - radius - 1);
            int xEnd = (int)(x + radius + 1);
            int yStart = (int)(y - radius - 1);
            int yEnd = (int)(y + radius + 1);

            int maxDistanceSquare = radius * radius;
            for (int yy = yStart; yy < yEnd; yy++)
            {
                for (int xx = xStart; xx < xEnd; xx++)
                {
                    if (xx < 0 || yy < 0 || xx > ww || yy > hh) continue;
                    float deltaX = xx - x;
                    float deltaY = yy - y;
                    if (deltaX * deltaX + deltaY * deltaY <= maxDistanceSquare)
                        this[yy, xx] = color;
                }
            }
        }

        public unsafe TPixel[] ToArray()
        {
            TPixel[] array = new TPixel[this.Length];
            for (int i = 0; i < Length; i++)
            {
                array[i] = this[i];
            }
            return array;
        }

        #endregion
    }
}
