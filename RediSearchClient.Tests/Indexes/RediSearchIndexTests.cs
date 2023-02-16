using Xunit;
using RediSearchClient.Indexes;

namespace RediSearchClient.Tests.Indexes
{
    public class RediSearchIndexTests
    {
        public class JsonIndexes
        {
            [Fact]
            public void CanBeCreatedWithoutAPrefix()
            {
                var index = RediSearchIndex.OnJson().WithSchema(x => x.Text("$.Testing", "Testing")).Build();

                Assert.NotNull(index);
            }
        }

        public class HashIndexes
        {
            [Fact]
            public void CanBeCreatedWithoutAPrefix()
            {
                var index = RediSearchIndex.OnHash().WithSchema(x => x.Text("Testing")).Build();

                Assert.NotNull(index);
            }
        }
    }
}