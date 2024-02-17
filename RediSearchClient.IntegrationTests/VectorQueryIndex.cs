using NReJSON;
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
    }
}
