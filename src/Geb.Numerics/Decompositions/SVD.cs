using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geb.Numerics
{
    /// <summary>
    /// SVD 分解
    /// </summary>
    public class SVD
    {
        const double EPS = Double.MinValue;

        public SVD(Matrix A)
        {
            int m = A.RowCount;
            int n = A.ColumnCount;
            int p = Math.Min( m, n );

            U = new Matrix(m,p);
            V = new Matrix(n,p);
            S = new Matrix(p);

            Matrix B;
            if( m >= n )
            {
                B = A;
                Decomposition( B, U, S, V );
            }
            else
            {
                B = A.Transpose();
                Decomposition( B, V, S, U );
            }

            if (B != A)
            {
                B.Dispose();
            }
        }

        public Matrix U;
        public Matrix V;
        public Matrix S;
        public Matrix SV;

        public Matrix GetSingularValuesMatrix()
        {
            int N = S.Length;
            Matrix tmp = new Matrix(N, N, true);
            for (int i = 0; i < N; ++i)
                tmp[i, i] = S[i];
            return tmp;
        }

        /// <summary>
        /// 2范数
        /// </summary>
        /// <returns></returns>
        public double GetNorm2()
        {
            return S[0];
        }

        /// <summary>
        /// 条件数
        /// </summary>
        /// <returns></returns>
        public double GetCond()
        {
            return (S[0] / S[S.Length-1]);
        }

        /// <summary>
        /// 得到矩阵的Rank
        /// </summary>
        /// <returns></returns>
        public int GetRank()
        {
            int N = S.Length;
            double tol = N * S[0] * EPS;
            int r = 0;
            for (int i = 0; i < N; ++i)
                if (S[i] > tol)
                    r++;
            return r;
        }

        private double Hypot(double a, double b)
        {
            return Math.Sqrt(a * a + b * b);
        }

        public void Decomposition(Matrix B, Matrix U, Matrix S, Matrix V)
        {
            double tmp;
            int m = B.RowCount, n = B.ColumnCount;
            Matrix e = new Matrix(n);
            Matrix work = new Matrix(m);

            // boolean
            int wantu = 1;
            int wantv = 1;

            // Reduce A to bidiagonal form, storing the diagonal elements
            // in s and the super-diagonal elements in e.
            int nct = Math.Min(m - 1, n);
            int nrt = Math.Max(0, n - 2);
            int i = 0,
                j = 0,
                k = 0;

            for (k = 0; k < Math.Max(nct, nrt); ++k)
            {
                if (k < nct)
                {
                    // Compute the transformation for the k-th column and
                    // place the k-th diagonal in s[k].
                    // Compute 2-norm of k-th column without under/overflow.
                    S[k] = 0;
                    for (i = k; i < m; ++i)
                        S[k] = Hypot(S[k], B[i, k]);

                    if (S[k] != 0)
                    {
                        if (B[k, k] < 0)
                            S[k] = -S[k];

                        for (i = k; i < m; ++i)
                            B[i, k] /= S[k];
                        B[k, k] += 1;
                    }
                    S[k] = -S[k];
                }

                for (j = k + 1; j < n; ++j)
                {
                    if ((k < nct) && (S[k] != 0))
                    {
                        // apply the transformation
                        double t = 0;
                        for (i = k; i < m; ++i)
                            t += B[i, k] * B[i, j];

                        t = -t / B[k, k];
                        for (i = k; i < m; ++i)
                            B[i, j] += t * B[i, k];
                    }
                    e[j] = B[k, j];
                }

                // Place the transformation in U for subsequent back
                // multiplication.
                if (wantu != 0 & (k < nct))
                    for (i = k; i < m; ++i)
                        U[i, k] = B[i, k];

                if (k < nrt)
                {
                    // Compute the k-th row transformation and place the
                    // k-th super-diagonal in e[k].
                    // Compute 2-norm without under/overflow.
                    e[k] = 0;
                    for (i = k + 1; i < n; ++i)
                        e[k] = Hypot(e[k], e[i]);

                    if (e[k] != 0)
                    {
                        if (e[k + 1] < 0)
                            e[k] = -e[k];

                        for (i = k + 1; i < n; ++i)
                            e[i] /= e[k];
                        e[k + 1] += 1;
                    }
                    e[k] = -e[k];

                    if ((k + 1 < m) && (e[k] != 0))
                    {
                        // apply the transformation
                        for (i = k + 1; i < m; ++i)
                            work[i] = 0;

                        for (j = k + 1; j < n; ++j)
                            for (i = k + 1; i < m; ++i)
                                work[i] += e[j] * B[i, j];

                        for (j = k + 1; j < n; ++j)
                        {
                            double t = -e[j] / e[k + 1];
                            for (i = k + 1; i < m; ++i)
                                B[i, j] += t * work[i];
                        }
                    }

                    // Place the transformation in V for subsequent
                    // back multiplication.
                    if (wantv != 0)
                        for (i = k + 1; i < n; ++i)
                            V[i, k] = e[i];
                }
            }

            // Set up the final bidiagonal matrix or order p.
            int p = n;

            if (nct < n)
                S[nct] = B[nct, nct];
            if (m < p)
                S[p - 1] = 0;

            if (nrt + 1 < p)
                e[nrt] = B[nrt, p - 1];
            e[p - 1] = 0;

            // if required, generate U
            if (wantu != 0)
            {
                for (j = nct; j < n; ++j)
                {
                    for (i = 0; i < m; ++i)
                        U[i, j] = 0;
                    U[j, j] = 1;
                }

                for (k = nct - 1; k >= 0; --k)
                    if (S[k] != 0)
                    {
                        for (j = k + 1; j < n; ++j)
                        {
                            double t = 0;
                            for (i = k; i < m; ++i)
                                t += U[i, k] * U[i, j];
                            t = -t / U[k, k];

                            for (i = k; i < m; ++i)
                                U[i, j] += t * U[i, k];
                        }

                        for (i = k; i < m; ++i)
                            U[i, k] = -U[i, k];
                        U[k, k] = 1 + U[k, k];

                        for (i = 0; i < k - 1; ++i)
                            U[i, k] = 0;
                    }
                    else
                    {
                        for (i = 0; i < m; ++i)
                            U[i, k] = 0;
                        U[k, k] = 1;
                    }
            }

            // if required, generate V
            if (wantv != 0)
                for (k = n - 1; k >= 0; --k)
                {
                    if ((k < nrt) && (e[k] != 0))
                        for (j = k + 1; j < n; ++j)
                        {
                            double t = 0;
                            for (i = k + 1; i < n; ++i)
                                t += V[i, k] * V[i, j];
                            t = -t / V[k + 1, k];

                            for (i = k + 1; i < n; ++i)
                                V[i, j] += t * V[i, k];
                        }

                    for (i = 0; i < n; ++i)
                        V[i, k] = 0;
                    V[k, k] = 1;
                }

            // main iteration loop for the singular values
            int pp = p - 1;
            int iter = 0;
            double eps = Math.Pow(2.0, -52.0);

            while (p > 0)
            {
                k = 0;
                int kase = 0;

                // Here is where a test for too many iterations would go.
                // This section of the program inspects for negligible
                // elements in the s and e arrays. On completion the
                // variables kase and k are set as follows.
                // kase = 1     if s(p) and e[k-1] are negligible and k<p
                // kase = 2     if s(k) is negligible and k<p
                // kase = 3     if e[k-1] is negligible, k<p, and
                //				s(k), ..., s(p) are not negligible
                // kase = 4     if e(p-1) is negligible (convergence).
                for (k = p - 2; k >= -1; --k)
                {
                    if (k == -1)
                        break;

                    if (Math.Abs(e[k]) <= eps * (Math.Abs(S[k]) + Math.Abs(S[k + 1])))
                    {
                        e[k] = 0;
                        break;
                    }
                }

                if (k == p - 2)
                    kase = 4;
                else
                {
                    int ks;
                    for (ks = p - 1; ks >= k; --ks)
                    {
                        if (ks == k)
                            break;

                        double t = ((ks != p) ? Math.Abs(e[ks]) : 0) +
                                 ((ks != k + 1) ? Math.Abs(e[ks - 1]) : 0);

                        if (Math.Abs(S[ks]) <= eps * t)
                        {
                            S[ks] = 0;
                            break;
                        }
                    }

                    if (ks == k)
                        kase = 3;
                    else if (ks == p - 1)
                        kase = 1;
                    else
                    {
                        kase = 2;
                        k = ks;
                    }
                }
                k++;

                // Perform the task indicated by kase.
                switch (kase)
                {
                    // deflate negligible s(p)
                    case 1:
                        {
                            double f = e[p - 2];
                            e[p - 2] = 0;

                            for (j = p - 2; j >= k; --j)
                            {
                                double t = Hypot(S[j], f);
                                double cs = S[j] / t;
                                double sn = f / t;
                                S[j] = t;

                                if (j != k)
                                {
                                    f = -sn * e[j - 1];
                                    e[j - 1] = cs * e[j - 1];
                                }

                                if (wantv != 0)
                                    for (i = 0; i < n; ++i)
                                    {
                                        t = cs * V[i, j] + sn * V[i, p - 1];
                                        V[i, p - 1] = -sn * V[i, j] + cs * V[i, p - 1];
                                        V[i, j] = t;
                                    }
                            }
                        }
                        break;

                    // split at negligible s(k)
                    case 2:
                        {
                            double f = e[k - 1];
                            e[k - 1] = 0;

                            for (j = k; j < p; ++j)
                            {
                                double t = Hypot(S[j], f);
                                double cs = S[j] / t;
                                double sn = f / t;
                                S[j] = t;
                                f = -sn * e[j];
                                e[j] = cs * e[j];

                                if (wantu != 0)
                                    for (i = 0; i < m; ++i)
                                    {
                                        t = cs * U[i, j] + sn * U[i, k - 1];
                                        U[i, k - 1] = -sn * U[i, j] + cs * U[i, k - 1];
                                        U[i, j] = t;
                                    }
                            }
                        }
                        break;

                    // perform one qr step
                    case 3:
                        {
                            // calculate the shift
                            double scale = Math.Max(Math.Max(Math.Max(Math.Max(
                                         Math.Abs(S[p - 1]), Math.Abs(S[p - 2])), Math.Abs(e[p - 2])),
                                         Math.Abs(S[k])), Math.Abs(e[k]));
                            double sp = S[p - 1] / scale;
                            double spm1 = S[p - 2] / scale;
                            double epm1 = e[p - 2] / scale;
                            double sk = S[k] / scale;
                            double ek = e[k] / scale;
                            double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
                            double c = (sp * epm1) * (sp * epm1);
                            double shift = 0;

                            if ((b != 0) || (c != 0))
                            {
                                shift = Math.Sqrt(b * b + c);
                                if (b < 0)
                                    shift = -shift;
                                shift = c / (b + shift);
                            }
                            double f = (sk + sp) * (sk - sp) + shift;
                            double g = sk * ek;

                            // chase zeros
                            for (j = k; j < p - 1; ++j)
                            {
                                double t = Hypot(f, g);
                                double cs = f / t;
                                double sn = g / t;
                                if (j != k)
                                    e[j - 1] = t;

                                f = cs * S[j] + sn * e[j];
                                e[j] = cs * e[j] - sn * S[j];
                                g = sn * S[j + 1];
                                S[j + 1] = cs * S[j + 1];

                                if (wantv != 0)
                                    for (i = 0; i < n; ++i)
                                    {
                                        t = cs * V[i, j] + sn * V[i, j + 1];
                                        V[i, j + 1] = -sn * V[i, j] + cs * V[i, j + 1];
                                        V[i, j] = t;
                                    }

                                t = Hypot(f, g);
                                cs = f / t;
                                sn = g / t;
                                S[j] = t;
                                f = cs * e[j] + sn * S[j + 1];
                                S[j + 1] = -sn * e[j] + cs * S[j + 1];
                                g = sn * e[j + 1];
                                e[j + 1] = cs * e[j + 1];

                                if (wantu != 0 && (j < m - 1))
                                    for (i = 0; i < m; ++i)
                                    {
                                        t = cs * U[i, j] + sn * U[i, j + 1];
                                        U[i, j + 1] = -sn * U[i, j] + cs * U[i, j + 1];
                                        U[i, j] = t;
                                    }
                            }
                            e[p - 2] = f;
                            iter = iter + 1;
                        }
                        break;

                    // convergence
                    case 4:
                        {
                            // Make the singular values positive.
                            if (S[k] <= 0)
                            {
                                S[k] = (S[k] < 0) ? -S[k] : 0;
                                if (wantv != 0)
                                    for (i = 0; i <= pp; ++i)
                                        V[i, k] = -V[i, k];
                            }

                            // Order the singular values.
                            while (k < pp)
                            {
                                if (S[k] >= S[k + 1])
                                    break;

                                double t = S[k];
                                S[k] = S[k + 1];
                                S[k + 1] = t;

                                if (wantv != 0 && (k < n - 1))
                                    for (i = 0; i < n; ++i)
                                    {
                                        // swap(V[i, k], V[i, k + 1]);
                                        tmp = V[i, k];
                                        V[i, k] = V[i, k + 1];
                                        V[i, k + 1] = tmp;
                                    }


                                if (wantu != 0 && (k < m - 1))
                                    for (i = 0; i < m; ++i)
                                    {
                                        // swap(V[i, k], V[i, k + 1]);
                                        tmp = V[i, k];
                                        V[i, k] = V[i, k + 1];
                                        V[i, k + 1] = tmp;
                                    }

                                k++;
                            }
                            iter = 0;
                            p--;
                        }
                        break;
                }
            }
        }

        public static void Test()
        {
            Matrix A = new Matrix(2, 4);
            A[0, 0] = 1; A[0, 1] = 3; A[0, 2] = 5; A[0, 3] = 7;
            A[1, 0] = 2; A[1, 1] = 4; A[1, 2] = 6; A[1, 3] = 8;
            SVD svd = new SVD(A);
            Matrix U = svd.U;
            Matrix V = svd.V;
            Matrix S = svd.GetSingularValuesMatrix();
            Console.WriteLine(svd);
            Console.WriteLine(S);
        }
    }
}
