namespace RediSearchClient.Indexes
{
    internal sealed class TextSchemaField : IRediSearchSchemaField
    {
        private readonly string _fieldName;
        private readonly bool _sortable;
        private readonly bool _nostem;
        private readonly bool _noindex;
        private readonly Language _phonetic;
        private readonly double _weight;

        public object[] FieldArguments => GenerateArguments();

        public TextSchemaField(string fieldName, bool sortable, bool nostem, bool noindex, Language phonetic, double weight)
        {
            _fieldName = fieldName;
            _sortable = sortable;
            _nostem = nostem;
            _noindex = noindex;
            _phonetic = phonetic;
            _weight = weight;
        }

        private object[] _fieldArguments;

        private object[] GenerateArguments()
        {
            if (_fieldArguments == null)
            {
                var argumentLength = 2;
                argumentLength += _sortable ? 1 : 0;
                argumentLength += _nostem ? 1 : 0;
                argumentLength += _noindex ? 1 : 0;
                argumentLength += _phonetic == Language.None ? 0 : 2;
                argumentLength += _weight == 1 ? 0 : 2;

                _fieldArguments = new object[argumentLength];

                var currentArgumentIndex = 0;

                _fieldArguments[currentArgumentIndex] = _fieldName;
                _fieldArguments[++currentArgumentIndex] = "TEXT";

                if (_nostem)
                {
                    _fieldArguments[++currentArgumentIndex] = "NOSTEM";
                }

                if (_weight != 1)
                {
                    _fieldArguments[++currentArgumentIndex] = "WEIGHT";
                    _fieldArguments[++currentArgumentIndex] = _weight;
                }

                if (_phonetic != Language.None)
                {
                    _fieldArguments[++currentArgumentIndex] = "PHONETIC";
                    _fieldArguments[++currentArgumentIndex] = GetMatcherForLanguage(_phonetic);
                }

                if (_sortable)
                {
                    _fieldArguments[++currentArgumentIndex] = "SORTABLE";
                }

                if (_noindex)
                {
                    _fieldArguments[++currentArgumentIndex] = "NOINDEX";
                }

            }

            return _fieldArguments;
        }

        private const string DoubleMetaphoneForEnglish = "dm:en";
        private const string DoubleMetaphoneForFrench = "dm:fr";
        private const string DoubleMetaphoneForPortuguese = "dm:pt";
        private const string DoubleMetaphoneForSpanish = "dm:es";

        private static string GetMatcherForLanguage(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return DoubleMetaphoneForEnglish;
                case Language.French:
                    return DoubleMetaphoneForFrench;
                case Language.Portuguese:
                    return DoubleMetaphoneForPortuguese;
                case Language.Spanish:
                    return DoubleMetaphoneForSpanish;
                default:
                    return DoubleMetaphoneForEnglish;
            }
        }
    }
}