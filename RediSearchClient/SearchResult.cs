using System;
using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a result from the `FT.SEARCH` command. 
    /// </summary>
    public class SearchResult : IEnumerable<SearchResultItem>
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

        private SearchResult(RedisResult rawResult) =>
            RawResult = (RedisResult[])rawResult;

        internal static SearchResult From(RedisResult redisResult) =>
            new SearchResult(redisResult);

        /// <summary>
        /// Allows for iterating over this enumerable. 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SearchResultItem> GetEnumerator() =>
            ResultProcessor().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ResultProcessor().GetEnumerator();

        private IEnumerable<SearchResultItem> ResultProcessor()
        {
            for (var i = 1; i < RawResult.Length; i++)
            {
                var documentKey = (string)RawResult[i];
                var fields = (RedisResult[])RawResult[++i];

                yield return SearchResultItem.Create(documentKey, fields);
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