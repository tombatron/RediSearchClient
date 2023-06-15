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

            [Fact]
            public void CanBeCreatedWithNoStopwords()
            {
                var index = RediSearchIndex
                    .OnHash()
                    .WithNoStopwords()
                    .WithSchema(x => x.Text("Testing"))
                    .Build();

                Assert.NotNull(index);
            }

            [Fact]
            public void CanBeCreatedWithCustomStopwordsList()
            {
                var index = RediSearchIndex
                    .OnHash()
                    .WithStopwords(new []{"if", "or", "else"})
                    .WithSchema(x => x.Text("Testing"))
                    .Build();

                Assert.NotNull(index);
            }
            [Fact]
            public void CanBeCreatedWithCustomStopwordsParam()
            {
                var index = RediSearchIndex
                    .OnHash()
                    .WithStopwords("if", "or", "else")
                    .WithSchema(x => x.Text("Testing"))
                    .Build();

                Assert.NotNull(index);
            }

            [Fact]
            public void CanBeCreatedWithCustomStopword()
            {
                var index = RediSearchIndex
                    .OnHash()
                    .WithStopwords("if")
                    .WithSchema(x => x.Text("Testing"))
                    .Build();

                Assert.NotNull(index);
            }
        }
    }
}