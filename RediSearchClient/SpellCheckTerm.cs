namespace RediSearchClient
{
    /// <summary>
    /// The class describes how include/exclude terms from the
    /// spell check.
    /// </summary>
    public class SpellCheckTerm
    {
        /// <summary>
        /// Are we including or excluding terms here?
        /// </summary>
        /// <value></value>
        public TermTreatment Treatment { get; set; }

        /// <summary>
        /// Name of the dictionary to include/exclude terms from.
        /// </summary>
        /// <value></value>
        public string DictionaryName { get; set; }

        /// <summary>
        /// Are we going to include or exclude the terms defined dictionary
        /// within the specified dictionary.
        /// </summary>
        public enum TermTreatment
        {
            /// <summary>
            /// Include terms in the dictionary.
            /// </summary>
            Include = 0,

            /// <summary>
            /// Exclude terms in the dictionary.
            /// </summary>
            Exclude = 1
        }
    }
}