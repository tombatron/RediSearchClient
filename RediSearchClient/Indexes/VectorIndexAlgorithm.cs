namespace RediSearchClient.Indexes
{
    /// <summary>
    /// RediSearch supports two different vector indexing algorithms which are
    /// used when searching `k` most similar vectors in the index or filtering
    /// vectors by range.
    /// </summary>
    public abstract class VectorIndexAlgorithm
    {
        internal abstract object[] GenerateArguments(int namePadding = 1);

        /// <summary>
        /// The "FLAT" (Fast, Lightweight, and Accurate Top-k) vector index algorithm 
        /// efficiently retrieves the top-k nearest neighbors for a given query 
        /// vector in a high-dimensional space. Using a combination of data pruning 
        /// techniques and distance metrics, FLAT ensures fast and accurate similarity 
        /// search, making it suitable for applications such as recommendation 
        /// systems and content retrieval.
        /// </summary>
        /// <param name="type">FLOAT32 or FLOAT64 precision on the vector components.</param>
        /// <param name="dimensions">The number of dimensions in the vector space for indexing.</param>
        /// <param name="distanceMetric">The distance metric used to measure the similarity between vectors.</param>
        /// <param name="initialCap">[Optional] The initial capacity for the FLAT index, indicating the expected number of vectors to be indexed.</param>
        /// <param name="blockSize">[Optional] The block size used in the index structure, influencing the organization of vector data for efficient search. (Defaults to 1,024.)</param>
        /// <returns>Instance of the FLAT algorithm descriptor class.</returns>
        public static VectorIndexAlgorithm FLAT(
            VectorType type,
            int dimensions,
            DistanceMetric distanceMetric,

            int? initialCap = null,
            int? blockSize = null) =>
                new FlatVectorIndexAlgorithm(type, dimensions, distanceMetric, initialCap, blockSize);


        /// <summary>
        /// The HNSW (Hierarchical Navigable Small World) algorithm is a scalable and 
        /// efficient method for building and searching approximate nearest neighbor 
        /// graphs in high-dimensional spaces. By organizing data points in a hierarchical 
        /// structure, HNSW achieves both fast insertion and query times while maintaining 
        /// high search accuracy. This makes it well-suited for applications such as 
        /// similarity search in large datasets, recommendation systems, and clustering tasks.
        /// </summary>
        /// <param name="type">FLOAT32 or FLOAT64 precision on the vector components.</param>
        /// <param name="dimensions">The number of dimensions in the vector space for indexing.</param>
        /// <param name="distanceMetric">The distance metric used to measure the similarity between vectors.</param>
        /// <param name="initialCap">Initial vector capacity in the index affecting memory allocation size of the index.</param>
        /// <param name="m">Number of maximum allowed outgoing edges for each node in the graph in each layer. on layer zero the maximal number of outgoing edges will be 2M. Default is 16.</param>
        /// <param name="efConstruction">Number of maximum allowed potential outgoing edges candidates for each node in the graph, during the graph building. Default is 200.</param>
        /// <param name="efRuntime">Number of maximum top candidates to hold during the KNN search. Higher values of EF_RUNTIME lead to more accurate results at the expense of a longer runtime. Default is 10.</param>
        /// <param name="epsilon">Relative factor that sets the boundaries in which a range query may search for candidates. That is, vector candidates whose distance from the query vector is radius*(1 + EPSILON) are potentially scanned, allowing more extensive search and more accurate results (on the expense of runtime). Default is 0.01.</param>
        /// <returns>Instance of the HNSW algorithm descriptor class.</returns>
        public static VectorIndexAlgorithm HNSW(
            VectorType type,
            int dimensions,
            DistanceMetric distanceMetric,

            int? initialCap = null,
            int? m = null,
            int? efConstruction = null,
            int? efRuntime = null,
            float? epsilon = null) => 
                new HnswVectorIndexAlgorithm(type, dimensions, distanceMetric, initialCap, m, efConstruction, efRuntime, epsilon);
    }
}
