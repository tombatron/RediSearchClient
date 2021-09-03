using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a result from the `FT.AGGREGATE` command.
    /// </summary>
    public class AggregateResult : IEnumerable<AggregateResultCollection>
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

        /// <summary>
        /// Get the result enumerator for the `AggregateResult`.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AggregateResultCollection> GetEnumerator() =>
            ResultProcessor().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ResultProcessor().GetEnumerator();

        private IEnumerable<AggregateResultCollection> ResultProcessor()
        {
            if (RawResult != default && RecordCount > 0)
            {
                for (var i = 1; i < RawResult.Length; i++)
                {
                    var recordFields = (RedisResult[])RawResult[i];

                    yield return new AggregateResultCollection(recordFields);
                }
            }
        }

        /// <summary>
        /// Convenience method for mapping search results to a collection of local types.
        /// </summary>
        /// <typeparam name="TMapped">Destination type that we're mapping the collection of results to.</typeparam>
        /// <returns></returns>
        public IEnumerable<TMapped> As<TMapped>() where TMapped : new() =>
            ResultMapper<TMapped>.MapTo(this);
    }
}