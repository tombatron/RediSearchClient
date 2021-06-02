using System;
using StackExchange.Redis;

namespace RediSearchClient.IntegrationTests
{
    public abstract class BaseIntegrationTest : IDisposable
    {
        private ConnectionMultiplexer _muxr;
        protected IDatabase _db;
        protected string _indexName;
        protected string _recordPrefix;

        public virtual void Setup()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _db = _muxr.GetDatabase(0);

            _indexName = Guid.NewGuid().ToString("n");
            _recordPrefix = Guid.NewGuid().ToString("n");
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
    }
}