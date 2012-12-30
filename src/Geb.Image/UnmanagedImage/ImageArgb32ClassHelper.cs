/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using TPixel = Argb32;
    using TChannel = System.Byte;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageArgb32;

    public static partial class ImageArgb32ClassHelper
    {
        #region include "ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageArgb32
    {
        #region include "Image_Template.cs"
        #endregion

        #region include "Image_Paramid_Argb_Templete.cs"
        #endregion
    }

    public partial struct Argb32
    {
        #region include "TPixel_Template.cs"
        #endregion
    }
}

