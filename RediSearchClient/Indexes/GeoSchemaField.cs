namespace RediSearchClient.Indexes
{
    internal sealed class GeoSchemaField : IRediSearchSchemaField
    {
        private readonly string _fieldName;

        public object[] FieldArguments => GenerateArguments();

        public GeoSchemaField(string fieldName)
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
                _fieldArguments[1] = "GEO";
            }

            return _fieldArguments;
        }
    }
}