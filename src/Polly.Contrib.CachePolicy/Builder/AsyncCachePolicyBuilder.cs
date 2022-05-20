using System;

using Polly.Contrib.CachePolicy.Builder.AgingStrategy;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Cache;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy.Builder
{
    /// <summary>
    /// A builder used for creating a configurable <see cref="AsyncCachePolicy{TResult}"/> instance.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public class AsyncCachePolicyBuilder<TResult> : IFallbackConditionStep<TResult>,
                                                         IOrFallbackConditionStep<TResult>,
                                                         IAsyncCachePolicyBuilder<TResult>
                                                         where TResult : CacheValue
    {
        /// <summary>
        /// An overall enablement feature flag for enabling/disabling <see cref="AsyncCachePolicy{TResult}"/>.
        /// </summary>
        private readonly bool isPolicyEnabled;

        /// <summary>
        /// Provides the contract to access cache layer.
        /// </summary>
        private readonly ICacheProvider cacheProvider;

        /// <summary>
        /// Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations.
        /// </summary>
        private readonly ILoggingProvider loggingProvider;

        /// <summary>
        /// Cache aging strategy which controls when cache will become stale and expired.
        /// </summary>
        private IAgingStrategy<TResult> agingStrategy;

        /// <summary>
        /// Builder class that holds the list of current execution predicates filtering TResult result values.
        /// </summary>
        private PolicyBuilder<TResult> policyBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCachePolicyBuilder{TResult}"/> class.
        /// </summary>
        /// <param name="isPolicyEnabled">An overall enablement feature flag for enabling/disabling <see cref="AsyncCachePolicy{TResult}"/>.</param>
        /// <param name="agingStrategy">Cache aging strategy which controls when cache will become stale and expired.</param>
        /// <param name="cacheProvider">Provides the contract to access cache layer</param>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations</param>
        internal AsyncCachePolicyBuilder(
            bool isPolicyEnabled,
            IAgingStrategy<TResult> agingStrategy,
            ICacheProvider cacheProvider,
            ILoggingProvider loggingProvider)
        {
            cacheProvider.ThrowIfNull(nameof(cacheProvider));
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            this.isPolicyEnabled = isPolicyEnabled;
            this.agingStrategy = agingStrategy;
            this.cacheProvider = cacheProvider;
            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> FallbackToCacheWhenThrows<TException>()
            where TException : Exception
        {
            this.policyBuilder = Policy<TResult>.Handle<TException>();
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> FallbackToCacheWhenReturns(TResult result)
        {
            this.policyBuilder = Policy.HandleResult<TResult>(result);
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> FallbackToCacheWhenThrows<TException>(Func<TException, bool> exceptionPredicate)
            where TException : Exception
        {
            this.policyBuilder = Policy<TResult>.Handle<TException>(exceptionPredicate);
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> FallbackToCacheWhenReturns(Func<TResult, bool> resultPredicate)
        {
            this.policyBuilder = Policy.HandleResult<TResult>(resultPredicate);
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> OrFallbackToCacheWhenThrows<TException>()
            where TException : Exception
        {
            this.policyBuilder.Or<TException>();
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> OrFallbackToCacheWhenReturns(TResult result)
        {
            this.policyBuilder.OrResult(result);
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> OrFallbackToCacheWhenThrows<TException>(Func<TException, bool> exceptionPredicate)
            where TException : Exception
        {
            this.policyBuilder.Or<TException>(exceptionPredicate);
            return this;
        }

        /// <inheritdoc/>
        public IOrFallbackConditionStep<TResult> OrFallbackToCacheWhenReturns(Func<TResult, bool> resultPredicate)
        {
            this.policyBuilder.OrResult(resultPredicate);
            return this;
        }

        /// <inheritdoc/>
        public AsyncPolicy<TResult> Build()
        {
            return new AsyncCachePolicy<TResult>(
                                this.isPolicyEnabled,
                                this.policyBuilder,
                                this.agingStrategy,
                                this.cacheProvider,
                                this.loggingProvider);
        }
    }
}
