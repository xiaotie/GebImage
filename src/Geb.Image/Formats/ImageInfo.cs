using System;
using System.Collections.Generic;
using System.Text;

using Geb.Image.Formats.MetaData;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Contains information about the image including dimensions, pixel type information and additional metadata
    /// </summary>
    internal sealed class ImageInfo : IImageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class.
        /// </summary>
        /// <param name="pixelType">The image pixel type information.</param>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <param name="metaData">The images metadata.</param>
        public ImageInfo(PixelTypeInfo pixelType, int width, int height, ImageMetaData meta = null)
        {
            this.PixelType = pixelType;
            this.Width = width;
            this.Height = height;
            this.MetaData = meta;
        }

        /// <inheritdoc />
        public PixelTypeInfo PixelType { get; }

        /// <inheritdoc />
        public int Width { get; }

        /// <inheritdoc />
        public int Height { get; }

        public ImageMetaData MetaData { get; }
    }
}
