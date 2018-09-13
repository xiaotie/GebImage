/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace Geb.Image
{
    public partial class ImageBgra32 : IDisposable
    {
        public const int ChannelCount = 4;

        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Bgr24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToBgra32(from, (Bgra32*)to, length);
        }

        private unsafe void Copy(Bgra32* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy((byte*)from, (byte*)to, 4 * length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.ToArgb32(from, (Bgra32*)to, length);
        }

        private PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format32bppBgra;
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.Copy(src, dst, width * 4);
        }
        
        #endregion

        public ImageU8 ToGrayscaleImage()
        {
            return ToGrayscaleImage(0.299, 0.587, 0.114);
        }

        public void SaveBmp(String imagePath)
        {
            new Formats.Bmp.BmpEncoder().Encode(this, imagePath);
        }

        public void SaveJpeg(String imagePath,int quality = 70)
        {
            var encoder = new Formats.Jpeg.JpegEncoder();
            encoder.Quality = quality;
            encoder.Encode(this, imagePath);
        }

        public void SavePng(String imagePath)
        {
            var encoder = new Formats.Png.PngEncoder();
            encoder.Encode(this, imagePath);
        }

        public ImageU8 ToGrayscaleImage(byte transparentColor)
        {
            return ToGrayscaleImage(0.299, 0.587, 0.114, transparentColor);
        }

        public unsafe ImageU8 ToGrayscaleImage(double rCoeff, double gCoeff, double bCoeff, byte transparentColor)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Bgra32* p = Start;
            Byte* to = img.Start;
            Bgra32* end = p + Length;

            while (p != end)
            {
                if (p->Alpha == 0)
                {
                    *to = transparentColor;
                }
                else
                {
                    *to = (Byte)(p->Red * rCoeff + p->Green * gCoeff + p->Blue * bCoeff);
                }
                p++;
                to++;
            }

            return img;
        }

        public enum Channel
        {
            Blue = 0, Green = 1, Red = 2, Alpha = 3
        }

        public unsafe ImageU8 ToGrayscaleImage(Channel channel)
        {
            int offset = (int)channel;
            if (offset < 0) offset = 0;
            else if (offset > 3) offset = 3;

            ImageU8 img = new ImageU8(this.Width, this.Height);
            Byte* p = (Byte*)Start + offset;
            Byte* to = img.Start;
            Byte* end = p + Length * 4;

            while (p < end)
            {
                *to = *p;
                p+=4;
                to++;
            }

            return img;
        }

        public unsafe ImageU8 ToGrayscaleImage(double rCoeff, double gCoeff, double bCoeff)
        {
            ImageU8 img = new ImageU8(this.Width, this.Height);
            Bgra32* p = Start;
            Byte* to = img.Start;
            Bgra32* end = p + Length;

            if (Length < 1024)
            {
                while (p != end)
                {
                    *to = (Byte)(p->Red * rCoeff + p->Green * gCoeff + p->Blue * bCoeff);
                    p++;
                    to++;
                }
            }
            else
            {
                int* bCache = stackalloc int[256];
                int* gCache = stackalloc int[256];
                int* rCache = stackalloc int[256];

                const int shift = 1 << 10;
                int rShift = (int)(rCoeff * shift);
                int gShift = (int)(gCoeff * shift);
                int bShift = shift - rShift - gShift;

                int r = 0, g = 0, b = 0;
                for (int i = 0; i < 256; i++)
                {
                    bCache[i] = b;
                    gCache[i] = g;
                    rCache[i] = r;
                    b += bShift;
                    g += gShift;
                    r += rShift;
                }

                while (p != end)
                {
                    *to = (Byte)((bCache[p->Red] + gCache[p->Green] + rCache[p->Red]) >> 10);

                    p++;
                    to++;
                }
            }
            return img;
        }

        public unsafe void SetAlpha(byte alpha)
        {
            Bgra32* start = (Bgra32*)this.Start;
            Bgra32* end = start + this.Length;
            while (start != end)
            {
                start->Alpha = alpha;
                start++;
            }
        }

        public unsafe void CombineAlpha(ImageBgra32 src, System.Drawing.Point start, System.Drawing.Rectangle region, System.Drawing.Point destAnchor)
        {
            if (start.X >= src.Width || start.Y >= src.Height) return;
            int startSrcX = Math.Max(0, start.X);
            int startSrcY = Math.Max(0, start.Y);
            int endSrcX = Math.Min(start.X + region.Width, src.Width);
            int endSrcY = Math.Min(start.Y + region.Height, src.Height);
            int offsetX = start.X < 0 ? -start.X : 0;
            int offsetY = start.Y < 0 ? -start.Y : 0;
            offsetX = destAnchor.X + offsetX;
            offsetY = destAnchor.Y + offsetY;
            int startDstX = Math.Max(0, offsetX);
            int startDstY = Math.Max(0, offsetY);
            offsetX = offsetX < 0 ? -offsetX : 0;
            offsetY = offsetY < 0 ? -offsetY : 0;
            startSrcX += offsetX;
            startSrcY += offsetY;
            int endDstX = Math.Min(destAnchor.X + region.Width, this.Width);
            int endDstY = Math.Min(destAnchor.Y + region.Height, this.Height);
            int copyWidth = Math.Min(endSrcX - startSrcX, endDstX - startDstX);
            int copyHeight = Math.Min(endSrcY - startSrcY, endDstY - startDstY);
            if (copyWidth <= 0 || copyHeight <= 0) return;

            int srcWidth = src.Width;
            int dstWidth = this.Width;

            Bgra32* srcLine = (Bgra32*)(src.Start) + srcWidth * startSrcY + startSrcX;
            Bgra32* dstLine = this.Start + dstWidth * startDstY + startDstX;
            Bgra32* endSrcLine = srcLine + srcWidth * copyHeight;
            while (srcLine < endSrcLine)
            {
                Bgra32* pSrc = srcLine;
                Bgra32* endPSrc = pSrc + copyWidth;
                Bgra32* pDst = dstLine;
                while (pSrc < endPSrc)
                {
                    Bgra32 p0 = *pSrc;
                    Bgra32 p1 = *pDst;
                    switch (p0.Alpha)
                    {
                        case 255:
                            *pDst = p0;
                            break;
                        case 0:
                        default:
                            break;
                    }
                    pSrc++;
                    pDst++;
                }
                srcLine += srcWidth;
                dstLine += dstWidth;
            }
        }

    }
}
