namespace RediSearchClient.Query
{
    /// <summary>
    /// Numeric query filter builder. 
    /// </summary>
    public sealed class RediSearchNumericFilterBuilder
    {
        /// <summary>
        /// Specify the field and range for a query's numeric filter.
        /// </summary>
        /// <param name="fieldName">Name of the numeric field.</param>
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// <param name="minExclusive">Is the lower end of the range "exclusive"?</param>
        /// <param name="maxExclusive">Is the upper end of the range "exclusive"?</param>
        /// <returns></returns>
        public IRediSearchNumericFilter Field(string fieldName, double min, double max, bool minExclusive = false, bool maxExclusive = false)
        {
            string minRange;

            if (minExclusive)
            {
                minRange = $"({min}";
            }
            else
            {
                minRange = min.ToString();
            }

            string maxRange;

            if (maxExclusive)
            {
                maxRange = $"({max}";
            }
            else
            {
                maxRange = max.ToString();
            }

            return new DefaultRediSearchNumericFilter(fieldName, minRange, maxRange);
        }
    }
}