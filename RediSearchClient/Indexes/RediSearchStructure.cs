namespace RediSearchClient.Indexes
{
    /// <summary>
    /// Supported structures to apply a RediSearch index to. 
    /// </summary>
    public enum RediSearchStructure
    {
        /// <summary>
        /// Raw documents are "Hash" data types. 
        /// </summary>
        HASH,
        
        /// <summary>
        /// Raw documents are "JSON" data types.
        ///
        /// This requires that the latest version of RedisJson and version 2.2+ of RediSearch are installed.
        /// </summary>
        JSON
    }
}