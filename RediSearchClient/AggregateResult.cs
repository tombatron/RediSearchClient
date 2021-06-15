using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a result from the `FT.AGGREGATE` command.
    /// </summary>
    public class AggregateResult : IEnumerable<KeyValuePair<string, RedisResult>[]>
    {
        /// <summary>
        /// The unparsed value returned from Redis.
        /// </summary>
        /// <value></value>
        public RedisResult[] RawResult { get; }

        /// <summary>
        /// The number of search results contained within the collection.
        /// </summary>
        /// <value></value>
        public int RecordCount => (int)RawResult[0];

        private AggregateResult(RedisResult rawResult) =>
            RawResult = (RedisResult[])rawResult;

        internal static AggregateResult From(RedisResult redisResult) =>
            new AggregateResult(redisResult);

        public IEnumerator<KeyValuePair<string, RedisResult>[]> GetEnumerator() =>
            ResultProcessor().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ResultProcessor().GetEnumerator();

        private IEnumerable<KeyValuePair<string, RedisResult>[]> ResultProcessor()
        {
            if (RawResult != default && RecordCount > 0)
            {
                for (var i = 1; i < RawResult.Length; i++)
                {
                    var aggregation = new List<KeyValuePair<string, RedisResult>>();

                    var recordFields = (RedisResult[])RawResult[i];

                    for (var j = 0; j < recordFields.Length; j++)
                    {
                        var key = (string)recordFields[j];
                        var value = recordFields[++j];

                        aggregation.Add(new KeyValuePair<string, RedisResult>(key, value));
                    }

                    yield return aggregation.ToArray();
                }
            }

            yield break;
        }
    }
}