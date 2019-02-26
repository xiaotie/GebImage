using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    /// <summary>
    /// Image decoder for generating an image out of a jpg stream.
    /// </summary>
    internal interface IJpegDecoderOptions
    {
        /// <summary>
        /// Gets a value indicating whether the metadata should be ignored when the image is being decoded.
        /// </summary>
        bool IgnoreMetadata { get; }
    }
}
