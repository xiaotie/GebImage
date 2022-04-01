using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 仿射变换矩阵：
    ///     a b tx
    ///     c d ty
    ///     0 0 1
    /// </summary>
    public struct AffineTransMatrix
    {
        public double A, B, C, D, TX, TY;

        public AffineTransMatrix(double a = 1, double b = 0, double c = 0, double d = 1, double tx = 0, double ty = 0)
        {
            A = a; B = b; C = c; D = d; TX = tx; TY = ty;
        }

        /// <summary>
        /// 转换成普通矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            return new Matrix(new double[] {  A,B,TX,C,D,TY,0,0,1  }, 3, 3);
        }

        /// <summary>
        /// 求逆矩阵
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MatrixException"></exception>
        public AffineTransMatrix Inverse()
        {
            Matrix m = this.ToMatrix();
            const double eps = 0.000000001;
            m = m.Inverse();
            double scale = m[8];
            if(scale < eps) throw new MatrixException("the matrix cant invert");
            scale = 1 / scale;

            AffineTransMatrix atm = new AffineTransMatrix();
            atm.A = m[0] * scale;
            atm.B = m[1] * scale;
            atm.TX = m[2] * scale;
            atm.C = m[3] * scale;
            atm.D = m[4] * scale;
            atm.TY = m[5] * scale;
            return atm;
        }

        public static PointD operator *(AffineTransMatrix m, PointD p)
        {
            return new PointD(m.A * p.X + m.B * p.Y + m.TX, m.C * p.X + m.D * p.Y + m.TY);
        }

        public static PointF operator *(AffineTransMatrix m, PointF p)
        {
            return new PointF(m.A * p.X + m.B * p.Y + m.TX, m.C * p.X + m.D * p.Y + m.TY);
        }

        public static PointF operator *(AffineTransMatrix m, Point p)
        {
            return new PointF(m.A * p.X + m.B * p.Y + m.TX, m.C * p.X + m.D * p.Y + m.TY);
        }

        public static AffineTransMatrix operator *(AffineTransMatrix m1, AffineTransMatrix m2)
        {
            AffineTransMatrix m3 = new AffineTransMatrix();
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
        /// <param name="radians">旋转弧度。对于 X 轴向右，Y轴向上的坐标系，按照逆时针计算。对于 X 轴向右，Y轴向下的坐标系，按照顺时针计算。</param>
        /// <returns></returns>
        public static AffineTransMatrix CreateRotationMatrix(PointD root, double radians)
        {
            return CreatePanningMatrix(root.X, root.Y) * CreateRotationMatrix(radians) * CreatePanningMatrix(-root.X, -root.Y);
        }

        public static AffineTransMatrix CreateRotationMatrix(PointF root, double radians)
        {
            return CreatePanningMatrix(root.X, root.Y) * CreateRotationMatrix(radians) * CreatePanningMatrix(-root.X, -root.Y);
        }

        /// <summary>
        /// 生成平移矩阵
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static AffineTransMatrix CreatePanningMatrix(double tx, double ty)
        {
            return new AffineTransMatrix(1, 0, 0, 1, tx, ty);
        }

        /// <summary>
        /// 生成旋转矩阵。
        /// </summary>
        /// <param name="radians">旋转弧度。对于 X 轴向右，Y轴向上的坐标系，按照逆时针计算。对于 X 轴向右，Y轴向下的坐标系，按照顺时针计算。</param>
        /// <returns></returns>
        public static AffineTransMatrix CreateRotationMatrix(double radians)
        {
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            return new AffineTransMatrix(cos, -sin, sin, cos);
        }

        public static AffineTransMatrix CreateResizeMatrix(double scaleX, double scaleY)
        {
            return new AffineTransMatrix(scaleX, 0, 0, scaleY);
        }
    }
}
