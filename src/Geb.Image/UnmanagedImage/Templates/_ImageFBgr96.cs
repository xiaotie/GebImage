/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = FBgr96;
    using TChannel = System.Single;
    using TCache = System.Single;
    using TKernel = System.Single;
    using TImage = Geb.Image.ImageFBgr96;

    public static partial class ImageFBgr96ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageFBgr96
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageFBgr96]
        #endregion
    }

    public partial struct FBgr96
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}
