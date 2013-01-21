/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 仿射变换类
    /// </summary>
    public class AffineTransform
    {
        public double a;
        public double b;
        public double c;
        public double d;
        public double tx;
        public double ty;

        public AffineTransform(TriangleF t1, TriangleF t2):
            this(t1.P0.X, t1.P0.Y, t1.P1.X, t1.P1.Y, t1.P2.X, t1.P2.Y,
                t2.P0.X, t2.P0.Y, t2.P1.X, t2.P1.Y, t2.P2.X, t2.P2.Y)
        {
        }

        /// <summary>
        /// 创建从点(x1,y1),(x2,y2),(x3,y3)变换为(xx1,yy1),(xx2,yy2),(xx3,yy3)仿射变换矩阵参数。
        /// </summary>
        public AffineTransform(double x1, double y1, double x2, double y2, double x3, double y3, double xx1, double yy1, double xx2, double yy2, double xx3, double yy3)
        {
            double x31 = x3 - x1;
            double x21 = x2 - x1;
            double x32 = x3 - x2;
            double y31 = y3 - y1;
            double y21 = y2 - y1;
            double y32 = y3 - y3;
            double xx31 = xx3 - xx1;
            double xx21 = xx2 - xx1;
            double xx32 = xx3 - xx2;
            double yy31 = yy3 - yy1;
            double yy21 = yy2 - yy1;
            double yy32 = yy3 - yy3;
            a = (y31 * xx21 - y21 * xx31) / (y31 * x21 - y21 * x31);
            b = (y31 * yy21 - y21 * yy31) / (y31 * x21 - y21 * x31);
            c = (x31 * xx21 - x21 * xx31) / (y21 * x31 - y31 * x21);
            d = (x31 * yy21 - x21 * yy31) / (y21 * x31 - y31 * x21);
            double val = (x31 * (y2 * x1 - y1 * x2) - x21 * (y3 * x1 - y1 * x3)); ;
            double tCoeff = val == 0 ? int.MaxValue : 1/val;
            tx = ((xx2 * x1 - xx1 * x2) * (y3 * x1 - y1 * x3) - (y2 * x1 - y1 * x2) * (xx3 * x1 - xx1 * x3)) * tCoeff;
            ty = ((yy2 * x1 - yy1 * x2) * (y3 * x1 - y1 * x3) - (y2 * x1 - y1 * x2) * (yy3 * x1 - yy1 * x3)) * tCoeff;
        }

        public void Transform(double x, double y, ref double xx, ref double yy)
        {
            xx = a * x + c * y + tx;
            yy = b * x + d * y + ty;
        }

        public void Transform(PointF p1, ref PointF p2)
        {
            p2.X = (float)(a * p1.X + c * p1.Y + tx);
            p2.Y = (float)(b * p1.X + d * p1.Y + ty);
        }
    }
}
