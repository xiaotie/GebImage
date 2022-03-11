// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

using Geb.Image.Formats.Quantization;

namespace Geb.Image.Formats.Png
{
    public sealed class PngEncoderOptions
    {
        public static readonly PngEncoderOptions Png32 = new PngEncoderOptions();
        public static readonly PngEncoderOptions Png24 = new PngEncoderOptions { PngColorType = PngColorType.Rgb };
        public static readonly PngEncoderOptions Png8 = new PngEncoderOptions { PngColorType = PngColorType.Grayscale };

        /// <summary>
        /// Gets or sets the png color type.
        /// </summary>
        public PngColorType PngColorType { get; set; } = PngColorType.RgbWithAlpha;

        /// <summary>
        /// Gets or sets the png filter method.
        /// </summary>
        public PngFilterMethod PngFilterMethod { get; set; } = PngFilterMethod.Adaptive;

        /// <summary>
        /// Gets or sets the compression level 1-9.
        /// <remarks>Defaults to 6.</remarks>
        /// </summary>
        public int CompressionLevel { get; set; } = 6;

        /// <summary>
        /// Gets or sets the gamma value, that will be written
        /// the the stream, when the <see cref="WriteGamma"/> property
        /// is set to true. The default value is 2.2F.
        /// </summary>
        /// <value>The gamma value of the image.</value>
        public float Gamma { get; set; } = 2.2F;

        /// <summary>
        /// Gets or sets quantizer for reducing the color count.
        /// Defaults to the <see cref="WuQuantizer"/>
        /// </summary>
        public IQuantizer Quantizer { get; set; } /*= new WuQuantizer();*/

        /// <summary>
        /// Gets or sets the transparency threshold.
        /// </summary>
        public byte Threshold { get; set; } = 255;

        /// <summary>
        /// Gets or sets a value indicating whether this instance should write
        /// gamma information to the stream. The default value is false.
        /// </summary>
        public bool WriteGamma { get; set; }
    }

    /// <summary>
    /// Image encoder for writing image data to a stream in png format.
    /// </summary>
    public sealed class PngEncoder
    {
        public static void Encode(ImageBgra32 image, Stream stream, PngEncoderOptions options = null)
        {
            using (var encoder = new PngEncoderCore(Configuration.Default.MemoryManager, options ?? PngEncoderOptions.Png32))
            {
                encoder.Encode(image, stream);
            }
        }

        public static void Encode(ImageBgr24 image, Stream stream, PngEncoderOptions options = null)
        {
            using (var encoder = new PngEncoderCore(Configuration.Default.MemoryManager, options ?? PngEncoderOptions.Png24))
            {
                encoder.Encode(image, stream);
            }
        }

        public static void Encode(ImageU8 image, Stream stream, PngEncoderOptions options = null)
        {
            using (var encoder = new PngEncoderCore(Configuration.Default.MemoryManager, options ?? PngEncoderOptions.Png8))
            {
                encoder.Encode(image, stream);
            }
        }

        public static void Encode(ImageBgra32 image, string path, PngEncoderOptions options = null)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs, options);
            }
        }

        public static void Encode(ImageBgr24 image, string path, PngEncoderOptions options = null)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs, options);
            }
        }

        public static void Encode(ImageU8 image, string path, PngEncoderOptions options = null)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs, options);
            }
        }

        public static byte[] Encode(ImageBgra32 image, PngEncoderOptions options = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Encode(image, ms, options);
                return ms.ToArray();
            }
        }

        public static byte[] Encode(ImageBgr24 image, PngEncoderOptions options = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Encode(image, ms, options);
                return ms.ToArray();
            }
        }

        public static byte[] Encode(ImageU8 image, PngEncoderOptions options = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Encode(image, ms, options);
                return ms.ToArray();
            }
        }
    }
}