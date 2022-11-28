
namespace Geb.Image.SkiaSharp.Test
{
    [TestClass]
    public class ImageFactoryTest
    {
        [TestMethod]
        public void TestRead()
        {
            var im = ImageIO.ReadBgra8888("Image/0001.png");
            var imU8 = ImageIO.ReadGray8("Image/0001.png");
            Assert.IsTrue(225 == im.Width);
            Assert.IsTrue(225 == imU8.Width);
            Assert.IsTrue(ImageIO.ReadBgra8888("Image/format-Penguins.webp").Width == 600);
            Assert.IsTrue(ImageIO.ReadBgra8888("Image/format-Penguins.tif").Width == 600);
            Assert.IsTrue(ImageIO.ReadBgra8888("Image/animated-pattern.gif").Width == 500);
        }
    }
}