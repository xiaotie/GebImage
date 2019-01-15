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

        public Bgra32(int blue, int green, int red, int alpha = 255)
        {
            Blue = (byte)blue;
            Green = (byte)green;
            Red = (byte)red;
            Alpha = (byte)alpha;
        }

        public Bgra32(byte blue, byte green, byte red, byte alpha = 255)
        {
            Blue = blue;
            Green = green;
            Red = red;
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
            return "Bgra32 [A=" + Alpha + ", R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct Rgba32
    {
        public static Rgba32 WHITE = new Rgba32 { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        public static Rgba32 BLACK = new Rgba32 { Alpha = 255 };
        public static Rgba32 RED = new Rgba32 { Red = 255, Alpha = 255 };
        public static Rgba32 BLUE = new Rgba32 { Blue = 255, Alpha = 255 };
        public static Rgba32 GREEN = new Rgba32 { Green = 255, Alpha = 255 };
        public static Rgba32 EMPTY = new Rgba32 { };

        [FieldOffset(0)]
        public Byte Red;
        [FieldOffset(1)]
        public Byte Green;
        [FieldOffset(2)]
        public Byte Blue;
        [FieldOffset(3)]
        public Byte Alpha;

        public Rgba32(int red, int green, int blue, int alpha = 255)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
            Alpha = (byte)alpha;
        }

        public Rgba32(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Rgba32* c)
        {
            this.Alpha = 255;
            this.Red = c->Red;
            this.Green = c->Green;
            this.Blue = c->Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Rgba32 c)
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
            return "Rgba32 [A=" + Alpha + ", R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct Rgb24
    {
        public static Rgb24 WHITE = new Rgb24 { Red = 255, Green = 255, Blue = 255};
        public static Rgb24 BLACK = new Rgb24 {  };
        public static Rgb24 RED = new Rgb24 { Red = 255};
        public static Rgb24 BLUE = new Rgb24 { Blue = 255};
        public static Rgb24 GREEN = new Rgb24 { Green = 255};
        public static Rgb24 EMPTY = new Rgb24 { };

        [FieldOffset(0)]
        public Byte Red;
        [FieldOffset(1)]
        public Byte Green;
        [FieldOffset(2)]
        public Byte Blue;

        public Rgb24(int red, int green, int blue)
        {
            Red = (byte)red;
            Green = (byte)green;
            Blue = (byte)blue;
        }

        public Rgb24(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Bgr24* c)
        {
            this.Red = c->Red;
            this.Green = c->Green;
            this.Blue = c->Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(Bgr24 c)
        {
            this.Red = c.Red;
            this.Green = c.Green;
            this.Blue = c.Blue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void From(ref Bgr24 c)
        {
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
            return "Rgb24 [R=" + Red.ToString() + ", G=" + Green.ToString() + ", B=" + Blue.ToString() + "]";
        }
    }
}
