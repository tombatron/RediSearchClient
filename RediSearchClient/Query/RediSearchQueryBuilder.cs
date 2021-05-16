using System;
using System.Linq;

namespace RediSearchClient.Query
{
    public sealed class RediSearchQueryBuilder
    {
        private readonly string _indexName;

        internal RediSearchQueryBuilder(string indexName) =>
            _indexName = indexName;

        private string _query;

        public RediSearchQueryBuilder UsingQuery(string query)
        {
            _query = query;

            return this;
        }

        private Func<RediSearchNumericFilterBuilder, IRediSearchNumericFilter>[] _numericFilters;

        public RediSearchQueryBuilder WithNumericFilters(params Func<RediSearchNumericFilterBuilder, IRediSearchNumericFilter>[] numericFilters)
        {
            _numericFilters = numericFilters;

            return this;
        }

        private GeoFilterDefinition _geoFilterDefinition;

        public RediSearchQueryBuilder WithGeoFilter(string fieldName, double latitude, double longitude, double radius, Unit unit)
        {
            _geoFilterDefinition = new GeoFilterDefinition(fieldName, latitude, longitude, radius, unit);

            return this;
        }

        private string[] _inKeys;

        public RediSearchQueryBuilder InKeys(params string[] inKeys)
        {
            _inKeys = inKeys;

            return this;
        }

        private string[] _inFields;

        public RediSearchQueryBuilder InFields(params string[] inFields)
        {
            _inFields = inFields;

            return this;
        }

        private string[] _returnFields;

        public RediSearchQueryBuilder Return(params string[] returnFields)
        {
            _returnFields = returnFields;

            return this;
        }

        private Action<SummarizeBuilder> _summarizeBuilderAction;

        public RediSearchQueryBuilder Summarize(Action<SummarizeBuilder> summarizeBuilder)
        {
            _summarizeBuilderAction = summarizeBuilder;

            return this;
        }

        private Action<HighlightBuilder> _highlightBuilderAction;

        public RediSearchQueryBuilder Highlight(Action<HighlightBuilder> highlightBuilder)
        {
            _highlightBuilderAction = highlightBuilder;

            return this;
        }

        private int _slop;

        public RediSearchQueryBuilder Slop(int slop)
        {
            _slop = slop;

            return this;
        }

        private bool _inOrder;

        public RediSearchQueryBuilder InOrder()
        {
            _inOrder = true;

            return this;
        }

        private string _language;

        public RediSearchQueryBuilder Language(SearchLanguage language)
        {
            _language = language;

            return this;
        }

        private string _expander;

        public RediSearchQueryBuilder Expander(string expander)
        {
            _expander = expander;

            return this;
        }

        private string _scorer;

        public RediSearchQueryBuilder Scorer(string scorer)
        {
            _scorer = scorer;

            return this;
        }

        private string _payload;

        public RediSearchQueryBuilder Payload(string payload)
        {
            _payload = payload;

            return this;
        }

        private (string fieldName, Direction direction) _sortBy;

        public RediSearchQueryBuilder SortBy(string fieldName, Direction direction)
        {
            _sortBy = (fieldName, direction);

            return this;
        }

        private (int offset, int limit) _limit;

        public RediSearchQueryBuilder Limit(int offset, int limit)
        {
            _limit = (offset, limit);

            return this;
        }

        private bool _noContent;

        public RediSearchQueryBuilder NoContent()
        {
            _noContent = true;

            return this;
        }

        private bool _verbatim;

        public RediSearchQueryBuilder Verbatim()
        {
            _verbatim = true;

            return this;
        }

        private bool _noStopWords;

        public RediSearchQueryBuilder NoStopWords()
        {
            _noStopWords = true;

            return this;
        }

        private bool _withScores;

        public RediSearchQueryBuilder WithScores()
        {
            _withScores = true;

            return this;
        }

        private bool _withSortKeys;

        public RediSearchQueryBuilder WithSortKeys()
        {
            _withSortKeys = true;

            return this;
        }

        private bool _withPayloads;

        public RediSearchQueryBuilder WithPayloads()
        {
            _withPayloads = true;

            return this;
        }

        private static readonly RediSearchNumericFilterBuilder _numericFilterBuilder = new RediSearchNumericFilterBuilder();
        private readonly SummarizeBuilder _summarizeBuilder = new SummarizeBuilder();
        private readonly HighlightBuilder _highlightBuilder = new HighlightBuilder();

