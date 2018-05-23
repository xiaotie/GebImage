/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = Geb.Image.Lab24;
    using TChannel = System.Byte;
    using TCache = Geb.Image.Lab24;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageLab24;

    public partial struct Lab24
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }

    public static partial class ImageLab24ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageLab24
    {
        #region include "__Image_Template.cs"  [Image_Template -> ImageLab24]
        #endregion
    }
}
