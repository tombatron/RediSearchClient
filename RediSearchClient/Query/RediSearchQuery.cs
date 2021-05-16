namespace RediSearchClient.Query
{
    public static class RediSearchQuery
    {
        public static RediSearchQueryBuilder On(string indexName) =>
            new RediSearchQueryBuilder(indexName);
    }
}