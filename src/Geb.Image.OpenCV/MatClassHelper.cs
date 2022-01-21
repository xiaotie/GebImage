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
            Mat mat = new Mat(img.Height, img.Width,MatType.CV_8U, img.StartIntPtr, img.Stride );
            return mat;
        }

        public unsafe static Mat ToCVMat(this ImageBgr24 img)
        {
            Mat mat = new Mat(img.Height, img.Width, MatType.CV_8UC3, img.StartIntPtr, img.Stride);
            return mat;
        }

        public unsafe static ImageU8 ToImageU8(this Mat mat)
        {
            if (mat == null) return null;
            if (mat.ElemSize() != 1) throw new ArgumentException("Element size is not 1");
            ImageU8 img = new ImageU8(mat.Width, mat.Height);
            long srcStride = mat.Step();
            long dstStride = img.Stride;
            Byte* src0 = mat.DataPointer;
            Byte* dst0 = img.Start;

            if(srcStride == dstStride)
            {
                // 整体 copy
                int bytes = (int)(srcStride * img.Height);
                Span<Byte> spanSrc = new Span<byte>(src0, bytes);
                Span<Byte> spanDst = new Span<byte>(dst0, bytes);
                spanSrc.CopyTo(spanDst);
            }
            else
            {
                // 逐行 copy
                int lineBytes = img.Width;
                for (int r = 0; r < img.Height; r++)
                {
                    Span<Byte> spanSrc = new Span<byte>(src0, lineBytes);
                    Span<Byte> spanDst = new Span<byte>(dst0, lineBytes);
                    spanSrc.CopyTo(spanDst);
                    src0 += srcStride;
                    dst0 += dstStride;
                }
            }
            return img;
        }
    }
}
