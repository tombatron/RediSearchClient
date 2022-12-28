using System;

namespace RediSearchClient.Attributes
{
    /// <summary>
    /// Use to specify if a property is indexable, defaults to true
    /// </summary>
    public sealed class IndexAttribute : Attribute
    {
        public bool Value { get; private set; }

        public IndexAttribute(bool value)
        {
            Value = value;
        }

        public IndexAttribute()
        {
            Value = true;
        }
    }
}
