using System;
using System.Globalization;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Teams.ServicesHub.Diagnostics.Core;
using Polly.Contrib.CachePolicy.Builder;
using Polly.Contrib.CachePolicy.Builder.AgingStrategy;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Cache;
using Polly.Contrib.CachePolicy.Providers.Logging;

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
        /// Configuration key for <see cref="LoggingProvider{TResult}"/>.
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
            var loggingProvider = new LoggingProvider<TResult>(
                                                   loggingProviderOptions,
                                                   serviceProvider.GetService<IOperationalMetricLogger>(),
                                                   serviceProvider.GetService<ILogger<LoggingProvider<TResult>>>());
            var cacheProvider = new CacheProvider<TResult>(
                                                  serviceProvider.GetService<IDistributedCache>(),
                                                  loggingProvider);
            var agingStrategy = serviceProvider.GetService<IAgingStrategy<TResult>>();

            return new AsyncCachePolicyBuilder<TResult>(
                                                policyEnabled,
                                                agingStrategy,
                                                cacheProvider,
                                                loggingProvider);
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
                MetricNameCacheGetAsyncLatency = overrideOptions?.MetricNameCacheGetAsyncLatency ?? DefaultMetricNameCacheGetAsyncLatency,
                MetricNameCacheSetAsyncLatency = overrideOptions?.MetricNameCacheSetAsyncLatency ?? DefaultMetricNameCacheSetAsyncLatency,
                MetricNameBackendGetAsyncLatency = overrideOptions?.MetricNameBackendGetAsyncLatency ?? DefaultMetricNameBackendGetAsyncLatency,
            };

            return result;
        }
    }
}
