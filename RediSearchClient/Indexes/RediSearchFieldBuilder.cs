namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Defines available schema fields types for the index builder. 
    /// </summary>
    public sealed class RediSearchSchemaFieldBuilder
    {
        /// <summary>
        /// Create a "TEXT" field on the schema.
        /// </summary>
        /// <param name="fieldName">Name of the hash key being indexed.</param>
        /// <param name="sortable">Allows for the indexed value to be sorted later. Adds memory overhead, don't use on large text fields.</param>
        /// <param name="nostem">If set to true, disables stemming the index values.</param>
        /// <param name="noindex">If set to true, this field will not be indexed.</param>
        /// <param name="phonetic">If set, phonetic matches will be performed on this field by default.</param>
        /// <param name="weight">Used when calculating result accuracy. Defaults to 1.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Text(string fieldName, bool sortable = false, bool nostem = false, bool noindex = false, Language phonetic = Language.None, double weight = 1) =>
            new TextSchemaField(fieldName, sortable, nostem, noindex, phonetic, weight);

        /// <summary>
        /// Create a "TAG" field on the schema.
        /// </summary>
        /// <param name="fieldName">Name of the hash key being indexed.</param>
        /// <param name="separator">The field separator. Defaults to `,`.</param>
        /// <param name="sortable">Allows for the indexed value to be sorted later. Adds memory overhead, don't use on large text fields.</param>
        /// <param name="noindex">If set to true, this field will not be indexed.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Tag(string fieldName, string separator = ",", bool sortable = false, bool noindex = false) =>
            new TagSchemaField(fieldName, separator, sortable, noindex);

        /// <summary>
        /// Create a "NUMERIC" field on the schema.
        /// </summary>
        /// <param name="fieldName">Name of the hash key being indexed.</param>
        /// <param name="sortable">Allows for the indexed value to be sorted later.</param>
        /// <param name="noindex">If set to true, this field will not be indexed.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Numeric(string fieldName, bool sortable = false, bool noindex = false) =>
            new NumericSchemaField(fieldName, sortable, noindex);

        /// <summary>
        /// Create a "GEO" field on the schema.
        /// </summary>
        /// <param name="fieldName">Name of the hash key being indexed.</param>
        /// <returns></returns>
        public IRediSearchSchemaField Geo(string fieldName) =>
            new GeoSchemaField(fieldName);
    }
}