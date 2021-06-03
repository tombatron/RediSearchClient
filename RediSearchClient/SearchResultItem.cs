using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    public class SearchResultItem
    {
        public string DocumentKey { get; private set; }

        public IDictionary<string, RedisResult> Fields { get; private set; }

        internal static SearchResultItem Create(string documentKey, RedisResult[] rawFields)
        {
            var result = new SearchResultItem
            {
                DocumentKey = documentKey
            };

            var fields = new Dictionary<string, RedisResult>();

            for (var i = 0; i < rawFields.Length; i++)
            {
                var key = (string)rawFields[i];
                var value = rawFields[++i];

                fields.Add(key, value);
            }

            result.Fields = fields;

            return result;
        }
    }
}