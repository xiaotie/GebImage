/*************************************************************************
 *  Copyright (c) 2015 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = GradXY;
    using TChannel = System.Int16;
    using TImage = Geb.Image.ImageGradXY;

    public static partial class ImageGradXYClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageGradXY
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageGradXY]
        #endregion
    }

    public partial struct GradXY
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}
