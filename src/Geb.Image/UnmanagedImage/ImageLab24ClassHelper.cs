/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    using TPixel = Geb.Image.Lab24;
    using TCache = Geb.Image.Lab24;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageLab24;

    public partial struct Lab24
    {
        #region include "TPixel_Template.cs"
        #endregion
    }

    public static partial class ImageLab24ClassHelper
    {
        #region include "ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageLab24
    {
        #region include "Image_Template.cs"
        #endregion
    }
}