        public object[] Build()
        {
            var argumentLength = 2; // {index} {query}

            argumentLength += _noContent ? 1 : 0; // [NOCONTENT]
            argumentLength += _verbatim ? 1 : 0; // [VERBATIM]
            argumentLength += _noStopWords ? 1 : 0; // [NOSTOPWORDS]
            argumentLength += _withScores ? 1 : 0; // [WITHSCORES]
            argumentLength += _withPayloads ? 1 : 0; // [WITHPAYLOADS]
            argumentLength += _withSortKeys ? 1 : 0; // [WITHSORTKEYS]

            // [FILTER {numeric_field} {min} {max}] ...
            var numericFilters = _numericFilters.Select(x => x(_numericFilterBuilder)).ToList();

            argumentLength += (numericFilters.Count * 4);

            // [GEOFILTER {geo_field} {lon} {lat} {radius} m|km|mi|ft]
            argumentLength += _geoFilterDefinition == null ? 0 : 6;

            // [INKEYS {num} {key} ... ]
            argumentLength += _inKeys != null ? 2 + _inKeys.Length : 0;

            // [INFIELDS {num} {key} ... ]
            argumentLength += _inFields != null ? 2 + _inFields.Length : 0;

            // [RETURN {num} {key} ... ]
            argumentLength += _returnFields != null ? 2 + _returnFields.Length : 0;

            // [SUMMARIZE [FIELDS {num} {field} ... ] [FRAGS {num}] [LEN {fragsize}] [SEPARATOR {separator}]]
            if (_summarizeBuilderAction != null)
            {
                _summarizeBuilderAction(_summarizeBuilder);

                argumentLength += _summarizeBuilder.FieldArguments.Length;
            }

            // [HIGHLIGHT [FIELDS {num} {field} ... ] [TAGS {open} {close}]]
            if (_highlightBuilderAction != null)
            {
                _highlightBuilderAction(_highlightBuilder);

                argumentLength += _highlightBuilder.FieldArguments.Length;
            }

            // [SLOP {slop}] 
            argumentLength += _slop > 0 ? 2 : 0;

            // [INORDER]
            argumentLength += _inOrder ? 1 : 0;

            // [LANGUAGE { language}]
            argumentLength += string.IsNullOrEmpty(_language) ? 0 : 2;

            // [EXPANDER { expander}]
            argumentLength += string.IsNullOrEmpty(_expander) ? 0 : 2;

            // [SCORER { scorer}] 
            argumentLength += string.IsNullOrEmpty(_scorer) ? 0 : 2;

            // [PAYLOAD { payload}]
            argumentLength += string.IsNullOrEmpty(_payload) ? 0 : 2;

            // [SORTBY { field} [ASC| DESC]]
            argumentLength += _sortBy == default ? 0 : 3;

            // [LIMIT offset num]
            argumentLength += _limit == default ? 0 : 3;

            var result = new object[argumentLength];

            var currentArgumentIndex = 0;

            // {index} {query}
            result[currentArgumentIndex] = _indexName;
            result[++currentArgumentIndex] = _query;

            // [NOCONTENT]
            if (_noContent)
            {
                result[++currentArgumentIndex] = "NOCONTENT";
            }

            // [VERBATIM]
            if (_verbatim)
            {
                result[++currentArgumentIndex] = "VERBATIM";
            }

            // [NOSTOPWORDS]
            if (_noStopWords)
            {
                result[++currentArgumentIndex] = "NOSTOPWORDS";
            }

            // [WITHSCORES]
            if (_withScores)
            {
                result[++currentArgumentIndex] = "WITHSCORES";
            }

            // [WITHPAYLOADS]
            if (_withPayloads)
            {
                result[++currentArgumentIndex] = "WITHPAYLOADS";
            }

            // [WITHSORTKEYS]
            if (_withSortKeys)
            {
                result[++currentArgumentIndex] = "WITHSORTKEYS";
            }

            // [FILTER {numeric_field} {min} {max}] ...
            if (_numericFilters != null)
            {
                foreach (var numericFilter in numericFilters)
                {
                    result[++currentArgumentIndex] = "FILTER";
                    result[++currentArgumentIndex] = numericFilter.FieldName;
                    result[++currentArgumentIndex] = numericFilter.Min;
                    result[++currentArgumentIndex] = numericFilter.Max;
                }
            }

