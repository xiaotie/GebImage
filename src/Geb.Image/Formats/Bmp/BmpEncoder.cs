using System.IO;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Image encoder for writing an image to a stream as a Windows bitmap.
    /// </summary>
    /// <remarks>The encoder can currently only write 24-bit rgb images to streams.</remarks>
    public sealed class BmpEncoder
    {
        /// <summary>
        /// Gets or sets the number of bits per pixel.
        /// </summary>
        public BmpBitsPerPixel BitsPerPixel { get; set; } = BmpBitsPerPixel.Pixel24;

        /// <inheritdoc/>
        public void Encode(ImageBgra32 image, Stream stream)
        {
            var encoder = new BmpEncoderCore(BmpBitsPerPixel.Pixel32);
            encoder.Encode(image, stream);
        }

        public void Encode(ImageBgra32 image, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs);
            }
        }
    }
}
