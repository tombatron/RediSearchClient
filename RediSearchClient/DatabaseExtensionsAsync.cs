using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RediSearchClient
{
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
        public static Task CreateIndexAsync(this IDatabase db, string indexName, RediSearchIndexDefinition indexDefinition)
        {
            var commandParameters = new object[1 + indexDefinition.Fields.Length];

            commandParameters[0] = indexName;

            Array.Copy(indexDefinition.Fields, 0, commandParameters, 1, indexDefinition.Fields.Length);

            return db.ExecuteAsync(RediSearchCommand.CREATE, commandParameters);
        }
    }
}