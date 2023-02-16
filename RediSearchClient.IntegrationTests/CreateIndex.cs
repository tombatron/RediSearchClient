using RediSearchClient.Indexes;
using System.Threading.Tasks;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class CreateIndex : BaseIntegrationTest
    {
        [Fact]
        public void WillCreateAHashIndex()
        {
            var indexDefinition = GetIndexDefinition();

            _db.CreateIndex(_indexName, indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(_indexName, indexes);
        }

        [Fact]
        public async Task WillCreateAHashIndexAsync()
        {
            var indexDefinition = GetIndexDefinition();

            var indexName = $"{_indexName}_async";

            await _db.CreateIndexAsync(indexName, indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateAJsonIndex()
        {
            var indexDefinition = GetJsonIndexDefinition();

            var indexName = $"json_{_indexName}";

            await _db.CreateIndexAsync(indexName, indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateAJsonIndexAsync()
        {
            var indexDefinition = GetJsonIndexDefinition();

            var indexName = $"json_{_indexName}_async";

            await _db.CreateIndexAsync(indexName, indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateAnIndexWithoutPrefix()
        {
            var indexDefinition = GetIndexDefinitionWithNoPrefix();

            var indexName = $"index_{_indexName}_noprefix";

            _db.CreateIndex(indexName, indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateAnIndexWithoutPrefixAsync()
        {
            var indexDefinition = GetIndexDefinitionWithNoPrefix();

            var indexName = $"index_{_indexName}_noprefix_async";

            await _db.CreateIndexAsync(indexName, indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateAJsonIndexWithoutPrefix()
        {
            var indexDefinition = GetJsonIndexDefinitionWithNoPrefix();

            var indexName = $"json_index_{_indexName}_noprefix";

            _db.CreateIndex(indexName, indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateAJsonIndexWithoutPrefixAsync()
        {
            var indexDefinition = GetJsonIndexDefinitionWithNoPrefix();

            var indexName = $"json_index_{_indexName}_noprefix_async";

            await _db.CreateIndexAsync(indexName, indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        private RediSearchIndexDefinition GetIndexDefinition()
        {
            return RediSearchIndex
                .OnHash()
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

        private RediSearchIndexDefinition GetIndexDefinitionWithNoPrefix()
        {
            return RediSearchIndex
                .OnHash()
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

        private RediSearchIndexDefinition GetJsonIndexDefinition()
        {
            return RediSearchIndex
                .OnJson()
                .ForKeysWithPrefix("laureate::")
                .WithSchema(
                    x => x.Text("$.Id", "Id"),
                    x => x.Text("$.FirstName", "FirstName", sortable: true),
                    x => x.Text("$.Surname", "LastName", sortable: true),
                    x => x.Numeric("$.BornSeconds", "Born", sortable: true),
                    x => x.Numeric("$.DiedSeconds", "Died", sortable: true)
                )
                .Build();
        }

        private RediSearchIndexDefinition GetJsonIndexDefinitionWithNoPrefix()
        {
            return RediSearchIndex
                .OnJson()
                .WithSchema(
                    x => x.Text("$.Id", "Id"),
                    x => x.Text("$.FirstName", "FirstName", sortable: true),
                    x => x.Text("$.Surname", "LastName", sortable: true),
                    x => x.Numeric("$.BornSeconds", "Born", sortable: true),
                    x => x.Numeric("$.DiedSeconds", "Died", sortable: true)
                )
                .Build();
        }        
    }
}