using System;
using System.Text.Json;
using NReJSON;
using StackExchange.Redis;
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

    protected virtual void Setup()
    {
        NReJSONSerializer.SerializerProxy = new SystemTextJsonSerializer();

        _muxr = ConnectionMultiplexer.Connect("localhost");

        _db = _muxr.GetDatabase(0);

        _indexName = Guid.NewGuid().ToString("n");
        _recordPrefix = Guid.NewGuid().ToString("n");
        _dictionaryName = Guid.NewGuid().ToString("n");

        SetupDemoMovieData();

        CleanupIndexes();
    }

    public virtual void TearDown()
    {
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