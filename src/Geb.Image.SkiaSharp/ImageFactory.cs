namespace Geb.Image.Skia
{
    using SkiaSharp;
    using BitMiracle.LibTiff.Classic;

    public class ImageFactory
    {
        public static unsafe ImageBgra32 ReadBgra8888(String imageFilePath)
        {
            var rtn = ReadImage<ImageBgra32>(imageFilePath, SKColorType.Bgra8888, (w, h) => new ImageBgra32(w, h));
            if (rtn == null) rtn = DecodeTiff(imageFilePath);
            if (rtn == null) throw new FileDecodeException(imageFilePath);
            return rtn;
        }

        public static unsafe ImageU8 ReadGray8(String imageFilePath)
        {
            var rtn = ReadImage<ImageU8>(imageFilePath, SKColorType.Gray8, (w, h) => new ImageU8(w, h));
            if(rtn == null)
            {
                ImageBgra32 img = DecodeTiff(imageFilePath);
                if(img != null)
                {
                    rtn = img.ToGrayscaleImage();
                    img.Dispose();
                }    
            }
            if (rtn == null) throw new FileDecodeException(imageFilePath);
            return rtn;
        }

        private static unsafe TImage ReadImage<TImage>(String imageFilePath, SKColorType colorType, Func<int, int, TImage> createFunc) where TImage : IImage
        {
            SKBitmap bmp = Decode(imageFilePath, colorType);
            if (bmp == null) return default(TImage);

            TImage im = createFunc(bmp.Width, bmp.Height);
            void* Data = (void*)bmp.GetPixels();
            im.CopyFrom(Data, bmp.RowBytes);
            bmp.Dispose();
            return im;
        }

        private static SKBitmap Decode(String imageFilePath, SKColorType colorType)
        {
            SKBitmap bmp = null;
            using (FileStream fs = new FileStream(imageFilePath, FileMode.Open))
            {
                bmp = SKBitmap.Decode(fs);
            }
            if (bmp == null || bmp.ColorType == colorType) return bmp;

            SKBitmap newBmp = null;
            if (bmp.CanCopyTo(colorType))
            {
                newBmp = bmp.Copy(colorType);
            }
            else
            {
                newBmp = new SKBitmap(bmp.Width, bmp.Height, bmp.ColorType, bmp.AlphaType);
                using (SKCanvas canvas = new SKCanvas(newBmp))
                {
                    canvas.DrawBitmap(bmp, new SKPoint(0, 0));
                }
            }
            bmp.Dispose();
            return newBmp;
        }

        public static ImageBgra32 DecodeTiff(String imageFilePath)
        {
            if (imageFilePath.EndsWith(".tiff") || imageFilePath.EndsWith(".tif")) // tif 文件
            {
                var bytes = File.ReadAllBytes(imageFilePath);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    ImageBgra32 imImff = DecodeTiff(ms);
                    return imImff;
                }
            }
            return null;
        }

        public static ImageBgra32 DecodeTiff(MemoryStream tifImage)
        {
            int width, height;
            // open a Tiff stored in the memory stream, and grab its pixels
            using (Tiff tifImg = Tiff.ClientOpen("in-memory", "r", tifImage, new TiffStream()))
            {
                FieldValue[] value = tifImg.GetField(TiffTag.IMAGEWIDTH);
                width = value[0].ToInt();

                value = tifImg.GetField(TiffTag.IMAGELENGTH);
                height = value[0].ToInt();

                // Read the image into the memory buffer 
                int[] raster = new int[width * height];
                if (!tifImg.ReadRGBAImageOriented(width, height, raster, Orientation.TOPLEFT))
                {
                    return null;
                }

                ImageBgra32 im = new ImageBgra32(width, height);
                Span<Bgra32> span = im.Span;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int arrayOffset = y * width + x;
                        int rgba = raster[arrayOffset];
                        Bgra32 c = new Bgra32((byte)Tiff.GetB(rgba), (byte)Tiff.GetG(rgba), (byte)Tiff.GetR(rgba), (byte)Tiff.GetA(rgba));
                        span[arrayOffset] = c;
                    }
                }
                return im;
            }
        }
    }
}