//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;

//namespace Geb.Image
//{
//    public class GdiplusHelper
//    {
//        private static ImageCodecInfo _jpegCodecInfo;
//        private static ImageCodecInfo _pngCodecInfo;

//        internal static ImageCodecInfo GetJpegEncoder()
//        {
//            if (_jpegCodecInfo != null) return _jpegCodecInfo;
//            _jpegCodecInfo = GetEncoderInfo("image/jpeg");
//            return _jpegCodecInfo;
//        }

//        internal static ImageCodecInfo GetPngEncoder()
//        {
//            if (_pngCodecInfo != null) return _pngCodecInfo;
//            _pngCodecInfo = GetEncoderInfo("image/png");
//            return _pngCodecInfo;
//        }

//        internal static EncoderParameters GetJpegEncoderParameters(int quality)
//        {
//            EncoderParameters ecParams = new EncoderParameters(1);
//            EncoderParameter myEncoderParameter = new EncoderParameter(Encoder.Quality, quality);
//            ecParams.Param[0] = myEncoderParameter;
//            return ecParams;
//        }

//        internal static ImageCodecInfo GetEncoderInfo(string mineType)
//        {
//            ImageCodecInfo[] myEncoders = ImageCodecInfo.GetImageEncoders();

//            foreach (ImageCodecInfo ec in myEncoders)
//                if (ec.MimeType == mineType)
//                    return ec;
//            return null;
//        }
//    }
//}
