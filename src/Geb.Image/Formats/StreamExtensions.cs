using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Extension methods for the <see cref="Stream"/> type.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Skips the number of bytes in the given stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="count">The count.</param>
        public static void Skip(this Stream stream, int count)
        {
            if (count < 1)
            {
                return;
            }

            if (stream.CanSeek)
            {
                stream.Seek(count, SeekOrigin.Current); // Position += count;
            }
            else
            {
                byte[] foo = new byte[count];
                while (count > 0)
                {
                    int bytesRead = stream.Read(foo, 0, count);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    count -= bytesRead;
                }
            }
        }
    }
}
