using Polly;

namespace Polly.Contrib.CachePolicy.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="Context"/>.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// A key representing the caching scenario so that logging and metrics can distinguish usages of policy instances at different call sites.
        /// </summary>
        private static readonly string OperationName = "OperationName";

        /// <summary>
        /// A cache key.
        /// </summary>
        private static readonly string CacheKey = "CacheKey";

        /// <summary>
        /// Set the cache key within the execution context.
        /// </summary>
        /// <param name="context">The Polly execution context.</param>
        /// <param name="cacheKey">Cache key.</param>
        /// <returns>The Polly execution context.</returns>
        public static Context WithCacheKey(this Context context, string cacheKey)
        {
            cacheKey.ThrowIfNullOrWhiteSpace(nameof(cacheKey));

            context[CacheKey] = cacheKey;

            return context;
        }

        /// <summary>
        /// Set the cache scenario within the execution context.
        /// </summary>
        /// <param name="context">The Polly execution context.</param>
        /// <param name="operationName"></param>
        /// <returns>The Polly execution context.</returns>
        public static Context WithOperationName(this Context context, string operationName)
        {
            operationName.ThrowIfNullOrWhiteSpace(nameof(operationName));

            context[OperationName] = operationName;

            return context;
        }

        /// <summary>
        /// Gets the cache key from the given execution context.
        /// </summary>
        /// <param name="context">The Polly execution context.</param>
        /// <returns>The cache key.</returns>
        internal static string GetCacheKey(this Context context)
        {
            if (context.TryGetValue(CacheKey, out object cacheKey))
            {
                return cacheKey as string;
            }

            return null;
        }

        /// <summary>
        /// Gets the scenario from the given execution context.
        /// </summary>
        /// <param name="context">The Polly execution context.</param>
        /// <returns>Operation name.</returns>
        internal static string GetOperationName(this Context context)
        {
            if (context.TryGetValue(OperationName, out object operationName))
            {
                return operationName as string;
            }

            return null;
        }
    }
}
