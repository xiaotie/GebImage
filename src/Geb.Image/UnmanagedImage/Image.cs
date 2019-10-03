/*************************************************************************
 *  Copyright (c) 2013 Hu Fei(xiaotie@geblab.com; geblab, www.geblab.com)
 *  
 *  建立时间: 3/8/2013 11:14:37 AM
 *  修改记录:
 * 
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    public class Image<T> : IDisposable
        where T:struct
    {
        public unsafe Byte* Data { get; private set; }

        public int ByteLength { get; private set; }

        public unsafe Span<T> DataSpan { get { return new Span<T>((void*)Data, ByteLength); } }

        public unsafe ReadOnlySpan<T> DataSpanReadOnly { get { return new ReadOnlySpan<T>((void*)Data, ByteLength); } }

        public unsafe Image(int width, int height = 1)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            else if (height <= 0) throw new ArgumentOutOfRangeException("height");
            ByteLength = Marshal.SizeOf(typeof(T)) * width * height;
            Data = (Byte*)Marshal.AllocHGlobal(ByteLength);
        }

        public unsafe void Dispose()
        {
            if (Data == null) return;
            Marshal.FreeHGlobal((IntPtr)Data);
            Data = null;
        }

        ~Image()
        {
            Dispose();
        }
    }
}
