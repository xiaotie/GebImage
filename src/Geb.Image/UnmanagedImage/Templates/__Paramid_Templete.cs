/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using TCache = System.Int32;
using TKernel = System.Int32;
using TImage = Geb.Image.ImageArgb32;
using TPixel = Geb.Image.Argb32;
using TChannel = System.Byte;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Hidden
{
    public partial class Image_Paramid_Argb_Templete : ImageArgb32
    {
        public Image_Paramid_Argb_Templete()
            :base(1,1)
        {
        }

        #region mixin

        public unsafe TImage GaussPyramidUp()
        {
            int width = this.Width;
            int height = this.Height;
            int ww = width / 2;
            int hh = height / 2;

            TImage imgUp = new TImage(ww, hh);
            TPixel* imgStart = Start;
            TPixel* imgPyUpStart = imgUp.Start;
            int hSrc, wSrc;
            TPixel* lineSrc;
            TPixel* lineDst;
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

                        TPixel* p = lineSrc + wSrc - 2 * width;

                        TPixel p00 = p[-2];
                        TPixel p01 = p[-1];
                        TPixel p02 = p[0];
                        TPixel p03 = p[1];
                        TPixel p04 = p[2];

                        p += width;
                        TPixel p10 = p[-2];
                        TPixel p11 = p[-1];
                        TPixel p12 = p[0];
                        TPixel p13 = p[1];
                        TPixel p14 = p[2];

                        p += width;
                        TPixel p20 = p[-2];
                        TPixel p21 = p[-1];
                        TPixel p22 = p[0];
                        TPixel p23 = p[1];
                        TPixel p24 = p[2];

                        p += width;
                        TPixel p30 = p[-2];
                        TPixel p31 = p[-1];
                        TPixel p32 = p[0];
                        TPixel p33 = p[1];
                        TPixel p34 = p[2];

                        p += width;
                        TPixel p40 = p[-2];
                        TPixel p41 = p[-1];
                        TPixel p42 = p[0];
                        TPixel p43 = p[1];
                        TPixel p44 = p[2];

                        //int alpha =
                        //      1 * p00.Alpha + 04 * p01.Alpha + 06 * p02.Alpha + 04 * p03.Alpha + 1 * p04.Alpha
                        //    + 4 * p10.Alpha + 16 * p11.Alpha + 24 * p12.Alpha + 16 * p13.Alpha + 4 * p14.Alpha
                        //    + 6 * p20.Alpha + 24 * p21.Alpha + 36 * p22.Alpha + 24 * p23.Alpha + 6 * p24.Alpha
                        //    + 4 * p30.Alpha + 16 * p31.Alpha + 24 * p32.Alpha + 16 * p33.Alpha + 4 * p34.Alpha
                        //    + 1 * p40.Alpha + 04 * p41.Alpha + 06 * p42.Alpha + 04 * p43.Alpha + 1 * p44.Alpha;

                        int red =
                              1 * p00.Red + 04 * p01.Red + 06 * p02.Red + 04 * p03.Red + 1 * p04.Red
                            + 4 * p10.Red + 16 * p11.Red + 24 * p12.Red + 16 * p13.Red + 4 * p14.Red
                            + 6 * p20.Red + 24 * p21.Red + 36 * p22.Red + 24 * p23.Red + 6 * p24.Red
                            + 4 * p30.Red + 16 * p31.Red + 24 * p32.Red + 16 * p33.Red + 4 * p34.Red
                            + 1 * p40.Red + 04 * p41.Red + 06 * p42.Red + 04 * p43.Red + 1 * p44.Red;

                        int green =
                              1 * p00.Green + 04 * p01.Green + 06 * p02.Green + 04 * p03.Green + 1 * p04.Green
                            + 4 * p10.Green + 16 * p11.Green + 24 * p12.Green + 16 * p13.Green + 4 * p14.Green
                            + 6 * p20.Green + 24 * p21.Green + 36 * p22.Green + 24 * p23.Green + 6 * p24.Green
                            + 4 * p30.Green + 16 * p31.Green + 24 * p32.Green + 16 * p33.Green + 4 * p34.Green
                            + 1 * p40.Green + 04 * p41.Green + 06 * p42.Green + 04 * p43.Green + 1 * p44.Green;

                        int blue =
                              1 * p00.Blue + 04 * p01.Blue + 06 * p02.Blue + 04 * p03.Blue + 1 * p04.Blue
                            + 4 * p10.Blue + 16 * p11.Blue + 24 * p12.Blue + 16 * p13.Blue + 4 * p14.Blue
                            + 6 * p20.Blue + 24 * p21.Blue + 36 * p22.Blue + 24 * p23.Blue + 6 * p24.Blue
                            + 4 * p30.Blue + 16 * p31.Blue + 24 * p32.Blue + 16 * p33.Blue + 4 * p34.Blue
                            + 1 * p40.Blue + 04 * p41.Blue + 06 * p42.Blue + 04 * p43.Blue + 1 * p44.Blue;

                        lineDst[w] = new TPixel(red >> 8, green >> 8, blue >> 8, 255);
                    }
                }
            }
            return imgUp;
        }

        public unsafe TImage GaussPyramidDown()
        {
            int width = Width;
            int height = Height;
            int ww = width * 2;
            int hh = height * 2;

            TImage imgDown = new TImage(ww, hh);
            TPixel* imgStart = this.Start;
            TPixel* imgPyDownStart = imgDown.Start;
            int hSrc, wSrc;
            TPixel* lineSrc;
            TPixel* lineDst;

            TPixel p0, p1, p2, p3;

            // 分四种情况进行处理：
            // (1) h,w 都是偶数；
            // (2) h 是偶数， w 是奇数
            // (3) h 是奇数， w 是偶数
            // (4) h 是奇数， w 是奇数

            // h 是偶数
            for (int h = 0; h < hh; h += 2)
            {
                hSrc = h / 2;
                lineDst = imgPyDownStart + h * ww;
                lineSrc = imgStart + hSrc * width;

                // w 是偶数
                for (int w = 0; w < ww; w += 2)
                {
                    wSrc = w / 2;
                    lineDst[w] = lineSrc[wSrc];
                }

                // w 是奇数
                for (int w = 1; w < ww; w += 2)
                {
                    // 防止取到最后一列
                    wSrc = Math.Min(w / 2,width-2);

                    p0 = lineSrc[wSrc];
                    p1 = lineSrc[wSrc + 1];
                    lineDst[w] = new TPixel((TChannel)((p0.Red + p1.Red) >> 1),
                        (TChannel)((p0.Green + p1.Green) >> 1),
                        (TChannel)((p0.Blue + p1.Blue) >> 1),
                        (TChannel)((p0.Alpha + p1.Alpha) >> 1));
                }
            }

            // h 是奇数
            for (int h = 1; h < hh; h += 2)
            {
                // 防止取到最后一行
                hSrc = Math.Min(h / 2, height - 2);

                lineDst = imgPyDownStart + h * ww;
                lineSrc = imgStart + hSrc * width;

                // w 是偶数
                for (int w = 0; w < ww; w += 2)
                {
                    wSrc = w / 2;
                    p0 = lineSrc[wSrc];
                    p1 = lineSrc[wSrc + width];
                    lineDst[w] = new TPixel((TChannel)((p0.Red + p1.Red) >> 1),
                        (TChannel)((p0.Green + p1.Green) >> 1),
                        (TChannel)((p0.Blue + p1.Blue) >> 1),
                        (TChannel)((p0.Alpha + p1.Alpha) >> 1));
                }

                // w 是奇数
                for (int w = 1; w < ww; w += 2)
                {
                    // 防止取到最后一列
                    wSrc = Math.Min(w / 2, width - 2);

                    p0 = lineSrc[wSrc];
                    p1 = lineSrc[wSrc + 1];
                    p2 = lineSrc[wSrc + width];
                    p3 = lineSrc[wSrc + width + 1];
                    lineDst[w] = new TPixel((TChannel)((p0.Red + p1.Red + p2.Red + p3.Red) >> 2),
                        (TChannel)((p0.Green + p1.Green + p2.Green + p3.Green) >> 2),
                        (TChannel)((p0.Blue + p1.Blue + p2.Blue + p3.Blue) >> 2),
                        (TChannel)((p0.Alpha + p1.Alpha + p2.Alpha + p3.Alpha) >> 2));
                }
            }

            return imgDown;
        }

        public unsafe TImage FastPyramidUp4X()
        {
            int width = this.Width;
            int height = this.Height;
            int ww = width / 4;
            int hh = height / 4;

            TImage imgUp = new TImage(ww, hh);
            TPixel* imgStart = Start;
            TPixel* imgPyUpStart = imgUp.Start;
            TPixel* lineSrc;
            TPixel* lineDst;
            for (int h = 0; h < hh; h++)
            {
                lineSrc = imgStart + 4 * h * width;
                lineDst = imgPyUpStart + h * ww;
                for (int w = 0; w < ww; w++)
                {
                    lineDst[w] = lineSrc[4 * w];
                }
            }
            return imgUp;
        }

        public unsafe TImage FastPyramidUp3X()
        {
            int width = this.Width;
            int height = this.Height;
            int ww = width / 3;
            int hh = height / 3;

            TImage imgUp = new TImage(ww, hh);
            TPixel* imgStart = Start;
            TPixel* imgPyUpStart = imgUp.Start;
            TPixel* lineSrc;
            TPixel* lineDst;
            for (int h = 0; h < hh; h++)
            {
                lineSrc = imgStart +  3 * h * width;
                lineDst = imgPyUpStart + h * ww;
                for (int w = 0; w < ww; w++)
                {
                    lineDst[w] = lineSrc[3 * w];
                }
            }
            return imgUp;
        }
        public unsafe TImage FastPyramidUp2X()
        {
            int width = this.Width;
            int height = this.Height;
            int ww = width / 2;
            int hh = height / 2;

            TImage imgUp = new TImage(ww, hh);
            TPixel* imgStart = Start;
            TPixel* imgPyUpStart = imgUp.Start;
            TPixel* lineSrc;
            TPixel* lineDst;
            for (int h = 0; h < hh; h++)
            {
                lineSrc = imgStart + 2 * h * width;
                lineDst = imgPyUpStart + h * ww;
                for (int w = 0; w < ww; w++)
                {
                    lineDst[w] = lineSrc[2 * w];
                }
            }
            return imgUp;
        }


        #endregion
    }
}
