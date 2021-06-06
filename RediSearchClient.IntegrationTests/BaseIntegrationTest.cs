using System;
using StackExchange.Redis;
using static RediSearchClient.IntegrationTests.SampleData;

namespace RediSearchClient.IntegrationTests
{
    public abstract class BaseIntegrationTest : IDisposable
    {
        protected const string MovieDataPrefix = "movie::";

        private ConnectionMultiplexer _muxr;
        protected IDatabase _db;
        protected string _indexName;
        protected string _recordPrefix;
        protected string _dictionaryName;

        public virtual void Setup()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _db = _muxr.GetDatabase(0);

            _indexName = Guid.NewGuid().ToString("n");
            _recordPrefix = Guid.NewGuid().ToString("n");
            _dictionaryName = Guid.NewGuid().ToString("n");

            SetupDemoMovieData();
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
    }
}