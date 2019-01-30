using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    public abstract class ImageHolder : IDisposable
    {
        public static ImageHolder Create(ImageU8 image)
        {
            return new ImageU8Holder(image);
        }

        public static ImageHolder Create(ImageBgra32 image)
        {
            return new ImageBgra32Holder(image);
        }

        public int Width => GetRawImage().Width;
        public int Height => GetRawImage().Height;

        protected ImageU8 _imageU8;
        protected ImageBgra32 _imageBgra32;

        public abstract IImage GetRawImage();

        public ImageBgra32 GetImageBgra32()
        {
            if(_imageBgra32 == null && _imageU8 != null)
            {
                _imageBgra32 = _imageU8.ToImageBgra32();
            }
            return _imageBgra32;
        }

        public ImageU8 GetImageU8()
        {
            if(_imageU8 == null && _imageBgra32 != null)
            {
                _imageU8 = _imageBgra32.ToGrayscaleImage();
            }
            return _imageU8;
        }

        public virtual void Release()
        {
        }

        public void Dispose()
        {
            Release();
        }
    }

    public class ImageU8Holder : ImageHolder
    {
        public override IImage GetRawImage()
        {
            return this._imageU8;
        }

        public ImageU8Holder(ImageU8 image)
        {
            this._imageU8 = image;
        }

        public override void Release()
        {
            if(this._imageBgra32 != null)
            {
                _imageBgra32.Dispose();
                _imageBgra32 = null;
            }
        }
    }

    public class ImageBgra32Holder : ImageHolder
    {
        public ImageBgra32Holder(ImageBgra32 image)
        {
            this._imageBgra32 = image;
        }

        public override IImage GetRawImage()
        {
            return this._imageBgra32;
        }

        public override void Release()
        {
            if (this._imageU8 != null)
            {
                _imageU8.Dispose();
                _imageU8 = null;
            }
        }
    }
}
