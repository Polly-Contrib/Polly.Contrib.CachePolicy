using System.Diagnostics;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy.Providers.Compressor
{
    /// <summary>
    /// An implementation of <see cref="IPlaintextCompressor"/> which does not do any compression
    /// </summary>
    public class NoOpPlaintextCompressor : IPlaintextCompressor
    {
        /// <summary>
        /// Defines the contract to logging <see cref="AsyncCachePolicy{TResult}"/> metrics and traces.
        /// </summary>
        private ILoggingProvider loggingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoOpPlaintextCompressor"/> class.
        /// </summary>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations.</param>
        public NoOpPlaintextCompressor(ILoggingProvider loggingProvider)
        {
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc/>
        public string Compress(string input, Context context)
        {
            var stopwatch = Stopwatch.StartNew();
            this.loggingProvider.OnCacheCompress(
                                        context.GetOperationName(),
                                        this.GetType().Name,
                                        stopwatch.ElapsedMilliseconds,
                                        input.Length,
                                        context);

            return input;
        }

        /// <inheritdoc/>
        public string Decompress(string input, Context context)
        {
            var stopwatch = Stopwatch.StartNew();
            this.loggingProvider.OnCacheDecompress(
                                        context.GetOperationName(),
                                        this.GetType().Name,
                                        stopwatch.ElapsedMilliseconds,
                                        context);

            return input;
        }
    }
}
