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
    using TPixel = Bgra32;
    using TChannel = System.Byte;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageBgra32;

    public static partial class ImageBgra32ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageBgra32
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageBgra32]
        #endregion

        #region include "__Paramid_Templete.cs"
        #endregion
    }

    public partial struct Bgra32
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}

