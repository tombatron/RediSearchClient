using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// This class represents a single aggregate result.
    /// </summary>
    public class AggregateResultItem
    {
        /// <summary>
        /// Name of the result field.
        /// </summary>
        /// <value></value>
        public string Key { get; }

        /// <summary>
        /// Value of the result field.
        /// </summary>
        /// <value></value>
        public RedisResult Value { get; }

        internal AggregateResultItem(string key, RedisResult value)
        {
            Key = key;
            Value = value;
        }
    }
}