using System;
using System.Collections.Generic;
using System.Linq;

namespace RediSearchClient.Indexes
{
    /// <summary>
    /// This is the builder wherein a majority of an index is defined.
    /// </summary>
    public sealed class RediSearchIndexBuilder
    {
        private readonly RediSearchStructure _structure;

        internal RediSearchIndexBuilder(RediSearchStructure structure) =>
            _structure = structure;

        private List<string> _prefixes;

        /// <summary>
        /// Builder method for defining the key pattern to index.
        /// </summary>
        /// <param name="prefix">Key pattern for which items to index.</param>
        /// <returns></returns>
        public RediSearchIndexBuilder ForKeysWithPrefix(string prefix)
        {
            if (_prefixes == null)
            {
                _prefixes = new List<string>(2);
            }

            _prefixes.Add(prefix);

            return this;
        }

        private string _filter;

        /// <summary>
        /// Allows for specifying a filter for indexable items.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder UsingFilter(string filter)
        {
            _filter = filter;

            return this;
        }

        private string _language;

        /// <summary>
        /// Sets the language for the index, defaults to English.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder UsingLanguage(string language)
        {
            _language = language;

            return this;
        }

        private string _languageField;

        /// <summary>
        /// Sets the field to source the document language from.
        /// </summary>
        /// <param name="languageField"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder UsingLanguageField(string languageField)
        {
            _languageField = languageField;

            return this;
        }

        private double _score = 1;

        /// <summary>
        /// Sets the default score for documents.
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder SetScore(double score)
        {
            _score = score;

            return this;
        }

        private string _scoreField;

        /// <summary>
        /// Sets the field to source the document score from.
        /// </summary>
        /// <param name="scoreField"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder SetScoreField(string scoreField)
        {
            _scoreField = scoreField;

            return this;
        }

        private string _payloadField;

        /// <summary>
        /// Sets the field that should be used as a binary safe payload string for the document.
        /// </summary>
        /// <param name="payloadField"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder SetPayloadField(string payloadField)
        {
            _payloadField = payloadField;

            return this;
        }

        private bool _maxTextFields;

        /// <summary>
        /// For efficiency, RediSearch encodes indexes differently if they are created with less 
        /// than 32 text fields. This option forces RediSearch to encode indexes as if there were 
        /// more than 32 text fields, which allows you to add additional fields (beyond 32) using 
        /// `AlterSchema` or `AlterSchemaAsync`.
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexBuilder MaxTextFields()
        {
            _maxTextFields = true;

            return this;
        }

        private bool _noOffsets;

        /// <summary>
        /// If set, we do not store term offsets for documents (saves memory, does not allow 
        /// exact searches or highlighting).
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexBuilder NoOffsets()
        {
            _noOffsets = true;

            return this;
        }

        private bool _noHighLights;

        /// <summary>
        /// Conserves storage space and memory by disabling highlighting support. If set, we 
        /// do not store corresponding byte offsets for term positions.
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexBuilder NoHighLights()
        {
            _noHighLights = true;

            return this;
        }

        private int _lifespanInSeconds;

        /// <summary>
        /// Create a lightweight temporary index which will expire after the specified period 
        /// of inactivity. The internal idle timer is reset whenever the index is searched or 
        /// added to. Because such indexes are lightweight, you can create thousands of such 
        /// indexes without negative performance implications and therefore you should consider 
        /// using `.SkipInitialScan()` to avoid costly scanning.
        /// </summary>
        /// <param name="lifespanInSeconds"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder Temporary(int lifespanInSeconds)
        {
            _lifespanInSeconds = lifespanInSeconds;

            return this;
        }

        private bool _noFields;

        /// <summary>
        /// If set, we do not store field bits for each term. Saves memory, does not allow 
        /// filtering by specific fields.
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexBuilder NoFields()
        {
            _noFields = true;

            return this;
        }

        private bool _noFrequencies;

        /// <summary>
        /// If set, we avoid saving the term frequencies in the index. This saves memory 
        /// but does not allow sorting based on the frequencies of a given term within the document.
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexBuilder NoFrequencies()
        {
            _noFrequencies = true;

            return this;
        }

        private bool _skipInitialScan;

        /// <summary>
        /// If set, we do not scan and index.
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexBuilder SkipInitialScan()
        {
            _skipInitialScan = true;

            return this;
        }

        private Func<RediSearchSchemaFieldBuilder, IRediSearchSchemaField>[] _fields;

        /// <summary>
        /// Allows for defining the schema of the search index. 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder WithSchema(params Func<RediSearchSchemaFieldBuilder, IRediSearchSchemaField>[] fields)
        {
            _fields = fields;

            return this;
        }

        private Func<RediSearchJsonSchemaFieldBuilder, IRediSearchSchemaField>[] _jsonFields;
        
        /// <summary>
        /// Allows for defining the schema of a search index for JSON documents.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public RediSearchIndexBuilder WithJsonSchema(params Func<RediSearchJsonSchemaFieldBuilder, IRediSearchSchemaField>[] fields)
        {
            _jsonFields = fields;

            return this;
        }

        private static readonly RediSearchSchemaFieldBuilder _fieldBuilder = new RediSearchSchemaFieldBuilder();

        private static readonly RediSearchJsonSchemaFieldBuilder _jsonFieldBuilder =
            new RediSearchJsonSchemaFieldBuilder();

