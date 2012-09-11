/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public abstract class UnmanagedImage<T> : IDisposable, IImage
        where T : struct
    {
        /// <summary>
        /// 图像所占字节数。
        /// </summary>
        public Int32 ByteCount { get; private set; }

        /// <summary>
        /// 图像的像素数量
        /// </summary>
        public Int32 Length { get; private set; }

        /// <summary>
        /// 每像素的尺寸（字节数）
        /// </summary>
        public Int32 SizeOfType { get; private set; }

        /// <summary>
        /// 图像宽（像素）
        /// </summary>
        public Int32 Width { get; protected set; }

        /// <summary>
        /// 图像的高（像素）
        /// </summary>
        public Int32 Height { get; protected set; }

        /// <summary>
        /// 图像的起始指针。
        /// </summary>
        public IntPtr StartIntPtr { get; private set; }

        public Size<Int32> ImageSize
        {
            get { return new Size<Int32>(Width, Height); }
        }

        private IColorConverter m_converter;
        private unsafe Byte* _start;

        /// <summary>
        /// 感兴趣区域。目前尚无用途。
        /// </summary>
        public ROI ROI { get; private set; }

        public unsafe UnmanagedImage(Int32 width, Int32 height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            else if (height <= 0) throw new ArgumentOutOfRangeException("height");
            AllocMemory(width,height);
        }

        private unsafe void AllocMemory(int width, int height)
        {
            Height = height;
            Width = width;
            Length = Width * Height;
            SizeOfType = SizeOfT();
            ByteCount = SizeOfType * Length;
            m_converter = this.CreateByteConverter();
            StartIntPtr = Marshal.AllocHGlobal(ByteCount);
            _start = (Byte*)StartIntPtr;
        }

        public unsafe UnmanagedImage(String path)
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                AllocMemory(bmp.Width, bmp.Height);
                this.CreateFromBitmap(bmp);
            }
        }

        public UnmanagedImage(Bitmap map)
        {
            if (map == null) throw new ArgumentNullException("map");
            AllocMemory(map.Width, map.Height);
            this.CreateFromBitmap(map);
        }

        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                 disposed = true;
                 Marshal.FreeHGlobal(StartIntPtr);
            }
        }

        private bool disposed;

        ~UnmanagedImage()
        {
            Dispose(false);
        }

        private static Int32 SizeOfT()
        {
            return Marshal.SizeOf(typeof(T));
        }

        protected virtual unsafe void CreateFromBitmap(Bitmap map)
        {
            int height = map.Height;
            int width = map.Width;

            const int PixelFormat32bppCMYK = 8207;

            PixelFormat format = map.PixelFormat;

            this.Width = width;
            this.Height = height;

            Bitmap newMap = map;
            Int32 step = SizeOfT();

            switch (format)
            {
                case PixelFormat.Format24bppRgb:
                    break;
                case PixelFormat.Format32bppArgb:
                    break;
                default:
                    if ((int)format == PixelFormat32bppCMYK)
                    {
                        format = PixelFormat.Format24bppRgb;
                        newMap = new Bitmap(width, height, format);
                        using (Graphics g = Graphics.FromImage(newMap))
                        {
                            g.DrawImage(map, new Point());
                        }
                    }
                    else
                    {
                        format = PixelFormat.Format32bppArgb;
                        newMap = map.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);
                    }
                    break;
            }

            BitmapData data = newMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
            Byte* line = (Byte*)data.Scan0;
            Byte* dstLine = (Byte*)StartIntPtr;
            try
            {
                if (format == PixelFormat.Format24bppRgb)
                {
                    for (int h = 0; h < height; h++)
                    {
                        m_converter.Copy((Rgb24*)line, (void*)dstLine, width);
                        line += data.Stride;
                        dstLine += step * width;
                    }
                }
                else
                {
                    for (int h = 0; h < height; h++)
                    {
                        m_converter.Copy((Argb32*)line, (void*)dstLine, width);

                        line += data.Stride;
                        dstLine += step * width;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                newMap.UnlockBits(data);
                if (newMap != map)
                {
                    newMap.Dispose();
                }
            }
        }

        public virtual unsafe Bitmap ToBitmap()
        {
            Bitmap map = new Bitmap(this.Width, this.Height, GetOutputBitmapPixelFormat());
            InitPalette(map);
            ToBitmap(map);
            return map;
        }

        protected virtual void InitPalette(Bitmap map)
        {
        }

        public virtual unsafe void ToBitmap(Bitmap map)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (map.Width != this.Width || map.Height != this.Height)
            {
                throw new ArgumentException("尺寸不匹配.");
            }

            if (map.PixelFormat != GetOutputBitmapPixelFormat())
            {
                throw new ArgumentException("只支持 " + GetOutputBitmapPixelFormat().ToString() + " 格式。 ");
            }

            Int32 step = SizeOfT();
            Byte* srcLine = (Byte*)StartIntPtr;

            BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadWrite, map.PixelFormat);
            try
            {
                int width = map.Width;
                int height = map.Height;
                Byte* dstLine = (Byte*)data.Scan0;
                for (int h = 0; h < height; h++)
                {
                    ToBitmapCore(srcLine, dstLine, width);
                    dstLine += data.Stride;
                    srcLine += step * width;
                }
            }
            finally
            {
                map.UnlockBits(data);
            }
        }

        public void ApplyMatrix(float a, float b, float c, float d, float e, float f)
        {
            //TODO: ApplyMatrix
            throw new NotImplementedException();
        }

        public void Resize(UnmanagedImage<T> dst)
        {
            //TODO: Resize
            throw new NotImplementedException();
        }

        protected abstract PixelFormat GetOutputBitmapPixelFormat();

        protected abstract unsafe void ToBitmapCore(byte* src, byte* dst, int width);

        public abstract IImage Clone();

        protected abstract IColorConverter CreateByteConverter();
    }
}
