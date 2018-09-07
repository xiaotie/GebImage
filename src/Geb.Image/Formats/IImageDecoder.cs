using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Encapsulates properties and methods required for decoding an image from a stream.
    /// </summary>
    public unsafe interface IImageDecoder
    {
        /// <summary>
        /// Decodes the image from the specified stream to the <see cref="ImageFrame{TPixel}"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="configuration">The configuration for the image.</param>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        /// <returns>The decoded image</returns>
        ImageBgra32 Decode(Stream stream);
    }
}
