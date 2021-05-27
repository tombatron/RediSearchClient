namespace RediSearchClient.Indexes
{
    internal sealed class TagSchemaField : IRediSearchSchemaField
    {
        private readonly string _fieldName;
        private readonly string _separator;

        public object[] FieldArguments => GenerateArguments();

        public TagSchemaField(string fieldName, string separator)
        {
            _fieldName = fieldName;
            _separator = separator;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var argumentLength = 2 + (string.IsNullOrEmpty(_separator) ? 0 : 2);

                _fieldArguments = new object[argumentLength];

                _fieldArguments[0] = _fieldName;
                _fieldArguments[1] = "TAG";

                if (argumentLength == 4)
                {
                    _fieldArguments[2] = "SEPARATOR";
                    _fieldArguments[3] = _separator;
                }
            }

            return _fieldArguments;
        }
    }
}