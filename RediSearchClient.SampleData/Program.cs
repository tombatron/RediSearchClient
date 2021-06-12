using RediSearchClient;
using RediSearchClient.Indexes;
using RediSearchClient.SampleData;
using StackExchange.Redis;
using System;
using System.Linq;

using var muxr = ConnectionMultiplexer.Connect("localhost");
var db = muxr.GetDatabase();

Console.WriteLine("Loading movie data into Redis.");

if (db.KeyExists("movie::1"))
{
    Console.WriteLine("Looks like we've already got some movie data here.");
}
else
{
    for (var i = 0; i < MovieData.Movies.Length; i++)
    {
        db.HashSet($"movie::{i + 1}", MovieData.Movies[i]);
    }

    Console.WriteLine("Movie data loaded.");
}

Console.WriteLine("Creating an index on the movie data called `movies`.");

if (db.ListIndexes().Any(x => x == "movies"))
{
    Console.WriteLine("Looks like we've already got a `movies` index.");
}
else
{
    db.CreateIndex("movies",
        RediSearchIndex
            .On(RediSearchStructure.HASH)
            .ForKeysWithPrefix("movie::")

            .WithSchema(
                x => x.Text("Plot"),
                x => x.Tag("Type"),
                x => x.Text("Poster", noindex: true),
                x => x.Numeric("imdbVotes"),
                x => x.Numeric("Runtime"),
                x => x.Tag("Rated"),
                x => x.Numeric("imdbRating"),
                x => x.Text("Writer"),
                x => x.Text("Director"),
                x => x.Numeric("YearEnd"),
                x => x.Numeric("Released"),
                x => x.Tag("Language"),
                x => x.Numeric("Metascore"),
                x => x.Text("imdbID"),
                x => x.Text("Title"),
                x => x.Text("Awards"),
                x => x.Tag("Genre"),
                x => x.Numeric("Year"),
                x => x.Text("Actors")
            )

            .Build()
    );

    Console.WriteLine("`movies` index created.");
}

Console.WriteLine("Loading zipcode data into Redis.");

if (db.KeyExists("zip::32506"))
{
    Console.WriteLine("Looks like we've already got zipcode data loaded.");
}
else
{
    foreach (var (zipcode, hash) in ZipCodeData.ZipCodes)
    {
        db.HashSet($"zip::{zipcode}", hash);
    }

    Console.WriteLine("Zipcode data loaded.");
}

Console.WriteLine("Creating an index on the zipcode data called `zips`.");

if (db.ListIndexes().Any(x => x == "zips"))
{
    Console.WriteLine("Looks like we've already got a `zips` index.");
}
else
{
    db.CreateIndex("zips",
        RediSearchIndex
            .On(RediSearchStructure.HASH)
            .ForKeysWithPrefix("zip::")

            .WithSchema(
                x => x.Text("ZipCode"),
                x => x.Text("City"),
                x => x.Text("State"),
                x => x.Geo("Coordinates"),
                x => x.Numeric("TimeZoneOffset"),
                x => x.Numeric("DaylightSavingsFlag")
            )

            .Build()
    );

    Console.WriteLine("`zips` index created.");
}