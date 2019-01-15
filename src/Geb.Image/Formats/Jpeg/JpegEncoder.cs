using System;
using System.Collections.Generic;
using System.IO;

namespace Geb.Image.Formats.Jpeg
{
    public enum JpegPixelFormats
    {
        YCbCr,
        Gray
    }

    /// <summary>
    /// Encoder for writing the data image to a stream in jpeg format.
    /// </summary>
    public sealed class JpegEncoder
    {
        public static void Encode(ImageBgra32 image, Stream stream, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            var encoder = new JpegEncoderCore(quality,fmt);
            encoder.Encode(image, stream);
        }

        public static void Encode(ImageBgra32 image, string path, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs, quality, fmt);
            }
        }

        public static byte[] Encode(ImageBgra32 image, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                Encode(image, fs, quality, fmt);
                return fs.ToArray();
            }
        }

        public static void Encode(ImageU8 image, Stream stream, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            var encoder = new JpegEncoderCore(quality, fmt);
            encoder.Encode(image, stream);
        }

        public static void Encode(ImageU8 image, string path, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs, quality, fmt);
            }
        }

        public static byte[] Encode(ImageU8 image, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                Encode(image, fs, quality, fmt);
                return fs.ToArray();
            }
        }
    }
}
