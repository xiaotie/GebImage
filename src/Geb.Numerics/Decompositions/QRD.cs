using System;
using System.Runtime.InteropServices;

namespace Geb.Numerics
{
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
                    if (_QR[k,k] < 0)
                    {
                        norm = -norm;
                    }

                    for (int i = k; i < RowCount; i++)
                    {
                        _QR[i,k] /= norm;
                    }

                    _QR[k,k] += 1.0;

                    for (int j = k + 1; j < ColumnCount; j++)
                    {
                        double s = 0.0;
                        for (int i = k; i < RowCount; i++)
                        {
                            s += _QR[i,k] * _QR[i,j];
                        }

                        s = (-s) / _QR[k,k];
                        for (int i = k; i < RowCount; i++)
                        {
                            _QR[i,j] += s * _QR[i,k];
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
                        s += _QR[i,k] * X[i,j];
                    }

                    s = (-s) / _QR[k,k];
                    for (int i = k; i < RowCount; i++)
                    {
                        X[i,j] += s * _QR[i,k];
                    }
                }
            }

            for (int k = ColumnCount - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X[k,j] /= _diag[k];
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i,j] -= X[k,j] * _QR[i,k];
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
}
