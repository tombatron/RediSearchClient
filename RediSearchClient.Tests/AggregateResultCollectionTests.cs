using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.Tests
{
    public class AggregateResultCollectionTests
    {
        [Fact]
        public void ItCanHandleNullAggregationResult()
        {
            var result = new AggregateResultCollection(null);

            Assert.Empty(result);
        }

        [Fact]
        public void ItCanHandleEmptyAggregationResult()
        {
            var emptyRedisResult = CreateEmptyRedisResultArray();

            var result = new AggregateResultCollection(emptyRedisResult);

            Assert.Empty(result);
        }

        [Fact]
        public void ItCanAccessResultByIndex()
        {
            var realishResult = CreateRealLookingRedisResultArray();

            var result = new AggregateResultCollection(realishResult);

            Assert.Equal("moon", (string)result[1]);
        }

        [Fact]
        public void ItCanAccessResultByKeyName()
        {
            var realishResult = CreateRealLookingRedisResultArray();

            var result = new AggregateResultCollection(realishResult);

            Assert.Equal("world", (string)result["hello"]);
        }

        [Fact]
        public void NonExistantResultByIndexReturnsDefault()
        {
            var realishResult = CreateRealLookingRedisResultArray();

            var result = new AggregateResultCollection(realishResult);

            Assert.Equal(default, (string)result[100]);
        }

        [Fact]
        public void NonExistantResultByKeyNameReturnsDefault()
        {
            var realishResult = CreateRealLookingRedisResultArray();

            var result = new AggregateResultCollection(realishResult);

            var whatever = result["supdawg"];

            Assert.Equal(default, (string)result["supdawg"]);
        }

        private RedisResult[] CreateEmptyRedisResultArray()
        {
            var item = RedisResult.Create(new RedisValue[0]);

            return (RedisResult[])item;
        }

        private RedisResult[] CreateRealLookingRedisResultArray()
        {
            var values = new RedisValue[4]
            {
                new RedisValue("hello"),
                new RedisValue("world"),
                new RedisValue("goodnight"),
                new RedisValue("moon")
            };

            var item = RedisResult.Create(values);

            return (RedisResult[])item;
        }
    }
}