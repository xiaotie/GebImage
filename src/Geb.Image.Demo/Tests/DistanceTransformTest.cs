using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geb.Image.Demo.Tests
{
    using Geb.Image;

    public class DistanceTransformTest
    {
        public void Run()
        {
            ImageU8 img = new ImageU8(800, 800);
            img.Fill(0);
            img.FillCircle(200, 200, 0xFF, 140);
            img.Fill(500, 500, 200, 200, 0xFF);
            img.ShowDialog();
            img.ApplyDistanceTransformFast();
            img.ShowDialog();
        }
    }
}
