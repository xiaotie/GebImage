using System;
using System.Runtime.CompilerServices;

namespace Geb.Image.Formats.Jpeg.Components.Encoder
{
    internal unsafe struct GrayForwardConverter
    {
        /// <summary>
        /// The Y component
        /// </summary>
        public Block8x8F Y;
        
        /// <summary>
        /// Temporal 8x8 block to hold TPixel data
        /// </summary>
        private GenericBlock8x8<Byte> pixelBlock;

        public static GrayForwardConverter Create()
        {
            var result = default(GrayForwardConverter);
            return result;
        }

        /// <summary>
        /// Converts a 8x8 image area inside 'pixels' at position (x,y) placing the result members of the structure (<see cref="Y"/>, <see cref="Cb"/>, <see cref="Cr"/>)
        /// </summary>
        public void Convert(ImageU8 pixels, int x, int y)
        {
            this.pixelBlock.LoadAndStretchEdges(ref Unsafe.AsRef<Byte>((void*)pixels.Start), pixels.Width, pixels.Height, x, y);

            for (int i = 0; i < 64; i++)
            {
                this.Y[i] = pixelBlock[i];
            }
        }
    }
}
