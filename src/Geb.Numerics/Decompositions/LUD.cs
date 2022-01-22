using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Numerics
{
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
                if (_LU[j,j] == 0.0)
                {
                    return false;
                }
            }

            return true;
        }

        public unsafe Matrix Solve( Matrix B )
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
            Matrix X = CreateSubMatrix(B,_pivot, _rowCount, 0, bColumnCount - 1);

            for (int k = 0; k < _columnCount; k++)
            {
                for (int i = k + 1; i < _columnCount; i++)
                {
                    for (int j = 0; j < bColumnCount; j++)
                    {
                        X[i,j] -= X[k,j] * _LU[i,k];
                    }
                }
            }

            for (int k = _columnCount - 1; k >= 0; k--)
            {
                for (int j = 0; j < bColumnCount; j++)
                {
                    X[k,j] /= _LU[k,k];
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < bColumnCount; j++)
                    {
                        X[i,j] -= X[k,j] * _LU[i,k];
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
}
