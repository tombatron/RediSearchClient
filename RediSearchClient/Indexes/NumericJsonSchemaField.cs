namespace RediSearchClient.Indexes
{
    internal sealed class NumericJsonSchemaField : IRediSearchSchemaField
    {
        private readonly string _jsonPath;
        private readonly string _alias;
        private readonly bool _sortable;
        private readonly bool _noindex;

        public object[] FieldArguments => GenerateArguments();

        public NumericJsonSchemaField(string jsonPath, string alias, bool sortable, bool noindex)
        {
            _jsonPath = jsonPath;
            _alias = alias;
            _sortable = sortable;
            _noindex = noindex;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var argumentLength = 4 +
                                     (_sortable ? 1 : 0) +
                                     (_noindex ? 1 : 0);

                var position = 0;

                _fieldArguments = new object[argumentLength];

                _fieldArguments[position] = _jsonPath;
                _fieldArguments[++position] = "AS";
                _fieldArguments[++position] = _alias;
                _fieldArguments[++position] = "NUMERIC";

                if (_sortable)
                {
                    _fieldArguments[++position] = "SORTABLE";
                }

                if (_noindex)
                {
                    _fieldArguments[++position] = "NOINDEX";
                }
            }

            return _fieldArguments;
        }
    }
}