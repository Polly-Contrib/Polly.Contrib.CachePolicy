using System;

using Microsoft.Extensions.Options;
using Polly;

namespace Polly.Contrib.CachePolicy.Builder.AgingStrategy
{
    /// <summary>
    /// Aging strategy which ages cache based on a fixed duration.
    /// </summary>
    public class FixedAgingStrategy<TResult> : IAgingStrategy<TResult>
    {
        /// <summary>
        /// Configuration options for <see cref="FixedAgingStrategy{TResult}"/> based on fixed intervals.
        /// </summary>
        private FixedAgingStrategyOptions<TResult> fixedAgingStrategyOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedAgingStrategy{TResult}"/> class.
        /// </summary>
        public FixedAgingStrategy(IOptionsSnapshot<FixedAgingStrategyOptions<TResult>> fixedAgingStrategyOptions)
        {
            this.fixedAgingStrategyOptions = fixedAgingStrategyOptions.Get(typeof(TResult).Name);
        }

        /// <inheritdoc/>
        public TimeSpan GetGraceRelativeToNow(TResult result, Context context)
        {
            return this.fixedAgingStrategyOptions.GraceRelativeToNow;
        }

        /// <inheritdoc/>
        public TimeSpan GetExpirationRelativeToNow(TResult result, Context context)
        {
            return this.fixedAgingStrategyOptions.ExpirationRelativeToNow;
        }
    }
}
