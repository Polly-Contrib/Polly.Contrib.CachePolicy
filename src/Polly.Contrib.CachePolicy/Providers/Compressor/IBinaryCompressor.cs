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
        /// <param name="context">The execution context.</param>
        /// <returns>Compressed byte array.</returns>
        byte[] Compress(byte[] input, Context context);

        /// <summary>
        /// Decompress byte array.
        /// </summary>
        /// <param name="input">Byte array to be decompressed</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Decompressed byte array.</returns>
        byte[] Decompress(byte[] input, Context context);
    }
}
