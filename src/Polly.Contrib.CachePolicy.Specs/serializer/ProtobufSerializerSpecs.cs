using System;
using Moq;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Providers.Serializer;
using Xunit;

namespace Polly.Contrib.CachePolicy.Specs.serializer
{
    public class ProtobufSerializerSpecs
    {
        private readonly Mock<ILoggingProvider> loggingProvider = new Mock<ILoggingProvider>();

        [Fact]
        public void UT_DateTimeOffset_Serialize()
        {
            var target = new SerializationTarget()
            {
                IsNull = true,
                GraceTimeStamp = DateTimeOffset.Now.AddHours(5),
                ChildMemberVariable = "hello world",
            };

            var serializer = new ProtobufSerializer(
                                   new LZ4PicklerBinaryCompressor(loggingProvider.Object),
                                   loggingProvider.Object);
            var serializedTarget = serializer.SerializeToBytes<SerializationTarget>(target, new Context());
            var deserializedTarget = serializer.DeserializeFromBytes<SerializationTarget>(serializedTarget, new Context());

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
            var loggingProviderMock = new Mock<ILoggingProvider>();

            var serializer = new ProtobufSerializer(
                                                                        new LZ4PicklerBinaryCompressor(loggingProvider.Object),
                                                                        loggingProvider.Object);
            var serializedTarget = serializer.SerializeToBytes<SerializationTarget>(target, new Context());
            var deserializedTarget = serializer.DeserializeFromBytes<SerializationTarget>(serializedTarget, new Context());

            Assert.Equal(target.GraceTimeStamp, deserializedTarget.GraceTimeStamp);
            Assert.Equal(target.IsNull, deserializedTarget.IsNull);
            Assert.Equal(target.ChildMemberVariable, deserializedTarget.ChildMemberVariable);
        }
    }    
}
