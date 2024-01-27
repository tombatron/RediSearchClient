using System;
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
        public string Term { get; }

        /// <summary>
        /// Suggestions for a term.
        /// </summary>
        /// <value></value>
        public Suggestion[] Suggestions { get; }

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

        internal SpellCheckResult(string term, Suggestion[] suggestions)
        {
            Term = term;
            Suggestions = suggestions;
        }

        internal static SpellCheckResult Create(RedisResult[] redisResult)
        {
            var term = (string)redisResult[1];

            var rawSuggestions = (RedisResult[])redisResult[2];

            var suggestions = new Suggestion[rawSuggestions.Length];

            for (var i = 0; i < rawSuggestions.Length; i++)
            {
                var suggestionComponents = (RedisResult[])rawSuggestions[i];

                suggestions[i] = new Suggestion
                {
                    Score = (double)suggestionComponents[0],
                    Value = (string)suggestionComponents[1]
                };
            }

            return new SpellCheckResult(term, suggestions);

        }

        internal static SpellCheckResult[] CreateArray(RedisResult redisResult)
        {
            if (redisResult.IsNull)
            {
                return Array.Empty<SpellCheckResult>();
            }
            var redisResultArray = (RedisResult[])redisResult;

            var results = new SpellCheckResult[redisResultArray.Length];

            for (var i = 0; i < redisResultArray.Length; i++)
            {
                var rr = (RedisResult[])redisResultArray[i];

                results[i] = Create(rr);
            }

            return results;
        }
    }
}