using System;
using System.Collections.Generic;
using System.Text;

namespace RediSearchClient.Query
{
    /// <summary>
    /// Query builder for range type vector queries.
    /// </summary>
    public sealed class RediSearchRangeVectorQueryBuilder
    {
        private readonly string _indexName;

        internal RediSearchRangeVectorQueryBuilder(string indexName) =>
            _indexName = indexName;

        private string _fieldName;

        /// <summary>
        /// Name of the vector field on the specified index.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder FieldName(string fieldName)
        {
            _fieldName = fieldName.StartsWith("@") ? fieldName.Substring(1) : fieldName;

            return this;
        }

        private float _range;

        /// <summary>
        /// How far to look for matches?
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder Range(float range)
        {
            _range = range;

            return this;
        }

        private byte[] _vector;

        /// <summary>
        /// The vector to be used as the query vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder Vector(byte[] vector)
        {
            _vector = vector;

            return this;
        }

        private string _scoreFieldName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoreFieldName"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder ScoreFieldName(string scoreFieldName)
        {
            _scoreFieldName = scoreFieldName;

            return this;
        }

        private float? _epsilon;

        /// <summary>
        /// Relative factor that sets the boundaries in which a range query may search for candidates. That is, vector candidates whose distance from the query vector is radius*(1 + EPSILON) are potentially scanned, allowing more extensive search and more accurate results (on the expense of runtime). Defaults to the EPSILON value passed on creation (which defaults to 0.01).
        /// 
        /// Note: Don't use this unless you've defined an HNSW index otherwise you'll probably get a `RedisServerException`.
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder Epsilon(float epsilon)
        {
            _epsilon = epsilon;

            return this;
        }

        private int _dialect = 2;

        /// <summary>
        /// Vector queries are supported by search dialet version two and above. 
        /// 
        /// The default value here is `2`, unless you want to specify a version HIGHER than
        /// two (if that exists) you don't need to use this method. 
        /// </summary>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder Dialect(int dialect)
        {
            _dialect = dialect;

            return this;
        }

        private bool _sortByAscending;
        private string _sortByField;

        /// <summary>
        /// Specify which field you'd like to sort by. 
        /// 
        /// If you want to sort by the distance of the result vector from the query vector then you'd also
        /// want to specify the `DistanceFieldName` and then provide the same value for the `sortByField`
        /// argument here. 
        /// </summary>
        /// <param name="sortByField">Name of the field to sort by.</param>
        /// <param name="sortByAscending">If true will sort by closest match to furthest. [Default: True]</param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder SortBy(string sortByField, bool sortByAscending = true)
        {
            _sortByField = sortByField;
            _sortByAscending = sortByAscending;

            return this;
        }

        private int _offset = 0;
        private int _limit = 10;

        /// <summary>
        /// How many matches to return? 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder Limit(int limit, int offset = 0)
        {
            _offset = offset;
            _limit = limit;

            return this;
        }

        private Action<ReturnFieldBuilder> _returnBuilder;

        /// <summary>
        /// Specify the fields for your index that you watch to return. For vector queries this is important because by
        /// default the search results WILL include your vectors. For some vectors with lower dimensionality this is probably
        /// fine, but if you're working with larger vectors you might start to notice some performance issues...
        /// </summary>
        /// <param name="returnBuilder"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder Return(Action<ReturnFieldBuilder> returnBuilder)
        {
            _returnBuilder = returnBuilder;

            return this;
        }

        /// <summary>
        /// Builds the query definition.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryDefinition Build()
        {
            var parameters = new List<object>(14) // At a minimum I think the result array will have 14 items. 
            {
                _indexName
            };

            if (string.IsNullOrEmpty(_fieldName))
            {
                // TODO: Create a custom exception type...
                throw new Exception("FieldName is required.");
            }

            var vectorQuery = new StringBuilder();

            vectorQuery.Append($"@{_fieldName}:[VECTOR_RANGE $r $BLOB]");

            // Check to see if we have any runtime parameters...
            if (_epsilon.HasValue || !string.IsNullOrEmpty(_scoreFieldName))
            {
                // Looks like we have some...
                vectorQuery.Append("=>{");

                if (_epsilon.HasValue)
                {
                    vectorQuery.Append($"$EPSILON:{_epsilon.Value}; ");
                }

                if (!string.IsNullOrEmpty(_scoreFieldName))
                {
                    vectorQuery.Append($"$YIELD_DISTANCE_AS: {_scoreFieldName}");
                }

                // Close it off.
                vectorQuery.Append("}");
            }

            parameters.Add(vectorQuery.ToString());

            parameters.Add("PARAMS");
            parameters.Add(4);

            parameters.Add("r");
            parameters.Add(_range);

            parameters.Add("BLOB");
            parameters.Add(_vector);

            if (!string.IsNullOrEmpty(_sortByField))
            {
                parameters.Add("SORTBY");
                parameters.Add(_sortByField);

                if (_sortByAscending)
                {
                    parameters.Add("ASC");
                }
                else
                {
                    parameters.Add("DESC");
                }
            }

            parameters.Add("LIMIT");
            parameters.Add(_offset);
            parameters.Add(_limit);

            // Check to see if we've specified RETURN fields...
            if (!(_returnBuilder is null))
            {
                var returnFieldParameters = new ReturnFieldBuilder();

                _returnBuilder(returnFieldParameters);

                parameters.AddRange(returnFieldParameters.ReturnParameters());
            }

            parameters.Add("DIALECT");
            parameters.Add(_dialect);

            return new RediSearchQueryDefinition(parameters.ToArray());
        }
    }
}
