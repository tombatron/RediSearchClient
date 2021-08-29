#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NReJSON;
using StackExchange.Redis;

namespace RediSearchClient.SampleData
{
    public sealed class SerializerProxy : ISerializerProxy
    {
        private static readonly JsonSerializerOptions _options;

        static SerializerProxy()
        {
            _options = new JsonSerializerOptions();
            
            _options.Converters.Add(new NullToEmptyStringConverter());
        }
        
        public TResult? Deserialize<TResult>(RedisResult serializedValue) =>
            JsonSerializer.Deserialize<TResult>((string) serializedValue);

        public string Serialize<TObjectType>(TObjectType obj) =>
            JsonSerializer.Serialize(obj, _options);
    }

    public sealed class NullToEmptyStringConverter : JsonConverter<string>
    {
        public override bool HandleNull => true;
        
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(string);

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType == JsonTokenType.Null ? String.Empty : reader.GetString();

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value ?? string.Empty);
    }
}