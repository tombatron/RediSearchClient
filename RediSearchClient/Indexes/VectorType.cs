namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Specifies the available precision for vector values.
    /// </summary>
    public enum VectorType
    {
        /// <summary>
        /// 32 bit.
        /// </summary>
        FLOAT32 = 0,

        /// <summary>
        /// 64 bit.
        /// </summary>
        FLOAT64 = 1
    }
}
