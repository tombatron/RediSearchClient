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

                Assert.Single(fields);
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

                Assert.Single(fields);
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

                await _db.AddAliasAsync(alias, _indexName);

                var success = await _db.DeleteAliasAsync(alias);

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

                Assert.Empty(tags);
            }

            [Fact]
            public async Task WillReturnEmptyArrayWhenMissingFieldsAsync()
            {
                var tags = await _db.TagValuesAsync(_indexName, "state");

                Assert.Empty(tags);
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
            public async Task AddASuggestionToTheIndexAsync()
            {
                var firstResult = await _db.AddSuggestionAsync(_dictionaryName, "hello", 1);
                var secondResult = await _db.AddSuggestionAsync(_dictionaryName, "goodnight", 1);

                Assert.Equal(1, firstResult);
                Assert.Equal(2, secondResult);
            }

            [Fact]
            public void AddASuggestionToTheIndexWithPayload()
            {
                var result = _db.AddSuggestion(_dictionaryName, "whoa", 1, payload: "hey there");
            }

            [Fact]
            public async Task AddASuggestionToTheIndexWithPayloadAsync()
            {
                var result = await _db.AddSuggestionAsync(_dictionaryName, "whoa", 1, payload: "hey there");
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
            public async Task ReturnSuggestionsAsync()
            {
                var suggestions = await _db.GetSuggestionsAsync(_dictionaryName, "he");

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
            public async Task ReturnSuggestionsWithScoresAsync()
            {
                var suggestions = await _db.GetSuggestionsAsync(_dictionaryName, "he", withScores: true);

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
            public async Task ReturnSuggestionsWithPayloadsAsync()
            {
                var suggestions = await _db.GetSuggestionsAsync(_dictionaryName, "he", withPayloads: true);

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

            [Fact]
            public async Task ReturnSuggestionsWithScoresAndPayloadsAsync()
            {
                var suggestions = await _db.GetSuggestionsAsync(_dictionaryName, "he", withScores: true, withPayloads: true);

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
            public async Task RemoveSuggestionIfItExistsAsync()
            {
                var result = await _db.DeleteSuggestionAsync(_dictionaryName, "hello world");

                Assert.True(result);
            }

            [Fact]
            public void NotRemoveSuggestionIfItDoesntExist()
            {
                var result = _db.DeleteSuggestion(_dictionaryName, "hello lucas");

                Assert.False(result);
            }

            [Fact]
            public async Task NotRemoveSuggestionIfItDoesntExistAsync()
            {
                var result = await _db.DeleteSuggestionAsync(_dictionaryName, "hello lucas");

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

            [Fact]
            public async Task ReturnLengthOfSuggestionDictionaryAsync()
            {
                var result = await _db.SuggestionsSizeAsync(_dictionaryName);

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

            [Fact]
            public async Task WorksAsync()
            {
                var synonyms = await _db.DumpSynonymsAsync(_indexName);

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

        public class SpellCheckWill : BaseIntegrationTest
        {
            public override void Setup()
            {
                base.Setup();

                if (!_db.ListIndexes().Contains("simple_movie_index"))
                {
                    _db.CreateIndex("simple_movie_index",
                        RediSearchIndex
                            .On(RediSearchStructure.HASH)
                            .ForKeysWithPrefix("movie::")
                            .WithSchema(x => x.Text("Title"))
                        .Build()
                    );

                    while (_db.GetInfo("simple_movie_index").Indexing == 1)
                    {
                        Thread.Sleep(500);
                    }
                }
            }

            [Fact]
            public void CheckSpelling()
            {
                var result = _db.SpellCheck("simple_movie_index", "@title:Garam Masaluh", 5);

                Assert.Single(result);
            }

            [Fact]
            public async Task CheckSpellingAsync()
            {
                var result = await _db.SpellCheckAsync("simple_movie_index", "@title:Garam Masaluh", 5);

                Assert.Single(result);
            }
        }

        public class DictionaryCommandsCan : BaseIntegrationTest
        {
            [Fact]
            public void AddToDictionary()
            {
                var result = _db.AddToDictionary("test_dict", Guid.NewGuid().ToString("n"));

                Assert.Equal(1, result);
            }

            [Fact]
            public async Task AddToDictionaryAsync()
            {
                var result = await _db.AddToDictionaryAsync("test_dict", Guid.NewGuid().ToString("n"));

                Assert.Equal(1, result);
            }

            [Fact]
            public void DeleteFromDictionary()
            {
                var added = _db.AddToDictionary("test_dict", "delete_from_dictionary");
                var deleted = _db.DeleteFromDictionary("test_dict", "delete_from_dictionary");

                Assert.Equal(1, added);
                Assert.Equal(1, deleted);
            }

            [Fact]
            public async Task DeleteFromDictionaryAsync()
            {
                var added = await _db.AddToDictionaryAsync("test_dict", "delete_from_dictionary_async");
                var deleted = await _db.DeleteFromDictionaryAsync("test_dict", "delete_from_dictionary_async");

                Assert.Equal(1, added);
                Assert.Equal(1, deleted);
            }

            [Fact]
            public void DumpADictionary()
            {
                var terms = Enumerable.Range(0, 5).Select(x => Guid.NewGuid().ToString("n")).ToArray();

                _db.AddToDictionary("test_dict", terms);

                var dumped = _db.DumpDictionary("test_dict");

                Assert.True(terms.All(t => dumped.Any(d => t == d)));
            }

            [Fact]
            public async Task DumpADictionaryAsync()
            {
                var terms = Enumerable.Range(0, 5).Select(x => Guid.NewGuid().ToString("n")).ToArray();

                await _db.AddToDictionaryAsync("test_dict", terms);

                var dumped = await _db.DumpDictionaryAsync("test_dict");

                Assert.True(terms.All(t => dumped.Any(d => t == d)));
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

            [Fact]
            public async Task ReturnInfoAboutAnIndexAsync()
            {
                var info = await _db.GetInfoAsync(_indexName);

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

        public class ListCommandWill : BaseIntegrationTest
        {
            public override void Setup()
            {
                base.Setup();

                _db.CreateIndex(_indexName, RediSearchIndex
                    .On(RediSearchStructure.HASH)
                    .ForKeysWithPrefix("Whoa::")
                    .WithSchema
                    (
                        x => x.Geo("whatever")
                    )
                    .NoFrequencies()
                    .NoOffsets()
                    .Build());

            }

            [Fact]
            public void ListExistingIndexes()
            {
                var indexes = _db.ListIndexes();

                Assert.Contains(_indexName, indexes);
            }

            [Fact]
            public async Task ListExistingIndexesAsync()
            {
                var indexes = await _db.ListIndexesAsync();

                Assert.Contains(_indexName, indexes);
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
            public async Task GetAllConfigurationOptionsAsync()
            {
                var configuration = await _db.GetConfigurationAsync("*");

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
            public async Task GetSpecificConfigurationOptionAsync()
            {
                var timeoutConfiguration = await _db.GetConfigurationAsync("TIMEOUT");

                Assert.Equal("TIMEOUT", timeoutConfiguration.First().Item1);
            }

            [Fact]
            public void SetConfigurationOption()
            {
                _db.SetConfiguration("TIMEOUT", "1000");
            }

            [Fact]
            public async Task SetConfigurationOptionAsync()
            {
                await _db.SetConfigurationAsync("TIMEOUT", "1000");
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

            [Fact]
            public async Task ThrowExceptionOnBadConfigurationAsync()
            {
                var exception = await Assert.ThrowsAsync<RediSearchConfigurationException>(async () =>
                {
                    await _db.SetConfigurationAsync("THIS ISNT GOING TO WORK", "WHOA NELLY");
                });

                Assert.Equal("Looks like `THIS ISNT GOING TO WORK` with `WHOA NELLY` wasn't valid.", exception.Message);
            }            
        }
    }
}