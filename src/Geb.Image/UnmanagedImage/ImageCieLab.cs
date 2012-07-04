/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public partial struct CieLab
    {
        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public double GetDistance(CieLab other)
        {
            double deltaL = this.L - other.L;
            double deltaA = this.A - other.A;
            double deltaB = this.B - other.B;
            double distance = deltaL * deltaL + deltaA * deltaA + deltaB * deltaB;
            return Math.Sqrt(distance);
        }

        public double GetDistanceSquare(CieLab other)
        {
            double deltaL = this.L - other.L;
            double deltaA = this.A - other.A;
            double deltaB = this.B - other.B;
            return deltaL * deltaL + deltaA * deltaA + deltaB * deltaB;
        }
    }
}
