using System;

namespace Polly.Contrib.CachePolicy.Builder.AgingStrategy
{
    /// <summary>
    /// Configuration options for <see cref="FixedAgingStrategy{TResult}"/>.
    /// </summary>
    public class FixedAgingStrategyOptions<TResult>
    {
        /// <summary>
        /// Expiration duration relative to now after which the cached item will be removed.
        /// </summary>
        public TimeSpan ExpirationRelativeToNow { get; set; }

        /// <summary>
        /// Grace duration relative to now after which the cached item will no longer be considered fresh and will only used for fall back to cache purpose.
        /// </summary>
        public TimeSpan GraceRelativeToNow { get; set; }
    }
}
