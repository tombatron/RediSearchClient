namespace RediSearchClient.Indexes
{
    public sealed class RediSearchSchemaFieldBuilder
    {
        public IRediSearchSchemaField Text(string fieldName, bool sortable = false, bool nostem = false, bool noindex = false, Language phonetic = Language.None, double weight = 1) =>
            new TextSchemaField(fieldName, sortable, nostem, noindex, phonetic, weight);

        public IRediSearchSchemaField Tag(string fieldName, string separator = ",") =>
            new TagSchemaField(fieldName, separator);

        public IRediSearchSchemaField Numeric(string fieldName) =>
            new NumericSchemaField(fieldName);

        public IRediSearchSchemaField Geo(string fieldName) =>
            new GeoSchemaField(fieldName);
    }
}