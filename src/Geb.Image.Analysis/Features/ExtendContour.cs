/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  建立时间: 3/5/2013 4:58:50 PM
 *  修改记录:
 * 
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// 扩展边缘
    /// </summary>
    public class ExtendContour : List<PointS>
    {
        public Int32 Id { get; private set; }

        public ExtendContour(RichBlob blob):base()
        {
            /**
            * 计算扩展边缘点。
            * 算法参照： Milan Sonka 等, Image Processing, Analysis, and Machine Vision  3ed. p194
            */

            // 计算最左上的点
            PointS current = RichBlob.EmptyPoint;
            int min = int.MaxValue;
            List<PointS> points = blob.ContourPoints;
            foreach (PointS p in points)
            {
                int hash = blob.GetPointHashCode(p);
                if (hash < min)
                {
                    min = hash;
                    current = p;
                }
            }

            PointS start = current;
            this.Add(current);

            int dir = 0; // 0 - Down,1-Left,2-Up,3-Right

            ImageInt32 map = blob.BlobMap;
            Rect region = new Rect(0, 0, map.Width, map.Height);
            PointS next = RichBlob.EmptyPoint;
            int height = map.Height;
            int width = map.Width;
            Id = blob.Id;
            Boolean flag0, flag1;
            while (next != start)
            {
                switch (dir)
                {
                    case 0:
                        next = new PointS(current.X, current.Y + 1);
                        if (next != start) this.Add(next);

                        flag0 = IsRegion(next, map);
                        flag1 = IsRegion(next.Left(), map);

                        if (flag0 == false)
                            dir = 3;
                        else if (flag1 == false)
                            dir = 0;
                        else
                            dir = 1;

                        current = next;
                        break;
                    case 1:
                        next = new PointS(current.X - 1, current.Y);
                        if (next != start) this.Add(next);

                        flag0 = IsRegion(next.Left(), map);
                        flag1 = IsRegion(next.LeftUp(), map);

                        if (flag0 == false)
                            dir = 0;
                        else if (flag1 == false)
                            dir = 1;
                        else
                            dir = 2;

                        current = next;
                        break;
                    case 2:
                        next = new PointS(current.X, current.Y - 1);
                        if (next != start) this.Add(next);

                        flag0 = IsRegion(next.Up(), map);
                        flag1 = IsRegion(next.LeftUp(), map);

                        if (flag1 == false)
                            dir = 1;
                        else if (flag0 == false)
                            dir = 2;
                        else
                            dir = 3;

                        current = next;
                        break;

                    case 3:
                    default:
                        next = new PointS(current.X + 1, current.Y);
                        if (next != start) this.Add(next);

                        flag0 = IsRegion(next,map);
                        flag1 = IsRegion(next.Up(), map);

                        if (flag1 == false)
                            dir = 2;
                        else if (flag0 == false)
                            dir = 3;
                        else
                            dir = 0;

                        current = next;
                        break;
                }
            }
        }

        private Boolean IsRegion(PointS p, ImageInt32 map)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= map.Width || p.Y >= map.Height)
                return false; // 为背景
            else return map[p] == Id;
        }
    }
}
