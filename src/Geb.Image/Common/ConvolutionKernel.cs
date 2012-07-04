/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// 卷积核
    /// </summary>
    public class ConvolutionKernel
    {
        /// <summary>
        /// 图像平滑卷积核01：
        /// 1,1,1
        /// 1,1,1   × (1/9)
        /// 1,1,1
        /// </summary>
        public static readonly ConvolutionKernel Average01 = new ConvolutionKernel( 
            new int[,]
            {
                {1,1,1},
                {1,1,1},
                {1,1,1}
            },9);

        /// <summary>
        /// 图像平滑卷积核02：
        /// 1,1,1
        /// 1,2,1   × (1/10)
        /// 1,1,1
        /// </summary>
        public static readonly ConvolutionKernel Average02 = new ConvolutionKernel(
            new int[,]
            {
                {1,1,1},
                {1,2,1},
                {1,1,1}
            },10);

        /// <summary>
        /// 图像平滑卷积核03：
        /// 1,2,1
        /// 2,4,2   × (1/16)
        /// 1,2,1
        /// </summary>
        public static readonly ConvolutionKernel Average03 = new ConvolutionKernel(
    new int[,]
            {
                {1,2,1},
                {2,4,2},
                {1,2,1}
            }, 16);

        /// <summary>
        /// 根据 4-邻域 计算的 Laplace 算子
        /// 0,1,0
        /// 1,-4,1
        /// 0,1,0
        /// </summary>
        public static readonly ConvolutionKernel Laplace01 = new ConvolutionKernel(
    new int[,]
            {
                {0,1,0},
                {1,-4,1},
                {0,1,0}
            });

        /// <summary>
        /// 根据 8-邻域 计算的 Laplace 算子
        /// 1,1, 1
        /// 1,-8,1 * 1/3
        /// 1,1, 1
        /// </summary>
        public static readonly ConvolutionKernel Laplace02 = new ConvolutionKernel(
    new int[,]
            {
                {1,1, 1},
                {1,-8,1},
                {1,1, 1}
            },3);

        public static readonly ConvolutionKernel SobelX = new ConvolutionKernel(
    new int[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            });

        public static readonly ConvolutionKernel SobelY = new ConvolutionKernel(
            new int[,]
            {
                {-1, -2, -1},
                {0, 0, 0},
                {1, 2, 1}
            });

        public static unsafe ConvolutionKernel CreateGaussianKernel(double sigma = 1.4, int size = 5)
        {
            sigma = Math.Max(0.5, Math.Min(5.0, sigma));
            size = Math.Max(3, Math.Min(21, size | 1));
            double sqrSigma = sigma * sigma;
            double* kernel = stackalloc double[size*size];
            int r = size / 2;

            for (int y = -r, i = 0; i < size; y++, i++)
            {
                for (int x = -r, j = 0; j < size; x++, j++)
                {
                    kernel[i + j * size] = Math.Exp((x * x + y * y) / (-2 * sqrSigma)) / (2 * Math.PI * sqrSigma);
                }
            }
            double min = kernel[0];
            int scale = 0;
            int[,] intKernel = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    double v = kernel[i + j*size] / min;
                    if (v > ushort.MaxValue)
                    {
                        v = ushort.MaxValue;
                    }
                    intKernel[i, j] = (int)v;
                    scale += intKernel[i, j];
                }
            }
            return new ConvolutionKernel(intKernel,scale);
        }



        /// <summary>
        /// int[height, width]
        /// </summary>
        public int[,] Kernel { get; private set; }
        public int Scale { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        public ConvolutionKernel(int[,] kernel):this(kernel,1)
        {
        }

        /// <summary>
        /// 用二维数组表示的ConvolutionKernel。
        /// </summary>
        /// <param name="kernel">kernel</param>
        /// <param name="divisor">divisor</param>
        public ConvolutionKernel(int[,] kernel, int scale)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            Height = kernel.GetUpperBound(0) + 1;
            Width = kernel.GetUpperBound(1) + 1;
            if (IsEvenNumber(Height) == true) throw new ArgumentException("Height must be odd number.");
            if (IsEvenNumber(Width) == true) throw new ArgumentException("Width must be odd number.");
            if (scale < 1) throw new ArgumentException("Scale must >= 1");
            Scale = scale;
            Kernel = kernel;
        }

        // 判断是否是偶数
        private Boolean IsEvenNumber(int number)
        {
            return number % 2 == 0;
        }
    }
}
