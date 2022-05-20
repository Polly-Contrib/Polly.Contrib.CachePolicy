using Polly.Contrib.CachePolicy.Models;

namespace Polly.Contrib.CachePolicy.Providers.Serializer
{
    /// <summary>
    /// Defines the contrat for serialize and deserialize binary.
    /// </summary>
    public interface IBinarySerializer
    {
        /// <summary>
        /// Serialize <see cref="CacheValue"/> into plaintext string.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="data">Target to be serialized.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Serialized byte array.</returns>
        byte[] SerializeToBytes<TResult>(TResult data, Context context)
            where TResult : CacheValue;

        /// <summary>
        /// Deserialize binary representation to <see cref="CacheValue"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
        /// <param name="data">Binary representation of object.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Deserialized object.</returns>
        TResult DeserializeFromBytes<TResult>(byte[] data, Context context)
            where TResult : CacheValue;
    }
}
