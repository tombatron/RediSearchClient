using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a result from the `FT.AGGREGATE` command.
    /// </summary>
    public class AggregateResult
    {
        /// <summary>
        /// The unparsed value returned from Redis.
        /// </summary>
        /// <value></value>
        public RedisResult[] RawResult { get; }

        private int _recordCount = -1;

        /// <summary>
        /// The number of search results contained within the collection.
        /// </summary>
        /// <value></value>
        public int RecordCount
        {
            get
            {
                if (_recordCount == -1)
                {
                    _recordCount = (int)RawResult[0];
                }

                return _recordCount;
            }
        }

        private Dictionary<string, RedisResult>[] _records;

        /// <summary>
        /// The parsed response from the `FT.AGGREGATE` command. 
        /// </summary>
        /// <value></value>
        public Dictionary<string, RedisResult>[] Records
        {
            get
            {
                if (_records == default)
                {
                    _records = new Dictionary<string, RedisResult>[RecordCount];

                    for (var i = 1; i < RawResult.Length; i++)
                    {
                        _records[i - 1] = new Dictionary<string, RedisResult>();

                        var recordFields = (RedisResult[])RawResult[i];

                        for (var j = 0; j < recordFields.Length; j++)
                        {
                            var key = (string)recordFields[j];
                            var value = recordFields[++j];

                            _records[i - 1].Add(key, value);
                        }
                    }
                }

                return _records;
            }
        }

        private AggregateResult(RedisResult rawResult) =>
            RawResult = (RedisResult[])rawResult;

        internal static AggregateResult From(RedisResult redisResult) =>
            new AggregateResult(redisResult);
    }
}