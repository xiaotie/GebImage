using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    using Geb.Utils;
    using Geb.Utils.WinForm;

    public static class ImageHelper
    {
        public static Bitmap ShowDialog(this Bitmap bmp, String title = null)
        {
            ImageBox.ShowDialog(bmp, title);
            return bmp;
        }

        public static T ShowDialog<T>(this T img, String title = null) 
            where T:IImage
        {
            img.ToBitmap().ShowDialog(title);
            return img;
        }
    }
}
