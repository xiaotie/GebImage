using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Geb.Image.Test
{
    [TestClass]
    public class BitmapTest
    {
        [TestMethod]
        public void TestDecode()
        {
            string path = @"./img/demo-bmp-24.bmp";
            var decoder = new Formats.Bmp.BmpDecoder();
            var img = decoder.Decode(path);
            Assert.AreEqual(120, img.Width);
            Assert.AreEqual(0, img[0].Red);
            Assert.AreEqual(255, img[img.Length - 1].Red);
        }

        [TestMethod]
        public void TestEncodeBmp()
        {
            string path = @"./img/demo-bmp-24.bmp";
            string enPath = GetTempFilePath("TestEncode_out.bmp");
            var decoder = new Formats.Bmp.BmpDecoder();
            var img = decoder.Decode(path);
            if (File.Exists(enPath) == true) File.Delete(enPath);
            img.SaveBmp(enPath);

            img = decoder.Decode(enPath);
            Assert.AreEqual(120, img.Width);
            Assert.AreEqual(0, img[0].Red);
            Assert.AreEqual(255, img[img.Length - 1].Red);
            if (File.Exists(enPath) == true) File.Delete(enPath);
        }

        protected string GetTempFilePath(string filePath)
        {
            DirectoryInfo dirTmpImfo = new DirectoryInfo("./temp");
            if (dirTmpImfo.Exists == false) dirTmpImfo.Create();
            return Path.Combine(dirTmpImfo.FullName, filePath);
        }

        [TestMethod]
        public void TestEncodeJpg()
        {
            string path = @"./img/demo-bmp-big-01.bmp";
            var decoder = new Formats.Bmp.BmpDecoder();
            var image = decoder.Decode(path);
            Assert.AreEqual(true, SaveJpeg(image, 5));
            Assert.AreEqual(true, SaveJpeg(image, 30));
            Assert.AreEqual(true, SaveJpeg(image, 60));
            Assert.AreEqual(true, SaveJpeg(image, 80));
            Assert.AreEqual(true, SaveJpeg(image, 95));

            string jpeg = GetTempJpegFilePath(95);
            Formats.Jpeg.JpegDecoder jpegDecoder = new Formats.Jpeg.JpegDecoder();
            var img = jpegDecoder.Decode(jpeg);
            Assert.AreEqual(true, img.Width > 0);
            Assert.AreEqual(true, SaveJpeg(img, 50));
        }

        [TestMethod]
        public void TestEncodeJpg8()
        {
            string path = @"./img/demo-bmp-big-01.bmp";
            var decoder = new Formats.Bmp.BmpDecoder();
            var image = decoder.Decode(path);
            Assert.AreEqual(true, SaveJpeg8(image, 30));
            Assert.AreEqual(true, SaveJpeg8(image, 80));
            string path2 = GetTempJpegFilePath(80, true);
            Formats.Jpeg.JpegDecoder jpegDecoder = new Formats.Jpeg.JpegDecoder();
            var img = jpegDecoder.Decode(path2);
            Assert.AreEqual(true, img.Width > 0);
        }

        [TestMethod]
        public void TestEncodePng()
        {
            string path = @"./img/demo-bmp-big-01.bmp";
            var decoder = new Formats.Bmp.BmpDecoder();
            var image = decoder.Decode(path);
            Assert.AreEqual(true, SavePng(image));
            Assert.AreEqual(true, SavePng8(image));
        }

        [TestMethod]
        public void TestDecodePng()
        {
            string path = @"./img/demo-bmp-big-01.bmp";
            var decoder = new Formats.Bmp.BmpDecoder();
            var image = decoder.Decode(path);
            Assert.AreEqual(true, SavePng(image));
            var pngDecoder = new Formats.Png.PngDecoder();
            image = pngDecoder.Decode("TestEncodePng.png");
            Assert.AreEqual(true, image.Length > 0);
            Assert.AreEqual(true, SavePng(image, "TestEncodePng2.png"));
        }

        protected bool SavePng(ImageBgra32 image, string path = "TestEncodePng.png")
        {
            if (File.Exists(path) == true) File.Delete(path);
            image.SavePng(path);
            if (File.Exists(path) == false) return false;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0) return false;
            return true;
        }

        protected bool SavePng8(ImageBgra32 image, string path = "TestEncodePng8.png")
        {
            if (File.Exists(path) == true) File.Delete(path);
            image.SavePng(path, Formats.Png.PngEncoderOptions.Png8);
            if (File.Exists(path) == false) return false;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0) return false;
            return true;
        }

        protected bool SaveJpeg(ImageBgra32 image, int quality)
        {
            string path = GetTempJpegFilePath(quality);
            if (File.Exists(path) == true) File.Delete(path);
            image.SaveJpeg(path, quality);
            if (File.Exists(path) == false) return false;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0) return false;
            return true;
        }

        protected bool SaveJpeg8(ImageBgra32 image, int quality)
        {
            string path = GetTempJpegFilePath(quality, true);
            if (File.Exists(path) == true) File.Delete(path);
            image.SaveJpeg(path, quality, Formats.Jpeg.JpegPixelFormats.Gray);
            if (File.Exists(path) == false) return false;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0) return false;
            return true;
        }

        protected string GetTempJpegFilePath(int quality, bool isJpeg8 = false)
        {
            string tail = isJpeg8 ? "_8" : "";
            return GetTempFilePath("TestEncodeJpg_" + quality + tail + ".jpg");
        }
    }
}
