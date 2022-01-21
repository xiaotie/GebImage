using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Geb.Image
{
    public interface IImage
    {
        int Width { get; }
        int Height { get; }
        int BytesPerPixel { get; }
        Bitmap ToBitmap();
    }
}
