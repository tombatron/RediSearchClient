using StackExchange.Redis;

namespace RediSearchClient
{
    public class GarbageCollectionStatistics
    {
        internal GarbageCollectionStatistics() { }

        public int BytesCollected { get; private set; }

        public int TotalMillisecondsRun { get; private set; }

        public int TotalCycles { get; private set; }

        public int AverageCycleTimeMilliseconds { get; private set; }

        public int LastRunTimeMilliseconds { get; private set; }

        public int GcNumericTreesMissed { get; private set; }

        public int GcBlocksDenied { get; private set; }

        internal static GarbageCollectionStatistics Create(RedisResult[] redisResult)
        {
            var result = new GarbageCollectionStatistics();

            for (var i = 0; i < redisResult.Length; i++)
            {
                var label = (string)redisResult[i];

                switch (label)
                {
                    case "bytes_collected":
                        result.BytesCollected = (int)redisResult[++i];
                        break;
                    case "total_ms_run":
                        result.TotalMillisecondsRun = (int)redisResult[++i];
                        break;
                    case "total_cycles":
                        result.TotalCycles = (int)redisResult[++i];
                        break;
                    case "average_cycle_time_ms":
                        result.AverageCycleTimeMilliseconds = (int)redisResult[++i];
                        break;
                    case "last_run_time_ms":
                        result.LastRunTimeMilliseconds = (int)redisResult[++i];
                        break;
                    case "gc_numeric_trees_missed":
                        result.GcNumericTreesMissed = (int)redisResult[++i];
                        break;
                    case "gc_blocked_denied":
                        result.GcBlocksDenied = (int)redisResult[++i];
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