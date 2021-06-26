namespace RediSearchClient.Indexes
{
    internal sealed class NumericSchemaField : IRediSearchSchemaField
    {
        private readonly string _fieldName;
        private readonly bool _sortable;
        private readonly bool _noindex;

        public object[] FieldArguments => GenerateArguments();

        public NumericSchemaField(string fieldName, bool sortable, bool noindex)
        {
            _fieldName = fieldName;
            _sortable = sortable;
            _noindex = noindex;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var argumentLength = 2 +
                    (_sortable ? 1 : 0) +
                    (_noindex ? 1 : 0);

                var position = 0;

                _fieldArguments = new object[argumentLength];

                _fieldArguments[position] = _fieldName;
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