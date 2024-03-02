using System;
using System.Collections.Generic;
using System.Linq;

namespace RediSearchClient.Query
{
    /// <summary>
    /// Primary builder for queries.
    /// </summary>
    public sealed class RediSearchQueryBuilder
    {
        private readonly string _indexName;

        internal RediSearchQueryBuilder(string indexName) =>
            _indexName = indexName;

        private string _query;

        /// <summary>
        /// Builder method for applying a query string.
        /// 
        /// https://oss.redislabs.com/redisearch/Query_Syntax/
        /// </summary>
        /// <param name="query">Valid query syntax goes here.</param>
        /// <returns></returns>
        public RediSearchQueryBuilder UsingQuery(string query)
        {
            _query = query;

            return this;
        }

        private Func<RediSearchNumericFilterBuilder, IRediSearchNumericFilter>[] _numericFilters;

        /// <summary>
        /// Builder method for applying numeric filter(s) to a query.
        /// </summary>
        /// <param name="numericFilters"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder WithNumericFilters(params Func<RediSearchNumericFilterBuilder, IRediSearchNumericFilter>[] numericFilters)
        {
            _numericFilters = numericFilters;

            return this;
        }

        private GeoFilterDefinition _geoFilterDefinition;

        /// <summary>
        /// Builder method for applying a geo filter to a query.
        /// </summary>
        /// <param name="fieldName">Name of the geo field to filter on.</param>
        /// <param name="latitude">The latitude to filter with.</param>
        /// <param name="longitude">The longitude to filter with.</param>
        /// <param name="radius">The radius around the coordinates to include.</param>
        /// <param name="unit">The unit of measurement to apply to the radius.</param>
        /// <returns></returns>
        public RediSearchQueryBuilder WithGeoFilter(string fieldName, double latitude, double longitude, double radius, Unit unit)
        {
            _geoFilterDefinition = new GeoFilterDefinition(fieldName, latitude, longitude, radius, unit);

            return this;
        }

        private string[] _inKeys;

        /// <summary>
        /// If set, we limit the result to a given set of keys specified in the list. Non-
        /// existent keys are ignored.
        /// </summary>
        /// <param name="inKeys">The key(s) to include in the result.</param>
        /// <returns></returns>
        public RediSearchQueryBuilder InKeys(params string[] inKeys)
        {
            _inKeys = inKeys;

            return this;
        }

        private string[] _inFields;

        /// <summary>
        /// If set, filter the results to ones appearing only in specific fields of the document, like title or URL.
        /// </summary>
        /// <param name="inFields">The field(s) to filter on.</param>
        /// <returns></returns>
        public RediSearchQueryBuilder InFields(params string[] inFields)
        {
            _inFields = inFields;

            return this;
        }

        private string[] _returnFields;

        /// <summary>
        /// Builder method for specifying the document fields to return.
        /// </summary>
        /// <param name="returnFields">The names of the fields to return.</param>
        /// <returns></returns>
        public RediSearchQueryBuilder Return(params string[] returnFields)
        {
            _returnFields = returnFields;

            return this;
        }

        private Action<SummarizeBuilder> _summarizeBuilderAction;

        /// <summary>
        /// Builder method to configure the returning of only the sections of a field
        /// that contain matched text.
        /// </summary>
        /// <param name="summarizeBuilder"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Summarize(Action<SummarizeBuilder> summarizeBuilder)
        {
            _summarizeBuilderAction = summarizeBuilder;

            return this;
        }

        private Action<HighlightBuilder> _highlightBuilderAction;

        /// <summary>
        /// Builder method for configuring the formatting of matched sections of text.
        /// </summary>
        /// <param name="highlightBuilder"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Highlight(Action<HighlightBuilder> highlightBuilder)
        {
            _highlightBuilderAction = highlightBuilder;

            return this;
        }

        private int _slop;

