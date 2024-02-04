using System;

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

        internal override object[] GenerateArguments()
        {
            throw new NotImplementedException();
        }
    }
}