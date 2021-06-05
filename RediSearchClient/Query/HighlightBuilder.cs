namespace RediSearchClient.Query
{
    /// <summary>
    /// A builder that aids in configuring how search results should be highlighted.
    /// </summary>
    public sealed class HighlightBuilder
    {
        private string[] _fields;

        /// <summary>
        /// Which fields are we to highlight?
        /// </summary>
        /// <param name="fields"></param>
        public void Fields(params string[] fields)
        {
            _fields = fields;
        }

        private string _open;
        private string _close;

        /// <summary>
        /// How should the highlighted fields be... highlighted?
        /// </summary>
        /// <param name="open">This will be prefixed on a matched highlighted value.</param>
        /// <param name="close">This will be suffixed on a matched highlighted value.</param>
        public void Tags(string open, string close)
        {
            _open = open;
            _close = close;
        }

        internal object[] FieldArguments => GenerateArguments();

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