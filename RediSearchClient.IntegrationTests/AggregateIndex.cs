using RediSearchClient.Aggregate;
using RediSearchClient.Indexes;
using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class AggregateIndex : BaseIntegrationTest
    {
        public override void Setup()
        {
            CreateTestSearchData();
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