/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = Argb32;
    using TChannel = System.Byte;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageArgb32;

    public static partial class ImageArgb32ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageArgb32
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageArgb32]
        #endregion

        #region include "__Paramid_Templete.cs"
        #endregion
    }

    public partial struct Argb32
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}

