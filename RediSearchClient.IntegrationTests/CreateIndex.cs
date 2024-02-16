using RediSearchClient.Indexes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RediSearchClient.IntegrationTests;

public class CreateIndex : BaseIntegrationTest
{
    [Theory]
    [MemberData(nameof(CreateIndexTestCase.TestCaseData), MemberType = typeof(CreateIndexTestCase))]
    public void WillCreateIndex(CreateIndexTestCase testCase)
    {
        var testIndexName = $"{_indexName}_{testCase.Description}";

        _db.CreateIndex(testIndexName, testCase.IndexDefinition);

        var indexes = _db.ListIndexes();

        Assert.Contains(testIndexName, indexes);
    }

    [Theory]
    [MemberData(nameof(CreateIndexTestCase.TestCaseData), MemberType = typeof(CreateIndexTestCase))]
    public async Task WillCreateIndexAsync(CreateIndexTestCase testCase)
    {
        var testIndexName = $"{_indexName}_{testCase.Description}_async";

        await _db.CreateIndexAsync(testIndexName, testCase.IndexDefinition);

        var indexes = await _db.ListIndexesAsync();

        Assert.Contains(testIndexName, indexes);
    }

    public record CreateIndexTestCase(RediSearchIndexDefinition IndexDefinition, string Description)
    {
        private static readonly CreateIndexTestCase[] TestCases =
        {
            new (GetIndexDefinition(), "WillCreateAHashIndex"),
            new (GetJsonIndexDefinition(), "WillCreateAJsonIndex"),
            new (GetIndexDefinitionWithNoPrefix(), "WillCreateAnIndexWithoutAPrefix"),
            new (GetJsonIndexDefinitionWithNoPrefix(), "WillCreateAJsonIndexWithoutAPrefix"),
            new (GetHashIndexWithFlatVectorField(), "WillCreateHashIndexWithFlatVectorField"),
            new (GetHashIndexWithFlatVectorFieldAliased(), "WillCreateHashIndexWithAliasedFlatVectorField"),
            new (GetJsonIndexWithFlatVectorField(), "WillCreateJsonIndexWithFlatVectorField"),
            new (GetHashIndexWithHnswVectorField(), "WillCreateHashIndexWithHnswVectorField"),
            new (GetHashIndexWithHnswVectorFieldAliased(), "WillCreateHashIndexWithAliasedHnswVectorField"),
            new (GetJsonIndexWithHnswVectorField(), "WillCreateJsonIndexWithHnswVectorField"),
        };

        public static IEnumerable<object[]> TestCaseData =>
            TestCases.Select(tc => new object[] { tc });

        public override string ToString() => Description;
    }

    private static RediSearchIndexDefinition GetIndexDefinition() =>
        RediSearchIndex
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

    private static RediSearchIndexDefinition GetIndexDefinitionWithNoPrefix() =>
        RediSearchIndex
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

    private static RediSearchIndexDefinition GetJsonIndexDefinition() =>
        RediSearchIndex
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

    private static RediSearchIndexDefinition GetJsonIndexDefinitionWithNoPrefix() =>
        RediSearchIndex
            .OnJson()
            .WithSchema(
                x => x.Text("$.Id", "Id"),
                x => x.Text("$.FirstName", "FirstName", sortable: true),
                x => x.Text("$.Surname", "LastName", sortable: true),
                x => x.Numeric("$.BornSeconds", "Born", sortable: true),
                x => x.Numeric("$.DiedSeconds", "Died", sortable: true)
            )
            .Build();

    private static RediSearchIndexDefinition GetHashIndexWithFlatVectorField() =>
        RediSearchIndex
            .OnHash()
            .WithSchema(
                x => x.Text("Name"),
                x => x.Vector("Embedding",
                    VectorIndexAlgorithm.FLAT(
                        type: VectorType.FLOAT32,
                        dimensions: 32,
                        distanceMetric: DistanceMetric.L2,
                        initialCap: 30,
                        blockSize: 20))
            )
            .Build();

    private static RediSearchIndexDefinition GetHashIndexWithFlatVectorFieldAliased() =>
        RediSearchIndex
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

    private static RediSearchIndexDefinition GetJsonIndexWithFlatVectorField() =>
        RediSearchIndex
            .OnJson()
            .WithSchema(
                x => x.Text("$.Id", "Id"),
                x => x.Vector("$.Embedding", alias: "Embedded",
                    VectorIndexAlgorithm.FLAT(
                        type: VectorType.FLOAT64,
                        dimensions: 33,
                        distanceMetric: DistanceMetric.L2,
                        initialCap: 23,
                        blockSize: 22
                        ))
            )
            .Build();

    private static RediSearchIndexDefinition GetHashIndexWithHnswVectorField() =>
        RediSearchIndex
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

    private static RediSearchIndexDefinition GetHashIndexWithHnswVectorFieldAliased() =>
        RediSearchIndex
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

    private static RediSearchIndexDefinition GetJsonIndexWithHnswVectorField() =>
        RediSearchIndex
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