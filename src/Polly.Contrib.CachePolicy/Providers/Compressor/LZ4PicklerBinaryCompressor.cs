using System.Diagnostics;
using K4os.Compression.LZ4;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy.Providers.Compressor
{
    /// <summary>
    /// An implementation of binary compressor based on LZ4 Pickler compression.
    /// </summary>
    public class LZ4PicklerBinaryCompressor : IBinaryCompressor
    {
        /// <summary>
        /// Defines the contract to logging <see cref="AsyncCachePolicy{TResult}"/> metrics and traces.
        /// </summary>
        private ILoggingProvider loggingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LZ4PicklerBinaryCompressor"/> class.
        /// </summary>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations.</param>
        public LZ4PicklerBinaryCompressor(ILoggingProvider loggingProvider)
        {
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc/>
        public byte[] Compress(byte[] input, Context context)
        {
            var stopwatch = Stopwatch.StartNew();
            var compressedByteArray = LZ4Pickler.Pickle(input);
            this.loggingProvider.OnCacheCompress(
                                        context.GetOperationName(),
                                        this.GetType().Name,
                                        stopwatch.ElapsedMilliseconds,
                                        compressedByteArray.Length,
                                        context);

            return compressedByteArray;
        }

        /// <inheritdoc/>
        public byte[] Decompress(byte[] input, Context context)
        {
            var stopwatch = Stopwatch.StartNew();
            var decompressedData = LZ4Pickler.Unpickle(input);
            this.loggingProvider.OnCacheDecompress(
                            context.GetOperationName(),
                            this.GetType().Name,
                            stopwatch.ElapsedMilliseconds,
                            context);

            return decompressedData;
        }
    }
}
