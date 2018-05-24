using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Geb.Image.Formats
{
    /// <summary>
    /// Contains common factory methods and configuration constants.
    /// </summary>
    public partial class ArrayPoolMemoryManager
    {
        /// <summary>
        /// The default value for: maximum size of pooled arrays in bytes.
        /// Currently set to 24MB, which is equivalent to 8 megapixels of raw <see cref="Rgba32"/> data.
        /// </summary>
        internal const int DefaultMaxPooledBufferSizeInBytes = 24 * 1024 * 1024;

        /// <summary>
        /// The value for: The threshold to pool arrays in <see cref="largeArrayPool"/> which has less buckets for memory safety.
        /// </summary>
        private const int DefaultBufferSelectorThresholdInBytes = 8 * 1024 * 1024;

        /// <summary>
        /// The default bucket count for <see cref="largeArrayPool"/>.
        /// </summary>
        private const int DefaultLargePoolBucketCount = 6;

        /// <summary>
        /// The default bucket count for <see cref="normalArrayPool"/>.
        /// </summary>
        private const int DefaultNormalPoolBucketCount = 16;

        /// <summary>
        /// This is the default. Should be good for most use cases.
        /// </summary>
        /// <returns>The memory manager</returns>
        public static ArrayPoolMemoryManager CreateDefault()
        {
            return new ArrayPoolMemoryManager(
                DefaultMaxPooledBufferSizeInBytes,
                DefaultBufferSelectorThresholdInBytes,
                DefaultLargePoolBucketCount,
                DefaultNormalPoolBucketCount);
        }

        /// <summary>
        /// For environments with limited memory capabilities. Only small images are pooled, which can result in reduced througput.
        /// </summary>
        /// <returns>The memory manager</returns>
        public static ArrayPoolMemoryManager CreateWithModeratePooling()
        {
            return new ArrayPoolMemoryManager(1024 * 1024, 32 * 1024, 16, 24);
        }

        /// <summary>
        /// Only pool small buffers like image rows.
        /// </summary>
        /// <returns>The memory manager</returns>
        public static ArrayPoolMemoryManager CreateWithMinimalPooling()
        {
            return new ArrayPoolMemoryManager(64 * 1024, 32 * 1024, 8, 24);
        }

        /// <summary>
        /// RAM is not an issue for me, gimme maximum througput!
        /// </summary>
        /// <returns>The memory manager</returns>
        public static ArrayPoolMemoryManager CreateWithAggressivePooling()
        {
            return new ArrayPoolMemoryManager(128 * 1024 * 1024, 32 * 1024 * 1024, 16, 32);
        }
    }

    /// <summary>
    /// Implements <see cref="MemoryManager"/> by allocating memory from <see cref="ArrayPool{T}"/>.
    /// </summary>
    public partial class ArrayPoolMemoryManager : MemoryManager
    {
        /// <summary>
        /// The <see cref="ArrayPool{T}"/> for small-to-medium buffers which is not kept clean.
        /// </summary>
        private ArrayPool<byte> normalArrayPool;

        /// <summary>
        /// The <see cref="ArrayPool{T}"/> for huge buffers, which is not kept clean.
        /// </summary>
        private ArrayPool<byte> largeArrayPool;

        private readonly int maxArraysPerBucketNormalPool;

        private readonly int maxArraysPerBucketLargePool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPoolMemoryManager"/> class.
        /// </summary>
        public ArrayPoolMemoryManager()
            : this(DefaultMaxPooledBufferSizeInBytes, DefaultBufferSelectorThresholdInBytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPoolMemoryManager"/> class.
        /// </summary>
        /// <param name="maxPoolSizeInBytes">The maximum size of pooled arrays. Arrays over the thershold are gonna be always allocated.</param>
        public ArrayPoolMemoryManager(int maxPoolSizeInBytes)
            : this(maxPoolSizeInBytes, GetLargeBufferThresholdInBytes(maxPoolSizeInBytes))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPoolMemoryManager"/> class.
        /// </summary>
        /// <param name="maxPoolSizeInBytes">The maximum size of pooled arrays. Arrays over the thershold are gonna be always allocated.</param>
        /// <param name="poolSelectorThresholdInBytes">Arrays over this threshold will be pooled in <see cref="largeArrayPool"/> which has less buckets for memory safety.</param>
        public ArrayPoolMemoryManager(int maxPoolSizeInBytes, int poolSelectorThresholdInBytes)
            : this(maxPoolSizeInBytes, poolSelectorThresholdInBytes, DefaultLargePoolBucketCount, DefaultNormalPoolBucketCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPoolMemoryManager"/> class.
        /// </summary>
        /// <param name="maxPoolSizeInBytes">The maximum size of pooled arrays. Arrays over the thershold are gonna be always allocated.</param>
        /// <param name="poolSelectorThresholdInBytes">The threshold to pool arrays in <see cref="largeArrayPool"/> which has less buckets for memory safety.</param>
        /// <param name="maxArraysPerBucketLargePool">Max arrays per bucket for the large array pool</param>
        /// <param name="maxArraysPerBucketNormalPool">Max arrays per bucket for the normal array pool</param>
        public ArrayPoolMemoryManager(int maxPoolSizeInBytes, int poolSelectorThresholdInBytes, int maxArraysPerBucketLargePool, int maxArraysPerBucketNormalPool)
        {
            Guard.MustBeGreaterThan(maxPoolSizeInBytes, 0, nameof(maxPoolSizeInBytes));
            Guard.MustBeLessThanOrEqualTo(poolSelectorThresholdInBytes, maxPoolSizeInBytes, nameof(poolSelectorThresholdInBytes));

            this.MaxPoolSizeInBytes = maxPoolSizeInBytes;
            this.PoolSelectorThresholdInBytes = poolSelectorThresholdInBytes;
            this.maxArraysPerBucketLargePool = maxArraysPerBucketLargePool;
            this.maxArraysPerBucketNormalPool = maxArraysPerBucketNormalPool;

            this.InitArrayPools();
        }

        /// <summary>
        /// Gets the maximum size of pooled arrays in bytes.
        /// </summary>
        public int MaxPoolSizeInBytes { get; }

        /// <summary>
        /// Gets the threshold to pool arrays in <see cref="largeArrayPool"/> which has less buckets for memory safety.
        /// </summary>
        public int PoolSelectorThresholdInBytes { get; }

        /// <inheritdoc />
        public override void ReleaseRetainedResources()
        {
            this.InitArrayPools();
        }

        /// <inheritdoc />
        internal override IBuffer<T> Allocate<T>(int length, bool clear)
        {
            int itemSizeBytes = Unsafe.SizeOf<T>();
            int bufferSizeInBytes = length * itemSizeBytes;

            ArrayPool<byte> pool = this.GetArrayPool(bufferSizeInBytes);
            byte[] byteArray = pool.Rent(bufferSizeInBytes);

            var buffer = new Buffer<T>(byteArray, length, pool);
            if (clear)
            {
                buffer.Clear();
            }

            return buffer;
        }

        /// <inheritdoc />
        internal override IManagedByteBuffer AllocateManagedByteBuffer(int length, bool clear)
        {
            ArrayPool<byte> pool = this.GetArrayPool(length);
            byte[] byteArray = pool.Rent(length);

            var buffer = new ManagedByteBuffer(byteArray, length, pool);
            if (clear)
            {
                buffer.Clear();
            }

            return buffer;
        }

        private static int GetLargeBufferThresholdInBytes(int maxPoolSizeInBytes)
        {
            return maxPoolSizeInBytes / 4;
        }

        private ArrayPool<byte> GetArrayPool(int bufferSizeInBytes)
        {
            return bufferSizeInBytes <= this.PoolSelectorThresholdInBytes ? this.normalArrayPool : this.largeArrayPool;
        }

        private void InitArrayPools()
        {
            this.largeArrayPool = ArrayPool<byte>.Create(this.MaxPoolSizeInBytes, this.maxArraysPerBucketLargePool);
            this.normalArrayPool = ArrayPool<byte>.Create(this.PoolSelectorThresholdInBytes, this.maxArraysPerBucketNormalPool);
        }
    }

    // <summary>
    /// Contains <see cref="Buffer{T}"/> and <see cref="ManagedByteBuffer"/>
    /// </summary>
    public partial class ArrayPoolMemoryManager
    {
        /// <summary>
        /// The buffer implementation of <see cref="ArrayPoolMemoryManager"/>
        /// </summary>
        private class Buffer<T> : IBuffer<T>
            where T : struct
        {
            /// <summary>
            /// The length of the buffer
            /// </summary>
            private readonly int length;

            /// <summary>
            /// A weak reference to the source pool.
            /// </summary>
            /// <remarks>
            /// By using a weak reference here, we are making sure that array pools and their retained arrays are always GC-ed
            /// after a call to <see cref="ArrayPoolMemoryManager.ReleaseRetainedResources"/>, regardless of having buffer instances still being in use.
            /// </remarks>
            private WeakReference<ArrayPool<byte>> sourcePoolReference;

            public Buffer(byte[] data, int length, ArrayPool<byte> sourcePool)
            {
                this.Data = data;
                this.length = length;
                this.sourcePoolReference = new WeakReference<ArrayPool<byte>>(sourcePool);
            }

            /// <summary>
            /// Gets the buffer as a byte array.
            /// </summary>
            protected byte[] Data { get; private set; }

            /// <inheritdoc />
            public Span<T> Span => MemoryMarshal.Cast<byte, T>(this.Data.AsSpan()).Slice(0, this.length);

            /// <inheritdoc />
            public void Dispose()
            {
                if (this.Data == null || this.sourcePoolReference == null)
                {
                    return;
                }

                if (this.sourcePoolReference.TryGetTarget(out ArrayPool<byte> pool))
                {
                    pool.Return(this.Data);
                }

                this.sourcePoolReference = null;
                this.Data = null;
            }
        }

        /// <summary>
        /// The <see cref="IManagedByteBuffer"/> implementation of <see cref="ArrayPoolMemoryManager"/>.
        /// </summary>
        private class ManagedByteBuffer : Buffer<byte>, IManagedByteBuffer
        {
            public ManagedByteBuffer(byte[] data, int length, ArrayPool<byte> sourcePool)
                : base(data, length, sourcePool)
            {
            }

            public byte[] Array => this.Data;
        }
    }
}
