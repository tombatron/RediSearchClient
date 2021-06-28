using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a single search result. 
    /// </summary>
    public class SearchResultItem
    {
        /// <summary>
        /// The key to the HASH or JSON object that was indexed by RediSearch. 
        /// </summary>
        /// <value></value>
        public string DocumentKey { get; private set; }

        /// <summary>
        /// The fields returned from the search index. 
        /// </summary>
        /// <value></value>
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

        /// <summary>
        /// Access a field by key name.
        /// </summary>
        /// <returns></returns>
        public RedisResult this[string key] => Fields.TryGetValue(key, out var value) ? value : default;
    }
}