/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 点。横纵坐标都是UInt16格式。
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PointS
    {
        [FieldOffset(0)]
        public UInt16 X;
        [FieldOffset(2)]
        public UInt16 Y;
    }

    /// <summary>
    /// Region of Interest (ROI)
    /// </summary>
    public struct ROI
    {
        public UInt16 X { get; private set; }
        public UInt16 Y { get; private set; }
        public UInt16 Width { get; private set; }
        public UInt16 Height { get; private set; }
    }

    /// <summary>
    /// 极坐标点
    /// </summary>
    public struct PolarPointD
    {
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius;

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle;

        public PolarPointD(double radius, double angle)
        {
            Radius = radius;
            Angle = angle;
        }
    }

    /// <summary>
    /// 极坐标点
    /// </summary>
    public struct Struts
    {
        /// <summary>
        /// 半径
        /// </summary>
        public int Radius;

        /// <summary>
        /// 角度
        /// </summary>
        public int Angle;

        public Struts(int radius, int angle)
        {
            Radius = radius;
            Angle = angle;
        }
    }
}
