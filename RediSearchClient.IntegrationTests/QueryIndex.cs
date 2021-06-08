using System.Threading;
using System.Threading.Tasks;
using System.Linq;
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

        [Fact]
        public async Task CanCreateAndExecuteASimpleQueryAsync()
        {
            var query = RediSearchQuery
                .On(_indexName)
                .UsingQuery("@first_name:Tom")
                .Build();

            var result = await _db.SearchAsync(query);

            Assert.NotNull(result.RawResult);
        }        

        [Fact]
        public void CanParseQueryResult()
        {
            var query = RediSearchQuery
                .On(_indexName)
                .UsingQuery("*")
                .Build();

            var result = _db.Search(query);

            Assert.NotNull(result);
            Assert.NotNull(result.RawResult);
            Assert.Equal(5, result.RecordCount);

            var tom = result.FirstOrDefault(x => (string)x.Fields["first_name"] == "Tom");

            Assert.NotNull(tom);
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

            while(_db.GetInfo(_indexName).Indexing == 1)
            {
                Thread.Sleep(500); // Yeah I know...
            }
        }
    }
}