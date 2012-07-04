/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Geb.Image
{
    public static class ClassHelper
    {
        public static System.Drawing.Bitmap Resize(this System.Drawing.Bitmap bmp, int width, int height)
        {
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }
            return result;
        }

        public static int Area(this Rectangle rec)
        {
            return rec.Width * rec.Height;
        }

        public static Boolean IsContain(this Rectangle rec, Rectangle other)
        {
            return rec.Top <= other.Top
                && rec.Bottom >= other.Bottom
                && rec.Left <= other.Left
                && rec.Right >= other.Right;
        }

        public static Boolean IsContain(this Rectangle rec, Point point)
        {
            return rec.Top <= point.Y
                && rec.Bottom > point.Y
                && rec.Left <= point.X
                && rec.Right > point.X;
        }

        public static Point Right(this Point p)
        {
            return new Point(p.X+1, p.Y);
        }

        public static Point Left(this Point p)
        {
            return new Point(p.X - 1, p.Y);
        }

        public static Point Upper(this Point p)
        {
            return new Point(p.X, p.Y-1);
        }

        public static Point Lower(this Point p)
        {
            return new Point(p.X, p.Y+1);
        }

        public static Point UpperRight(this Point p)
        {
            return new Point(p.X + 1, p.Y-1);
        }

        public static Point UpperLeft(this Point p)
        {
            return new Point(p.X - 1, p.Y-1);
        }

        public static Point LowerRight(this Point p)
        {
            return new Point(p.X + 1, p.Y+1);
        }

        public static Point LowerLeft(this Point p)
        {
            return new Point(p.X - 1, p.Y+1);
        }

        public static Point Move(this Point p, Point shift)
        {
            return new Point(p.X + shift.X, p.Y + shift.Y);
        }

        public static Point Move(this Point p, int xShift, int yShift)
        {
            return new Point(p.X + xShift, p.Y + yShift);
        }

        public static int GetHashCode32(this Point p)
        {
            return p.Y * Int16.MaxValue + p.X;
        }

        public static PolarPointD ToPolarPointD(this Point p)
        {
            double angle = Math.Atan2(p.Y, p.X) * (180 / Math.PI);
            if (angle < 0) angle = 360 + angle;
            double radius = Math.Sqrt(p.X * p.X + p.Y * p.Y);
            return new PolarPointD(radius, angle);
        }

        public static Struts ToPolarPoint(this Point p)
        {
            double angle = Math.Atan2(p.Y, p.X) * (180 / Math.PI);
            if (angle < 0) angle = 360 + angle;
            double radius = Math.Sqrt(p.X * p.X + p.Y * p.Y);
            return new Struts((int)radius, (int)angle);
        }

        public static Bitmap CloneBitmap(this Bitmap map, PixelFormat dstFormat)
        {
            PixelFormat format = map.PixelFormat;
            Bitmap newMap = null;

            const int PixelFormat32bppCMYK = 8207;
            if ((int)format == PixelFormat32bppCMYK)
            {
                format = PixelFormat.Format24bppRgb;
                newMap = new Bitmap(map.Width, map.Height, format);
                using (Graphics g = Graphics.FromImage(newMap))
                {
                    g.DrawImage(map, new Point());
                }
            }
            else
            {
                format = PixelFormat.Format32bppArgb;
                newMap = map.Clone(new Rectangle(0, 0, map.Width, map.Height), dstFormat);
            }

            return newMap;
        }

        public static void Fill(this Bitmap bmp, PointF p0, PointF p1, PointF p2, PointF p3, Color color)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillPolygon(new SolidBrush(color), new PointF[] { p0, p1, p2, p3 });
            }
        }

        public static void InitGrayscalePalette(this System.Drawing.Image img)
        {
            ColorPalette palette = img.Palette;
            for (int i = 0; i < 255; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            img.Palette = palette;
        }
    }
}
