using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public partial class AsyncCachePolicy<TResult> : AsyncPolicy<TResult>
        where TResult : CacheValue
    {
        /// <summary>
        /// An overall enablement feature flag for enabling/disabling <see cref="AsyncCachePolicy{TResult}"/>.
        /// </summary>
        private readonly bool isPolicyEnabled;

        /// <summary>
        /// Cache aging strategy which controls when cache will become stale and expired.
        /// </summary>
        private IAgingStrategy<TResult> agingStrategy;

        /// <summary>
        /// Provides the contract to access cache layer.
        /// </summary>
        private ICacheProvider cacheProvider;

        /// <summary>
        /// Defines the contract to logging <see cref="AsyncCachePolicy{TResult}"/> metrics and traces
        /// </summary>
        private ILoggingProvider loggingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCachePolicy{TResult}"/> class.
        /// </summary>
        /// <param name="isPolicyEnabled">An overall enablement feature flag for enabling/disabling <see cref="AsyncCachePolicy{TResult}"/>.</param>
        /// <param name="policyBuilder">Builder class that holds the list of current execution predicates filtering TResult result values</param>
        /// <param name="agingStrategy">Cache aging strategy which controls when cache will become stale and expired.</param>
        /// <param name="cacheProvider">Provides the contract to access cache layer</param>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations</param>
        internal AsyncCachePolicy(
            bool isPolicyEnabled,
            PolicyBuilder<TResult> policyBuilder,
            IAgingStrategy<TResult> agingStrategy,
            ICacheProvider cacheProvider,
            ILoggingProvider loggingProvider)
            : base(policyBuilder)
        {
            agingStrategy.ThrowIfNull(nameof(agingStrategy));
            cacheProvider.ThrowIfNull(nameof(cacheProvider));
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            this.isPolicyEnabled = isPolicyEnabled;
            this.agingStrategy = agingStrategy;
            this.cacheProvider = cacheProvider;
            this.loggingProvider = loggingProvider;
        }

        /// <summary>
        /// Execution logic for the async cache policy. 
        /// </summary>
        /// <param name="backendGet">Operation to get an item from backend services.</param>
        /// <param name="context">Polly context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="continueOnCapturedContext">Whether to continue on captured context.</param>
        /// <returns>Task of type which the data is retrieved.</returns>
        protected override async Task<TResult> ImplementationAsync(
                        Func<Context, CancellationToken, Task<TResult>> backendGet,
                        Context context,
                        CancellationToken cancellationToken,
                        bool continueOnCapturedContext)
        {
            if (!this.isPolicyEnabled)
            {
                return await backendGet(context, cancellationToken);
            }

            // Get from cache
            var cacheKey = context.GetCacheKey();
            TResult valueFromCache = await this.cacheProvider.GetAsync<TResult>(cacheKey, context);
            if (valueFromCache != null && valueFromCache.IsFresh())
            {
                return valueFromCache;
            }

            // Get from backend
            var stopwatch = Stopwatch.StartNew();
            var isSuccess = true;
            var isFallbackToCache = false;
            TResult result = null;
            DelegateResult<TResult> delegateFailureOutcome = null;
            try
            {
                result = await backendGet(context, cancellationToken);
                if (!this.ResultPredicates.AnyMatch(result))
                {
                    // Set cache
                    if (result != null)
                    {
                        var graceRelativeToNow = this.agingStrategy.GetGraceRelativeToNow(result, context);
                        result.SetGraceTimeStamp(graceRelativeToNow);

                        var expirationRelativeToNow = this.agingStrategy.GetExpirationRelativeToNow(result, context);
                        if (!expirationRelativeToNow.Equals(default(TimeSpan)))
                        {
#pragma warning disable 4014
                            Task.Run(async () =>
#pragma warning restore 4014
                            {
                                await this.cacheProvider.SetAsync<TResult>(cacheKey, result, expirationRelativeToNow, context);
                            });
                        }
                    }
                }
                else
                {
                    delegateFailureOutcome = new DelegateResult<TResult>(result);

                    if (valueFromCache != null)
                    {
                        // Result error should be handled and cache is available for falling back to
                        result = valueFromCache;
                        isFallbackToCache = true;
                    }
                }
            }
            catch (Exception exception)
            {
                delegateFailureOutcome = new DelegateResult<TResult>(exception);

                Exception exceptionToFallback = this.ExceptionPredicates.FirstMatchOrDefault(exception);
                if (exceptionToFallback == null || valueFromCache == null)
                {
                    isSuccess = false;
                    throw;
                }
                else
                {
                    // Exception should be handled and cache is available for falling back to
                    result = valueFromCache;
                    isFallbackToCache = true;
                }
            }
            finally
            {
                this.loggingProvider.OnBackendGet(
                                        cacheKey,
                                        isSuccess,
                                        isFallbackToCache,
                                        stopwatch.ElapsedMilliseconds,
                                        delegateFailureOutcome,
                                        context);
            }

            return result;
        }
    }
}