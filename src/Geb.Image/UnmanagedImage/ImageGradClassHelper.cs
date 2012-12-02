/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using TPixel = Grad;
    using TImage = Geb.Image.ImageGrad;

    public static partial class ImageGradClassHelper
    {
        #region include "ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageGrad
    {
        #region include "Image_Template.cs"
        #endregion
    }

    public partial struct Grad
    {
        #region include "TPixel_Template.cs"
        #endregion
    }
}
