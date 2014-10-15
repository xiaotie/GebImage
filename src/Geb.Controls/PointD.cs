using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Geb.Controls
{
    public struct PointD
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x; Y = y;
        }

        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }

        public Point ToPoint()
        {
            return new Point((int)Math.Round(X), (int)Math.Round(Y));
        }

        public static PointD operator  +(PointD left, PointD right)
        {
            return new PointD { X = left.X + right.X, Y = left.Y + right.Y };
        }

        public static PointD operator -(PointD left, PointD right)
        {
            return new PointD { X = left.X - right.X, Y = left.Y - right.Y };
        }
    }
}
