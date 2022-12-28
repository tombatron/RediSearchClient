using System.Collections.Generic;

namespace RediSearchClient.SampleData.TestingTypes
{
    public class Seller
    {
        public string Name { get; set; }

        public IEnumerable<Product> SoldProducts { get; set; } //Will be ignored to avoid recursion

        public IEnumerable<string> Locations { get; set; }
    }
}