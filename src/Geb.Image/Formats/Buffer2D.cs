using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Represents a buffer of value type objects
    /// interpreted as a 2D region of <see cref="Width"/> x <see cref="Height"/> elements.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    internal class Buffer2D<T> : IBuffer2D<T>, IDisposable
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer2D{T}"/> class.
        /// </summary>
        /// <param name="wrappedBuffer">The buffer to wrap</param>
        /// <param name="width">The number of elements in a row</param>
        /// <param name="height">The number of rows</param>
        public Buffer2D(IBuffer<T> wrappedBuffer, int width, int height)
        {
            this.Buffer = wrappedBuffer;
            this.Width = width;
            this.Height = height;
        }

        /// <inheritdoc />
        public int Width { get; private set; }

        /// <inheritdoc />
        public int Height { get; private set; }

        /// <summary>
        /// Gets the span to the whole area.
        /// </summary>
        public Span<T> Span => this.Buffer.Span;

        /// <summary>
        /// Gets the backing <see cref="IBuffer{T}"/>
        /// </summary>
        public IBuffer<T> Buffer { get; private set; }

        /// <summary>
        /// Gets a reference to the element at the specified position.
        /// </summary>
        /// <param name="x">The x coordinate (row)</param>
        /// <param name="y">The y coordinate (position at row)</param>
        /// <returns>A reference to the element.</returns>
        public ref T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Span<T> span = this.Buffer.Span;
                return ref span[(this.Width * y) + x];
            }
        }

        /// <summary>
        /// Disposes the <see cref="Buffer2D{T}"/> instance
        /// </summary>
        public void Dispose()
        {
            this.Buffer?.Dispose();
        }

        /// <summary>
        /// Swap the contents (<see cref="Buffer"/>, <see cref="Width"/>, <see cref="Height"/>) of the two buffers.
        /// Useful to transfer the contents of a temporary <see cref="Buffer2D{T}"/> to a persistent <see cref="ImageFrame{TPixel}.PixelBuffer"/>
        /// </summary>
        /// <param name="a">The first buffer</param>
        /// <param name="b">The second buffer</param>
        public static void SwapContents(Buffer2D<T> a, Buffer2D<T> b)
        {
            Size aSize = a.Size();
            Size bSize = b.Size();

            IBuffer<T> temp = a.Buffer;
            a.Buffer = b.Buffer;
            b.Buffer = temp;

            b.Width = aSize.Width;
            b.Height = aSize.Height;

            a.Width = bSize.Width;
            a.Height = bSize.Height;
        }
    }

    /// <summary>
    /// Defines extension methods for <see cref="IBuffer2D{T}"/>.
    /// </summary>
    internal static class Buffer2DExtensions
    {
        /// <summary>
        /// Gets a <see cref="Span{T}"/> to the row 'y' beginning from the pixel at 'x'.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="x">The x coordinate (position in the row)</param>
        /// <param name="y">The y (row) coordinate</param>
        /// <typeparam name="T">The element type</typeparam>
        /// <returns>The <see cref="Span{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> GetRowSpan<T>(this IBuffer2D<T> buffer, int x, int y)
            where T : struct
        {
            return buffer.Span.Slice((y * buffer.Width) + x, buffer.Width - x);
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> to the row 'y' beginning from the pixel at the first pixel on that row.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="y">The y (row) coordinate</param>
        /// <typeparam name="T">The element type</typeparam>
        /// <returns>The <see cref="Span{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> GetRowSpan<T>(this IBuffer2D<T> buffer, int y)
            where T : struct
        {
            return buffer.Span.Slice(y * buffer.Width, buffer.Width);
        }

        /// <summary>
        /// Returns the size of the buffer.
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="buffer">The <see cref="IBuffer2D{T}"/></param>
        /// <returns>The <see cref="Size{T}"/> of the buffer</returns>
        public static Size Size<T>(this IBuffer2D<T> buffer)
            where T : struct
        {
            return new Size(buffer.Width, buffer.Height);
        }

        ///// <summary>
        ///// Returns a <see cref="Rectangle"/> representing the full area of the buffer.
        ///// </summary>
        ///// <typeparam name="T">The element type</typeparam>
        ///// <param name="buffer">The <see cref="IBuffer2D{T}"/></param>
        ///// <returns>The <see cref="Rectangle"/></returns>
        //public static Rectangle FullRectangle<T>(this IBuffer2D<T> buffer)
        //    where T : struct
        //{
        //    return new Rectangle(0, 0, buffer.Width, buffer.Height);
        //}

        ///// <summary>
        ///// Return a <see cref="BufferArea{T}"/> to the subarea represented by 'rectangle'
        ///// </summary>
        ///// <typeparam name="T">The element type</typeparam>
        ///// <param name="buffer">The <see cref="IBuffer2D{T}"/></param>
        ///// <param name="rectangle">The rectangle subarea</param>
        ///// <returns>The <see cref="BufferArea{T}"/></returns>
        //public static BufferArea<T> GetArea<T>(this IBuffer2D<T> buffer, Rectangle rectangle)
        //    where T : struct => new BufferArea<T>(buffer, rectangle);

        //public static BufferArea<T> GetArea<T>(this IBuffer2D<T> buffer, int x, int y, int width, int height)
        //    where T : struct
        //{
        //    var rectangle = new Rectangle(x, y, width, height);
        //    return new BufferArea<T>(buffer, rectangle);
        //}

        ///// <summary>
        ///// Return a <see cref="BufferArea{T}"/> to the whole area of 'buffer'
        ///// </summary>
        ///// <typeparam name="T">The element type</typeparam>
        ///// <param name="buffer">The <see cref="IBuffer2D{T}"/></param>
        ///// <returns>The <see cref="BufferArea{T}"/></returns>
        //public static BufferArea<T> GetArea<T>(this IBuffer2D<T> buffer)
        //    where T : struct => new BufferArea<T>(buffer);
    }
}
