using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Statistics about cursors I guess. 
    /// </summary>
    public class CursorStatistics
    {
        internal CursorStatistics() { }

        public int GlobalIdle { get; private set; }

        public int GlobalTotal { get; private set; }

        public int IndexCapacity { get; private set; }

        public int IndexTotal { get; private set; }

        internal static CursorStatistics Create(RedisResult[] redisResult)
        {
            var result = new CursorStatistics();

            for (var i = 0; i < redisResult.Length; i++)
            {
                var label = (string)redisResult[i];

                switch(label)
                {
                    case "global_idle":
                        result.GlobalIdle = (int)redisResult[++i];
                        break;
                    case "global_total":
                        result.GlobalTotal = (int)redisResult[++i];
                        break;
                    case "index_capacity":
                        result.IndexCapacity = (int)redisResult[++i];
                        break;
                    case "index_total":
                        result.IndexTotal = (int)redisResult[++i];
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