        /// <summary>
        /// Builds the index definition. 
        /// </summary>
        /// <returns></returns>
        public RediSearchIndexDefinition Build()
        {
            var argumentLength = 2; // ON {structure}

            argumentLength += 2 + _prefixes.Count; // [PREFIX {count} {prefix} [{prefix} ..]
            argumentLength += string.IsNullOrEmpty(_filter) ? 0 : 2; // [FILTER {filter}]		
            argumentLength += string.IsNullOrEmpty(_language) ? 0 : 2; // [LANGUAGE {default_lang}]
            argumentLength += string.IsNullOrEmpty(_languageField) ? 0 : 2; // [LANGUAGE_FIELD {lang_field}]
            argumentLength += _score != 1 ? 2 : 0; // [SCORE {default_score}]
            argumentLength += string.IsNullOrEmpty(_scoreField) ? 0 : 2; // [SCORE_FIELD {score_field}]
            argumentLength += string.IsNullOrEmpty(_payloadField) ? 0 : 2; // [PAYLOAD_FIELD {payload_field}]
            argumentLength += _maxTextFields ? 1 : 0; // [MAXTEXTFIELDS]
            argumentLength += _lifespanInSeconds > 0 ? 2 : 0; // [TEMPORARY {seconds}]
            argumentLength += _noOffsets ? 1 : 0; // [NOOFFSETS]
            argumentLength += _noHighLights ? 1 : 0; // [NOHL]
            argumentLength += _noFields ? 1 : 0; // [NOFIELDS]
            argumentLength += _noFrequencies ? 1 : 0; // [NOFREQS]
            argumentLength += _skipInitialScan ? 1 : 0; // [SKIPINITIALSCAN]

            // If there are no schema fields we should probably throw an exception eh?
            var schemaFields = _fields.Select(x => x(_fieldBuilder)).ToList();

            var jsonSchemaFields = _jsonFields.Select(x => x(_jsonFieldBuilder)).ToList();

            argumentLength += schemaFields.Sum(x => x.FieldArguments.Length) + 1;

            var result = new object[argumentLength];

            var currentArgumentIndex = 0;

            // ON {structure}
            result[currentArgumentIndex] = "ON";
            result[++currentArgumentIndex] = _structure.ToString();

            // [PREFIX {count} {prefix} [{prefix} ..]
            result[++currentArgumentIndex] = "PREFIX";
            result[++currentArgumentIndex] = _prefixes.Count;

            foreach (var prefix in _prefixes)
            {
                result[++currentArgumentIndex] = prefix;
            }

            // [FILTER {filter}]
            if (!string.IsNullOrEmpty(_filter))
            {
                result[++currentArgumentIndex] = "FILTER";
                result[++currentArgumentIndex] = _filter;
            }

            // [LANGUAGE {default_lang}]
            if (!string.IsNullOrEmpty(_language))
            {
                result[++currentArgumentIndex] = "LANGUAGE";
                result[++currentArgumentIndex] = _language;
            }

            // [LANGUAGE_FIELD {lang_field}]
            if (!string.IsNullOrEmpty(_languageField))
            {
                result[++currentArgumentIndex] = "LANGUAGE_FIELD";
                result[++currentArgumentIndex] = _languageField;
            }

            // [SCORE {default_score}]
            if (_score != 1)
            {
                result[++currentArgumentIndex] = "SCORE";
                result[++currentArgumentIndex] = _score;
            }

            // [SCORE_FIELD {score_field}]
            if (!string.IsNullOrEmpty(_scoreField))
            {
                result[++currentArgumentIndex] = "SCORE_FIELD";
                result[++currentArgumentIndex] = _scoreField;
            }

            // [PAYLOAD_FIELD {payload_field}]
            if (!string.IsNullOrEmpty(_payloadField))
            {
                result[++currentArgumentIndex] = "PAYLOAD_FIELD";
                result[++currentArgumentIndex] = _payloadField;
            }

            // [MAXTEXTFIELDS]
            if (_maxTextFields)
            {
                result[++currentArgumentIndex] = "MAXTEXTFIELDS";
            }

            // [TEMPORARY {seconds}]
            if (_lifespanInSeconds > 0)
            {
                result[++currentArgumentIndex] = "TEMPORARY";
                result[++currentArgumentIndex] = _lifespanInSeconds;
            }

            // [NOOFFSETS]
            if (_noOffsets)
            {
                result[++currentArgumentIndex] = "NOOFFSETS";
            }

            // [NOHL]
            if (_noHighLights)
            {
                result[++currentArgumentIndex] = "NOHL";
            }

            // [NOFIELDS]
            if (_noFields)
            {
                result[++currentArgumentIndex] = "NOFIELDS";
            }

            // [NOFREQS]
            if (_noFrequencies)
            {
                result[++currentArgumentIndex] = "NOFREQS";
            }

            // [SKIPINITIALSCAN]
            if (_skipInitialScan)
            {
                result[++currentArgumentIndex] = "SKIPINITIALSCAN";
            }

            // SCHEMA {field} [TEXT [NOSTEM] [WEIGHT {weight}] [PHONETIC {matcher}] | NUMERIC | GEO | TAG [SEPARATOR {sep}] ] [SORTABLE][NOINDEX] ...
            result[++currentArgumentIndex] = "SCHEMA";

            foreach (var field in schemaFields.SelectMany(x => x.FieldArguments))
            {
                result[++currentArgumentIndex] = field;
            }

            return new RediSearchIndexDefinition(result);
        }
    }
}