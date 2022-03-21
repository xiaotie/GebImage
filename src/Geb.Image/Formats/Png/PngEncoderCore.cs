// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using Geb.Image.Formats.Png.Filters;
using Geb.Image.Formats.Png.Zlib;
using Geb.Image.Formats.Quantization;

namespace Geb.Image.Formats.Png
{
    /// <summary>
    /// Performs the png encoding operation.
    /// </summary>
    internal sealed class PngEncoderCore : IDisposable
    {
        private readonly MemoryManager memoryManager;

        /// <summary>
        /// The maximum block size, defaults at 64k for uncompressed blocks.
        /// </summary>
        private const int MaxBlockSize = 65535;

        /// <summary>
        /// Reusable buffer for writing general data.
        /// </summary>
        private readonly byte[] buffer = new byte[8];

        /// <summary>
        /// Reusable buffer for writing chunk data.
        /// </summary>
        private readonly byte[] chunkDataBuffer = new byte[16];

        /// <summary>
        /// Reusable crc for validating chunks.
        /// </summary>
        private readonly Crc32 crc = new Crc32();

        /// <summary>
        /// The png color type.
        /// </summary>
        private readonly PngColorType pngColorType;

        /// <summary>
        /// The png filter method.
        /// </summary>
        private readonly PngFilterMethod pngFilterMethod;

        /// <summary>
        /// The quantizer for reducing the color count.
        /// </summary>
        private readonly IQuantizer quantizer;

        /// <summary>
        /// Gets or sets the CompressionLevel value
        /// </summary>
        private readonly int compressionLevel;

        /// <summary>
        /// Gets or sets the Gamma value
        /// </summary>
        private readonly float gamma;

        /// <summary>
        /// Gets or sets the Threshold value
        /// </summary>
        private readonly byte threshold;

        /// <summary>
        /// Gets or sets a value indicating whether to Write Gamma
        /// </summary>
        private readonly bool writeGamma;

        /// <summary>
        /// Contains the raw pixel data from an indexed image.
        /// </summary>
        private byte[] palettePixelData;

        /// <summary>
        /// The image width.
        /// </summary>
        private int width;

        /// <summary>
        /// The image height.
        /// </summary>
        private int height;

        /// <summary>
        /// The number of bits required to encode the colors in the png.
        /// </summary>
        private byte bitDepth;

        /// <summary>
        /// The number of bytes per pixel.
        /// </summary>
        private int bytesPerPixel;

        /// <summary>
        /// The number of bytes per scanline.
        /// </summary>
        private int bytesPerScanline;

        /// <summary>
        /// The previous scanline.
        /// </summary>
        private IManagedByteBuffer previousScanline;

        /// <summary>
        /// The raw scanline.
        /// </summary>
        private IManagedByteBuffer rawScanline;

        /// <summary>
        /// The filtered scanline result.
        /// </summary>
        private IManagedByteBuffer result;

        /// <summary>
        /// The buffer for the sub filter
        /// </summary>
        private IManagedByteBuffer sub;

        /// <summary>
        /// The buffer for the up filter
        /// </summary>
        private IManagedByteBuffer up;

        /// <summary>
        /// The buffer for the average filter
        /// </summary>
        private IManagedByteBuffer average;

        /// <summary>
        /// The buffer for the Paeth filter
        /// </summary>
        private IManagedByteBuffer paeth;

        /// <summary>
        /// Initializes a new instance of the <see cref="PngEncoderCore"/> class.
        /// </summary>
        /// <param name="memoryManager">The <see cref="MemoryManager"/> to use for buffer allocations.</param>
        /// <param name="options">The options for influencing the encoder</param>
        public PngEncoderCore(MemoryManager memoryManager, PngEncoderOptions options)
        {
            if (options == null) options = PngEncoderOptions.Png32;
            this.memoryManager = memoryManager;
            this.pngColorType = options.PngColorType;
            this.pngFilterMethod = options.PngFilterMethod;
            this.compressionLevel = options.CompressionLevel;
            this.gamma = options.Gamma;
            this.quantizer = options.Quantizer;
            this.threshold = options.Threshold;
            this.writeGamma = options.WriteGamma;
        }

