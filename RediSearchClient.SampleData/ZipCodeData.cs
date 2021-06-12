using StackExchange.Redis;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RediSearchClient.SampleData
{
    public static class ZipCodeData
    {
        // Source: https://public.opendatasoft.com/explore/dataset/us-zip-code-latitude-and-longitude/export/
        public static IEnumerable<(string zipCode, HashEntry[])> ZipCodes
        {
            get
            {
                var zipDataPath = Path.Combine(Directory.GetCurrentDirectory(), "zips.txt");
                var zipDataStream = File.OpenRead(zipDataPath);

                var deserializedZipData = JsonSerializer.DeserializeAsync<IEnumerable<ZipData>>(zipDataStream).GetAwaiter().GetResult();

                foreach (var zip in deserializedZipData)
                {
                    yield return (zip.ZipCode, new[]
                    {
                        new HashEntry("ZipCode", zip.ZipCode),
                        new HashEntry("City", zip.City),
                        new HashEntry("State", zip.State),
                        new HashEntry("Coordinates", $"{zip.Latitude},{zip.Longitude}"),
                        new HashEntry("TimeZoneOffset", zip.TimeZoneOffset),
                        new HashEntry("DaylightSavingsFlag", zip.DaylightSavingsFlag),
                    });
                }
            }
        }

        public class ZipData
        {
            public string ZipCode { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public double TimeZoneOffset { get; set; }

            public int DaylightSavingsFlag { get; set; }
        }
    }
}