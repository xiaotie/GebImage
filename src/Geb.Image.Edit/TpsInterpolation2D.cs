/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Edit
{
    using Geb.Image;

    public class TpsException : Exception
    {
        public TpsException(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// 2D薄板样条插值
    /// </summary>
    public class TpsInterpolation2D
    {
        private static readonly int MinNumOfSamples = 4;

        public List<PointF> SrcPoints { get; private set; }
        public List<PointF> DstPoints { get; private set; }

        private int m_offset;
        private Geb.Image.Matrix m_mapMatrix = null;

        public TpsInterpolation2D(ICollection<PointF> srcPoints, ICollection<PointF> dstPoints)
        {
            if (srcPoints == null)
            {
                throw new ArgumentNullException("srcPoints");
            }
            else if (dstPoints == null)
            {
                throw new ArgumentNullException("dstPoints");
            }
            else if (srcPoints.Count != dstPoints.Count)
            {
                throw new TpsException("srcPoints 与 dstPoints 的 Count 不相等，无法进行薄板样条插值.");
            }
            else if (srcPoints.Count < MinNumOfSamples)
            {
                throw new TpsException("薄板样条插值至少需要 " + MinNumOfSamples + " 个点.");
            }

            m_offset = srcPoints.Count;

            SrcPoints = new List<PointF>(srcPoints.Count);
            DstPoints = new List<PointF>(dstPoints.Count);
            SrcPoints.AddRange(srcPoints);
            DstPoints.AddRange(dstPoints);
        }

        public PointF Interpolate(PointF point)
        {
            if (m_mapMatrix == null)
            {
                lock (this)
                {
                    if (m_mapMatrix == null)
                        this.Prepair();
                }
            }

            int count = this.m_offset;
            double x = m_mapMatrix[count, 0] + m_mapMatrix[count + 1, 0] * point.X + m_mapMatrix[count + 2, 0] * point.Y;
            double y = m_mapMatrix[count, 1] + m_mapMatrix[count + 1, 1] * point.X + m_mapMatrix[count + 2, 1] * point.Y;
            for (int i = 0; i < count; i++)
            {
                double r = this.RadialBasis(point, this.SrcPoints[i]);
                x += r * m_mapMatrix[i, 0];
                y += r * m_mapMatrix[i, 1];
            }

            return new PointF(x, y);
        }

        private void Prepair()
        {
            Geb.Image.Matrix l = this.GetLandmarksMapMatrix();
            l = l.Inverse();
            Geb.Image.Matrix y = GetTargetsMapMatrix();
            m_mapMatrix = l * y; // m_mapMatrix 是  (W|a1,a2,a3) 的转置
        }

        /// <summary>
        /// 将 n*2 的矩阵 V 增加 3*2 的 0 元素 ，得到矩阵Y
        /// </summary>
        /// <returns>矩阵Y。</returns>
        private Geb.Image.Matrix GetTargetsMapMatrix()
        {
            Geb.Image.Matrix ret = new Geb.Image.Matrix(this.DstPoints.Count + 3, 2);

            for (int i = 0; i < ret.RowCount; i++)
            {
                if (i >= this.DstPoints.Count)
                {
                    ret[i, 0] = 0;
                    ret[i, 1] = 0;
                }
                else
                {
                    PointF p = this.DstPoints[i];
                    ret[i, 0] = p.X;
                    ret[i, 1] = p.Y;
                }
            }
            return ret;
        }

        /// <summary>
        /// 计算 p0 和 p1 两点间距的 径向基函数值。采用下面的径向基函数：
        ///		U(r)=r^2log(r^2)
        /// </summary>
        /// <param name="p0">点p0</param>
        /// <param name="p1">点p1</param>
        /// <returns>径向基函数计算结果</returns>
        private double RadialBasis(PointF p0, PointF p1)
        {
            double deltaX = p0.X - p1.X;
            double deltaY = p0.Y - p1.Y;

            double result = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); // r^2

            if (result != 0)
            {
                result = result * Math.Log(result);
            }

            return result;
        }

        /// <summary>
        /// 产生矩阵 L
        /// </summary>
        /// <returns>矩阵L(见 Bookstein(1989))，矩阵大小为(n+3)*(n+3)。</returns>
        private Geb.Image.Matrix GetLandmarksMapMatrix()
        {
            IList<PointF> src = this.SrcPoints;
            Geb.Image.Matrix ret = new Geb.Image.Matrix(src.Count + 3, src.Count + 3);

            for (int i = 0; i < src.Count; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    ret[i, j] = ret[j, i] = this.RadialBasis(src[i], src[j]);
                }
            }

            for (int row = 0; row < src.Count; ++row)
            {
                ret[row, src.Count] = ret[src.Count, row] = 1;
                ret[row, src.Count + 0 + 1] = ret[src.Count + 0 + 1, row] = src[row].X;
                ret[row, src.Count + 1 + 1] = ret[src.Count + 1 + 1, row] = src[row].Y;
            }

            for (int row = src.Count; row < src.Count + 3; ++row)
            {
                for (int col = src.Count; col < src.Count + 3; ++col)
                {
                    ret[row, col] = 0;
                }
            }

            return ret;
        }
    }
}
