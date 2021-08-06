using System.Linq;
using StackExchange.Redis;
using Xunit;

namespace RediSearchClient.Tests
{
    public class ResultMapperTests
    {
        [Fact]
        public void CanCreateResultMap()
        {
            ResultMapper<DemoClass>.CreateMap()
                .ForMember(x => x.BoolProperty)
                .ForMember(x => x.StringArrayProperty, r => ((string) r).Split(","))
                .ForMember("StringThatIsArray", d => d.StringThatIsAnArrayProperty, r => ((string) r).Split(","))
                .ForMember("IntegerProperty", p => p.IntProperty);

            var searchResult = CreateFakeSearchResult();

            var mappedResult = searchResult.As<DemoClass>().FirstOrDefault();

            Assert.NotNull(mappedResult);

            Assert.True(mappedResult.BoolProperty);
            Assert.Equal(new[] {"1", "2", "3", "4"}, mappedResult.StringArrayProperty);
            Assert.Equal(new[] {"1", "2", "3", "4"}, mappedResult.StringArrayProperty);
            Assert.Equal(123, mappedResult.IntProperty);
        }

        private static SearchResult CreateFakeSearchResult()
        {
            var rawResult = RedisResult.Create(new[]
            {
                RedisResult.Create(1, ResultType.Integer),

                RedisResult.Create("this_is_the_document_key", ResultType.BulkString),

                RedisResult.Create(new[]
                {
                    RedisResult.Create("BoolProperty", ResultType.BulkString),
                    RedisResult.Create(true, ResultType.BulkString),

                    RedisResult.Create("ByteArrayProperty", ResultType.BulkString),
                    RedisResult.Create(new byte[] {1, 2}, ResultType.BulkString),

                    RedisResult.Create("DoubleProperty", ResultType.BulkString),
                    RedisResult.Create((double) 1, ResultType.BulkString),

                    RedisResult.Create("IntProperty", ResultType.BulkString),
                    RedisResult.Create(3, ResultType.BulkString),

                    RedisResult.Create("IntegerProperty", ResultType.BulkString),
                    RedisResult.Create(123, ResultType.BulkString),

                    RedisResult.Create("LongProperty", ResultType.BulkString),
                    RedisResult.Create(4L, ResultType.BulkString),

                    RedisResult.Create("ULongProperty", ResultType.BulkString),
                    RedisResult.Create((ulong) 5, ResultType.BulkString),

                    RedisResult.Create("NullableBoolProperty", ResultType.BulkString),
                    RedisResult.Create(true, ResultType.BulkString),

                    RedisResult.Create("NullableDoubleProperty", ResultType.BulkString),
                    RedisResult.Create((double) 234, ResultType.BulkString),

                    RedisResult.Create("NullableIntProperty", ResultType.BulkString),
                    RedisResult.Create(63, ResultType.BulkString),

                    RedisResult.Create("NullableLongProperty", ResultType.BulkString),
                    RedisResult.Create(56L, ResultType.BulkString),

                    RedisResult.Create("NullableULongProperty", ResultType.BulkString),
                    RedisResult.Create((ulong) 888, ResultType.BulkString),

                    RedisResult.Create("StringProperty", ResultType.BulkString),
                    RedisResult.Create("string value", ResultType.BulkString),

                    RedisResult.Create("StringArrayProperty", ResultType.BulkString),
                    RedisResult.Create("1,2,3,4", ResultType.BulkString),

                    RedisResult.Create("StringThatIsArray", ResultType.BulkString),
                    RedisResult.Create("1,2,3,4", ResultType.BulkString)
                })
            });

            return SearchResult.From(rawResult);
        }

        private static AggregateResult CreateFakeAggregateResult()
        {
            var rawResult = RedisResult.Create(new[]
            {
                RedisResult.Create(1, ResultType.Integer),

                RedisResult.Create(new[]
                {
                    RedisResult.Create("ag_field1", ResultType.BulkString),
                    RedisResult.Create("total_things", ResultType.BulkString),

                    RedisResult.Create("ag_field2", ResultType.BulkString),
                    RedisResult.Create("128", ResultType.BulkString),
                })
            });

            return AggregateResult.From(rawResult);
        }
    }

    public class DemoClass
    {
        public bool BoolProperty { get; set; }

        public bool[] BoolArrayProperty { get; set; }

        public byte[] ByteArrayProperty { get; set; }

        public byte[][] ByteArrayArrayProperty { get; set; }

        public double DoubleProperty { get; set; }

        public double[] DoubleArrayProperty { get; set; }

        public int IntProperty { get; set; }

        public int[] IntArrayProperty { get; set; }

        public long LongProperty { get; set; }

        public long[] LongArrayProperty { get; set; }

        public ulong ULongProperty { get; set; }

        public ulong[] ULongArrayProperty { get; set; }

        public bool? NullableBoolProperty { get; set; }

        public double? NullableDoubleProperty { get; set; }

        public int? NullableIntProperty { get; set; }

        public long? NullableLongProperty { get; set; }

        public ulong? NullableULongProperty { get; set; }

        public string StringProperty { get; set; }

        public string[] StringArrayProperty { get; set; }

        public string[] StringThatIsAnArrayProperty { get; set; }
    }
}