using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Analysis
{
    /// <summary>
    /// 多边形近似算法。
    /// from: https://github.com/BobLd/RamerDouglasPeuckerNet
    /// </summary>
    public class PolygonalApproximation
    {
        ///// <summary>
        ///// Ramer-Douglas-Peucker 多边形近似算法。该算法的思想是设置一个距离阈值，当点到线的距离小于该阈值，
        ///// 那么点视为可以删除的点。最后剩下的点即为最终结果。
        ///// 给定数据：
        /////   - 按线数据排序好的坐标数组；
        /////   - 距离阈值 epsilon
        ///// 具体步骤如下：
        /////   - 将首尾点标记为保留点，首尾点连线记为线 L1；
        /////   - 在首尾点之间的剩余点中找到距离 L1 最远的点 P，如果点 P 到直线的距离 d 大于阈值 epsilon，那么
        /////     将点 P 标记为保留点，否则点 P 为待剔除点；
        /////   - 将首点和点 P 连接构成线 L2；将尾点和点 P 连接构成线 L3，分别重复步骤 2；
        /////   - 一直到点小于三个点时停止。
        ///// </summary>
        public static PointF[] ApproxPolyDP(PointF[] points, double tolerance)
        {
            if (points == null || points.Length < 3) return points;
            if (double.IsInfinity(tolerance) || double.IsNaN(tolerance)) return points;
            tolerance *= tolerance;
            if (tolerance <= float.Epsilon) return points;

            int firstIndex = 0;
            int lastIndex = points.Length - 1;
            List<int> indexesToKeep = new List<int>();

            // Add the first and last index to the keepers
            indexesToKeep.Add(firstIndex);
            indexesToKeep.Add(lastIndex);

            // The first and the last point cannot be the same
            while (points[firstIndex].Equals(points[lastIndex]))
            {
                lastIndex--;
            }

            Reduce(points, firstIndex, lastIndex, tolerance, ref indexesToKeep);

            int l = indexesToKeep.Count;
            PointF[] returnPoints = new PointF[l];
            indexesToKeep.Sort();

            unsafe
            {
                fixed (PointF* ptr = points, result = returnPoints)
                {
                    for (int i = 0; i < l; ++i)
                        *(result + i) = *(ptr + indexesToKeep[i]);
                }
            }

            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstIndex">The first point's index.</param>
        /// <param name="lastIndex">The last point's index.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="indexesToKeep">The points' index to keep.</param>
        private static void Reduce(PointF[] points, int firstIndex, int lastIndex, double tolerance,
            ref List<int> indexesToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            unsafe
            {
                fixed (PointF* samples = points)
                {
                    PointF point1 = *(samples + firstIndex);
                    PointF point2 = *(samples + lastIndex);
                    double distXY = point1.X * point2.Y - point2.X * point1.Y;
                    double distX = point2.X - point1.X;
                    double distY = point1.Y - point2.Y;
                    double bottom = distX * distX + distY * distY;

                    for (int i = firstIndex; i < lastIndex; i++)
                    {
                        // Perpendicular Distance
                        PointF point = *(samples + i);
                        double area = distXY + distX * point.Y + distY * point.X;
                        double distance = (area / bottom) * area;

                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            indexFarthest = i;
                        }
                    }
                }
            }

            if (maxDistance > tolerance) // && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                indexesToKeep.Add(indexFarthest);
                Reduce(points, firstIndex, indexFarthest, tolerance, ref indexesToKeep);
                Reduce(points, indexFarthest, lastIndex, tolerance, ref indexesToKeep);
            }
        }
    }
}
