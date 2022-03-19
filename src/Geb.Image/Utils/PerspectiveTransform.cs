using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geb.Image.Utils
{
    public class PerspectiveTransform
    {
        public static void GetPerspectiveTransform(PointF[] src, PointF[] dst, int solveMethod)
        {
            /* Calculates coefficients of perspective transformation
             * which maps <quad> into rectangle ((0,0), (w,0), (w,h), (h,0)):
             *
             *      c00*xi + c01*yi + c02
             * ui = ---------------------
             *      c20*xi + c21*yi + c22
             *
             *      c10*xi + c11*yi + c12
             * vi = ---------------------
             *      c20*xi + c21*yi + c22
             *
             * Coefficients are calculated by solving linear system:
             * / x0 y0  1  0  0  0 -x0*u0 -y0*u0 \ /c00\ /u0\
             * | x1 y1  1  0  0  0 -x1*u1 -y1*u1 | |c01| |u1|
             * | x2 y2  1  0  0  0 -x2*u2 -y2*u2 | |c02| |u2|
             * | x3 y3  1  0  0  0 -x3*u3 -y3*u3 |.|c10|=|u3|,
             * |  0  0  0 x0 y0  1 -x0*v0 -y0*v0 | |c11| |v0|
             * |  0  0  0 x1 y1  1 -x1*v1 -y1*v1 | |c12| |v1|
             * |  0  0  0 x2 y2  1 -x2*v2 -y2*v2 | |c20| |v2|
             * \  0  0  0 x3 y3  1 -x3*v3 -y3*v3 / \c21/ \v3/
             *
             * where:
             *   (xi, yi) = (quad[i][0], quad[i][1])
             *        cij - coeffs[i][j], coeffs[2][2] = 1
             *   (ui, vi) - rectangle vertices
             */

            //CV_INSTRUMENT_REGION();

            //Mat M(3, 3, CV_64F), X(8, 1, CV_64F, M.ptr());
            //double a[8][8], b[8];
            //Mat A(8, 8, CV_64F, a), B(8, 1, CV_64F, b);

            //for (int i = 0; i < 4; ++i)
            //{
            //    a[i][0] = a[i + 4][3] = src[i].x;
            //    a[i][1] = a[i + 4][4] = src[i].y;
            //    a[i][2] = a[i + 4][5] = 1;
            //    a[i][3] = a[i][4] = a[i][5] =
            //    a[i + 4][0] = a[i + 4][1] = a[i + 4][2] = 0;
            //    a[i][6] = -src[i].x * dst[i].x;
            //    a[i][7] = -src[i].y * dst[i].x;
            //    a[i + 4][6] = -src[i].x * dst[i].y;
            //    a[i + 4][7] = -src[i].y * dst[i].y;
            //    b[i] = dst[i].x;
            //    b[i + 4] = dst[i].y;
            //}

            //solve(A, B, X, solveMethod);
            //M.ptr<double>()[8] = 1.;

            //return M;

        }
    }
}
