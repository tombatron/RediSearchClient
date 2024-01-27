using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.Tests;

public class SuggestionResultCollectionTests
{
    [Fact]
    public void Create_ReturnsEmptyArray_WithNullInput()
    {
        RedisResult redisResult = RedisResult.Create(new RedisValue(null));
        var res = SuggestionResult.CreateArray(redisResult, true, true);

        Assert.Empty(res);
    }

}