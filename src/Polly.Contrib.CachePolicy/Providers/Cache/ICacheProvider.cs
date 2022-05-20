using System;
using System.Threading.Tasks;

using Polly;
using Polly.Contrib.CachePolicy.Models;

namespace Polly.Contrib.CachePolicy.Providers.Cache
{
    /// <summary>
    /// Provides the contract to access cache layer.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Asynchronously get a <see cref="CacheValue"/> from the underlying cache with key.
        /// </summary>
        /// <typeparam name="TResult">Type into which the data retrieved from the cache is to be deserialized into, if found</typeparam>
        /// <param name="key">The key against which the data is stored in the cache</param>
        /// <param name="context">The execution context.</param>
        /// <returns>The deserialized value of the item retrieved from the cache, if found. Else, returns null</returns>
        Task<TResult> GetAsync<TResult>(string key, Context context)
            where TResult : CacheValue;

        /// <summary>
        /// Asynchronously set a <see cref="CacheValue"/> to the underlying cache with key.
        /// </summary>
        /// <typeparam name="TResult">Type into which the data retrieved from the cache is to be serialized into, if found</typeparam>
        /// <param name="key">The key against which the data is stored in the cache.</param>
        /// <param name="value">The data to store in the cache.</param>
        /// <param name="expirationRelativeToNow">Expiration duration relative to now after which the cached item will be removed</param>
        /// <param name="graceRelativeToNow">Grace duration relative to now after which the cached item will no longer be considered fresh and will only used for fall back to cache purpose (TTR).</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Task handle</returns>
        Task SetAsync<TResult>(string key, TResult value, TimeSpan expirationRelativeToNow, TimeSpan graceRelativeToNow, Context context)
            where TResult : CacheValue;
    }
}
