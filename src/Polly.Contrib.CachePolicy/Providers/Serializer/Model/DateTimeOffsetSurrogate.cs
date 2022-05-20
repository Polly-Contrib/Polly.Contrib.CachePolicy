using System;

using ProtoBuf;

namespace Polly.Contrib.CachePolicy.Providers.Serializer.Model
{
    /// <summary>
    /// There is no built-in type support for DateTimeOffset in protobuf-net.
    /// Please see this post on how to define a customized converter https://stackoverflow.com/questions/39087577/protobuf-net-and-serialization-of-datetimeoffset.
    /// </summary>
    [ProtoContract]
    public class DateTimeOffsetSurrogate
    {
        /// <summary>
        /// DateTime in string format to be serialized
        /// </summary>
        [ProtoMember(1)]
        public string DateTimeString { get; set; }

        /// <summary>
        /// Convert DateTimeOffSet to DateTimeOffsetSurrogate
        /// </summary>
        /// <param name="value">DateTimeOffset instance.</param>
        public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset value)
        {
            return new DateTimeOffsetSurrogate { DateTimeString = value.ToString("o") };
        }

        /// <summary>
        /// Convert DateTimeOffsetSurrogate to DateTimeOffSet
        /// </summary>
        /// <param name="value">DateTimeOffset instance.</param>
        public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate value)
        {
            return DateTimeOffset.Parse(value.DateTimeString);
        }
    }
}
