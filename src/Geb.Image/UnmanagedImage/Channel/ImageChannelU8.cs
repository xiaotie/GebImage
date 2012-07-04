/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Byte;
    using TValue = System.Int32;

    public partial class ImageChannelU8 : ImageChannel<Byte>
    {
        public ImageChannelU8(int width, int height, IntPtr start, int pixelSize)
            : base(width,height,start,pixelSize)
        {
        }

        public ImageChannelU8(int width, int height)
            : base(width,height)
        {
        }
    }
}

