namespace RediSearchClient.Query
{
    public sealed class SearchLanguage
    {
        public static readonly SearchLanguage Arabic = new SearchLanguage("Arabic");

        public static readonly SearchLanguage Basque = new SearchLanguage("Basque");

        public static readonly SearchLanguage Catalan = new SearchLanguage("Catalan");

        public static readonly SearchLanguage Danish = new SearchLanguage("Danish");

        public static readonly SearchLanguage Dutch = new SearchLanguage("Dutch");

        public static readonly SearchLanguage English = new SearchLanguage("English");

        public static readonly SearchLanguage Finnish = new SearchLanguage("Finnish");

        public static readonly SearchLanguage French = new SearchLanguage("French");

        public static readonly SearchLanguage German = new SearchLanguage("German");

        public static readonly SearchLanguage Greek = new SearchLanguage("Greek");

        public static readonly SearchLanguage Hungarian = new SearchLanguage("Hungarian");

        public static readonly SearchLanguage Indonesian = new SearchLanguage("Indonesian");

        public static readonly SearchLanguage Irish = new SearchLanguage("Irish");

        public static readonly SearchLanguage Italian = new SearchLanguage("Italian");

        public static readonly SearchLanguage Lithuanian = new SearchLanguage("Lithuanian");

        public static readonly SearchLanguage Nepali = new SearchLanguage("Nepali");

        public static readonly SearchLanguage Norwegian = new SearchLanguage("Norwegian");

        public static readonly SearchLanguage Portuguese = new SearchLanguage("Portuguese");

        public static readonly SearchLanguage Romanian = new SearchLanguage("Romanian");

        public static readonly SearchLanguage Russian = new SearchLanguage("Russian");

        public static readonly SearchLanguage Spanish = new SearchLanguage("Spanish");

        public static readonly SearchLanguage Swedish = new SearchLanguage("Swedish");

        public static readonly SearchLanguage Tamil = new SearchLanguage("Tamil");

        public static readonly SearchLanguage Turkish = new SearchLanguage("Turkish");

        public static readonly SearchLanguage Chinese = new SearchLanguage("Chinese");

        public string Language { get; }

        private SearchLanguage(string searchLanguage) =>
            Language = searchLanguage;

        public override string ToString() =>
            Language;

        public static implicit operator string(SearchLanguage language) =>
            language.ToString();
    }
}