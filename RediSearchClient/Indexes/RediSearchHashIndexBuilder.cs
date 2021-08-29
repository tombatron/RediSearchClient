namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Essentially guides the index creation toward the path of HASH based.
    /// </summary>
    public class RediSearchHashIndexBuilder : BaseRediSearchIndexBuilder<RediSearchSchemaFieldBuilder>
    {
        /// <summary>
        /// Returns the kind of structure that we're creating the index on.
        /// </summary>
        /// <returns></returns>
        protected override string ResolveStructure() => "HASH";
    }
}