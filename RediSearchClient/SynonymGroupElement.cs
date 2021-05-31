using System.Linq;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Describes a synonym and the group that it belongs to. 
    /// </summary>
    public class SynonymGroupElement
    {
        /// <summary>
        /// The term that in the synonym group. 
        /// </summary>
        /// <value></value>
        public string Term { get; private set; }

        /// <summary>
        /// Group IDs that the term belongs to.
        /// </summary>
        /// <value></value>
        public string[] GroupIds { get; private set; }

        internal static SynonymGroupElement[] CreateGroupResult(RedisResult redisResult)
        {
            var redisResultArray = (RedisResult[])redisResult;

            var result = new SynonymGroupElement[redisResultArray.Length / 2];
            var resultIndex = -1;

            for (var i = 0; i < redisResultArray.Length; i++)
            {
                result[++resultIndex] = new SynonymGroupElement
                {
                    Term = (string)redisResultArray[i],
                    GroupIds = ((RedisResult[])redisResultArray[++i]).Select(x => x.ToString()).ToArray()
                };
            }

            return result;
        }
    }
}