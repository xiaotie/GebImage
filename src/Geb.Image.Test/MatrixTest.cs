using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Geb.Image.Test
{
    [TestClass]
    public class MatrixTest
    {
        public static Boolean AreEqual(PointD p0,PointD p1)
        {
            const double eps = 0.000001;
            return Math.Abs(p1.X - p0.X) < eps && Math.Abs(p1.Y - p0.Y) < eps;
        }

        [TestMethod]
        public void TestMultify()
        {
            Matrix m0 = new Matrix(2, 6, 3, 4, 1, 1);
            Matrix m1 = new Matrix(7, 2, 3, 5, -2, -2);
            Matrix m2 = m0 * m1;
            Matrix m = new Matrix(32,34,33,26,-15,-13);
            Assert.AreEqual(m2,m);
        }

        [TestMethod]
        public void TestRotation()
        {
            {
                Matrix m0 = Matrix.CreateRotationMatrix(Math.PI * 0.5);
                PointD p0 = new PointD(1, 0);
                PointD p1 = m0 * p0;
                PointD p = new PointD(0, 1);
                Assert.AreEqual(true, AreEqual(p,p1));
            }

            {
                Matrix m = Matrix.CreateRotationMatrix(new PointD(2, 1), Math.PI * 0.5);
                PointD p = new PointD(3, 1);
                p = m * p;
                PointD pResult = new PointD(2, 2);
                Assert.AreEqual(true, AreEqual(p,pResult));
            }
           
        }
    }
}
