using System;

namespace RediSearchClient.Attributes
{
    public sealed class AliasAttribute : Attribute
    {
        public string Name { get; private set; }

        public AliasAttribute(string name)
        {
            Name = name;
        }
    }
}
