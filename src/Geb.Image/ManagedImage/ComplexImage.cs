// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright ?Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Image
{
    /// <summary>
    /// Complex image.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to keep image represented in complex numbers sutable for Fourier
    /// transformations.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create complex image
    /// ComplexImage complexImage = ComplexImage.FromBitmap( image );
    /// // do forward Fourier transformation
    /// complexImage.ForwardFourierTransform( );
    /// // get complex image as bitmat
    /// Bitmap fourierImage = complexImage.ToBitmap( );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample3.jpg" width="256" height="256" />
    /// <para><b>Fourier image:</b></para>
    /// <img src="img/imaging/fourier.jpg" width="256" height="256" />
    /// </remarks>
    /// 
    public class ImageComplex : ICloneable
    {
        // image complex data
        private Complex[,] data;
        // image dimension
        private int width;
        private int height;
        // current state of the image (transformed with Fourier ot not)
        private bool fourierTransformed = false;

        /// <summary>
        /// Image width.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Image height.
        /// </summary>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Status of the image - Fourier transformed or not.
        /// </summary>
        /// 
        public bool FourierTransformed
        {
            get { return fourierTransformed; }
        }

        /// <summary>
        /// Complex image's data.
        /// </summary>
        /// 
        /// <remarks>Return's 2D array of [<b>height</b>, <b>width</b>] size, which keeps image's
        /// complex data.</remarks>
        /// 
        public Complex[,] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageComplex"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <remarks>The constractor is protected, what makes it imposible to instantiate this
        /// class directly. To create an instance of this class <see cref="FromBitmap(Bitmap)"/> or
        /// <see cref="FromBitmap(BitmapData)"/> method should be used.</remarks>
        ///
        protected ImageComplex(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new Complex[height, width];
            this.fourierTransformed = false;
        }

        /// <summary>
        /// Clone the complex image.
        /// </summary>
        /// 
        /// <returns>Returns copy of the complex image.</returns>
        /// 
        public object Clone()
        {
            // create new complex image
            ImageComplex dstImage = new ImageComplex(width, height);
            Complex[,] data = dstImage.data;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    data[i, j] = this.data[i, j];
                }
            }

            // clone mode as well
            dstImage.fourierTransformed = fourierTransformed;

            return dstImage;
        }

        public static ImageComplex CreateFrom(ImageU8 imageData)
        {
            // get source image size
            int width = imageData.Width;
            int height = imageData.Height;
            int offset = 0;

            // check image size
            if ((!Tools.IsPowerOf2(width)) || (!Tools.IsPowerOf2(height)))
            {
                throw new NotSupportedException("Image width and height should be power of 2.");
            }

            // create new complex image
            ImageComplex complexImage = new ImageComplex(width, height);
            Complex[,] data = complexImage.data;

            // do the job
            unsafe
            {
                byte* src = (byte*)imageData.Start;

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++)
                    {
                        data[y, x].Re = (float)*src / 255;
                    }
                    src += offset;
                }
            }

            return complexImage;
        }

        public ImageU8 ToImage()
        {
            ImageU8 img = new ImageU8(width, height);
            int offset = 0;
            double scale = (fourierTransformed) ? Math.Sqrt(width * height) : 1;

            unsafe
            {
                byte* dst = (byte*)img.Start;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        *dst = (byte)System.Math.Max(0, System.Math.Min(255, data[y, x].Magnitude * scale * 255));
                    }
                    dst += offset;
                }
            }

            return img;
        }

        /// <summary>
        /// Applies forward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void ForwardFourierTransform()
        {
            if (!fourierTransformed)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                        {
                            data[y, x].Re *= -1;
                            data[y, x].Im *= -1;
                        }
                    }
                }

                FourierTransform.FFT2(data, FourierTransform.Direction.Forward);
                fourierTransformed = true;
            }
        }

        /// <summary>
        /// Applies backward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void BackwardFourierTransform()
        {
            if (fourierTransformed)
            {
                FourierTransform.FFT2(data, FourierTransform.Direction.Backward);
                fourierTransformed = false;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                        {
                            data[y, x].Re *= -1;
                            data[y, x].Im *= -1;
                        }
                    }
                }
            }
        }
    }
}
