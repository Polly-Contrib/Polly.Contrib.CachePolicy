namespace Polly.Contrib.CachePolicy.Providers.Compressor
{
    /// <summary>
    /// Defines the contract to compress and decompress plaintext.
    /// </summary>
    public interface IPlaintextCompressor
    {
        /// <summary>
        /// Compress plaintext.
        /// </summary>
        /// <param name="input">Plaintext to be compressed.</param>
        /// <returns>Compressed plaintext.</returns>
        string Compress(string input);

        /// <summary>
        /// Decompress plaintext.
        /// </summary>
        /// <param name="input">Compressed plaintext.</param>
        /// <returns>Decompressed plaintext.</returns>
        string Decompress(string input);
    }
}
