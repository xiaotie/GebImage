using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Geb.Image
{
    public static class BitmapExtentions
    {
        public unsafe static ImageU8 ToImageU8(this Bitmap bmp)
        {
            ImageU8 img = new ImageU8(bmp.Width, bmp.Height);

            int height = bmp.Height;
            int width = bmp.Width;

            const int PixelFormat32bppCMYK = 8207;
            var format = bmp.PixelFormat;

            Bitmap newMap = bmp;
            const Int32 step = 1;

            switch (format)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    break;
                default:
                    if ((int)format == PixelFormat32bppCMYK)
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        newMap = new Bitmap(width, height, format);
                        using (Graphics g = Graphics.FromImage(newMap))
                        {
                            g.DrawImage(bmp, new Point());
                        }
                    }
                    else
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                        newMap = bmp.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                    break;
            }

            BitmapData data = newMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
            Byte* line = (Byte*)data.Scan0;
            Byte* dstLine = (Byte*)img.Start;
            try
            {
                if (format == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    for (int h = 0; h < height; h++)
                    {
                        UnmanagedImageConverter.ToByte((Bgr24*)line, dstLine, width);
                        line += data.Stride;
                        dstLine += step * width;
                    }
                }
                else
                {
                    for (int h = 0; h < height; h++)
                    {
                        UnmanagedImageConverter.ToByte((Bgra32*)line, dstLine, width);

                        line += data.Stride;
                        dstLine += step * width;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                newMap.UnlockBits(data);
                if (newMap != bmp)
                {
                    newMap.Dispose();
                }
            }

            return img;
        }

        public static void ShowDialog(this Bitmap bmp)
        {
        }
    }
}
