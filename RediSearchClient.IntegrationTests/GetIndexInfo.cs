using System.Collections.Generic;
using RediSearchClient.Indexes;
using System.Threading.Tasks;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class GetIndexInfo : BaseIntegrationTest
    {
        protected override void Setup()
        {
            base.Setup();

            CreateTestInfoData();
        }

        [Fact]
        public async Task CanReturnAttributesAsync()
        {
            var result = await _db.GetInfoAsync(_indexName);

            Assert.NotNull(result.Attributes.GetValueOrDefault("first_name"));
        }

        private void CreateTestInfoData()
        {
            var index = RediSearchIndex
                .On(RediSearchStructure.HASH)
                .ForKeysWithPrefix($"{_recordPrefix}:")
                .WithSchema(x => x.Text("first_name"))
                .Build();

            _db.CreateIndex(_indexName, index);
        }
    }
}