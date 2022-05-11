using RediSearchClient;
using RediSearchClient.Indexes;
using RediSearchClient.SampleData;
using StackExchange.Redis;
using System;
using System.Linq;
using NReJSON;

using var muxr = ConnectionMultiplexer.Connect("localhost");
var db = muxr.GetDatabase();

NReJSONSerializer.SerializerProxy = new SerializerProxy();

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
            .OnHash()
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
            .OnHash()
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

Console.WriteLine("Creating auto suggest dictionaries.");

if (db.SuggestionsSize("cities") == 0)
{
    foreach (var (_, hash) in ZipCodeData.ZipCodes)
    {
        try
        {
            db.AddSuggestion("cities", (string)hash[1].Value, 1, false, (String)hash[3].Value);
        }
        catch (RedisServerException ex)
        {
            //Console.WriteLine($"Exception while adding auto-suggestion: {hash[1].Value}, {hash[3].Value}, {ex.Message}");
        }

    }

    Console.WriteLine("`cities` auto suggest dictionary created.");
}
else
{
    Console.WriteLine("`cities` auto suggestion dictionary already exists.");
}

Console.WriteLine($"Loading Nobel Laureate (JSON) data into Redis.");

if (db.KeyExists("laureate::459"))
{
    Console.WriteLine("Looks like we've already got Nobel laureate data loaded.");
}
else
{
    foreach (var p in NobelLaureate.People)
    {
        db.JsonSet($"laureate::{p.Id}", p);
    }
}

Console.WriteLine("Nobel laureate data loaded.");

if (db.ListIndexes().Any(x => x == "nobel"))
{
    db.DropIndex("nobel");
}

db.CreateIndex("nobel",
    RediSearchIndex
        .OnJson()
        .ForKeysWithPrefix("laureate::")
        .WithSchema(
            x => x.Text("$.Id", "Id")
            , x => x.Text("$.FirstName", "FirstName", sortable: true)
            , x => x.Text("$.Surname", "LastName", sortable: true)
            , x => x.Numeric("$.BornSeconds", "Born", sortable: true)
            , x => x.Numeric("$.DiedSeconds", "Died", sortable: true)
            , x => x.Text("$.BornCountry", "BornCountry")
            , x => x.Text("$.BornCountyCode", "BornCountryCode")
            , x => x.Text("$.DiedCountry", "DiedCountry")
            , x => x.Text("$.DiedCountryCode", "DiedCountryCode")
            , x => x.Text("$.DiedCity", "DiedCity")
            , x => x.Text("$.Gender", "Gender")
            , x => x.Numeric("$.Prizes[*].YearInt", "YearAwarded")
            , x => x.Text("$.Prizes[*].Category", "AwardCategory")
            , x => x.Text("$.Prizes[*].Share", "AwardSharedWith")
            , x => x.Text("$.Prizes[*].Motivation", "Motivation")
            , x => x.Text("$.Prizes[*].affiliations[*].name", "InstitutionName")
            , x => x.Text("$.Prizes[*].affiliations[*].city", "InstitutionCity")
            , x => x.Text("$.Prizes[*].affiliations[*].country", "InstitutionCountry")
        )
        .Build()
);