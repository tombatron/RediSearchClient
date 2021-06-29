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