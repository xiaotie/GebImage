/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Int32;
    using TTempValue = System.Int32;

    public partial class ImageChannel32 : ImageChannel<Int32>
    {
        public ImageChannel32(int width, int height, IntPtr start, int pixelSize)
            : base(width,height,start,pixelSize)
        {
        }

        public ImageChannel32(int width, int height)
            : base(width,height)
        {
        }
    }
}

