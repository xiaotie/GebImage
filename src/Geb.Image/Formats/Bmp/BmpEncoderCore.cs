using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Image encoder for writing an image to a stream as a Windows bitmap.
    /// </summary>
    internal unsafe sealed class BmpEncoderCore
    {
        /// <summary>
        /// The amount to pad each row by.
        /// </summary>
        private int padding;

        private readonly BmpBitsPerPixel bitsPerPixel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BmpEncoderCore"/> class.
        /// </summary>
        /// <param name="options">The encoder options</param>
        /// <param name="memoryManager">The memory manager</param>
        public BmpEncoderCore(BmpBitsPerPixel bitsPerPixel)
        {
            this.bitsPerPixel = bitsPerPixel;
        }

        /// <summary>
        /// Encodes the image to the specified stream from the <see cref="ImageFrame{TPixel}"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="image">The <see cref="ImageFrame{TPixel}"/> to encode from.</param>
        /// <param name="stream">The <see cref="Stream"/> to encode the image data to.</param>
        public void Encode(ImageBgra32 image, Stream stream)
        {
            // Cast to int will get the bytes per pixel
            short bpp = (short)(8 * (int)this.bitsPerPixel);
            int bytesPerLine = 4 * (((image.Width * bpp) + 31) / 32);
            this.padding = bytesPerLine - (image.Width * (int)this.bitsPerPixel);

            var infoHeader = new BmpInfoHeader(
                headerSize: BmpInfoHeader.Size,
                height: image.Height,
                width: image.Width,
                bitsPerPixel: bpp,
                planes: 1,
                imageSize: image.Height * bytesPerLine,
                clrUsed: 0,
                clrImportant: 0);

            var fileHeader = new BmpFileHeader(
                type: 19778, // BM
                offset: 54,
                reserved: 0,
                fileSize: 54 + infoHeader.ImageSize);

            byte[] buffer = new byte[40]; // TODO: stackalloc

            fileHeader.WriteTo(buffer);

            stream.Write(buffer, 0, BmpFileHeader.Size);

            infoHeader.WriteTo(buffer);

            stream.Write(buffer, 0, 40);

            this.WriteImage(stream, image);

            stream.Flush();
        }

        private Byte[] AllocatePaddedPixelRowBuffer(int width, int bytesPerPixel, int padding)
        {
            return new byte[width * bytesPerPixel + padding];
        }

        private Byte[] AllocateRow(int width, int bytesPerPixel)
        {
            return AllocatePaddedPixelRowBuffer(width, bytesPerPixel, this.padding);
        }

        /// <summary>
        /// Writes the 32bit color palette to the stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> containing pixel data.</param>
        private void WriteImage(Stream stream, ImageBgra32 pixels)
        {
            Byte[] row = this.AllocateRow(pixels.Width, 4);
            fixed(Byte* pRow = row)
            for (int y = pixels.Height - 1; y >= 0; y--)
            {
                Bgra32* pSrc = pixels.Start + y * pixels.Width;
                Unsafe.CopyBlock((void*)pRow, (void*)pSrc, (uint)pixels.Width * 4);
                stream.Write(row, 0, row.Length);
            }
        }
    }
}
