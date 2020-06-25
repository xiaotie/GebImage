/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 *      2012.12.13 Hu Fei  调整 Point 的扩展方法的名称。
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

        #region Image 的扩展方法

        public static void InitGrayscalePalette(this System.Drawing.Image img)
        {
            ColorPalette palette = img.Palette;
            for (int i = 0; i < 255; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            img.Palette = palette;
        }

        #endregion

        #region Bitmap 的扩展方法

        /// <summary>
        /// 弹出模态窗口，显示Bitmap图像。安静模式下不弹出窗口，直接返回。
        /// </summary>
        /// <param name="bmp">Bitmap图像</param>
        /// <param name="title">模态窗口的标题</param>
        /// <returns>当前Bitmap图像</returns>
        //public static Bitmap ShowDialog(this Bitmap bmp, String title = null, Boolean zoom = true)
        //{
        //    if (Config.SilentMode == false)  // 非安静模式下，弹出窗体
        //    {
        //        ImageBox.ShowDialog(bmp, title, zoom);
        //    }

        //    return bmp;
        //}

        /// <summary>
        /// 复制 Bitmap
        /// </summary>
        /// <param name="bmp">Bitmap对象</param>
        /// <returns>复制后的Bitmap对象</returns>
        public static Bitmap CloneBitmap(this Bitmap bmp)
        {
            return bmp.Clone() as Bitmap;
        }

        /// <summary>
        /// 用指定的颜色来填充 p0,p1,p2,p3 组成的多边形
        /// </summary>
        /// <param name="bmp">Bitmap对象</param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="color">填充色</param>
        public static void Fill(this Bitmap bmp, PointF p0, PointF p1, PointF p2, PointF p3, Color color)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillPolygon(new SolidBrush(color), new System.Drawing.PointF[]
                {
                    new System.Drawing.PointF(p0.X, p0.Y),
                    new System.Drawing.PointF(p1.X, p1.Y),
                    new System.Drawing.PointF(p2.X, p2.Y),
                    new System.Drawing.PointF(p3.X, p3.Y)
                }
                );
            }
        }

        /// <summary>
        /// 缩放Bitmap图像
        /// </summary>
        /// <param name="bmp">将Bitmap图像缩放到指定的宽度和高度</param>
        /// <param name="width">缩放后的宽度</param>
        /// <param name="height">缩放后的高度</param>
        /// <param name="disposePolicy">转换完毕后的Dispose策略，默认为DisposePolicy.None</param>
        /// <returns>缩放后的新Bitmap图像</returns>
        public static Bitmap Resize(this Bitmap bmp, int width, int height, System.Drawing.Drawing2D.InterpolationMode mode = System.Drawing.Drawing2D.InterpolationMode.Default, DisposePolicy disposePolicy = DisposePolicy.None)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = mode;
                g.DrawImage(bmp, 0, 0, width, height);
            }
            if (disposePolicy == DisposePolicy.DisposeCaller) bmp.Dispose();
            return result;
        }

        /// <summary>
        /// 转换为指定格式的 Bitmap 
        /// </summary>
        /// <param name="bmp">Bitmap对象</param>
        /// <param name="dstFormat">指定的PixelFormat</param>
        /// <returns>新的指定格式的Bitmap</returns>
        public static Bitmap ToBitmap(this Bitmap bmp, System.Drawing.Imaging.PixelFormat dstFormat, DisposePolicy disposePolicy = DisposePolicy.None)
        {
            var format = bmp.PixelFormat;
            Bitmap newMap = null;

            const int PixelFormat32bppCMYK = 8207;
            if ((int)format == PixelFormat32bppCMYK)
            {
                format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                newMap = new Bitmap(bmp.Width, bmp.Height, format);
                using (Graphics g = Graphics.FromImage(newMap))
                {
                    g.DrawImage(bmp, new Point());
                }
            }
            else
            {
                format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                newMap = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), dstFormat);
            }

            if (disposePolicy == DisposePolicy.DisposeCaller) bmp.Dispose();
            return newMap;
        }

        /// <summary>
        /// 将Bitmap图像转换成 ImageBgra32 图像
        /// </summary>
        /// <param name="bmp">Bitmap 图像</param>
        /// <param name="disposePolicy">转换完毕后的Dispose策略，默认为DisposePolicy.None</param>
        /// <returns>ImageBgra32 图像</returns>
        public static ImageBgra32 ToImageBgra32(this Bitmap bmp, DisposePolicy disposePolicy = DisposePolicy.None)
        {
            ImageBgra32 img = new ImageBgra32(bmp);
            if (disposePolicy == DisposePolicy.DisposeCaller) bmp.Dispose();
            return img;
        }

        #endregion

        public static System.Drawing.Imaging.PixelFormat ToSystemDrawingPixelFormat(this Geb.Image.PixelFormat fmt)
        {
            switch (fmt)
            {
                case PixelFormat.Format8bpp:
                    return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                    break;
                case PixelFormat.Format24bppBgr:
                    return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                    break;
                case PixelFormat.Format32bppBgra:
                    return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                    break;
                default:
                    throw new ArgumentException("Invalid PixelFormat");
            }
        }

        public static int Area(this Rectangle rec)
        {
            return rec.Width * rec.Height;
        }

        public static Boolean IsContains(this Rectangle rec, Rectangle other)
        {
            return rec.Top <= other.Top
                && rec.Bottom >= other.Bottom
                && rec.Left <= other.Left
                && rec.Right >= other.Right;
        }

        public static Boolean IsContains(this Rectangle rec, Point point)
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

        public static Point Up(this Point p)
        {
            return new Point(p.X, p.Y-1);
        }

        public static Point Down(this Point p)
        {
            return new Point(p.X, p.Y+1);
        }

        public static Point RightUp(this Point p)
        {
            return new Point(p.X + 1, p.Y-1);
        }

        public static Point LeftUp(this Point p)
        {
            return new Point(p.X - 1, p.Y-1);
        }

        public static Point RightDown(this Point p)
        {
            return new Point(p.X + 1, p.Y+1);
        }

        public static Point LeftDown(this Point p)
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
    }
}
