using System;

using Newtonsoft.Json;

namespace Polly.Contrib.CachePolicy.Models
{
    /// <summary>
    /// Defines the data model required for async cache policy.
    /// </summary>
    public abstract class CacheValue
    {
        /// <summary>
        /// A point in time after which the cache will no longer be considered fresh and will only used for fall back to cache purpose
        /// </summary>
        [JsonPropertyAttribute(NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTimeOffset? GraceTimeStamp { get; set; }

        /// <summary>
        /// Whether the value cached is null.
        /// </summary>
        public virtual bool IsNull { get; set; }

        /// <summary>
        /// Whether the cache item is still fresh.
        /// </summary>
        /// <returns>True, if fresh; Otherwise, false.</returns>
        /// <remarks>For grace cache migration, if grace timestamp does not exist in the cache item, it is still fresh.</remarks>
        public virtual bool IsFresh()
        {
            return !this.GraceTimeStamp.HasValue
                || this.GraceTimeStamp.Value.CompareTo(DateTimeOffset.Now) > 0;
        }

        /// <summary>
        /// Set the grace time stamp for the cache item.
        /// </summary>
        /// <param name="graceTimeRelativeToNow">Grace duration relative to now after which the cached item will no longer be considered fresh and will only used for fall back to cache purpose</param>
        public virtual void SetGraceTimeStamp(TimeSpan graceTimeRelativeToNow)
        {
            this.GraceTimeStamp = DateTimeOffset.Now.Add(graceTimeRelativeToNow);
        }
    }
}
