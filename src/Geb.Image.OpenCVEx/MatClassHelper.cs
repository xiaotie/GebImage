using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;

namespace Geb.Image
{
    public static class MatClassHelper
    {
        public unsafe static Mat ToCVMat(this ImageU8 img)
        {
            Mat mat = new Mat(new OpenCvSharp.Size(img.Width, img.Height), MatType.CV_8U);
            new Mat(img.Height, img.Width, MatType.CV_8U, img.StartIntPtr, img.Width);
            return mat;
        }
    }
}
