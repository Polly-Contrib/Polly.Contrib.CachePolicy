using Polly;
using Polly.Contrib.CachePolicy.Models;

namespace Polly.Contrib.CachePolicy.Builder
{
    /// <summary>
    /// The final build step within builder step pattern to build <see cref="AsyncPolicy{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public interface IAsyncCachePolicyBuilder<TResult>
                        where TResult : CacheValue
    {
        /// <summary>
        /// Build a <see cref="AsyncCachePolicy{TResult}"/>.
        /// </summary>
        /// <returns>An instance of <see cref="AsyncPolicy{TResult}"/>.</returns>
        AsyncPolicy<TResult> Build();
    }
}
