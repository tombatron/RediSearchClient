namespace RediSearchClient.Query
{
    /// <summary>
    /// Available languages for query stemming.
    /// </summary>
    public sealed class SearchLanguage
    {
        /// <summary>
        /// Arabic.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Arabic = new SearchLanguage("Arabic");

        /// <summary>
        /// Basque.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Basque = new SearchLanguage("Basque");

        /// <summary>
        /// Catalan.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Catalan = new SearchLanguage("Catalan");

        /// <summary>
        /// Danish.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Danish = new SearchLanguage("Danish");

        /// <summary>
        /// Dutch.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Dutch = new SearchLanguage("Dutch");

        /// <summary>
        /// English (default).
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage English = new SearchLanguage("English");

        /// <summary>
        /// Finnish.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Finnish = new SearchLanguage("Finnish");

        /// <summary>
        /// French.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage French = new SearchLanguage("French");

        /// <summary>
        /// German.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage German = new SearchLanguage("German");

        /// <summary>
        /// Greek.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Greek = new SearchLanguage("Greek");

        /// <summary>
        /// Hungarian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Hungarian = new SearchLanguage("Hungarian");

        /// <summary>
        /// Indonesian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Indonesian = new SearchLanguage("Indonesian");

        /// <summary>
        /// Irish.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Irish = new SearchLanguage("Irish");

        /// <summary>
        /// Italian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Italian = new SearchLanguage("Italian");

        /// <summary>
        /// Lithuanian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Lithuanian = new SearchLanguage("Lithuanian");

        /// <summary>
        /// Nepali.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Nepali = new SearchLanguage("Nepali");

        /// <summary>
        /// Norwegian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Norwegian = new SearchLanguage("Norwegian");

        /// <summary>
        /// Portuguese.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Portuguese = new SearchLanguage("Portuguese");

        /// <summary>
        /// Romanian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Romanian = new SearchLanguage("Romanian");

        /// <summary>
        /// Russian.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Russian = new SearchLanguage("Russian");

        /// <summary>
        /// Spanish.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Spanish = new SearchLanguage("Spanish");

        /// <summary>
        /// Swedish.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Swedish = new SearchLanguage("Swedish");

        /// <summary>
        /// Tamil.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Tamil = new SearchLanguage("Tamil");

        /// <summary>
        /// Turkish.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Turkish = new SearchLanguage("Turkish");

        /// <summary>
        /// Chinese.
        /// </summary>
        /// <returns></returns>
        public static readonly SearchLanguage Chinese = new SearchLanguage("Chinese");

        /// <summary>
        /// Initialize language.
        /// </summary>
        /// <value></value>
        public string Language { get; }

        private SearchLanguage(string searchLanguage) =>
            Language = searchLanguage;

        /// <summary>
        /// String representation of the initialized language.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Language;

        /// <summary>
        /// Implicit conversion to a string.
        /// </summary>
        /// <param name="language"></param>
        public static implicit operator string(SearchLanguage language) =>
            language.ToString();
    }
}