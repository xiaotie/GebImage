// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using System.Text;
using Geb.Image.Formats.Quantization;

namespace Geb.Image.Formats.Gif
{
    /// <summary>
    /// Image encoder for writing image data to a stream in gif format.
    /// </summary>
    public sealed class GifEncoder : IGifEncoderOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metadata should be ignored when the image is being encoded.
        /// </summary>
        public bool IgnoreMetadata { get; set; } = false;

        /// <summary>
        /// Gets or sets the encoding that should be used when writing comments.
        /// </summary>
        public Encoding TextEncoding { get; set; } = GifConstants.DefaultEncoding;

        /// <summary>
        /// Gets or sets the quantizer for reducing the color count.
        /// Defaults to the <see cref="OctreeQuantizer"/>
        /// </summary>
        public IQuantizer Quantizer { get; set; } /*= new OctreeQuantizer();*/

        /// <inheritdoc/>
        public void Encode(ImageBgra32 image, Stream stream)
        {
            //var encoder = new GifEncoderCore(image.GetConfiguration().MemoryManager, this);
            //encoder.Encode(image, stream);
        }
    }
}