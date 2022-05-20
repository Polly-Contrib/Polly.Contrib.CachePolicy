using Polly.Contrib.CachePolicy.Builder;
using Polly.Contrib.CachePolicy.Builder.AgingStrategy;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Cache;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy
{
    /// <summary>
    /// A Polly cache policy which could be applied to asynchronous delegates.
    /// </summary>
    public partial class AsyncCachePolicy<TResult> : AsyncPolicy<TResult>
        where TResult : CacheValue
    {
        /// <summary>
        /// Fluent API to create an instance of <see cref="AsyncCachePolicyBuilder{TResult}"/>.
        /// </summary>
        /// <param name="isPolicyEnabled">An overall enablement feature flag for enabling/disabling <see cref="AsyncCachePolicy{TResult}"/>.</param>
        /// <param name="agingStrategy">Cache aging strategy which controls when cache will become stale and expired.</param>
        /// <param name="cacheProvider">Provides the contract to access cache layer</param>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations</param>
        /// <returns>A builder to create <see cref="AsyncCachePolicy{TResult}"/></returns>
        public static IFallbackConditionStep<TResult> CreateBuilder(
                                                            bool isPolicyEnabled,
                                                            IAgingStrategy<TResult> agingStrategy,
                                                            ICacheProvider cacheProvider,
                                                            ILoggingProvider loggingProvider)
        {
            agingStrategy.ThrowIfNull(nameof(agingStrategy));
            cacheProvider.ThrowIfNull(nameof(cacheProvider));
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            return new AsyncCachePolicyBuilder<TResult>(
                                                        isPolicyEnabled,
                                                        agingStrategy,
                                                        cacheProvider,
                                                        loggingProvider);
        }
    }
}