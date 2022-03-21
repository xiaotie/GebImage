using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image.Formats
{
    public unsafe static class SpanClassHelper
    {
        public static Span<Bgra32> GetPixelRowSpan(this ImageBgra32 pixels, int row)
        {
            return new Span<Bgra32>(pixels.Start + pixels.Width * row, pixels.Width);
        }

        public static ReadOnlySpan<Bgra32> GetPixelRowReadOnlySpan(this ImageBgra32 pixels, int row)
        {
            return new ReadOnlySpan<Bgra32>(pixels.Start + pixels.Width * row, pixels.Width);
        }

        public static ReadOnlySpan<Bgr24> GetPixelRowReadOnlySpan(this ImageBgr24 pixels, int row)
        {
            return new ReadOnlySpan<Bgr24>(pixels.Start + pixels.Width * row, pixels.Width);
        }

        public static ReadOnlySpan<Byte> GetPixelRowReadOnlySpan(this ImageU8 pixels, int row)
        {
            return new ReadOnlySpan<Byte>(pixels.Start + pixels.Width * row, pixels.Width);
        }

        public static void ToRgba32Bytes(this ReadOnlySpan<Bgra32> rowSpan, Span<Byte> span, int width)
        {
            Span<Rgba32> dstSpan = MemoryMarshal.Cast<byte, Rgba32>(span);
            for (int i = 0; i < width; i++)
            {
                Bgra32 val = rowSpan[i];
                dstSpan[i] = new Rgba32(val.Red, val.Green, val.Blue, val.Alpha);
            }
        }

        public static void ToRgb24Bytes(this ReadOnlySpan<Bgra32> rowSpan, Span<Byte> span, int width)
        {
            Span<Rgb24> dstSpan = MemoryMarshal.Cast<byte, Rgb24>(span);
            for (int i = 0; i < width; i++)
            {
                Bgra32 val = rowSpan[i];
                dstSpan[i] = new Rgb24(val.Red, val.Green, val.Blue);
            }
        }

        public static void ToRgba32Bytes(this ReadOnlySpan<Bgr24> rowSpan, Span<Byte> span, int width)
        {
            Span<Rgba32> dstSpan = MemoryMarshal.Cast<byte, Rgba32>(span);
            for (int i = 0; i < width; i++)
            {
                Bgr24 val = rowSpan[i];
                dstSpan[i] = new Rgba32(val.Red, val.Green, val.Blue);
            }
        }

        public static void ToRgb24Bytes(this ReadOnlySpan<Bgr24> rowSpan, Span<Byte> span, int width)
        {
            Span<Rgb24> dstSpan = MemoryMarshal.Cast<byte, Rgb24>(span);
            for (int i = 0; i < width; i++)
            {
                Bgr24 val = rowSpan[i];
                dstSpan[i] = new Rgb24(val.Red, val.Green, val.Blue);
            }
        }

        public static void PackFromRgba32Bytes(this Span<Bgra32> rowSpan, ReadOnlySpan<Byte> span,  int width)
        {
            ReadOnlySpan<Rgba32> sourceSpan = MemoryMarshal.Cast<byte, Rgba32>(span);
            for (int i = 0; i < width; i++)
            {
                Rgba32 val = sourceSpan[i];
                rowSpan[i] = new Bgra32(val.Blue, val.Green, val.Red, val.Alpha);
            }
        }

        public static void PackFromRgb24Bytes(this Span<Bgra32> rowSpan, ReadOnlySpan<Byte> span, int width)
        {
            ReadOnlySpan<Rgb24> sourceSpan = MemoryMarshal.Cast<byte, Rgb24>(span);
            for (int i = 0; i < width; i++)
            {
                Rgb24 val = sourceSpan[i];
                rowSpan[i] = new Bgra32(val.Blue, val.Green, val.Red);
            }
        }
    }
}
