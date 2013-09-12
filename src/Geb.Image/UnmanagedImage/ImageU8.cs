/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Geb.Image
{
    public partial class ImageU8 : IDisposable
    {
        public const int ChannelCount = 1;

        #region Image <-> Bitmap 所需的方法

        private unsafe void Copy(Rgb24* from, void* to, int length)
        {
            UnmanagedImageConverter.ToByte(from, (byte*)to, length);
        }

        private unsafe void Copy(Argb32* from, void* to, int length)
        {
            UnmanagedImageConverter.ToByte(from, (byte*)to, length);
        }

        private unsafe void Copy(byte* from, void* to, int length)
        {
            UnmanagedImageConverter.Copy(from, (byte*)to, length);
        }

        private System.Drawing.Imaging.PixelFormat GetOutputBitmapPixelFormat()
        {
            return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
        }

        private unsafe void ToBitmapCore(byte* src, byte* dst, int width)
        {
            UnmanagedImageConverter.Copy(src, dst, width);
        }

        #endregion

        public unsafe ImageU8 ApplyInvert()
        {
            Byte* p = this.Start;
            Byte* end = p + this.Length;
            while (p != end)
            {
                *p = (Byte)(255 - *p);
                p++;
            }
            return this;
        }

        public unsafe ImageArgb32 ToImageArgb32()
        {
            ImageArgb32 img = new ImageArgb32(this.Width, this.Height);
            Argb32* p = img.Start;
            Byte* to = Start;
            Argb32* end = p + Length;
            while (p != end)
            {
                p->Alpha = 255;
                p->Red = *to;
                p->Green = *to;
                p->Blue = *to;

                p++;
                to++;
            }
            return img;
        }

        public unsafe ImageRgb24 ToImageRgb24()
        {
            ImageRgb24 img = new ImageRgb24(this.Width, this.Height);
            Rgb24* p = img.Start;
            Byte* to = Start;
            Rgb24* end = p + Length;
            while (p != end)
            {
                p->Red = *to;
                p->Green = *to;
                p->Blue = *to;

                p++;
                to++;
            }
            return img;
        }

        /// <summary>
        /// 计算八联结的联结数，计算公式为：
        ///     (p6 - p6*p7*p0) + sigma(pk - pk*p(k+1)*p(k+2)), k = {0,2,4)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private unsafe Int32 DetectConnectivity(Int32* list)
        {
            Int32 count = list[6] - list[6] * list[7] * list[0];
            count += list[0] - list[0] * list[1] * list[2];
            count += list[2] - list[2] * list[3] * list[4];
            count += list[4] - list[4] * list[5] * list[6];
            return count;
        }

        private unsafe void FillNeighbors(Byte* p, Int32* list, Int32 width, Byte foreground = 255)
        {
            // list 存储的是补集，即前景点为0，背景点为1，以方便联结数的计算

            list[0] = p[1] == foreground ? 0 : 1;
            list[1] = p[1 - width] == foreground ? 0 : 1;
            list[2] = p[-width] == foreground ? 0 : 1;
            list[3] = p[-1 - width] == foreground ? 0 : 1;
            list[4] = p[-1] == foreground ? 0 : 1;
            list[5] = p[-1 + width] == foreground ? 0 : 1;
            list[6] = p[width] == foreground ? 0 : 1;
            list[7] = p[1 + width] == foreground ? 0 : 1;
        }

        /// <summary>
        /// 使用 hilditch 算法进行细化
        /// </summary>
        public unsafe void Thinning(Byte foreground = 255)
        {
            // TODO: laviewpbt 说这个算法有问题，性能不是最高，且有错误。他的代码如下：
            /*
        // http://www.cnblogs.com/xiaotie/archive/2010/08/12/1797760.html
        public static void ThinUseHilditchAndXiaoTie(FastBitmap bmp)
        {
            int P1, P2, P3, P4, P5, P6, P7, P8;
            int Speed, Fast, Connectivity;
            int Width, Height, Stride, X, Y, Sum;
            byte* Pointer;
            bool Start = true;
            Width = bmp.Width; Height = bmp.Height; Stride = bmp.Stride;
            byte* Mark = (byte*)Marshal.AllocHGlobal(Stride * Height);          // 用于标记那些点是已经被处理为背景了
            byte* Clone = (byte*)Marshal.AllocHGlobal(Stride * Height);         // 图像数据的克隆
            Win32Api.FillMemory((IntPtr)Mark, Stride * Height, 0);              // 先都为0
            Win32Api.CopyMemory(Clone, bmp.Pointer, Stride * Height);           // 复制数据
            Pointer = bmp.Pointer;

            while (Start == true)
            {
                Start = false;
                for (Y = 1; Y < Height - 1; Y++)
                {
                    Speed = Stride * Y;
                    for (X = 1; X < Width - 1; X++)
                    {
                        Speed++;
                        if (Clone[Speed] != 255) continue;                  // 条件1：必须是前景点（=255）

                        // P4 P3 P2
                        // P5 P0 P1
                        // P6 P7 P8
                        Fast = Speed - Stride;                              // 获取其周边的点，将前景点（白色）映射为0，背景点（黑色）隐射为1
                        P4 = 1 - (Clone[Fast - 1] & 1);                     // 以方便计算8连通联结数。
                        P3 = 1 - (Clone[Fast] & 1);
                        P2 = 1 - (Clone[Fast + 1] & 1);
                        P5 = 1 - (Clone[Speed - 1] & 1);
                        P1 = 1 - (Clone[Speed + 1] & 1);
                        Fast = Speed + Stride;
                        P6 = 1 - (Clone[Fast - 1] & 1);
                        P7 = 1 - (Clone[Fast] & 1);
                        P8 = 1 - (Clone[Fast + 1] & 1);

                        Sum = P1 + P3 + P5 + P7;                            // 如果先只取这四个变量的值，然后根据判断结果来决定是否需要取后结果值，实践证明速度没什么提升
                        if (Sum == 0) continue;                             // 条件2： P1,P3,P5,P7不全部为前景点

                        Sum += P2 + P4 + P6 + P8;
                        if (Sum > 6) continue;                              // 条件3：至少有两个是前景点

                        Connectivity = P7 - (P7 & P8 & P1)                  // 计算8连通联结数,0和1之间的乘法可以优化为位运算
                                     + P1 - (P1 & P2 & P3)
                                     + P3 - (P3 & P4 & P5)
                                     + P5 - (P5 & P6 & P7);
                        if (Connectivity != 1) continue;                    // 条件4：P0的8连通联结数为必须为

                        if (Mark[Speed - Stride] == 255)
                        {
                            Connectivity = P7 - (P7 & P8 & P1)
                                        + P1 - (P1 & P2)
                                        + 1 - (P4 & P5)
                                        + P5 - (P5 & P6 & P7);
                            if (Connectivity != 1) continue;                // 条件5: 假设P3已经标记删除，那么当P3为背景色（P3=1)时，P0的8联通联结数为必须为1
                        }

                        if (Mark[Speed - 1] == 255)
                        {
                            Connectivity = P7 - (P7 & P8 & P1)
                                         + P1 - (P1 & P2 & P3)
                                         + P3 - (P3 & P4)
                                         + 1 - (P6 & P7);
                            if (Connectivity != 1) continue;                // 条件6: 假设P5已经标记删除，那么当P5为背景色（P5=1)时，P0的8联通联结数为必须为1
                        }
                        Mark[Speed] = 255;                                  // 标记改点（前景点）已经被删除
                        Pointer[Speed] = 0;                                 // 将前景色更改为背景色
                        Start = true;                                       // 需要继续循环
                    }
                }
                Win32Api.CopyMemory(Clone, Pointer, Stride * Height);       // 注意上面的循环体里用的备份数据，当原始数据更新后，必须同步更新备份数据，这个拷贝的速度要快很多
            }
            Marshal.FreeHGlobal((IntPtr)Mark);
            Marshal.FreeHGlobal((IntPtr)Clone);
        }

            */
            Byte* start = this.Start;
            Int32 width = this.Width;
            Int32 height = this.Height;
            Int32* list = stackalloc Int32[8];
            Byte background = (Byte)(255 - foreground);
            Int32 length = this.Length;

            using (ImageU8 mask = new ImageU8(this.Width, this.Height))
            {
                mask.Fill(0);

                Boolean loop = true;
                while (loop == true)
                {
                    loop = false;
                    for (Int32 r = 1; r < height - 1; r++)
                    {
                        for (Int32 c = 1; c < width - 1; c++)
                        {
                            Byte* p = start + r * width + c;

                            // 条件1：p 必须是前景点
                            if (*p != foreground) continue;

                            //  p3  p2  p1
                            //  p4  p   p0
                            //  p5  p6  p7
                            // list 存储的是补集，即前景点为0，背景点为1，以方便联结数的计算
                            FillNeighbors(p, list, width, foreground);

                            // 条件2：p0,p2,p4,p6 不皆为前景点
                            if (list[0] == 0 && list[2] == 0 && list[4] == 0 && list[6] == 0)
                                continue;

                            // 条件3: p0~p7至少两个是前景点
                            Int32 count = 0;
                            for (int i = 0; i < 8; i++)
                            {
                                count += list[i];
                            }

                            if (count > 6) continue;

                            // 条件4：联结数等于1
                            if (DetectConnectivity(list) != 1) continue;

                            // 条件5: 假设p2已标记删除，则令p2为背景，不改变p的联结数
                            if (mask[r - 1, c] == 1)
                            {
                                list[2] = 1;
                                if (DetectConnectivity(list) != 1)
                                    continue;
                                list[2] = 0;
                            }

                            // 条件6: 假设p4已标记删除，则令p4为背景，不改变p的联结数
                            if (mask[r, c - 1] == 1)
                            {
                                list[4] = 1;
                                if (DetectConnectivity(list) != 1)
                                    continue;
                            }
                            mask[r, c] = 1; // 标记删除
                            loop = true;
                        }
                    }

                    for (int i = 0; i < length; i++)
                    {
                        if (mask[i] == 1)
                        {
                            this[i] = background;
                        }
                    }
                }
            }
        }

        public unsafe void SkeletonizeByMidPoint(Byte foreground = 255)
        {
            using (ImageU8 mask = new ImageU8(this.Width, this.Height))
            {
                mask.Fill(0);
                Int32 width = this.Width;
                Int32 height = this.Height;
                for (Int32 r = 0; r < height; r++)
                {
                    Int32 lineStart = -1;
                    for (Int32 c = 0; c < width; c++)
                    {
                        if (this[r, c] == foreground)
                        {
                            if (lineStart == -1) lineStart = c;
                        }
                        else
                        {
                            if (lineStart != -1)
                            {
                                mask[r, (lineStart + c) / 2] = 1;
                                lineStart = -1;
                            }
                        }
                    }
                }

                for (Int32 c = 0; c < width; c++)
                {
                    Int32 lineStart = -1;
                    for (Int32 r = 0; r < height; r++)
                    {
                        if (this[r, c] == foreground)
                        {
                            if (lineStart == -1) lineStart = r;
                        }
                        else
                        {
                            if (lineStart != -1)
                            {
                                mask[(lineStart + r) / 2, c] = 1;
                                lineStart = -1;
                            }
                        }
                    }
                }

                Byte bg = (Byte)(255 - foreground);
                Int32 length = this.Length;
                for (int i = 0; i < length; i++)
                {
                    if (mask[i] == 1)
                    {
                        this[i] = foreground;
                    }
                    else
                    {
                        this[i] = bg;
                    }
                }
            }
        }

        /// <summary>
        /// 创建积分图像
        /// </summary>
        /// <returns>所得到的积分图像</returns>
        public unsafe ImageInt32 CreateIntegral()
        {
            int width = Width;
            int height = Height;
            int widthPlus = width + 1;
            ImageInt32 img = new ImageInt32(width, height);
            Byte* p = this.Start;
            Byte* pEnd = null;
            Int32* v = img.Start;
            
            // 第一行
            pEnd = p + width;
            *v = *p;
            p++; v++;   // 第一个元素跳开
            while(p < pEnd) // 剩下的元素
            {
                *v = v[-1] + *p;
                p++;
                v++;
            }

            for (int h = 1; h < height; h++)
            {
                p = this.Start + h * width;
                v = img.Start + h * width;
                pEnd = p + width;
                *v = *p + v[-width];
                p++; v++;

                while (p < pEnd) // 剩下的元素
                {
                    *v = v[-1] + v[-width] - v[-widthPlus] + *p;
                    p++;
                    v++;
                }
            }

            return img;
        }

        /// <summary>
        /// 创建平方后的积分图像
        /// </summary>
        /// <returns></returns>
        public unsafe ImageInt32 CreateSquareIntegral()
        {
            int width = Width;
            int height = Height;
            int widthPlus = width + 1;
            ImageInt32 img = new ImageInt32(width, height);
            Byte* p = this.Start;
            Byte* pEnd = null;
            Int32* v = img.Start;

            // 第一行
            pEnd = p + width;
            *v = (*p) * (*p);
            p++; v++;   // 第一个元素跳开
            while (p < pEnd) // 剩下的元素
            {
                *v = v[-1] + (*p)*(*p);
                p++;
                v++;
            }

            for (int h = 1; h < height; h++)
            {
                p = this.Start + h * width;
                v = img.Start + h * width;
                pEnd = p + width;
                *v = (*p) * (*p) + v[-width];
                p++; v++;

                while (p < pEnd) // 剩下的元素
                {
                    *v = v[-1] + v[-width] - v[-widthPlus] + (*p) * (*p);
                    p++;
                    v++;
                }
            }

            return img;
        }

        public unsafe ImageInt32 ToImageInt32()
        {
            ImageInt32 img32 = new ImageInt32(this.Width, this.Height);
            Byte* start = this.Start;
            Byte* end = this.Start + this.Length;
            Int32* dst = img32.Start;
            while (start != end)
            {
                *dst = *start;
                start++;
                dst++;
            }
            return img32;
        }

        public void ApplyGaussianBlur(double sigma = 1.4, int size = 5)
        {
            ConvolutionKernel kernel = ConvolutionKernel.CreateGaussianKernel(sigma, size);
            this.ApplyConvolution(kernel);
        }

        public unsafe ImageU8 ApplyCannyEdgeDetector(double gaussianSiama = 1.4, int gaussianSize = 5, byte lowThreshold = 20, byte highThreshold = 100)
        {
            int startX = 1;
            int startY = 1;
            int width = this.Width;
            int height = this.Height;

            int stopX = width - 1;
            int stopY = height - 1;
            int ww = width - 2;
            int hh = height - 2;

            using (ImageU8 copy = this.Clone() as ImageU8)
            using (ImageU8 orients = new ImageU8(ww, hh))             // orientation array
            using (ImageFloat gradients = new ImageFloat(this.Width, this.Height))                  // gradients array
            {
                float maxGradient = float.NegativeInfinity;
                double gx, gy;
                double orientation, toAngle = 180.0 / System.Math.PI;
                float leftPixel = 0, rightPixel = 0;

                // 第一步，Gauss 平滑
                copy.ApplyGaussianBlur(gaussianSiama, gaussianSize);
                byte* start = copy.Start + startX;
                byte* p;
                int o = 0;
                for (int y = startY; y < stopY; y++)
                {
                    p = start + y * width;
                    for (int x = startX; x < stopX; x++, p++, o++)
                    {
                        gx = p[-width + 1] + p[width + 1]
                            - p[-width - 1] - p[width - 1]
                            + 2 * (p[1] - p[-1]);
                        gy = p[-width - 1] + p[-width + 1]
                            - p[width - 1] - p[width + 1]
                            + 2 * (p[-width] - p[width]);
                        gradients[y, x] = (float)Math.Sqrt(gx * gx + gy * gy);
                        if (gradients[y, x] > maxGradient)
                            maxGradient = gradients[y, x];

                        // get orientation
                        if (gx == 0)
                        {
                            orientation = (gy == 0) ? 0 : 90;
                        }
                        else
                        {
                            double div = gy / gx;
                            // handle angles of the 2nd and 4th quads
                            if (div < 0)
                            {
                                orientation = 180 - System.Math.Atan(-div) * toAngle;
                            }
                            // handle angles of the 1st and 3rd quads
                            else
                            {
                                orientation = System.Math.Atan(div) * toAngle;
                            }

                            // get closest angle from 0, 45, 90, 135 set
                            if (orientation < 22.5)
                                orientation = 0;
                            else if (orientation < 67.5)
                                orientation = 45;
                            else if (orientation < 112.5)
                                orientation = 90;
                            else if (orientation < 157.5)
                                orientation = 135;
                            else orientation = 0;
                        }

                        // save orientation
                        orients[o] = (byte)orientation;
                    }
                }

                // STEP 3 - suppres non maximums
                o = 0;
                start = this.Start + startX;
                for (int y = startY; y < stopY; y++)
                {
                    p = start + y * width;
                    // for each pixel
                    for (int x = startX; x < stopX; x++, p++, o++)
                    {
                        // get two adjacent pixels
                        switch (orients[o])
                        {
                            case 0:
                                leftPixel = gradients[y, x - 1];
                                rightPixel = gradients[y, x + 1];
                                break;
                            case 45:
                                leftPixel = gradients[y + 1, x - 1];
                                rightPixel = gradients[y - 1, x + 1];
                                break;
                            case 90:
                                leftPixel = gradients[y + 1, x];
                                rightPixel = gradients[y - 1, x];
                                break;
                            case 135:
                                leftPixel = gradients[y + 1, x + 1];
                                rightPixel = gradients[y - 1, x - 1];
                                break;
                        }
                        // compare current pixels value with adjacent pixels
                        if ((gradients[y, x] < leftPixel) || (gradients[y, x] < rightPixel))
                        {
                            *p = 0;
                        }
                        else
                        {
                            *p = (byte)(gradients[y, x] / maxGradient * 255);
                        }
                    }
                }

                // STEP 4 - hysteresis
                start = this.Start + startX;
                byte* pUp;
                byte* pDown;
                for (int y = startY; y < stopY; y++)
                {
                    p = start + y * width;
                    pUp = p - width;
                    pDown = p + width;
                    for (int x = startX; x < stopX; x++, p++, pUp++, pDown++)
                    {
                        if (*p < highThreshold)
                        {
                            if (*p < lowThreshold)
                            {
                                // non edge
                                *p = 0;
                            }
                            else
                            {
                                // check 8 neighboring pixels
                                if ((p[-1] < highThreshold) &&
                                    (p[1] < highThreshold) &&
                                    (pUp[-1] < highThreshold) &&
                                    (pUp[0] < highThreshold) &&
                                    (pUp[1] < highThreshold) &&
                                    (pDown[-1] < highThreshold) &&
                                    (pDown[0] < highThreshold) &&
                                    (pDown[1] < highThreshold))
                                {
                                    *p = 0;
                                }
                            }
                        }
                    }
                }

                // STEP 4 将第1行，最后一行，第0列，最后1列

                // 第1行
                start = this.Start;
                byte* end = start + width;
                while (start != end)
                {
                    *start = 0;
                    start++;
                }

                // 最后一行
                start = this.Start + width * height - width;
                end = start + width;
                while (start != end)
                {
                    *start = 0;
                    start++;
                }

                // 第一列和最后一列
                start = this.Start;
                for (int y = 0; y < height; y++, start += width)
                {
                    start[0] = 0;
                    start[width - 1] = 0;
                }
            }
            return this;
        }

        /// <summary>
        /// 进行中值滤波。
        /// </summary>
        /// <param name="medianRadius">
        /// 中值滤波的核半径，不得小于1.
        /// </param>
        public unsafe void ApplyMedianFilter(int medianRadius)
        {
            if (medianRadius > 0)
            {
                // 进行中值滤波
                using (ImageU8 copy = this.Clone() as ImageU8)
                {
                    int size = medianRadius * 2 + 1;
                    int count = 0;
                    byte[] data = new byte[size * size];
                    int height = this.Height;
                    int width = this.Width;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            count = 0;
                            for (int h = -medianRadius; h <= medianRadius; h++)
                            {
                                for (int w = -medianRadius; w <= medianRadius; w++)
                                {
                                    int hh = y + h;
                                    int ww = x + w;
                                    if (hh >= 0 && hh < height && ww >= 0 && ww < width)
                                    {
                                        data[count] = copy[hh, ww];
                                        count++;
                                    }
                                }
                            }

                            Array.Sort(data, 0, count);
                            int m = count >> 1;
                            byte median = data[m];
                            this[y, x] = median;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="threshold">阈值。小于此阈值的设为 lowerValue，大于此阈值的设为 upperValue</param>
        /// <param name="lowerValue">lowerValue</param>
        /// <param name="upperValue">upperValue</param>
        public unsafe ImageU8 ApplyThreshold(byte threshold, byte lowerValue = 0, byte upperValue = 255)
        {
            byte* start = this.Start;
            byte* end = start + this.Length;
            while (start != end)
            {
                if (*start < threshold)
                    *start = lowerValue;
                else
                    *start = upperValue;

                start++;
            }
            return this;
        }

        /// <summary>
        /// 使用 Otsu 二值化方法进行二值化
        /// </summary>
        /// <param name="lowerValue">lowerValue</param>
        /// <param name="upperValue">upperValue</param>
        public unsafe ImageU8 ApplyOtsuThreshold(byte lowerValue = 0, byte upperValue = 255)
        {
            // from AForge.Net 
            int calculatedThreshold = 0;
            int[] integerHistogram = new int[256];
            float[] histogram = new float[256];
            byte* start = this.Start;
            byte* end = start + this.Length;
            while (start != end)
            {
                integerHistogram[*start]++;
                start++;
            }

            int pixelCount = this.Length;
            float imageMean = 0;

            for (int i = 0; i < 256; i++)
            {
                histogram[i] = (float)integerHistogram[i] / pixelCount;
                imageMean += histogram[i] * i;
            }

            float max = float.MinValue;

            // initial class probabilities
            float class1ProbabiltyInit = 0;
            float class2ProbabiltyInit = 1;

            // initial class 1 mean value
            float class1MeanInit = 0;

            // check all thresholds
            for (int t = 0; t < 256; t++)
            {
                // calculate class probabilities for the given threshold
                float class1Probability = class1ProbabiltyInit;
                float class2Probability = class2ProbabiltyInit;

                // calculate class means for the given threshold
                float class1Mean = class1MeanInit;
                float class2Mean = (imageMean - (class1Mean * class1Probability)) / class2Probability;

                // calculate between class variance
                float betweenClassVariance = (float)((class1Probability) * (1.0 - class1Probability) * Math.Pow(class1Mean - class2Mean, 2));

                // check if we found new threshold candidate
                if (betweenClassVariance > max)
                {
                    max = betweenClassVariance;
                    calculatedThreshold = t;
                }

                // update initial probabilities and mean value
                class1MeanInit *= class1ProbabiltyInit;

                class1ProbabiltyInit += histogram[t];
                class2ProbabiltyInit -= histogram[t];

                class1MeanInit += (float)t * (float)histogram[t];

                if (class1ProbabiltyInit != 0)
                    class1MeanInit /= class1ProbabiltyInit;
            }

            ApplyThreshold((byte)calculatedThreshold, lowerValue, upperValue);

            return this;
        }

        public unsafe ImageU8 ApplyLocalThreshold(int windowSize)
        {
            // 参照了 AForge.Net 里的 BradleyLocalThresholding 
            using (ImageInt32 imgIntegral = this.CreateIntegral())
            {
                int width = this.Width;
                int height = this.Height;
                Byte* p = this.Start;
                int radius = windowSize / 2;
                if (radius < 3) radius = 3;
                float avgBrightnessPart = 0.85f;
                for (int h = 0; h < height; h++)
                {
                    int y1 = h - radius - 1;
                    int y2 = h + radius;
                    if (y1 < 0) y1 = 0;
                    if (y2 >= height) y2 = height - 1;

                    for (int w = 0; w < width; w++, p++)
                    {
                        int x1 = w - radius - 1;
                        int x2 = w + radius;
                        if (x1 < 0) x1 = 0;
                        if (x2 >= width) x2 = width - 1;
                        int val = imgIntegral[y2, x2] + imgIntegral[y1, x1] - imgIntegral[y2, x1] - imgIntegral[y1, x2];
                        val = val / ((x2 - x1) * (y2 - y1));
                        if (*p < val * avgBrightnessPart) *p = 0;
                        else *p = 255;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// 进行距离变换。距离变换之前，请保证前景像素的值为0。计算D8距离. 忽略第一行和最后一行，第一列和最后一列。
        /// </summary>
        public unsafe ImageU8 ApplyDistanceTransformFast()
        {
            Byte* start = Start;
            int width = this.Width;
            int height = this.Height;

            Int32 val;

            // 从上向下，从左向右扫描
            for (int h = 1; h < height - 1; h++)
            {
                // 位于每行的头部
                Byte* line0 = start + (h - 1) * width;
                Byte* line1 = start + (h) * width;
                Byte* line2 = start + (h + 1) * width;
                for (int w = 1; w < width; w++)
                {
                    if (line1[1] > 0) // 当前像素
                    {
                        val = Math.Min(line0[0], line0[1]);
                        val = Math.Min(val, line1[0]);
                        val = Math.Min(val, line2[0]);
                        val = Math.Min(val + 1, line1[1]);
                        line1[1] = (byte)(Math.Min(val, 255));
                    }

                    line0++;
                    line1++;
                    line2++;
                }
            }

            // 从下向上，从右向左扫描
            for (int h = height - 2; h > 0; h--)
            {
                Byte* line0 = start + (h - 1) * width;
                Byte* line1 = start + (h) * width;
                Byte* line2 = start + (h + 1) * width;

                for (int w = width - 2; w >= 0; w--)
                {
                    if (line1[w] > 0)
                    {
                        val = Math.Min(line0[w + 1], line1[w + 1]);
                        val = Math.Min(val, line2[w + 1]);
                        val = Math.Min(val, line2[w]);
                        val = Math.Min(val + 1, line1[w]);
                        line1[w] = (byte)(Math.Min(val, 255)); ;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// 形态学腐蚀
        /// </summary>
        /// <returns>腐蚀后的当前图像</returns>
        public unsafe ImageU8 ApplyErosionFast()
        {
            ImageU8 cache = this.Clone() as ImageU8;

            Byte* start = cache.Start;
            Byte* dstStart = this.Start;
            int width = this.Width;
            int height = this.Height;

            Int32 val;

            // 从上向下，从左向右扫描
            for (int h = 1; h < height - 1; h++)
            {
                // 位于每行的头部
                Byte* line0 = start + (h - 1) * width;
                Byte* line1 = start + (h) * width;
                Byte* line2 = start + (h + 1) * width;
                Byte* line = dstStart + (h) * width;
                for (int w = 1; w < width - 1; w++)
                {
                    val = Math.Min(line0[w - 1], line0[w]);
                    val = Math.Min(val, line0[w + 1]);
                    val = Math.Min(val, line1[w - 1]);
                    val = Math.Min(val, line1[w]);
                    val = Math.Min(val, line1[w + 1]);
                    val = Math.Min(val, line2[w - 1]);
                    val = Math.Min(val, line2[w]);
                    val = Math.Min(val, line2[w + 1]);
                    line[w] = (byte)val;
                }
            }

            cache.Dispose();
            return this;
        }

        /// <summary>
        ///  形态学开运算
        /// </summary>
        public ImageU8 ApplyOpenFast(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                this.ApplyErosionFast();
                this.ApplyDilationFast();
            }
            return this;
        }

        /// <summary>
        ///  形态学闭运算
        /// </summary>
        public ImageU8 ApplyCloseFast(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                this.ApplyDilationFast();
                this.ApplyErosionFast();
            }
            return this;
        }

        /// <summary>
        /// 形态学膨胀
        /// </summary>
        /// <returns>膨胀后的当前图像</returns>
        public unsafe ImageU8 ApplyDilationFast()
        {
            ImageU8 cache = this.Clone() as ImageU8;

            Byte* start = cache.Start;
            Byte* dstStart = this.Start;
            int width = this.Width;
            int height = this.Height;

            Int32 val;

            // 从上向下，从左向右扫描
            for (int h = 1; h < height - 1; h++)
            {
                // 位于每行的头部
                Byte* line0 = start + (h - 1) * width;
                Byte* line1 = start + (h) * width;
                Byte* line2 = start + (h + 1) * width;
                Byte* line = dstStart + (h) * width;
                for (int w = 1; w < width - 1; w++)
                {
                    val = Math.Max(line0[w - 1], line0[w]);
                    val = Math.Max(val, line0[w + 1]);
                    val = Math.Max(val, line1[w - 1]);
                    val = Math.Max(val, line1[w]);
                    val = Math.Max(val, line1[w + 1]);
                    val = Math.Max(val, line2[w - 1]);
                    val = Math.Max(val, line2[w]);
                    val = Math.Max(val, line2[w + 1]);
                    line[w] = (byte)val;
                }
            }

            cache.Dispose();
            return this;
        }

        /// <summary>
        /// 使用数学形态学进行边缘平滑
        /// </summary>
        /// <param name="distance"></param>
        public unsafe void SmoothByMorphology(int distance0, int distance1)
        {
            // 距离变换
            this.ApplyDistanceTransformFast();

            Byte* start = Start;
            Byte* end = start + this.Length;
            while (start < end)
            {
                *start = (*start > distance0) ? (byte)0 : (byte)255;
                start++;
            }

            this.ApplyDistanceTransformFast();
            start = Start;
            while (start < end)
            {
                *start = (*start > distance1) ? (byte)0 : (byte)255;
                start++;
            }
        }

        /// <summary>
        /// 进行双边滤波。双边滤波（Bilateral filter）是一种非线性的滤波方法，是结合图像的空间邻近度和像素值相似度的一种折衷处理，同时考虑空域信息和灰度相似性，达到保边去噪的目的。具有简单、非迭代、局部的特点。
        /// 双边滤波器的好处是可以做边缘保存（edge preserving），一般过去用的维纳滤波或者高斯滤波去降噪，都会较明显地模糊边缘，对于高频细节的保护效果并不明显。双边滤波器顾名思义比高斯滤波多了一个高斯方差sigma－d，它是基于空间分布的高斯滤波函数，所以在边缘附近，离的较远的像素不会太多影响到边缘上的像素值，这样就保证了边缘附近像素值的保存。但是由于保存了过多的高频信息，对于彩色图像里的高频噪声，双边滤波器不能够干净的滤掉，只能够对于低频信息进行较好的滤波，
        /// </summary>
        /// <param name="sigmaDistance">距离加权的高斯方差</param>
        /// <param name="sigmaColor">颜色加权的高斯方差</param>
        /// <param name="radius">加权窗口的半径</param>
        public unsafe ImageU8 ApplyBilateralFilter(double sigmaDistance, double sigmaColor, int radius)
        {
            using (ImageU8 tmp = this.Clone())
            {
                int windowSize = radius * 2 + 1;
                double* distanceCoeff = stackalloc double[windowSize * windowSize]; //计算距离中间点的几何距离  
                double colorCoeff = 0;

                // 计算空间权重
                for (int y = 0; y < windowSize; y++)
                {
                    for (int x = 0; x < windowSize; x++)
                    {
                        int idx = y * windowSize + x;
                        distanceCoeff[idx] = (radius - x) * (radius - x) + (radius - y) * (radius - y);
                        distanceCoeff[idx] = Math.Exp(-0.5 * distanceCoeff[idx] / sigmaDistance / sigmaDistance); 
                    }
                }

                //以下求解灰度值的差  
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        double p = tmp[h, w];    //当前点的灰度值  
                        double val = 0.0;
                        double sum = 0.0;                      //用于进行归一化   

                        int x0 = Math.Max(0,w - radius);
                        int x1 = Math.Min(Width, w + radius);
                        int y0 = Math.Max(0, h - radius);
                        int y1 = Math.Min(Height, h + radius);

                        for (int y = y0; y < y1; y++)
                        {
                            for (int x = x0; x < x1; x++)
                            {
                                int idx = (y - h) * windowSize + (x - w);
                                double p0 = tmp[y, x];
                                colorCoeff = Math.Abs(p - p0);
                                colorCoeff = Math.Exp(-0.5 * colorCoeff * colorCoeff / sigmaColor / sigmaColor);
                                val += p0 * colorCoeff * distanceCoeff[idx];
                                sum += colorCoeff * distanceCoeff[idx];    
                            }
                        }

                        if(sum != 0) val = val / sum;
                        this[h, w] = (Byte)Math.Round(val);
                    }
                }
            }

            return this;
        }
    }
}
