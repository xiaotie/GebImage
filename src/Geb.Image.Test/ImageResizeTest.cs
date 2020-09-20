using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Geb.Image.Test
{
    [TestClass]
    public class ImageResizeTest
    {
        [TestMethod]
        public void TestDecode()
        {
            ImageBgr24 img = new ImageBgr24(80, 100);
            for (int h = 0; h < img.Height; h++)
            {
                for (int w = 0; w < img.Width; w++)
                {
                    int g = (h + w) * 100;
                    Bgr24 val = new Bgr24(g, g, g);
                    img[h, w] = val;
                }
            }

            ImageBgr24 img2 = img.Resize(60, 60, InterpolationMode.Bilinear);
            Bgr24 c = img2[1, 1];
            Assert.AreEqual(101, c.Blue);
        }
    }
}
