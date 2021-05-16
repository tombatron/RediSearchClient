namespace RediSearchClient.Query
{
    public sealed class HighlightBuilder
    {
        private string[] _fields;

        public void Fields(params string[] fields)
        {
            _fields = fields;
        }

        private string _open;
        private string _close;

        public void Tags(string open, string close)
        {
            _open = open;
            _close = close;
        }

        public object[] FieldArguments => GenerateArguments();

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var argumentLength = 1; // HIGHLIGHT
                argumentLength += 2 + _fields.Length; // FIELDS

                if (!string.IsNullOrEmpty(_open) && !string.IsNullOrEmpty(_close))
                {
                    argumentLength += 3; // TAGS {open} {close}
                }

                var result = new object[argumentLength];

                var currentArgumentIndex = 0;

                result[currentArgumentIndex] = "HIGHLIGHT";
                result[++currentArgumentIndex] = "FIELDS";
                result[++currentArgumentIndex] = _fields.Length.ToString();

                foreach (var field in _fields)
                {
                    result[++currentArgumentIndex] = field;
                }

                if (!string.IsNullOrEmpty(_open) && !string.IsNullOrEmpty(_close))
                {
                    result[++currentArgumentIndex] = "TAGS";
                    result[++currentArgumentIndex] = _open;
                    result[++currentArgumentIndex] = _close;
                }

                _fieldArguments = result;
            }

            return _fieldArguments;
        }
    }
}