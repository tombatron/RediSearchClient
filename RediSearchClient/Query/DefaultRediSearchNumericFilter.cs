namespace RediSearchClient.Query
{
    internal class DefaultRediSearchNumericFilter : IRediSearchNumericFilter
    {
        public string FieldName { get; }

        public string Min { get; }

        public string Max { get; }

        internal DefaultRediSearchNumericFilter(string fieldName, string min, string max)
        {
            FieldName = fieldName;
            Min = min;
            Max = max;
        }
    }
}