using System;
using System.Runtime.CompilerServices;

namespace Geb.Image.Formats.Jpeg.Components.Encoder
{
    /// <summary>
    /// On-stack worker struct to efficiently encapsulate the TPixel -> Rgb24 -> YCbCr conversion chain of 8x8 pixel blocks.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type to work on</typeparam>
    internal unsafe struct YCbCrForwardConverter
    {
        /// <summary>
        /// The Y component
        /// </summary>
        public Block8x8F Y;

        /// <summary>
        /// The Cb component
        /// </summary>
        public Block8x8F Cb;

        /// <summary>
        /// The Cr component
        /// </summary>
        public Block8x8F Cr;

        /// <summary>
        /// The color conversion tables
        /// </summary>
        private RgbToYCbCrTables colorTables;

        /// <summary>
        /// Temporal 8x8 block to hold TPixel data
        /// </summary>
        private GenericBlock8x8<Bgra32> pixelBlock;

        /// <summary>
        /// Temporal 8x8 block to hold TPixel data
        /// </summary>
        private GenericBlock8x8<Bgr24> pixelBgr24Block;

        /// <summary>
        /// Temporal RGB block
        /// </summary>
        private GenericBlock8x8<Bgr24> rgbBlock;

        public static YCbCrForwardConverter Create()
        {
            var result = default(YCbCrForwardConverter);
            result.colorTables = RgbToYCbCrTables.Create();
            return result;
        }

        /// <summary>
        /// Converts a 8x8 image area inside 'pixels' at position (x,y) placing the result members of the structure (<see cref="Y"/>, <see cref="Cb"/>, <see cref="Cr"/>)
        /// </summary>
        public void Convert(ImageBgra32 pixels, int x, int y)
        {
            this.pixelBlock.LoadAndStretchEdges(ref Unsafe.AsRef<Bgra32>((void*)pixels.Start), pixels.Width, pixels.Height, x, y);

            Span<Bgr24> rgbSpan = this.rgbBlock.AsSpanUnsafe();
            var pixelSpan = this.pixelBlock.AsSpanUnsafe();
            
            for(int i = 0; i < 64; i++)
            {
                rgbSpan[i].Red = pixelSpan[i].Red;
                rgbSpan[i].Green = pixelSpan[i].Green;
                rgbSpan[i].Blue = pixelSpan[i].Blue;
            }

            ref float yBlockStart = ref Unsafe.As<Block8x8F, float>(ref this.Y);
            ref float cbBlockStart = ref Unsafe.As<Block8x8F, float>(ref this.Cb);
            ref float crBlockStart = ref Unsafe.As<Block8x8F, float>(ref this.Cr);
            ref Bgr24 rgbStart = ref rgbSpan[0];

            for (int i = 0; i < 64; i++)
            {
                ref Bgr24 c = ref Unsafe.Add(ref rgbStart, i);

                this.colorTables.ConvertPixelInto(
                    c.Red,
                    c.Green,
                    c.Blue,
                    ref Unsafe.Add(ref yBlockStart, i),
                    ref Unsafe.Add(ref cbBlockStart, i),
                    ref Unsafe.Add(ref crBlockStart, i));
            }
        }

        /// <summary>
        /// Converts a 8x8 image area inside 'pixels' at position (x,y) placing the result members of the structure (<see cref="Y"/>, <see cref="Cb"/>, <see cref="Cr"/>)
        /// </summary>
        public void Convert(ImageBgr24 pixels, int x, int y)
        {
            this.pixelBgr24Block.LoadAndStretchEdges(ref Unsafe.AsRef<Bgr24>((void*)pixels.Start), pixels.Width, pixels.Height, x, y);

            Span<Bgr24> rgbSpan = this.rgbBlock.AsSpanUnsafe();
            var pixelSpan = this.pixelBgr24Block.AsSpanUnsafe();

            for (int i = 0; i < 64; i++)
            {
                rgbSpan[i].Red = pixelSpan[i].Red;
                rgbSpan[i].Green = pixelSpan[i].Green;
                rgbSpan[i].Blue = pixelSpan[i].Blue;
            }

            ref float yBlockStart = ref Unsafe.As<Block8x8F, float>(ref this.Y);
            ref float cbBlockStart = ref Unsafe.As<Block8x8F, float>(ref this.Cb);
            ref float crBlockStart = ref Unsafe.As<Block8x8F, float>(ref this.Cr);
            ref Bgr24 rgbStart = ref rgbSpan[0];

            for (int i = 0; i < 64; i++)
            {
                ref Bgr24 c = ref Unsafe.Add(ref rgbStart, i);

                this.colorTables.ConvertPixelInto(
                    c.Red,
                    c.Green,
                    c.Blue,
                    ref Unsafe.Add(ref yBlockStart, i),
                    ref Unsafe.Add(ref cbBlockStart, i),
                    ref Unsafe.Add(ref crBlockStart, i));
            }
        }
    }
}