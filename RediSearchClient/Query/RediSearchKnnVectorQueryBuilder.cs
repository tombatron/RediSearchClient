using System;
using System.Collections.Generic;
using System.Text;

namespace RediSearchClient.Query
{
    /// <summary>
    /// Query builder for KNN type vector queries.
    /// </summary>
    public sealed class RediSearchKnnVectorQueryBuilder
    {
        private readonly string _indexName;

        internal RediSearchKnnVectorQueryBuilder(string indexName) =>
            _indexName = indexName;

        private string _prefilter = "*";

        /// <summary>
        /// Set the prefilter that will filter the available records before the actual
        /// vector search is performed.
        /// 
        /// By default, we will consider all available vectors in the index. 
        /// </summary>
        /// <param name="prefilter">The prefilter to used as a first pass. The following example assumes that your index contains a number field called `price` and a tag field called `condition`... Example: "@price:[500 1000] @condition:{new}"</param>
        public RediSearchKnnVectorQueryBuilder Prefilter(string prefilter)
        {
            _prefilter = prefilter;

            return this;
        }

        private int _numberOfNeighbors = 10;

        /// <summary>
        /// Specify the number of neighbors to return.
        /// </summary>
        /// <param name="numberOfNeighbors">How many neighbors to return? Defaults to `10`.</param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder NumberOfNeighbors(int numberOfNeighbors)
        {
            _numberOfNeighbors = numberOfNeighbors;

            return this;
        }

        private string _fieldName;

        /// <summary>
        /// The name of the field on the index that contains the vector data.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder FieldName(string fieldName)
        {
            _fieldName = fieldName;

            if (string.IsNullOrEmpty(_scoreFieldName))
            {
                _scoreFieldName = $"__{_fieldName}_score";
            }

            return this;
        }

        private string _scoreFieldName;

        /// <summary>
        /// Specify the name of the score field returned with the results.
        /// 
        /// This defaults to $"__{_fieldName}_score"
        /// </summary>
        /// <param name="scoreFieldName"></param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder ScoreFieldName(string scoreFieldName)
        {
            _scoreFieldName = scoreFieldName; 
            
            return this;
        }

        private byte[] _vector;

        /// <summary>
        /// The vector to be used as the query vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder Vector(byte[] vector)
        {
            _vector = vector;

            return this;
        }

        private int _offset = 0;
        private int _limit = 10;

        /// <summary>
        /// How many matches to return? If you specify a value of neighbors to return greater
        /// than the default of `10`, you should specify a higher limit here.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder Limit(int limit)
        {
            _limit = limit;

            return this;
        }

        /// <summary>
        /// How many matches to return after an offset?
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder Limit(int offset, int limit)
        {
            _offset = offset;
            _limit = limit;

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
        public RediSearchKnnVectorQueryBuilder Dialect(int dialect)
        {
            _dialect = dialect;

            return this;
        }

        private bool? _sortByDistanceAcending;

        /// <summary>
        /// RediSearch doesn't order the results automatically. 
        /// 
        /// Use this method if you want to sort by the computed distance of the result vector versus
        /// the query vector. 
        /// </summary>
        /// <param name="sortByDistanceAcending">`True` to get the closest matches first, `False` to get the closest matches last.</param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder SortByDistance(bool sortByDistanceAcending = true)
        {
            _sortByDistanceAcending = sortByDistanceAcending;

            return this;
        }

        private int? _efRuntime;

        /// <summary>
        /// The number of maximum top candidates to hold during the KNN search. Higher values of EF_RUNTIME will lead to a more accurate results on the expense of a longer runtime. Defaults to the EF_RUNTIME value passed on creation (which defaults to 10).
        /// 
        /// Note: Don't use this unless you've defined an HNSW index otherwise you'll probably get a `RedisServerException`.
        /// </summary>
        /// <param name="efRuntime"></param>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder EfRuntime(int efRuntime)
        {
            _efRuntime = efRuntime;

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
        public RediSearchKnnVectorQueryBuilder Epsilon(float epsilon)
        {
            _epsilon = epsilon;

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
        public RediSearchKnnVectorQueryBuilder Return(Action<ReturnFieldBuilder> returnBuilder)
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
            var parameters = new List<object>()
            {
                _indexName
            };

            // Formulate the vector query. 

            var vectorQuery = new StringBuilder();

            // Start with the prefilter...
            if (string.IsNullOrEmpty(_prefilter))
            {
                vectorQuery.Append("*=>");
            }
            else
            {
                vectorQuery.Append($"({_prefilter})=>");
            }

            // Moving on to the specification of the vector...
            vectorQuery.Append($"[KNN {_numberOfNeighbors} @{_fieldName} $BLOB]");// AS {_scoreFieldName}");

            vectorQuery.Append($"=>{{$YIELD_DISTANCE_AS: {_scoreFieldName};");

            var paramCount = 2; // We're always goign to send the BLOB parameter, but...

            if (_efRuntime.HasValue)
            {
                // Looks like we have an EF_RUNTIME parameter being passed, add that in...
                paramCount += 2;

                vectorQuery.Append(" $EF_RUNTIME: $ef_runtime;");
            }

            if(_epsilon.HasValue)
            {
                // Looks like we have an EPSILON parameter being passed, add that in too...
                paramCount += 2;

                vectorQuery.Append(" $EPSILON: $epsilon;");
            }

            vectorQuery.Append("}"); // Done...

            parameters.Add(vectorQuery.ToString());

            parameters.Add("PARAMS");
            parameters.Add(paramCount);

            parameters.Add("BLOB");
            parameters.Add(_vector);

            if (_efRuntime.HasValue)
            {
                parameters.Add("ef_runtime");
                parameters.Add(_efRuntime.Value);
            }

            if (_epsilon.HasValue)
            {
                parameters.Add("epsilon");
                parameters.Add(_epsilon.Value);
            }

            if (_sortByDistanceAcending.HasValue)
            {
                parameters.Add("SORTBY");

                // This builder doesn't give the user the ability specify the name of the distance score. By
                // default on KNN type queries the score field is `__<vector_field>_score` so we're just using
                // that.
                parameters.Add($"__{_fieldName}_score");

                parameters.Add(_sortByDistanceAcending.Value ? "ASC" : "DESC");
            }

            parameters.Add("LIMIT");
            parameters.Add(_offset);
            parameters.Add(_limit);

            // Check to see if we've specified RETURN fields...
            if (!(_returnBuilder is null))
            {
                // Looks like we have return fields, so we'll new up an instance of the
                // `ReturnFieldBuilder` class...
                var returnFieldParameters = new ReturnFieldBuilder();

                // ...pass that instance to the delegate that was defined...
                _returnBuilder(returnFieldParameters);

                // ...and append the final result to the existing lists of parameters.
                parameters.AddRange(returnFieldParameters.ReturnParameters());
            }

            parameters.Add("DIALECT");
            parameters.Add(_dialect);

            return new RediSearchQueryDefinition(parameters.ToArray());
        }
    }
}
