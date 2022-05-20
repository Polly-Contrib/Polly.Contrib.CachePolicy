using System;

using Polly;
using Polly.Contrib.CachePolicy.Models;

namespace Polly.Contrib.CachePolicy.Providers.Logging
{
    /// <summary>
    /// Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations.
    /// </summary>
    public interface ILoggingProvider
    {
        /// <summary>
        /// Logs the metrics for any single cache get operation.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="isSuccess">Whether the cache get operation succeeds without any exception.</param>
        /// <param name="isCacheHit">Whether there is a cache hit for the key.</param>
        /// <param name="isCacheFresh">Whether the cache hit is considered fresh for the key.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a cache get operation.</param>
        /// <param name="failureException">The failure exception case happened during the cache get operation.</param>
        /// <param name="context">The execution context.</param>
        void OnCacheGet(string key, bool isSuccess, bool isCacheHit, bool isCacheFresh, long latencyInMilliSeconds, Exception failureException, Context context);

        /// <summary>
        /// Logs the metrics for any single cache set operation.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="isSuccess">Whether the cache put operation succeeds without any exception.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a cache put operation.</param>
        /// <param name="failureException">The failure exception case happened during the cache set operation.</param>
        /// <param name="context">The execution context.</param>
        void OnCacheSet(string key, bool isSuccess, long latencyInMilliSeconds, Exception failureException, Context context);

        /// <summary>
        /// Logs the metrics/traces for any single operation to get value from backend services.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="isSuccess">Whether the fetch function finished without any exception and error responses.</param>
        /// <param name="isFallbackToCache">Whether falling back to cache is being triggered.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a single operation to fetch result from backend services with the option to fall back to cache.</param>
        /// <param name="delegateFailureOutcome">The captured outcome of a single failure operation to fetch result from backend services with the option to fall back to cache.</param>
        /// <param name="context">The execution context.</param>
        void OnBackendGet<TResult>(string key, bool isSuccess, bool isFallbackToCache, long latencyInMilliSeconds, DelegateResult<TResult> delegateFailureOutcome, Context context)
            where TResult : CacheValue;

        /// <summary>
        /// Logs the metrics/traces for any single operation to serialize a cache object.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="serializationStrategy">The serializatiion strategy used to serialize the cache object, such as Json, Protobuf, etc.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a single operation to fetch result from backend services with the option to fall back to cache.</param>
        /// <param name="serializedSize">The size of the serialized cache object without any compression.</param>
        /// <param name="context">The execution context.</param>
        void OnCacheSerialize(string key, string serializationStrategy, long latencyInMilliSeconds, long serializedSize, Context context);

        /// <summary>
        /// Logs the metrics/traces for any single operation to deserialize a cache object.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="serializationStrategy">The serializatiion strategy used to serialize the cache object, such as Json, Protobuf, etc.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a single operation to fetch result from backend services with the option to fall back to cache.</param>
        /// <param name="context">The execution context.</param>
        void OnCacheDeserialize(string key, string serializationStrategy, long latencyInMilliSeconds, Context context);

        /// <summary>
        /// Logs the metrics/traces for any single operation to compress a serialized cache object.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="compressionStrategy">The compression strategy used to compress the serialized cache objects, such as LZ4, MessagePack, etc.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a single operation to fetch result from backend services with the option to fall back to cache.</param>
        /// <param name="compressedSize">The size of the serialized cache object after compression.</param>
        /// <param name="context">The execution context.</param>
        void OnCacheCompress(string key, string compressionStrategy, long latencyInMilliSeconds, long compressedSize, Context context);

        /// <summary>
        /// Logs the metrics/traces for any single operation to decompress a compressed serialized cache object.
        /// </summary>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="compressionStrategy">The compression strategy used to compress the serialized cache objects, such as LZ4, MessagePack, etc.</param>
        /// <param name="latencyInMilliSeconds">The overall latency for a single operation to fetch result from backend services with the option to fall back to cache.</param>
        /// <param name="context">The execution context.</param>
        void OnCacheDecompress(string key, string compressionStrategy, long latencyInMilliSeconds, Context context);
    }
}
