namespace RediSearchClient.Indexes
{
    internal class FlatVectorIndexAlgorithm : VectorIndexAlgorithm
    {
        private readonly VectorType _type;
        private readonly int _dimensions;
        private readonly DistanceMetric _distanceMetric;

        private readonly int? _initialCap;
        private readonly int? _blockSize;

        internal FlatVectorIndexAlgorithm(
            VectorType type,
            int dimensions,
            DistanceMetric distanceMetric,

            int? initialCap,
            int? blockSize)
        {
            _type = type;
            _dimensions = dimensions;
            _distanceMetric = distanceMetric;

            _initialCap = initialCap;
            _blockSize = blockSize;
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

            if (!(_blockSize is null)) 
            { 
                argCount += 2;
            }

            var args = new object[argCount];

            args[1] = "VECTOR";
            args[2] = "FLAT";
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

            if (!(_blockSize is null))
            {
                args[++currentIndex] = "BLOCK_SIZE";
                args[++currentIndex] = _blockSize;
            }

            return args;
        }
    }
}