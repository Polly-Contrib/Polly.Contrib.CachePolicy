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
    }
}
