namespace Polly.Contrib.CachePolicy.Providers.Logging
{
    /// <summary>
    /// Configuration option for <see cref="LoggingProvider"/>.
    /// </summary>
    public class LoggingProviderOptions
    {
        /// <summary>
        /// Metric name for cache get operation latencies.
        /// </summary>
        public string MetricNameCacheGetAsyncLatency { get; set; }

        /// <summary>
        /// Metric name for cache set operation latency.
        /// </summary>
        public string MetricNameCacheSetAsyncLatency { get; set; }

        /// <summary>
        /// Metric name for backend get operation latency.
        /// </summary>
        public string MetricNameBackendGetAsyncLatency { get; set; }

        /// <summary>
        /// Metric name for cache serialize operation latency.
        /// </summary>
        public string MetricNameCacheSerializeLatency { get; set; }

        /// <summary>
        /// Metric name for cache deserialize operation latency.
        /// </summary>
        public string MetricNameCacheDeserializeLatency { get; set; }

        /// <summary>
        /// Metric name for cache compress operation latency.
        /// </summary>
        public string MetricNameCacheCompressLatency { get; set; }

        /// <summary>
        /// Metric name for cache decompress operation latency.
        /// </summary>
        public string MetricNameCacheDecompressLatency { get; set; }

        /// <summary>
        /// Metric name for serialized size of a cache object.
        /// </summary>
        public string MetricNameCacheSerializedSize { get; set; }

        /// <summary>
        /// Metric name for compressed serialized size of a cache object.
        /// </summary>
        public string MetricNameCacheCompressedSerializedSize { get; set; }

        /// <summary>
        /// Dimension name for the cache scenario.
        /// </summary>
        public string DimensionNameOperationName { get; set; }

        /// <summary>
        /// Dimension name for operation states of CacheGetAsync, CacheSetAsync, BackendGetAsync events.
        /// </summary>
        public string DimensionNameIsSuccess { get; set; }

        /// <summary>
        /// Dimension name for cache get operation results (hit/miss).
        /// </summary>
        public string DimensionNameIsCacheHit { get; set; }

        /// <summary>
        /// Dimension name for freshness of cache get operation results (fresh/stale).
        /// </summary>
        public string DimensionNameIsCacheFresh { get; set; }

        /// <summary>
        /// Dimension name for falling back to cache operations.
        /// </summary>
        public string DimensionNameIsCacheFallback { get; set; }

        /// <summary>
        /// Dimension name for strategy used to serialize a cache object.
        /// </summary>
        public string DimensionNameSerializationStrategy { get; set; }

        /// <summary>
        /// Dimension name for strategy used to compress serialized cache objects.
        /// </summary>
        public string DimensionNameCompressionStrategy { get; set; }
    }
}
