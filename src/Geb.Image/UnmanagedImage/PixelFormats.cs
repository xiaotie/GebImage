using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace Geb.Image
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct Argb32
    {
        public static Argb32 WHITE = new Argb32 { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static Argb32 BLACK = new Argb32 { Alpha = 255 };
        public static Argb32 RED = new Argb32 { Red = 255, Alpha = 255 };
        public static Argb32 BLUE = new Argb32 { Blue = 255, Alpha = 255 };
        public static Argb32 GREEN = new Argb32 { Green = 255, Alpha = 255 };
        public static Argb32 EMPTY = new Argb32 { };

        [FieldOffset(0)]
        public Byte Blue;
        [FieldOffset(1)]
        public Byte Green;
        [FieldOffset(2)]
        public Byte Red;
        [FieldOffset(3)]
        public Byte Alpha;

        public Argb32(int red, int green, int blue, int alpha = 255)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
            Alpha = (byte)alpha;
        }

        public Argb32(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Rgb24* c)
        {
            this.Alpha = 255;
            this.Red = c->Red;
            this.Green = c->Green;
            this.Blue = c->Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Rgb24 c)
        {
            this.Alpha = 255;
            this.Red = c.Red;
            this.Green = c.Green;
            this.Blue = c.Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(ref Rgb24 c)
        {
            this.Alpha = 255;
            this.Red = c.Red;
            this.Green = c.Green;
            this.Blue = c.Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void To(Rgb24* c)
        {
            c->Blue = this.Blue;
            c->Green = this.Green;
            c->Red = this.Red;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void To(ref Rgb24 c)
        {
            c.Blue = this.Blue;
            c.Green = this.Green;
            c.Red = this.Red;
        }

        public Byte ToGray()
        {
            return (Byte)(0.299 * Red + 0.587 * Green + 0.114 * Blue);
        }

        public override string ToString()
        {
            return "Argb32 [A=" + Alpha + ", R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }
}
