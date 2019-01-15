using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    public class ImageConverter
    {
        protected ImageU8 _imageU8;
        protected ImageBgra32 _imageBgra32;

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
    }

    public class ImageU8Converter : ImageConverter
    {
        public ImageU8Converter(ImageU8 image)
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

    public class ImageBgra32Converter : ImageConverter
    {
        public ImageBgra32Converter(ImageBgra32 image)
        {
            this._imageBgra32 = image;
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
