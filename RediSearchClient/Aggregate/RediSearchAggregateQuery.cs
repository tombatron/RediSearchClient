namespace RediSearchClient.Aggregate
{
	/// <summary>
	/// This is a factory of sorts that kicks off the aggregate query builder.
	/// </summary>
    public static class RediSearchAggregateQuery
    {
		/// <summary>
		/// Builder method for specifying the index to create an aggregation query for.
		/// </summary>
		/// <param name="indexName">The name of the index to apply the aggregation.</param>
		/// <returns></returns>
        public static RediSearchAggregateQueryBuilder On(string indexName) =>
            new RediSearchAggregateQueryBuilder(indexName);
    }
}