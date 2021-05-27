using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using StackExchange.Redis;
using RediSearchClient.Indexes;

namespace RediSearchClient.IntegrationTests
{
    public class DatabaseExtensionsTests
    {
        public class AlterSchemaWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _indexName;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _indexName = Guid.NewGuid().ToString("n");
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void AlterExistingSchema()
            {
                var indexDefinition = RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix("Bogus::")
                    .WithSchema(x => x.Text("field_one"))
                    .Build();

                _db.CreateIndex(_indexName, indexDefinition);

                var indexInfo = _db.Execute("FT.INFO", _indexName);

                var fields = GetSchemaFields(indexInfo).ToList();

                Assert.Equal(1, fields.Count);
                Assert.Contains("field_one", fields);

                _db.AlterSchema(_indexName, f => f.Text("field_two"));

                indexInfo = _db.Execute("FT.INFO", _indexName);

                fields = GetSchemaFields(indexInfo).ToList();

                Assert.Equal(2, fields.Count);
                Assert.Contains("field_two", fields);
            }

            private static IEnumerable<string> GetSchemaFields(RedisResult rawResult)
            {
                var results = (RedisResult[])rawResult;
                var fields = (RedisResult[])results[7];

                foreach (var field in fields)
                {
                    var props = (RedisResult[])field;

                    yield return props[0].ToString();
                }

                yield break;
            }
        }

        public class DropIndexWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _indexName;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _indexName = Guid.NewGuid().ToString("n");
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void DropTheIndex()
            {
                var indexDefinition = RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix("Whatever::*")
                    .WithSchema(s => s.Text("Hello"))
                    .Build();

                _db.CreateIndex(_indexName, indexDefinition);

                var indexes = ((RedisResult[])_db.Execute("FT._LIST")).Select(x => x.ToString());

                Assert.Contains(_indexName, indexes);

                _db.DropIndex(_indexName);

                indexes = ((RedisResult[])_db.Execute("FT._LIST")).Select(x => x.ToString());

                Assert.DoesNotContain(_indexName, indexes);
            }
        }

        public class TagValuesWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _indexName;
            private string _recordPrefix;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _indexName = Guid.NewGuid().ToString("n");
                _recordPrefix = Guid.NewGuid().ToString("n");

                CreateTestSearchData();
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void WillReturnAnIndexesTags()
            {
                var tags = _db.TagValues(_indexName, "city");

                Assert.Equal(5, tags.Length);
                Assert.Contains("pensacola", tags);
            }

            [Fact]
            public void WillReturnEmptyArrayWhenMissingFields()
            {
                var tags = _db.TagValues(_indexName, "state");

                Assert.Equal(0, tags.Length);
            }

            private void CreateTestSearchData()
            {
                _db.HashSet($"{_recordPrefix}:1", new[]
                {
                    new HashEntry("city", "Pensacola")
                });

                _db.HashSet($"{_recordPrefix}:2", new[]
                {
                    new HashEntry("city", "Mobile")
                });

                _db.HashSet($"{_recordPrefix}:3", new[]
                {
                    new HashEntry("city", "Murphy")
                });

                _db.HashSet($"{_recordPrefix}:4", new[]
                {
                    new HashEntry("city", "Tampa")
                });

                _db.HashSet($"{_recordPrefix}:5", new[]
                {
                    new HashEntry("city", "Key West")
                });

                var index = RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix($"{_recordPrefix}:")
                    .WithSchema(
                        x => x.Tag("city"),
                        x => x.Tag("state")
                    )
                    .Build();

                _db.CreateIndex(_indexName, index);
            }
        }
    }
}