using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Geb.Image.Test
{
    [TestClass]
    public class ImageU8Test
    {
        [TestMethod]
        public void TestCreateIntegral()
        {
            using (ImageU8 img = new ImageU8(3, 3))
            {
                img.Fill(1);
                using (ImageInt32 imgInt = img.CreateIntegral())
                {
                    Assert.AreEqual(3, imgInt[0, 2]);
                    Assert.AreEqual(2, imgInt[1, 0]);
                    Assert.AreEqual(9, imgInt[2, 2]);
                }
            }
        }
    }
}
