using System;
using System.Collections.Generic;
using System.Linq;
using RediSearchClient.Query;

namespace RediSearchClient.Aggregate
{
    /// <summary>
    /// This is the builder wherein a majority of an aggreagtion query is defined.
    /// </summary>
    public sealed class RediSearchAggregateQueryBuilder
    {
        private Queue<Func<object[]>> _queryComponents;

        internal RediSearchAggregateQueryBuilder(string indexName)
        {
            _queryComponents = new Queue<Func<object[]>>();
            _queryComponents.Enqueue(() => new[] { indexName });
        }

        /// <summary>
        /// Builder method for specifying the initial filtering query.
        /// </summary>
        /// <param name="query">Query that follows the exact syntax as a standard search query.</param>
        /// <returns></returns>
        public RediSearchAggregateQueryBuilder Query(string query)
        {
            _queryComponents.Enqueue(() => new[] { query });

            return this;
        }

        /// <summary>
        /// Invoking this method will set the query so that no stemming is used for query expansion.
        /// </summary>
        /// <returns></returns>
        public RediSearchAggregateQueryBuilder Verbatim()
        {
            _queryComponents.Enqueue(() => new[] { "VERBATIM" });

            return this;
        }

        /// <summary>
        /// Builder method for specifying which fields to load from the underlying hash objects.
        /// 
        /// In general you should avoid using this. Instead you should try and only work with fields that are indexed as
        /// "sortable" as those queries will be executed with much less overhead.
        /// </summary>
        /// <param name="loadProperties">The field(s) to be loaded from the indexed hash objects.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builder method for specifying the grouping operation on the results pipeline. 
        /// </summary>
        /// <param name="groupBy">The GROUPBY is specified by a builder.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builder method that provides access to the "SORTBY" builder.
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public RediSearchAggregateQueryBuilder SortBy(Action<SortByBuilder> sortBy)
        {
            _queryComponents.Enqueue(() =>
            {

                var builder = new SortByBuilder();

                sortBy(builder);

                return builder.Build();
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