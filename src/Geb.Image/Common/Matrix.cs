using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 矩阵：
    ///     a b tx
    ///     c d ty
    ///     0 0 1
    /// </summary>
    public struct Matrix
    {
        public double A, B, C, D, TX, TY;

        public Matrix(double a = 1, double b = 0, double c = 0, double d = 1, double tx = 0, double ty = 0)
        {
            A = a; B = b; C = c; D = d; TX = tx; TY = ty;
        }

        public static PointD operator *(Matrix m, PointD p)
        {
            return new PointD(m.A * p.X + m.B * p.Y + m.TX, m.C * p.X + m.D * p.Y + m.TY);
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix m3 = new Matrix();
            m3.A = m2.A * m1.A + m2.C * m1.B;
            m3.B = m2.B * m1.A + m2.D * m1.B;
            m3.C = m2.A * m1.C + m2.C * m1.D;
            m3.D = m2.B * m1.C + m2.D * m1.D;
            m3.TX = m1.A * m2.TX + m1.B * m2.TY + m1.TX;
            m3.TY = m1.C * m2.TX + m1.D * m2.TY + m1.TY;
            return m3;
        }

        /// <summary>
        /// 计算旋转矩阵
        /// </summary>
        /// <param name="root">旋转点</param>
        /// <param name="radians">旋转弧度。按照逆时针计算。</param>
        /// <returns></returns>
        public static Matrix CreateRotationMatrix(PointD root, double radians)
        {
            return CreatePanningMatrix(root.X, root.Y) * CreateRotationMatrix(radians) * CreatePanningMatrix(-root.X, -root.Y);
        }

        /// <summary>
        /// 生成平移矩阵
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static Matrix CreatePanningMatrix(double tx, double ty)
        {
            return new Matrix(1, 0, 0, 1, tx, ty);
        }

        /// <summary>
        /// 生成旋转矩阵
        /// </summary>
        /// <param name="radians">旋转弧度。按照逆时针计算</param>
        /// <returns></returns>
        public static Matrix CreateRotationMatrix(double radians)
        {
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            return new Matrix(cos, -sin, sin, cos);
        }
    }
}
