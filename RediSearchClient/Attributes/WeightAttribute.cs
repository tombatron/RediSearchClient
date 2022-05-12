using System;

namespace RediSearchClient.Attributes
{
    public class WeightAttribute : Attribute
    {
        public int Weight { get; private set; }

        public WeightAttribute(int weight)
        {
            Weight = weight;
        }
    }
}
