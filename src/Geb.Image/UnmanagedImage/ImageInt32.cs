/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public struct Int32Converter : IColorConverter
    {
        public unsafe void Copy(Rgb24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToArgb32(from, (Argb32*)to, length);
        }

        public unsafe void Copy(Argb32* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy((Byte*)from, (Byte*)to, length * 4);
        }

        public unsafe void Copy(byte* from, void* to, int length)
        {
            if (length < 1) return;
            Byte* end = from + length;
            Int32* dst = (Int32*)to;
            while (from != end)
            {
                *dst = *from;
                from++;
                dst++;
            }
        }
    }

    public partial class ImageInt32 : UnmanagedImage<Int32>
    {
        public unsafe ImageInt32(Int32 width, Int32 height, void* data)
            : base(width, height,data)
        {
        }

        public unsafe ImageInt32(Int32 width, Int32 height)
            : base(width, height)
        {
        }

        public ImageInt32(Bitmap map)
            : base(map)
        {
        }

        private ImageChannel32 _channel;

        public ImageChannel32 Channel
        {
            get 
            {
                if (_channel == null)
                {
                    lock (this)
                    {
                        if (_channel == null)
                        {
                            _channel = new ImageChannel32(this.Width, this.Height, this.StartIntPtr, sizeof(Int32));
                        }
                    }
                }
                return _channel;
            }
        }

        public unsafe ImageRgb24 ToImageRgb24WithRamdomColorMap()
        {
            ImageRgb24 img = new ImageRgb24(this.Width, this.Height);
            Random r = new Random();
            int length = this.Length;
            Dictionary<int, Rgb24> map = new Dictionary<int, Rgb24>();
            for (int i = 0; i < length; i++)
            {
                int val = this[i];
                if (map.ContainsKey(val))
                {
                    img[i] = map[val];
                }
                else
                {
                    Rgb24 newRgb = new Rgb24();
                    newRgb.Red = (byte)(r.Next(256));
                    newRgb.Green = (byte)(r.Next(256));
                    newRgb.Blue = (byte)(r.Next(256));
                    img[i] = newRgb;
                    map.Add(val, newRgb);
                }
            }
            return img;
        }

        /// <summary>
        /// 直接将 int32 和 32bppArgb 一一对应的复制
        /// </summary>
        /// <param name="map"></param>
        public unsafe Bitmap ToBitmapDirect()
        {
            Bitmap map = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Int32* t = Start;

            BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadWrite, map.PixelFormat);
            try
            {
                int width = map.Width;
                int height = map.Height;

                Byte* line = (Byte*)data.Scan0;

                for (int h = 0; h < height; h++)
                {
                    Int32* p = (Int32*)line;
                    Int32* pEnd = p + width;
                    while(p != pEnd)
                    {
                        *p = *t;
                        p++;
                        t++;
                    }
                    line += data.Stride;
                }
            }
            finally
            {
                map.UnlockBits(data);
            }
            return map;
        }

        /// <summary>
        /// 生成地势图。原理：将参数空间->HSV空间->RGB空间。
        /// 将参数值映射到色相（hue）空间中，使用固定的饱和度与色调，找到对应的RGB值。
        /// </summary>
        /// <param name="hueOfMinVal">最小参数值所对应的色相值</param>
        /// <param name="hueOfMaxVal">最大参数值所对应的色相值</param>
        /// <param name="useBgVal">是否使用背景参数值。如果使用背景参数值，则所有该值将映射到指定的RGB值</param>
        /// <param name="bgVal">背景参数值（只有当useBgVal为true时才有效）</param>
        /// <param name="bgColor">背景参数值所对应的背景色</param>
        /// <returns>所生成的地势图</returns>
        public unsafe ImageRgb24 ToHypsometricMap(double hueOfMinVal, double hueOfMaxVal, bool useBgVal, Int32 bgVal, Rgb24 bgColor)
        {
            int min = int.MaxValue;
            int max = int.MinValue;
            int length = this.Length;
            Int32* start = this.Start;
            for (int i = 0; i < length; i++)
            {
                min = Math.Min(min, start[i]);
                max = Math.Max(max, start[i]);
            }

            double hueDiff = hueOfMaxVal - hueOfMinVal;
            double valDiff = max - min;
            double step = valDiff > 0 ? (hueDiff / valDiff) : 0;
            ImageRgb24 img = new ImageRgb24(Width, Height);
            Rgb24* rgb0 = img.Start;
            for (int i = 0; i < length; i++)
            {
                Int32 val = start[i];
                if (useBgVal && bgVal == val)
                {
                    rgb0[i] = bgColor;
                }
                else
                {
                    // hsv => rgb

                    double h = (val - min) * step + hueOfMinVal;
                    double s = 1;
                    double v = 1;

                    while (h < 0) { h += 360; };
                    while (h >= 360) { h -= 360; };

                    double r, g, b;
                    double hf = h / 60.0;
                    int floor = (int)Math.Floor(hf);
                    double f = hf - floor;
                    double pv = v * (1 - s);
                    double qv = v * (1 - s * f);
                    double tv = v * (1 - s * (1 - f));
                    switch (floor)
                    {
                        case 0:
                            r = v;
                            g = tv;
                            b = pv;
                            break;
                        case 1:
                            r = qv;
                            g = v;
                            b = pv;
                            break;
                        case 2:
                            r = pv;
                            g = v;
                            b = tv;
                            break;
                        case 3:
                            r = pv;
                            g = qv;
                            b = v;
                            break;
                        case 4:
                            r = tv;
                            g = pv;
                            b = v;
                            break;
                        case 5:
                            r = v;
                            g = pv;
                            b = qv;
                            break;
                        case 6:
                            r = v;
                            g = tv;
                            b = pv;
                            break;
                        case -1:
                            r = v;
                            g = pv;
                            b = qv;
                            break;
                        default:
                            r = g = b = v; // Just pretend its black/white
                            break;
                    }
                    int red = (int)(r * 255.0);
                    int green = (int)(g * 255.0);
                    int blue = (int)(b * 255.0);
                    red = Math.Min(255, Math.Max(0, red));
                    green = Math.Min(255, Math.Max(0, green));
                    blue = Math.Min(255, Math.Max(0, blue));
                    rgb0[i] = new Rgb24(red, green, blue);
                }
            }
            return img;
        }

        protected override IColorConverter CreateByteConverter()
        {
            return new Int32Converter();
        }

        public unsafe ImageU8 ToImageU8()
        {
            ImageU8 imgU8 = new ImageU8(this.Width, this.Height);
            int length = imgU8.Length;
            Int32* start = this.Start;
            Byte* dst = imgU8.Start;
            Int32* end = start + this.Length;
            while (start != end)
            {
                int val = *start;
                *dst = val < 0 ? (Byte)0 : (val > 255 ? (Byte)255 : (Byte)val);
                start++;
                dst++;
            }
            return imgU8;
        }

        protected override PixelFormat GetOutputBitmapPixelFormat()
        {
            return PixelFormat.Format8bppIndexed;
        }

        protected override unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            Int32* start = (Int32*)src;
            Int32* end = start + width;
            while (start != end)
            {
                Int32 val = *start;
                val = val < 0? 0: val > 255 ? 255 : val;
                *dst = (byte)val;
                start++;
                dst++;
            }
        }

        /// <summary>
        /// 进行距离变换。距离变换之前，请保证前景像素的值为0，背景像素的值为一个足够大的整数。暂不考虑边界。计算D8距离.
        /// </summary>
        public unsafe void ApplyDistanceTransformFast()
        {
            Int32* start = Start;
            int width = this.Width;
            int height = this.Height;

            Int32 val;

            // 从上向下，从左向右扫描
            for (int h = 1; h < height - 1; h++)
            {
                // 位于每行的头部
                Int32* line0 = start + (h-1) * width;
                Int32* line1 = start + (h ) * width;
                Int32* line2 = start + (h + 1) * width;
                for (int w = 1; w < width; w++)
                {
                    if (line1[1] > 0) // 当前像素
                    {
                        val = Math.Min(line0[0], line0[1]);
                        val = Math.Min(val, line1[0]);
                        val = Math.Min(val, line2[0]);
                        val = Math.Min(val + 1, line1[1]);
                        line1[1] = val;
                    }

                    line0++;
                    line1++;
                    line2++;
                }
            }

            // 从下向上，从右向左扫描
            for (int h = height - 2; h > 0; h--)
            {
                Int32* line0 = start + (h - 1) * width;
                Int32* line1 = start + (h) * width;
                Int32* line2 = start + (h + 1) * width;

                for (int w = width - 2; w >= 0; w--)
                {
                    if (line1[w] > 0)
                    {
                        val = Math.Min(line0[w + 1], line1[w + 1]);
                        val = Math.Min(val, line2[w + 1]);
                        val = Math.Min(val, line2[w]);
                        val = Math.Min(val + 1, line1[w]);
                        line1[w] = val;
                    }
                }
            }
        }

        public unsafe ImageInt32 GaussPyramidUp()
        {
            int width = this.Width;
            int height = this.Height;
            int ww = width / 2;
            int hh = height / 2;

            ImageInt32 imgUp = new ImageInt32(ww, hh);
            Int32* imgStart = Start;
            Int32* imgPyUpStart = imgUp.Start;
            int hSrc, wSrc;
            Int32* lineSrc;
            Int32* lineDst;
            for (int h = 0; h < hh; h++)
            {
                hSrc = 2 * h;
                lineSrc = imgStart + hSrc * width;
                lineDst = imgPyUpStart + h * ww;
                for (int w = 0; w < ww; w++)
                {
                    wSrc = 2 * w;

                    // 对于四边不够一个高斯核半径的地方，直接赋值
                    if (hSrc < 2 || hSrc > height - 3 || wSrc < 2 || wSrc > width - 3)
                    {
                        lineDst[w] = lineSrc[wSrc];
                    }
                    else
                    {
                        // 计算高斯

                        Int32* p = lineSrc + wSrc - 2 * width;

                        Int32 p00 = p[-2];
                        Int32 p01 = p[-1];
                        Int32 p02 = p[0];
                        Int32 p03 = p[1];
                        Int32 p04 = p[2];

                        p += width;
                        Int32 p10 = p[-2];
                        Int32 p11 = p[-1];
                        Int32 p12 = p[0];
                        Int32 p13 = p[1];
                        Int32 p14 = p[2];

                        p += width;
                        Int32 p20 = p[-2];
                        Int32 p21 = p[-1];
                        Int32 p22 = p[0];
                        Int32 p23 = p[1];
                        Int32 p24 = p[2];

                        p += width;
                        Int32 p30 = p[-2];
                        Int32 p31 = p[-1];
                        Int32 p32 = p[0];
                        Int32 p33 = p[1];
                        Int32 p34 = p[2];

                        p += width;
                        Int32 p40 = p[-2];
                        Int32 p41 = p[-1];
                        Int32 p42 = p[0];
                        Int32 p43 = p[1];
                        Int32 p44 = p[2];

                        int val =
                              1 * p00 + 04 * p01 + 06 * p02 + 04 * p03 + 1 * p04
                            + 4 * p10 + 16 * p11 + 24 * p12 + 16 * p13 + 4 * p14
                            + 6 * p20 + 24 * p21 + 36 * p22 + 24 * p23 + 6 * p24
                            + 4 * p30 + 16 * p31 + 24 * p32 + 16 * p33 + 4 * p34
                            + 1 * p40 + 04 * p41 + 06 * p42 + 04 * p43 + 1 * p44;

                        lineDst[w] = val >> 8;
                    }
                }
            }
            return imgUp;
        }
    }
}
