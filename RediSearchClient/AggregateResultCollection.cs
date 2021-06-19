using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;

namespace RediSearchClient
{
    public class AggregateResultCollection : IReadOnlyCollection<AggregateResultItem>
    {
        private readonly RedisResult[] _internalResult;

        internal AggregateResultCollection(RedisResult[] internalResult) =>
            _internalResult = internalResult;

        public int Count => (_internalResult?.Length ?? 0 / 2);

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

                for (var i = 0; i < _internalResult.Length; i++)
                {
                    var key = (string)_internalResult[i];
                    var value = _internalResult[++i];

                    _parsedItems.Add(new AggregateResultItem(key, value));
                }
            }

            return _parsedItems;
        }

        public RedisResult this[int index] => GetItems().ElementAtOrDefault(index)?.Value;

        public RedisResult this[string key] => GetItems().FirstOrDefault(x => x.Key == key)?.Value;
    }
}