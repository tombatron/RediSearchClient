using StackExchange.Redis;

namespace RediSearchClient
{
    public static partial class DatabaseExtensions
    {
        public static void CreateIndex(this IDatabase db)
        {

        }

        public static SearchResult Search(this IDatabase db)
        {
            return null;
        }

        public static AggregateResult Aggregate(this IDatabase db)
        {
            return null;
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