        /// <summary>
        /// Encodes the image to the specified stream from the <see cref="Image{TPixel}"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="image">The <see cref="ImageFrame{TPixel}"/> to encode from.</param>
        /// <param name="stream">The <see cref="Stream"/> to encode the image data to.</param>
        public void Encode(IImage image, Stream stream)
        {
            this.width = image.Width;
            this.height = image.Height;

            stream.Write(PngConstants.HeaderBytes, 0, PngConstants.HeaderBytes.Length);

            this.bitDepth = 8;
            this.bytesPerPixel = this.CalculateBytesPerPixel();

            var header = new PngHeader(
                width: image.Width,
                height: image.Height,
                colorType: this.pngColorType,
                bitDepth: this.bitDepth,
                filterMethod: 0, // None
                compressionMethod: 0,
                interlaceMethod: 0);

            this.WriteHeaderChunk(stream, header);
            this.WriteDataChunks(image, stream);
            this.WriteEndChunk(stream);
            stream.Flush();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.previousScanline?.Dispose();
            this.rawScanline?.Dispose();
            this.result?.Dispose();
            this.sub?.Dispose();
            this.up?.Dispose();
            this.average?.Dispose();
            this.paeth?.Dispose();
        }

        /// <summary>
        /// Collects a row of grayscale pixels.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="rowSpan">The image row span.</param>
        private void CollectGrayscaleBytes(ReadOnlySpan<Bgra32> rowSpan)
        {
            byte[] rawScanlineArray = this.rawScanline.Array;

            // Copy the pixels across from the image.
            // Reuse the chunk type buffer.
            for (int x = 0; x < this.width; x++)
            {
                // Convert the color to YCbCr and store the luminance
                // Optionally store the original color alpha.
                int offset = x * this.bytesPerPixel;
                Bgra32 bgra = rowSpan[x];
                byte luminance = (byte)((0.299F * bgra.Red) + (0.587F * bgra.Green) + (0.114F * bgra.Blue));

                for (int i = 0; i < this.bytesPerPixel; i++)
                {
                    if (i == 0)
                    {
                        rawScanlineArray[offset] = luminance;
                    }
                    else
                    {
                        rawScanlineArray[offset + i] = bgra.Alpha;
                    }
                }
            }
        }

        private void CollectGrayscaleBytes(ReadOnlySpan<Bgr24> rowSpan)
        {
            byte[] rawScanlineArray = this.rawScanline.Array;

            // Copy the pixels across from the image.
            // Reuse the chunk type buffer.
            for (int x = 0; x < this.width; x++)
            {
                // Convert the color to YCbCr and store the luminance
                // Optionally store the original color alpha.
                int offset = x * this.bytesPerPixel;
                Bgr24 bgra = rowSpan[x];
                byte luminance = (byte)((0.299F * bgra.Red) + (0.587F * bgra.Green) + (0.114F * bgra.Blue));

                for (int i = 0; i < this.bytesPerPixel; i++)
                {
                    rawScanlineArray[offset] = luminance;
                }
            }
        }

