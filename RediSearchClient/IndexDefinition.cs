using System.Linq;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Description of the options used to create an index.
    /// </summary>
    public class IndexDefinition
    {
        internal IndexDefinition() { }

        /// <summary>
        /// It's the type... of the key. Can be: TEXT, NUMERIC, GEO, TAG
        /// </summary>
        /// <value></value>
        public string KeyType { get; private set; }

        /// <summary>
        /// Which key prefixes are being indexed?
        /// </summary>
        /// <value></value>
        public string[] Prefixes { get; private set; }

        /// <summary>
        /// The filter expression applied to the index during creation. 
        /// </summary>
        /// <value></value>
        public string Filter { get; private set; }

        /// <summary>
        /// If set, indicates the field on the document that should specify the document's language.
        /// </summary>
        /// <value></value>
        public string LanguageField { get; private set; }

        /// <summary>
        /// The default score for documents added to the specific index.
        /// </summary>
        /// <value></value>
        public double DefaultScore { get; private set; }

        /// <summary>
        /// Indicates which field on a document should be used as the document's rank.
        /// </summary>
        /// <value></value>
        public string ScoreField { get; private set; }

        /// <summary>
        /// If set, indicates which field on the document should as a binary safe payload string.
        /// </summary>
        /// <value></value>
        public string PayloadField { get; private set; }

        internal static IndexDefinition Create(RedisResult[] redisResults)
        {
            var result = new IndexDefinition();

            for (var i = 0; i < redisResults.Length; i++)
            {
                var label = (string)redisResults[i];

                switch (label)
                {
                    case "key_type":
                        result.KeyType = (string)redisResults[++i];
                        break;
                    case "prefixes":
                        var prefixes = (RedisResult[])redisResults[++i];
                        result.Prefixes = prefixes.Select(x => x.ToString()).ToArray();
                        break;
                    case "filter":
                        result.Filter = (string)redisResults[++i];
                        break;
                    case "language_field":
                        result.LanguageField = (string)redisResults[++i];
                        break;
                    case "default_score":
                        result.DefaultScore = (double)redisResults[++i];
                        break;
                    case "score_field":
                        result.ScoreField = (string)redisResults[++i];
                        break;
                    case "payload_field":
                        result.PayloadField = (string)redisResults[++i];
                        break;
                    default:
                        ++i;
                        break;
                }
            }

            return result;
        }
    }
}