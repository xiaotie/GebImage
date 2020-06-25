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
    using TPixel = RgbS;
    using TCache = System.Single;
    using TKernel = System.Single;
    using TImage = Geb.Image.ImageRgbS;
    using TChannel = System.Single;

    public static partial class ImageRgbSClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageRgbS
    {
        #region include "__Image_Template.cs"  [Image_Template -> ImageRgbS]
        #endregion
    }

    public partial struct RgbS
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}

