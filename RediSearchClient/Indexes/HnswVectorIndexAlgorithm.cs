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
        internal override object[] GenerateArguments()
        {
            var argCount = 10;

            if (!(_initialCap is null))
            {
                argCount += 2;
            }

            if(!(_m is null))
            {
                argCount += 2;
            }

            if(!(_efConstruction is null))
            {
                argCount += 2;
            }

            if(!(_efRuntime is null))
            {
                argCount += 2;
            }

            if(!(_epsilon is null))
            {
                argCount += 2;
            }

            var args = new object[argCount];

            args[1] = "VECTOR";
            args[2] = "HNSW";
            args[3] = (argCount - 4);

            args[4] = "TYPE";
            args[5] = _type.ToString();

            args[6] = "DIM";
            args[7] = _dimensions;

            args[8] = "DISTANCE_METRIC";
            args[9] = _distanceMetric.ToString();

            var currentIndex = 9;

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