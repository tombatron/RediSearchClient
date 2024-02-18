using RediSearchClient.Query;
using Xunit;

namespace RediSearchClient.IntegrationTests;

public class VectorQueryIndex
{
    public class KnnQuery : BaseIntegrationTest
    {
        protected override void Setup()
        {
            base.Setup();
        }

        [Fact]
        public void CanExecuteSimpleQuery()
        {
            var knnQuery = RediSearchQuery
                .On(HashVectorIndexName)
                    .VectorKnn()
                        .FieldName("feature_embeddings")
                        .ScoreFieldName("score")
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .Return(r =>
                        {
                            r.Field("name", "Name");
                            r.Field("score");
                        })
                    .Build();

            var result = _db.Search(knnQuery);

            Assert.NotNull(result);
        }
    }

    public class RangeQuery : BaseIntegrationTest
    {
        [Fact]
        public void CanExecuteSimpleQuery()
        {
            var rangeQuery = RediSearchQuery
                .On(HashVectorIndexName)
                    .VectorRange()
                        .FieldName("feature_embeddings")
                        .Range(0.5f)
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .ScoreFieldName("distance")
                        .Epsilon(0.5f)
                        .Dialect(2)
                        .SortBy("distance")
                    .Build();
        }
    }
}
