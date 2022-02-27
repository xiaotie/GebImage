using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Image
{
    public static class PixelHelper
    {
        /// <summary>
        /// 反色
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Byte Invert(this Byte p)
        {
            return (byte)(255 - p);
        }
    }
}
