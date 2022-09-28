using System;
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

        /// <summary>
        /// <para>Attributes defined on the schema.</para>
        /// <para>This will be populated instead of <see cref="Fields"/> versions of RediSearch >= 2.2.1 </para>
        /// </summary>
        /// <value></value>
        public Dictionary<string, Dictionary<string, object>> Attributes { get; private set; }

        /// <summary>
        /// Count of documents in the index.
        /// </summary>
        /// <value></value>
        public int NumberOfDocuments { get; private set; }

        /// <summary>
        /// TODO: Populate `MaxDocumentId` summary.
        /// </summary>
        /// <value></value>
        public int MaxDocumentId { get; private set; }

        /// <summary>
        /// TODO: Populate `NumberOfTerms` summary.
        /// </summary>
        /// <value></value>
        public int NumberOfTerms { get; private set; }

        /// <summary>
        /// TODO: Populate `NumberOfRecords` summary.
        /// </summary>
        /// <value></value>
        public int NumberOfRecords { get; private set; }

        /// <summary>
        /// TODO: Populate `InvertedSizeMegabytes` summary.
        /// </summary>
        /// <value></value>
        public double InvertedSizeMegabytes { get; private set; }

        /// <summary>
        /// TODO: Populate `TotalInvertedIndexBlocks` summary.
        /// </summary>
        /// <value></value>
        public int TotalInvertedIndexBlocks { get; private set; }

        /// <summary>
        /// TODO: Populate `OffsetVectorsSizeMegabytes` summary.
        /// </summary>
        /// <value></value>
        public double OffsetVectorsSizeMegabytes { get; private set; }

        /// <summary>
        /// TODO: Populate `DocumentTableSizeMegabytes` summary.
        /// </summary>
        /// <value></value>
        public double DocumentTableSizeMegabytes { get; private set; }

        /// <summary>
        /// TODO: Populate `SortableVluesSizeMegabytes` summary.
        /// </summary>
        /// <value></value>
        public double SortableValuesSizeMegabytes { get; private set; }

        /// <summary>
        /// TODO: Populate `KeyTableSizeMegabytes` summary.
        /// </summary>
        /// <value></value>
        public double KeyTableSizeMegabytes { get; private set; }

        /// <summary>
        /// TODO: Populate `RecordsPerDocumentAverage` summary.
        /// </summary>
        /// <value></value>
        public double RecordsPerDocumentAverage { get; private set; }

        /// <summary>
        /// TODO: Populate `BytesPerRecordAverage` summary.
        /// </summary>
        /// <value></value>
        public double BytesPerRecordAverage { get; private set; }

        /// <summary>
        /// TODO: Populate `OffsetsPerTermAverage` summary.
        /// </summary>
        /// <value></value>
        public double OffsetsPerTermAverage { get; private set; }

        /// <summary>
        /// TODO: Populate `OffsetBitsPerRecordAverage` summary.
        /// </summary>
        /// <value></value>
        public double OffsetBitsPerRecordAverage { get; private set; }

        /// <summary>
        /// The count of documents (hashes) that couldn't be index.
        /// </summary>
        /// <value></value>
        public int HashIndexingFailures { get; private set; }

        /// <summary>
        /// Whether or not the index is being scanned in the background. 
        /// </summary>
        /// <value></value>
        public int Indexing { get; private set; }

        /// <summary>
        /// Progress of background indexing. This will be `1` if complete. 
        /// </summary>
        /// <value></value>
        public double PercentIndexed { get; private set; }

        /// <summary>
        /// The garbage collection statistics portion of the info result.
        /// </summary>
        /// <value></value>
        public GarbageCollectionStatistics GarbageCollectionStatistics { get; private set; }

        /// <summary>
        /// The cursor statistcis portion of the info result.
        /// </summary>
        /// <value></value>
        public CursorStatistics CursorStatistics { get; private set; }

        /// <summary>
        /// The list of stop words overrides provided when the index was created.
        /// </summary>
        /// <value></value>
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
                    case "attributes":
                        var attributeValues = (RedisResult[])redisResult[++i];
                        var attributes = new Dictionary<string, Dictionary<string, object>>();

                        for (var j = 0; j < attributeValues.Length; j++)
                        {
                            var parsed = ParseRedisResult(attributeValues[j]);

                            if (!(parsed is Dictionary<string, object> attrDict))
                            {
                                continue;
                            }

                            if (!attrDict.TryGetValue("identifier", out var identifierValue))
                            {
                                continue;
                            }

                            if (identifierValue is string fieldName)
                            {
                                attributes.Add(fieldName, attrDict);
                                attrDict.Remove(fieldName);
                            }
                        }

                        result.Attributes = attributes;
                        break;
                    case "fields":
                        var fieldValues = (RedisResult[])redisResult[++i];
                        var fields = new Dictionary<string, object>[fieldValues.Length];

                        for (var j = 0; j < fieldValues.Length; j++)
                        {
                            var properties = (RedisResult[])fieldValues[j];
                            var fieldDict = new Dictionary<string, object>();

                            fieldDict.Add("fieldName", (string)properties[0]);

                            List<string> extraFieldOptions = default;

                            for (var k = 1; k < properties.Length; k++)
                            {
                                var fieldName = (string)properties[k];

                                if (fieldName == "SORTABLE" || fieldName == "NOSTEM" || fieldName == "NOINDEX")
                                {
                                    if (extraFieldOptions == default)
                                    {
                                        extraFieldOptions = new List<string>();
                                    }

                                    extraFieldOptions.Add(fieldName);
                                }
                                else
                                {
                                    fieldDict.Add(fieldName, properties[++k]);
                                }
                            }

                            if (extraFieldOptions != default)
                            {
                                fieldDict.Add("extraoptions", extraFieldOptions.ToArray());
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

        private static object ParseRedisResult(RedisResult redisResult)
        {
            if (redisResult?.Type == ResultType.MultiBulk && !redisResult.IsNull)
            {
                var results = ((RedisResult[]) redisResult);

                if (results?.Length == 0)
                {
                    return Array.Empty<object>();
                }

                if (results[0].Type ==  ResultType.MultiBulk)
                {
                    return results.Select(ParseRedisResult);
                }

                var result = new Dictionary<string, object>(results.Length / 2);

                for (var i = 0; i < results.Length-1; i++)
                {
                    result.Add((string)results[i], ParseRedisResult(results[++i]));
                }

                return result;
            }

            var val = (RedisValue)redisResult;

            return val.HasValue && val.TryParse(out double d) && !double.IsNaN(d) ? (object)d : (string) val;
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