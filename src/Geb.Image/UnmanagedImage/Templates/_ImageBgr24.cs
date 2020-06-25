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
    using TPixel = Bgr24;
    using TChannel = System.Byte;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageBgr24;

    public static partial class ImageBgr24ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageBgr24
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageBgr24]
        #endregion
    }

    public partial struct Bgr24
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}