        private void CollectGrayscaleBytes(ReadOnlySpan<Byte> rowSpan)
        {
            byte[] rawScanlineArray = this.rawScanline.Array;

            // Copy the pixels across from the image.
            // Reuse the chunk type buffer.
            if(this.bytesPerPixel == 1)
            {
                for (int x = 0; x < this.width; x++)
                {
                    rawScanlineArray[x] = rowSpan[x];
                }
            }
            else
            {
                for (int x = 0; x < this.width; x++)
                {
                    // Convert the color to YCbCr and store the luminance
                    // Optionally store the original color alpha.
                    int offset = x * this.bytesPerPixel;
                    Byte bgra = rowSpan[x];
                    for (int i = 0; i < this.bytesPerPixel; i++)
                    {
                        if (i == 0)
                        {
                            rawScanlineArray[offset] = bgra;
                        }
                        else
                        {
                            rawScanlineArray[offset + i] = 0xFF;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Collects a row of true color pixel data.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="rowSpan">The row span.</param>
        private void CollectTPixelBytes(ReadOnlySpan<Bgra32> rowSpan)
        {
            if (this.bytesPerPixel == 4)
            {
                rowSpan.ToRgba32Bytes(rawScanline.Span, this.width);
            }
            else
            {
                rowSpan.ToRgb24Bytes(rawScanline.Span, this.width);
            }
        }

        private void CollectTPixelBytes(ReadOnlySpan<Bgr24> rowSpan)
        {
            if (this.bytesPerPixel == 4)
            {
                rowSpan.ToRgba32Bytes(rawScanline.Span, this.width);
            }
            else
            {
                rowSpan.ToRgb24Bytes(rawScanline.Span, this.width);
            }
        }

        private void CollectTPixelBytes(ReadOnlySpan<Byte> rowSpan)
        {
            byte[] rawScanlineArray = this.rawScanline.Array;
            for (int x = 0; x < this.width; x++)
            {
                // Convert the color to YCbCr and store the luminance
                // Optionally store the original color alpha.
                int offset = x * this.bytesPerPixel;
                Byte val = rowSpan[x];
                for (int i = 0; i < this.bytesPerPixel; i++)
                {
                    rawScanlineArray[offset + i] = i < 2 ? val : (Byte)0xFF;
                }
            }
        }

        /// <summary>
        /// Encodes the pixel data line by line.
        /// Each scanline is encoded in the most optimal manner to improve compression.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="rowSpan">The row span.</param>
        /// <param name="row">The row.</param>
        /// <returns>The <see cref="IManagedByteBuffer"/></returns>
        private IManagedByteBuffer EncodePixelRow(ReadOnlySpan<Bgra32> rowSpan, int row)
        {
            switch (this.pngColorType)
            {
                case PngColorType.Palette:
                    Buffer.BlockCopy(this.palettePixelData, row * this.rawScanline.Length(), this.rawScanline.Array, 0, this.rawScanline.Length());
                    break;
                case PngColorType.Grayscale:
                case PngColorType.GrayscaleWithAlpha:
                    this.CollectGrayscaleBytes(rowSpan);
                    break;
                default:
                    this.CollectTPixelBytes(rowSpan);
                    break;
            }

            return FilterPixelRow();
        }

        private IManagedByteBuffer EncodePixelRow(ReadOnlySpan<Bgr24> rowSpan, int row)
        {
            switch (this.pngColorType)
            {
                case PngColorType.Palette:
                    Buffer.BlockCopy(this.palettePixelData, row * this.rawScanline.Length(), this.rawScanline.Array, 0, this.rawScanline.Length());
                    break;
                case PngColorType.Grayscale:
                case PngColorType.GrayscaleWithAlpha:
                    this.CollectGrayscaleBytes(rowSpan);
                    break;
                default:
                    this.CollectTPixelBytes(rowSpan);
                    break;
            }

            return FilterPixelRow();
        }

        private IManagedByteBuffer EncodePixelRow(ReadOnlySpan<Byte> rowSpan, int row)
        {
            switch (this.pngColorType)
            {
                case PngColorType.Palette:
                    Buffer.BlockCopy(this.palettePixelData, row * this.rawScanline.Length(), this.rawScanline.Array, 0, this.rawScanline.Length());
                    break;
                case PngColorType.Grayscale:
                case PngColorType.GrayscaleWithAlpha:
                    this.CollectGrayscaleBytes(rowSpan);
                    break;
                default:
                    this.CollectTPixelBytes(rowSpan);
                    break;
            }

            return FilterPixelRow();
        }

        private IManagedByteBuffer FilterPixelRow()
        {
            switch (this.pngFilterMethod)
            {
                case PngFilterMethod.None:
                    NoneFilter.Encode(this.rawScanline.Span, this.result.Span);
                    return this.result;

                case PngFilterMethod.Sub:
                    SubFilter.Encode(this.rawScanline.Span, this.sub.Span, this.bytesPerPixel, out int _);
                    return this.sub;

                case PngFilterMethod.Up:
                    UpFilter.Encode(this.rawScanline.Span, this.previousScanline.Span, this.up.Span, out int _);
                    return this.up;

                case PngFilterMethod.Average:
                    AverageFilter.Encode(this.rawScanline.Span, this.previousScanline.Span, this.average.Span, this.bytesPerPixel, out int _);
                    return this.average;

                case PngFilterMethod.Paeth:
                    PaethFilter.Encode(this.rawScanline.Span, this.previousScanline.Span, this.paeth.Span, this.bytesPerPixel, out int _);
                    return this.paeth;

                default:
                    return this.GetOptimalFilteredScanline();
            }

            return this.GetOptimalFilteredScanline();
        }

        /// <summary>
        /// Applies all PNG filters to the given scanline and returns the filtered scanline that is deemed
        /// to be most compressible, using lowest total variation as proxy for compressibility.
        /// </summary>
        /// <returns>The <see cref="T:byte[]"/></returns>
        private IManagedByteBuffer GetOptimalFilteredScanline()
        {
            // Palette images don't compress well with adaptive filtering.
            if (this.pngColorType == PngColorType.Palette || this.bitDepth < 8)
            {
                NoneFilter.Encode(this.rawScanline.Span, this.result.Span);
                return this.result;
            }

            Span<byte> scanSpan = this.rawScanline.Span;
            Span<byte> prevSpan = this.previousScanline.Span;

            // This order, while different to the enumerated order is more likely to produce a smaller sum
            // early on which shaves a couple of milliseconds off the processing time.
            UpFilter.Encode(scanSpan, prevSpan, this.up.Span, out int currentSum);

            int lowestSum = currentSum;
            IManagedByteBuffer actualResult = this.up;

            PaethFilter.Encode(scanSpan, prevSpan, this.paeth.Span, this.bytesPerPixel, out currentSum);

            if (currentSum < lowestSum)
            {
                lowestSum = currentSum;
                actualResult = this.paeth;
            }

            SubFilter.Encode(scanSpan, this.sub.Span, this.bytesPerPixel, out currentSum);

            if (currentSum < lowestSum)
            {
                lowestSum = currentSum;
                actualResult = this.sub;
            }

            AverageFilter.Encode(scanSpan, prevSpan, this.average.Span, this.bytesPerPixel, out currentSum);

            if (currentSum < lowestSum)
            {
                actualResult = this.average;
            }

            return actualResult;
        }

        /// <summary>
        /// Calculates the correct number of bytes per pixel for the given color type.
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        private int CalculateBytesPerPixel()
        {
            switch (this.pngColorType)
            {
                case PngColorType.Grayscale:
                    return 1;

                case PngColorType.GrayscaleWithAlpha:
                    return 2;

                case PngColorType.Palette:
                    return 1;

                case PngColorType.Rgb:
                    return 3;

                // PngColorType.RgbWithAlpha
                // TODO: Maybe figure out a way to detect if there are any transparent
                // pixels and encode RGB if none.
                default:
                    return 4;
            }
        }

        /// <summary>
        /// Writes the header chunk to the stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        /// <param name="header">The <see cref="PngHeader"/>.</param>
        private void WriteHeaderChunk(Stream stream, in PngHeader header)
        {
            BinaryPrimitives.WriteInt32BigEndian(this.chunkDataBuffer.AsSpan(0, 4), header.Width);
            BinaryPrimitives.WriteInt32BigEndian(this.chunkDataBuffer.AsSpan(4, 4), header.Height);

            this.chunkDataBuffer[8] = header.BitDepth;
            this.chunkDataBuffer[9] = (byte)header.ColorType;
            this.chunkDataBuffer[10] = header.CompressionMethod;
            this.chunkDataBuffer[11] = header.FilterMethod;
            this.chunkDataBuffer[12] = (byte)header.InterlaceMethod;

            this.WriteChunk(stream, PngChunkType.Header, this.chunkDataBuffer, 0, 13);
        }

        /// <summary>
        /// Writes the gamma information to the stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        private void WriteGammaChunk(Stream stream)
        {
            if (this.writeGamma)
            {
                // 4-byte unsigned integer of gamma * 100,000.
                uint gammaValue = (uint)(this.gamma * 100_000F);

                BinaryPrimitives.WriteUInt32BigEndian(this.chunkDataBuffer.AsSpan(0, 4), gammaValue);

                this.WriteChunk(stream, PngChunkType.Gamma, this.chunkDataBuffer, 0, 4);
            }
        }

        /// <summary>
        /// Writes the pixel information to the stream.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The image.</param>
        /// <param name="stream">The stream.</param>
        private void WriteDataChunks(IImage pixels, Stream stream)
        {
            this.bytesPerScanline = this.width * this.bytesPerPixel;
            int resultLength = this.bytesPerScanline + 1;

            this.previousScanline = this.memoryManager.AllocateCleanManagedByteBuffer(this.bytesPerScanline);
            this.rawScanline = this.memoryManager.AllocateCleanManagedByteBuffer(this.bytesPerScanline);
            this.result = this.memoryManager.AllocateCleanManagedByteBuffer(resultLength);

            if (this.pngColorType != PngColorType.Palette)
            {
                this.sub = this.memoryManager.AllocateCleanManagedByteBuffer(resultLength);
                this.up = this.memoryManager.AllocateCleanManagedByteBuffer(resultLength);
                this.average = this.memoryManager.AllocateCleanManagedByteBuffer(resultLength);
                this.paeth = this.memoryManager.AllocateCleanManagedByteBuffer(resultLength);
            }

            byte[] buffer;
            int bufferLength;

            using (var memoryStream = new MemoryStream())
            {
                using (var deflateStream = new ZlibDeflateStream(memoryStream, this.compressionLevel))
                {
                    for (int y = 0; y < this.height; y++)
                    {
                        IManagedByteBuffer r = EncodePixelRow(pixels, y);
                        deflateStream.Write(r.Array, 0, resultLength);

                        IManagedByteBuffer temp = this.rawScanline;
                        this.rawScanline = this.previousScanline;
                        this.previousScanline = temp;
                    }
                }

                buffer = memoryStream.ToArray();
                bufferLength = buffer.Length;
            }

            // Store the chunks in repeated 64k blocks.
            // This reduces the memory load for decoding the image for many decoders.
            int numChunks = bufferLength / MaxBlockSize;

            if (bufferLength % MaxBlockSize != 0)
            {
                numChunks++;
            }

            for (int i = 0; i < numChunks; i++)
            {
                int length = bufferLength - (i * MaxBlockSize);

                if (length > MaxBlockSize)
                {
                    length = MaxBlockSize;
                }

                this.WriteChunk(stream, PngChunkType.Data, buffer, i * MaxBlockSize, length);
            }
        }

        private IManagedByteBuffer EncodePixelRow(IImage image, int y)
        {
            if(image.BytesPerPixel == 4)
                return this.EncodePixelRow(((ImageBgra32)image).GetPixelRowReadOnlySpan(y), y);
            else if (image.BytesPerPixel == 3)
                return this.EncodePixelRow(((ImageBgr24)image).GetPixelRowReadOnlySpan(y), y);
            else
                return this.EncodePixelRow(((ImageU8)image).GetPixelRowReadOnlySpan(y), y);
        }

        /// <summary>
        /// Writes the chunk end to the stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        private void WriteEndChunk(Stream stream)
        {
            this.WriteChunk(stream, PngChunkType.End, null);
        }

        /// <summary>
        /// Writes a chunk to the stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="type">The type of chunk to write.</param>
        /// <param name="data">The <see cref="T:byte[]"/> containing data.</param>
        private void WriteChunk(Stream stream, PngChunkType type, byte[] data)
        {
            this.WriteChunk(stream, type, data, 0, data?.Length ?? 0);
        }

        /// <summary>
        /// Writes a chunk of a specified length to the stream at the given offset.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="type">The type of chunk to write.</param>
        /// <param name="data">The <see cref="T:byte[]"/> containing data.</param>
        /// <param name="offset">The position to offset the data at.</param>
        /// <param name="length">The of the data to write.</param>
        private void WriteChunk(Stream stream, PngChunkType type, byte[] data, int offset, int length)
        {
            BinaryPrimitives.WriteInt32BigEndian(this.buffer, length);
            BinaryPrimitives.WriteUInt32BigEndian(this.buffer.AsSpan(4, 4), (uint)type);

            stream.Write(this.buffer, 0, 8);

            this.crc.Reset();

            this.crc.Update(this.buffer.AsSpan(4, 4)); // Write the type buffer

            if (data != null && length > 0)
            {
                stream.Write(data, offset, length);

                this.crc.Update(data.AsSpan(offset, length));
            }

            BinaryPrimitives.WriteUInt32BigEndian(this.buffer, (uint)this.crc.Value);

            stream.Write(this.buffer, 0, 4); // write the crc
        }
    }
}