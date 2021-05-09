using RediSearchClient.Indexes;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RediSearchClient
{
    public static partial class DatabaseExtensions
    {
        public static Task CreateIndexAsync(this IDatabase db, string indexName, RediSearchIndexDefinition indexDefinition)
        {
            var commandParameters = new object[1 + indexDefinition.Fields.Length];

            commandParameters[0] = indexName;

            Array.Copy(indexDefinition.Fields, 0, commandParameters, 1, indexDefinition.Fields.Length);

            return db.ExecuteAsync(RediSearchCommands.CREATE, commandParameters);
        }
    }
}