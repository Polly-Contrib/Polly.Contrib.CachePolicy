using System;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Serializer;
using Xunit;

namespace Polly.Contrib.CachePolicy.Specs.serializer
{
    public class ProtobufSerializerSpecs
    {
        [Fact]
        public void UT_DateTimeOffset_Serialize()
        {
            var target = new SerializationTarget()
            {
                IsNull = true,
                GraceTimeStamp = DateTimeOffset.Now.AddHours(5),
                ChildMemberVariable = "hello world",
            };

            var serializer = new ProtobufSerializer(new LZ4PicklerBinaryCompressor());
            var serializedTarget = serializer.SerializeToBytes(target);
            var deserializedTarget = serializer.DeserializeFromBytes<SerializationTarget>(serializedTarget);

            Assert.Equal(target.GraceTimeStamp, deserializedTarget.GraceTimeStamp);
            Assert.Equal(target.IsNull, deserializedTarget.IsNull);
            Assert.Equal(target.ChildMemberVariable, deserializedTarget.ChildMemberVariable);
        }

        [Fact]
        public void UT_Null_Serialize()
        {
            var target = new SerializationTarget()
            {
                IsNull = true,
            };

            var serializer = new ProtobufSerializer(new LZ4PicklerBinaryCompressor());
            var serializedTarget = serializer.SerializeToBytes(target);
            var deserializedTarget = serializer.DeserializeFromBytes<SerializationTarget>(serializedTarget);

            Assert.Equal(target.GraceTimeStamp, deserializedTarget.GraceTimeStamp);
            Assert.Equal(target.IsNull, deserializedTarget.IsNull);
            Assert.Equal(target.ChildMemberVariable, deserializedTarget.ChildMemberVariable);
        }
    }    
}
