using System.Collections.Generic;
using RediSearchClient.Query;
using System.Linq;

namespace RediSearchClient.Aggregate
{
    /// <summary>
    /// This is the builder class used to construct a `SORTBY` clause for an aggregation query.
    /// </summary>
    public sealed class SortByBuilder
    {
        internal SortByBuilder()
        {

        }

        private List<(string fieldName, Direction direction)> _fields =
            new List<(string fieldName, Direction direction)>();

        /// <summary>
        /// Builder method for specifying the field name and direction for sorting.
        /// </summary>
        /// <param name="fieldName">The name of the field to sort on.</param>
        /// <param name="direction">The direction of the sort.</param>
        public void Field(string fieldName, Direction direction)
        {
            _fields.Add((fieldName, direction));
        }

        private int? _max = default;

        /// <summary>
        /// Builder method for specifying the max amount of records to sort.
        /// </summary>
        /// <param name="max">Number of records to sort.</param>
        public void Max(int max)
        {
            _max = max;
        }

        internal object[] Build()
        {
            var result = new List<object>();

            if (_fields.Any())
            {
                result.Add("SORTBY");
                result.Add(_fields.Count * 2);

                foreach (var (field, direction) in _fields)
                {
                    result.Add(field);
                    result.Add(direction == Direction.Ascending ? "ASC" : "DESC");
                }

                if (_max.HasValue)
                {
                    result.Add("MAX");
                    result.Add(_max.Value);
                }
            }

            return result.ToArray();
        }
    }
}