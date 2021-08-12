namespace RediSearchClient.Indexes
{
    internal sealed class GeoJsonSchemaField : IRediSearchSchemaField
    {
        private readonly string _jsonPath;
        private readonly string _alias;

        public object[] FieldArguments => GenerateArguments();

        public GeoJsonSchemaField(string jsonPath, string alias)
        {
            _jsonPath = jsonPath;
            _alias = alias;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                _fieldArguments = new object[4];
                _fieldArguments[0] = _jsonPath;
                _fieldArguments[1] = "AS";
                _fieldArguments[2] = _alias;
                _fieldArguments[3] = "GEO";
            }

            return _fieldArguments;
        }
    }
}