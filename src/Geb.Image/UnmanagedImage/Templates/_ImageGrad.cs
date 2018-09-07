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
    using TPixel = Grad;
    using TChannel = System.Single;
    using TImage = Geb.Image.ImageGrad;

    public static partial class ImageGradClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageGrad
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageGrad]
        #endregion
    }

    public partial struct Grad
    {
        #region include "__Pixel_Template.cs"
        #endregion
    }
}
