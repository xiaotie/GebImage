/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Int32;
    using TChannelTemp = System.Int32;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageInt32;

    public static partial class ImageInt32ClassHelper
    {
        #region include "ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageInt32
    {
        #region include "Image_Template.cs"
        #endregion

        #region include "ImageFilter_Template.cs"
        #endregion
    }
}



