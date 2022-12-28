using System;

namespace RediSearchClient.Attributes
{
    public sealed class WeightAttribute : Attribute
    {
        public int Weight { get; private set; }

        public WeightAttribute(int weight)
        {
            Weight = weight;
        }
    }
}
