using StackExchange.Redis;

namespace RediSearchClient
{
    internal static class ConversionUtilities
    {
        internal static int ConvertToInt(RedisResult redisResult)
        {
            var stringValue = (string)redisResult;

            if (stringValue.Contains("nan"))
            {
                return default;
            }
            else
            {
                return int.TryParse(stringValue, out var result) ? result : default;
            }
        }

        internal static double ConvertToDouble(RedisResult redisResult)
        {
            var stringValue = (string)redisResult;

            if (stringValue.Contains("nan"))
            {
                return default;
            }
            else
            {
                return double.TryParse(stringValue, out var result) ? result : default;
            }
        }
    }
}