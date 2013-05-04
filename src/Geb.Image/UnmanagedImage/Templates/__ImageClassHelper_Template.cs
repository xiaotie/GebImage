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
    static class ImageClassHelper_Template
    {
        #region mixin

        /// <summary>
        /// 对每个像素进行操作
        /// </summary>
        /// <param name="p">指向像素的指针</param>
        public unsafe delegate void ActionOnPixel(TPixel* p);

        /// <summary>
        /// 对每个位置的像素进行操作
        /// </summary>
        /// <param name="row">像素所在行</param>
        /// <param name="column">像素所在列</param>
        /// <param name="p">指向像素的指针</param>
        public unsafe delegate void ActionWithPosition(Int32 row, Int32 column, TPixel* p);

        /// <summary>
        /// 对每个像素进行判断
        /// </summary>
        /// <param name="p">指向像素的指针</param>
        /// <returns></returns>
        public unsafe delegate Boolean PredicateOnPixel(TPixel* p);

        /// <summary>
        /// 遍历图像，对每个像素进行操作
        /// </summary>
        /// <param name="src">图像</param>
        /// <param name="handler">void ActionOnPixel(TPixel* p)</param>
        /// <returns>处理后的图像（同传入图像是一个对象）</returns>
        public unsafe static TImage ForEach(this TImage src, ActionOnPixel handler)
        {
            TPixel* start = src.Start;
            if (start == null) return src;

            TPixel* end = start + src.Length;
            while (start != end)
            {
                handler(start);
                ++start;
            }
            return src;
        }

        /// <summary>
        /// 遍历图像，对每个位置的像素进行操作
        /// </summary>
        /// <param name="src">图像</param>
        /// <param name="handler">void ActionWithPosition(Int32 row, Int32 column, TPixel* p)</param>
        /// <returns>处理后的图像（同传入图像是一个对象）</returns>
        public unsafe static TImage ForEach(this TImage src, ActionWithPosition handler)
        {
            Int32 width = src.Width;
            Int32 height = src.Height;

            TPixel* p = src.Start;
            if (p == null) return src;

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

        /// <summary>
        /// 遍历图像中的一段，对每个像素进行操作
        /// </summary>
        /// <param name="src">图像</param>
        /// <param name="start">指向开始像素的指针</param>
        /// <param name="length">处理的像素数量</param>
        /// <param name="handler">void ActionOnPixel(TPixel* p)</param>
        /// <returns>处理后的图像（同传入图像是一个对象）</returns>
        public unsafe static TImage ForEach(this TImage src, TPixel* start, uint length, ActionOnPixel handler)
        {
            if (start == null) return src;

            TPixel* end = start + src.Length;
            while (start != end)
            {
                handler(start);
                ++start;
            }
            return src;
        }

        /// <summary>
        /// 统计符合条件的像素数量
        /// </summary>
        /// <param name="src">图像</param>
        /// <param name="handler">Boolean PredicateOnPixel(TPixel* p)</param>
        /// <returns>符合条件的像素数量</returns>
        public unsafe static Int32 Count(this TImage src, PredicateOnPixel handler)
        {
            TPixel* start = src.Start;
            TPixel* end = start + src.Length;

            if (start == null) return 0;

            Int32 count = 0;
            while (start != end)
            {
                if (handler(start) == true) count++;
                ++start;
            }
            return count;
        }

        /// <summary>
        /// 统计符合条件的像素数量
        /// </summary>
        /// <param name="src">图像</param>
        /// <param name="handler">Boolean Predicate<TPixel></param>
        /// <returns>符合条件的像素数量</returns>
        public unsafe static Int32 Count(this TImage src, Predicate<TPixel> handler)
        {
            TPixel* start = src.Start;
            TPixel* end = start + src.Length;
            if (start == null) return 0;

            Int32 count = 0;
            while (start != end)
            {
                if (handler(*start) == true) count++;
                ++start;
            }
            return count;
        }

        /// <summary>
        /// 查找模板。模板中值代表实际像素值。负数代表任何像素。返回查找得到的像素的左上端点的位置。
        /// </summary>
        /// <param name="template">TPixel[,]</param>
        /// <returns>查找到的模板集合</returns>
        public static unsafe List<PointS> FindTemplate(this TImage src, TPixel[,] template)
        {
            List<PointS> finds = new List<PointS>();
            int tHeight = template.GetUpperBound(0) + 1;
            int tWidth = template.GetUpperBound(1) + 1;
            int toWidth = src.Width - tWidth + 1;
            int toHeight = src.Height - tHeight + 1;
            int stride = src.Width;
            TPixel* start = src.Start;
            for (int r = 0; r < toHeight; r++)
            {
                for (int c = 0; c < toWidth; c++)
                {
                    TPixel* srcStart = start + r * stride + c;
                    for (int rr = 0; rr < tHeight; rr++)
                    {
                        for (int cc = 0; cc < tWidth; cc++)
                        {
                            TPixel pattern = template[rr, cc];
                            if (srcStart[rr * stride + cc] != pattern)
                            {
                                goto Next;
                            }
                        }
                    }

                    finds.Add(new PointS(c, r));

                Next:
                    continue;
                }
            }

            return finds;
        }

        #endregion
    }
}
