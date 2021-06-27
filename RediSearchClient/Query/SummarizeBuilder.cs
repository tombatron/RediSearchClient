namespace RediSearchClient.Query
{
    /// <summary>
    /// Builder class for forumating the SUMMARIZE portion of a query.
    /// </summary>
    public sealed class SummarizeBuilder
    {
        private string[] _fields;

        /// <summary>
        /// Builder method for specifying the fields to summarize.
        /// </summary>
        /// <param name="fields"></param>
        public void Fields(params string[] fields)
        {
            _fields = fields;
        }

        private int _fragments;

        /// <summary>
        /// Builder method for specifying how many fragments should be returned. (Default is 3)
        /// </summary>
        /// <param name="fragments"></param>
        public void Frags(int fragments)
        {
            _fragments = fragments;
        }

        private int _length;

        /// <summary>
        /// Builder method for specifying how many context words each fragment should contain. (Default is 20)
        /// </summary>
        /// <param name="length"></param>
        public void Length(int length)
        {
            _length = length;
        }

        private string _separator;

        /// <summary>
        /// Builder method for specifying the string used to divide between individual summary snippets. The
        /// default value is "...".
        /// </summary>
        /// <param name="separator"></param>
        public void Separator(string separator)
        {
            _separator = separator;
        }

        /// <summary>
        /// The constructed arguments from this builder.
        /// </summary>
        /// <returns></returns>
        public object[] FieldArguments => GenerateArguments();

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var argumentLength = 1; // SUMMARIZE
                argumentLength += 2 + _fields.Length; // FIELDS
                argumentLength += _fragments > 0 ? 2 : 0; // [FRAGS {num}]
                argumentLength += _length > 0 ? 2 : 0; // [LEN {fragsize}]
                argumentLength += string.IsNullOrEmpty(_separator) ? 0 : 2; // [SEPARATOR {separator}]

                var result = new object[argumentLength];

                var currentArgumentIndex = 0;

                result[currentArgumentIndex] = "SUMMARIZE";
                result[++currentArgumentIndex] = "FIELDS";
                result[++currentArgumentIndex] = _fields.Length.ToString();

                foreach (var field in _fields)
                {
                    result[++currentArgumentIndex] = field;
                }

                if (_fragments > 0)
                {
                    result[++currentArgumentIndex] = "FRAGS";
                    result[++currentArgumentIndex] = _fragments.ToString();
                }

                if (_length > 0)
                {
                    result[++currentArgumentIndex] = "LEN";
                    result[++currentArgumentIndex] = _length.ToString();
                }

                if (!string.IsNullOrEmpty(_separator))
                {
                    result[++currentArgumentIndex] = "SEPARATOR";
                    result[++currentArgumentIndex] = _separator;
                }

                _fieldArguments = result;
            }

            return _fieldArguments;
        }
    }
}