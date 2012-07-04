/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Single;

    public partial class ImageChannelF : ImageChannel<Single>
    {
        public ImageChannelF(int width, int height, IntPtr start, int pixelSize)
            : base(width,height,start,pixelSize)
        {
        }

        public ImageChannelF(int width, int height)
            : base(width,height)
        {
        }
    }
}

