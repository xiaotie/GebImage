using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public interface IImage
    {
        int Width { get; }
        int Height { get; }
        int BytesPerPixel { get; }
    }
}
