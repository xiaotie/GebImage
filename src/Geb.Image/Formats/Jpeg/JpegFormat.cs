using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the jpeg format.
    /// </summary>
    internal sealed class JpegFormat : IImageFormat
    {
        /// <inheritdoc/>
        public string Name => "JPEG";

        /// <inheritdoc/>
        public string DefaultMimeType => "image/jpeg";

        /// <inheritdoc/>
        public IEnumerable<string> MimeTypes => JpegConstants.MimeTypes;

        /// <inheritdoc/>
        public IEnumerable<string> FileExtensions => JpegConstants.FileExtensions;
    }
}
