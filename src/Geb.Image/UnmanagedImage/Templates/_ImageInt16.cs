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
    using TPixel = System.Int16;
    using TChannel = System.Int16;
    using TChannelTemp = System.Int16;
    using TCache = System.Int32;
    using TKernel = System.Int16;
    using TImage = Geb.Image.ImageInt16;

    public static partial class ImageInt16ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageInt16
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageInt16]
        #endregion

        #region include "__ImageFilter_Template.cs"
        #endregion
    }
}



