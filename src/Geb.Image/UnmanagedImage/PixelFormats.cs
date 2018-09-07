using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace Geb.Image
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct Bgra32
    {
        public static Bgra32 WHITE = new Bgra32 { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static Bgra32 BLACK = new Bgra32 { Alpha = 255 };
        public static Bgra32 RED = new Bgra32 { Red = 255, Alpha = 255 };
        public static Bgra32 BLUE = new Bgra32 { Blue = 255, Alpha = 255 };
        public static Bgra32 GREEN = new Bgra32 { Green = 255, Alpha = 255 };
        public static Bgra32 EMPTY = new Bgra32 { };

        [FieldOffset(0)]
        public Byte Blue;
        [FieldOffset(1)]
        public Byte Green;
        [FieldOffset(2)]
        public Byte Red;
        [FieldOffset(3)]
        public Byte Alpha;

        public Bgra32(int red, int green, int blue, int alpha = 255)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
            Alpha = (byte)alpha;
        }

        public Bgra32(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Bgr24* c)
        {
            this.Alpha = 255;
            this.Red = c->Red;
            this.Green = c->Green;
            this.Blue = c->Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Bgr24 c)
        {
            this.Alpha = 255;
            this.Red = c.Red;
            this.Green = c.Green;
            this.Blue = c.Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(ref Bgr24 c)
        {
            this.Alpha = 255;
            this.Red = c.Red;
            this.Green = c.Green;
            this.Blue = c.Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void To(Bgr24* c)
        {
            c->Blue = this.Blue;
            c->Green = this.Green;
            c->Red = this.Red;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void To(ref Bgr24 c)
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
