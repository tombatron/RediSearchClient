using StackExchange.Redis;

namespace RediSearchClient
{
    public class SearchResult
    {
        public RedisResult RawResult { get; }

        private SearchResult(RedisResult rawResult) =>
            RawResult = rawResult;
        public static SearchResult From(RedisResult redisResult) =>
            new SearchResult(redisResult);
    }
}