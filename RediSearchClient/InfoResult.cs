using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a search index. 
    /// </summary>
    public class InfoResult
    {
        internal InfoResult() { }

        /// <summary>
        /// Name of the index.
        /// </summary>
        /// <value></value>
        public string IndexName { get; private set; }

        /// <summary>
        /// Options that the index was created with.
        /// 
        /// Example: NOFREQS, NOOFFSETS
        /// </summary>
        /// <value></value>
        public string[] IndexOptions { get; private set; }

        /// <summary>
        /// Description of the options used to create an index.
        /// </summary>
        /// <value></value>
        public IndexDefinition IndexDefinition { get; private set; }

        /// <summary>
        /// Fields defined on the schema.
        /// </summary>
        /// <value></value>
        public SchemaFieldDefinition[] Fields { get; private set; }

        public int NumberOfDocuments { get; private set; }

        public int MaxDocumentId { get; private set; }

        public int NumberOfTerms { get; private set; }

        public int NumberOfRecords { get; private set; }

        public double InvertedSizeMegabytes { get; private set; }

        public int TotalInvertedIndexBlocks { get; private set; }

        public double OffsetVectorsSizeMegabytes { get; private set; }

        public double DocumentTableSizeMegabytes { get; private set; }

        public double SortableValuesSizeMegabytes { get; private set; }

        public double KeyTableSizeMegabytes { get; private set; }

        public double RecordsPerDocumentAverage { get; private set; }

        public double BytesPerRecordAverage { get; private set; }

        public double OffsetsPerTermAverage { get; private set; }

        public int OffsetBitsPerRecordAverage { get; private set; }

        public int HashIndexingFailures { get; private set; }

        public int Indexing { get; private set; }

        public double PercentIndexed { get; private set; }

        // gc stats

        // cursor stats

        // stopwords list

        internal static InfoResult Create(RedisResult[] redisResult)
        {
            var result = new InfoResult();

            for (var i = 0; i < redisResult.Length; i++)
            {
                var label = (string)redisResult[i];

                switch (label)
                {
                    case "index_name":
                        result.IndexName = (string)redisResult[++i];
                        break;
                    case "index_options":
                        result.IndexOptions = ParseIndexOptions((RedisResult[])redisResult[++i]);
                        break;
                    case "index_definition":
                        result.IndexDefinition = IndexDefinition.Create((RedisResult[])redisResult[++i]);
                        break;
                    case "fields":
                        result.Fields = SchemaFieldDefinition.CreateArray((RedisResult[])redisResult[++i]);
                        break;
                    default:
                        ++i;
                        break;
                }
            }

            return result;
        }

        private static string[] ParseIndexOptions(RedisResult[] redisResult)
        {
            var result = new string[redisResult.Length];

            for (var i = 0; i < redisResult.Length; i++)
            {
                result[i] = (string)redisResult[i];
            }

            return result;
        }

    }
}