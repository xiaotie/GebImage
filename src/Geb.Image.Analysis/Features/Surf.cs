using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Analysis
{
    using Geb.Image;

    public class SurfDescriptor
    {

        /// <summary>
        /// Gaussian distribution with sigma = 2.5.  Used as a fast lookup
        /// </summary>
        float[,] gauss25 = new float[7, 7] {
      {0.02350693969273f,0.01849121369071f,0.01239503121241f,0.00708015417522f,0.00344628101733f,0.00142945847484f,0.00050524879060f},
      {0.02169964028389f,0.01706954162243f,0.01144205592615f,0.00653580605408f,0.00318131834134f,0.00131955648461f,0.00046640341759f},
      {0.01706954162243f,0.01342737701584f,0.00900063997939f,0.00514124713667f,0.00250251364222f,0.00103799989504f,0.00036688592278f},
      {0.01144205592615f,0.00900063997939f,0.00603330940534f,0.00344628101733f,0.00167748505986f,0.00069579213743f,0.00024593098864f},
      {0.00653580605408f,0.00514124713667f,0.00344628101733f,0.00196854695367f,0.00095819467066f,0.00039744277546f,0.00014047800980f},
      {0.00318131834134f,0.00250251364222f,0.00167748505986f,0.00095819467066f,0.00046640341759f,0.00019345616757f,0.00006837798818f},
      {0.00131955648461f,0.00103799989504f,0.00069579213743f,0.00039744277546f,0.00019345616757f,0.00008024231247f,0.00002836202103f}
    };

        /// <summary>
        /// The integral image which is being used
        /// </summary>
        IntegralImage img;

        /// <summary>
        /// Static one-call do it all function
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ipts"></param>
        /// <param name="extended"></param>
        /// <param name="upright"></param>
        public static void DecribeInterestPoints(List<SurfPoint> ipts, bool upright, bool extended, IntegralImage img)
        {
            SurfDescriptor des = new SurfDescriptor();
            des.DescribeInterestPoints(ipts, upright, extended, img);
        }


        /// <summary>
        /// Build descriptor vector for each interest point in the supplied list
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ipts"></param>
        /// <param name="upright"></param>
        public void DescribeInterestPoints(List<SurfPoint> ipts, bool upright, bool extended, IntegralImage img)
        {
            if (ipts.Count == 0) return;
            this.img = img;

            foreach (SurfPoint ip in ipts)
            {
                // determine descriptor size
                if (extended) ip.descriptorLength = 128;
                else ip.descriptorLength = 64;

                // if we want rotation invariance get the orientation
                if (!upright) GetOrientation(ip);

                // Extract SURF descriptor
                GetDescriptor(ip, upright, extended);
            }
        }

        /// <summary>
        /// Determine dominant orientation for InterestPoint
        /// </summary>
        /// <param name="ip"></param>
        void GetOrientation(SurfPoint ip)
        {
            const byte Responses = 109;
            float[] resX = new float[Responses];
            float[] resY = new float[Responses];
            float[] Ang = new float[Responses];
            int idx = 0;
            int[] id = { 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5, 6 };

            // Get rounded InterestPoint data
            int X = (int)Math.Round(ip.x, 0);
            int Y = (int)Math.Round(ip.y, 0);
            int S = (int)Math.Round(ip.scale, 0);

            // calculate haar responses for points within radius of 6*scale
            for (int i = -6; i <= 6; ++i)
            {
                for (int j = -6; j <= 6; ++j)
                {
                    if (i * i + j * j < 36)
                    {
                        float gauss = gauss25[id[i + 6], id[j + 6]];
                        resX[idx] = gauss * img.HaarX(Y + j * S, X + i * S, 4 * S);
                        resY[idx] = gauss * img.HaarY(Y + j * S, X + i * S, 4 * S);
                        Ang[idx] = (float)GetAngle(resX[idx], resY[idx]);
                        ++idx;
                    }
                }
            }

            // calculate the dominant direction 
            float sumX, sumY, max = 0, orientation = 0;
            float ang1, ang2;
            float pi = (float)Math.PI;

            // loop slides pi/3 window around feature point
            for (ang1 = 0; ang1 < 2 * pi; ang1 += 0.15f)
            {
                ang2 = (ang1 + pi / 3f > 2 * pi ? ang1 - 5 * pi / 3f : ang1 + pi / 3f);
                sumX = sumY = 0;

                for (int k = 0; k < Responses; ++k)
                {
                    // determine whether the point is within the window
                    if (ang1 < ang2 && ang1 < Ang[k] && Ang[k] < ang2)
                    {
                        sumX += resX[k];
                        sumY += resY[k];
                    }
                    else if (ang2 < ang1 &&
                      ((Ang[k] > 0 && Ang[k] < ang2) || (Ang[k] > ang1 && Ang[k] < pi)))
                    {
                        sumX += resX[k];
                        sumY += resY[k];
                    }
                }

                // if the vector produced from this window is longer than all 
                // previous vectors then this forms the new dominant direction
                if (sumX * sumX + sumY * sumY > max)
                {
                    // store largest orientation
                    max = sumX * sumX + sumY * sumY;
                    orientation = (float)GetAngle(sumX, sumY);
                }
            }

            // assign orientation of the dominant response vector
            ip.orientation = (float)orientation;
        }


        /// <summary>
        /// Construct descriptor vector for this interest point
        /// </summary>
        /// <param name="bUpright"></param>
        void GetDescriptor(SurfPoint ip, bool bUpright, bool bExtended)
        {
            int sample_x, sample_y, count = 0;
            int i = 0, ix = 0, j = 0, jx = 0, xs = 0, ys = 0;
            float dx, dy, mdx, mdy, co, si;
            float dx_yn, mdx_yn, dy_xn, mdy_xn;
            float gauss_s1 = 0f, gauss_s2 = 0f;
            float rx = 0f, ry = 0f, rrx = 0f, rry = 0f, len = 0f;
            float cx = -0.5f, cy = 0f; //Subregion centers for the 4x4 gaussian weighting

            // Get rounded InterestPoint data
            int X = (int)Math.Round(ip.x, 0);
            int Y = (int)Math.Round(ip.y, 0);
            int S = (int)Math.Round(ip.scale, 0);

            // Allocate descriptor memory
            ip.SetDescriptorLength(64);

            if (bUpright)
            {
                co = 1;
                si = 0;
            }
            else
            {
                co = (float)Math.Cos(ip.orientation);
                si = (float)Math.Sin(ip.orientation);
            }

            //Calculate descriptor for this interest point
            i = -8;
            while (i < 12)
            {
                j = -8;
                i = i - 4;

                cx += 1f;
                cy = -0.5f;

                while (j < 12)
                {
                    cy += 1f;

                    j = j - 4;

                    ix = i + 5;
                    jx = j + 5;

                    dx = dy = mdx = mdy = 0f;
                    dx_yn = mdx_yn = dy_xn = mdy_xn = 0f;

                    xs = (int)Math.Round(X + (-jx * S * si + ix * S * co), 0);
                    ys = (int)Math.Round(Y + (jx * S * co + ix * S * si), 0);

                    // zero the responses
                    dx = dy = mdx = mdy = 0f;
                    dx_yn = mdx_yn = dy_xn = mdy_xn = 0f;

                    for (int k = i; k < i + 9; ++k)
                    {
                        for (int l = j; l < j + 9; ++l)
                        {
                            //Get coords of sample point on the rotated axis
                            sample_x = (int)Math.Round(X + (-l * S * si + k * S * co), 0);
                            sample_y = (int)Math.Round(Y + (l * S * co + k * S * si), 0);

                            //Get the gaussian weighted x and y responses
                            gauss_s1 = Gaussian(xs - sample_x, ys - sample_y, 2.5f * S);
                            rx = (float)img.HaarX(sample_y, sample_x, 2 * S);
                            ry = (float)img.HaarY(sample_y, sample_x, 2 * S);

                            //Get the gaussian weighted x and y responses on rotated axis
                            rrx = gauss_s1 * (-rx * si + ry * co);
                            rry = gauss_s1 * (rx * co + ry * si);


                            if (bExtended)
                            {
                                // split x responses for different signs of y
                                if (rry >= 0)
                                {
                                    dx += rrx;
                                    mdx += Math.Abs(rrx);
                                }
                                else
                                {
                                    dx_yn += rrx;
                                    mdx_yn += Math.Abs(rrx);
                                }

                                // split y responses for different signs of x
                                if (rrx >= 0)
                                {
                                    dy += rry;
                                    mdy += Math.Abs(rry);
                                }
                                else
                                {
                                    dy_xn += rry;
                                    mdy_xn += Math.Abs(rry);
                                }
                            }
                            else
                            {
                                dx += rrx;
                                dy += rry;
                                mdx += Math.Abs(rrx);
                                mdy += Math.Abs(rry);
                            }
                        }
                    }

                    //Add the values to the descriptor vector
                    gauss_s2 = Gaussian(cx - 2f, cy - 2f, 1.5f);

                    ip.descriptor[count++] = dx * gauss_s2;
                    ip.descriptor[count++] = dy * gauss_s2;
                    ip.descriptor[count++] = mdx * gauss_s2;
                    ip.descriptor[count++] = mdy * gauss_s2;

                    // add the extended components
                    if (bExtended)
                    {
                        ip.descriptor[count++] = dx_yn * gauss_s2;
                        ip.descriptor[count++] = dy_xn * gauss_s2;
                        ip.descriptor[count++] = mdx_yn * gauss_s2;
                        ip.descriptor[count++] = mdy_xn * gauss_s2;
                    }

                    len += (dx * dx + dy * dy + mdx * mdx + mdy * mdy
                            + dx_yn + dy_xn + mdx_yn + mdy_xn) * gauss_s2 * gauss_s2;

                    j += 9;
                }
                i += 9;
            }

            //Convert to Unit Vector
            len = (float)Math.Sqrt((double)len);
            if (len > 0)
            {
                for (int d = 0; d < ip.descriptorLength; ++d)
                {
                    ip.descriptor[d] /= len;
                }
            }
        }


        /// <summary>
        /// Get the angle formed by the vector [x,y]
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        double GetAngle(float X, float Y)
        {
            if (X >= 0 && Y >= 0)
                return Math.Atan(Y / X);
            else if (X < 0 && Y >= 0)
                return Math.PI - Math.Atan(-Y / X);
            else if (X < 0 && Y < 0)
                return Math.PI + Math.Atan(Y / X);
            else if (X >= 0 && Y < 0)
                return 2 * Math.PI - Math.Atan(-Y / X);

            return 0;
        }


        /// <summary>
        /// Get the value of the gaussian with std dev sigma
        /// at the point (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        float Gaussian(int x, int y, float sig)
        {
            float pi = (float)Math.PI;
            return (1f / (2f * pi * sig * sig)) * (float)Math.Exp(-(x * x + y * y) / (2.0f * sig * sig));
        }


        /// <summary>
        /// Get the value of the gaussian with std dev sigma
        /// at the point (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        float Gaussian(float x, float y, float sig)
        {
            float pi = (float)Math.PI;
            return 1f / (2f * pi * sig * sig) * (float)Math.Exp(-(x * x + y * y) / (2.0f * sig * sig));
        }


    } // SurfDescriptor

    public class SurfPoint
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public SurfPoint()
        {
            orientation = 0;
        }

        /// <summary>
        /// Coordinates of the detected interest point
        /// </summary>
        public float x, y;

        /// <summary>
        /// Detected scale
        /// </summary>
        public float scale;

        /// <summary>
        /// Response of the detected feature (strength)
        /// </summary>
        public float response;

        /// <summary>
        /// Orientation measured anti-clockwise from +ve x-axis
        /// </summary>
        public float orientation;

        /// <summary>
        /// Sign of laplacian for fast matching purposes
        /// </summary>
        public int laplacian;

        /// <summary>
        /// Descriptor vector
        /// </summary>
        public int descriptorLength;
        public float[] descriptor = null;
        public void SetDescriptorLength(int Size)
        {
            descriptorLength = Size;
            descriptor = new float[Size];
        }
    }

    public class IntegralImage
    {
        const float cR = .2989f;
        const float cG = .5870f;
        const float cB = .1140f;

        internal float[,] Matrix;
        public int Width, Height;

        public float this[int y, int x]
        {
            get { return Matrix[y, x]; }
            set { Matrix[y, x] = value; }
        }

        private IntegralImage(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.Matrix = new float[height, width];
        }

        public static IntegralImage FromImage(ImageBgr24 image)
        {
            IntegralImage pic = new IntegralImage(image.Width, image.Height);

            float rowsum = 0;
            for (int x = 0; x < image.Width; x++)
            {
                Bgr24 c = image[0, x];
                rowsum += (cR * c.Red + cG * c.Green + cB * c.Blue) / 255f;
                pic[0, x] = rowsum;
            }

            for (int y = 1; y < image.Height; y++)
            {
                rowsum = 0;
                for (int x = 0; x < image.Width; x++)
                {
                    Bgr24 c = image[y, x];
                    rowsum += (cR * c.Red + cG * c.Green + cB * c.Blue) / 255f;

                    // integral image is rowsum + value above        
                    pic[y, x] = rowsum + pic[y - 1, x];
                }
            }

            return pic;
        }


        public float BoxIntegral(int row, int col, int rows, int cols)
        {
            // The subtraction by one for row/col is because row/col is inclusive.
            int r1 = Math.Min(row, Height) - 1;
            int c1 = Math.Min(col, Width) - 1;
            int r2 = Math.Min(row + rows, Height) - 1;
            int c2 = Math.Min(col + cols, Width) - 1;

            float A = 0, B = 0, C = 0, D = 0;
            if (r1 >= 0 && c1 >= 0) A = Matrix[r1, c1];
            if (r1 >= 0 && c2 >= 0) B = Matrix[r1, c2];
            if (r2 >= 0 && c1 >= 0) C = Matrix[r2, c1];
            if (r2 >= 0 && c2 >= 0) D = Matrix[r2, c2];

            return Math.Max(0, A - B - C + D);
        }

        /// <summary>
        /// Get Haar Wavelet X repsonse
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public float HaarX(int row, int column, int size)
        {
            return BoxIntegral(row - size / 2, column, size, size / 2)
              - 1 * BoxIntegral(row - size / 2, column - size / 2, size, size / 2);
        }

        /// <summary>
        /// Get Haar Wavelet Y repsonse
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public float HaarY(int row, int column, int size)
        {
            return BoxIntegral(row, column - size / 2, size / 2, size)
              - 1 * BoxIntegral(row - size / 2, column - size / 2, size / 2, size);
        }
    }

    public class FastHessian
    {

        /// <summary>
        /// Reponse Layer 
        /// </summary>
        private class ResponseLayer
        {
            public int width, height, step, filter;
            public float[] responses;
            public byte[] laplacian;

            public ResponseLayer(int width, int height, int step, int filter)
            {
                this.width = width;
                this.height = height;
                this.step = step;
                this.filter = filter;

                responses = new float[width * height];
                laplacian = new byte[width * height];
            }

            public byte getLaplacian(int row, int column)
            {
                return laplacian[row * width + column];
            }

            public byte getLaplacian(int row, int column, ResponseLayer src)
            {
                int scale = this.width / src.width;
                return laplacian[(scale * row) * width + (scale * column)];
            }

            public float getResponse(int row, int column)
            {
                return responses[row * width + column];
            }

            public float getResponse(int row, int column, ResponseLayer src)
            {
                int scale = this.width / src.width;
                return responses[(scale * row) * width + (scale * column)];
            }
        }


        /// <summary>
        /// Static one-call do it all method
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="octaves"></param>
        /// <param name="init_sample"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static List<SurfPoint> getIpoints(float thresh, int octaves, int init_sample, IntegralImage img)
        {
            FastHessian fh = new FastHessian(thresh, octaves, init_sample, img);
            return fh.getIpoints();
        }


        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="octaves"></param>
        /// <param name="init_sample"></param>
        /// <param name="img"></param>
        public FastHessian(float thresh, int octaves, int init_sample, IntegralImage img)
        {
            this.thresh = thresh;
            this.octaves = octaves;
            this.init_sample = init_sample;
            this.img = img;
        }


        /// <summary>
        /// These are passed in
        /// </summary>
        private float thresh;
        private int octaves;
        private int init_sample;
        private IntegralImage img;


        /// <summary>
        /// These get built
        /// </summary>
        private List<SurfPoint> ipts;
        private List<ResponseLayer> responseMap;


        /// <summary>
        /// Find the image features and write into vector of features
        /// </summary>
        public List<SurfPoint> getIpoints()
        {
            // filter index map
            int[,] filter_map = { { 0, 1, 2, 3 }, { 1, 3, 4, 5 }, { 3, 5, 6, 7 }, { 5, 7, 8, 9 }, { 7, 9, 10, 11 } };

            // Clear the vector of exisiting ipts
            if (ipts == null) ipts = new List<SurfPoint>();
            else ipts.Clear();

            // Build the response map
            buildResponseMap();

            // Get the response layers
            ResponseLayer b, m, t;
            for (int o = 0; o < octaves; ++o) for (int i = 0; i <= 1; ++i)
                {
                    b = responseMap[filter_map[o, i]];
                    m = responseMap[filter_map[o, i + 1]];
                    t = responseMap[filter_map[o, i + 2]];

                    // loop over middle response layer at density of the most 
                    // sparse layer (always top), to find maxima across scale and space
                    for (int r = 0; r < t.height; ++r)
                    {
                        for (int c = 0; c < t.width; ++c)
                        {
                            if (isExtremum(r, c, t, m, b))
                            {
                                interpolateExtremum(r, c, t, m, b);
                            }
                        }
                    }
                }

            return ipts;
        }


        /// <summary>
        /// Build map of DoH responses
        /// </summary>
        void buildResponseMap()
        {
            // Calculate responses for the first 4 octaves:
            // Oct1: 9,  15, 21, 27
            // Oct2: 15, 27, 39, 51
            // Oct3: 27, 51, 75, 99
            // Oct4: 51, 99, 147,195
            // Oct5: 99, 195,291,387

            // Deallocate memory and clear any existing response layers
            if (responseMap == null) responseMap = new List<ResponseLayer>();
            else responseMap.Clear();

            // Get image attributes
            int w = (img.Width / init_sample);
            int h = (img.Height / init_sample);
            int s = (init_sample);

            // Calculate approximated determinant of hessian values
            if (octaves >= 1)
            {
                responseMap.Add(new ResponseLayer(w, h, s, 9));
                responseMap.Add(new ResponseLayer(w, h, s, 15));
                responseMap.Add(new ResponseLayer(w, h, s, 21));
                responseMap.Add(new ResponseLayer(w, h, s, 27));
            }

            if (octaves >= 2)
            {
                responseMap.Add(new ResponseLayer(w / 2, h / 2, s * 2, 39));
                responseMap.Add(new ResponseLayer(w / 2, h / 2, s * 2, 51));
            }

            if (octaves >= 3)
            {
                responseMap.Add(new ResponseLayer(w / 4, h / 4, s * 4, 75));
                responseMap.Add(new ResponseLayer(w / 4, h / 4, s * 4, 99));
            }

            if (octaves >= 4)
            {
                responseMap.Add(new ResponseLayer(w / 8, h / 8, s * 8, 147));
                responseMap.Add(new ResponseLayer(w / 8, h / 8, s * 8, 195));
            }

            if (octaves >= 5)
            {
                responseMap.Add(new ResponseLayer(w / 16, h / 16, s * 16, 291));
                responseMap.Add(new ResponseLayer(w / 16, h / 16, s * 16, 387));
            }

            // Extract responses from the image
            for (int i = 0; i < responseMap.Count; ++i)
            {
                buildResponseLayer(responseMap[i]);
            }
        }


        /// <summary>
        /// Build Responses for a given ResponseLayer
        /// </summary>
        /// <param name="rl"></param>
        private void buildResponseLayer(ResponseLayer rl)
        {
            int step = rl.step;                      // step size for this filter
            int b = (rl.filter - 1) / 2;             // border for this filter
            int l = rl.filter / 3;                   // lobe for this filter (filter size / 3)
            int w = rl.filter;                       // filter size
            float inverse_area = 1f / (w * w);       // normalisation factor
            float Dxx, Dyy, Dxy;

            for (int r, c, ar = 0, index = 0; ar < rl.height; ++ar)
            {
                for (int ac = 0; ac < rl.width; ++ac, index++)
                {
                    // get the image coordinates
                    r = ar * step;
                    c = ac * step;

                    // Compute response components
                    Dxx = img.BoxIntegral(r - l + 1, c - b, 2 * l - 1, w)
                        - img.BoxIntegral(r - l + 1, c - l / 2, 2 * l - 1, l) * 3;
                    Dyy = img.BoxIntegral(r - b, c - l + 1, w, 2 * l - 1)
                        - img.BoxIntegral(r - l / 2, c - l + 1, l, 2 * l - 1) * 3;
                    Dxy = +img.BoxIntegral(r - l, c + 1, l, l)
                          + img.BoxIntegral(r + 1, c - l, l, l)
                          - img.BoxIntegral(r - l, c - l, l, l)
                          - img.BoxIntegral(r + 1, c + 1, l, l);

                    // Normalise the filter responses with respect to their size
                    Dxx *= inverse_area;
                    Dyy *= inverse_area;
                    Dxy *= inverse_area;

                    // Get the determinant of hessian response & laplacian sign
                    rl.responses[index] = (Dxx * Dyy - 0.81f * Dxy * Dxy);
                    rl.laplacian[index] = (byte)(Dxx + Dyy >= 0 ? 1 : 0);
                }
            }
        }


        /// <summary>
        /// Test whether the point r,c in the middle layer is extremum in 3x3x3 neighbourhood
        /// </summary>
        /// <param name="r">Row to be tested</param>
        /// <param name="c">Column to be tested</param>
        /// <param name="t">Top ReponseLayer</param>
        /// <param name="m">Middle ReponseLayer</param>
        /// <param name="b">Bottome ReponseLayer</param>
        /// <returns></returns>
        bool isExtremum(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            // bounds check
            int layerBorder = (t.filter + 1) / (2 * t.step);
            if (r <= layerBorder || r >= t.height - layerBorder || c <= layerBorder || c >= t.width - layerBorder)
                return false;

            // check the candidate point in the middle layer is above thresh 
            float candidate = m.getResponse(r, c, t);
            if (candidate < thresh)
                return false;

            for (int rr = -1; rr <= 1; ++rr)
            {
                for (int cc = -1; cc <= 1; ++cc)
                {
                    // if any response in 3x3x3 is greater candidate not maximum
                    if (t.getResponse(r + rr, c + cc) >= candidate ||
                      ((rr != 0 || cc != 0) && m.getResponse(r + rr, c + cc, t) >= candidate) ||
                      b.getResponse(r + rr, c + cc, t) >= candidate)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Interpolate scale-space extrema to subpixel accuracy to form an image feature
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <param name="m"></param>
        /// <param name="b"></param>
        void interpolateExtremum(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            Geb.Image.Matrix D = new Geb.Image.Matrix(BuildDerivative(r, c, t, m, b));
            Geb.Image.Matrix H = new Geb.Image.Matrix(BuildHessian(r, c, t, m, b));
            Geb.Image.Matrix Hi = H.Inverse();
            Geb.Image.Matrix Of = -1 * Hi * D;

            // get the offsets from the interpolation
            double[] O = { Of[0, 0], Of[1, 0], Of[2, 0] };

            // get the step distance between filters
            int filterStep = (m.filter - b.filter);

            // If point is sufficiently close to the actual extremum
            if (Math.Abs(O[0]) < 0.5f && Math.Abs(O[1]) < 0.5f && Math.Abs(O[2]) < 0.5f)
            {
                SurfPoint ipt = new SurfPoint();
                ipt.x = (float)((c + O[0]) * t.step);
                ipt.y = (float)((r + O[1]) * t.step);
                ipt.scale = (float)((0.1333f) * (m.filter + O[2] * filterStep));
                ipt.laplacian = (int)(m.getLaplacian(r, c, t));
                ipts.Add(ipt);
            }
        }


        /// <summary>
        /// Build Matrix of First Order Scale-Space derivatives
        /// </summary>
        /// <param name="octave"></param>
        /// <param name="interval"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>3x1 Matrix of Derivatives</returns>
        private double[,] BuildDerivative(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            double dx, dy, ds;

            dx = (m.getResponse(r, c + 1, t) - m.getResponse(r, c - 1, t)) / 2f;
            dy = (m.getResponse(r + 1, c, t) - m.getResponse(r - 1, c, t)) / 2f;
            ds = (t.getResponse(r, c) - b.getResponse(r, c, t)) / 2f;

            double[,] D = { { dx }, { dy }, { ds } };
            return D;
        }


        /// <summary>
        /// Build Hessian Matrix 
        /// </summary>
        /// <param name="octave"></param>
        /// <param name="interval"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>3x3 Matrix of Second Order Derivatives</returns>
        private double[,] BuildHessian(int r, int c, ResponseLayer t, ResponseLayer m, ResponseLayer b)
        {
            double v, dxx, dyy, dss, dxy, dxs, dys;

            v = m.getResponse(r, c, t);
            dxx = m.getResponse(r, c + 1, t) + m.getResponse(r, c - 1, t) - 2 * v;
            dyy = m.getResponse(r + 1, c, t) + m.getResponse(r - 1, c, t) - 2 * v;
            dss = t.getResponse(r, c) + b.getResponse(r, c, t) - 2 * v;
            dxy = (m.getResponse(r + 1, c + 1, t) - m.getResponse(r + 1, c - 1, t) -
                    m.getResponse(r - 1, c + 1, t) + m.getResponse(r - 1, c - 1, t)) / 4f;
            dxs = (t.getResponse(r, c + 1) - t.getResponse(r, c - 1) -
                    b.getResponse(r, c + 1, t) + b.getResponse(r, c - 1, t)) / 4f;
            dys = (t.getResponse(r + 1, c) - t.getResponse(r - 1, c) -
                    b.getResponse(r + 1, c, t) + b.getResponse(r - 1, c, t)) / 4f;

            double[,] H = new double[3, 3];
            H[0, 0] = dxx;
            H[0, 1] = dxy;
            H[0, 2] = dxs;
            H[1, 0] = dxy;
            H[1, 1] = dyy;
            H[1, 2] = dys;
            H[2, 0] = dxs;
            H[2, 1] = dys;
            H[2, 2] = dss;
            return H;
        }
    } // FastHessian
}
