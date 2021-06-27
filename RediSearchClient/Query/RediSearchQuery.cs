namespace RediSearchClient.Query
{
    /// <summary>
    /// Factory class that kicks off the query builder.
    /// </summary>
    public static class RediSearchQuery
    {
        /// <summary>
        /// Builder method for specifying while index to apply the query to.
        /// </summary>
        /// <param name="indexName">The index to query.</param>
        /// <returns></returns>
        public static RediSearchQueryBuilder On(string indexName) =>
            new RediSearchQueryBuilder(indexName);
    }
}