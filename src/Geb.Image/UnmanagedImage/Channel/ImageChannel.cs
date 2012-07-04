/*************************************************************************
 *  Copyright (c) 2010 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Image
{
    public class ImageChannel<T> : IDisposable where T : struct
    {
        /// <summary>
        /// ImageChannel 所包含的像素数量
        /// </summary>
        public Int32 PixelCount { get; private set; }

        public Int32 PixelSize { get; private set; }
        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }
        public Int32 SizeOfType { get; private set; }

        /// <summary>
        /// 每行图像所占的字节数
        /// </summary>
        public Int32 Stride { get; private set; }

        public IntPtr StartIntPtr { get; private set; }
        public Int32 Step { get { return PixelSize / SizeOfType; } }
        public Boolean IsMemoryManager { get; private set; }

        private static Int32 SizeOfT()
        {
            return Marshal.SizeOf(typeof(T));
        }

        public ImageChannel(int width, int height, IntPtr start, int pixelSize)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            else if (height <= 0) throw new ArgumentOutOfRangeException("height");
            else if (pixelSize <= 0) throw new ArgumentOutOfRangeException("pixelSize");

            this.Width = width;
            this.Height = height;
            this.PixelCount = width * height;
            this.StartIntPtr = start;
            this.PixelSize = pixelSize;
            this.SizeOfType = SizeOfT();
        }

        public ImageChannel(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            else if (height <= 0) throw new ArgumentOutOfRangeException("height");
            this.Width = width;
            this.Height = height;
            this.PixelCount = width * height;
            this.SizeOfType = SizeOfT();
            this.PixelSize = this.SizeOfType;
            IsMemoryManager = true;
            StartIntPtr = Marshal.AllocHGlobal(SizeOfType * PixelCount);
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
                 if (IsMemoryManager == true)
                 {
                     Marshal.FreeHGlobal(StartIntPtr);
                 }
            }
        }

        private bool disposed;

        ~ImageChannel()
        {
            Dispose(false);
        }
    }
}
