using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Introduce
{
    using Geb.Image;

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
