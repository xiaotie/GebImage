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
    using Geb.Utils;

    public class Image<T> : IDisposable
        where T:struct
    {
        public unsafe Byte* Data { get; private set; }

        public unsafe Image(int width, int height = 1)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            else if (height <= 0) throw new ArgumentOutOfRangeException("height");
            Data = (Byte*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * width * height);
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
