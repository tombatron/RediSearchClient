using RediSearchClient.Aggregate;
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

            db.Execute(RediSearchCommands.CREATE, commandParameters);
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
            var redisResult = db.Execute(RediSearchCommands.SEARCH, queryDefinition.Fields);

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
            var redisResult = db.Execute(RediSearchCommands.AGGREGATE, aggregateDefinition.Fields);

            return AggregateResult.From(redisResult); ;
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

            db.Execute(RediSearchCommands.ALTER, commandArguments.ToArray());
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

            var result = db.Execute(RediSearchCommands.DROPINDEX, commandArguments.ToArray());

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
            var result = db.Execute(RediSearchCommands.ALIASADD, alias, indexName);

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
            var result = db.Execute(RediSearchCommands.ALIASUPDATE, alias, indexName);

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
            var result = db.Execute(RediSearchCommands.ALIASDEL, alias);

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
            var result = db.Execute(RediSearchCommands.TAGVALS, index, fieldName);

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

            var result = db.Execute(RediSearchCommands.SUGADD, parameters.ToArray());
            
            return (int)result;
        }

        public static SuggestionResult[] GetSuggestions(this IDatabase db)
        {
            return null;
        }

        public static int DeleteSuggestion(this IDatabase db)
        {
            return 0;
        }

        public static int SuggestionsSize(this IDatabase db)
        {
            return 0;
        }

        public static void UpdateSynonyms(this IDatabase db)
        {

        }

        public static string[] DumpSynonyms(this IDatabase db)
        {
            return null;
        }

        public static SpellCheckResult[] SpellCheck(this IDatabase db)
        {
            return null;
        }

        public static int AddToDictionary(this IDatabase db)
        {
            return 0;
        }

        public static int DeleteFromDictionary(this IDatabase db)
        {
            return 0;
        }

        public static string[] DumpDictionary(this IDatabase db)
        {
            return null;
        }

        public static InfoResult GetInfo(this IDatabase db)
        {
            return null;
        }

        public static string[] ListIndexes(this IDatabase db)
        {
            return null;
        }

        public static ConfigureResult Configure(this IDatabase db)
        {
            return null;
        }
    }
}