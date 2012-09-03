/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    using TPixel = System.Single;
    using TChannelTemp = System.Single;
    using TCache = System.Single;
    using TKernel = System.Single;
    using TImage = Geb.Image.ImageFloat;

    public static partial class ImageFloatClassHelper
    {
        #region include "ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageFloat
    {
        #region include "Image_Template.cs"
        #endregion

        #region include "ImageFilter_Template.cs"
        #endregion
    }
}



