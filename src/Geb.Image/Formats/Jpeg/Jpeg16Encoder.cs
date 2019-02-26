using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    using Geb.Image.Formats.Jpeg.Components;
    using Geb.Image.Formats.Jpeg.Components.Encoder;

    internal abstract class Jpeg16Encoder : BaseJpegEncoder
    {
        public Jpeg16Encoder(int quality = 50) : base(quality, JpegPixelFormats.Gray)
        {
        }
    }

    internal unsafe class Jpeg16GrayEncoder : Jpeg16Encoder
    {
        protected override byte[] GetSosHeader()
        {
            return BaseJpegEncoder.SosHeaderGray;
        }

        protected override void WriteImageData(ImageHolder image)
        {
            ImageU8 pixels = image.GetImageU8();

            Block8x8F temp1 = default;
            Block8x8F temp2 = default;

            Block8x8F onStackLuminanceQuantTable = this.luminanceQuantTable;
            var unzig = ZigZag.CreateUnzigTable();
            GrayForwardConverter pixelConverter = GrayForwardConverter.Create();

            int prevDCY = 0;
            for (int y = 0; y < pixels.Height; y += 8)
            {
                for (int x = 0; x < pixels.Width; x += 8)
                {
                    pixelConverter.Convert(pixels, x, y);
                    prevDCY = this.WriteBlock(
                        QuantIndex.Luminance,
                        prevDCY,
                        &pixelConverter.Y,
                        &temp1,
                        &temp2,
                        &onStackLuminanceQuantTable,
                        unzig.Data);
                }
            }
        }
    }
}
