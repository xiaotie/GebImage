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

        public PointS(UInt16 x, UInt16 y)
        {
            X = x; Y = y;
        }

        public PointS(Int32 x, Int32 y)
        {
            X = (UInt16)x; Y = (UInt16)y;
        }

        public PointS(Int64 x, Int64 y)
        {
            X = (UInt16)x; Y = (UInt16)y;
        } 
    }

    public class RectD
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public RectD() { }

        public RectD(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public RectF ToRectF()
        {
            return new RectF((float)X, (float)Y, (float)Width, (float)Height);
        }
    }

    public class RectF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public RectF() { }

        public RectF(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public RectD ToRectD()
        {
            return new RectD(X, Y, Width, Height);
        }
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
}
