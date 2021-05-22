using RediSearchClient.Aggregate;
using RediSearchClient.Indexes;
using RediSearchClient.Query;
using StackExchange.Redis;
using System;

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
            
            return AggregateResult.From(redisResult);;
        }

        public static string Explain(this IDatabase db)
        {
            return null;
        }

        public static void AlterSchema(this IDatabase db)
        {

        }

        public static bool DropIndex(this IDatabase db)
        {
            return false;
        }

        public static void AddAlias(this IDatabase db)
        {

        }

        public static void UpdateAlias(this IDatabase db)
        {

        }

        public static void DeleteAlias(this IDatabase db)
        {

        }

        public static string[] TagValues(this IDatabase db)
        {
            return null;
        }

        public static int AddSuggestion(this IDatabase db)
        {
            return 0;
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