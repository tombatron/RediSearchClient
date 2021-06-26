namespace RediSearchClient.Indexes
{
    internal sealed class TagSchemaField : IRediSearchSchemaField
    {
        private readonly string _fieldName;
        private readonly string _separator;
        private readonly bool _sortable;
        private readonly bool _noindex;

        public object[] FieldArguments => GenerateArguments();

        public TagSchemaField(string fieldName, string separator, bool sortable, bool noindex)
        {
            _fieldName = fieldName;
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

                var argumentLength = 2 +
                    (string.IsNullOrEmpty(_separator) ? 0 : 2) +
                    (_sortable ? 1 : 0) +
                    (_noindex ? 1 : 0);

                _fieldArguments = new object[argumentLength];

                _fieldArguments[position] = _fieldName;
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