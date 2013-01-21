/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Byte;
    using TChannel = System.Byte;
    using TChannelTemp = System.Int32;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageU8;

    public static partial class ImageU8ClassHelper
    {
        

        public unsafe delegate void ActionOnPixel(TPixel* p);
        public unsafe delegate void ActionWithPosition(Int32 row, Int32 column, TPixel* p);
        public unsafe delegate Boolean PredicateOnPixel(TPixel* p);

        public unsafe static UnmanagedImage<TPixel> ForEach(this UnmanagedImage<TPixel> src, ActionOnPixel handler)
        {
            TPixel* start = (TPixel*)src.StartIntPtr;
            TPixel* end = start + src.Length;
            while (start != end)
            {
                handler(start);
                ++start;
            }
            return src;
        }

        public unsafe static UnmanagedImage<TPixel> ForEach(this UnmanagedImage<TPixel> src, ActionWithPosition handler)
        {
            Int32 width = src.Width;
            Int32 height = src.Height;

            TPixel* p = (TPixel*)src.StartIntPtr;
            for (Int32 r = 0; r < height; r++)
            {
                for (Int32 w = 0; w < width; w++)
                {
                    handler(w, r, p);
                    p++;
                }
            }
            return src;
        }

        public unsafe static UnmanagedImage<TPixel> ForEach(this UnmanagedImage<TPixel> src, TPixel* start, uint length, ActionOnPixel handler)
        {
            TPixel* end = start + src.Length;
            while (start != end)
            {
                handler(start);
                ++start;
            }
            return src;
        }

        public unsafe static Int32 Count(this UnmanagedImage<TPixel> src, PredicateOnPixel handler)
        {
            TPixel* start = (TPixel*)src.StartIntPtr;
            TPixel* end = start + src.Length;
            Int32 count = 0;
            while (start != end)
            {
                if (handler(start) == true) count++;
                ++start;
            }
            return count;
        }

        public unsafe static Int32 Count(this UnmanagedImage<TPixel> src, Predicate<TPixel> handler)
        {
            TPixel* start = (TPixel*)src.StartIntPtr;
            TPixel* end = start + src.Length;
            Int32 count = 0;
            while (start != end)
            {
                if (handler(*start) == true) count++;
                ++start;
            }
            return count;
        }

        public unsafe static List<TPixel> Where(this UnmanagedImage<TPixel> src, PredicateOnPixel handler)
        {
            List<TPixel> list = new List<TPixel>();

            TPixel* start = (TPixel*)src.StartIntPtr;
            TPixel* end = start + src.Length;
            while (start != end)
            {
                if (handler(start) == true) list.Add(*start);
                ++start;
            }

            return list;
        }

        public unsafe static List<TPixel> Where(this UnmanagedImage<TPixel> src, Predicate<TPixel> handler)
        {
            List<TPixel> list = new List<TPixel>();

            TPixel* start = (TPixel*)src.StartIntPtr;
            TPixel* end = start + src.Length;
            while (start != end)
            {
                if (handler(*start) == true) list.Add(*start);
                ++start;
            }

            return list;
        }

        /// <summary>
        /// 查找模板。模板中值代表实际像素值。负数代表任何像素。返回查找得到的像素的左上端点的位置。
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static unsafe List<System.Drawing.Point> FindTemplate(this UnmanagedImage<TPixel> src, int[,] template)
        {
            List<System.Drawing.Point> finds = new List<System.Drawing.Point>();
            int tHeight = template.GetUpperBound(0) + 1;
            int tWidth = template.GetUpperBound(1) + 1;
            int toWidth = src.Width - tWidth + 1;
            int toHeight = src.Height - tHeight + 1;
            int stride = src.Width;
            TPixel* start = (TPixel*)src.StartIntPtr;
            for (int r = 0; r < toHeight; r++)
            {
                for (int c = 0; c < toWidth; c++)
                {
                    TPixel* srcStart = start + r * stride + c;
                    for (int rr = 0; rr < tHeight; rr++)
                    {
                        for (int cc = 0; cc < tWidth; cc++)
                        {
                            int pattern = template[rr, cc];
                            if (pattern >= 0 && srcStart[rr * stride + cc] != pattern)
                            {
                                goto Next;
                            }
                        }
                    }

                    finds.Add(new System.Drawing.Point(c, r));

                Next:
                    continue;
                }
            }

            return finds;
        }

        
    }

    public partial class ImageU8
    {
        

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

        /// <summary>
        /// 代表当前图像内容的二维数组。
        /// .Net 的 IDE 均不支持直接查看.Net程序中的指针内容，DataSnapshot 提供了调试时查看
        /// 图像数据的唯一途径。请谨慎使用本方法。
        /// </summary>
        public unsafe TPixel[,] DataSnapshot
        {
            get
            {
                TPixel[,] data = new TPixel[Height, Width];
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        data[h, w] = this[h, w];
                    }
                }
                return data;
            }
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

            if (srcLine[0] is Argb32)
            {
                int beta;
                while (srcLine < endSrcLine)
                {
                    Argb32* pSrc = (Argb32*)srcLine;
                    Argb32* endPSrc = pSrc + copyWidth;
                    Argb32* pDst = (Argb32*)dstLine;
                    while (pSrc < endPSrc)
                    {
                        if (pSrc->Alpha == 255 || pDst->Alpha == 0)
                        {
                            *pDst = *pSrc;
                        }
                        else if (pSrc->Alpha > 0)
                        {
                            beta = 255 - pSrc->Alpha;
                            pDst->Blue = (Byte)((pSrc->Blue * pSrc->Alpha + pDst->Blue * beta) >> 8);
                            pDst->Green = (Byte)((pSrc->Green * pSrc->Alpha + pDst->Green * beta) >> 8);
                            pDst->Red = (Byte)((pSrc->Red * pSrc->Alpha + pDst->Red * beta) >> 8);
                        }
                        pSrc++;
                        pDst++;
                    }
                    srcLine += srcWidth;
                    dstLine += dstWidth;
                }
            }
            else
            {
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

        public unsafe void DrawLine(PointF start, PointF end, TPixel color)
        {
            // Bresenham画线算法
            int x1 = (int)Math.Floor(start.X);
            int y1 = (int)Math.Floor(start.Y);
            int x2 = (int)Math.Floor(end.X);
            int y2 = (int)Math.Floor(end.Y);
            int xMin = Math.Min(x1, x2);
            int yMin = Math.Min(y1, y2);
            int xMax = Math.Max(x1, x2);
            int yMax = Math.Max(y1, y2);

            // 线一定在图像外部
            if (xMin >= Width || yMin >= Height || yMax < 0 || xMax < 0)
            {
                return;
            }

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            Boolean dyIsLargerThanDx = false;
            int tmp;
            if (dx < dy)
            {
                dyIsLargerThanDx = true;

                tmp = x1;
                x1 = y1;
                y1 = tmp;

                tmp = x2;
                x2 = y2;
                y2 = tmp;

                tmp = dx;
                dx = dy;
                dy = tmp;
            }

            int ix = (x2 - x1) > 0 ? 1 : -1;
            int iy = (y2 - y1) > 0 ? 1 : -1;
            int cx = x1;
            int cy = y1;
            int n2dy = dy * 2;
            int n2dydx = (dy - dx) * 2;
            int d = dy * 2 - dx;

            // 线在图像内部，则不检查是否指针越位
            if (xMin >= 0 && yMin >= 0 && yMax < (Height - 1) && xMax < Width)
            {
                if (dyIsLargerThanDx == true)
                { // 如果直线与 x 轴的夹角大于 45 度
                    this[cx, cy] = color;
                    while (cx != x2)
                    {
                        cx += ix;
                        if (d < 0)
                        {
                            d += n2dy;
                        }
                        else
                        {
                            cy += iy;
                            d += n2dydx;
                        }
                        this[cx, cy] = color;
                    }
                }
                else
                { // 如果直线与 x 轴的夹角小于 45 度
                    this[cy, cx] = color;
                    while (cx != x2)
                    {
                        cx += ix;
                        if (d < 0)
                        {
                            d += n2dy;
                        }
                        else
                        {
                            cy += iy;
                            d += n2dydx;
                        }
                        this[cy, cx] = color;
                    }
                }
            }
            else
            {
                TPixel* p0 = Start;
                int width = this.Width;
                int height = this.Height;
                int count = 0;

                if (dyIsLargerThanDx == true)
                { // 如果直线与 x 轴的夹角大于 45 度
                    if (cy >= 0 && cy < width && cx >= 0 && cx < height)
                    {
                        p0[width * cx + cy] = color;
                        count++;
                    }
                    while (cx != x2)
                    {
                        cx += ix;

                        if (d < 0)
                        {
                            d += n2dy;
                        }
                        else
                        {
                            cy += iy;
                            d += n2dydx;
                        }

                        if (cy >= 0 && cy < width && cx >= 0 && cx < height)
                        {
                            p0[width * cx + cy] = color;
                            count++;
                        }
                        else
                        {
                            if (count > 0) return;
                        }
                    }
                }
                else
                { // 如果直线与 x 轴的夹角小于 45 度
                    if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                    {
                        p0[width * cy + cx] = color;
                        count++;
                    }
                    while (cx != x2)
                    {
                        cx += ix;

                        if (d < 0)
                        {
                            d += n2dy;
                        }
                        else
                        {
                            cy += iy;
                            d += n2dydx;
                        }

                        if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                        {
                            p0[width * cy + cx] = color;
                            count++;
                        }
                        else
                        {
                            if (count > 0) return;
                        }
                    }
                }
            }
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

        /// <summary>
        /// 绘制圆
        /// </summary>
        /// <param name="x">圆心横坐标</param>
        /// <param name="y">圆心纵坐标</param>
        /// <param name="color">颜色</param>
        /// <param name="radius">圆的半径</param>
        public void DrawCircle(float x, float y, TPixel color, int radius = 1)
        {
            SetColor(x, y, color, radius, Width - 1, Height - 1);
        }

        public void SetColor(float x, float y, TPixel color, int radius = 1)
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

        public unsafe TImage Resize(int width, int height, InterpolationMode mode = InterpolationMode.NearestNeighbor)
        {
            if (width < 1) throw new ArgumentException("width must > 0");
            if (height < 1) throw new ArgumentException("height must > 0");

            // 计算 channel 数量
            int nChannel = sizeof(TPixel) / sizeof(TChannel);
            TImage imgDst = new TImage(width, height);
            TChannel* rootSrc = (TChannel*)this.Start;
            TChannel* rootDst = (TChannel*)imgDst.Start;

            int wSrc = this.Width;
            int hSrc = this.Height;
            int wSrcIdxMax = wSrc - 1;
            int hSrcIdxMax = hSrc - 1;
            float wCoeff = wSrc / width;
            float hCoeff = hSrc / height;

            if (mode == InterpolationMode.NearestNeighbor)
            {
                // 对每个 channel 进行分别处理
                for (int n = 0; n < nChannel; n++)
                {
                    TChannel* s0 = rootSrc + n;
                    TChannel* d0 = rootDst + n;
                    for (int h = 0; h < height; h++)
                    {
                        float yDstF = h * hCoeff;
                        int yDst = (int)Math.Round(yDstF);
                        yDst = Math.Min(yDst, hSrcIdxMax);

                        TChannel* sLine = s0 + yDst * wSrc * nChannel;
                        TChannel* dLine = d0 + h * width * nChannel;

                        for (int w = 0; w < width; w++)
                        {
                            float xDstF = w * wCoeff;
                            int xDst = (int)Math.Round(xDstF);
                            xDst = Math.Min(xDst, wSrcIdxMax);
                            dLine[w * nChannel] = sLine[xDst * nChannel];
                        }
                    }
                }
            }
            else
            {
                //TODO: 实现双线性插值
                throw new NotImplementedException("InterpolationMode not implemented");
            }
            return imgDst;
        }

        

        

        public unsafe void ApplyConvolution(ConvolutionKernel  k)
        {
            int kernelHeight = k.Width;
            int kernelWidth = k.Height;
            int scale = k.Scale;
            int[,] kernel = k.Kernel;
            int extend = Math.Max(kernelWidth, kernelHeight) / 2;
            TImage maskImage = new TImage(Width + extend * 2, Height + extend * 2);
            maskImage.Fill(0);//这里效率不高。原本只需要填充四周扩大的部分即可

            maskImage.CopyFrom(this, new System.Drawing.Point(0, 0), new System.Drawing.Rectangle(0, 0, this.Width, this.Height), new System.Drawing.Point(extend, extend));

            int width = this.Width;
            int height = this.Height;
            TPixel* start = (TPixel*)this.StartIntPtr;

            // 复制边界像素
            TPixel* dstStart = maskImage.Start + extend;
            int extendWidth = this.Width + extend * 2;
            int extendHeight = this.Height + extend * 2;

            // 复制上方的像素
            for (int y = 0; y < extend; y++)
            {
                TPixel* dstP = dstStart + y * extendWidth;
                TPixel* srcStart = start;
                TPixel* srcEnd = srcStart + width;

                while (srcStart != srcEnd)
                {
                    *dstP = *srcStart;
                    srcStart++;
                    dstP++;
                }
            }

            // 复制下方的像素
            for (int y = height + extend; y < extendHeight; y++)
            {
                TPixel* dstP = dstStart + y * extendWidth;
                TPixel* srcStart = start + (height - 1)*width;
                TPixel* srcEnd = srcStart + width;

                while (srcStart != srcEnd)
                {
                    *dstP = *srcStart;
                    srcStart++;
                    dstP++;
                }
            }

            // 复制左右两侧的像素
            TPixel* dstLine = maskImage.Start + extendWidth * extend;
            TPixel* srcLine = start;
            TPixel p = default(TPixel);
            for (int y = extend; y < height + extend; y++)
            {
                for(int x = 0; x < extend; x ++)
                {
                    p = srcLine[0];
                    dstLine[x] = p;
                }

                p = srcLine[width-1];
                for(int x = width + extend; x < extendWidth; x++)
                {
                   dstLine[x] = p;
                }
                dstLine += extendWidth;
                srcLine += width;
            }

            // 复制四个角落的像素

            // 左上
            p = start[0];
            for (int y = 0; y < extend; y++)
            {
                for (int x = 0; x < extend; x++)
                {
                    maskImage[y, x] = p;
                }
            }

            // 右上
            p = start[width - 1];
            for (int y = 0; y < extend; y++)
            {
                for (int x = width + extend; x < extendWidth; x++)
                {
                    maskImage[y, x] = p;
                }
            }

            // 左下
            p = start[(height - 1) * width];
            for (int y = height + extend; y < extendHeight; y++)
            {
                for (int x = 0; x < extend; x++)
                {
                    maskImage[y, x] = p;
                }
            }

            // 右下
            p = start[height * width - 1];
            for (int y = height + extend; y < extendHeight; y++)
            {
                for (int x = width + extend; x < extendWidth; x++)
                {
                    maskImage[y, x] = p;
                }
            }

            if (scale == 1)
            {
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        TChannelTemp val = 0;
                        for (int kw = 0; kw < kernelWidth; kw++)
                        {
                            for (int kh = 0; kh < kernelHeight; kh++)
                            {
                                val += maskImage[h + kh, w + kw] * kernel[kh, kw];
                            }
                        }
                        start[h * width + w] = (TPixel)val;
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
                        TChannelTemp val = 0;
                        for (int kw = 0; kw < kernelWidth; kw++)
                        {
                            for (int kh = 0; kh < kernelHeight; kh++)
                            {
                                val += maskImage[h + kh, w + kw] * kernel[kh, kw];
                            }
                        }
                        start[h * width + w] = (TPixel)(val * factor);
                    }
                }
            }
            maskImage.Dispose();
        }

        
    }
}



