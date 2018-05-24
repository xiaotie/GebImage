using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Geb.Image.Formats
{
    public static class MathF
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float val)
        {
            
            return (float)Math.Round(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float val, MidpointRounding midpointRounding)
        {
            return (float)Math.Round(val, midpointRounding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceiling(float val)
        {

            return (float)Math.Ceiling(val);
        }
    }
}
