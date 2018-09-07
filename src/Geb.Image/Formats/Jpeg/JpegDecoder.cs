using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    using Geb.Image.Formats.Jpeg.PdfJsPort;

    /// <summary>
    /// Image decoder for generating an image out of a jpg stream.
    /// </summary>
    public sealed class JpegDecoder : IJpegDecoderOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
        /// </summary>
        public bool IgnoreMetadata { get; set; }

        /// <inheritdoc/>
        public ImageBgra32 Decode(Stream stream)
        {
            using (var decoder = new PdfJsJpegDecoderCore(null, this))
            {
                return decoder.Decode(stream);
            }
        }

        public ImageBgra32 Decode(String filePath)
        {
            using (var decoder = new PdfJsJpegDecoderCore(null, this))
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return decoder.Decode(fs);
            }
        }

        /// <inheritdoc/>
        public IImageInfo Identify(Stream stream)
        {
            using (var decoder = new PdfJsJpegDecoderCore(null, this))
            {
                return decoder.Identify(stream);
            }
        }
    }
}
