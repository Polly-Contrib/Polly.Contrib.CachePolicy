using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy.Providers.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILoggingProvider"/>.
    /// </summary>
    public class LoggingProvider : ILoggingProvider
    {
        /// <summary>
        /// Configuration options for <see cref="LoggingProvider"/>.
        /// </summary>
        private readonly LoggingProviderOptions loggingProviderOptions;

        /// <summary>
        /// Logger primarily used to log a metric for the purposes of perf-evaluation and monitoring.
        /// </summary>
        private IOperationalMetricLogger metricLogger;

        /// <summary>
        /// Logger meant for message logging.
        /// </summary>
        private ILogger<LoggingProvider> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingProvider"/> class.
        /// </summary>
        /// <param name="loggingProviderOptions">Configuration options for <see cref="LoggingProvider"/>.</param>
        /// <param name="metricLogger">Logger primarily used to log a metric for the purposes of perf-evaluation and monitoring.</param>
        /// <param name="logger">Logger meant for message logging.</param>
        public LoggingProvider(
                LoggingProviderOptions loggingProviderOptions,
                IOperationalMetricLogger metricLogger,
                ILogger<LoggingProvider> logger)
        {
            loggingProviderOptions.ThrowIfNull(nameof(loggingProviderOptions));
            metricLogger.ThrowIfNull(nameof(metricLogger));
            logger.ThrowIfNull(nameof(logger));

            this.loggingProviderOptions = loggingProviderOptions;
            this.metricLogger = metricLogger;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public void OnCacheGet(string key, bool isSuccess, bool isCacheHit, bool isCacheFresh, long latencyInMilliSeconds, Exception failureException, Context context)
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheGetAsyncLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameIsSuccess, isSuccess.ToString() },
                    { this.loggingProviderOptions.DimensionNameIsCacheHit, isCacheHit.ToString() },
                    { this.loggingProviderOptions.DimensionNameIsCacheFresh, isCacheFresh.ToString() },
                },
                latencyInMilliSeconds);

            if (failureException != null)
            {
                this.logger.LogError($"Exception encountered during the cache operation 'GetAsync'. OperationName: {context.GetOperationName()}. Exception: {failureException}");
            }
        }

        /// <inheritdoc/>
        public void OnCacheSet(string key, bool isSuccess, long latencyInMilliSeconds, Exception failureException, Context context)
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheSetAsyncLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameIsSuccess, isSuccess.ToString() },
                },
                latencyInMilliSeconds);

            if (failureException != null)
            {
                this.logger.LogError($"Exception encountered during the cache operation 'SetAsync'. OperationName: {context.GetOperationName()}. Exception: {failureException}");
            }
        }

        /// <inheritdoc/>
        public void OnBackendGet<TResult>(string key, bool isSuccess, bool isFallbackToCache, long latencyInMilliSeconds, DelegateResult<TResult> delegateFailureOutcome, Context context)
                where TResult : CacheValue
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameBackendGetAsyncLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameIsSuccess, isSuccess.ToString() },
                    { this.loggingProviderOptions.DimensionNameIsCacheFallback, isFallbackToCache.ToString() },
                },
                latencyInMilliSeconds);

            if (delegateFailureOutcome?.Exception != null)
            {
                this.logger.LogError($"Exception encountered during fetching backend operation. OperationName: {context.GetOperationName()}. Exception: {delegateFailureOutcome.Exception}");
            }

            if (delegateFailureOutcome?.Result != null)
            {
                this.logger.LogError($"Error results returned during fetching backend operation. OperationName: {context.GetOperationName()}. Error results: {delegateFailureOutcome.Result}.");
            }
        }

        /// <inheritdoc/>
        public void OnCacheSerialize(string key, string serializationStrategy, long latencyInMilliSeconds, long serializedSize, Context context)
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheSerializeLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameSerializationStrategy, serializationStrategy },
                },
                latencyInMilliSeconds);

            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheSerializedSize,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameSerializationStrategy, serializationStrategy },
                },
                serializedSize);
        }

        /// <inheritdoc/>
        public void OnCacheDeserialize(string key, string serializationStrategy, long latencyInMilliSeconds, Context context)
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheDeserializeLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameSerializationStrategy, serializationStrategy },
                },
                latencyInMilliSeconds);
        }

        /// <inheritdoc/>
        public void OnCacheCompress(string key, string compressionStrategy, long latencyInMilliSeconds, long compressedSize, Context context)
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheCompressLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameCompressionStrategy, compressionStrategy },
                },
                latencyInMilliSeconds);

            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheCompressedSerializedSize,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameCompressionStrategy, compressionStrategy },
                },
                compressedSize);
        }

        /// <inheritdoc/>
        public void OnCacheDecompress(string key, string compressionStrategy, long latencyInMilliSeconds, Context context)
        {
            this.metricLogger.LogMetric(
                this.loggingProviderOptions.MetricNameCacheDecompressLatency,
                new Dictionary<string, string>
                {
                    { this.loggingProviderOptions.DimensionNameOperationName, context.GetOperationName() },
                    { this.loggingProviderOptions.DimensionNameCompressionStrategy, compressionStrategy },
                },
                latencyInMilliSeconds);
        }
    }
}
