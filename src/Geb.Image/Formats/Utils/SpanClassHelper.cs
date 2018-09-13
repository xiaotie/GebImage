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

        public static void ToRgba32Bytes(this ReadOnlySpan<Bgra32> rowSpan, Span<Byte> span, int width)
        {
            Span<Rgba32> dstSpan = MemoryMarshal.Cast<byte, Rgba32>(span);
            for (int i = 0; i < width; i++)
            {
                Bgra32 val = rowSpan[i];
                dstSpan[i] = new Rgba32(val.Red, val.Green, val.Blue);
            }
        }

        public static void ToRgb24Bytes(this ReadOnlySpan<Bgra32> rowSpan, Span<Byte> span, int width)
        {
            Span<Rgba32> dstSpan = MemoryMarshal.Cast<byte, Rgba32>(span);
            for (int i = 0; i < width; i++)
            {
                Bgra32 val = rowSpan[i];
                dstSpan[i] = new Rgba32(val.Red, val.Green, val.Blue);
            }
        }
    }
}
