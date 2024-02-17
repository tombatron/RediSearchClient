using NReJSON;
using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace RediSearchClient.IntegrationTests;

public class VectorQueryIndex : BaseIntegrationTest
{
    [Fact]
    public void CanDoVectorQuery()
    {
        CreateTestVectorData();
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

        _db.CreateIndex("test_hash_vector_query", hashIndex);
        _db.CreateIndex("test_json_vector_query", jsonIndex);
    }
}
