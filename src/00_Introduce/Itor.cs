using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Introduce
{
    using Geb.Image;

    public unsafe struct ItArgb32Old
    {
        public unsafe Argb32* Current;
        public unsafe Argb32* End;

        public unsafe Argb32* Next()
        {
            if (Current < End) return Current ++;
            else return null;
        }
    }

    public static class ImageArgb32Helper
    {
        public unsafe static ItArgb32Old CreateItorOld(this ImageArgb32 img)
        {
            ItArgb32Old itor = new ItArgb32Old();
            itor.Current = img.Start;
            itor.End = img.Start + img.Length;
            return itor;
        }

        public unsafe static ItArgb32 CreateItor(this ImageArgb32 img)
        {
            ItArgb32 itor = new ItArgb32();
            itor.Start = img.Start;
            itor.End = img.Start + img.Length;
            return itor;
        }

        public unsafe static ItRoiArgb32 CreateRoiItor(this ImageArgb32 img,
            int x, int y, int roiWidth, int roiHeight)
        {
            ItRoiArgb32 itor = new ItRoiArgb32();
            itor.Width = img.Width;
            itor.RoiWidth = roiWidth;
            itor.Start = img.Start + img.Width * y + x;
            itor.End = itor.Start + img.Width * roiHeight;
            return itor;
        }
    }

    public unsafe struct ItArgb32
    {
        public unsafe Argb32* Start;
        public unsafe Argb32* End;

        public int Step(Argb32* ptr)
        {
            return 1;
        }
    }

    public unsafe struct ItRoiArgb32
    {
        public unsafe Argb32* Start;
        public unsafe Argb32* End;
        public int Width;
        public int RoiWidth;

        public int Step(Argb32* ptr)
        {
            return Width;
        }

        public ItArgb32 Itor(Argb32* p)
        {
            ItArgb32 it = new ItArgb32();
            it.Start = p;
            it.End = p + RoiWidth;
            return it;
        }
    }
}
