using Moq;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Providers.Serializer;
using Xunit;

namespace Polly.Contrib.CachePolicy.Specs.serializer
{
    public class NewtonsoftJsonSerializerSpecs
    {
        private readonly Mock<ILoggingProvider> loggingProvider = new Mock<ILoggingProvider>();

        [Fact]
        public void UT_Deserialize_CaseInsensitive()
        {
            var jsonPlaintext = @"{
                                    ""graceTimeStamp"": ""2020-05-18T01:58:34.5330156+00:00"",
                                    ""ChildMemberVariable"": null,
                                    ""IsNull"": false
                                  }";
            var serializer = new NewtonsoftJsonSerializer(
                                            new NoOpPlaintextCompressor(loggingProvider.Object),
                                            loggingProvider.Object);
            var deserializedObject = serializer.DeserializeFromString<SerializationTarget>(jsonPlaintext, new Context());
            Assert.NotNull(deserializedObject.GraceTimeStamp);
        }
    }
}
