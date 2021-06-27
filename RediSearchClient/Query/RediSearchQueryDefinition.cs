namespace RediSearchClient.Query
{
    /// <summary>
    /// Container class for the result from the query builder. 
    /// </summary>
    public sealed class RediSearchQueryDefinition
    {
        internal object[] Fields { get; }

        internal RediSearchQueryDefinition(object[] fields) =>
            Fields = fields;
    }
}