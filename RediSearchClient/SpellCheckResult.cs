using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Represents a spell check result.
    /// </summary>
    public class SpellCheckResult
    {
        /// <summary>
        /// The term that the suggestion is for. 
        /// </summary>
        /// <value></value>
        public string Term { get; private set; }

        /// <summary>
        /// Suggestions for a term.
        /// </summary>
        /// <value></value>
        public Suggestion[] Suggestions { get; private set; }

        /// <summary>
        /// Represents a term's suggestion with confidence. 
        /// </summary>
        public class Suggestion
        {
            /// <summary>
            /// Score of the suggestion.
            /// </summary>
            /// <value></value>
            public double Score { get; internal set; }

            /// <summary>
            /// The uh... suggestion.
            /// </summary>
            /// <value></value>
            public string Value { get; internal set; }
        }

        internal static SpellCheckResult[] CreateArray(RedisResult redisResult)
        {
            var redisResultArray = (RedisResult[])redisResult;

            var results = new List<SpellCheckResult>();

            for (var i = 0; i < redisResultArray.Length; i++)
            {
                var result = new SpellCheckResult
                {
                    Term = (string)redisResultArray[++i]
                };

                var rawSuggestions = (RedisResult[])redisResultArray[++i];

                for (var j = 0; j < rawSuggestions.Length; j++)
                {
                    var suggestion = new Suggestion
                    {
                        Score = (double)rawSuggestions[j],
                        Value = (string)rawSuggestions[++j]
                    };
                }
            }

            return results.ToArray();
        }
    }
}