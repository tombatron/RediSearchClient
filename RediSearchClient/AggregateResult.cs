using StackExchange.Redis;

namespace RediSearchClient
{
    public class AggregateResult
    {
        public RedisResult RawResult { get; }

        private AggregateResult(RedisResult rawResult) =>
            RawResult = rawResult;

        public static AggregateResult From(RedisResult redisResult) =>
            new AggregateResult(redisResult);
    }
}