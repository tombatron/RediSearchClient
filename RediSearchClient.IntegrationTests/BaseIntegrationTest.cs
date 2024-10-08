using NReJSON;
using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using static RediSearchClient.IntegrationTests.SampleData;

namespace RediSearchClient.IntegrationTests;

public abstract class BaseIntegrationTest : IDisposable
{
    private static bool HasIndexCleanupRun = false;

    protected const string MovieDataPrefix = "movie::";

    private ConnectionMultiplexer _muxr;
    protected IDatabase _db;
    protected string _indexName;
    protected string _recordPrefix;
    protected string _dictionaryName;
    protected string _hashVectorIndexName;
    protected string _jsonVectorIndexName;

    protected virtual void Setup()
    {
        NReJSONSerializer.SerializerProxy = new SystemTextJsonSerializer();

        _muxr = ConnectionMultiplexer.Connect("localhost");

        _db = _muxr.GetDatabase(0);

        _indexName = Guid.NewGuid().ToString("n");
        _recordPrefix = Guid.NewGuid().ToString("n");
        _dictionaryName = Guid.NewGuid().ToString("n");
        _hashVectorIndexName = Guid.NewGuid().ToString("n");
        _jsonVectorIndexName = Guid.NewGuid().ToString("n");

        SetupDemoMovieData();
        SetupTestVectorData();
    }

    public virtual void TearDown()
    {
        //CleanupIndexes();

        _muxr.Dispose();
    }

    public BaseIntegrationTest()
    {
        Setup();
    }

    public void Dispose()
    {
        TearDown();
    }

    private void SetupDemoMovieData()
    {
        if (_db.KeyExists($"{MovieDataPrefix}1"))
        {
            // Movies are already in the database, bail. 
            return;
        }

        for (var i = 0; i < Movies.Length; i++)
        {
            _db.HashSet($"{MovieDataPrefix}{i + 1}", Movies[i]);
        }
    }

    private void SetupTestVectorData()
    {
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

        _db.CreateIndex(_hashVectorIndexName, hashIndex);
        _db.CreateIndex(_jsonVectorIndexName, jsonIndex);

        Thread.Sleep(500);
    }

    private static object locker = new object();

    private void CleanupIndexes()
    {
        if (!HasIndexCleanupRun)
        {
            lock (locker)
            {
                if (!HasIndexCleanupRun)
                {
                    foreach (var index in _db.ListIndexes())
                    {
                        if(Guid.TryParse(index, out var _))
                        {
                            _db.DropIndex(index);
                        }
                    }

                    HasIndexCleanupRun = true;
                }
            }
        }
    }
}

public sealed class SystemTextJsonSerializer : ISerializerProxy
{
    public TResult Deserialize<TResult>(RedisResult serializedValue) =>
        JsonSerializer.Deserialize<TResult>(serializedValue.ToString());

    public string Serialize<TObjectType>(TObjectType obj) =>
        JsonSerializer.Serialize(obj);
}