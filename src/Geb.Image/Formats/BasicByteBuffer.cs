using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats
{
    internal class BasicByteBuffer : BasicArrayBuffer<byte>, IManagedByteBuffer
    {
        internal BasicByteBuffer(byte[] array)
            : base(array)
        {
        }
    }
}
