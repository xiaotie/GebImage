/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public partial struct CieXyz
    {
        public static readonly CieXyz D65 = new CieXyz { X = 0.9505f, Y = 1.0f, Z = 1.0890f };

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public CieLab ToCieLab()
        {
            float l = 116.0f * Transform(Y / D65.Y) - 16;
            float a = 500.0f * (Transform(X / D65.X) - Transform(Y / D65.Y));
            float b = 200.0f * (Transform(Y / D65.Y) - Transform(Z / D65.Z));
            return new CieLab { L = l, A = a, B = b };
        }

        private static float Transform(float t)
        {
            return ((t > 0.008856) ? (float) Math.Pow(t, (1.0f / 3.0f)) : (7.787f * t + 16.0f / 116.0f));
        }
    }
}
