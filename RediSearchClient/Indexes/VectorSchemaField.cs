namespace RediSearchClient.Indexes
{
    internal sealed class VectorSchemaField : IRediSearchSchemaField
    {
        private readonly string _name;
        private readonly int _dimensions;
        private readonly DistanceMetric _distanceMetric;
        private readonly VectorType _vectorType;

        public object[] FieldArguments => GenerateArguments();

        public VectorSchemaField(string name, int dimensions, DistanceMetric distanceMetric, VectorType vectorType)
        {
            _name = name;
            _dimensions = dimensions;
            _distanceMetric = distanceMetric;
            _vectorType = vectorType;
        }

        private object[] GenerateArguments()
        {
            return new object[] { 0 };
        }
    }
}