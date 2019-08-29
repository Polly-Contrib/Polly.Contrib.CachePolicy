using FluentAssertions;
using Xunit;

namespace Polly.Contrib.BlankTemplate.Specs // Match the namespace to the main project, then add: .Specs
{
    public class MyContribSpecs
    {
        [Fact]
        public void ReplaceMeWithRealTests()
        {
            var instance = new MyContrib();

            instance.Should().NotBeNull();
        }
    }
}
