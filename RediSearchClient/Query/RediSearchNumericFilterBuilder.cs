namespace RediSearchClient.Query
{
    public sealed class RediSearchNumericFilterBuilder
    {
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