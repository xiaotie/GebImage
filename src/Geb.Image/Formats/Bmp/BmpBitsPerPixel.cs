using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Enumerates the available bits per pixel for bitmap.
    /// </summary>
    public enum BmpBitsPerPixel
    {
        /// <summary>
        /// 24 bits per pixel. Each pixel consists of 3 bytes.
        /// </summary>
        Pixel24 = 3,

        /// <summary>
        /// 32 bits per pixel. Each pixel consists of 4 bytes.
        /// </summary>
        Pixel32 = 4
    }
}
