namespace RediSearchClient.Indexes
{
    public abstract class VectorIndexAlgorithm
    {
        internal abstract object[] GenerateArguments();

        public static VectorIndexAlgorithm FLAT(
            VectorType type,
            int dimensions,
            DistanceMetric distanceMetric,

            int? initialCap = null,
            int? blockSize = null)
        {
            return default;
        }

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
