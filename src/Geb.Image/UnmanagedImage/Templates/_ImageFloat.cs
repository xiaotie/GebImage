/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = System.Single;
    using TChannel = System.Single;
    using TChannelTemp = System.Single;
    using TCache = System.Single;
    using TKernel = System.Single;
    using TImage = Geb.Image.ImageFloat;

    public static partial class ImageFloatClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageFloat
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageFloat]
        #endregion

        #region include "__ImageFilter_Template.cs"
        #endregion
    }
}



