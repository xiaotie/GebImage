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
        public void TestEncode()
        {
            string path = @"./img/demo-bmp-24.bmp";
            string enPath = path + "_out_tmp.bmp";
            string enJpgPath = path + "_out_tmp.jpg";
            var decoder = new Formats.Bmp.BmpDecoder();
            var img = decoder.Decode(path);
            if (File.Exists(enPath) == true) File.Delete(enPath);
            img.SaveBmp(enPath);

            img = decoder.Decode(enPath);
            Assert.AreEqual(120, img.Width);
            Assert.AreEqual(0, img[0].Red);
            Assert.AreEqual(255, img[img.Length - 1].Red);
            if (File.Exists(enPath) == true) File.Delete(enPath);

            if (File.Exists(enJpgPath) == true) File.Delete(enJpgPath);
            img.SaveJpeg(enJpgPath);
            // if (File.Exists(enJpgPath) == true) File.Delete(enJpgPath);
        }
    }
}
