using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Encapsulates properties that descibe basic image information including dimensions, pixel type information
    /// and additional metadata
    /// </summary>
    public interface IImageInfo
    {
        /// <summary>
        /// Gets information about the image pixels.
        /// </summary>
        PixelTypeInfo PixelType { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        int Height { get; }
    }
}
