namespace RediSearchClient.Query
{
    /// <summary>
    /// Builder interface applying a numeric filter to a query.
    /// </summary>
    public interface IRediSearchNumericFilter
    {
        /// <summary>
        /// The name of the numeric field to filter on. 
        /// </summary>
        /// <value></value>
        string FieldName { get; }

        /// <summary>
        /// The minimum value in a numeric range. This field is a string so that it's possible
        /// to provide "-inf" or prefix the lower end of a range with "(" to indicate an exclusive
        /// range.
        /// </summary>
        /// <value></value>
        string Min { get; }

        /// <summary>
        /// The maximum value in a numeric range. This field is a string so that it's possible
        /// to provide "+inf" or prefix the higher end of a range with "(" to indicate an exclusive
        /// range.
        /// </summary>
        /// <value></value>
        string Max { get; }
    }
}