using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Image decoder for generating an image out of a Windows bitmap stream.
    /// </summary>
    /// <remarks>
    /// Does not support the following formats at the moment:
    /// <list type="bullet">
    ///    <item>JPG</item>
    ///    <item>PNG</item>
    ///    <item>RLE4</item>
    ///    <item>RLE8</item>
    ///    <item>BitFields</item>
    /// </list>
    /// Formats will be supported in a later releases. We advise always
    /// to use only 24 Bit Windows bitmaps.
    /// </remarks>
    public sealed class BmpDecoder : IImageDecoder
    {
        /// <inheritdoc/>
        public ImageBgra32 Decode(Stream stream)
        {
            return new BmpDecoderCore().Decode(stream);
        }

        public ImageBgra32 Decode(String path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return new BmpDecoderCore().Decode(fs);
            }
        }

        /// <inheritdoc/>
        public IImageInfo Identify(Stream stream)
        {
            return new BmpDecoderCore().Identify(stream);
        }
    }
}
