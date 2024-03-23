namespace RediSearchClient.Indexes
{
    internal sealed class VectorSchemaField : IRediSearchSchemaField
    {
        private readonly string _name;
        private readonly string _alias;
        private readonly VectorIndexAlgorithm _vectorIndexAlgorithm;

        public object[] FieldArguments => GenerateArguments();

        public VectorSchemaField(
            string name,
            VectorIndexAlgorithm vectorIndexAlgorithm) : this(name, null, vectorIndexAlgorithm) { }

        public VectorSchemaField(string name, string alias, VectorIndexAlgorithm vectorIndexAlgorithm)
        {
            _name = name;
            _alias = alias;
            _vectorIndexAlgorithm = vectorIndexAlgorithm;
        }

        private object[] GenerateArguments()
        {

            bool hasAlias = !string.IsNullOrEmpty(_alias);
            int namePadding = hasAlias ? 3 : 1;

            var vectorIndexSpecification = _vectorIndexAlgorithm.GenerateArguments(namePadding: namePadding);

            vectorIndexSpecification[0] = _name;

            if (hasAlias)
            {
                vectorIndexSpecification[1] = "AS";
                vectorIndexSpecification[2] = _alias;
            }

            return vectorIndexSpecification;
        }
    }
}