using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

namespace Geb.Image.Formats.Bmp
{
    /// <summary>
    /// Performs the bmp decoding operation.
    /// </summary>
    /// <remarks>
    /// A useful decoding source example can be found at <see href="https://dxr.mozilla.org/mozilla-central/source/image/decoders/nsBMPDecoder.cpp"/>
    /// </remarks>
    internal unsafe sealed class BmpDecoderCore
    {
        /// <summary>
        /// The mask for the red part of the color for 16 bit rgb bitmaps.
        /// </summary>
        private const int Rgb16RMask = 0x7C00;

        /// <summary>
        /// The mask for the green part of the color for 16 bit rgb bitmaps.
        /// </summary>
        private const int Rgb16GMask = 0x3E0;

        /// <summary>
        /// The mask for the blue part of the color for 16 bit rgb bitmaps.
        /// </summary>
        private const int Rgb16BMask = 0x1F;

        /// <summary>
        /// RLE8 flag value that indicates following byte has special meaning.
        /// </summary>
        private const int RleCommand = 0x00;

        /// <summary>
        /// RLE8 flag value marking end of a scan line.
        /// </summary>
        private const int RleEndOfLine = 0x00;

        /// <summary>
        /// RLE8 flag value marking end of bitmap data.
        /// </summary>
        private const int RleEndOfBitmap = 0x01;

        /// <summary>
        /// RLE8 flag value marking the start of [x,y] offset instruction.
        /// </summary>
        private const int RleDelta = 0x02;

        /// <summary>
        /// The stream to decode from.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The file header containing general information.
        /// TODO: Why is this not used? We advance the stream but do not use the values parsed.
        /// </summary>
        private BmpFileHeader fileHeader;

        /// <summary>
        /// The info header containing detailed information about the bitmap.
        /// </summary>
        private BmpInfoHeader infoHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="BmpDecoderCore"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options</param>
        public BmpDecoderCore()
        {
        }

        /// <summary>
        /// Decodes the image from the specified this._stream and sets
        /// the data to image.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="stream">The stream, where the image should be
        /// decoded from. Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="System.ArgumentNullException">
        ///    <para><paramref name="stream"/> is null.</para>
        /// </exception>
        /// <returns>The decoded image.</returns>
        public ImageBgra32 Decode(Stream stream)
        {
            try
            {
                this.ReadImageHeaders(stream, out bool inverted, out byte[] palette);

                var image = new ImageBgra32(this.infoHeader.Width, this.infoHeader.Height);
                
                switch (this.infoHeader.Compression)
                {
                    case BmpCompression.RGB:
                        if (this.infoHeader.BitsPerPixel == 32)
                        {
                            this.ReadRgb32(image.Start, this.infoHeader.Width, this.infoHeader.Height, inverted);
                        }
                        else if (this.infoHeader.BitsPerPixel == 24)
                        {
                            this.ReadRgb24(image.Start, this.infoHeader.Width, this.infoHeader.Height, inverted);
                        }
                        else if (this.infoHeader.BitsPerPixel == 16)
                        {
                            this.ReadRgb16(image.Start, this.infoHeader.Width, this.infoHeader.Height, inverted);
                        }
                        else if (this.infoHeader.BitsPerPixel <= 8)
                        {
                            throw new NotSupportedException("Does not support this kind of bitmap files.");
                        }
                        break;
                    case BmpCompression.RLE8:
                    default:
                        throw new NotSupportedException("Does not support this kind of bitmap files.");
                }

                return image;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ImageFormatException("Bitmap does not have a valid format.", e);
            }
        }

        /// <summary>
        /// Reads the raw image information from the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        public IImageInfo Identify(Stream stream)
        {
            this.ReadImageHeaders(stream, out _, out _);
            return new ImageInfo(new PixelTypeInfo(this.infoHeader.BitsPerPixel), this.infoHeader.Width, this.infoHeader.Height);
        }

        /// <summary>
        /// Returns the y- value based on the given height.
        /// </summary>
        /// <param name="y">The y- value representing the current row.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        /// <returns>The <see cref="int"/> representing the inverted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Invert(int y, int height, bool inverted)
        {
            return (!inverted) ? height - y - 1 : y;
        }

        /// <summary>
        /// Calculates the amount of bytes to pad a row.
        /// </summary>
        /// <param name="width">The image width.</param>
        /// <param name="componentCount">The pixel component count.</param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CalculatePadding(int width, int componentCount)
        {
            int padding = (width * componentCount) % 4;

            if (padding != 0)
            {
                padding = 4 - padding;
            }

            return padding;
        }

