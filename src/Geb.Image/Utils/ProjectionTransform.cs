using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Utils
{
    /// <summary>
    /// 射影变换
    /// </summary>
    public class ProjectionTransform
    {
        public double X00, X01, X10, X11;
        public double Y00, Y01, Y10, Y11;
        public double XX00, XX01, XX10, XX11;
        public double YY00, YY01, YY10, YY11;

        public ProjectionTransform(
            double x00, double y00, double x01, double y01, double x10, double y10, double x11, double y11,
            double xx00, double yy00, double xx01, double yy01, double xx10, double yy10, double xx11, double yy11)
        {
            this.X00 = x00;
            this.X01 = x01;
            this.X10 = x10;
            this.X11 = x11;
            this.Y00 = y00;
            this.Y01 = y01;
            this.Y10 = y10;
            this.Y11 = y11;
            this.XX00 = xx00;
            this.XX01 = xx01;
            this.XX10 = xx10;
            this.XX11 = xx11;
            this.YY00 = yy00;
            this.YY01 = yy01;
            this.YY10 = yy10;
            this.YY11 = yy11;
        }
    }
}
