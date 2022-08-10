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

        public static ImageHolder Create(ImageBgr24 image)
        {
            return new ImageBgr24Holder(image);
        }

        public int Width => GetRawImage().Width;
        public int Height => GetRawImage().Height;

        protected ImageU8 _imageU8;
        protected ImageBgra32 _imageBgra32;
        protected ImageBgr24 _imageBgr24;

        public abstract IImage GetRawImage();

        public ImageBgra32 GetImageBgra32()
        {
            if(_imageBgra32 == null && _imageU8 != null)
            {
                _imageBgra32 = _imageU8.ToImageBgra32();
            }
            return _imageBgra32;
        }

        public ImageBgr24 GetImageBgr24()
        {
            if (_imageBgr24 == null && _imageU8 != null)
            {
                _imageBgr24 = _imageU8.ToImageBgr24();
            }
            return _imageBgr24;
        }

        public ImageU8 GetImageU8()
        {
            if(_imageU8 == null && _imageBgra32 != null)
            {
                _imageU8 = _imageBgra32.ToGrayscaleImage();
            }
            return _imageU8;
        }

        public void Release()
        {
            // 对于不是 raw image 的
            var rawImg = this.GetRawImage();

            if (this._imageU8 != null && this._imageU8 != rawImg)
            {
                _imageU8.Dispose();
                _imageU8 = null;
            }

            if (this._imageBgr24 != null && this._imageBgr24 != rawImg)
            {
                _imageBgr24.Dispose();
                _imageBgr24 = null;
            }

            if (this._imageBgra32 != null && this._imageBgra32 != rawImg)
            {
                _imageBgra32.Dispose();
                _imageBgra32 = null;
            }
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
    }

    public class ImageBgr24Holder : ImageHolder
    {
        public ImageBgr24Holder(ImageBgr24 image)
        {
            this._imageBgr24 = image;
        }

        public override IImage GetRawImage()
        {
            return this._imageBgr24;
        }
    }
}
