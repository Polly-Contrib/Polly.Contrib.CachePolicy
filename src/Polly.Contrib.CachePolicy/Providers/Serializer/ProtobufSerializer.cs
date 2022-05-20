using System;
using System.Diagnostics;
using System.IO;
using Polly.Contrib.CachePolicy.Models;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Logging;
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
        /// Defines the contract to logging <see cref="AsyncCachePolicy{TResult}"/> metrics and traces.
        /// </summary>
        private ILoggingProvider loggingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufSerializer"/> class.
        /// </summary>
        /// <param name="binaryCompressor">A binary compressor which compresses a byte array.</param>
        /// <param name="loggingProvider">Provides the contract to logging <see cref="AsyncCachePolicy{TResult}"/> operations.</param>
        public ProtobufSerializer(
            IBinaryCompressor binaryCompressor,
            ILoggingProvider loggingProvider)
        {
            binaryCompressor.ThrowIfNull(nameof(binaryCompressor));
            loggingProvider.ThrowIfNull(nameof(loggingProvider));

            if (!RuntimeTypeModel.Default.CanSerialize(typeof(DateTimeOffset)))
            {
                RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
            }

            this.binaryCompressor = binaryCompressor;
            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc/>
        public byte[] SerializeToBytes<T>(T data, Context context)
            where T : CacheValue
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var stopwatch = Stopwatch.StartNew();
                ProtoBuf.Serializer.Serialize(stream, data);
                var uncompressedByteArray = stream.ToArray();
                this.loggingProvider.OnCacheSerialize(
                                            context.GetOperationName(),
                                            this.GetType().Name,
                                            stopwatch.ElapsedMilliseconds,
                                            uncompressedByteArray.Length, // TODO: Check
                                            context);

                var compressedByteArray = this.binaryCompressor.Compress(uncompressedByteArray, context);

                return compressedByteArray;
            }
        }

        /// <inheritdoc/>
        public TResult DeserializeFromBytes<TResult>(byte[] serializedObject, Context context)
            where TResult : CacheValue
        {
            var decompressedData = this.binaryCompressor.Decompress(serializedObject, context);

            var stopwatch = Stopwatch.StartNew();
            TResult result = null;
            using (MemoryStream stream = new MemoryStream(decompressedData))
            {
                result = ProtoBuf.Serializer.Deserialize<TResult>(stream);
                this.loggingProvider.OnCacheDeserialize(
                                context.GetOperationName(),
                                this.GetType().Name,
                                stopwatch.ElapsedMilliseconds,
                                context);
            }

            return result;
        }
    }
}
