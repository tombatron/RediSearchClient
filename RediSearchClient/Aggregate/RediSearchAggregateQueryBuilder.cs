using System;
using System.Collections.Generic;
using System.Linq;
using RediSearchClient.Query;

namespace RediSearchClient.Aggregate
{
    public sealed class RediSearchAggregateQueryBuilder
    {
        private Queue<Func<object[]>> _queryComponents;

        internal RediSearchAggregateQueryBuilder(string indexName)
        {
            _queryComponents = new Queue<Func<object[]>>();
            _queryComponents.Enqueue(() => new[] { indexName });
        }

        public RediSearchAggregateQueryBuilder Query(string query)
        {
            _queryComponents.Enqueue(() => new[] { query });

            return this;
        }

        public RediSearchAggregateQueryBuilder Verbatim()
        {
            _queryComponents.Enqueue(() => new[] { "VERBATIM" });

            return this;
        }

        public RediSearchAggregateQueryBuilder Load(params object[] loadProperties)
        {
            _queryComponents.Enqueue(() =>
            {
                var result = new List<object>();

                result.Add("LOAD");
                result.Add(loadProperties.Length);
                result.AddRange(loadProperties);

                return result.ToArray();
            });

            return this;
        }

        private static readonly GroupByBuilder _groupByBuilder = new GroupByBuilder();

        public RediSearchAggregateQueryBuilder GroupBy(Action<GroupByBuilder> groupBy)
        {
            _queryComponents.Enqueue(() =>
            {
                var builder = new GroupByBuilder();

                groupBy(builder);

                return builder.Build();
            });

            return this;
        }

        public RediSearchAggregateQueryBuilder SortBy(string field, Direction direction)
        {
            _queryComponents.Enqueue(() =>
            {
                var result = new object[3];

                result[0] = "SORTBY";
                result[1] = field;
                result[2] = direction == Direction.Ascending ? "ASC" : "DESC";

                return result;
            });
            return this;
        }

        public RediSearchAggregateQueryBuilder Max(int maxRecords)
        {
            _queryComponents.Enqueue(() =>
            {
                return new object[]
                {
                "MAX",
                maxRecords
                };
            });

            return this;
        }

        public RediSearchAggregateQueryBuilder Apply(string expression, string alias)
        {
            _queryComponents.Enqueue(() =>
            {
                return new object[]
                {
                    "APPLY",
                    expression,
                    "AS",
                    alias
                };
            });

            return this;
        }

        public RediSearchAggregateQueryBuilder Limit(int offset, int limit)
        {
            _queryComponents.Enqueue(() =>
            {
                return new object[]
                {
                "LIMIT",
                offset,
                limit
                };
            });
            return this;
        }

        public RediSearchAggregateQueryBuilder Filter(string expression)
        {
            _queryComponents.Enqueue(() =>
            {
                return new object[]
                {
                "FILTER",
                expression
                };
            });

            return this;
        }

        public RediSearchAggregateDefinition Build() =>
            new RediSearchAggregateDefinition(_queryComponents.SelectMany(x => x()).ToArray());
    }
}