        /// <summary>
        /// Performs final shifting from a 5bit value to an 8bit one.
        /// </summary>
        /// <param name="value">The masked and shifted value</param>
        /// <returns>The <see cref="byte"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte GetBytesFrom5BitValue(int value)
        {
            return (byte)((value << 3) | (value >> 2));
        }

        /// <summary>
        /// Produce uncompressed bitmap data from RLE8 stream
        /// </summary>
        /// <remarks>
        /// RLE8 is a 2-byte run-length encoding
        /// <br/>If first byte is 0, the second byte may have special meaning
        /// <br/>Otherwise, first byte is the length of the run and second byte is the color for the run
        /// </remarks>
        /// <param name="w">The width of the bitmap.</param>
        /// <param name="buffer">Buffer for uncompressed data.</param>
        private void UncompressRle8(int w, Span<byte> buffer)
        {
            byte[] cmd = new byte[2];
            int count = 0;

            while (count < buffer.Length)
            {
                if (this.stream.Read(cmd, 0, cmd.Length) != 2)
                {
                    throw new Exception("Failed to read 2 bytes from stream");
                }

                if (cmd[0] == RleCommand)
                {
                    switch (cmd[1])
                    {
                        case RleEndOfBitmap:
                            return;

                        case RleEndOfLine:
                            int extra = count % w;
                            if (extra > 0)
                            {
                                count += w - extra;
                            }

                            break;

                        case RleDelta:
                            int dx = this.stream.ReadByte();
                            int dy = this.stream.ReadByte();
                            count += (w * dy) + dx;

                            break;

                        default:
                            // If the second byte > 2, we are in 'absolute mode'
                            // Take this number of bytes from the stream as uncompressed data
                            int length = cmd[1];

                            byte[] run = new byte[length];

                            this.stream.Read(run, 0, run.Length);

                            run.AsSpan().CopyTo(buffer.Slice(count));

                            count += run.Length;

                            // Absolute mode data is aligned to two-byte word-boundary
                            int padding = length & 1;

                            this.stream.Skip(padding);

                            break;
                    }
                }
                else
                {
                    for (int i = 0; i < cmd[0]; i++)
                    {
                        buffer[count++] = cmd[1];
                    }
                }
            }
        }

