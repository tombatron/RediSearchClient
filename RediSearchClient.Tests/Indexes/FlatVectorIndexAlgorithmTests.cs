using RediSearchClient.Indexes;
using Xunit;

namespace RediSearchClient.Tests.Indexes
{
    public class FlatVectorIndexAlgorithmTests
    {
        public class GenerateArguments
        {
            [Fact]
            public void WillReturnArrayWithEmpty0thIndex()
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 100, DistanceMetric.COSINE, null, null);

                var args = flatAlgo.GenerateArguments();

                Assert.Null(args[0]);
            }

            [Fact]
            public void WillReturnArrayWithRequiredParameters()
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 100, DistanceMetric.IP, null, null);

                var args = flatAlgo.GenerateArguments();

                Assert.Equal("VECTOR", args[1]);
                Assert.Equal("FLAT", args[2]);
                Assert.Equal(6, args[3]);
            }

            [Theory]
            [InlineData(VectorType.FLOAT32, "FLOAT32")]
            [InlineData(VectorType.FLOAT64, "FLOAT64")]
            public void WillReturnVectorType(VectorType vectorType, string expectedVectorType)
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(vectorType, 50, DistanceMetric.L2, null, null);

                var args = flatAlgo.GenerateArguments();

                Assert.Equal("TYPE", args[4]);
                Assert.Equal(expectedVectorType, args[5]);
            }

            [Fact]
            public void WillReturnDimensions()
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 345, DistanceMetric.IP, null, null);

                var args = flatAlgo.GenerateArguments();

                Assert.Equal("DIM", args[6]);
                Assert.Equal(345, args[7]);
            }

            [Theory]
            [InlineData(DistanceMetric.IP, "IP")]
            [InlineData(DistanceMetric.L2, "L2")]
            [InlineData(DistanceMetric.COSINE, "COSINE")]
            public void WillReturnDistanceMetric(DistanceMetric distanceMetric, string expectedDistanceMetric)
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 234, distanceMetric, null, null);

                var args = flatAlgo.GenerateArguments();

                Assert.Equal("DISTANCE_METRIC", args[8]);
                Assert.Equal(expectedDistanceMetric, args[9]);
            }

            [Fact]
            public void WillReturnInitialCap()
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.L2, initialCap: 10, null);

                var args = flatAlgo.GenerateArguments();

                Assert.Equal("INITIAL_CAP", args[10]);
                Assert.Equal(10, args[11]);
            }

            [Fact]
            public void WillReturnBlockSize()
            {
                var flatAlgo = new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.L2, null, blockSize: 1_200);

                var args = flatAlgo.GenerateArguments();

                Assert.Equal("BLOCK_SIZE", args[10]);
                Assert.Equal(1_200, args[11]);
            }

            [Theory]
            [MemberData(nameof(FlatIndexAlgorithmInstances))]
            public void WillReturnCorrectArgumentCount(VectorIndexAlgorithm algo, int expectedCount)
            {
                var args = algo.GenerateArguments();

                Assert.Equal(expectedCount, args[3]);
            }

            public static object[][] FlatIndexAlgorithmInstances
            {
                get
                {
                    return new object[][] {
                        new object[]
                        {
                            new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, null, null),
                            6
                        },

                        new object[]
                        {
                            new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, null),
                            8
                        },

                        new object[]
                        {
                            new FlatVectorIndexAlgorithm(VectorType.FLOAT32, 10, DistanceMetric.IP, 100, 200),
                            10
                        },
                    };
                }
            }
        }
    }
}
