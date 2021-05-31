using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// It's just a suggestion.
    /// </summary>
    public class SuggestionResult
    {
        /// <summary>
        /// Suggestion entry.
        /// </summary>
        /// <value></value>
        public string Suggestion { get; private set; }

        /// <summary>
        /// Suggestion score.
        /// </summary>
        /// <value></value>
        public double? Score { get; private set; }

        /// <summary>
        /// Suggestion payload.
        /// </summary>
        /// <value></value>
        public string Payload { get; private set; }

        internal static SuggestionResult[] CreateArray(RedisResult redisResult, bool withScores, bool withPayloads)
        {
            var redisResultArray = (RedisResult[])redisResult;

            var suggestionComponentLength = 1 + (withScores ? 1 : 0) + (withPayloads ? 1 : 0);

            var suggestionResult = new SuggestionResult[redisResultArray.Length / suggestionComponentLength];
            var suggestionResultIndex = -1;

            for (var i = 0; i < redisResultArray.Length; i++)
            {
                suggestionResult[++suggestionResultIndex] = new SuggestionResult
                {
                    Suggestion = (string)redisResultArray[i],
                    Score = (double?)(withScores ? (double?)redisResultArray[++i] : null),
                    Payload = (string)(withPayloads ? (string)redisResultArray[++i] : default)
                };
            }

            return suggestionResult;
        }
    }
}