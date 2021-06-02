using RediSearchClient.Indexes;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class CreateIndex : BaseIntegrationTest
    {
        [Fact]
        public void WillCreateASimpleIndex()
        {
            var indexName = Guid.NewGuid().ToString("n");

            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix("Bogus::")
                .WithSchema(x => x.Text("field_one"))
                .Build();

            _db.CreateIndex(indexName, index);
        }

        [Fact]
        public async Task WillCreateASimpleIndexAsync()
        {
            var indexName = Guid.NewGuid().ToString("n");

            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix("Bogus::")
                .WithSchema(x=>x.Text("field_one"))
                .Build();

            await _db.CreateIndexAsync(indexName, index);
        }
    }
}