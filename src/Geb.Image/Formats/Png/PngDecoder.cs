// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Text;

namespace Geb.Image.Formats.Png
{
    /// <summary>
    /// Encoder for generating an image out of a png encoded stream.
    /// </summary>
    /// <remarks>
    /// At the moment the following features are supported:
    /// <para>
    /// <b>Filters:</b> all filters are supported.
    /// </para>
    /// <para>
    /// <b>Pixel formats:</b>
    /// <list type="bullet">
    ///     <item>RGBA (True color) with alpha (8 bit).</item>
    ///     <item>RGB (True color) without alpha (8 bit).</item>
    ///     <item>grayscale with alpha (8 bit).</item>
    ///     <item>grayscale without alpha (8 bit).</item>
    ///     <item>Palette Index with alpha (8 bit).</item>
    ///     <item>Palette Index without alpha (8 bit).</item>
    /// </list>
    /// </para>
    /// </remarks>
    public sealed class PngDecoder : IPngDecoderOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
        /// </summary>
        public bool IgnoreMetadata { get; set; }

        /// <summary>
        /// Gets or sets the encoding that should be used when reading text chunks.
        /// </summary>
        public Encoding TextEncoding { get; set; } = PngConstants.DefaultEncoding;

        /// <summary>
        /// Decodes the image from the specified stream to the <see cref="ImageFrame{TPixel}"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="configuration">The configuration for the image.</param>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        /// <returns>The decoded image.</returns>
        public ImageBgra32 Decode(Configuration configuration, Stream stream)
        {
            var decoder = new PngDecoderCore(configuration, this);
            return decoder.Decode(stream);
        }

        public ImageBgra32 Decode(Stream stream)
        {
            var decoder = new PngDecoderCore(null, this);
            return decoder.Decode(stream);
        }

        public ImageBgra32 Decode(String filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return Decode(null, fs);
            }
        }

        /// <inheritdoc/>
        public IImageInfo Identify(Configuration configuration, Stream stream)
        {
            var decoder = new PngDecoderCore(configuration, this);
            return decoder.Identify(stream);
        }
    }
}