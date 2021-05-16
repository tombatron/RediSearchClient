namespace RediSearchClient.Indexes
{
    public sealed class RediSearchIndexDefinition
    {
        internal object[] Fields { get; }

        internal RediSearchIndexDefinition(object[] fields) =>
            Fields = fields;
    }
}