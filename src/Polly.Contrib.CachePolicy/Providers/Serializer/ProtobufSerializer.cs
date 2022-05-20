using System;
using System.IO;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Serializer.Model;
using Polly.Contrib.CachePolicy.Utilities;
using ProtoBuf.Meta;

namespace Polly.Contrib.CachePolicy.Providers.Serializer
{
    /// <summary>
    ///  An implementation of <see cref="IBinarySerializer"/> which converts a <see cref="CacheValue"/> to protobuf based serialization.
    /// </summary>
    public class ProtobufSerializer : IBinarySerializer
    {
        /// <summary>
        /// A binary compressor which compresses a byte array.
        /// </summary>
        private IBinaryCompressor binaryCompressor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufSerializer"/> class.
        /// </summary>
        /// <param name="binaryCompressor">A binary compressor which compresses a byte array.</param>
        public ProtobufSerializer(
            IBinaryCompressor binaryCompressor)
        {
            binaryCompressor.ThrowIfNull(nameof(binaryCompressor));

            if (!RuntimeTypeModel.Default.CanSerialize(typeof(DateTimeOffset)))
            {
                RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
            }

            this.binaryCompressor = binaryCompressor;
        }

        /// <inheritdoc/>
        public byte[] SerializeToBytes(CacheValue data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, data);
                var uncompressedByteArray = stream.ToArray();
                return this.binaryCompressor.Compress(uncompressedByteArray);
            }
        }

        /// <inheritdoc/>
        public T DeserializeFromBytes<T>(byte[] data)
            where T : CacheValue
        {
            T result = null;
            var decompressedData = this.binaryCompressor.Decompress(data);
            using (MemoryStream stream = new MemoryStream(decompressedData))
            {
                result = ProtoBuf.Serializer.Deserialize<T>(stream);
            }

            return result;
        }
    }
}
