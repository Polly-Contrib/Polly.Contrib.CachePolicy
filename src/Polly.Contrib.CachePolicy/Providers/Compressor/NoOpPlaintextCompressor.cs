namespace Polly.Contrib.CachePolicy.Providers.Compressor
{
    /// <summary>
    /// An implementation of <see cref="IPlaintextCompressor"/> which does not do any compression
    /// </summary>
    public class NoOpPlaintextCompressor : IPlaintextCompressor
    {
        /// <inheritdoc/>
        public string Compress(string input)
        {
            return input;
        }

        /// <inheritdoc/>
        public string Decompress(string input)
        {
            return input;
        }
    }
}
