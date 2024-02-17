using RediSearchClient.Query;
using Xunit;

namespace RediSearchClient.Tests.Query;

public class RediSearchRangeVectorQueryBuilderTests
{
    [Fact]
    public void CanBuildQuery()
    {
        var byteArray = new byte[] { 1, 2, 3, 4, 5 };

        var query = RediSearchQuery
            .On("index_name")
                .VectorRange()
                    .FieldName("feature_embeddings")
                    .Range(0.5f)
                    .Vector(byteArray)
                    .DistanceFieldName("distance")
                    .Epsilon(0.5f)
                    .SortBy("distance")
                    .Limit(10)
                    .Dialect(2)
                .Build();

        Assert.Equal("index_name", query.Fields[0]);
        Assert.Equal("@feature_embeddings:[VECTOR_RANGE $r $BLOB]=>{$EPSILON:0.5; $YIELD_DISTANCE_AS: distance", query.Fields[1]);
        Assert.Equal("PARAMS", query.Fields[2]);
        Assert.Equal(4, query.Fields[3]);
        Assert.Equal("r", query.Fields[4]);
        Assert.Equal(0.5f, query.Fields[5]);
        Assert.Equal("BLOB", query.Fields[6]);
        Assert.Equal(byteArray, query.Fields[7]);
        Assert.Equal("SORTBY", query.Fields[8]);
        Assert.Equal("distance", query.Fields[9]);
        Assert.Equal("ASC", query.Fields[10]);
        Assert.Equal("LIMIT", query.Fields[11]);
        Assert.Equal(0, query.Fields[12]);
        Assert.Equal(10, query.Fields[13]);
        Assert.Equal("DIALECT", query.Fields[14]);
        Assert.Equal(2, query.Fields[15]);
    }
}
