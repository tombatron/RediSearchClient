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
            _fieldName = fieldName;

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

        private string _distanceFieldName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distanceFieldName"></param>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder DistanceFieldName(string distanceFieldName)
        {
            _distanceFieldName = distanceFieldName;

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

        /// <summary>
        /// Builds the query definition.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryDefinition Build()
        {
            return default;
        }
    }
}
