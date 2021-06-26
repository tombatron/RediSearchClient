namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Container class for the result from the index builder. 
    /// </summary>
    public sealed class RediSearchIndexDefinition
    {
        internal object[] Fields { get; }

        internal RediSearchIndexDefinition(object[] fields) =>
            Fields = fields;
    }
}