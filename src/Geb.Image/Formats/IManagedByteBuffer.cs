using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Represents a byte buffer backed by a managed array. Useful for interop with classic .NET API-s.
    /// </summary>
    internal interface IManagedByteBuffer : IBuffer<byte>
    {
        /// <summary>
        /// Gets the managed array backing this buffer instance.
        /// </summary>
        byte[] Array { get; }
    }
}
