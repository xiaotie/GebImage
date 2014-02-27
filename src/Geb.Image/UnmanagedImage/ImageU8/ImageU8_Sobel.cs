using System;
using System.Collections.Generic;
using System.Text;

using TPixel = System.Byte;
using TImage = Geb.Image.ImageU8;
using TCache = System.Int32;

namespace Geb.Image
{
    public partial class ImageU8
    {
        public unsafe ImageU8 ApplySobelX()
        {
            int width = this.Width;
            int height = this.Height;
            TPixel* start = this.Start;

            TImage imgPadding = CreatePaddingImage(1);

            TPixel* l0 = imgPadding.Start;
            TPixel* l1 = l0 + imgPadding.Width;
            TPixel* l2 = l1 + imgPadding.Width;
            TPixel* l = start;
            TPixel* p0 = l0;
            TPixel* p1 = l1;
            TPixel* p2 = l2;
            TPixel* p = l;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++,p0++,p1++,p2++,p++)
                {
                    TCache val = -(*p0)-2*(*p1)-(*p2) + p0[2] + 2 * p1[2] + p2[2];
                    val = Math.Abs(val);
                    val = Math.Min(val, 255);
                    *p = (Byte)val;
                }

                l0 += imgPadding.Width;
                l1 += imgPadding.Width;
                l2 += imgPadding.Width;
                l += width;
                p0 = l0;
                p1 = l1;
                p2 = l2;
                p = l;
            }
  
            imgPadding.Dispose();
            return this;
        }

        public unsafe ImageU8 ApplySobelY()
        {
            int width = this.Width;
            int height = this.Height;
            TPixel* start = this.Start;

            TImage imgPadding = CreatePaddingImage(1);
            TPixel* l0 = imgPadding.Start;
            TPixel* l1 = l0 + imgPadding.Width;
            TPixel* l2 = l1 + imgPadding.Width;
            TPixel* l = start;
            TPixel* p0 = l0;
            TPixel* p1 = l1;
            TPixel* p2 = l2;
            TPixel* p = l;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++, p0++, p1++, p2++, p++)
                {
                    TCache val = -(p2[0]) - 2 * (p2[1]) - (p2[2]) + p0[0] + 2 * p0[1] + p0[2];
                    val = Math.Abs(val);
                    val = Math.Min(val, 255);
                    *p = (Byte)val;
                }

                l0 += imgPadding.Width;
                l1 += imgPadding.Width;
                l2 += imgPadding.Width;
                l += width;
                p0 = l0;
                p1 = l1;
                p2 = l2;
                p = l;
            }

            imgPadding.Dispose();
            return this;
        }
    }
}
