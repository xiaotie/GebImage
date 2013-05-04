/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public enum InterpolationMode
    {
        NearestNeighbor = 1,
        Bilinear = 2
    }

    /// <summary>
    /// 插值的工具类
    /// </summary>
    public sealed class Interpolate
    {
        public static Byte LinearInterpolate(Byte v00, Byte v10, Byte v01,  Byte v11, double a, double b)
        {
            int val = (int)Math.Round((1 - a) * (1 - b) * v00 + a * (1 - b) * v10 + b * (1 - a) * v01 + a * b * v11);
            if (val < 0) return 0;
            else if (val > 255) return 255;
            else return (Byte)val;
        }
    }
}
