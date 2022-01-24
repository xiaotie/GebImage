using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Geb.Image.Test
{
    using Geb.Image.Analysis;

    [TestClass]
    public class ConvexHullTest
    {
        [TestMethod]
        public void TestMinAreaRect()
        {
            PointF[] points = new PointF[] { new PointF(10.1f, 10.3f), new PointF(23.4f, 15.9f), new PointF(14.2f, 32.9f), new PointF(25.9f, 32.1f) };
            RotatedRectF rect = ConvexHull.MinAreaRect(points);
            Assert.AreEqual(true, rect.Size.Width > 1);
        }
    }
}
