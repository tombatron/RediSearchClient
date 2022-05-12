using RediSearchClient.Attributes;
using System;
using System.Collections.Generic;

namespace RediSearchClient.SampleData.TestingTypes
{
    [Type("product")]
    public class Product
    {
        [Alias("id")]
        public string Identifier { get; set; }

        [Sortable]
        public decimal Price { get; set; }

        [NoIndex]
        public string Meta { get; set; }

        public Dimensions Dimensions { get; set; }

        public IEnumerable<Seller> Sellers { get; set; }
    }
}
