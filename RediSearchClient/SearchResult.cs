using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

namespace RediSearchClient
{
    public class SearchResult : IEnumerable<SearchResultItem>
    {
        public RedisResult[] RawResult { get; }

        private int _recordCount = -1;

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
    }
}