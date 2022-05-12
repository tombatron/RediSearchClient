using System;

namespace RediSearchClient.Attributes
{
    public class AliasAttribute : Attribute
    {
        public string Name { get; private set; }

        public AliasAttribute(string name)
        {
            Name = name;
        }
    }
}
