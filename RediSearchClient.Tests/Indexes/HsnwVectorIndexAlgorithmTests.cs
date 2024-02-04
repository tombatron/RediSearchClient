using RediSearchClient.Indexes;
using Xunit;

namespace RediSearchClient.Tests.Indexes
{
    public class HsnwVectorIndexAlgorithmTests
    {
        public class GenerateArguments
        {
            [Fact]
            public void WillReturnArrayWithEmpty0thIndex()
            {
                var hsnwAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, null, null, null, null, null);

                var args = hsnwAlgo.GenerateArguments();

                Assert.Null(args[0]);
            }

            [Fact]
            public void WillReturnArrayWithRequiredParameters()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT64, 20, DistanceMetric.IP, null, null, null, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("VECTOR", args[1]);
                Assert.Equal("HNSW", args[2]);
                Assert.Equal(6, args[3]);
            }

            [Theory]
            [InlineData(VectorType.FLOAT32, "FLOAT32")]
            [InlineData(VectorType.FLOAT64, "FLOAT64")]
            public void WillReturnVectorType(VectorType vectorType, string expectedVectorType)
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(vectorType, 20, DistanceMetric.IP, null, null, null, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("TYPE", args[4]);
                Assert.Equal(expectedVectorType, args[5]);
            }

            [Fact]
            public void WillReturnDimensions()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 123, DistanceMetric.IP, null, null, null, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("DIM", args[6]);
                Assert.Equal(123, args[7]);
            }

            [Theory]
            [InlineData(DistanceMetric.IP, "IP")]
            [InlineData(DistanceMetric.L2, "L2")]
            [InlineData(DistanceMetric.COSINE, "COSINE")]
            public void WillReturnDistanceMetric(DistanceMetric distanceMetric, string expectedDistanceMetric)
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 312, distanceMetric, null, null, null, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("DISTANCE_METRIC", args[8]);
                Assert.Equal(expectedDistanceMetric, args[9]);
            }

            [Fact]
            public void WillReturnInitialCap()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 312, DistanceMetric.IP, initialCap: 123, null, null, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("INITIAL_CAP", args[10]);
                Assert.Equal(123, args[11]);
            }

            [Fact]
            public void WillReturnM()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 312, DistanceMetric.IP, null, m: 16, null, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("M", args[10]);
                Assert.Equal(16, args[11]);
            }

            [Fact]
            public void WillReturnEfConstruction()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 312, DistanceMetric.IP, null, null, efConstruction: 32, null, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("EF_CONSTRUCTION", args[10]);
                Assert.Equal(32, args[11]);
            }

            [Fact]
            public void WillReturnEfRuntime()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 312, DistanceMetric.IP, null, null, null, efRuntime: 10, null);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("EF_RUNTIME", args[10]);
                Assert.Equal(10, args[11]);
            }

            [Fact]
            public void WillReturnEpsilon()
            {
                var hnswAlgo = new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 312, DistanceMetric.IP, null, null, null, null, epsilon: 0.01f);

                var args = hnswAlgo.GenerateArguments();

                Assert.Equal("EPSILON", args[10]);
                Assert.Equal(0.01f, args[11]);
            }

            [Theory]
            [MemberData(nameof(HnswIndexAlgorithmInstances))]
            public void WillReturnCorrectArgumentCount(VectorIndexAlgorithm algo, int expectedCount)
            {
                var args = algo.GenerateArguments();

                Assert.Equal(expectedCount, args[3]);
            }

            public static object[][] HnswIndexAlgorithmInstances
            {
                get
                {
                    return new object[][] {
                        new object[]
                        {
                            new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, null, null, null, null, null),
                            6
                        },

                        new object[]
                        {
                            new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, null, null, null, null),
                            8
                        },

                        new object[]
                        {
                            new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, 200, null, null, null),
                            10
                        },

                        new object[]
                        {
                            new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, 200, 300, null, null),
                            12
                        },

                        new object[]
                        {
                            new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, 200, 300, 10, null),
                            14
                        },

                        new object[]
                        {
                            new HnswVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, 200, 300, 10, 0.01f),
                            16
                        },
                    };
                }
            }
        }
    }
}
