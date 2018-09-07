/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    using TPixel = System.Int32;
    using TChannel = System.Int32;
    using TChannelTemp = System.Int32;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageInt32;

    public static partial class ImageInt32ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageInt32
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageInt32]
        #endregion

        #region include "__ImageFilter_Template.cs"
        #endregion
    }
}



