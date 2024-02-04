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

        /// <summary>
        /// This method will return an `object[]` containing the bulk of the vector index
        /// specification.
        /// 
        /// The 0th item in the array is null to allow for a space to put the field name.
        /// </summary>
        /// <returns></returns>
        internal override object[] GenerateArguments()
        {
            throw new NotImplementedException();
        }
    }
}