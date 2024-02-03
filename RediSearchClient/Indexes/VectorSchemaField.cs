namespace RediSearchClient.Indexes
{
    internal sealed class VectorSchemaField : IRediSearchSchemaField
    {
        private readonly string _name;
        private readonly VectorIndexAlgorithm _vectorIndexAlgorithm;

        public object[] FieldArguments => GenerateArguments();

        public VectorSchemaField(
            string name, 
            VectorIndexAlgorithm vectorIndexAlgorithm)
        {
            _name = name;
            _vectorIndexAlgorithm = vectorIndexAlgorithm;
        }

        private object[] GenerateArguments()
        {
            return new object[] { 0 };
        }
    }
}