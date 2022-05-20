using K4os.Compression.LZ4;

namespace Polly.Contrib.CachePolicy.Providers.Compressor
{
    /// <summary>
    /// An implementation of binary compressor based on LZ4 Pickler compression.
    /// </summary>
    public class LZ4PicklerBinaryCompressor : IBinaryCompressor
    {
        /// <inheritdoc/>
        public byte[] Compress(byte[] input)
        {
            return LZ4Pickler.Pickle(input);
        }

        /// <inheritdoc/>
        public byte[] Decompress(byte[] input)
        {
            return LZ4Pickler.Unpickle(input);
        }
    }
}
