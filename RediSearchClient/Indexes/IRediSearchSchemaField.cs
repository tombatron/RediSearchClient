namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Contract representing a field definition on a RediSearch schema.
    /// </summary>
    public interface IRediSearchSchemaField
    {
        /// <summary>
        /// The collection of arguments that will be passed as the definition
        /// of the schema field through the `FT.CREATE` command.
        /// </summary>
        /// <value>Collection of schema field arguments.</value>
        object[] FieldArguments { get; }
    }
}