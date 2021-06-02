using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, object>[] Fields { get; private set; }

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

        public double OffsetBitsPerRecordAverage { get; private set; }

        public int HashIndexingFailures { get; private set; }

        public int Indexing { get; private set; }

        public double PercentIndexed { get; private set; }

        // gc stats
        public GarbageCollectionStatistics GarbageCollectionStatistics { get; private set; }

        // cursor stats
        public CursorStatistics CursorStatistics { get; private set; }

        // stopwords list
        public string[] StopWordsList { get; private set; }

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
                        var fieldValues = (RedisResult[])redisResult[++i];
                        var fields = new Dictionary<string, object>[fieldValues.Length];

                        for (var j = 0; j < fieldValues.Length; j++)
                        {
                            var properties = (RedisResult[])fieldValues[j];
                            var fieldDict = new Dictionary<string, object>();

                            for (var k = 0; k < properties.Length; k++)
                            {
                                fieldDict.Add((string)properties[k], properties[++k]);
                            }

                            fields[j] = fieldDict;
                        }

                        result.Fields = fields;
                        break;
                    case "num_docs":
                        result.NumberOfDocuments = (int)redisResult[++i];
                        break;
                    case "max_doc_id":
                        result.MaxDocumentId = (int)redisResult[++i];
                        break;
                    case "num_terms":
                        result.NumberOfTerms = (int)redisResult[++i];
                        break;
                    case "num_records":
                        result.NumberOfRecords = (int)redisResult[++i];
                        break;
                    case "inverted_sz_mb":
                        result.InvertedSizeMegabytes = (double)redisResult[++i];
                        break;
                    case "total_inverted_index_blocks":
                        result.TotalInvertedIndexBlocks = (int)redisResult[++i];
                        break;
                    case "offset_vectors_sz_mb":
                        result.OffsetVectorsSizeMegabytes = (double)redisResult[++i];
                        break;
                    case "doc_table_size_mb":
                        result.DocumentTableSizeMegabytes = (double)redisResult[++i];
                        break;
                    case "sortable_values_size_mb":
                        result.SortableValuesSizeMegabytes = (double)redisResult[++i];
                        break;
                    case "key_table_size_mb":
                        result.KeyTableSizeMegabytes = (double)redisResult[++i];
                        break;
                    case "records_per_doc_avg":
                        result.RecordsPerDocumentAverage = (double)redisResult[++i];
                        break;
                    case "bytes_per_record_avg":
                        result.BytesPerRecordAverage = (double)redisResult[++i];
                        break;
                    case "offsets_per_term_avg":
                        result.OffsetsPerTermAverage = (double)redisResult[++i];
                        break;
                    case "offset_bits_per_record_avg":
                        result.OffsetBitsPerRecordAverage = (double)redisResult[++i];
                        break;
                    case "hash_indexing_failures":
                        result.HashIndexingFailures = (int)redisResult[++i];
                        break;
                    case "indexing":
                        result.Indexing = (int)redisResult[++i];
                        break;
                    case "percent_indexed":
                        result.PercentIndexed = (double)redisResult[++i];
                        break;
                    case "gc_stats":
                        result.GarbageCollectionStatistics = GarbageCollectionStatistics.Create((RedisResult[])redisResult[++i]);
                        break;
                    case "cursor_stats":
                        result.CursorStatistics = CursorStatistics.Create((RedisResult[])redisResult[++i]);
                        break;
                    case "stopwords_list":
                        var stopwords = (RedisResult[])redisResult[++i];

                        result.StopWordsList = stopwords.Select(x => x.ToString()).ToArray();
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