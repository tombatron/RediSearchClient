using System;

namespace RediSearchClient.Attributes
{
    public sealed class TagAttribute : Attribute
    {
        public string Separator { get; private set; } = ",";

        public TagAttribute(string separator)
        {
            Separator = separator;
        }

        public TagAttribute()
        {

        }
    }
}