        /// <summary>
        /// Builder method for specifying the maximum number of unmatched offsets between phrase terms.
        /// </summary>
        /// <param name="slop"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Slop(int slop)
        {
            _slop = slop;

            return this;
        }

        private bool _inOrder;

        /// <summary>
        /// Builder method for specifying that matches to a query term appear in the same order in the document
        /// as in the query. Usually used with "Slop".
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder InOrder()
        {
            _inOrder = true;

            return this;
        }

        private string _language;

        /// <summary>
        /// Builder method for specifying the language stemmer to be used during query expansion.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Language(SearchLanguage language)
        {
            _language = language;

            return this;
        }

        private string _expander;

        /// <summary>
        /// Builder method for specifying a custome expander for a query.
        /// </summary>
        /// <param name="expander"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Expander(string expander)
        {
            _expander = expander;

            return this;
        }

        private string _scorer;

        /// <summary>
        /// Builder method for specifying a custom scorer for a query.
        /// </summary>
        /// <param name="scorer"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Scorer(string scorer)
        {
            _scorer = scorer;

            return this;
        }

        private string _payload;

        /// <summary>
        /// Builder method for specifying an abritary, binary safe payload that will be 
        /// exposed to the scoring functions.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Payload(string payload)
        {
            _payload = payload;

            return this;
        }

        private (string fieldName, Direction direction) _sortBy;

        /// <summary>
        /// Builder method for specifying that results should be sorted in a specific order by
        /// a specified field.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder SortBy(string fieldName, Direction direction)
        {
            _sortBy = (fieldName, direction);

            return this;
        }

        private (int offset, int limit) _limit;

        /// <summary>
        /// Builder method for specifying how to limit the returned results.
        /// 
        /// The default is "0" offset with a limit of "10" items.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public RediSearchQueryBuilder Limit(int offset, int limit)
        {
            _limit = (offset, limit);

            return this;
        }

        private bool _noContent;

        /// <summary>
        /// Builder method for specifying "NOCONTENT". If set the query will only return
        /// document IDs with no content.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder NoContent()
        {
            _noContent = true;

            return this;
        }

        private bool _verbatim;

        /// <summary>
        /// Builder method for specifying "VERBATIM". If set, no stemming will be applied
        /// to the query.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder Verbatim()
        {
            _verbatim = true;

            return this;
        }

        private bool _noStopWords;

        /// <summary>
        /// Builder method for specifying "NOSTOPWORD". If set, stopwords will not be filtered from the 
        /// query.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder NoStopWords()
        {
            _noStopWords = true;

            return this;
        }

        private bool _withScores;

        /// <summary>
        /// Builder method for specifying "WITHSCORES". If set, the query results will also include the 
        /// internal score of the result.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder WithScores()
        {
            _withScores = true;

            return this;
        }

        private bool _withSortKeys;

        /// <summary>
        /// Builder method for specifying "WITHSORTKEYS". Only relevant in conjunction with SORTBY . 
        /// Returns the value of the sorting key, right after the id and score and /or payload if 
        /// requested. This is usually not needed by users, and exists for distributed search 
        /// coordination purposes.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder WithSortKeys()
        {
            _withSortKeys = true;

            return this;
        }

        private bool _withPayloads;

        /// <summary>
        /// Builder method for specifying "WITHPAYLOADS". If set, we retrieve optional document payloads.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder WithPayloads()
        {
            _withPayloads = true;

            return this;
        }

        private int? _dialect;

        /// <summary>
        /// Builder method for specifying "DIALECT". This allows targetting a specific RediSearch dialect. If it is not specified, the DEFAULT_DIALECT is used, 
        /// which can be set using FT.CONFIG SET or by passing it as an argument to the redisearch module when it is loaded.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryBuilder Dialect(int dialect)
        {
            _dialect = dialect;

            return this;
        }

