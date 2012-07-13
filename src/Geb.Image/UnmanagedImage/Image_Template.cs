/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using TPixel = System.Byte;
using TCache = System.Int32;
using TKernel = System.Int32;
using TImage = Geb.Image.ImageU8;

using System;
using System.Collections.Generic;
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
                return this[location.Y, location.X];
            }
            set
            {
                this[location.Y, location.X] = value;
            }
        }

        public unsafe TPixel* Row(Int32 row)
        {
            if (row < 0 || row >= this.Height) throw new ArgumentOutOfRangeException("row");
            return Start + row * this.Width;
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

        #endregion
    }
}
