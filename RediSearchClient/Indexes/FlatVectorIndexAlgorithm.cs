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
        internal override object[] GenerateArguments(int namePadding = 1)
        {
            var argCount = 6;
            var resultLength = 9 + namePadding;

            if (!(_initialCap is null))
            {
                argCount += 2;
                resultLength += 2;
            }

            if (!(_blockSize is null)) 
            { 
                argCount += 2;
                resultLength += 2;
            }

            var args = new object[resultLength];

            var currentIndex = (namePadding - 1);

            args[++currentIndex] = "VECTOR";
            args[++currentIndex] = "FLAT";
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

            if (!(_blockSize is null))
            {
                args[++currentIndex] = "BLOCK_SIZE";
                args[++currentIndex] = _blockSize;
            }

            return args;
        }
    }
}