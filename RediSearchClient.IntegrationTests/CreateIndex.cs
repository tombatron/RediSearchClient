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
            var indexDefinition = GetComplexIndexDefinition();

            _db.CreateIndex(_indexName, indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(_indexName, indexes);
        }

        [Fact]
        public async Task WillCreateASimpleIndexAsync()
        {
            var indexDefinition = GetComplexIndexDefinition();

            var indexName = $"{_indexName}_async";
            
            await _db.CreateIndexAsync(indexName, indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        private RediSearchIndexDefinition GetComplexIndexDefinition()
        {
            return RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix("zip::")
                .UsingFilter("@State=='FL'")
                .UsingLanguage("English")
                .SetScore(0.5)
                .Temporary(600)
                .NoHighLights()
                .WithSchema(
                    x => x.Text("ZipCode", sortable: false, nostem: true),
                    x => x.Text("City", sortable: true),
                    x => x.Text("State", sortable: true, nostem: true),
                    x => x.Geo("Coordinates"),
                    x => x.Numeric("TimeZoneOffset"),
                    x => x.Numeric("DaylightSavingsFlag")
                )
                .Build();
        }
    }
}