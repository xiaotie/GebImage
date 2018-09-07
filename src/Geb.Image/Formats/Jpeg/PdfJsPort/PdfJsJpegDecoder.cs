// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace Geb.Image.Formats.Jpeg.PdfJsPort
{
    /// <summary>
    /// Image decoder for generating an image out of a jpg stream.
    /// </summary>
    internal sealed class PdfJsJpegDecoder : IJpegDecoderOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
        /// </summary>
        public bool IgnoreMetadata { get; set; }

        /// <inheritdoc/>
        public ImageBgra32 Decode(Configuration configuration, Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            using (var decoder = new PdfJsJpegDecoderCore(configuration, this))
            {
                //return decoder.Decode<TPixel>(stream);
                return null;
            }
        }

        /// <inheritdoc/>
        public IImageInfo Identify(Configuration configuration, Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            using (var decoder = new PdfJsJpegDecoderCore(configuration, this))
            {
                return decoder.Identify(stream);
            }
        }
    }
}