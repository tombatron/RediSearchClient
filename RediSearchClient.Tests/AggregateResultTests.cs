using System.Linq;
using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.Tests
{
    public class AggregateResultTests
    {
        [Fact]
        public void CanDynamicallyCreateResultMapping()
        {
            var aggregateResult = AggregateResult.From(FakeAggregateResult);

            var mappedResult = aggregateResult.As<BasicAggregateResult>();

            var firstResult = mappedResult.First();

            Assert.Equal("total_things", firstResult.ag_field1);
            Assert.Equal(128, firstResult.ag_field2);
        }

        [Fact]
        public void CanProvideResultMapping()
        {
            ResultMapper.CreateMapFor<SomeAggregateModel>(
                ("ag_field1", "Label", (r) => (string)r),
                ("ag_field2", "Value", (r) => (int)r)
            );

            var searchResult = AggregateResult.From(FakeAggregateResult);

            var mappedResult = searchResult.As<SomeAggregateModel>();

            var firstResult = mappedResult.First();

            Assert.Equal("total_things", firstResult.Label);
            Assert.Equal(128, firstResult.Value);
        }

        private static RedisResult FakeAggregateResult = RedisResult.Create(new[]
        {
            RedisResult.Create(1, ResultType.Integer),

            RedisResult.Create(new[]
            {
                RedisResult.Create("ag_field1", ResultType.BulkString),
                RedisResult.Create("total_things", ResultType.BulkString),

                RedisResult.Create("ag_field2", ResultType.BulkString),
                RedisResult.Create("128", ResultType.BulkString),
            })
        });

        public class BasicAggregateResult
        {
            public string ag_field1 { get; set; }

            public int ag_field2 { get; set; }
        }

        public class SomeAggregateModel
        {
            public string Label { get; set; }

            public int Value { get; set; }
        }
    }
}