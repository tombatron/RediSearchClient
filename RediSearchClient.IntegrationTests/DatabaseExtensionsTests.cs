using RediSearchClient.Exceptions;
using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RediSearchClient.IntegrationTests
{
    public class DatabaseExtensionsTests
    {
        public class AlterSchemaWill : BaseIntegrationTest
        {
            [Fact]
            public void AlterExistingSchema()
            {
                var indexDefinition = CreateSampleIndex();

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

            [Fact]
            public async Task AlterExistingSchemaAsync()
            {
                var indexDefinition = CreateSampleIndex();
                var indexName = $"{_indexName}_async";

                await _db.CreateIndexAsync(indexName, indexDefinition);

                var indexInfo = await _db.ExecuteAsync("FT.INFO", indexName);

                var fields = GetSchemaFields(indexInfo).ToList();

                Assert.Equal(1, fields.Count);
                Assert.Contains("field_one", fields);

                await _db.AlterSchemaAsync(indexName, f => f.Text("field_two"));

                indexInfo = await _db.ExecuteAsync("FT.INFO", indexName);

                fields = GetSchemaFields(indexInfo).ToList();

                Assert.Equal(2, fields.Count);
                Assert.Contains("field_two", fields);
            }

            private RediSearchIndexDefinition CreateSampleIndex()
            {
                return RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix("Bogus::")
                    .WithSchema(x => x.Text("field_one"))
                    .Build();
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
            [Fact]
            public void DropTheIndex()
            {
                var indexDefinition = CreateSimpleIndexDefinition();

                _db.CreateIndex(_indexName, indexDefinition);

                var indexes = ((RedisResult[])_db.Execute("FT._LIST")).Select(x => x.ToString());

                Assert.Contains(_indexName, indexes);

                _db.DropIndex(_indexName);

                indexes = ((RedisResult[])_db.Execute("FT._LIST")).Select(x => x.ToString());

                Assert.DoesNotContain(_indexName, indexes);
            }

            [Fact]
            public async Task DropTheIndexAsync()
            {
                var indexDefinition = CreateSimpleIndexDefinition();
                var indexName = $"{_indexName}_async";

                await _db.CreateIndexAsync(indexName, indexDefinition);

                var indexes = ((RedisResult[])(await _db.ExecuteAsync("FT._LIST"))).Select(x => x.ToString());

                Assert.Contains(indexName, indexes);

                await _db.DropIndexAsync(indexName);

                indexes = ((RedisResult[])(await _db.ExecuteAsync("FT._LIST"))).Select(x => x.ToString());

                Assert.DoesNotContain(indexName, indexes);
            }

            private RediSearchIndexDefinition CreateSimpleIndexDefinition()
            {
                return RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix("Whatever::*")
                    .WithSchema(s => s.Text("Hello"))
                    .Build();
            }
        }

        public class AliasCommands : BaseIntegrationTest
        {
            public override void Setup()
            {
                base.Setup();

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

            [Fact]
            public void CanAddAlias()
            {
                var alias = Guid.NewGuid().ToString("n");

                var success = _db.AddAlias(alias, _indexName);

                Assert.True(success);
            }

            [Fact]
            public async Task CanAddAliasAsync()
            {
                var alias = Guid.NewGuid().ToString("n");
                
                var success = await _db.AddAliasAsync(alias, _indexName);

                Assert.True(success);
            }

            [Fact]
            public void CanUpdateAlias()
            {
                var alias = Guid.NewGuid().ToString("n");

                var success = _db.UpdateAlias(alias, _indexName);

                Assert.True(success);
            }

            [Fact]
            public async Task CanUpdateAliasAsync()
            {
                var alias = Guid.NewGuid().ToString("n");

                var success = await _db.UpdateAliasAsync(alias, _indexName);

                Assert.True(success);
            }

            [Fact]
            public void CanDeleteAlias()
            {
                var alias = Guid.NewGuid().ToString("n");

                _db.AddAlias(alias, _indexName);

                var success = _db.DeleteAlias(alias);

                Assert.True(success);
            }

            [Fact]
            public async Task CanDeleteAliasAsync()
            {
                var alias = Guid.NewGuid().ToString("n");

                _db.AddAlias(alias, _indexName);

                var success = _db.DeleteAlias(alias);

                Assert.True(success);
            }
        }
 
        public class TagValuesWill : BaseIntegrationTest
        {
            public override void Setup()
            {
                base.Setup();

                CreateTestSearchData();
            }

            [Fact]
            public void WillReturnAnIndexsTags()
            {
                var tags = _db.TagValues(_indexName, "city");

                Assert.Equal(5, tags.Length);
                Assert.Contains("pensacola", tags);
            }

            [Fact]
            public async Task WillReturnAnIndexsTagsAsync()
            {
                var tags = await _db.TagValuesAsync(_indexName, "city");

                Assert.Equal(5, tags.Length);
                Assert.Contains("pensacola", tags);
            }

            [Fact]
            public void WillReturnEmptyArrayWhenMissingFields()
            {
                var tags = _db.TagValues(_indexName, "state");

                Assert.Equal(0, tags.Length);
            }

            [Fact]
            public async Task WillReturnEmptyArrayWhenMissingFieldsAsync()
            {
                var tags = await _db.TagValuesAsync(_indexName, "state");

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

                while (_db.GetInfo(_indexName).Indexing == 1)
                {
                    Thread.Sleep(500);
                }
            }
        }

        public class AddSuggestionWill : BaseIntegrationTest
        {
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
            public override void Setup()
            {
                base.Setup();

                SetupSuggestions();
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
            public override void Setup()
            {
                base.Setup();

                SetupSuggestions();
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
            public override void Setup()
            {
                base.Setup();

                SetupSuggestions();
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
            private string _synonymGroupId;

            public override void Setup()
            {
                base.Setup();

                _synonymGroupId = Guid.NewGuid().ToString("n");

                CreateSynonyms();
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

        public class GetInfoWill : BaseIntegrationTest
        {
            public override void Setup()
            {
                base.Setup();

                SetupTestIndex();
            }

            [Fact]
            public void ReturnInfoAboutAnIndex()
            {
                var info = _db.GetInfo(_indexName);

                Assert.Equal(_indexName, info.IndexName);

                Assert.Contains("NOFREQS", info.IndexOptions);
                Assert.Contains("NOOFFSETS", info.IndexOptions);
            }

            private void SetupTestIndex()
            {
                var index = RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix("Whoa::")
                    .WithSchema
                    (
                        x => x.Geo("whereami"),
                        x => x.Numeric("numberz"),
                        x => x.Tag("tag_field", separator: "|"),
                        x => x.Text("wurds", sortable: true, nostem: true, noindex: false, phonetic: Language.French, weight: 2)
                    )
                    .NoFrequencies()
                    .NoOffsets()
                    .Build();

                _db.CreateIndex(_indexName, index);
            }
        }

        public class ConfigurationCommandsWill : BaseIntegrationTest
        {
            [Fact]
            public void GetAllConfigurationOptions()
            {
                var configuration = _db.GetConfiguration("*");

                var maxExpansions = configuration.Where(x => x.Item1 == "MAXEXPANSIONS").FirstOrDefault();

                Assert.NotNull(maxExpansions);
                Assert.Equal("200", maxExpansions.Item2);
            }

            [Fact]
            public void GetSpecificConfigurationOption()
            {
                var timeoutConfiguration = _db.GetConfiguration("TIMEOUT");

                Assert.Equal("TIMEOUT", timeoutConfiguration.First().Item1);
            }

            [Fact]
            public void SetConfigurationOption()
            {
                _db.SetConfiguration("TIMEOUT", "1000");
            }

            [Fact]
            public void ThrowExceptionOnBadConfiguration()
            {
                var exception = Assert.Throws<RediSearchConfigurationException>(() =>
                {
                    _db.SetConfiguration("THIS ISNT GOING TO WORK", "WHOA NELLY");
                });

                Assert.Equal("Looks like `THIS ISNT GOING TO WORK` with `WHOA NELLY` wasn't valid.", exception.Message);
            }
        }
    }
}