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

        [Fact]
        public void WillCreateHashIndexWithFlatVectorField()
        {
            var indexDefinition = GetHashIndexWithFlatVectorField();

            var indexName = $"hash_index_{_indexName}_flatvector";

            _db.CreateIndex($"{indexName}", indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateHashIndexWithFlatVectorFieldAsync()
        {
            var indexDefinition = GetHashIndexWithFlatVectorField();

            var indexName = $"hash_index_{_indexName}_flatvector_async";

            await _db.CreateIndexAsync($"{indexName}", indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateHashIndexWithAliasedFlatVectorField()
        {
            var indexDefinition = GetHashIndexWithFlatVectorFieldAliased();

            var indexName = $"hash_index_{_indexName}_flatvector_aliased";

            _db.CreateIndex($"{indexName}", indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateHashIndexWithAliasedFlatVectorFieldAsync()
        {
            var indexDefinition = GetHashIndexWithFlatVectorFieldAliased();

            var indexName = $"hash_index_{_indexName}_flatvector_aliased_async";

            await _db.CreateIndexAsync($"{indexName}", indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateJsonIndexWithFlatVectorField()
        {
            var indexDefinition = GetJsonIndexWithFlatVectorField();

            var indexName = $"json_index_{_indexName}_flatvector";

            _db.CreateIndex($"{indexName}", indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateJsonIndexWithFlatVectorFieldAsync()
        {
            var indexDefinition = GetJsonIndexWithFlatVectorField();

            var indexName = $"json_index_{_indexName}_flatvector_async";

            await _db.CreateIndexAsync($"{indexName}", indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateHashIndexWithHnswVectorField()
        {
            var indexDefinition = GetHashIndexWithHnswVectorField();

            var indexName = $"hash_index_{_indexName}_hnswvector";

            _db.CreateIndex($"{indexName}", indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateHashIndexWithHnswVectorFieldAsync()
        {
            var indexDefinition = GetHashIndexWithHnswVectorField();

            var indexName = $"hash_index_{_indexName}_hnswvector_async";

            await _db.CreateIndexAsync($"{indexName}", indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateHashIndexWithAliasedHnswVectorField()
        {
            var indexDefinition = GetHashIndexWithHnswVectorFieldAliased();

            var indexName = $"hash_index_{_indexName}_hnswvector_aliased";

            _db.CreateIndex($"{indexName}", indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateHashIndexWithAliasedHnswVectorFieldAsync()
        {
            var indexDefinition = GetHashIndexWithHnswVectorFieldAliased();

            var indexName = $"hash_index_{_indexName}_hnswvector_aliased_async";

            await _db.CreateIndexAsync($"{indexName}", indexDefinition);

            var indexes = await _db.ListIndexesAsync();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public void WillCreateJsonIndexWithHnswVectorField()
        {
            var indexDefinition = GetHashIndexWithHnswVectorField();

            var indexName = $"json_index_{_indexName}_hnswvector";

            _db.CreateIndex($"{indexName}", indexDefinition);

            var indexes = _db.ListIndexes();

            Assert.Contains(indexName, indexes);
        }

        [Fact]
        public async Task WillCreateJsonIndexWithHnswVectorFieldAsync()
        {
            var indexDefinition = GetHashIndexWithHnswVectorField();

            var indexName = $"json_index_{_indexName}_hnswvector_async";

            await _db.CreateIndexAsync($"{indexName}", indexDefinition);

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

        private RediSearchIndexDefinition GetHashIndexWithFlatVectorField()
        {
            return RediSearchIndex
                .OnHash()
                .WithSchema(
                    x=> x.Text("Name"),
                    x=> x.Vector("Embedding", 
                        VectorIndexAlgorithm.FLAT(
                            type: VectorType.FLOAT32, 
                            dimensions: 32, 
                            distanceMetric: DistanceMetric.L2, 
                            initialCap: 30, 
                            blockSize: 20))
                )
                .Build();
        }

        public RediSearchIndexDefinition GetHashIndexWithFlatVectorFieldAliased()
        {
            return RediSearchIndex
                .OnHash()
                .WithSchema(
                    x => x.Text("Name"),
                    x => x.Vector("Embedding", alias: "AliasedEmbedding",
                        VectorIndexAlgorithm.FLAT(
                            VectorType.FLOAT32,
                            32,
                            DistanceMetric.L2,
                            initialCap: 30,
                            blockSize: 20))
                )
                .Build();
        }

        public RediSearchIndexDefinition GetJsonIndexWithFlatVectorField()
        {
            return RediSearchIndex
                .OnJson()
                .WithSchema(
                    x=> x.Text("$.Id", "Id"),
                    x=> x.Vector("$.Embedding", alias: "Embedded",
                        VectorIndexAlgorithm.FLAT(
                            type: VectorType.FLOAT64,
                            dimensions: 33,
                            distanceMetric: DistanceMetric.L2,
                            initialCap: 23,
                            blockSize: 22
                            ))
                )
                .Build();
        }

        private RediSearchIndexDefinition GetHashIndexWithHnswVectorField()
        {
            return RediSearchIndex
                .OnHash()
                .WithSchema(
                    x => x.Text("Name"),
                    x => x.Vector("Embedding",
                        VectorIndexAlgorithm.HNSW(
                            VectorType.FLOAT32,
                            32,
                            DistanceMetric.COSINE,
                            initialCap: 20,
                            m: 30,
                            efConstruction: 22,
                            efRuntime: 1,
                            epsilon: 1.2f))
                )
                .Build();
        }

        public RediSearchIndexDefinition GetHashIndexWithHnswVectorFieldAliased()
        {
            return RediSearchIndex
                .OnHash()
                .WithSchema(
                    x => x.Text("Name"),
                    x => x.Vector("Embedding", alias: "AliasedEmbedding",
                        VectorIndexAlgorithm.HNSW(
                            VectorType.FLOAT32,
                            32,
                            DistanceMetric.COSINE,
                            initialCap: 20,
                            m: 30,
                            efConstruction: 22,
                            efRuntime: 1,
                            epsilon: 1.2f))
                )
                .Build();
        }

        public RediSearchIndexDefinition GetJsonIndexWithHnswVectorField()
        {
            return RediSearchIndex
                .OnJson()
                .WithSchema(
                    x => x.Text("$.Id", "Id"),
                    x => x.Vector("$.Embedding", alias: "Embedded",
                        VectorIndexAlgorithm.HNSW(
                            VectorType.FLOAT32,
                            32,
                            DistanceMetric.COSINE,
                            initialCap: 20,
                            m: 30,
                            efConstruction: 22,
                            efRuntime: 1,
                            epsilon: 1.2f))
                )
                .Build();
        }
    }
}