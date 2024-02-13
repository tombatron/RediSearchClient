namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Defines available JSON based schema field types for the index builder. 
    /// </summary>
    public sealed class RediSearchJsonSchemaFieldBuilder
    {
        /// <summary>
        /// Creates a "TEXT" field on the schema.
        /// </summary>
        /// <param name="jsonPath">JSONPath to the desired field.</param>
        /// <param name="alias">The alias used to refer to the value at the specified JSONPath.</param>
        /// <param name="sortable">Allows for the indexed value to be sorted later. Adds memory overhead, don't use on large text fields.</param>
        /// <param name="nostem">If set to true, disables stemming the index values.</param>
        /// <param name="noindex">If set to true, this field will not be indexed.</param>
        /// <param name="phonetic">If set, phonetic matches will be performed on this field by default.</param>
        /// <param name="weight">Used when calculated result accuracy. Defaults to 1.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Text(string jsonPath, string alias, bool sortable = false, bool nostem = false,
            bool noindex = false, Language phonetic = Language.None, double weight = 1) =>
            new TextJsonSchemaField(jsonPath, alias, sortable, nostem, noindex, phonetic, weight);

        /// <summary>
        /// Create a "TAG" field on the schema.
        /// </summary>
        /// <param name="jsonPath">JSONPath to the desired field.</param>
        /// <param name="alias">The alias used to refer to the value at the specified JSONPath.</param>
        /// <param name="seperator">The field separator. Defaults to `,`.</param>
        /// <param name="sortable">Allows for the indexed value to be sorted later. Adds memory overhead, don't use on large text fields.</param>
        /// <param name="noindex">If set to true, this field will not be indexed.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Tag(string jsonPath, string alias, string seperator = ",", bool sortable = false,
            bool noindex = false) =>
            new TagJsonSchemaField(jsonPath, alias, seperator, sortable, noindex);

        /// <summary>
        /// Create a "NUMERIC" field on the schema.
        /// </summary>
        /// <param name="jsonPath">JSONPath to the desired field.</param>
        /// <param name="alias">The alias used to refer to the value at the specified JSONPath.</param>
        /// <param name="sortable">Allows for the indexed value to be sorted later.</param>
        /// <param name="noindex">If set to true, this field will not be indexed.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Numeric(string jsonPath, string alias, bool sortable = false, bool noindex = false) => 
            new NumericJsonSchemaField(jsonPath, alias, sortable, noindex);

        /// <summary>
        /// Create a "GEO" field on the schema.
        /// </summary>
        /// <param name="jsonPath">JSONPath to the desired field.</param>
        /// <param name="alias">The alias used to refer to the value at the specified JSONPath.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Geo(string jsonPath, string alias) => 
            new GeoJsonSchemaField(jsonPath, alias);

        /// <summary>
        /// Creates a "VECTOR" field on the schema.
        /// </summary>
        /// <param name="jsonPath">JSONPath to the desired field.</param>
        /// <param name="alias">The alias used to refer to the value at the specified JSONPath.</param>
        /// <param name="vectorIndexAlgorithm">HNSW or FLAT with the attendent parameters.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Vector(string jsonPath, string alias, VectorIndexAlgorithm vectorIndexAlgorithm) =>
            new VectorSchemaField(jsonPath, alias, vectorIndexAlgorithm);
    }
}