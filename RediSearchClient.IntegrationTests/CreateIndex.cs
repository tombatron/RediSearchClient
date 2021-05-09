using System.Threading.Tasks;
using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class CreateIndex : IDisposable
    {
        private ConnectionMultiplexer _muxr;
        private IDatabase _db;

        private void Setup()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _db = _muxr.GetDatabase(0);
        }

        private void TearDown()
        {
            _muxr.Dispose();
        }

        [Fact]
        public void WillCreateASimpleIndex()
        {
            var indexName = Guid.NewGuid().ToString("n");

            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix("Bogus::")
                .WithSchema(x => x.Text("field_one"))
                .Build();

            _db.CreateIndex(indexName, index);
        }

        [Fact]
        public async Task WillCreateASimpleIndexAsync()
        {
            var indexName = Guid.NewGuid().ToString("n");

            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix("Bogus::")
                .WithSchema(x=>x.Text("field_one"))
                .Build();

            await _db.CreateIndexAsync(indexName, index);
        }

        public CreateIndex() => Setup();

        public void Dispose() => TearDown();
    }
}