namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Essentially guides the index creation toward the path of JSON based.
    /// </summary>
    public class RediSearchJsonIndexBuilder : BaseRediSearchIndexBuilder<RediSearchJsonSchemaFieldBuilder>
    {
        /// <summary>
        /// Returns the kind of structure that we're creating the index on.
        /// </summary>
        /// <returns></returns>
        protected override string ResolveStructure() => "JSON";
    }
}