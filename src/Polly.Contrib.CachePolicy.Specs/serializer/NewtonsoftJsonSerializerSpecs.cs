using System;
using Polly.Contrib.CachePolicy.Providers.Compressor;
using Polly.Contrib.CachePolicy.Providers.Serializer;
using Xunit;

namespace Polly.Contrib.CachePolicy.Specs.serializer
{
    public class NewtonsoftJsonSerializerSpecs
    {
        [Fact]
        public void UT_Deserialize_CaseInsensitive()
        {
            var jsonPlaintext = @"{
                                ""graceTimeStamp"": ""2020-05-18T01:58:34.5330156+00:00"",
                                ""ChildMemberVariable"": null,
                                ""IsNull"": false
                              }";
            var serializer = new NewtonsoftJsonSerializer(new NoOpPlaintextCompressor());
            var deserializedObject = serializer.DeserializeFromString<SerializationTarget>(jsonPlaintext);
            Assert.NotNull(deserializedObject.GraceTimeStamp);
        }
    }
}
