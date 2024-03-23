namespace RediSearchClient.Indexes
{
    internal class HnswVectorIndexAlgorithm : VectorIndexAlgorithm
    {
        private readonly VectorType _type;
        private readonly int _dimensions;
        private readonly DistanceMetric _distanceMetric;

        private readonly int? _initialCap;
        private readonly int? _m;
        private readonly int? _efConstruction;
        private readonly int? _efRuntime;
        private readonly float? _epsilon;

        internal HnswVectorIndexAlgorithm(
            VectorType type,
            int dimensions,
            DistanceMetric distanceMetric,

            int? initialCap,
            int? m,
            int? efConstruction,
            int? efRuntime,
            float? epsilon)
        {
            _type = type;
            _dimensions = dimensions;
            _distanceMetric = distanceMetric;

            _initialCap = initialCap;
            _m = m;
            _efConstruction = efConstruction;
            _efRuntime = efRuntime;
            _epsilon = epsilon;
        }

        /// <summary>
        /// This method will return an `object[]` containing the bulk of the vector index
        /// specification. 
        /// 
        /// The 0th item in the array is null to allow for a space to put the field name.
        /// </summary>
        /// <returns></returns>
        internal override object[] GenerateArguments(int namePadding = 1)
        {
            var argCount = 6;
            var resultLength = 9 + namePadding;

            if (!(_initialCap is null))
            {
                argCount += 2;
                resultLength += 2;
            }

            if(!(_m is null))
            {
                argCount += 2;
                resultLength += 2;
            }

            if(!(_efConstruction is null))
            {
                argCount += 2;
                resultLength += 2;
            }

            if(!(_efRuntime is null))
            {
                argCount += 2;
                resultLength += 2;
            }

            if(!(_epsilon is null))
            {
                argCount += 2;
                resultLength += 2;
            }

            var args = new object[resultLength];

            var currentIndex = (namePadding - 1);

            args[++currentIndex] = "VECTOR";
            args[++currentIndex] = "HNSW";
            args[++currentIndex] = argCount;

            args[++currentIndex] = "TYPE";
            args[++currentIndex] = _type.ToString();

            args[++currentIndex] = "DIM";
            args[++currentIndex] = _dimensions;

            args[++currentIndex] = "DISTANCE_METRIC";
            args[++currentIndex] = _distanceMetric.ToString();

            if (!(_initialCap is null))
            {
                args[++currentIndex] = "INITIAL_CAP";
                args[++currentIndex] = _initialCap;
            }

            if (!(_m is null))
            {
                args[++currentIndex] = "M";
                args[++currentIndex] = _m;
            }

            if (!(_efConstruction is null))
            {
                args[++currentIndex] = "EF_CONSTRUCTION";
                args[++currentIndex] = _efConstruction;
            }

            if (!(_efRuntime is null))
            {
                args[++currentIndex] = "EF_RUNTIME";
                args[++currentIndex] = _efRuntime;
            }

            if (!(_epsilon is null))
            {
                args[++currentIndex] = "EPSILON";
                args[++currentIndex] = _epsilon;
            }

            return args;
        }
    }
}