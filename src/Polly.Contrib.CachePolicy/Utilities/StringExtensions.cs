using System;

namespace Polly.Contrib.CachePolicy.Utilities
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Throws an exception if the given string is null, empty or consists of only whitespaces
        /// </summary>
        /// <exception cref="ArgumentException">The string is null, empty or consists of only whitespaces</exception>
        /// <param name="value">Value of the string</param>
        /// <param name="stringName">Name of the string</param>
        public static void ThrowIfNullOrWhiteSpace(this string value, string stringName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{stringName} cannot be null, empty, or only whitespace.");
            }
        }
    }
}
