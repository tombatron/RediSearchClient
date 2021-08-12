using System.Text.Json;
using NReJSON;
using StackExchange.Redis;

namespace RediSearchClient.SampleData
{
    public sealed class SerializerProxy : ISerializerProxy
    {
        public TResult Deserialize<TResult>(RedisResult serializedValue) =>
            JsonSerializer.Deserialize<TResult>((string) serializedValue);

        public string Serialize<TObjectType>(TObjectType obj) =>
            JsonSerializer.Serialize(obj);
    }
}