using System;
using Polly.Contrib.CachePolicy.Models;
using ProtoBuf;

namespace Polly.Contrib.CachePolicy.Specs.serializer
{
    [ProtoContract]
    public class SerializationTarget : CacheValue
    {
        [ProtoMember(1)]
        public override DateTimeOffset? GraceTimeStamp { get; set; }

        [ProtoMember(2)]
        public override bool IsNull { get; set; }

        [ProtoMember(3)]
        public string ChildMemberVariable { get; set; }
    }
}
