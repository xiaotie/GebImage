/*************************************************************************
 *  Copyright (c) 2012 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  修改记录:
 * 
 ************************************************************************/

// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright ?Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image.Analysis
{
    public class HoughLine : IComparable
    {
        public readonly double Theta;

        public readonly short Radius;

        public readonly short Intensity;

        public readonly double RelativeIntensity;

        public HoughLine(double theta, short radius, short intensity, double relativeIntensity)
        {
            Theta = theta;
            Radius = radius;
            Intensity = intensity;
            RelativeIntensity = relativeIntensity;
        }

        public int CompareTo(object value)
        {
            return (-Intensity.CompareTo(((HoughLine)value).Intensity));
        }
    }

    public class HoughLineTransformation
    {
        // Hough transformation quality settings
        private int stepsPerDegree;
        private int houghHeight;
        private double thetaStep;

        // precalculated Sine and Cosine values
        private double[] sinMap;
        private double[] cosMap;
        // Hough map
        private short[,] houghMap;
        private short maxMapIntensity = 0;

        private int localPeakRadius = 4;
        private short minLineIntensity = 10;
        private ArrayList lines = new ArrayList();

        /// <summary>
        /// Steps per degree.
        /// </summary>
        /// 
        /// <remarks><para>The value defines quality of Hough line transformation and its ability to detect
        /// lines' slope precisely.</para>
        /// 
        /// <para>Default value is set to <b>1</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int StepsPerDegree
        {
            get { return stepsPerDegree; }
            set
            {
                stepsPerDegree = Math.Max(1, Math.Min(10, value));
                houghHeight = 180 * stepsPerDegree;
                thetaStep = Math.PI / houghHeight;

                // precalculate Sine and Cosine values
                sinMap = new double[houghHeight];
                cosMap = new double[houghHeight];

                for (int i = 0; i < houghHeight; i++)
                {
                    sinMap[i] = Math.Sin(i * thetaStep);
                    cosMap[i] = Math.Cos(i * thetaStep);
                }
            }
        }

        /// <summary>
        /// Minimum <see cref="HoughLine.Intensity">line's intensity</see> in Hough map to recognize a line.
        /// </summary>
        ///
        /// <remarks><para>The value sets minimum intensity level for a line. If a value in Hough
        /// map has lower intensity, then it is not treated as a line.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para></remarks>
        ///
        public short MinLineIntensity
        {
            get { return minLineIntensity; }
            set { minLineIntensity = value; }
        }

        /// <summary>
        /// Radius for searching local peak value.
        /// </summary>
        /// 
        /// <remarks><para>The value determines radius around a map's value, which is analyzed to determine
        /// if the map's value is a local maximum in specified area.</para>
        /// 
        /// <para>Default value is set to <b>4</b>. Minimum value is <b>1</b>. Maximum value is <b>10</b>.</para></remarks>
        /// 
        public int LocalPeakRadius
        {
            get { return localPeakRadius; }
            set { localPeakRadius = Math.Max(1, Math.Min(10, value)); }
        }

        /// <summary>
        /// Maximum found <see cref="HoughLine.Intensity">intensity</see> in Hough map.
        /// </summary>
        /// 
        /// <remarks><para>The property provides maximum found line's intensity.</para></remarks>
        /// 
        public short MaxIntensity
        {
            get { return maxMapIntensity; }
        }

        /// <summary>
        /// Found lines count.
        /// </summary>
        /// 
        /// <remarks><para>The property provides total number of found lines, which intensity is higher (or equal to),
        /// than the requested <see cref="MinLineIntensity">minimum intensity</see>.</para></remarks>
        /// 
        public int LinesCount
        {
            get { return lines.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HoughLineTransformation"/> class.
        /// </summary>
        /// 
        public HoughLineTransformation()
        {
            StepsPerDegree = 1;
        }

        /// <summary>
        /// Process an image building Hough map.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image to process.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage(ImageU8 image)
        {
            // get source image size
            int width = image.Width;
            int height = image.Height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            int toWidth = width - halfWidth;
            int toHeight = height - halfHeight;

            // calculate Hough map's width
            int halfHoughWidth = (int)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
            int houghWidth = halfHoughWidth * 2;

            houghMap = new short[houghHeight, houghWidth];

            // do the job
            unsafe
            {
                byte* src = (byte*)image.Start;

                // for each row
                for (int y = -halfHeight; y < toHeight; y++)
                {
                    // for each pixel
                    for (int x = -halfWidth; x < toWidth; x++, src++)
                    {
                        if (*src != 0)
                        {
                            // for each Theta value
                            for (int theta = 0; theta < houghHeight; theta++)
                            {
                                int radius = (int)Math.Round(cosMap[theta] * x - sinMap[theta] * y) + halfHoughWidth;

                                if ((radius < 0) || (radius >= houghWidth))
                                    continue;

                                houghMap[theta, radius]++;
                            }
                        }
                    }
                }
            }

            // find max value in Hough map
            maxMapIntensity = 0;
            for (int i = 0; i < houghHeight; i++)
            {
                for (int j = 0; j < houghWidth; j++)
                {
                    if (houghMap[i, j] > maxMapIntensity)
                    {
                        maxMapIntensity = houghMap[i, j];
                    }
                }
            }

            CollectLines();
        }

        /// <summary>
        /// Get specified amount of lines with highest <see cref="HoughLine.Intensity">intensity</see>.
        /// </summary>
        /// 
        /// <param name="count">Amount of lines to get.</param>
        /// 
        /// <returns>Returns array of most intesive lines. If there are no lines detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughLine[] GetMostIntensiveLines(int count)
        {
            // lines count
            int n = Math.Min(count, lines.Count);

            // result array
            HoughLine[] dst = new HoughLine[n];
            lines.CopyTo(0, dst, 0, n);

            return dst;
        }

        /// <summary>
        /// Get lines with <see cref="HoughLine.RelativeIntensity">relative intensity</see> higher then specified value.
        /// </summary>
        /// 
        /// <param name="minRelativeIntensity">Minimum relative intesity of lines.</param>
        /// 
        /// <returns>Returns array of lines. If there are no lines detected,
        /// the returned array has zero length.</returns>
        /// 
        public HoughLine[] GetLinesByRelativeIntensity(double minRelativeIntensity)
        {
            int count = 0, n = lines.Count;

            while ((count < n) && (((HoughLine)lines[count]).RelativeIntensity >= minRelativeIntensity))
                count++;

            return GetMostIntensiveLines(count);
        }


        // Collect lines with intesities greater or equal then specified
        private void CollectLines()
        {
            int maxTheta = houghMap.GetLength(0);
            int maxRadius = houghMap.GetLength(1);

            short intensity;
            bool foundGreater;

            int halfHoughWidth = maxRadius >> 1;

            // clean lines collection
            lines.Clear();

            // for each Theta value
            for (int theta = 0; theta < maxTheta; theta++)
            {
                // for each Radius value
                for (int radius = 0; radius < maxRadius; radius++)
                {
                    // get current value
                    intensity = houghMap[theta, radius];

                    if (intensity < minLineIntensity)
                        continue;

                    foundGreater = false;

                    // check neighboors
                    for (int tt = theta - localPeakRadius, ttMax = theta + localPeakRadius; tt < ttMax; tt++)
                    {
                        // break if it is not local maximum
                        if (foundGreater == true)
                            break;

                        int cycledTheta = tt;
                        int cycledRadius = radius;

                        // check limits
                        if (cycledTheta < 0)
                        {
                            cycledTheta = maxTheta + cycledTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }
                        if (cycledTheta >= maxTheta)
                        {
                            cycledTheta -= maxTheta;
                            cycledRadius = maxRadius - cycledRadius;
                        }

                        for (int tr = cycledRadius - localPeakRadius, trMax = cycledRadius + localPeakRadius; tr < trMax; tr++)
                        {
                            // skip out of map values
                            if (tr < 0)
                                continue;
                            if (tr >= maxRadius)
                                break;

                            // compare the neighboor with current value
                            if (houghMap[cycledTheta, tr] > intensity)
                            {
                                foundGreater = true;
                                break;
                            }
                        }
                    }

                    // was it local maximum ?
                    if (!foundGreater)
                    {
                        // we have local maximum
                        lines.Add(new HoughLine((double)theta / stepsPerDegree, (short)(radius - halfHoughWidth), intensity, (double)intensity / maxMapIntensity));
                    }
                }
            }

            lines.Sort();
        }
    }
}
