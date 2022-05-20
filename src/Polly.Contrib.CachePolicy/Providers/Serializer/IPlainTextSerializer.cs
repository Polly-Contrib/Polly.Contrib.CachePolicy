using Polly.Contrib.CachePolicy.Models;

namespace Polly.Contrib.CachePolicy.Providers.Serializer
{
    /// <summary>
    /// Defines the contract for serializing and deserializing plaintext.
    /// </summary>
    public interface IPlainTextSerializer
    {
        /// <summary>
        /// Serialize <see cref="CacheValue"/> into plaintext string.
        /// </summary>
        /// <param name="data">Target to be serialized.</param>
        /// <returns>Serialized string.</returns>
        string SerializeToString(CacheValue data);

        /// <summary>
        /// Deserialize string to <see cref="CacheValue"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="data">Plaintext representation of object.</param>
        /// <returns>Deserialized object.</returns>
        TResult DeserializeFromString<TResult>(string data)
            where TResult : CacheValue;
    }
}
