using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Image
{
    public unsafe class MemoryStorage : IDisposable
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        internal static extern IntPtr memcpy(byte* dest, byte* src, uint count);

        public int _bytes;
        public int Bytes
        {
            get { return _bytes; }
        }

        private byte* _start;

        public Byte* Start
        {
            get { return _start; }
        }
        
        public MemoryStorage(int bytes)
        {
            if (bytes < 0) throw new ArgumentOutOfRangeException("bytes 不能为负");
            _bytes = bytes;
            IntPtr ptr = Marshal.AllocHGlobal(bytes);
            _start = (Byte*)ptr;
        }

        public void Resize(int newSize)
        {
            if (newSize < 0) throw new ArgumentOutOfRangeException("newSize 不能为负");
            byte* newStart = (Byte*)Marshal.AllocHGlobal(newSize);
            if(_start != null)
            {
                int size = Math.Min(_bytes, newSize);
                MemoryStorage.memcpy(newStart, _start, (uint)size);
                Marshal.FreeHGlobal((IntPtr)_start);
            }
            _start = newStart;
        }

        public void Dispose()
        {
            if(_start != null)
            {
                IntPtr ptr = (IntPtr)_start;
                _start = null;
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
