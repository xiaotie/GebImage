using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Jpeg
{
    using Geb.Image.Formats.Jpeg.Components;
    using Geb.Image.Formats.Jpeg.Components.Encoder;

    internal abstract class Jpeg8Encoder : BaseJpegEncoder
    {
        public Jpeg8Encoder(int quality = 50, JpegPixelFormats fmt = JpegPixelFormats.YCbCr):base(quality, fmt)
        {
        }
    }

    internal unsafe class Jpeg8GrayEncoder : Jpeg8Encoder
    {
        public Jpeg8GrayEncoder(int quality = 50):base(quality, JpegPixelFormats.Gray)
        {
        }

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

    internal unsafe class Jpeg8YCbCrEncoder : Jpeg8Encoder
    {
        public Jpeg8YCbCrEncoder(int quality = 50) : base(quality, JpegPixelFormats.YCbCr)
        {
        }

        protected override byte[] GetSosHeader()
        {
            return BaseJpegEncoder.SosHeaderYCbCr;
        }

        protected override void WriteImageData(ImageHolder image)
        {
            if(image is ImageBgra32Holder)
            {
                ImageBgra32 imageRaw = image.GetImageBgra32();
                switch (this.subsample)
                {
                    case JpegSubsample.Ratio444:
                        this.Encode444(imageRaw);
                        break;
                    case JpegSubsample.Ratio420:
                        this.Encode420(imageRaw);
                        break;
                }
            }
            else
            {
                ImageBgr24 imageRaw = image.GetImageBgr24();
                switch (this.subsample)
                {
                    case JpegSubsample.Ratio444:
                        this.Encode444(imageRaw);
                        break;
                    case JpegSubsample.Ratio420:
                        this.Encode420(imageRaw);
                        break;
                }
            }
        }
        /// <summary>
        /// Encodes the image with no subsampling.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The pixel accessor providing access to the image pixels.</param>
        private void Encode444(ImageBgra32 pixels)
        {
            // TODO: Need a JpegScanEncoder<TPixel> class or struct that encapsulates the scan-encoding implementation. (Similar to JpegScanDecoder.)
            // (Partially done with YCbCrForwardConverter<TPixel>)
            Block8x8F temp1 = default;
            Block8x8F temp2 = default;

            Block8x8F onStackLuminanceQuantTable = this.luminanceQuantTable;
            Block8x8F onStackChrominanceQuantTable = this.chrominanceQuantTable;

            var unzig = ZigZag.CreateUnzigTable();

            // ReSharper disable once InconsistentNaming
            int prevDCY = 0, prevDCCb = 0, prevDCCr = 0;

            var pixelConverter = YCbCrForwardConverter.Create();

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

                    prevDCCb = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCb,
                        &pixelConverter.Cb,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);

                    prevDCCr = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCr,
                        &pixelConverter.Cr,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);
                }
            }
        }

        private void Encode444(ImageBgr24 pixels)
        {
            // TODO: Need a JpegScanEncoder<TPixel> class or struct that encapsulates the scan-encoding implementation. (Similar to JpegScanDecoder.)
            // (Partially done with YCbCrForwardConverter<TPixel>)
            Block8x8F temp1 = default;
            Block8x8F temp2 = default;

            Block8x8F onStackLuminanceQuantTable = this.luminanceQuantTable;
            Block8x8F onStackChrominanceQuantTable = this.chrominanceQuantTable;

            var unzig = ZigZag.CreateUnzigTable();

            // ReSharper disable once InconsistentNaming
            int prevDCY = 0, prevDCCb = 0, prevDCCr = 0;

            var pixelConverter = YCbCrForwardConverter.Create();

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

                    prevDCCb = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCb,
                        &pixelConverter.Cb,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);

                    prevDCCr = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCr,
                        &pixelConverter.Cr,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);
                }
            }
        }

        /// <summary>
        /// Encodes the image with subsampling. The Cb and Cr components are each subsampled
        /// at a factor of 2 both horizontally and vertically.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="pixels">The pixel accessor providing access to the image pixels.</param>
        private void Encode420(ImageBgra32 pixels)
        {
            // TODO: Need a JpegScanEncoder<TPixel> class or struct that encapsulates the scan-encoding implementation. (Similar to JpegScanDecoder.)
            Block8x8F b = default;

            BlockQuad cb = default;
            BlockQuad cr = default;
            var cbPtr = (Block8x8F*)cb.Data;
            var crPtr = (Block8x8F*)cr.Data;

            Block8x8F temp1 = default;
            Block8x8F temp2 = default;

            Block8x8F onStackLuminanceQuantTable = this.luminanceQuantTable;
            Block8x8F onStackChrominanceQuantTable = this.chrominanceQuantTable;

            var unzig = ZigZag.CreateUnzigTable();

            var pixelConverter = YCbCrForwardConverter.Create();

            // ReSharper disable once InconsistentNaming
            int prevDCY = 0, prevDCCb = 0, prevDCCr = 0;

            for (int y = 0; y < pixels.Height; y += 16)
            {
                for (int x = 0; x < pixels.Width; x += 16)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int xOff = (i & 1) * 8;
                        int yOff = (i & 2) * 4;

                        pixelConverter.Convert(pixels, x + xOff, y + yOff);

                        cbPtr[i] = pixelConverter.Cb;
                        crPtr[i] = pixelConverter.Cr;

                        prevDCY = this.WriteBlock(
                            QuantIndex.Luminance,
                            prevDCY,
                            &pixelConverter.Y,
                            &temp1,
                            &temp2,
                            &onStackLuminanceQuantTable,
                            unzig.Data);
                    }

                    Block8x8F.Scale16X16To8X8(&b, cbPtr);
                    prevDCCb = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCb,
                        &b,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);

                    Block8x8F.Scale16X16To8X8(&b, crPtr);
                    prevDCCr = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCr,
                        &b,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);
                }
            }
        }

        private void Encode420(ImageBgr24 pixels)
        {
            // TODO: Need a JpegScanEncoder<TPixel> class or struct that encapsulates the scan-encoding implementation. (Similar to JpegScanDecoder.)
            Block8x8F b = default;

            BlockQuad cb = default;
            BlockQuad cr = default;
            var cbPtr = (Block8x8F*)cb.Data;
            var crPtr = (Block8x8F*)cr.Data;

            Block8x8F temp1 = default;
            Block8x8F temp2 = default;

            Block8x8F onStackLuminanceQuantTable = this.luminanceQuantTable;
            Block8x8F onStackChrominanceQuantTable = this.chrominanceQuantTable;

            var unzig = ZigZag.CreateUnzigTable();

            var pixelConverter = YCbCrForwardConverter.Create();

            // ReSharper disable once InconsistentNaming
            int prevDCY = 0, prevDCCb = 0, prevDCCr = 0;

            for (int y = 0; y < pixels.Height; y += 16)
            {
                for (int x = 0; x < pixels.Width; x += 16)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int xOff = (i & 1) * 8;
                        int yOff = (i & 2) * 4;

                        pixelConverter.Convert(pixels, x + xOff, y + yOff);

                        cbPtr[i] = pixelConverter.Cb;
                        crPtr[i] = pixelConverter.Cr;

                        prevDCY = this.WriteBlock(
                            QuantIndex.Luminance,
                            prevDCY,
                            &pixelConverter.Y,
                            &temp1,
                            &temp2,
                            &onStackLuminanceQuantTable,
                            unzig.Data);
                    }

                    Block8x8F.Scale16X16To8X8(&b, cbPtr);
                    prevDCCb = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCb,
                        &b,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);

                    Block8x8F.Scale16X16To8X8(&b, crPtr);
                    prevDCCr = this.WriteBlock(
                        QuantIndex.Chrominance,
                        prevDCCr,
                        &b,
                        &temp1,
                        &temp2,
                        &onStackChrominanceQuantTable,
                        unzig.Data);
                }
            }
        }
    }
}
