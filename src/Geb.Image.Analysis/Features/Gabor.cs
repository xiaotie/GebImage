using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Analysis
{
    public class Gabor
    {
        protected double Sigma;
        protected double F;
        protected double K;
        protected double Phi;
        protected int Width;
        protected ConvolutionKernel RealKernel;
        protected ConvolutionKernel ImKernel;

        /// <summary>
        /// 创建 Gabor 滤波器
        /// </summary>
        /// <param name="mu">方向 mu * PI/8 </param>
        /// <param name="nu">scale值</param>
        /// <param name="sigma">sigma值</param>
        public Gabor(int mu, int nu, double sigma)
            :this(mu,nu,sigma,Math.Sqrt(2.0))
        {
        }

        public ImageU8 GetRealKernelImage()
        {
            ImageU8 img = new ImageU8(Width, Width);
            double max = 0;
            double scale = RealKernel.Scale;

            for (int h = 0; h < Width; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    double val = RealKernel.Kernel[h, w]/scale;
                    if (max < Math.Abs(val)) max = Math.Abs(val);
                }
            }

            for (int h = 0; h < Width; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    double val = RealKernel.Kernel[h, w] / scale;
                    val = 127 * val / max;
                    byte nVal = (Byte)(128 + val);
                    img[h, w] = nVal;
                }
            }

            return img;
        }

        /// <summary>
        /// 创建 Gabor 滤波器
        /// </summary>
        /// <param name="mu">方向 mu * PI/8 </param>
        /// <param name="nu">scale值</param>
        /// <param name="sigma">sigma值</param>
        /// <param name="f">空间频率</param>
        public Gabor(int mu, int nu, double sigma, double f)
        {
            Sigma = sigma;
            F = f;

            double kmax = Math.PI / 2;
            K = kmax / Math.Pow(F, (double)nu);
            Phi = Math.PI * mu / 8;

            int width = (int)Math.Round(Sigma / K * 6 + 1);
            if (width % 2 == 0) width++; // 确保 width 是奇数

            Width = width;
            InitKernel(); 
        }

        private void InitKernel()
        {
            int x, y;
            double real;
            double im;
            double v1, v2, v3;
            int[,] realKernel = new int[Width, Width];
            int[,] imKernel = new int[Width, Width];
            const int scale = 1 << 16;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    x = i - (Width - 1) / 2;
                    y = j - (Width - 1) / 2;
                    v1 = (Math.Pow(K, 2) / Math.Pow(Sigma, 2)) * Math.Exp(-(Math.Pow((double)x, 2) + Math.Pow((double)y, 2)) * Math.Pow(K, 2) / (2 * Math.Pow(Sigma, 2)));
                    v2 = Math.Cos(K * Math.Cos(Phi) * x + K * Math.Sin(Phi) * y) - Math.Exp(-(Math.Pow(Sigma, 2) / 2));
                    v3 = Math.Sin(K * Math.Cos(Phi) * x + K * Math.Sin(Phi) * y);
                    real = v1 * v2;
                    im = v1 * v3;
                    realKernel[i, j] = (int)(real * scale);
                    imKernel[i, j] = (int)(im * scale);
                }
            }
            RealKernel = new ConvolutionKernel(realKernel, scale);
            ImKernel = new ConvolutionKernel(imKernel, scale);
        }

        public ImageInt32 ApplyReal(ImageU8 img)
        {
            return img.ToImageInt32().ApplyConvolution(RealKernel);
        }
    }
}
