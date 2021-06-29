using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;

namespace RediSearchClient
{
    /// <summary>
    /// This class serves as an abstraction around a collection of `AggregateResultItem`'s.
    /// 
    /// The value add here is the ability to access aggreg
    /// </summary>
    public class AggregateResultCollection : IReadOnlyCollection<AggregateResultItem>
    {
        private readonly RedisResult[] _internalResult;

        internal AggregateResultCollection(RedisResult[] internalResult) =>
            _internalResult = internalResult;

        /// <summary>
        /// The count of results in the collection. 
        /// </summary>
        /// <returns></returns>
        public int Count => (_internalResult?.Length ?? 0 / 2);

        /// <summary>
        /// It's the... enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AggregateResultItem> GetEnumerator() =>
            GetItems().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetItems().GetEnumerator();

        private List<AggregateResultItem> _parsedItems;

        private IEnumerable<AggregateResultItem> GetItems()
        {
            if (_parsedItems == default)
            {
                _parsedItems = new List<AggregateResultItem>();

                if (_internalResult != default)
                {
                    for (var i = 0; i < _internalResult.Length; i++)
                    {
                        var key = (string)_internalResult[i];
                        var value = _internalResult[++i];

                        _parsedItems.Add(new AggregateResultItem(key, value));
                    }
                }
            }

            return _parsedItems;
        }

        /// <summary>
        /// Access results by index. 
        /// </summary>
        /// <returns>The RedisResult at the specified index or "default".</returns>
        public RedisResult this[int index] => GetItems().ElementAtOrDefault(index)?.Value;

        /// <summary>
        /// Access a single result by the result key. If more than one key exists for a name the
        /// first value is returned.
        /// </summary>
        /// <returns>The first RedisResult having the specified key name.</returns>
        public RedisResult this[string key] => GetItems().FirstOrDefault(x => x.Key == key)?.Value;
    }
}