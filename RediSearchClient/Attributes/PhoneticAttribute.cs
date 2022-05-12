using RediSearchClient.Indexes;
using System;

namespace RediSearchClient.Attributes
{
    public class PhoneticAttribute : Attribute
    {
        public Language Language { get; private set; }

        public PhoneticAttribute(Language language)
        {
            Language = language;
        }
    }
}
