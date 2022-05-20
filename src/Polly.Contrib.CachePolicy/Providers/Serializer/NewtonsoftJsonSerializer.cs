using Newtonsoft.Json;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Utilities;

namespace Polly.Contrib.CachePolicy.Providers.Serializer
{
    /// <summary>
    /// An implementation of <see cref="IPlainTextSerializer"/> based on Newtonsoft Json.
    /// </summary>
    public class NewtonsoftJsonSerializer : IPlainTextSerializer
    {
        /// <summary>
        /// A plaintext compressor to compress and decompress plaintext.
        /// </summary>
        private IPlaintextCompressor plaintextCompressor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
        /// </summary>
        /// <param name="plaintextCompressor">A plaintext compressor to compress and decompress plaintext.</param>
        public NewtonsoftJsonSerializer(IPlaintextCompressor plaintextCompressor)
        {
            plaintextCompressor.ThrowIfNull(nameof(plaintextCompressor));

            this.plaintextCompressor = plaintextCompressor;
        }

        /// <inheritdoc/>
        public string SerializeToString(CacheValue data)
        {
            var uncompressed = JsonConvert.SerializeObject(data);
            return this.plaintextCompressor.Compress(uncompressed);
        }

        /// <inheritdoc/>
        public TResult DeserializeFromString<TResult>(string data)
            where TResult : CacheValue
        {
            var uncompressed = this.plaintextCompressor.Decompress(data);
            return JsonConvert.DeserializeObject<TResult>(uncompressed);
        }
    }
}
