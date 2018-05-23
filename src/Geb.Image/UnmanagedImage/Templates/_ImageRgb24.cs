/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = Rgb24;
    using TChannel = System.Byte;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageRgb24;

    public static partial class ImageRgb24ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageRgb24
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageRgb24]
        #endregion
    }

    public partial struct Rgb24
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}