        /// <summary>
        /// Builder method for returning the KNN vector query builder.
        /// 
        /// https://redis.io/docs/interact/search-and-query/query/vector-search/
        /// </summary>
        /// <returns></returns>
        public RediSearchKnnVectorQueryBuilder VectorKnn()
        {
            var vectorQueryBuilder = new RediSearchKnnVectorQueryBuilder(_indexName);

            if (string.IsNullOrEmpty(_query))
            {
                return vectorQueryBuilder;
            }
            else
            {
                return vectorQueryBuilder.Prefilter(_query);
            }
        }


        /// <summary>
        /// Builder method for returning the Range vector query builder.
        /// 
        /// https://redis.io/docs/interact/search-and-query/query/vector-search/
        /// </summary>
        /// <returns></returns>
        public RediSearchRangeVectorQueryBuilder VectorRange()
        {
            var vectorQueryBuilder = new RediSearchRangeVectorQueryBuilder(_indexName);

            if (string.IsNullOrEmpty(_query))
            {
                return vectorQueryBuilder;
            }
            else
            {
                return vectorQueryBuilder.AdditionalFilter(_query);
            }
        }

        private static readonly RediSearchNumericFilterBuilder _numericFilterBuilder = new RediSearchNumericFilterBuilder();
        private readonly SummarizeBuilder _summarizeBuilder = new SummarizeBuilder();
        private readonly HighlightBuilder _highlightBuilder = new HighlightBuilder();

        /// <summary>
        /// Builds the query definition.
        /// </summary>
        /// <returns></returns>
        public RediSearchQueryDefinition Build()
        {
            var argumentLength = 2; // {index} {query}

            argumentLength += _noContent ? 1 : 0; // [NOCONTENT]
            argumentLength += _verbatim ? 1 : 0; // [VERBATIM]
            argumentLength += _noStopWords ? 1 : 0; // [NOSTOPWORDS]
            argumentLength += _withScores ? 1 : 0; // [WITHSCORES]
            argumentLength += _withPayloads ? 1 : 0; // [WITHPAYLOADS]
            argumentLength += _withSortKeys ? 1 : 0; // [WITHSORTKEYS]

            // [FILTER {numeric_field} {min} {max}] ...
            List<IRediSearchNumericFilter> numericFilters = null;

            if (_numericFilters != null)
            {
                numericFilters = _numericFilters.Select(x => x(_numericFilterBuilder)).ToList();

                argumentLength += (numericFilters.Count * 4);
            }

            // [GEOFILTER {geo_field} {lon} {lat} {radius} m|km|mi|ft]
            argumentLength += _geoFilterDefinition == null ? 0 : 6;

            // [INKEYS {num} {key} ... ]
            argumentLength += _inKeys != null ? 2 + _inKeys.Length : 0;

            // [INFIELDS {num} {key} ... ]
            argumentLength += _inFields != null ? 2 + _inFields.Length : 0;

            // [RETURN {num} {key} ... ]
            argumentLength += _returnFields != null ? 2 + _returnFields.Length : 0;

            // [DIALECT dialect]
            argumentLength += _dialect.HasValue ? 2 : 0;

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
            if (numericFilters != null)
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
                result[++currentArgumentIndex] = _geoFilterDefinition.Longitude.ToString();
                result[++currentArgumentIndex] = _geoFilterDefinition.Latitude.ToString();
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
            if (_summarizeBuilderAction != null && _summarizeBuilder.FieldArguments.Length > 0)
            {
                foreach (var arg in _summarizeBuilder.FieldArguments)
                {
                    result[++currentArgumentIndex] = arg;
                }
            }

            // [HIGHLIGHT [FIELDS {num} {field} ... ] [TAGS {open} {close}]]
            if (_highlightBuilderAction != null && _highlightBuilder.FieldArguments.Length > 0)
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

            // [DIALECT]
            if (_dialect.HasValue)
            {
                result[++currentArgumentIndex] = "DIALECT";
                result[++currentArgumentIndex] = _dialect.Value;
            }

            return new RediSearchQueryDefinition(result);
        }
    }
}