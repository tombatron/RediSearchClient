namespace RediSearchClient.Indexes
{
    internal sealed class TagJsonSchemaField : IRediSearchSchemaField
    {
        private readonly string _jsonPath;
        private readonly string _alias;
        private readonly string _separator;
        private readonly bool _sortable;
        private readonly bool _noindex;

        public object[] FieldArguments => GenerateArguments();

        public TagJsonSchemaField(string jsonPath, string alias, string separator, bool sortable, bool noindex)
        {
            _jsonPath = jsonPath;
            _alias = alias;
            _separator = separator;
            _sortable = sortable;
            _noindex = noindex;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var position = 0;

                var argumentLength = 4 +
                    (string.IsNullOrEmpty(_separator) ? 0 : 2) +
                    (_sortable ? 1 : 0) +
                    (_noindex ? 1 : 0);

                _fieldArguments = new object[argumentLength];

                _fieldArguments[position] = _jsonPath;
                _fieldArguments[++position] = "AS";
                _fieldArguments[++position] = _alias;
                _fieldArguments[++position] = "TAG";

                if (!string.IsNullOrEmpty(_separator))
                {
                    _fieldArguments[++position] = "SEPARATOR";
                    _fieldArguments[++position] = _separator;
                }

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