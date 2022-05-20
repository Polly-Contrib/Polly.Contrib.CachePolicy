namespace Polly.Contrib.CachePolicy.Providers.Compressor
{
    /// <summary>
    /// Defines the contract to compress and decompress byte array.
    /// </summary>
    public interface IBinaryCompressor
    {
        /// <summary>
        /// Compress byte array.
        /// </summary>
        /// <param name="input">Byte array to compress.</param>
        /// <returns>Compressed byte array.</returns>
        byte[] Compress(byte[] input);

        /// <summary>
        /// Decompress byte array.
        /// </summary>
        /// <param name="input">Byte array to be decompressed</param>
        /// <returns>Decompressed byte array.</returns>
        byte[] Decompress(byte[] input);
    }
}
