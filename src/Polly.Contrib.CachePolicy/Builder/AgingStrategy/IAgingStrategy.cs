using System;

using Polly;

namespace Polly.Contrib.CachePolicy.Builder.AgingStrategy
{
    /// <summary>
    /// Cache aging strategy which controls when cache will become stale and expired.
    /// </summary>
    public interface IAgingStrategy<TResult>
    {
        /// <summary>
        /// Grace duration relative to now after which the cached item will no longer be considered fresh and will only used for fall back to cache purpose.
        /// </summary>
        /// <param name="result">A result which is returned by the <see cref="AsyncCachePolicy{TResult}"/>.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Grace duration relative to now after which the cached item will no longer be considered fresh and will only used for fall back to cache purpose.</returns>
        TimeSpan GetGraceRelativeToNow(TResult result, Context context);

        /// <summary>
        /// Get expiration timespan relative to now for a given result.
        /// </summary>
        /// <param name="result">A result which is returned by the <see cref="AsyncCachePolicy{TResult}"/>.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Expiration duration relative to now after which the cached item will be removed.</returns>
        TimeSpan GetExpirationRelativeToNow(TResult result, Context context);
    }
}
