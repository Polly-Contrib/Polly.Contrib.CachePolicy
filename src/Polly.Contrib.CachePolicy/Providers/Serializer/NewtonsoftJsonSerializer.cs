using System.Diagnostics;
using Newtonsoft.Json;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy.Providers.Serializer
{
    /// <summary>
    /// An implementation of <see cref="IPlainTextSerializer"/> based on Newtonsoft Json.
    /// </summary>
    public class NewtonsoftJsonSerializer : IPlainTextSerializer
    {
        /// <summary>
        /// A plaintext compressor to compress and decompress plaintext.
        /// </summary>
        private IPlaintextCompressor plaintextCompressor;

        /// <summary>
        /// Defines the contract to logging <see cref="AsyncCachePolicy{TResult}"/> metrics and traces.
        /// </summary>
        private ILoggingProvider loggingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
        /// </summary>
        /// <param name="plaintextCompressor">A plaintext compressor to compress and decompress plaintext.</param>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations.</param>
        public NewtonsoftJsonSerializer(
            IPlaintextCompressor plaintextCompressor,
            ILoggingProvider loggingProvider)
        {
            plaintextCompressor.ThrowIfNull(nameof(plaintextCompressor));
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            this.plaintextCompressor = plaintextCompressor;
            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc/>
        public string SerializeToString<T>(T data, Context context)
            where T : CacheValue
        {
            var stopwatch = Stopwatch.StartNew();
            var uncompressedString = JsonConvert.SerializeObject(data);
            this.loggingProvider.OnCacheSerialize(
                                        context.GetOperationName(),
                                        this.GetType().Name,
                                        stopwatch.ElapsedMilliseconds,
                                        uncompressedString.Length,
                                        context);

            var compressedString = this.plaintextCompressor.Compress(uncompressedString, context);
            return compressedString;
        }

        /// <inheritdoc/>
        public TResult DeserializeFromString<TResult>(string serializedObject, Context context)
            where TResult : CacheValue
        {
            var decompressed = this.plaintextCompressor.Decompress(serializedObject, context);

            var stopwatch = Stopwatch.StartNew();
            TResult result = JsonConvert.DeserializeObject<TResult>(decompressed);
            this.loggingProvider.OnCacheDeserialize(
                 context.GetOperationName(),
                 this.GetType().Name,
                 stopwatch.ElapsedMilliseconds,
                 context);

            return result;
        }
    }
}
