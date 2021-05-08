namespace RediSearchClient.Indexes
{
public static class RediSearchIndex
{
	public static RediSearchIndexBuilder On(RediSearchStructure structure) =>
		new RediSearchIndexBuilder(structure);
}
}