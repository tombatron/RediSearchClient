using RediSearchClient.Aggregate;
using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class AggregateIndex : BaseIntegrationTest
    {
        private ConnectionMultiplexer _muxr;
        private IDatabase _db;
        private string _indexName;
        private string _recordPrefix;

        public override void Setup()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _db = _muxr.GetDatabase(0);

            _indexName = Guid.NewGuid().ToString("n");
            _recordPrefix = Guid.NewGuid().ToString("n");

            CreateTestSearchData();
        }

        public override void TearDown()
        {
            _muxr.Dispose();
        }

        [Fact]
        public void CanCreateAndExecuteASimpleAggregation()
        {
            var aggregation = RediSearchAggregateQuery
                .On(_indexName)
                .Query("*")
                .Load("@score")
                .GroupBy(gb => 
                { 
                    gb.Fields("@documentType");
                    gb.Reduce(Reducer.Sum, "@score").As("total");
                })
                .Build();

            var query = aggregation.ToString();

            var result = _db.Aggregate(aggregation);

            Assert.NotNull(result.RawResult);
        }

        private void CreateTestSearchData()
        {
            _db.HashSet($"{_recordPrefix}:1", new[]
            {
                 new HashEntry("score", 1),
                 new HashEntry("documentType", "demo")
            });

            _db.HashSet($"{_recordPrefix}:2", new[]
            {
                new HashEntry("score", 2),
                new HashEntry("documentType", "demo")
            });

            _db.HashSet($"{_recordPrefix}:3", new[]
            {
                new HashEntry("score", 3),
                new HashEntry("documentType", "demo")
            });

            _db.HashSet($"{_recordPrefix}:4", new[]
            {
                new HashEntry("score", 4),
                new HashEntry("documentType", "demo")
            });

            _db.HashSet($"{_recordPrefix}:5", new[]
            {
                new HashEntry("score", 5),
                new HashEntry("documentType", "demo")
            });

            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix($"{_recordPrefix}:")
                .WithSchema(x => x.Numeric("score"), x => x.Text("documentType"))
                .Build();

            _db.CreateIndex(_indexName, index);
        }
    }
}