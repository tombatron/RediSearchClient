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
