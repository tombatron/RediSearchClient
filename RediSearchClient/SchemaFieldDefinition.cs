using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a field defined within a schema.
    /// </summary>
    public class SchemaFieldDefinition
    {
        internal SchemaFieldDefinition() {}

        internal static SchemaFieldDefinition[] CreateArray(RedisResult[] redisResult)
        {
            return default;
        }
    }
}