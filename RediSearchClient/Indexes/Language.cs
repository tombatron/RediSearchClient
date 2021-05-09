namespace RediSearchClient.Indexes
{
    /// <summary>
    /// An enumeration indicating supported languages for a search index.
    /// 
    /// `None` will resort to "default" which in this case would essentially
    /// be "English".
    /// </summary>
    public enum Language
    {
        /// <summary>
		/// No language specified.
        /// </summary>
        None,

        /// <summary>
		/// English is specified as the default language for the search index.
        /// </summary>
        English,

        /// <summary>
		/// French is specified as the default language for the search index.
        /// </summary>
        French,

        /// <summary>
		/// Portuguese is specified as the default language for the search index.
        /// </summary>
        Portuguese,

        /// <summary>
		/// Spanish is specified as the default language for the search index.
        /// </summary>
        Spanish
    }
}