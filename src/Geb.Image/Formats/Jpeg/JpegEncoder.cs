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
        private static BaseJpegEncoder CreateJpegEncoder(int quality, JpegPixelFormats fmt)
        {
            if (fmt == JpegPixelFormats.Gray) return new Jpeg8GrayEncoder(quality);
            else return new Jpeg8YCbCrEncoder(quality);
        }

        public static void Encode(ImageBgra32 image, Stream stream, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (ImageHolder h = ImageHolder.Create(image))
                CreateJpegEncoder(quality, fmt).Encode(h, stream);
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

        public static void Encode(ImageBgr24 image, Stream stream, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (ImageHolder h = ImageHolder.Create(image))
                CreateJpegEncoder(quality, fmt).Encode(h, stream);
        }

        public static void Encode(ImageBgr24 image, string path, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                Encode(image, fs, quality, fmt);
            }
        }

        public static byte[] Encode(ImageBgr24 image, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                Encode(image, fs, quality, fmt);
                return fs.ToArray();
            }
        }

        public static void Encode(ImageU8 image, Stream stream, int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr)
        {
            using (ImageHolder h = ImageHolder.Create(image))
                CreateJpegEncoder(quality, fmt).Encode(h, stream);
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
