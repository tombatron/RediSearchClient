using StackExchange.Redis;

namespace RediSearchClient
{
    public class AggregateResultItem
    {
        public string Key { get; }
        public RedisResult Value { get; }

        internal AggregateResultItem(string key, RedisResult value)
        {
            Key = key;
            Value = value;
        }
    }
}