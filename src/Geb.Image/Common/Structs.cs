/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  为 PointS 添加 Left() 等方法
 *      2020.9.21 Hu Fei 添加 Point 类型
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 点。横纵坐标都是Int16格式。
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct PointS
    {
        [FieldOffset(0)]
        public Int16 X;
        [FieldOffset(2)]
        public Int16 Y;

        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }

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

    /// <summary>
    /// 点。横纵坐标都是Int32格式。
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct Point
    {
        [FieldOffset(0)]
        public Int32 X;
        [FieldOffset(2)]
        public Int32 Y;

        public Point ToPointF()
        {
            return new Point(X, Y);
        }

        public Point(Int16 x, Int16 y)
        {
            X = x; Y = y;
        }

        public Point(Int32 x, Int32 y)
        {
            X = x; Y = y;
        }

        public Point(Int64 x, Int64 y)
        {
            X = (Int32)x; Y = (Int32)y;
        }

        public Point Right()
        {
            return new Point(X + 1, Y);
        }

        public Point Left()
        {
            return new Point(X - 1, Y);
        }

        public Point Up()
        {
            return new Point(X, Y - 1);
        }

        public Point Down()
        {
            return new Point(X, Y + 1);
        }

        public Point RightUp()
        {
            return new Point(X + 1, Y - 1);
        }

        public Point LeftUp()
        {
            return new Point(X - 1, Y - 1);
        }

        public Point RightDown()
        {
            return new Point(X + 1, Y + 1);
        }

        public Point LeftDown()
        {
            return new Point(X - 1, Y + 1);
        }

        public Point Move(Point shift)
        {
            return new Point(X + shift.X, Y + shift.Y);
        }

        public Point Move(int xShift, int yShift)
        {
            return new Point(X + xShift, Y + yShift);
        }

        public Int32 GetHashCode32()
        {
            return Y * Int32.MaxValue + X;
        }

        public static Boolean operator ==(Point lhs, Point rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Point lhs, Point rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct Vector2F
    {
        public float X;
        public float Y;

        public Vector2F(float x, float y)
        {
            X = x; Y = y;
        }

        public Vector2F(double x, double y)
        {
            X = (float)x; Y = (float)y;
        }

        public Vector2F(Int16 x, Int16 y)
        {
            X = x; Y = y;
        }

        public Vector2F(Int32 x, Int32 y)
        {
            X = x; Y = y;
        }

        public Vector2F(Int64 x, Int64 y)
        {
            X = (float)x; Y = (float)y;
        }

        public static Boolean operator ==(Vector2F lhs, Vector2F rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Vector2F lhs, Vector2F rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public partial struct PointF
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x; Y = y;
        }

        public PointF(double x, double y)
        {
            X = (float)x; Y = (float)y;
        }

        public PointF(Int16 x, Int16 y)
        {
            X = x; Y = y;
        }

        public PointF(Int32 x, Int32 y)
        {
            X = x; Y = y;
        }

        public PointF(Int64 x, Int64 y)
        {
            X = (float)x; Y = (float)y;
        }

        public static Boolean operator ==(PointF lhs, PointF rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(PointF lhs, PointF rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public partial struct PointD
    {
        public double X;
        public double Y;

        public PointD(float x, float y)
        {
            X = x; Y = y;
        }

        public PointD(double x, double y)
        {
            X = x; Y = y;
        }

        public PointD(Int16 x, Int16 y)
        {
            X = x; Y = y;
        }

        public PointD(Int32 x, Int32 y)
        {
            X = x; Y = y;
        }

        public PointD(Int64 x, Int64 y)
        {
            X = (float)x; Y = (float)y;
        }

        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }

        public static Boolean operator ==(PointD lhs, PointD rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(PointD lhs, PointD rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct TriangleF
    {
        public PointF P0;
        public PointF P1;
        public PointF P2;

        public TriangleF(PointF p0, PointF p1, PointF p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
        }

        public Boolean Contains(PointF p)
        {
            float maxX = float.MinValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float minY = float.MaxValue;
            maxX = Math.Max(maxX, P0.X);
            maxX = Math.Max(maxX, P1.X);
            maxX = Math.Max(maxX, P2.X);
            minX = Math.Min(minX, P0.X);
            minX = Math.Min(minX, P1.X);
            minX = Math.Min(minX, P2.X);
            maxY = Math.Max(maxY, P0.Y);
            maxY = Math.Max(maxY, P1.Y);
            maxY = Math.Max(maxY, P2.Y);
            minY = Math.Min(minY, P0.Y);
            minY = Math.Min(minY, P1.Y);
            minY = Math.Min(minY, P2.Y);

            if (maxX < p.X || maxY < p.Y || minX > p.X || minY > p.Y) return false;

            return (IsOnSameSide(p, P0, P1, P2) == false)
                && (IsOnSameSide(p, P1, P2, P0) == false)
                && (IsOnSameSide(p, P2, P0, P1) == false);
        }

        private Boolean IsOnSameSide(PointF p0, PointF p1, PointF p2, PointF p3)
        {
            float a = p0.Y - p1.Y;
            float b = p1.X - p0.X;
            float c = p0.X * p1.Y - p1.X * p0.Y;
            return (a * p2.X + b * p2.Y + c) * (a * p3.X + b * p3.Y + c) >= 0;
        }
    }

    public struct Size
    {
        public Int32 Width;
        public Int32 Height;

        public Size(Int32 width, Int32 height)
        {
            Width = width;
            Height = height;
        }

        public static Boolean operator ==(Size lhs, Size rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Size lhs, Size rhs)
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

        public Boolean IsIntersects(Rect r)
        {
            return !(r.Left>Right || r.Right<Left || r.Top>Bottom || r.Bottom<Top);
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

        public Boolean IsIntersects(Rect r)
        {
            return !(r.Left > Right || r.Right < Left || r.Top > Bottom || r.Bottom < Top);
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
