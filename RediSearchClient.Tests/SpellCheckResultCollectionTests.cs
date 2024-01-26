using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.Tests
{
    public class SpellCheckResultCollectionTests
    {
        [Fact]
        public void ItWillReturnZeroCountIfNullArrayIsProvided()
        {
            var collection = new SpellCheckResultCollection(null);

            Assert.Empty(collection);
        }   

        [Fact]
        public void ItWillReturnLengthOfUnderlyingArrayForCount()
        {
            var array = new SpellCheckResult[5];

            var collection = new SpellCheckResultCollection(array);

            Assert.Equal(5, collection.Count);
        }   

        [Fact]
        public void CanIterateUsingForEach()
        {
            var array = new SpellCheckResult[5];

            var collection = new SpellCheckResultCollection(array);

            var count = 0;

            foreach(var r in collection)
            {
                ++count;
            }

            Assert.Equal(5, count);
        }

        [Fact]
        public void Create_ReturnsEmptyArray_WithNullInput()
        {
            RedisResult redisResult = RedisResult.Create(new RedisValue(null));
            var res = SpellCheckResult.CreateArray(redisResult);

            Assert.Empty(res);
        }

        [Fact]
        public void CanGetASuggestionByIndex()
        {

        }

        [Fact]
        public void CanGetASuggestionByTerm()
        {

        }

        [Fact]
        public void AccessingSuggestionByIndexWithOutOfBoundsReturnsNull()
        {

        }

        [Fact]
        public void AccessingSuggestionByNameWithNonExistingItemReturnsNull()
        {

        }
    }
}