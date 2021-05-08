namespace RediSearchClient.Indexes
{
    internal sealed class NumericSchemaField : IRediSearchSchemaField
    {
        private readonly string _fieldName;

        public object[] FieldArguments => GenerateArguments();

        public NumericSchemaField(string fieldName)
        {
            _fieldName = fieldName;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                _fieldArguments = new object[2];

                _fieldArguments[0] = _fieldName;
                _fieldArguments[1] = "NUMERIC";
            }

            return _fieldArguments;
        }
    }
}