            // [GEOFILTER {geo_field} {lon} {lat} {radius} m|km|mi|ft]
            if (_geoFilterDefinition != null)
            {
                result[++currentArgumentIndex] = "GEOFILTER";
                result[++currentArgumentIndex] = _geoFilterDefinition.FieldName;
                result[++currentArgumentIndex] = _geoFilterDefinition.Latitude.ToString();
                result[++currentArgumentIndex] = _geoFilterDefinition.Longitude.ToString();
                result[++currentArgumentIndex] = _geoFilterDefinition.Radius.ToString();
                result[++currentArgumentIndex] = _geoFilterDefinition.DistanceUnit.ToString();
            }

            // [INKEYS {num} {key} ... ]
            if (_inKeys != null)
            {
                result[++currentArgumentIndex] = "INKEYS";
                result[++currentArgumentIndex] = _inKeys.Length.ToString();

                foreach (var inKey in _inKeys)
                {
                    result[++currentArgumentIndex] = inKey;
                }
            }

            // [INFIELDS {num} {key} ... ]
            if (_inFields != null)
            {
                result[++currentArgumentIndex] = "INFIELDS";
                result[++currentArgumentIndex] = _inFields.Length.ToString();

                foreach (var inField in _inFields)
                {
                    result[++currentArgumentIndex] = inField;
                }
            }

            // [RETURN {num} {key} ... ]
            if (_returnFields != null)
            {
                result[++currentArgumentIndex] = "RETURN";
                result[++currentArgumentIndex] = _returnFields.Length.ToString();

                foreach (var returnField in _returnFields)
                {
                    result[++currentArgumentIndex] = returnField;
                }
            }

            // [SUMMARIZE [FIELDS {num} {field} ... ] [FRAGS {num}] [LEN {fragsize}] [SEPARATOR {separator}]]
            if (_summarizeBuilder.FieldArguments.Length > 0)
            {
                foreach (var arg in _summarizeBuilder.FieldArguments)
                {
                    result[++currentArgumentIndex] = arg;
                }
            }

            // [HIGHLIGHT [FIELDS {num} {field} ... ] [TAGS {open} {close}]]
            if (_highlightBuilder.FieldArguments.Length > 0)
            {
                foreach (var arg in _highlightBuilder.FieldArguments)
                {
                    result[++currentArgumentIndex] = arg;
                }
            }

            // [SLOP {slop}] 
            if (_slop > 0)
            {
                result[++currentArgumentIndex] = "SLOP";
                result[++currentArgumentIndex] = _slop.ToString();
            }

            // [INORDER]
            if (_inOrder)
            {
                result[++currentArgumentIndex] = "INORDER";
            }

            // [LANGUAGE { language}]
            if (!string.IsNullOrEmpty(_language))
            {
                result[++currentArgumentIndex] = "LANGUAGE";
                result[++currentArgumentIndex] = _language;
            }

            // [EXPANDER { expander}]
            if (!string.IsNullOrEmpty(_expander))
            {
                result[++currentArgumentIndex] = "EXPANDER";
                result[++currentArgumentIndex] = _expander;
            }

            // [SCORER { scorer}] 
            if (!string.IsNullOrEmpty(_scorer))
            {
                result[++currentArgumentIndex] = "SCORER";
                result[++currentArgumentIndex] = _scorer;
            }

            // [PAYLOAD { payload}]
            if (!string.IsNullOrEmpty(_payload))
            {
                result[++currentArgumentIndex] = "PAYLOAD";
                result[++currentArgumentIndex] = _payload;
            }

            // [SORTBY { field} [ASC| DESC]]
            if (_sortBy != default)
            {
                result[++currentArgumentIndex] = "SORTBY";
                result[++currentArgumentIndex] = _sortBy.fieldName;
                result[++currentArgumentIndex] = _sortBy.direction == Direction.Ascending ? "ASC" : "DESC";
            }

            // [LIMIT offset num]
            if (_limit != default)
            {
                result[++currentArgumentIndex] = "LIMIT";
                result[++currentArgumentIndex] = _limit.offset.ToString();
                result[++currentArgumentIndex] = _limit.limit.ToString();
            }

            return result;
        }
    }
}