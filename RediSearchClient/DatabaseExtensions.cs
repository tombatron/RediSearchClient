using RediSearchClient.Aggregate;
using RediSearchClient.Exceptions;
using RediSearchClient.Indexes;
using RediSearchClient.Query;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RediSearchClient
{
    /// <summary>
    /// This class defines the extension methods for StackExchange.Redis.IDatabase that allow
    /// for the interaction with the RediSearch (2.x) Redis module.
    /// </summary>
    public static partial class DatabaseExtensions
    {
        /// <summary>
        /// `FT.CREATE`
        /// 
        /// Creates an index with the given spec.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftcreate
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName">Name of the index to create.</param>
        /// <param name="indexDefinition">The definition of the index to create.</param>
        public static void CreateIndex(this IDatabase db, string indexName, RediSearchIndexDefinition indexDefinition)
        {
            var commandParameters = new object[1 + indexDefinition.Fields.Length];

            commandParameters[0] = indexName;

            Array.Copy(indexDefinition.Fields, 0, commandParameters, 1, indexDefinition.Fields.Length);

            db.Execute(RediSearchCommand.CREATE, commandParameters);
        }

        /// <summary>
        /// `FT.SEARCH`
        /// 
        /// Searches the index with a textual query, returning either documents or just ids.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftsearch
        /// </summary>
        /// <param name="db"></param>
        /// <param name="queryDefinition"></param>
        /// <returns></returns>
        public static SearchResult Search(this IDatabase db, RediSearchQueryDefinition queryDefinition)
        {
            var redisResult = db.Execute(RediSearchCommand.SEARCH, queryDefinition.Fields);

            return SearchResult.From(redisResult);
        }

        /// <summary>
        /// `FT.AGGREGATE`
        /// 
        /// Runs a search query on an index, and performs aggregate transformations on the results.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftaggregate
        /// </summary>
        /// <param name="db"></param>
        /// <param name="aggregateDefinition"></param>
        /// <returns></returns>
        public static AggregateResult Aggregate(this IDatabase db, RediSearchAggregateDefinition aggregateDefinition)
        {
            var redisResult = db.Execute(RediSearchCommand.AGGREGATE, aggregateDefinition.Fields);

            return AggregateResult.From(redisResult);
        }

        /// <summary>
        /// `FT.ALTER {index} SCHEMA ADD {field} {options} ...`
        /// 
        /// Adds a new field to the index.
        ///
        /// Adding a field to the index will cause any future document updates to use the new field when indexing 
        /// and reindexing of existing documents.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftalter_schema_add
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName"></param>
        /// <param name="fields"></param>
        public static void AlterSchema(this IDatabase db, string indexName, params Func<RediSearchSchemaFieldBuilder, IRediSearchSchemaField>[] fields)
        {
            var builder = new RediSearchSchemaFieldBuilder();

            var builtFields = fields.Select(x => x(builder)).SelectMany(x => x.FieldArguments).ToArray();

            var commandArguments = new List<object>(3 + builtFields.Length)
            {
                indexName,
                "SCHEMA",
                "ADD"
            };

            commandArguments.AddRange(builtFields);

            db.Execute(RediSearchCommand.ALTER, commandArguments.ToArray());
        }

        /// <summary>
        /// `FT.DROPINDEX`
        /// 
        /// Deletes the index.
        /// 
        /// By default, FT.DROPINDEX does not delete the document hashes associated with the index. 
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftdropindex
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName"></param>
        /// <param name="dropDocumentHashes">Drop associated document hashes (defaults to `false`)</param>
        /// <returns></returns>
        public static bool DropIndex(this IDatabase db, string indexName, bool dropDocumentHashes = false)
        {
            var commandArguments = new List<object>(2)
            {
                indexName
            };

            if (dropDocumentHashes)
            {
                commandArguments.Add("DD");
            }

            var result = db.Execute(RediSearchCommand.DROPINDEX, commandArguments.ToArray());

            return result.ToString() == "OK";
        }

        /// <summary>
        /// `FT.ALIASADD` {name} {index}
        /// 
        /// The FT.ALIASADD will add an alias to an index. Index aliases can be used to refer to actual indexes 
        /// in data commands such as FT.SEARCH or FT.ADD . This allows an administrator to transparently redirect 
        /// application queries to alternative indexes.
        /// 
        /// Indexes can have more than one alias, though an alias cannot refer to another alias.
        /// 
        /// FT.ALIASADD will fail if the alias is already associated with another index.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftaliasadd
        /// </summary>
        /// <param name="db"></param>
        /// <param name="alias"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static bool AddAlias(this IDatabase db, string alias, string indexName)
        {
            var result = db.Execute(RediSearchCommand.ALIASADD, alias, indexName);

            return result.ToString() == "OK";
        }

        /// <summary>
        /// `FT.ALIASUPDATE` {name} {index}
        /// 
        /// The FT.ALIASUPDATE command differs from the FT.ALIASADD command in that it will remove the alias 
        /// association with a previous index, if any. 
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftaliasupdate
        /// </summary>
        /// <param name="db"></param>
        /// <param name="alias"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static bool UpdateAlias(this IDatabase db, string alias, string indexName)
        {
            var result = db.Execute(RediSearchCommand.ALIASUPDATE, alias, indexName);

            return result.ToString() == "OK";
        }

        /// <summary>
        /// `FT.ALIASDEL` {name}
        /// 
        /// Removes an existing index alias.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftaliasdel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static bool DeleteAlias(this IDatabase db, string alias)
        {
            var result = db.Execute(RediSearchCommand.ALIASDEL, alias);

            return result.ToString() == "OK";
        }

        /// <summary>
        /// `FT.TAGVALS` {index} {field_name}
        /// 
        /// Returns the distinct tags indexed in a Tag field .
        /// 
        /// This is useful if your tag field indexes things like cities, categories, etc.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#fttagvals
        /// </summary>
        /// <param name="db"></param>
        /// <param name="index"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string[] TagValues(this IDatabase db, string index, string fieldName)
        {
            var result = db.Execute(RediSearchCommand.TAGVALS, index, fieldName);

            return ((RedisResult[])result).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// `FT.SUGADD`
        /// 
        /// Adds a suggestion string to an auto-complete suggestion dictionary. This is disconnected 
        /// from the index definitions, and leaves creating and updating suggestions dictionaries to the user.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftsugadd
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The suggestion dictionary key.</param>
        /// <param name="value">The suggestion string we index.</param>
        /// <param name="score">A floating point number of the suggestion string's weight.</param>
        /// <param name="increment">If set, we increment the existing entry of the suggestion by the given score, instead of replacing the score. This is useful for updating the dictionary based on user queries in real time.</param>
        /// <param name="payload">If set, we save an extra payload with the suggestion, that can be fetched by adding the WITHPAYLOADS argument to FT.SUGGET.</param>
        /// <returns>The current size of the suggestion dictionary.</returns>
        public static int AddSuggestion(this IDatabase db, string key, string value, double score, bool increment = false, string payload = default)
        {
            var parameters = new List<object>(5)
            {
                key,
                value,
                score
            };

            if (increment)
            {
                parameters.Add("INCR");
            }

            if (!string.IsNullOrEmpty(payload))
            {
                parameters.Add("PAYLOAD");
                parameters.Add(payload);
            }

            var result = db.Execute(RediSearchCommand.SUGADD, parameters.ToArray());

            return (int)result;
        }

        /// <summary>
        /// `FT.SUGGET`
        /// 
        /// Gets completion suggestions for a prefix.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftsugget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The suggestion dictionary key.</param>
        /// <param name="prefix">The prefix to complete on.</param>
        /// <param name="fuzzy">If set, we do a fuzzy prefix search, including prefixes at Levenshtein distance of 1 from the prefix sent.</param>
        /// <param name="withScores">If set, we also return the score of each suggestion. this can be used to merge results from multiple instances.</param>
        /// <param name="withPayloads">If set, we return optional payloads saved along with the suggestions. If no payload is present for an entry, we return a Null Reply.</param>
        /// <param name="max">If set, we limit the results to a maximum of num (default: 5).</param>
        /// <returns>A list of the top suggestions matching the prefix, optionally with score after each entry.</returns>
        public static SuggestionResult[] GetSuggestions(this IDatabase db, string key, string prefix, bool fuzzy = false, bool withScores = false, bool withPayloads = false, int max = 5)
        {
            var parameters = new List<object>(7)
            {
                key,
                prefix
            };

            if (fuzzy)
            {
                parameters.Add("FUZZY");
            }

            if (withScores)
            {
                parameters.Add("WITHSCORES");
            }

            if (withPayloads)
            {
                parameters.Add("WITHPAYLOADS");
            }

            if (max != 5)
            {
                parameters.Add("MAX");
                parameters.Add(max);
            }

            var result = db.Execute(RediSearchCommand.SUGGET, parameters.ToArray());

            return SuggestionResult.CreateArray(result, withScores, withPayloads);
        }

        /// <summary>
        /// `FT.SUGDEL
        /// 
        /// Deletes a string from a suggestion index. 
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftsugdel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The suggestion dictionary key.</param>
        /// <param name="value">The value to delete.</param>
        /// <returns>True if the value was found, false otherwise.</returns>
        public static bool DeleteSuggestion(this IDatabase db, string key, string value)
        {
            var result = db.Execute(RediSearchCommand.SUGDEL, key, value);

            return (int)result == 1;
        }

        /// <summary>
        /// `FT.SUGLEN`
        /// 
        /// Gets the size of an auto-complete suggestion dictionary.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftsuglen
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">The suggestion dictionary key.</param>
        /// <returns>The current size of the suggestion dictionary.</returns>
        public static int SuggestionsSize(this IDatabase db, string key)
        {
            return (int)db.Execute(RediSearchCommand.SUGLEN, key);
        }

        /// <summary>
        /// `FT.SYNUPDATE`
        /// 
        /// Updates a synonym group.
        ///
        /// The command is used to create or update a synonym group with additional terms. 
        /// Only documents which were indexed after the update will be affected.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#synonym
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName"></param>
        /// <param name="synonymGroupId"></param>
        /// <param name="terms"></param>
        public static void UpdateSynonyms(this IDatabase db, string indexName, string synonymGroupId, params string[] terms) =>
            db.UpdateSynonyms(indexName, synonymGroupId, false, terms);

        /// <summary>
        /// `FT.SYNUPDATE`
        /// 
        /// Updates a synonym group.
        ///
        /// The command is used to create or update a synonym group with additional terms. 
        /// Only documents which were indexed after the update will be affected.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#synonym
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName"></param>
        /// <param name="synonymGroupId"></param>
        /// <param name="skipInitialScan">If set, we do not scan and index.</param>
        /// <param name="terms"></param>
        public static void UpdateSynonyms(this IDatabase db, string indexName, string synonymGroupId, bool skipInitialScan, params string[] terms)
        {
            var parameters = new List<object>()
            {
                indexName,
                synonymGroupId
            };

            if (skipInitialScan)
            {
                parameters.Add("SKIPINITIALSCAN");
            }

            parameters.AddRange(terms);

            db.Execute(RediSearchCommand.SYNUPDATE, parameters.ToArray());
        }

        /// <summary>
        /// `FT.SYNDUMP`
        /// 
        /// Dumps the contents of a synonym group.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftsyndump
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static SynonymGroupElement[] DumpSynonyms(this IDatabase db, string indexName)
        {
            var rawResult = db.Execute(RediSearchCommand.SYNDUMP, indexName);

            return SynonymGroupElement.CreateGroupResult(rawResult);
        }

        /// <summary>
        /// `FT.SPELLCHECK`
        /// 
        /// Performs spelling correction on a query, returning suggestions for misspelled terms.
        /// 
        /// https://oss.redislabs.com/redisearch/Spellcheck/
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftspellcheck
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName">The index with the indexed terms.</param>
        /// <param name="query">The search query.</param>
        /// <param name="distance">The maximal Levenshtein distance for spelling suggestions (default: 1, max: 4).</param>
        /// <param name="terms">Specifies an inclusion or exclusion custom dictionary named.</param>
        /// <returns></returns>
        public static SpellCheckResultCollection SpellCheck(this IDatabase db, string indexName, string query, int distance = 1, params SpellCheckTerm[] terms)
        {
            var parameters = new List<object>(2)
            {
                indexName,
                query
            };

            if (distance != 1)
            {
                parameters.Add("DISTANCE");
                parameters.Add(distance);
            }

            var result = db.Execute(RediSearchCommand.SPELLCHECK, parameters.ToArray());

            return new SpellCheckResultCollection(SpellCheckResult.CreateArray(result));
        }

        /// <summary>
        /// `FT.SPELLCHECK`
        /// 
        /// Performs spelling correction on a query, returning suggestions for misspelled terms.
        /// 
        /// https://oss.redislabs.com/redisearch/Spellcheck/
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftspellcheck
        /// </summary>
        /// <param name="db"></param>
        /// <param name="queryDefinition">The search query.</param>
        /// <param name="distance">The maximal Levenshtein distance for spelling suggestions (default: 1, max: 4).</param>
        /// <param name="terms">Specifies an inclusion or exclusion custom dictionary named.</param>
        /// <returns></returns>
        public static SpellCheckResultCollection SpellCheck(this IDatabase db, RediSearchQueryDefinition queryDefinition, int distance = 1, params SpellCheckTerm[] terms)
        {
            return SpellCheck(db, (string)queryDefinition.Fields[0], (string)queryDefinition.Fields[1], distance, terms);
        }

        /// <summary>
        /// `FT.DICTADD {dict} {term} [{term} ...]
        /// 
        /// Adds terms to a dictionary.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftdictadd
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dictionaryName">The dictionary name.</param>
        /// <param name="terms">The term(s) to add to the dictionary.</param>
        /// <returns>The number of new terms added to the dictionary.</returns>
        public static int AddToDictionary(this IDatabase db, string dictionaryName, params string[] terms)
        {
            var parameters = new List<object>
            {
                dictionaryName
            };

            parameters.AddRange(terms);

            var result = db.Execute(RediSearchCommand.DICTADD, parameters.ToArray());

            return (int)result;
        }

        /// <summary>
        /// `FT.DICTDEL`
        /// 
        /// Deletes terms from a dictionary.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftdictdel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dictionaryName">The dictionary name.</param>
        /// <param name="terms">The term(s) to delete from the dictionary.</param>
        /// <returns>The number of terms deleted.</returns>
        public static int DeleteFromDictionary(this IDatabase db, string dictionaryName, params string[] terms)
        {
            var parameters = new List<object>
            {
                dictionaryName
            };

            parameters.AddRange(terms);

            var result = db.Execute(RediSearchCommand.DICTDEL, parameters.ToArray());

            return (int)result;
        }

        /// <summary>
        /// `FT.DICTDUMP`
        /// 
        /// Dumps all terms in the given dictionary.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftdictdump
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dictionaryName">The dictionary name.</param>
        /// <returns>Returns array where each element is a dictionary term.</returns>
        public static string[] DumpDictionary(this IDatabase db, string dictionaryName)
        {
            var result = db.Execute(RediSearchCommand.DICTDUMP, dictionaryName);

            return (string[])result;
        }

        /// <summary>
        /// `FT.INFO`
        /// 
        /// Returns information and statistics on the index.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftinfo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static InfoResult GetInfo(this IDatabase db, string indexName)
        {
            var result = db.Execute(RediSearchCommand.INFO, indexName);

            return InfoResult.Create((RedisResult[])result);
        }

        /// <summary>
        /// `FT._LIST`
        /// 
        /// Returns a list of all existing indexes.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ft_list
        /// </summary>
        /// <param name="db"></param>
        /// <returns>An array with index names.</returns>
        public static string[] ListIndexes(this IDatabase db)
        {
            var result = (RedisResult[])db.Execute(RediSearchCommand.LIST);

            return result.Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        /// `FT.CONFIG SET`
        /// 
        /// Sets a runtime configuration option.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftconfig
        /// </summary>
        /// <param name="db"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public static void SetConfiguration(this IDatabase db, string option, string value)
        {
            var result = db.Execute(RediSearchCommand.CONFIG, "SET", option, value);

            if ((string)result != "OK")
            {
                throw new RediSearchConfigurationException($"Looks like `{option}` with `{value}` wasn't valid.");
            }
        }

        /// <summary>
        /// `FT.CONFIG GET`
        /// 
        /// Gets a runtime configuration option.
        /// 
        /// https://oss.redislabs.com/redisearch/Commands/#ftconfig
        /// </summary>
        /// <param name="db"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static Tuple<string, string>[] GetConfiguration(this IDatabase db, string option)
        {
            var result = (RedisResult[])db.Execute(RediSearchCommand.CONFIG, "GET", option);

            return result.Select(x =>
            {
                var pair = (RedisResult[])x;

                return new Tuple<string, string>((string)pair[0], (string)pair[1]);
            })
            .ToArray();
        }
    }
}