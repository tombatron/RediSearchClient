using RediSearchClient.Aggregate;
using RediSearchClient.Indexes;
using RediSearchClient.Query;
using StackExchange.Redis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class AggregateIndex : BaseIntegrationTest
    {
        protected override void Setup()
        {
            base.Setup();

            CreateTestSearchData();
        }

        [Fact]
        public void CanCreateAndExecuteASimpleAggregation()
        {
            var aggregation = CreateSampleAggregationQuery();

            var query = aggregation.ToString();

            var result = _db.Aggregate(aggregation);

            Assert.NotNull(result.RawResult);
        }

        [Fact]
        public async Task CanCreateAndExecuteASimpleAggregationAsync()
        {
            var aggregation = CreateSampleAggregationQuery();

            var query = aggregation.ToString();

            var result = await _db.AggregateAsync(aggregation);

            Assert.NotNull(result.RawResult);
        }

        [Fact]
        public void CanParseQueryResult()
        {
            var aggregation = CreateSampleAggregationQuery();

            var result = _db.Aggregate(aggregation);

            Assert.NotNull(result);
            Assert.NotNull(result.RawResult);
            Assert.Equal(1, result.RecordCount);

            var firstResult = result.First();

            Assert.Equal("demo", (string)firstResult["documentType"]);
            Assert.Equal(15, (int)firstResult["total"]);
        }

        private RediSearchAggregateDefinition CreateSampleAggregationQuery()
        {
            return RediSearchAggregateQuery
                .On(_indexName)
                .Query("*")
                .Load("@score")
                .GroupBy(gb =>
                {
                    gb.Fields("@documentType");
                    gb.Reduce(Reducer.Sum, "@score").As("total");
                })
                .SortBy(sb => 
                {
                    sb.Field("@documentType", Direction.Ascending);
                    sb.Field("@total", Direction.Descending);
                    sb.Max(100);
                })
                .Build();
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

            while (_db.GetInfo(_indexName).Indexing == 1)
            {
                Thread.Sleep(500);
            }
        }
    }
}