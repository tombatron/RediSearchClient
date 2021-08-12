using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RediSearchClient.SampleData
{
    public class NobelLaureate
    {
        public static IEnumerable<Person> People
        {
            get
            {
                var laureateDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "laureate.json");
                var laureateDataStream = File.OpenRead(laureateDataPath);

                var options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };

                var deserializedLaureateData = JsonSerializer.DeserializeAsync<Container>(laureateDataStream, options)
                    .GetAwaiter().GetResult();

                return deserializedLaureateData?.Laureates;
            }
        }

        public sealed class Container
        {
            public Person[] Laureates { get; set; }
        }

        public sealed class Affiliation
        {
            [JsonPropertyName("name")] public string Name { get; set; }

            [JsonPropertyName("city")] public string City { get; set; }

            [JsonPropertyName("country")] public string Country { get; set; }
        }

        public sealed class Prize
        {
            public string Year { get; set; }

            public int YearInt => int.Parse(Year);

            public string Category { get; set; }

            public string Share { get; set; }

            public string Motivation { get; set; }

            [JsonPropertyName("affiliations")] public Affiliation[] Affiliations { get; set; }
        }

        public sealed class Person
        {
            public string Id { get; set; }

            public string FirstName { get; set; }

            public string Surname { get; set; }

            public string Born { get; set; }

            public double BornSeconds => ToSeconds(Born);

            public string Died { get; set; }

            public double DiedSeconds => ToSeconds(Died);

            public string BornCountry { get; set; }

            public string BornCountryCode { get; set; }

            public string BornCity { get; set; }

            public string DiedCountry { get; set; }

            public string DiedCountryCode { get; set; }

            public string DiedCity { get; set; }

            public string Gender { get; set; }

            public Prize[] Prizes { get; set; }

            private static double ToSeconds(string stringDate)
            {
                if (stringDate is null or "0000-00-00")
                {
                    return 0;
                }

                if (stringDate.EndsWith("-00-00"))
                {
                    var year = int.Parse(stringDate.Substring(0, 4));

                    return (new DateTime(year, 1, 1) - DateTime.MinValue).TotalSeconds;
                }

                return (DateTime.Parse(stringDate) - DateTime.MinValue).TotalSeconds;
            }
        }
    }
}