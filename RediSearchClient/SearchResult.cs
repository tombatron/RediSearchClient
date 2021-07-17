using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="mappers"></param>
        /// <typeparam name="TMapped">Destination type that we're mapping the collection of results to.</typeparam>
        /// <returns></returns>
        public IEnumerable<TMapped> As<TMapped>(
            params (string sourceFieldName, string destinationPropertyName, Func<RedisResult, object> converter)[] mappers
        ) where TMapped : new()
        {
            foreach (var r in this)
            {
                var mappedResult = new TMapped();

                foreach (var p in mappedResult.GetType().GetProperties())
                {
                    if (mappers?.Any(x => x.destinationPropertyName == p.Name) ?? false)
                    {
                        var mapper = mappers.First(x => x.destinationPropertyName == p.Name);

                        p.SetValue(mappedResult, mapper.converter(r[mapper.sourceFieldName]));

                        continue;
                    }
                    else
                    {
                        var mappedValue = r[p.Name];

                        switch (p.GetValue(default))
                        {
                            case string v:
                                p.SetValue(mappedResult, (string)mappedValue);
                                break;
                            case int v:
                                p.SetValue(mappedResult, (int)mappedValue);
                                break;
                            default:
                                break;
                        }
                    }
                }

                yield return mappedResult;
            }

            yield break;
        }
    }
}