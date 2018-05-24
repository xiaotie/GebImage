using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats.Quantization
{
    /// <summary>
    /// Provides methods for allowing quantization of images pixels with configurable dithering.
    /// </summary>
    public interface IQuantizer
    {
        ///// <summary>
        ///// Gets the error diffusion algorithm to apply to the output image.
        ///// </summary>
        //IErrorDiffuser Diffuser { get; }

        ///// <summary>
        ///// Creates the generic frame quantizer
        ///// </summary>
        ///// <typeparam name="TPixel">The pixel format.</typeparam>
        ///// <returns>The <see cref="IFrameQuantizer{TPixel}"/></returns>
        //IFrameQuantizer<TPixel> CreateFrameQuantizer<TPixel>()
        //    where TPixel : struct, IPixel<TPixel>;
    }
}
