/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = RgbSingle96;
    using TChannel = System.Single;
    using TCache = System.Single;
    using TKernel = System.Single;
    using TImage = Geb.Image.ImageRgbSingle96;

    public static partial class ImageRgbSingle96ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageRgbSingle96
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageRgbSingle96]
        #endregion
    }

    public partial struct RgbSingle96
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}
