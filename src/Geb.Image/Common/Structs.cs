/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  为 PointS 添加 Left() 等方法
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 点。横纵坐标都是UInt16格式。
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct PointS
    {
        [FieldOffset(0)]
        public Int16 X;
        [FieldOffset(2)]
        public Int16 Y;

        public PointS(Int16 x, Int16 y)
        {
            X = x; Y = y;
        }

        public PointS(Int32 x, Int32 y)
        {
            X = (Int16)x; Y = (Int16)y;
        }

        public PointS(Int64 x, Int64 y)
        {
            X = (Int16)x; Y = (Int16)y;
        }

        public PointS Right()
        {
            return new PointS(X + 1, Y);
        }

        public PointS Left()
        {
            return new PointS(X - 1, Y);
        }

        public PointS Up()
        {
            return new PointS(X, Y - 1);
        }

        public PointS Down()
        {
            return new PointS(X, Y + 1);
        }

        public PointS RightUp()
        {
            return new PointS(X + 1, Y - 1);
        }

        public PointS LeftUp()
        {
            return new PointS(X - 1, Y - 1);
        }

        public PointS RightDown()
        {
            return new PointS(X + 1, Y + 1);
        }

        public PointS LeftDown()
        {
            return new PointS(X - 1, Y + 1);
        }

        public PointS Move(PointS shift)
        {
            return new PointS(X + shift.X, Y + shift.Y);
        }

        public PointS Move(int xShift, int yShift)
        {
            return new PointS(X + xShift, Y + yShift);
        }

        public Int32 GetHashCode32()
        {
            return Y * Int16.MaxValue + X;
        }

        public static Boolean operator ==(PointS lhs, PointS rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(PointS lhs, PointS rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct Size<T> where T : struct
    {
        public T Width;
        public T Height;

        public Size(T width, T height)
        {
            Width = width;
            Height = height;
        }

        public static Boolean operator ==(Size<T> lhs, Size<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Size<T> lhs, Size<T> rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct Rect
    {
        public Int32 X;
        public Int32 Y;
        public Int32 Width;
        public Int32 Height;

        public int Top { get { return Y; } }
        public int Bottom { get { return Y + Height; } }
        public int Left { get { return X; } }
        public int Right { get { return X + Width; } }

        public Rect(Int32 x = 0, Int32 y = 0, Int32 w = 0, Int32 h = 0)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public Boolean IsContains(Rect other)
        {
            return this.Top <= other.Top
                && this.Bottom >= other.Bottom
                && this.Left <= other.Left
                && this.Right >= other.Right;
        }

        public Boolean IsContains(PointS point)
        {
            return this.Top <= point.Y
                && this.Bottom > point.Y
                && this.Left <= point.X
                && this.Right > point.X;
        }

        public static Boolean operator ==(Rect lhs, Rect rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Rect lhs, Rect rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct RectS
    {
        public Int16 X;
        public Int16 Y;
        public Int16 Width;
        public Int16 Height;

        public int Top { get { return Y; } }
        public int Bottom { get { return Y + Height; } }
        public int Left { get { return X; } }
        public int Right { get { return X + Width; } }

        public RectS(Int16 x = 0, Int16 y = 0, Int16 w = 0, Int16 h = 0)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public Boolean IsContains(Rect other)
        {
            return this.Top <= other.Top
                && this.Bottom >= other.Bottom
                && this.Left <= other.Left
                && this.Right >= other.Right;
        }

        public Boolean IsContains(RectS other)
        {
            return this.Top <= other.Top
                && this.Bottom >= other.Bottom
                && this.Left <= other.Left
                && this.Right >= other.Right;
        }

        public Boolean IsContains(PointS point)
        {
            return this.Top <= point.Y
                && this.Bottom > point.Y
                && this.Left <= point.X
                && this.Right > point.X;
        }

        public static Boolean operator ==(RectS lhs, RectS rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(RectS lhs, RectS rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct RectD
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;

        public RectD(double x = 0, double y = 0, double w = 0, double h = 0)
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

        public static Boolean operator ==(RectD lhs, RectD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(RectD lhs, RectD rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct RectF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public RectF(float x = 0, float y = 0, float w = 0, float h = 0)
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

        public static Boolean operator ==(RectF lhs, RectF rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(RectF lhs, RectF rhs)
        {
            return !lhs.Equals(rhs);
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

        public static Boolean operator ==(ROI lhs, ROI rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(ROI lhs, ROI rhs)
        {
            return !lhs.Equals(rhs);
        }
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

        public static Boolean operator ==(PolarPointD lhs, PolarPointD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(PolarPointD lhs, PolarPointD rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
