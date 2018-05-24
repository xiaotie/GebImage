using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Detects bmp file headers
    /// </summary>
    public sealed class BmpImageFormatDetector : IImageFormatDetector
    {
        /// <inheritdoc/>
        public int HeaderSize => 2;

        /// <inheritdoc/>
        public IImageFormat DetectFormat(ReadOnlySpan<byte> header)
        {
            if (this.IsSupportedFileFormat(header))
            {
                return ImageFormats.Bmp;
            }

            return null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            // TODO: This should be in constants
            return header.Length >= this.HeaderSize &&
                   header[0] == 0x42 && // B
                   header[1] == 0x4D;   // M
        }
    }
}
