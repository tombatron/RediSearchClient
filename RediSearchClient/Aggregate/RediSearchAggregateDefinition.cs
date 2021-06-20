namespace RediSearchClient.Aggregate
{
    /// <summary>
    /// Used as a container for the output of the aggregate query builder.
    /// </summary>
    public sealed class RediSearchAggregateDefinition
    {
        internal object[] Fields { get; }

        internal RediSearchAggregateDefinition(object[] fields) =>
            Fields = fields;
    }
}