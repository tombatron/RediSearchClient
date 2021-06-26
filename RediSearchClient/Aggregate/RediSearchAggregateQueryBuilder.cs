using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Builder method for applying (APPLY) a transformation to results.
        /// </summary>
        /// <param name="expression">The transformation to apply.</param>
        /// <param name="alias">The alias of the transformation.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builder method for limiting (LIMIT) the number of results.
        /// </summary>
        /// <param name="offset">Result offset (0 based) to start limiting the result at.</param>
        /// <param name="limit">The number of records to limit the result to.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builder method for filtering (FILTER) the results.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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

        /// <summary>
        /// The end of the line. Invoking this method will process all of the fields for the aggregation query.
        /// </summary>
        /// <returns>An aggregate definition object to be passed to RediSearch.</returns>
        public RediSearchAggregateDefinition Build() =>
            new RediSearchAggregateDefinition(_queryComponents.SelectMany(x => x()).ToArray());
    }
}