/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com
 ************************************************************************/

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public interface IImage
    {
        IImage Clone();
        Bitmap ToBitmap();
    }
}
