using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RediSearchClient.Converters
{
    /// <summary>
    /// RediSearch does not natively support DateTime types, therefore we need to convert DateTimes to numerics in this case
    /// </summary>
    internal sealed class DateTimeToNumericConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(new DateTimeOffset(value).ToUnixTimeSeconds());
        }
    }
}
