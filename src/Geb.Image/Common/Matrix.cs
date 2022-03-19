using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Geb.Image
{
    /// <summary>
    /// 矩阵异常
    /// </summary>
    public class MatrixException : Exception
    {
        /// <summary>
        /// 创建矩阵异常
        /// </summary>
        /// <param name="Message"></param>
        public MatrixException(string Message)
            : base(Message)
        { }
    }

    public class Matrix : IDisposable
    {
        /// <summary>
        /// 创建单位矩阵，
        /// </summary>
        /// <param name="rank">矩阵的秩</param>
        /// <returns>单位矩阵</returns>
        public static Matrix CreateIdentity(int rank)
        {
            Matrix m = new Matrix(rank, rank);
            for (int i = 0; i < rank; i++)
            {
                m[i, i] = 1;
            }
            return m;
        }

        private unsafe double* Data;

        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly int _length;

        /// <summary>
        /// 矩阵的行数
        /// </summary>
        public int RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// 矩阵的列数
        /// </summary>
        public int ColumnCount
        {
            get { return _columnCount; }
        }

        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// 代表当前矩阵数据的二维数组。
        /// .Net 的 IDE 均不支持直接查看.Net程序中的指针内容，DataSnapshot 提供了调试时查看
        /// 矩阵数据的唯一途径。请谨慎使用本方法。
        /// </summary>
        public double[,] DataSnapshot
        {
            get
            {
                double[,] array = new double[RowCount, ColumnCount];
                for (int r = 0; r < RowCount; r++)
                {
                    for (int c = 0; c < ColumnCount; c++)
                    {
                        array[r, c] = this[r, c];
                    }
                }
                return array;
            }
        }

        /// <summary>
        /// 读写矩阵的元素。本处读写不进行边界检查。
        /// </summary>
        /// <param name="row">元素所处行</param>
        /// <param name="column">元素所处值</param>
        /// <returns>矩阵元素值</returns>
        public unsafe double this[int row, int column]
        {
            get { return Data[row * _columnCount + column]; }
            set { Data[row * _columnCount + column] = value; }
        }

        /// <summary>
        /// 读写矩阵的元素。本处读写不进行边界检查。主要用于将矩阵用作向量（一维矩阵）时的操作。
        /// </summary>
        /// <param name="index">将矩阵当作向量，元素在向量中所处的位置</param>
        /// <returns>矩阵元素值</returns>
        public unsafe double this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        /// <summary>
        /// 读取子矩阵或者使用子矩阵赋值。
        /// </summary>
        /// <param name="firstRowIdx">子矩阵首行行号</param>
        /// <param name="lastRowIdx">子矩阵末行行号</param>
        /// <param name="firstColumnIdx">子矩阵首列列号</param>
        /// <param name="lastColumnIdx">子矩阵末列列号</param>
        /// <returns>子矩阵</returns>
        public unsafe Matrix this[int firstRowIdx, int lastRowIdx,
            int firstColumnIdx, int lastColumnIdx]
        {
            get
            {
                if (firstRowIdx < 0 || lastRowIdx >= this.RowCount || firstRowIdx >= lastRowIdx
                    || firstColumnIdx < 0 || lastColumnIdx >= this.ColumnCount || firstColumnIdx >= lastColumnIdx)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // 这里性能可以用指针优化。
                Matrix m = new Matrix(lastRowIdx - firstRowIdx + 1, lastColumnIdx - firstColumnIdx + 1);
                for (int r = 0; r < m.RowCount; r++)
                {
                    for (int c = 0; c < m.ColumnCount; c++)
                    {
                        m[r, c] = this[firstRowIdx + r, firstColumnIdx + c];
                    }
                }
                return m;
            }
            set
            {
                if (firstRowIdx < 0 || lastRowIdx >= this.RowCount || firstRowIdx >= lastRowIdx
                    || firstColumnIdx < 0 || lastColumnIdx >= this.ColumnCount || firstColumnIdx >= lastColumnIdx)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // 这里性能可以用指针优化。
                Matrix m = value;
                if (m.RowCount != (lastRowIdx - firstRowIdx + 1) || m.ColumnCount != (lastColumnIdx - firstColumnIdx + 1))
                {
                    throw new ArgumentOutOfRangeException();
                }

                for (int r = 0; r < m.RowCount; r++)
                {
                    for (int c = 0; c < m.ColumnCount; c++)
                    {
                        this[firstRowIdx + r, firstColumnIdx + c] = m[r, c];
                    }
                }
            }
        }

        public unsafe Matrix(double[] data, int rows, int columns)
        {
            if (rows < 1) throw new ArgumentException("rows must > 0");
            if (columns < 1) throw new ArgumentException("columns must > 0");
            if (data.Length != rows * columns) throw new ArgumentException("data != rows * columns");

            _rowCount = rows;
            _columnCount = columns;
            _length = _rowCount * _columnCount;
            Data = (double*)Marshal.AllocHGlobal(_rowCount * _columnCount * sizeof(double));
            for (int r = 0; r < _rowCount; r++)
            {
                int cStart = r * columns;
                for (int c = 0; c < _columnCount; c++)
                {
                    this[r, c] = data[cStart + c];
                }
            }
        }

        public unsafe Matrix(double[,] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            int rows = data.GetUpperBound(0) + 1;
            int columns = data.GetUpperBound(1) + 1;

            _rowCount = rows;
            _columnCount = columns;
            _length = _rowCount * _columnCount;
            Data = (double*)Marshal.AllocHGlobal(_rowCount * _columnCount * sizeof(double));
            for (int r = 0; r < _rowCount; r++)
            {
                for (int c = 0; c < _columnCount; c++)
                {
                    this[r, c] = data[r, c];
                }
            }
        }

        public unsafe Matrix(int rows, int columns = 1, Boolean fillWidthZero = true)
        {
            if (rows < 1) throw new ArgumentException("rows must > 0");
            if (columns < 1) throw new ArgumentException("columns must > 0");
            _rowCount = rows;
            _columnCount = columns;
            _length = _rowCount * _columnCount;
            Data = (double*)Marshal.AllocHGlobal(_rowCount * _columnCount * sizeof(double));
            if (fillWidthZero == true)
            {
                Fill(0);
            }
        }

        /// <summary>
        /// 向矩阵中填充元素。
        /// </summary>
        /// <param name="val"></param>
        public unsafe void Fill(double val)
        {
            double* pStart = Data;
            double* pEnd = pStart + _rowCount * _columnCount;
            while (pStart != pEnd)
            {
                *pStart = val;
                pStart++;
            }
        }

        /// <summary>
        /// 求矩阵的逆矩阵或者广义逆矩阵。
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            Matrix m = null;
            Matrix I = null;
            if (_rowCount >= _columnCount)
            {
                I = CreateIdentity(_rowCount);
                m = Solve(I);
                I.Dispose();
                return m;
            }

            I = CreateIdentity(_columnCount);
            Matrix t = Transpose();
            Matrix tInversed = t.Solve(I);
            m = tInversed.Transpose();
            I.Dispose();
            t.Dispose();
            tInversed.Dispose();
            return m;
        }

        /// <summary>
        /// 产生一个新的矩阵，新的矩阵是当前矩阵的转置
        /// </summary>
        /// <returns>转置矩阵</returns>
        public Matrix Transpose()
        {
            Matrix m = new Matrix(this.ColumnCount, this.RowCount);
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    m[c, r] = this[r, c];
                }
            }
            return m;
        }

        /// <summary>
        /// 解方程 A*X = B。A 为本矩阵。
        /// </summary>
        /// <param name="B">右侧矩阵</param>
        /// <returns></returns>
        public Matrix Solve(Matrix B)
        {
            if (_rowCount == _columnCount)
            {
                LUD lu = new LUD(this);
                Matrix m = lu.Solve(B);
                lu.Dispose();
                return m;
            }

            if (_rowCount > _columnCount)
            {
                QRD qr = new QRD(this);
                Matrix m = qr.Solve(B);
                return m;
            }

            throw new InvalidOperationException("Only square supported.");
        }

        public unsafe Matrix Clone()
        {
            Matrix m = new Matrix(this.RowCount, this.ColumnCount, false);
            double* pSrc = this.Data;
            double* pDst = m.Data;
            double* pSrcEnd = pSrc + this.RowCount * this.ColumnCount;
            while (pSrc != pSrcEnd)
            {
                *pDst = *pSrc;
                pSrc++;
                pDst++;
            }
            return m;
        }

        #region 操作符重载

        public unsafe static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.Length != m2.Length) throw new ArgumentException("m1 and m2 must be same size");

            Matrix m = m2.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            double* p1 = m1.Data;
            while (p != pEnd)
            {
                *p = *p + *p1;
                p++; p1++;
            }
            return m;
        }

        public unsafe static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.Length != m2.Length) throw new ArgumentException("m1 and m2 must be same size");

            Matrix m = m2.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            double* p1 = m1.Data;
            while (p != pEnd)
            {
                *p = *p1 - *p;
                p++; p1++;
            }
            return m;
        }

        public unsafe static Matrix operator -(Matrix m1)
        {
            Matrix m = m1.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            while (p != pEnd)
            {
                *p = -*p;
                p++;
            }
            return m;
        }

        public unsafe static Matrix operator *(Matrix m1, double val)
        {
            Matrix m = m1.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            while (p != pEnd)
            {
                *p *= val;
                p++;
            }
            return m;
        }

        public unsafe static Matrix operator *(double val, Matrix m1)
        {
            return m1 * val;
        }

        public unsafe static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.ColumnCount != m2.RowCount) throw new ArgumentException("m1 and m2 cann't multiply");

            Matrix m = new Matrix(m1.RowCount, m2.ColumnCount, false);
            for (int j = 0; j < m2.ColumnCount; j++)
            {
                for (int i = 0; i < m1.RowCount; i++)
                {
                    double s = 0;
                    for (int k = 0; k < m1.ColumnCount; k++)
                    {
                        s += m1[i, k] * m2[k, j];
                    }

                    m[i, j] = s;
                }
            }
            return m;
        }

        #endregion 

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected unsafe virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                disposed = true;
                Marshal.FreeHGlobal((IntPtr)Data);
            }
        }

        private bool disposed;

        ~Matrix()
        {
            Dispose(false);
        }

        #endregion

        #region SVD

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
                int p = Math.Min(m, n);

                U = new Matrix(m, p);
                V = new Matrix(n, p);
                S = new Matrix(p);

                Matrix B;
                if (m >= n)
                {
                    B = A;
                    Decomposition(B, U, S, V);
                }
                else
                {
                    B = A.Transpose();
                    Decomposition(B, V, S, U);
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
                return (S[0] / S[S.Length - 1]);
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

        #endregion

        #region QRD

        /// <summary>
        /// QR 分解。本类未进行测试。
        /// </summary>
        public class QRD : IDisposable
        {
            private Matrix _QR;
            private Matrix _diag;
            private Boolean _isFullRank;

            private int RowCount
            {
                get { return _QR.RowCount; }
            }

            private int ColumnCount
            {
                get { return _QR.ColumnCount; }
            }

            public QRD(Matrix A)
            {
                _QR = A.Clone();
                _diag = new Matrix(ColumnCount);
                double val = 0;
                double norm = 0;
                for (int k = 0; k < ColumnCount; k++)
                {
                    for (int i = k; i < RowCount; i++)
                    {
                        norm = 0;
                        val = _QR[i, k];

                        if (Math.Abs(norm) > Math.Abs(val))
                        {
                            double r = val / norm;
                            norm = Math.Abs(norm) * Math.Sqrt(1 + r * r);
                        }
                        else if (Math.Abs(val) > 0.00000001)
                        {
                            double r = norm / val;
                            norm = Math.Abs(val) * Math.Sqrt(1 + r * r);
                        }
                    }

                    if (norm != 0.0)
                    {
                        if (_QR[k, k] < 0)
                        {
                            norm = -norm;
                        }

                        for (int i = k; i < RowCount; i++)
                        {
                            _QR[i, k] /= norm;
                        }

                        _QR[k, k] += 1.0;

                        for (int j = k + 1; j < ColumnCount; j++)
                        {
                            double s = 0.0;
                            for (int i = k; i < RowCount; i++)
                            {
                                s += _QR[i, k] * _QR[i, j];
                            }

                            s = (-s) / _QR[k, k];
                            for (int i = k; i < RowCount; i++)
                            {
                                _QR[i, j] += s * _QR[i, k];
                            }
                        }
                    }

                    _diag[k] = -norm;
                }

                _isFullRank = CheckIsDiagFullRank();
            }

            private bool CheckIsDiagFullRank()
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    if (_diag[j] == 0.0)
                    {
                        return false;
                    }
                }

                return true;
            }

            public Matrix Solve(Matrix B)
            {
                if (B.RowCount != RowCount)
                {
                    throw new ArgumentException("B");
                }

                if (_isFullRank == false)
                {
                    throw new InvalidOperationException("diag matrix is not fullrank");
                }

                int nx = B.ColumnCount;
                Matrix X = B.Clone();

                for (int k = 0; k < ColumnCount; k++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        double s = 0.0;
                        for (int i = k; i < RowCount; i++)
                        {
                            s += _QR[i, k] * X[i, j];
                        }

                        s = (-s) / _QR[k, k];
                        for (int i = k; i < RowCount; i++)
                        {
                            X[i, j] += s * _QR[i, k];
                        }
                    }
                }

                for (int k = ColumnCount - 1; k >= 0; k--)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[k, j] /= _diag[k];
                    }

                    for (int i = 0; i < k; i++)
                    {
                        for (int j = 0; j < nx; j++)
                        {
                            X[i, j] -= X[k, j] * _QR[i, k];
                        }
                    }
                }

                return X[0, ColumnCount - 1, 0, nx - 1];
            }

            #region Dispose

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected unsafe virtual void Dispose(bool disposing)
            {
                if (false == disposed)
                {
                    disposed = true;
                    _QR.Dispose();
                    _diag.Dispose();
                }
            }

            private bool disposed;

            ~QRD()
            {
                Dispose(false);
            }

            #endregion

        }

        #endregion

        #region LUD

        /// <summary>
        /// LU 分解
        /// </summary>
        public class LUD : IDisposable
        {
            private Matrix _LU;
            private readonly int _rowCount;
            private readonly int _columnCount;
            private unsafe int* _pivot;
            private bool _isNonSingular;

            /// <summary>
            /// LU Decomposition
            /// </summary>
            /// <param name="A">矩阵</param>
            /// <returns>矩阵的LU分解</returns>
            public unsafe LUD(Matrix A)
            {
                _LU = A.Clone();
                _rowCount = A.RowCount;
                _columnCount = A.ColumnCount;

                _pivot = (int*)Marshal.AllocHGlobal(sizeof(int) * _rowCount);
                for (int i = 0; i < _rowCount; i++)
                {
                    _pivot[i] = i;
                }

                using (Matrix v = new Matrix(_rowCount))
                {
                    for (int j = 0; j < _columnCount; j++)
                    {
                        for (int i = 0; i < v.Length; i++)
                        {
                            v[i] = _LU[i, j];
                        }

                        for (int i = 0; i < v.Length; i++)
                        {
                            int kmax = Math.Min(i, j);
                            double s = 0.0;
                            for (int k = 0; k < kmax; k++)
                            {
                                s += _LU[i, k] * v[k];
                            }

                            _LU[i, j] = v[i] -= s;
                        }

                        int p = j;

                        for (int i = j + 1; i < v.Length; i++)
                        {
                            if (Math.Abs(v[i]) > Math.Abs(v[p]))
                            {
                                p = i;
                            }
                        }

                        if (p != j)
                        {
                            for (int k = 0; k < _columnCount; k++)
                            {
                                double t = _LU[p, k];
                                _LU[p, k] = _LU[j, k];
                                _LU[j, k] = t;
                            }

                            int k2 = _pivot[p];
                            _pivot[p] = _pivot[j];
                            _pivot[j] = k2;
                        }

                        if ((j < _rowCount) && (_LU[j, j] != 0.0))
                        {
                            for (int i = j + 1; i < _rowCount; i++)
                            {
                                _LU[i, j] /= _LU[j, j];
                            }
                        }
                    }
                }

                _isNonSingular = CheckIsNonSingular();
            }

            private bool CheckIsNonSingular()
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    if (_LU[j, j] == 0.0)
                    {
                        return false;
                    }
                }

                return true;
            }

            public unsafe Matrix Solve(Matrix B)
            {
                if (B.RowCount != _rowCount)
                {
                    throw new ArgumentException("B");
                }

                if (_isNonSingular == false)
                {
                    throw new InvalidOperationException("Not Singular Matrix");
                }

                int bColumnCount = B.ColumnCount;
                Matrix X = CreateSubMatrix(B, _pivot, _rowCount, 0, bColumnCount - 1);

                for (int k = 0; k < _columnCount; k++)
                {
                    for (int i = k + 1; i < _columnCount; i++)
                    {
                        for (int j = 0; j < bColumnCount; j++)
                        {
                            X[i, j] -= X[k, j] * _LU[i, k];
                        }
                    }
                }

                for (int k = _columnCount - 1; k >= 0; k--)
                {
                    for (int j = 0; j < bColumnCount; j++)
                    {
                        X[k, j] /= _LU[k, k];
                    }

                    for (int i = 0; i < k; i++)
                    {
                        for (int j = 0; j < bColumnCount; j++)
                        {
                            X[i, j] -= X[k, j] * _LU[i, k];
                        }
                    }
                }

                return X;
            }

            private unsafe Matrix CreateSubMatrix(Matrix m2, int* r, int rowCount, int j0, int j1)
            {
                Matrix m = new Matrix(rowCount, j1 - j0 + 1);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        m[i, j - j0] = m2[r[i], j];
                    }
                }
                return m;
            }

            #region Dispose

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected unsafe virtual void Dispose(bool disposing)
            {
                if (false == disposed)
                {
                    disposed = true;
                    Marshal.FreeHGlobal((IntPtr)_pivot);
                    _pivot = null;
                    _LU.Dispose();
                }
            }

            private bool disposed;

            ~LUD()
            {
                Dispose(false);
            }

            #endregion
        }

        #endregion

    }
}