        /// <summary>
        /// Reads the 16 bit color palette from the stream
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgb16(Bgra32* p0, int width, int height, bool inverted)
        {
            int padding = CalculatePadding(width, 2);
            int stride = (width * 2) + padding;
            var rgba = new Bgra32(0, 0, 0, 255);
            Byte[] row = new byte[stride];
            fixed (Byte* pRow0 = row)
            {
                for (int y = 0; y < height; y++)
                {
                    this.stream.Read(row, 0, row.Length);
                    int newY = Invert(y, height, inverted);
                    short* pRow = (short*)pRow0;
                    Bgra32* pDst = p0 + width * newY;
                    for (int w = 0; w < width; w++)
                    {
                        short temp = pRow[w];
                        rgba.Red = GetBytesFrom5BitValue((temp & Rgb16RMask) >> 10);
                        rgba.Green = GetBytesFrom5BitValue((temp & Rgb16GMask) >> 5);
                        rgba.Blue = GetBytesFrom5BitValue(temp & Rgb16BMask);
                        pDst[w] = rgba;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the 24 bit color palette from the stream
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgb24(Bgra32* p0, int width, int height, bool inverted)
        {
            int padding = CalculatePadding(width, 3);

            Byte[] row = AllocatePaddedPixelRowBuffer(width, 3, padding);
            fixed (Byte* pRow0 = row)
            {
                for (int y = 0; y < height; y++)
                {
                    this.stream.Read(row, 0, row.Length);
                    int newY = Invert(y, height, inverted);
                    Bgr24* pRow = (Bgr24*)pRow0;
                    Bgra32* pDst = p0 + width * newY;
                    for(int w = 0; w < width; w++ )
                    {
                        pDst[w].From(pRow[w]);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the 32 bit color palette from the stream
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The <see cref="PixelAccessor{TPixel}"/> to assign the palette to.</param>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        /// <param name="inverted">Whether the bitmap is inverted.</param>
        private void ReadRgb32(Bgra32* p0, int width, int height, bool inverted)
        {
            int padding = CalculatePadding(width, 4);

            Byte[] row = AllocatePaddedPixelRowBuffer(width, 4, padding);
            fixed(Byte* pRow = row)
            for (int y = 0; y < height; y++)
            {
                this.stream.Read(row, 0, row.Length);
                int newY = Invert(y, height, inverted);
                Bgra32* pLine = p0 + width * newY;
                Unsafe.CopyBlock((void*)pLine, (void*)pRow, (uint)width * 4);
            }
        }

        private Byte[] AllocatePaddedPixelRowBuffer(int width, int bytesPerPixel, int padding)
        {
            return new byte[width * bytesPerPixel + padding];
        }

        /// <summary>
        /// Reads the <see cref="BmpInfoHeader"/> from the stream.
        /// </summary>
        private void ReadInfoHeader()
        {
            byte[] buffer = new byte[BmpInfoHeader.MaxHeaderSize];

            // read header size
            this.stream.Read(buffer, 0, BmpInfoHeader.HeaderSizeSize);

            int headerSize = BitConverter.ToInt32(buffer, 0);
            if (headerSize < BmpInfoHeader.CoreSize)
            {
                throw new NotSupportedException($"ImageSharp does not support this BMP file. HeaderSize: {headerSize}.");
            }

            int skipAmount = 0;
            if (headerSize > BmpInfoHeader.MaxHeaderSize)
            {
                skipAmount = headerSize - BmpInfoHeader.MaxHeaderSize;
                headerSize = BmpInfoHeader.MaxHeaderSize;
            }

            // read the rest of the header
            this.stream.Read(buffer, BmpInfoHeader.HeaderSizeSize, headerSize - BmpInfoHeader.HeaderSizeSize);

            if (headerSize == BmpInfoHeader.CoreSize)
            {
                // 12 bytes
                this.infoHeader = BmpInfoHeader.ParseCore(buffer);
            }
            else if (headerSize >= BmpInfoHeader.Size)
            {
                // >= 40 bytes
                this.infoHeader = BmpInfoHeader.Parse(buffer.AsSpan(0, 40));
            }
            else
            {
                throw new NotSupportedException($"ImageSharp does not support this BMP file. HeaderSize: {headerSize}.");
            }

            // skip the remaining header because we can't read those parts
            this.stream.Skip(skipAmount);
        }

        /// <summary>
        /// Reads the <see cref="BmpFileHeader"/> from the stream.
        /// </summary>
        private void ReadFileHeader()
        {
            byte[] buffer = new byte[BmpFileHeader.Size];

            this.stream.Read(buffer, 0, BmpFileHeader.Size);

            this.fileHeader = BmpFileHeader.Parse(buffer);
        }

        /// <summary>
        /// Reads the <see cref="BmpFileHeader"/> and <see cref="BmpInfoHeader"/> from the stream and sets the corresponding fields.
        /// </summary>
        private void ReadImageHeaders(Stream stream, out bool inverted, out byte[] palette)
        {
            this.stream = stream;

            this.ReadFileHeader();
            this.ReadInfoHeader();

            // see http://www.drdobbs.com/architecture-and-design/the-bmp-file-format-part-1/184409517
            // If the height is negative, then this is a Windows bitmap whose origin
            // is the upper-left corner and not the lower-left. The inverted flag
            // indicates a lower-left origin.Our code will be outputting an
            // upper-left origin pixel array.
            inverted = false;
            if (this.infoHeader.Height < 0)
            {
                inverted = true;
                this.infoHeader.Height = -this.infoHeader.Height;
            }

            int colorMapSize = -1;

            if (this.infoHeader.ClrUsed == 0)
            {
                if (this.infoHeader.BitsPerPixel == 1 ||
                    this.infoHeader.BitsPerPixel == 4 ||
                    this.infoHeader.BitsPerPixel == 8)
                {
                    colorMapSize = (int)Math.Pow(2, this.infoHeader.BitsPerPixel) * 4;
                }
            }
            else
            {
                colorMapSize = this.infoHeader.ClrUsed * 4;
            }

            palette = null;

            if (colorMapSize > 0)
            {
                // 256 * 4
                if (colorMapSize > 1024)
                {
                    throw new ImageFormatException($"Invalid bmp colormap size '{colorMapSize}'");
                }

                palette = new byte[colorMapSize];

                this.stream.Read(palette, 0, colorMapSize);
            }

            if (this.infoHeader.Width > int.MaxValue || this.infoHeader.Height > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    $"The input bmp '{this.infoHeader.Width}x{this.infoHeader.Height}' is "
                    + $"bigger then the max allowed size '{int.MaxValue}x{int.MaxValue}'");
            }
        }
    }
}
