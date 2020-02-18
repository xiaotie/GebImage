using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public static class ImageClassHelper
    {
        public static ImageBgra32 ToImageBgra32(this Bitmap bitmap)
        {
            if (bitmap == null) return null;

            ImageBgra32 imageBgra32 = new ImageBgra32(bitmap.Width, bitmap.Height);
            imageBgra32.CreateFromBitmap(bitmap);
            return imageBgra32;
        }

        public static unsafe void CreateFromBitmap(this ImageBgra32 image, Bitmap map)
        {
            int height = map.Height;
            int width = map.Width;

            const int PixelFormat32bppCMYK = 8207;

            System.Drawing.Imaging.PixelFormat format = map.PixelFormat;

            Bitmap newMap = map;
            Int32 step = ImageBgra32.SizeOfPixel();

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
                            g.DrawImage(map, new Point());
                        }
                    }
                    else
                    {
                        format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                        newMap = map.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                    break;
            }

            BitmapData data = newMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
            Byte* line = (Byte*)data.Scan0;
            Byte* dstLine = (Byte*)image.Start;
            try
            {
                if (format == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    for (int h = 0; h < height; h++)
                    {
                        UnmanagedImageConverter.ToBgra32((Bgr24*)line, (Bgra32*)dstLine, width);
                        line += data.Stride;
                        dstLine += step * width;
                    }
                }
                else
                {
                    for (int h = 0; h < height; h++)
                    {
                        UnmanagedImageConverter.Copy((byte*)line, (byte*)dstLine, 4 * width);
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
                if (newMap != map)
                {
                    newMap.Dispose();
                }
            }
        }
    }
}
