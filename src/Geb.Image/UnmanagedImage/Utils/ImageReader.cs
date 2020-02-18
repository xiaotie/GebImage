using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Geb.Image
{
    using Geb.Image.Formats.Jpeg;
    using Geb.Image.Formats.Png;
    using Geb.Image.Formats.Bmp;
    using Geb.Image.Formats;

    public class UnsupportedImageFormatException: Exception
    {
        public UnsupportedImageFormatException(String fileName):base("File's Image Format Unsupported: " + fileName)
        {
        }
    }

    public class ImageReader
    {
        public static ImageReader Instance = new ImageReader();

        private JpegDecoder jpegDecoder = new JpegDecoder();
        private PngDecoder pngDecoder = new PngDecoder();
        private List<IImageFormatDetector> formatDetectors;
        private ImageReader() {
            formatDetectors = new List<IImageFormatDetector>();
            formatDetectors.Add(new JpegImageFormatDetector());
            formatDetectors.Add(new PngImageFormatDetector());
        }

        private IImageFormat DetectFormat(Stream stream)
        {
            stream.Position = 0;
            int headerLength = (int)Math.Min(stream.Length, 1024);
            Byte[] buff = new byte[headerLength];
            stream.Read(buff, 0, headerLength);
            stream.Position = 0;
            ReadOnlySpan<Byte> span = new ReadOnlySpan<byte>(buff);
            foreach(var item in formatDetectors)
            {
                IImageFormat fmt = item.DetectFormat(buff);
                if (fmt != null) return fmt; 
            }
            return null;
        }

        public ImageBgra32 Read(String imgFilePath)
        {
            using(Stream stream = new FileStream(imgFilePath, FileMode.Open))
            {
                IImageFormat fmt = DetectFormat(stream);
                if (fmt is JpegFormat)
                    return jpegDecoder.Decode(stream);
                else if (fmt is PngFormat)
                    return pngDecoder.Decode(stream);
            }

            throw new UnsupportedImageFormatException(imgFilePath);

            return null;
        }
    }
}
