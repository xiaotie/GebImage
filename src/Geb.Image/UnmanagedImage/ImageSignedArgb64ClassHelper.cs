/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using TPixel = SignedArgb64;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageSignedArgb64;
    using TChannel = System.Int16;

    public static partial class ImageSignedArgb64ClassHelper
    {
        #region include "ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageSignedArgb64
    {
        #region include "Image_Template.cs"
        #endregion

        #region include "Image_Paramid_Argb_Templete.cs"
        #endregion
    }

    public partial struct SignedArgb64
    {
        #region include "TPixel_Template.cs"
        #endregion
    }
}

