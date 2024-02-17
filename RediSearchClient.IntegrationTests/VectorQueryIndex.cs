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

        var sampleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples");
        var binFiles = Directory.GetFiles(sampleDirectory, "*.bin");
        var names = binFiles.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();

        if (_db.KeyExists($"test_hash_vector:{names[0]}"))
        {
            return;
        }

        foreach (var binFile in binFiles)
        {
            var fileBytes = File.ReadAllBytes(binFile);
            var fileFloats = Enumerable.Range(0, fileBytes.Length / sizeof(float)).Select(i => BitConverter.ToSingle(fileBytes, i * sizeof(float))).ToArray();
            var name = Path.GetFileNameWithoutExtension(binFile);

            _db.HashSet($"test_hash_vector:{name}", new[]
            {
                    new HashEntry("name", name),
                    new HashEntry("feature_embeddings", fileBytes)
            });

            _db.JsonSet($"test_json_vector:{name}", new 
            { 
                name = name,
                feature_embeddings = fileFloats
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
