using RediSearchClient.Converters;
using System.Text.Json;

namespace RediSearchClient
{
    internal sealed class RediSearchJsonSerializerOptionsFactory
    {
        private static JsonSerializerOptions _jsonSerializerOptions;

        internal static JsonSerializerOptions GetOptions()
        {
            if(_jsonSerializerOptions is null)
            {
                _jsonSerializerOptions = new JsonSerializerOptions();
                _jsonSerializerOptions.Converters.Add(new DateTimeToNumericConverter());
            }

            return _jsonSerializerOptions;
        }
    }
}
