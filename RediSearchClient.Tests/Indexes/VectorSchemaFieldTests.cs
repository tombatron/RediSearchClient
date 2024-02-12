using RediSearchClient.Indexes;
using Xunit;

namespace RediSearchClient.Tests.Indexes
{
    public class VectorSchemaFieldTests
    {
        public class ConstructingWithHnsw
        {
            [Fact]
            public void WillReturnAppropriateArguments()
            {
                var vectorField = new VectorSchemaField("test_hnsw",
                    VectorIndexAlgorithm.HNSW(
                        type: VectorType.FLOAT32,
                        dimensions: 32,
                        distanceMetric: DistanceMetric.COSINE,

                        initialCap: 34,
                        m: 3,
                        efConstruction: 12,
                        efRuntime: 22,
                        epsilon: 1.2f
                    ));

                var arguments = vectorField.FieldArguments;

                Assert.Equal("test_hnsw", arguments[0]);
                Assert.Equal("VECTOR", arguments[1]);
                Assert.Equal("HNSW", arguments[2]);
                Assert.Equal(16, arguments[3]);
                Assert.Equal("TYPE", arguments[4]);
                Assert.Equal("FLOAT32", arguments[5]);
                Assert.Equal("DIM", arguments[6]);
                Assert.Equal(32, arguments[7]);
                Assert.Equal("DISTANCE_METRIC", arguments[8]);
                Assert.Equal("COSINE", arguments[9]);
                Assert.Equal("INITIAL_CAP", arguments[10]);
                Assert.Equal(34, arguments[11]);
                Assert.Equal("M", arguments[12]);
                Assert.Equal(3, arguments[13]);
                Assert.Equal("EF_CONSTRUCTION", arguments[14]);
                Assert.Equal(12, arguments[15]);
                Assert.Equal("EF_RUNTIME", arguments[16]);
                Assert.Equal(22, arguments[17]);
                Assert.Equal("EPSILON", arguments[18]);
                Assert.Equal(1.2F, arguments[19]);
            }

            [Fact]
            public void WillReturnAppropriateArgumentsWithAlias()
            {
                var vectorField = new VectorSchemaField("test_hnsw", alias: "test_alias",
                    VectorIndexAlgorithm.HNSW(
                        type: VectorType.FLOAT32,
                        dimensions: 32,
                        distanceMetric: DistanceMetric.COSINE,

                        initialCap: 34,
                        m: 3,
                        efConstruction: 12,
                        efRuntime: 22,
                        epsilon: 1.2f
                    ));

                var arguments = vectorField.FieldArguments;

                Assert.Equal("test_hnsw", arguments[0]);
                Assert.Equal("AS", arguments[1]);
                Assert.Equal("test_alias", arguments[2]);
            }
        }

        public class ConstructingWithFlat
        {
            [Fact]
            public void WillReturnAppropriateArguments()
            {
                var vectorField = new VectorSchemaField("test_flat",
                    VectorIndexAlgorithm.FLAT(
                        type: VectorType.FLOAT64,
                        dimensions: 32,
                        distanceMetric: DistanceMetric.L2,

                        initialCap: 20,
                        blockSize: 50
                    ));

                var arguments = vectorField.FieldArguments;

                Assert.Null(arguments);
            }

            [Fact]
            public void WillReturnAppropriateArgumentsWithAlias()
            {

            }
        }
    }
}
