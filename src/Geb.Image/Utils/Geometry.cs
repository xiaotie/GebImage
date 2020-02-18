using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 集合算法
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// 计算任意多边形的面积。
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static float ComputePolygonArea(IList<PointF> points)
        {
            if (points.Count < 3) return 0;
            float s = points[0].Y * (points[points.Count - 1].X - points[1].X);
            for (int i = 1; i < points.Count; i++)
            {
                s += points[i].Y * (points[i - 1].X - points[(i + 1) % points.Count].X);
            }
            return Math.Abs(s * 0.5f);
        }

        /// <summary>
        /// 计算任意多边形的面积。
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static float ComputePolygonArea(PointF[] points)
        {
            return ComputePolygonArea(new Span<PointF>(points));
        }

        /// <summary>
        /// 计算任意多边形的面积。
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static float ComputePolygonArea(Span<PointF> points)
        {
            if (points.Length < 3) return 0;
            float s = points[0].Y * (points[points.Length - 1].X - points[1].X);
            for (int i = 1; i < points.Length; i++)
            {
                s += points[i].Y * (points[i - 1].X - points[(i + 1) % points.Length].X);
            }
            return Math.Abs(s * 0.5f);
        }

        /// <summary>
        /// 计算任意多边形的面积。
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static float ComputePolygonArea(IList<PointS> points)
        {
            if (points.Count < 3) return 0;
            float s = points[0].Y * (points[points.Count - 1].X - points[1].X);
            for (int i = 1; i < points.Count; i++)
            {
                s += points[i].Y * (points[i - 1].X - points[(i + 1) % points.Count].X);
            }
            return Math.Abs(s * 0.5f);
        }

        /// <summary>
        /// 计算任意多边形的面积。
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static float ComputePolygonArea(Span<PointS> points)
        {
            if (points.Length < 3) return 0;
            float s = points[0].Y * (points[points.Length - 1].X - points[1].X);
            for (int i = 1; i < points.Length; i++)
            {
                s += points[i].Y * (points[i - 1].X - points[(i + 1) % points.Length].X);
            }
            return Math.Abs(s * 0.5f);
        }

        /// <summary>
        /// 计算任意多边形的面积。
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static float ComputePolygonArea(PointS[] points)
        {
            return ComputePolygonArea(new Span<PointS>(points));
        }
    }
}
