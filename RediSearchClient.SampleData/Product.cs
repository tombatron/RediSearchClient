using RediSearchClient.Attributes;
using System;
using System.Collections.Generic;

namespace RediSearchClient.SampleData.TestingTypes
{
    [DefaultNoIndex]
    public class Product
    {
        [Tag]
        [Index]
        [Alias("id")]
        public string Identifier { get; set; }

        [Sortable]
        public decimal Price { get; set; }

        public string Meta { get; set; }

        [SchemaIgnore]
        public int Count { get; set; }

        public Dimensions Dimensions { get; set; }

        public ICollection<Seller> Sellers { get; set; }

        public Product RelatedProduct { get; set; } //Will be ignored to avoid recursion

        public DateTime DateAdded { get; set; }
    }
}