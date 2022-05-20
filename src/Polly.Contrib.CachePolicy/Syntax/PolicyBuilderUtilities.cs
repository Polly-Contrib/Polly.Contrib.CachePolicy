using System;
using System.Globalization;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.Contrib.CachePolicy.Builder;
using Polly.Contrib.CachePolicy.Builder.AgingStrategy;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Cache;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Providers.Serializer;

namespace Polly.Contrib.CachePolicy.Syntax
{
    /// <summary>
    /// Extension method for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class PolicyBuilderUtilities
    {
        /// <summary>
        /// Configuration key format for reading async cache policy enablement flag through an <see cref="IConfiguration"/> instance
        /// </summary>
        private const string ConfigKeyFormatPolicyEnabled = "AsyncCachePolicy:{0}:Enabled";

        /// <summary>
        /// Configuration key for <see cref="LoggingProvider"/>.
        /// </summary>
        private const string ConfigKeyLoggingProvider = "AsyncCachePolicy:LoggingProvider";

        /// <summary>
        /// Default metric name for cache get operation latencies.
        /// </summary>
        private const string DefaultMetricNameCacheGetAsyncLatency = @"Metric\DistributedCache\GetAsync\Latency";

        /// <summary>
        /// Default metric name for cache set operation latency.
        /// </summary>
        private const string DefaultMetricNameCacheSetAsyncLatency = @"Metric\DistributedCache\SetAsync\Latency";

        /// <summary>
        /// Default metric name for backend get operation latency.
        /// </summary>
        private const string DefaultMetricNameBackendGetAsyncLatency = @"Metric\Backend\GetAsync\Latency";

        /// <summary>
        /// Default metric name for cache serialize operation latency.
        /// </summary>
        private const string DefaultMetricNameCacheSerializeLatency = @"Metric\DistributedCache\Serialize\Latency";

        /// <summary>
        /// Default metric name for cache deserialize operation latency.
        /// </summary>
        private const string DefaultMetricNameCacheDeserializeLatency = @"Metric\DistributedCache\Deserialize\Latency";

        /// <summary>
        /// Default metric name for cache compress operation latency.
        /// </summary>
        private const string DefaultMetricNameCacheCompressLatency = @"Metric\DistributedCache\Compress\Latency";

        /// <summary>
        /// Default metric name for cache decompress operation latency.
        /// </summary>
        private const string DefaultMetricNameCacheDecompressLatency = @"Metric\DistributedCache\Decompress\Latency";

        /// <summary>
        /// Default metric name for serialized size of a cache object.
        /// </summary>
        private const string DefaultMetricNameCacheSerializedSize = @"Metric\DistributedCache\SerializedSize";

        /// <summary>
        /// Default metric name for compressed serialized size of a cache object.
        /// </summary>
        private const string DefaultMetricNameCacheCompressedSerializedSize = @"Metric\DistributedCache\CompressedSerializedSize";

        /// <summary>
        /// Default dimension name for the cache operation scenario name.
        /// </summary>
        private const string DefaultDimensionNameOperationName = "OperationName";

        /// <summary>
        /// Default dimension name for operation states of CacheGetAsync, CacheSetAsync, BackendGetAsync events.
        /// </summary>
        private const string DefaultDimensionNameIsSuccess = "IsSuccess";

        /// <summary>
        /// Default dimension name for cache get operation results (hit/miss).
        /// </summary>
        private const string DefaultDimensionNameIsCacheHit = "IsCacheHit";

        /// <summary>
        /// Default dimension name for cache get operation results (hit/miss).
        /// </summary>
        private const string DefaultDimensionNameIsCacheFresh = "IsCacheFresh";

        /// <summary>
        /// Default dimension name for falling back to cache operations.
        /// </summary>
        private const string DefaultDimensionNameIsCacheFallback = "IsCacheFallback";

        /// <summary>
        /// Default dimension name for strategy used to serialize a cache object.
        /// </summary>
        private const string DefaultDimensionNameSerializationStrategy = "SerializationStrategy";

        /// <summary>
        /// Default dimension name for strategy used to compress serialized cache objects.
        /// </summary>
        private const string DefaultDimensionNameCompressionStrategy = "CompressionStrategy";

        /// <summary>
        /// Create a <see cref="AsyncCachePolicyBuilder{TResult}"/> with default configs.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="configuration">Represents a set of key/value application configuration properties</param>]
        /// <param name="serviceProvider">Defines a mechanism for retrieving a service object.</param>
        /// <param name="configKeyPolicyEnabled">Configuration key for whether the policy is enabled. If absent, it will use "AsyncCachePolicy:{0}:Enabled".</param>
        /// <param name="configKeyLoggingProviderOptions">Configuration key for <see cref="LoggingProviderOptions"/>. If absent, it will use "AsyncCachePolicy:LoggingProvider".</param>
        /// <returns>A collection of service descriptors</returns>
        public static IFallbackConditionStep<TResult> CreateAsyncCachePolicyBuilder<TResult>(
                                                IConfiguration configuration,
                                                IServiceProvider serviceProvider,
                                                string configKeyPolicyEnabled = null,
                                                string configKeyLoggingProviderOptions = null)
                                                where TResult : CacheValue
        {
            // Configuration key for policy
            configKeyPolicyEnabled = configKeyPolicyEnabled ?? string.Format(CultureInfo.InvariantCulture, ConfigKeyFormatPolicyEnabled, typeof(TResult).Name);
            configKeyLoggingProviderOptions = configKeyLoggingProviderOptions ?? ConfigKeyLoggingProvider;

            // bind configurations
            var policyEnabled = configuration.GetSection(configKeyPolicyEnabled)
                                             .Get<bool>();
            var loggingProviderOptions = CreateLoggingProivderOptions(configuration, configKeyLoggingProviderOptions);

            // build policy dependencies
            var loggingProvider = new LoggingProvider(
                                                   loggingProviderOptions,
                                                   serviceProvider.GetService<IOperationalMetricLogger>(),
                                                   serviceProvider.GetService<ILogger<LoggingProvider>>());
            var cacheProvider = new PlainTextCacheProvider(
                                                  serviceProvider.GetService<IDistributedCache>(),
                                                  serviceProvider.GetService<IPlainTextSerializer>(),
                                                  loggingProvider);
            var agingStrategy = serviceProvider.GetService<IAgingStrategy<TResult>>();

            return new AsyncCachePolicyBuilder<TResult>(
                                                policyEnabled,
                                                agingStrategy,
                                                cacheProvider,
                                                loggingProvider);
        }


        /// <summary>
        /// Create a <see cref="AsyncCachePolicyBuilder{TResult}"/> with overloaded <see cref="ICacheProvider"/> and <see cref="ILoggingProvider"/> and other default configs.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="configuration">Represents a set of key/value application configuration properties</param>]
        /// <param name="serviceProvider">Defines a mechanism for retrieving a service object.</param>
        /// <param name="cacheProvider">An instance of <see cref="ICacheProvider"/> to access cache layer</param>
        /// <param name="loggingProvider">An instance of <see cref="ILoggingProvider"/>.</param>
        /// <param name="configKeyPolicyEnabled">Configuration key for whether the policy is enabled. If absent, it will use "AsyncCachePolicy:{0}:Enabled".</param>
        /// <returns>A collection of service descriptors</returns>
        public static IFallbackConditionStep<TResult> CreateAsyncCachePolicyBuilder<TResult>(
                                                IConfiguration configuration,
                                                IServiceProvider serviceProvider,
                                                ICacheProvider cacheProvider,
                                                ILoggingProvider loggingProvider,
                                                string configKeyPolicyEnabled = null)
                                                where TResult : CacheValue
        {
            // Configuration key for policy
            configKeyPolicyEnabled = configKeyPolicyEnabled ?? string.Format(CultureInfo.InvariantCulture, ConfigKeyFormatPolicyEnabled, typeof(TResult).Name);
            var policyEnabled = configuration.GetSection(configKeyPolicyEnabled)
                                             .Get<bool>();
            var agingStrategy = serviceProvider.GetService<IAgingStrategy<TResult>>();

            return new AsyncCachePolicyBuilder<TResult>(
                                                policyEnabled,
                                                agingStrategy,
                                                cacheProvider,
                                                loggingProvider);
        }

        /// <summary>
        /// Create an instance <see cref="PlainTextCacheProvider"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="serviceProvider">Defines a mechanism for retrieving a service object.</param>
        /// <param name="loggingProvider">Logging provider for logging <see cref="AsyncCachePolicy{TResult}"/> operations.</param>
        /// <returns>An instance of <see cref="ICacheProvider"/> to access cache layer.</returns>
        public static ICacheProvider CreateDefaultPlainTextCacheProvider<TResult>(
                                     IServiceProvider serviceProvider,
                                     ILoggingProvider loggingProvider)
                                                            where TResult : CacheValue
        {
            var cacheProvider = new PlainTextCacheProvider(
                                                  serviceProvider.GetService<IDistributedCache>(),
                                                  serviceProvider.GetService<IPlainTextSerializer>(),
                                                  loggingProvider);
            return cacheProvider;
        }

        /// <summary>
        /// Create an instance <see cref="BinaryCacheProvider"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="serviceProvider">Defines a mechanism for retrieving a service object.</param>
        /// <param name="loggingProvider">Logging provider for logging <see cref="AsyncCachePolicy{TResult}"/> operations.</param>
        /// <returns>An instance of <see cref="ICacheProvider"/> to access cache layer.</returns>
        public static ICacheProvider CreateDefaultBinaryCacheProvider<TResult>(
                                        IServiceProvider serviceProvider,
                                        ILoggingProvider loggingProvider)
                                                    where TResult : CacheValue
        {
            var cacheProvider = new BinaryCacheProvider(
                                                  serviceProvider.GetService<IDistributedCache>(),
                                                  serviceProvider.GetService<IBinarySerializer>(),
                                                  loggingProvider);
            return cacheProvider;
        }

        /// <summary>
        /// Create an instance of <see cref="ILoggerProvider"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="configuration">Represents a set of key/value application configuration properties</param>]
        /// <param name="serviceProvider">Defines a mechanism for retrieving a service object.</param>
        /// <param name="configKeyLoggingProviderOptions">Configuration key for <see cref="LoggingProviderOptions"/>. If absent, it will use "AsyncCachePolicy:LoggingProvider".</param>
        /// <returns>Logging provider for logging <see cref="AsyncCachePolicy{TResult}"/> operations.</returns>
        public static ILoggingProvider CreateLoggingProvider<TResult>(
                                                    IConfiguration configuration,
                                                    IServiceProvider serviceProvider,
                                                    string configKeyLoggingProviderOptions = null)
                                                    where TResult : CacheValue
        {
            configKeyLoggingProviderOptions = configKeyLoggingProviderOptions ?? ConfigKeyLoggingProvider;

            var loggingProviderOptions = CreateLoggingProivderOptions(configuration, configKeyLoggingProviderOptions);

            var loggingProvider = new LoggingProvider(
                                                   loggingProviderOptions,
                                                   serviceProvider.GetService<IOperationalMetricLogger>(),
                                                   serviceProvider.GetService<ILogger<LoggingProvider>>());
            return loggingProvider;
        }

        /// <summary>
        /// Create a default <see cref="LoggingProviderOptions"/> instance.
        /// </summary>
        /// <returns>A default <see cref="LoggingProviderOptions"/> instance.</returns>
        private static LoggingProviderOptions CreateLoggingProivderOptions(IConfiguration configuration, string configKeyLoggingProviderOptions)
        {
            var overrideOptions = configuration.GetSection(configKeyLoggingProviderOptions)
                                               .Get<LoggingProviderOptions>();

            var result = new LoggingProviderOptions()
            {
                DimensionNameIsSuccess = overrideOptions?.DimensionNameIsSuccess ?? DefaultDimensionNameIsSuccess,
                DimensionNameIsCacheHit = overrideOptions?.DimensionNameIsCacheHit ?? DefaultDimensionNameIsCacheHit,
                DimensionNameIsCacheFresh = overrideOptions?.DimensionNameIsCacheFresh ?? DefaultDimensionNameIsCacheFresh,
                DimensionNameIsCacheFallback = overrideOptions?.DimensionNameIsCacheFallback ?? DefaultDimensionNameIsCacheFallback,
                DimensionNameOperationName = overrideOptions?.DimensionNameOperationName ?? DefaultDimensionNameOperationName,
                DimensionNameSerializationStrategy = overrideOptions?.DimensionNameSerializationStrategy ?? DefaultDimensionNameSerializationStrategy,
                DimensionNameCompressionStrategy = overrideOptions?.DimensionNameCompressionStrategy ?? DefaultDimensionNameCompressionStrategy,

                MetricNameCacheGetAsyncLatency = overrideOptions?.MetricNameCacheGetAsyncLatency ?? DefaultMetricNameCacheGetAsyncLatency,
                MetricNameCacheSetAsyncLatency = overrideOptions?.MetricNameCacheSetAsyncLatency ?? DefaultMetricNameCacheSetAsyncLatency,
                MetricNameBackendGetAsyncLatency = overrideOptions?.MetricNameBackendGetAsyncLatency ?? DefaultMetricNameBackendGetAsyncLatency,
                MetricNameCacheSerializeLatency = overrideOptions?.MetricNameCacheSerializeLatency ?? DefaultMetricNameCacheSerializeLatency,
                MetricNameCacheDeserializeLatency = overrideOptions?.MetricNameCacheDeserializeLatency ?? DefaultMetricNameCacheDeserializeLatency,
                MetricNameCacheCompressLatency = overrideOptions?.MetricNameCacheCompressLatency ?? DefaultMetricNameCacheCompressLatency,
                MetricNameCacheDecompressLatency = overrideOptions?.MetricNameCacheDecompressLatency ?? DefaultMetricNameCacheDecompressLatency,
                MetricNameCacheSerializedSize = overrideOptions?.MetricNameCacheSerializedSize ?? DefaultMetricNameCacheSerializedSize,
                MetricNameCacheCompressedSerializedSize = overrideOptions?.MetricNameCacheCompressedSerializedSize ?? DefaultMetricNameCacheCompressedSerializedSize,
            };

            return result;
        }
    }
}
