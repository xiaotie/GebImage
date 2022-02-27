using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// 凸包。
    /// </summary>
    public class ConvexHull
    {
        public class GrahamScan
        {
            const int TURN_LEFT = 1;
            const int TURN_RIGHT = -1;
            const int TURN_NONE = 0;
            public static int Turn(PointF p, PointF q, PointF r)
            {
                return ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y)).CompareTo(0);
            }

            public static void KeepLeft(List<PointF> hull, PointF r)
            {
                while (hull.Count > 1 && Turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TURN_LEFT)
                {
                    hull.RemoveAt(hull.Count - 1);
                }
                if (hull.Count == 0 || hull[hull.Count - 1] != r)
                {
                    hull.Add(r);
                }
            }

            public static double GetAngle(PointF p1, PointF p2)
            {
                float xDiff = p2.X - p1.X;
                float yDiff = p2.Y - p1.Y;
                return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
            }

            public static List<PointF> MergeSort(PointF p0, List<PointF> arrPoint)
            {
                if (arrPoint.Count == 1)
                {
                    return arrPoint;
                }
                List<PointF> arrSortedInt = new List<PointF>();
                int middle = (int)arrPoint.Count / 2;
                List<PointF> leftArray = arrPoint.GetRange(0, middle);
                List<PointF> rightArray = arrPoint.GetRange(middle, arrPoint.Count - middle);
                leftArray = MergeSort(p0, leftArray);
                rightArray = MergeSort(p0, rightArray);
                int leftptr = 0;
                int rightptr = 0;
                for (int i = 0; i < leftArray.Count + rightArray.Count; i++)
                {
                    if (leftptr == leftArray.Count)
                    {
                        arrSortedInt.Add(rightArray[rightptr]);
                        rightptr++;
                    }
                    else if (rightptr == rightArray.Count)
                    {
                        arrSortedInt.Add(leftArray[leftptr]);
                        leftptr++;
                    }
                    else if (GetAngle(p0, leftArray[leftptr]) < GetAngle(p0, rightArray[rightptr]))
                    {
                        arrSortedInt.Add(leftArray[leftptr]);
                        leftptr++;
                    }
                    else
                    {
                        arrSortedInt.Add(rightArray[rightptr]);
                        rightptr++;
                    }
                }
                return arrSortedInt;
            }

            public static List<PointF> ConvexHull(IList<PointF> points)
            {
                PointF p0 = points[0];
                foreach (PointF value in points)
                {
                    if (p0.Y > value.Y)
                        p0 = value;
                }
                List<PointF> order = new List<PointF>();
                foreach (PointF value in points)
                {
                    if (p0 != value)
                        order.Add(value);
                }

                order = MergeSort(p0, order);
                List<PointF> result = new List<PointF>();
                result.Add(p0);
                result.Add(order[0]);
                result.Add(order[1]);
                order.RemoveAt(0);
                order.RemoveAt(0);
                foreach (PointF value in order)
                {
                    KeepLeft(result, value);
                }
                return result;
            }
        }

        public static List<PointF> CreateConvexHull(IList<PointF> pts)
        {
            return GrahamScan.ConvexHull(pts);
        }

        #region 旋转卡壳算法

        /// <summary>
        /// 旋转卡壳的模式
        /// </summary>
        public enum RotatingCalipersMode
        {
            /// <summary>
            /// 最小面积矩阵
            /// </summary>
            MinAreaRect,

            /// <summary>
            /// 最大高度
            /// </summary>
            MaxHeight
        }

        /// <summary>
        /// 旋转卡壳算法。翻译自 opencv。注意，输入的凸包顶点需要时按顺序排列的，如果是乱序，返回的是错误的结果。
        /// </summary>
        /// <param name="points">输入凸包的顶点</param>
        /// <param name="mode">模式，MinAreaRect 或 MaxHeight</param>
        /// <param name="output">输出。输出数据与 opencv 的 rotatingCalipers 一致</param>
        /// <exception cref="Exception"></exception>
        public static unsafe void RotatingCalipers(Span<PointF> points, RotatingCalipersMode mode, Span<float> output)
        {
            int n = points.Length;
            float minarea = float.MaxValue;
            float max_dist = 0;
            byte* pBuf = stackalloc byte[32];
            Span<float> buf = new Span<float>(pBuf, 8);
            Span<int> buf_u = new Span<int>(pBuf, 8);

            int i, k;
            ImageFloat abuf = new ImageFloat(n * 3, 1);
            float* inv_vect_length = abuf.Start;
            PointF* vect = (PointF*)(inv_vect_length + n);
            int left = 0, bottom = 0, right = 0, top = 0;
            Span<int> seq = stackalloc int[4] { -1, -1, -1, -1 };

            /* rotating calipers sides will always have coordinates
             (a,b) (-b,a) (-a,-b) (b, -a)
             */
            /* this is a first base bector (a,b) initialized by (1,0) */
            float orientation = 0;
            float base_a;
            float base_b = 0;

            float left_x, right_x, top_y, bottom_y;

            float* dp = stackalloc float[4];

            PointF pt0 = points[0];

            left_x = right_x = pt0.X;
            top_y = bottom_y = pt0.Y;

            for (i = 0; i < n; i++)
            {
                double dx, dy;

                if (pt0.X < left_x)
                {
                    left_x = pt0.X; left = i;
                }

                if (pt0.X > right_x)
                {
                    right_x = pt0.X; right = i;
                }

                if (pt0.Y > top_y)
                {
                    top_y = pt0.Y; top = i;
                }

                if (pt0.Y < bottom_y)
                {
                    bottom_y = pt0.Y; bottom = i;
                }

                PointF pt = points[(i + 1) & (i + 1 < n ? -1 : 0)];

                dx = pt.X - pt0.X;
                dy = pt.Y - pt0.Y;

                vect[i].X = (float)dx;
                vect[i].Y = (float)dy;
                inv_vect_length[i] = (float)(1.0 / Math.Sqrt(dx * dx + dy * dy));

                pt0 = pt;
            }

            // find convex hull orientation
            {
                double ax = vect[n - 1].X;
                double ay = vect[n - 1].Y;

                for (i = 0; i < n; i++)
                {
                    double bx = vect[i].X;
                    double by = vect[i].Y;

                    double convexity = ax * by - ay * bx;

                    if (convexity != 0)
                    {
                        orientation = (convexity > 0) ? 1.0f : (-1.0f);
                        break;
                    }
                    ax = bx;
                    ay = by;
                }
            }
            base_a = orientation;

            /*****************************************************************************************/
            /*                         init calipers position                                        */
            seq[0] = bottom;
            seq[1] = right;
            seq[2] = top;
            seq[3] = left;
            /*****************************************************************************************/
            /*                         Main loop - evaluate angles and rotate calipers               */

            /* all of edges will be checked while rotating calipers by 90 degrees */
            for (k = 0; k < n; k++)
            {
                /* sinus of minimal angle */
                /*float sinus;*/

                /* compute cosine of angle between calipers side and polygon edge */
                /* dp - dot product */
                dp[0] = +base_a * vect[seq[0]].X + base_b * vect[seq[0]].Y;
                dp[1] = -base_b * vect[seq[1]].X + base_a * vect[seq[1]].Y;
                dp[2] = -base_a * vect[seq[2]].X - base_b * vect[seq[2]].Y;
                dp[3] = +base_b * vect[seq[3]].X - base_a * vect[seq[3]].Y;

                float maxcos = dp[0] * inv_vect_length[seq[0]];

                /* number of calipers edges, that has minimal angle with edge */
                int main_element = 0;

                /* choose minimal angle */
                for (i = 1; i < 4; ++i)
                {
                    float cosalpha = dp[i] * inv_vect_length[seq[i]];
                    if (cosalpha > maxcos)
                    {
                        main_element = i;
                        maxcos = cosalpha;
                    }
                }

                /*rotate calipers*/
                {
                    //get next base
                    int pindex = seq[main_element];
                    float lead_x = vect[pindex].X * inv_vect_length[pindex];
                    float lead_y = vect[pindex].Y * inv_vect_length[pindex];
                    switch (main_element)
                    {
                        case 0:
                            base_a = lead_x;
                            base_b = lead_y;
                            break;
                        case 1:
                            base_a = lead_y;
                            base_b = -lead_x;
                            break;
                        case 2:
                            base_a = -lead_x;
                            base_b = -lead_y;
                            break;
                        case 3:
                            base_a = -lead_y;
                            base_b = lead_x;
                            break;
                        default:
                            throw new Exception("main_element should be 0, 1, 2 or 3");
                    }
                }
                /* change base point of main edge */
                seq[main_element] += 1;
                seq[main_element] = (seq[main_element] == n) ? 0 : seq[main_element];

                switch (mode)
                {
                    case RotatingCalipersMode.MaxHeight:
                        {
                            /* now main element lies on edge aligned to calipers side */

                            /* find opposite element i.e. transform  */
                            /* 0->2, 1->3, 2->0, 3->1                */
                            int opposite_el = main_element ^ 2;

                            float dx = points[seq[opposite_el]].X - points[seq[main_element]].X;
                            float dy = points[seq[opposite_el]].Y - points[seq[main_element]].Y;
                            float dist;

                            if ((main_element & 1) != 0)
                                dist = (float)Math.Abs(dx * base_a + dy * base_b);
                            else
                                dist = (float)Math.Abs(dx * (-base_b) + dy * base_a);

                            if (dist > max_dist)
                                max_dist = dist;
                        }
                        break;
                    case RotatingCalipersMode.MinAreaRect:
                        /* find area of rectangle */
                        {
                            float height;
                            float area;

                            /* find vector left-right */
                            float dx = points[seq[1]].X - points[seq[3]].X;
                            float dy = points[seq[1]].Y - points[seq[3]].Y;

                            /* dotproduct */
                            float width = dx * base_a + dy * base_b;

                            /* find vector left-right */
                            dx = points[seq[2]].X - points[seq[0]].X;
                            dy = points[seq[2]].Y - points[seq[0]].Y;

                            /* dotproduct */
                            height = -dx * base_b + dy * base_a;

                            area = width * height;
                            if (area <= minarea)
                            {
                                minarea = area;
                                /* leftist point */
                                buf_u[0] = seq[3];
                                buf[1] = base_a;
                                buf[2] = width;
                                buf[3] = base_b;
                                buf[4] = height;
                                /* bottom point */
                                buf_u[5] = seq[0];
                                buf[6] = area;
                            }
                        }
                        break;
                }                       /*switch */
            }                           /* for */

            switch (mode)
            {
                case RotatingCalipersMode.MinAreaRect:
                    {
                        float A1 = buf[1];
                        float B1 = buf[3];

                        float A2 = -buf[3];
                        float B2 = buf[1];

                        float C1 = A1 * points[buf_u[0]].X + points[buf_u[0]].Y * B1;
                        float C2 = A2 * points[buf_u[5]].X + points[buf_u[5]].Y * B2;

                        float idet = 1.0f / (A1 * B2 - A2 * B1);

                        float px = (C1 * B2 - C2 * B1) * idet;
                        float py = (A1 * C2 - A2 * C1) * idet;

                        output[0] = px;
                        output[1] = py;

                        output[2] = A1 * buf[2];
                        output[3] = B1 * buf[2];

                        output[4] = A2 * buf[4];
                        output[5] = B2 * buf[4];
                    }
                    break;
                case RotatingCalipersMode.MaxHeight:
                    {
                        output[0] = max_dist;
                    }
                    break;
            }

        }

        /// <summary>
        /// 输入多边形（可以不是凸多边形）顶点，返回包含所有顶点的面积最小的矩阵。翻译自 opencv 的 cv::minAreaRect 方法
        /// </summary>
        /// <param name="points">多边形顶点。多边形可以非凸多边形</param>
        /// <param name="isClockwiseConvexHullVertices">传入的点是否是凸包顺时针排列的顶点</param>
        /// <returns>返回包含所有顶点的面积最小的矩阵</returns>
        public static unsafe RotatedRectF MinAreaRect(PointF[] points, bool isClockwiseConvexHullVertices = false)
        {
            RotatedRectF box = new RotatedRectF();
            PointF* output = stackalloc PointF[3];
            PointF[] hpoints = isClockwiseConvexHullVertices ? points : CreateConvexHull(points).ToArray();
            if(hpoints.Length > 2)
            {
                RotatingCalipers(new Span<PointF>(hpoints), RotatingCalipersMode.MinAreaRect, new Span<float>(output, 6));
                box.Center.X = output[0].X + (output[1].X + output[2].X)*0.5f;
                box.Center.Y = output[0].Y + (output[1].Y + output[2].Y)*0.5f;
                box.Size.Width = (float)Math.Sqrt((double)output[1].X * output[1].X + (double)output[1].Y * output[1].Y);
                box.Size.Height = (float)Math.Sqrt((double)output[2].X * output[2].X + (double)output[2].Y * output[2].Y);
                box.Angle = (float)Math.Atan2((double)output[1].Y, (double)output[1].X);
            }
            else if(hpoints.Length == 2)
            {
                box.Center.X = (hpoints[0].X + hpoints[1].X) * 0.5f;
                box.Center.Y = (hpoints[0].Y + hpoints[1].Y) * 0.5f;
                double dx = hpoints[1].X - hpoints[0].X;
                double dy = hpoints[1].Y - hpoints[0].Y;
                box.Size.Width = (float)Math.Sqrt(dx * dx + dy * dy);
                box.Size.Height = 0;
                box.Angle = (float)Math.Atan2(dy, dx);
            }
            else if(hpoints.Length == 1)
            {
                box.Center = hpoints[0];
            }
            box.Angle = (float)(box.Angle * 180 / Math.PI);
            return box;
        }

        #endregion
    }
}
