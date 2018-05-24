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
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                var img = decoder.Decode(fs);
                Assert.AreEqual(120, img.Width);
                Assert.AreEqual(0, img[0].Red);
                Assert.AreEqual(255, img[img.Length - 1].Red);
            }
        }
    }
}
