using NReJSON;
using RediSearchClient.Indexes;
using RediSearchClient.Query;
using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.IntegrationTests;

public class VectorQueryIndex : BaseIntegrationTest
{
    private const string HashVectorIndexName = "test_hash_vector_index";
    private const string JsonVectorIndexName = "test_json_vector_index";

    [Fact]
    public void CanDoVectorQuery()
    {
        CreateTestVectorData();
    }

    public class KNearestNeighorQuery
    {
        [Fact]
        public void AgainstHashIndex()
        {
            var knnQuery = RediSearchQuery
                .On(HashVectorIndexName)
                    .VectorKnn()
                        .Prefilter("")
                        .NumberOfNeighbors(10)
                        .FieldName("feature_embeddings")
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .Limit(0, 10)
                        .Dialect(2)
                        .SortByDistance()
                        .EfRuntime(10)
                        .Epsilon(0.01f)
                    .Build();

            var rangeQuery = RediSearchQuery
                .On(HashVectorIndexName)
                    .VectorRange()
                        .FieldName("feature_embeddings")
                        .Range(0.5f)
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .DistanceFieldName("distance")
                        .Epsilon(0.5f)
                        .Dialect(2)
                        .SortBy("distance")
                    .Build();
        }

       
    }

    private void CreateTestVectorData()
    {
        // Load the vector data.
        if (_db.KeyExists($"test_hash_vector:{SampleData.SampleVectorData[0].Name}"))
        {
            return;
        }

        foreach (var vec in SampleData.SampleVectorData)
        {
            _db.HashSet($"test_hash_vector:{vec.Name}", new[]
            {
                new HashEntry("name", vec.Name),
                new HashEntry("feature_embeddings", vec.FileBytes)
            });

            _db.JsonSet($"test_json_vector:{vec.Name}", new 
            { 
                name = vec.Name,
                feature_embeddings = vec.FileFloats
            });
        }

        // Create the test indexes. 
        var hashIndex = RediSearchIndex
            .OnHash()
            .ForKeysWithPrefix("test_hash_vector:")
            .WithSchema(
                s => s.Text("name"),
                s => s.Vector("feature_embeddings", 
                    VectorIndexAlgorithm.HNSW(
                        type: VectorType.FLOAT32, 
                        dimensions: 512, // Used ResNet34 to generate feature embeddings...
                        distanceMetric: DistanceMetric.COSINE
                     ))
                ).Build();

        var jsonIndex = RediSearchIndex
            .OnJson()
            .ForKeysWithPrefix("test_hash_vector:")
            .WithSchema(
                s => s.Text("$.name", "name"),
                s => s.Vector("$.feature_embeddings", "feature_embeddings",
                    VectorIndexAlgorithm.HNSW(
                        type: VectorType.FLOAT32,
                        dimensions: 512, // Used ResNet34 to generate feature embeddings...
                        distanceMetric: DistanceMetric.COSINE
                     ))
                ).Build();

        _db.CreateIndex(HashVectorIndexName, hashIndex);
        _db.CreateIndex(JsonVectorIndexName, jsonIndex);
    }
}
