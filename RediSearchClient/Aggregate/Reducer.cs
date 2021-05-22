namespace RediSearchClient.Aggregate
{
    /// <summary>
    /// A smart-enum that represents the supported GROUPBY reducers.
    /// </summary>
    public sealed class Reducer
    {
        /// <summary>
        /// Count the number of records in each group.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#count
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer Count = new Reducer("COUNT");

        /// <summary>
        /// Count the number of distinct values for property.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#count_distinct
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer CountDistinct = new Reducer("COUNT_DISTINCT");

        /// <summary>
        /// Same as COUNT_DISTINCT - but provide an approximation instead of an exact count, at the expense of less memory and CPU in big groups.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#count_distinctish
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer CountDistinctish = new Reducer("COUNT_DISTINCTISH");

        /// <summary>
        /// Return the sum of all numeric values of a given property in a group. Non numeric values if the group are counted as 0.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#sum
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer Sum = new Reducer("SUM");

        /// <summary>
        /// Return the minimal value of a property, whether it is a string, number or NULL.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#min
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer Minimum = new Reducer("MIN");

        /// <summary>
        /// Return the maximal value of a property, whether it is a string, number or NULL.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#max
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer Maximum = new Reducer("MAX");

        /// <summary>
        /// Return the average value of a numeric property. This is equivalent to reducing by sum and count, and later on applying the ratio of them as an APPLY step.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#avg
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer Average = new Reducer("AVG");

        /// <summary>
        /// Return the standard deviation of a numeric property in the group.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#stddev
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer StandardDeviation = new Reducer("STDDEV");

        /// <summary>
        /// Return the value of a numeric property at a given quantile of the results. Quantile is expressed as a number between 0 and 1. For example, the median can be expressed as the quantile at 0.5, e.g. REDUCE QUANTILE 2 @foo 0.5 AS median .
        ///
        /// If multiple quantiles are required, just repeat the QUANTILE reducer for each quantile. e.g. REDUCE QUANTILE 2 @foo 0.5 AS median REDUCE QUANTILE 2 @foo 0.99 AS p99
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#quantile
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer Quantile = new Reducer("QUANTILE");

        /// <summary>
        /// Merge all distinct values of a given property into a single array.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#tolist
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer ToList = new Reducer("TOLIST");

        /// <summary>
        /// Return the first or top value of a given property in the group, optionally by comparing that or another property. 
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#first_value
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer FirstValue = new Reducer("FIRST_VALUE");

        /// <summary>
        /// Perform a reservoir sampling of the group elements with a given size, and return an array of the sampled items with an even distribution.
        /// 
        /// https://oss.redislabs.com/redisearch/Aggregations/#random_sample
        /// </summary>
        /// <returns></returns>
        public static readonly Reducer RandomSample = new Reducer("RANDOM_SAMPLE");

        private readonly string _functionName;

        private Reducer(string functionName) =>
            _functionName = functionName;

        /// <summary>
        /// The string representation will be returned as the backing reducer function name.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _functionName;

        /// <summary>
        /// Implicitly convert a reducer to string.
        /// </summary>
        /// <param name="reducer"></param>
        public static implicit operator string(Reducer reducer) => reducer.ToString();
    }
}