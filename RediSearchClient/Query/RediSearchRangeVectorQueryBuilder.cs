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
