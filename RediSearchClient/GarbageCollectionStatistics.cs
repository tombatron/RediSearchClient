using StackExchange.Redis;
using static RediSearchClient.ConversionUtilities;

namespace RediSearchClient
{
    /// <summary>
    /// Statistics... about garbage collection. 
    /// </summary>
    public class GarbageCollectionStatistics
    {
        internal GarbageCollectionStatistics() { }

        /// <summary>
        /// Total bytes collected by the GC.
        /// </summary>
        /// <value></value>
        public int BytesCollected { get; private set; }

        /// <summary>
        /// Total time (in milliseconds) that GC has run.
        /// </summary>
        /// <value></value>
        public int TotalMillisecondsRun { get; private set; }

        /// <summary>
        /// Total number of GC cycles run.
        /// </summary>
        /// <value></value>
        public int TotalCycles { get; private set; }

        /// <summary>
        /// Average GC cycle time: TotalMillisecondsRun / TotalCycles
        /// </summary>
        /// <value></value>
        public double AverageCycleTimeMilliseconds { get; private set; }

        /// <summary>
        /// In relation to the start time of the RediSearch/Redis process when was the 
        /// last time GC was run? (I think...)
        /// </summary>
        /// <value></value>
        public double LastRunTimeMilliseconds { get; private set; }

        /// <summary>
        /// TODO: Populate `GcNumericTreesMissed` summary.
        /// </summary>
        /// <value></value>
        public double GcNumericTreesMissed { get; private set; }

        /// <summary>
        /// TODO: Populate `GcBlocksDenied` summary.
        /// </summary>
        /// <value></value>
        public double GcBlocksDenied { get; private set; }

        internal static GarbageCollectionStatistics Create(RedisResult[] redisResult)
        {
            var result = new GarbageCollectionStatistics();

            for (var i = 0; i < redisResult.Length; i++)
            {
                var label = (string)redisResult[i];

                switch (label)
                {
                    case "bytes_collected":
                        result.BytesCollected = ConvertToInt(redisResult[++i]);
                        break;
                    case "total_ms_run":
                        result.TotalMillisecondsRun = ConvertToInt(redisResult[++i]);
                        break;
                    case "total_cycles":
                        result.TotalCycles = ConvertToInt(redisResult[++i]);
                        break;
                    case "average_cycle_time_ms":
                        result.AverageCycleTimeMilliseconds = ConvertToInt(redisResult[++i]);
                        break;
                    case "last_run_time_ms":
                        result.LastRunTimeMilliseconds = ConvertToInt(redisResult[++i]);
                        break;
                    case "gc_numeric_trees_missed":
                        result.GcNumericTreesMissed = ConvertToInt(redisResult[++i]);
                        break;
                    case "gc_blocked_denied":
                        result.GcBlocksDenied = ConvertToInt(redisResult[++i]);
                        break;
                    default:
                        ++i;
                        break;
                }
            }

            return result;
        }
    }
}