using RediSearchClient.Query;
using System.Linq;
using Xunit;

namespace RediSearchClient.IntegrationTests;

public class VectorQueryIndex
{
    public class SimpleSearchResult
    {
        public string Name { get; set; }
        public double Score { get; set; }
    }

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
                        .ScoreFieldName("Score")
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .Return(r =>
                        {
                            r.Field("name", "Name");
                            r.Field("Score"); // Can't alias the aliased score field.
                        })
                    .Build();

            var result = _db.Search(knnQuery).As<SimpleSearchResult>();

            Assert.Equal(2, result.Count());

            // We haven't specified a sort order, but we want to make sure that the
            // expected results are here. 
            Assert.Contains(result, r => r.Name == "baby");
            Assert.Contains(result, r => r.Name == "not_baby");
        }

        [Fact]
        public void CanExecuteSortedSimpleQuery()
        {
            var knnQuery = RediSearchQuery
                .On(HashVectorIndexName)
                    .VectorKnn()
                        .FieldName("feature_embeddings")
                        .ScoreFieldName("Score")
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .Return(r =>
                        {
                            r.Field("name", "Name");
                            r.Field("Score"); // Can't alias the aliased score field.
                        })
                        .SortByDistance(sortByDistanceAcending: true) // Best result first!
                    .Build();

            var result = _db.Search(knnQuery).As<SimpleSearchResult>();

            Assert.Equal("baby", result.First().Name);
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
                        .ScoreFieldName("Score")
                        .Epsilon(0.5f)
                        .Return(r =>
                        {
                            r.Field("name", "Name");
                            r.Field("Score");
                        })
                    .Build();

            var result = _db.Search(rangeQuery).As<SimpleSearchResult>();

            Assert.Equal(2, result.Count());

            // We haven't specified a sort order, but we want to make sure that the
            // expected results are here. 
            Assert.Contains(result, r => r.Name == "baby");
            Assert.Contains(result, r => r.Name == "not_baby");
        }

        [Fact]
        public void CanExecuteSortedSimpleQuery()
        {
            var rangeQuery = RediSearchQuery
                .On(HashVectorIndexName)
                    .VectorRange()
                        .FieldName("feature_embeddings")
                        .Range(0.5f)
                        .Vector(SampleData.SampleVectorData[0].FileBytes)
                        .ScoreFieldName("Score")
                        .Epsilon(0.5f)
                        .Return(r =>
                        {
                            r.Field("name", "Name");
                            r.Field("Score");
                        })
                        .SortBy("Score", sortByAscending: true)
                    .Build();

            var result = _db.Search(rangeQuery).As<SimpleSearchResult>();

            Assert.Equal("baby", result.First().Name);
        }
    }
}
