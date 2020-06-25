/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = SBgra64;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageSBgra64;
    using TChannel = System.Int16;

    public static partial class ImageSBgra64ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageSBgra64
    {
        #region include "__Image_Template.cs"  [Image_Template -> ImageSBgra64]
        #endregion

        #region include "__Paramid_Templete.cs"
        #endregion
    }

    public partial struct SBgra64
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}

