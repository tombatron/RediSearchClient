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

            foreach (RedisResult[] rr in redisResultArray)
            {
                for (var i = 0; i < rr.Length; i++)
                {
                    var result = new SpellCheckResult
                    {
                        Term = (string)rr[++i]
                    };

                    var rawSuggestions = (RedisResult[])rr[++i];

                    for (var j = 0; j < rawSuggestions.Length; j++)
                    {
                        var suggestionComponents = (RedisResult[])rawSuggestions[j];

                        var suggestion = new Suggestion
                        {
                            Score = (double)suggestionComponents[0],
                            Value = (string)suggestionComponents[1]
                        };
                    }

                    results.Add(result);
                }
            }

            return results.ToArray();
        }
    }
}