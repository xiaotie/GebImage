using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Defines constants relating to BMPs
    /// </summary>
    internal static class BmpConstants
    {
        /// <summary>
        /// The list of mimetypes that equate to a bmp.
        /// </summary>
        public static readonly IEnumerable<string> MimeTypes = new[] { "image/bmp", "image/x-windows-bmp" };

        /// <summary>
        /// The list of file extensions that equate to a bmp.
        /// </summary>
        public static readonly IEnumerable<string> FileExtensions = new[] { "bm", "bmp", "dip" };
    }
}
