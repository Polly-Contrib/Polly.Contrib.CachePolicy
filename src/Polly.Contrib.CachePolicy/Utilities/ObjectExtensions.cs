using System;

namespace Polly.Contrib.CachePolicy.Utilities
{
    /// <summary>
    /// Extension methods for type <see cref="object"/>.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given object is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <param name="value">Value of the object</param>
        /// <param name="objectName">Name of the object</param>
        public static void ThrowIfNull(this object value, string objectName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(objectName);
            }
        }
    }
}
