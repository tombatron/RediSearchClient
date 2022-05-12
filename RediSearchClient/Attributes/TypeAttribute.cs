using System;

namespace RediSearchClient.Attributes
{
    public class TypeAttribute : Attribute
    {
        public string Name { get; private set; }

        public TypeAttribute(string name)
        {
            Name = name;
        }
    }
}
