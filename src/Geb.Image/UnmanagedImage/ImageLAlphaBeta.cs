/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public partial struct LAlaphaBeta
    {
        public Double L;
        public Double Alpha;
        public Double Beta;

        public override string ToString()
        {
            return "Lab [L=" + L + ", Alpha=" + Alpha + ", Beta=" + Beta + "]";
        }

        public Rgb24 ToRgb24()
        {
            const double c00 = 0.57735;
            const double c01 = 0.408248;
            const double c02 = 0.707107;
            const double c10 = 0.57735;
            const double c11 = 0.408248;
            const double c12 = -0.707107;
            const double c20 = 0.57735;
            const double c21 = -0.8164965;
            const double c22 = 0;

            double l = c00 * L + c01 * Alpha + c02 * Beta;
            double m = c10 * L + c11 * Alpha + c12 * Beta;
            double s = c20 * L + c21 * Alpha + c22 * Beta;

            l = Math.Pow(10, l);
            m = Math.Pow(10, m);
            s = Math.Pow(10, s);

            int r = (int)(4.4679 * l -3.5873 * m + 0.1193 * s);
            int g = (int)(-1.2186 * l + 2.3809 * m - 0.1624 * s);
            int b = (int)(0.0497 * l - 0.2439 * m + 1.2045 * s);
            if (r < 0) r = 0; else if (r > 255) r = 255;
            if (g < 0) g = 0; else if (g > 255) g = 255;
            if (b < 0) b = 0; else if (b > 255) b = 255;
            Rgb24 rgb = new Rgb24();
            rgb.Red = (Byte)r;
            rgb.Green = (Byte)g;
            rgb.Blue = (Byte)b;

            return rgb;
        }

        //public static Lab Create(Rgb24 rgb)
        //{
        //    Lab lab = new Lab();
            
        //    double l = 0.3811 * rgb.Red + 0.5783 * rgb.Green + 0.0402 * rgb.Blue;
        //    double m = 0.1967 * rgb.Red + 0.7244 * rgb.Green + 0.0782 * rgb.Blue;
        //    double s = 0.0241 * rgb.Red + 0.1288 * rgb.Green + 0.8444 * rgb.Blue;
        //    l = Math.Log10(l);
        //    m = Math.Log10(m);
        //    s = Math.Log10(s);

        //    const double c00 = 0.57735;
        //    const double c01 = 0.57735;
        //    const double c02 = 0.57735;
        //    const double c10 = 0.408248;
        //    const double c11 = 0.408248;
        //    const double c12 = -0.8164965;
        //    const double c20 = 0.707107;
        //    const double c21 = -0.707107;
        //    const double c22 = 0;
        //    lab.L = 
        //    return lab;
        //}
    }
}
