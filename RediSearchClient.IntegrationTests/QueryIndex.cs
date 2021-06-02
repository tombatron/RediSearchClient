using RediSearchClient.Indexes;
using RediSearchClient.Query;
using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class QueryIndex : BaseIntegrationTest
    {
        public override void Setup()
        {
            base.Setup();

            CreateTestSearchData();
        }

        [Fact]
        public void CanCreateAndExecuteASimpleQuery()
        {
            var query = RediSearchQuery
                .On(_indexName)
                .UsingQuery("@first_name:Tom")
                .Build();

            var result = _db.Search(query);

            Assert.NotNull(result.RawResult);
        }

        private void CreateTestSearchData()
        {
            _db.HashSet($"{_recordPrefix}:1", new[] { new HashEntry("first_name", "Tom") });
            _db.HashSet($"{_recordPrefix}:2", new[] { new HashEntry("first_name", "Keith") });
            _db.HashSet($"{_recordPrefix}:3", new[] { new HashEntry("first_name", "Jason") });
            _db.HashSet($"{_recordPrefix}:4", new[] { new HashEntry("first_name", "James") });
            _db.HashSet($"{_recordPrefix}:5", new[] { new HashEntry("first_name", "Glen") });

            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix($"{_recordPrefix}:")
                .WithSchema(x => x.Text("first_name"))
                .Build();

            _db.CreateIndex(_indexName, index);
        }
    }
}