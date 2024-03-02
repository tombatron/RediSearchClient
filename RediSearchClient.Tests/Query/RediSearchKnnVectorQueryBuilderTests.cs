using RediSearchClient.Query;
using Xunit;

namespace RediSearchClient.Tests.Query;

public class RediSearchKnnVectorQueryBuilderTests
{
    [Fact]
    public void CanBuildQuery()
    {
        var byteArray = new byte[] { 1, 2, 3, 4, 5 };

        var query = RediSearchQuery
            .On("index_name")
                .VectorKnn()
                    .Prefilter("@category=='test'")
                    .NumberOfNeighbors(10)
                    .FieldName("feature_embeddings")
                    .ScoreFieldName("score")
                    .Vector(byteArray)
                    .Limit(15)
                    .Dialect(2)
                    .SortBy("score")
                    .EfRuntime(10)
                    .Epsilon(0.01f)
                    .Return(r => 
                    {
                        r.Field("name", "Name");
                        r.Field("score", alias: "Score");
                    })
                .Build();

        Assert.Equal("index_name", query.Fields[0]);
        Assert.Equal("(@category=='test')=>[KNN 10 @feature_embeddings $BLOB]=>{$YIELD_DISTANCE_AS: score; $EF_RUNTIME: $ef_runtime; $EPSILON: $epsilon;}", query.Fields[1]);
        Assert.Equal("PARAMS", query.Fields[2]);
        Assert.Equal(6, query.Fields[3]);
        Assert.Equal("BLOB", query.Fields[4]);
        Assert.Equal(byteArray, query.Fields[5]);
        Assert.Equal("ef_runtime", query.Fields[6]);
        Assert.Equal(10, query.Fields[7]);
        Assert.Equal("epsilon", query.Fields[8]);
        Assert.Equal(0.01f, query.Fields[9]);
        Assert.Equal("SORTBY", query.Fields[10]);
        Assert.Equal("score", query.Fields[11]);
        Assert.Equal("ASC", query.Fields[12]);
        Assert.Equal("LIMIT", query.Fields[13]);
        Assert.Equal(0, query.Fields[14]);
        Assert.Equal(15, query.Fields[15]);
        Assert.Equal("RETURN", query.Fields[16]);
        Assert.Equal(6, query.Fields[17]);
        Assert.Equal("name", query.Fields[18]);
        Assert.Equal("AS", query.Fields[19]);
        Assert.Equal("Name", query.Fields[20]);
        Assert.Equal("score", query.Fields[21]);
        Assert.Equal("AS", query.Fields[22]);
        Assert.Equal("Score", query.Fields[23]);
        Assert.Equal("DIALECT", query.Fields[24]);
        Assert.Equal(2, query.Fields[25]);
    }
}
