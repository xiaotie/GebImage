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
    using TPixel = System.Byte;
    using TChannel = System.Byte;
    using TCache = System.Int32;
    using TKernel = System.Int32;
    using TImage = Geb.Image.ImageU8;

    public static partial class ImageU8ClassHelper
    {
        #region include "__ImageClassHelper_Template.cs"
        #endregion
    }

    public partial class ImageU8
    {
        #region include "__Image_Template.cs" [Image_Template -> ImageU8]
        #endregion

        #region include "__ImageFilter_Template.cs"
        #endregion
    }
}



