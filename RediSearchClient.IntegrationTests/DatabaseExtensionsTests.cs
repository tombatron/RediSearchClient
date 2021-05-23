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
    }
}