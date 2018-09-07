/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = Geb.Image.Hsl;
    using TChannel = System.Single;
    using TCache = Geb.Image.Hsl;
    using TKernel = System.Single;
    using TImage = Geb.Image.ImageHsl;

    public partial struct Hsl
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }

    public static partial class ImageHslClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageHsl
    {
        #region include "__Image_Template.cs"  [Image_Template -> ImageHsl]
        #endregion
    }
}
