/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using TPixel = System.Byte;
using TCache = System.Int32;
using TKernel = System.Int32;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Hidden
{
    static class ImageClassHelper_Template
    {
        #region mixin

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

        #endregion
    }
}
