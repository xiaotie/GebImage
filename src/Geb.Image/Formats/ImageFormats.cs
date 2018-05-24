using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats
{
    /// <summary>
    /// The static collection of all the default image formats
    /// </summary>
    public static class ImageFormats
    {
        /// <summary>
        /// The format details for the jpegs.
        /// </summary>
        public static readonly IImageFormat Jpeg = new Jpeg.JpegFormat();

        /// <summary>
        /// The format details for the pngs.
        /// </summary>
        public static readonly IImageFormat Png = new Png.PngFormat();

        /// <summary>
        /// The format details for the gifs.
        /// </summary>
        public static readonly IImageFormat Gif = new Gif.GifFormat();

        /// <summary>
        /// The format details for the bitmaps.
        /// </summary>
        public static readonly IImageFormat Bmp = new Bmp.BmpFormat();
    }
}
