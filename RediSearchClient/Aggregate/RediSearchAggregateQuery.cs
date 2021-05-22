namespace RediSearchClient.Aggregate
{
public static class RediSearchAggregateQuery
{
	public static RediSearchAggregateQueryBuilder On(string indexName) =>
		new RediSearchAggregateQueryBuilder(indexName);
}
}