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

        public string KeyType { get; private set; }

        public string[] Prefixes { get; private set; }

        public string Filter { get; private set; }

        public string LanguageField { get; private set; }

        public double DefaultScore { get; private set; }

        public string ScoreField { get; private set; }

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