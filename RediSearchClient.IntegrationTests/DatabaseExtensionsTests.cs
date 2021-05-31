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

        public class AddSuggestionWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _dictionaryName;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _dictionaryName = Guid.NewGuid().ToString("n");
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void AddASuggestionToTheIndex()
            {
                var firstResult = _db.AddSuggestion(_dictionaryName, "hello", 1);
                var secondResult = _db.AddSuggestion(_dictionaryName, "goodnight", 1);

                Assert.Equal(1, firstResult);
                Assert.Equal(2, secondResult);
            }

            [Fact]
            public void AddASuggestionToTheIndexWithPayload()
            {
                var result = _db.AddSuggestion(_dictionaryName, "whoa", 1, payload: "hey there");
            }
        }

        public class GetSuggestionWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _dictionaryName;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _dictionaryName = Guid.NewGuid().ToString("n");

                SetupSuggestions();
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void ReturnSuggestions()
            {
                var suggestions = _db.GetSuggestions(_dictionaryName, "he");

                Assert.Equal(3, suggestions.Length);
            }

            [Fact]
            public void ReturnSuggestionsWithScores()
            {
                var suggestions = _db.GetSuggestions(_dictionaryName, "he", withScores: true);

                var hasScores = suggestions.All(x => x.Score > 0);

                Assert.Equal(3, suggestions.Length);
                Assert.True(hasScores);
            }

            [Fact]
            public void ReturnSuggestionsWithPayloads()
            {
                var suggestions = _db.GetSuggestions(_dictionaryName, "he", withPayloads: true);

                var hasPayloads = suggestions.All(x => !string.IsNullOrEmpty(x.Payload));

                Assert.Equal(3, suggestions.Length);
                Assert.True(hasPayloads);
            }

            [Fact]
            public void ReturnSuggestionsWithScoresAndPayloads()
            {
                var suggestions = _db.GetSuggestions(_dictionaryName, "he", withScores: true, withPayloads: true);

                var hasScores = suggestions.All(x => x.Score > 0);
                var hasPayloads = suggestions.All(x => !string.IsNullOrEmpty(x.Payload));

                Assert.Equal(3, suggestions.Length);
                Assert.True(hasScores);
                Assert.True(hasPayloads);
            }

            private void SetupSuggestions()
            {
                _db.AddSuggestion(_dictionaryName, "hello world", 2, false, "this is hello world's payload");
                _db.AddSuggestion(_dictionaryName, "hello tom", 2, false, "this is hello tom's payload");
                _db.AddSuggestion(_dictionaryName, "hey joe", 2, false, "this is hey joe's payload");
            }
        }

        public class DeleteSuggestionWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _dictionaryName;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _dictionaryName = Guid.NewGuid().ToString("n");

                SetupSuggestions();
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void RemoveSuggestionIfItExists()
            {
                var result = _db.DeleteSuggestion(_dictionaryName, "hello world");

                Assert.True(result);
            }

            [Fact]
            public void NotRemoveSuggestionIfItDoesntExist()
            {
                var result = _db.DeleteSuggestion(_dictionaryName, "hello lucas");

                Assert.False(result);
            }

            private void SetupSuggestions()
            {
                _db.AddSuggestion(_dictionaryName, "hello world", 2, false, "this is hello world's payload");
                _db.AddSuggestion(_dictionaryName, "hello tom", 2, false, "this is hello tom's payload");
                _db.AddSuggestion(_dictionaryName, "hey joe", 2, false, "this is hey joe's payload");
            }
        }

        public class SuggestionSizeWill : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _dictionaryName;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _dictionaryName = Guid.NewGuid().ToString("n");

                SetupSuggestions();
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void ReturnLengthOfSuggestionDictionary()
            {
                var result = _db.SuggestionsSize(_dictionaryName);

                Assert.Equal(3, result);
            }

            private void SetupSuggestions()
            {
                _db.AddSuggestion(_dictionaryName, "hello world", 2, false, "this is hello world's payload");
                _db.AddSuggestion(_dictionaryName, "hello tom", 2, false, "this is hello tom's payload");
                _db.AddSuggestion(_dictionaryName, "hey joe", 2, false, "this is hey joe's payload");
            }
        }

        public class SynonymsManagement : BaseIntegrationTest
        {
            private ConnectionMultiplexer _muxr;
            private IDatabase _db;
            private string _indexName;
            private string _synonymGroupId;

            public override void Setup()
            {
                _muxr = ConnectionMultiplexer.Connect("localhost");

                _db = _muxr.GetDatabase(0);

                _indexName = Guid.NewGuid().ToString("n");

                _synonymGroupId = Guid.NewGuid().ToString("n");

                CreateSynonyms();
            }

            public override void TearDown()
            {
                _muxr.Dispose();
            }

            [Fact]
            public void Works()
            {
                var synonyms = _db.DumpSynonyms(_indexName);

                Assert.NotNull(synonyms);
            }

            private void CreateSynonyms()
            {
                var index = RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix($"this_doesnt_matter:")
                    .WithSchema(
                        x => x.Tag("city"),
                        x => x.Tag("state")
                    )
                    .Build();

                _db.CreateIndex(_indexName, index);

                _db.UpdateSynonyms(_indexName, _synonymGroupId, 
                    ("poop", "dookie"), 
                    ("pee pee", "tink tink"),
                    ("dog", "dag"),
                    ("toliet", "head"),
                    ("child", "kid")
                );
            }
        }
    }
}