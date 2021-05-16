namespace RediSearchClient.Query
{
    public sealed class RediSearchQueryDefinition
    {
        internal object[] Fields { get; }

        internal RediSearchQueryDefinition(object[] fields) =>
            Fields = fields;
    }
}