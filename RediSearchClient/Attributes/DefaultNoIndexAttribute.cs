using System;

namespace RediSearchClient.Attributes
{
    /// <summary>
    /// All properties are indexable by default. This attribute will specify that properties on a type that is used to create a RediSearch JSON Schema are non-indexable by default, use [Index] on target properties to override this behavior
    /// </summary>
    public sealed class DefaultNoIndexAttribute : Attribute
    {

    }
}
