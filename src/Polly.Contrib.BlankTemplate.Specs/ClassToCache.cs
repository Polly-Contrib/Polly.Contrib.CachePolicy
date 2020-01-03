using Polly.Contrib.CachePolicy.Models;

namespace Polly.Contrib.CachePolicy.Specs
{
    public class ClassToCache : CacheValue
    {
        public string Property { get; set; }

        public bool Freshness { get; set; }

        public override bool IsFresh()
        {
            return Freshness;
        }
    }
}