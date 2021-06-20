# RediSearchClient
[![Build Status](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml)

## Overview

What you have here is a set of extensions for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) Redis client that allows for interacting with version 2.x of the RediSearch Redis module. 

Support for RedisJson "JSON" datatypes is probably the most exciting forthcoming feature of RediSearch, and once that support goes mainstream and is documented, expect this package to support that as well. 

### What about NRediSearch?

NRediSearch is woefully behind in terms of support for the latest functionality for the RediSearch module. That's not meant as a slight against the StackExchange team that maintains that package, it's simply not a priority for them. I thought, why not break out a brand new package that isn't coupled to the main StackExchange.Redis project. 

So here we are. 

## Table of Contents

* [Installation](#installation)
* [Sample Data](#sample-data)
* [Creating an Index](#creating-an-index)

## Installation

```
PM> Install-Package RediSearchClient
```

## Sample Data

Just need some sample data to play with? Hey, we've all been there. 

Included in this repository is a small console application that will load a local Redis instance with two sample datasets and create two samples indexes and an auto-suggest dictionary for you to mess around with. 

You can load the sample data by following these steps:

1. Ensure that you have a Redis instance running (with the RediSearch 2.x module loaded). See the [RediSearch Quick Start](https://oss.redislabs.com/redisearch/Quick_Start/) if you don't already have that. 

2. Clone this repository. 

3. Execute the following command from within the root of the cloned repository: `dotnet run --project .\RediSearchClient.SampleData\RediSearchClient.SampleData.csproj`

And that's pretty much it!

I suggest before writing any code you play around with query language using the sample data/indexes that we just loaded using a tool like [RedisInsight](https://redislabs.com/redis-enterprise/redis-insight/) which has support for the RediSearch (and a few others) module.

## Usage

Where appropriate the following examples will use the sample data and indexes that are provided by the `RediSearchClient.SampleData` application.

### Creating an Index.

Creating an index is done by using the `RediSearchIndex` builder to create an index definitions and then invoking the `CreateIndex` or `CreateIndexAsync` extension method. 

```csharp
var indexDefinition = RediSearchIndex
    .On(RediSearchStructure.HASH)
    .ForKeysWithPrefix("zip::")
    .WithSchema(
        x => x.Text("ZipCode", sortable: false, nostem: true),
        x => x.Text("City", sortable: true), 
        x => x.Text("State", sortable: true, nostem: true),
        x => x.Geo("Coordinates"),
        x => x.Numeric("TimeZoneOffset"),
        x => x.Numeric("DaylightSavingsFlag")
    )
    .Build();

await _db.CreateIndexAsync("zipcodes", indexDefinition);
```

#### Dates and Times

The only field types that RediSearch supports are "Text", "Tag", "Numeric", and "Geo". No dates and times.

If you need to store and query date/time data I suggest you convert it to a numeric value and index it that way. 

The sample "movies" index does this with the "Released" field. The value in that field represents the number of seconds that have elapsed since `DateTime.MinValue`. Why not use the standard "Unix epoch"?! There's nothing stopping you from doing that, it's just a number. In this specific case however, some of the movies in the index were released before 1970 so you'd have negative values in your index. Still not a deal breaker, but it'd require extra handling later (I think). 

Here are a couple of helper methods that you could use to convert a DateTime back and forth to seconds:

```csharp
public static double ToSeconds(DateTime dateTime) =>
	(dateTime - DateTime.MinValue).TotalSeconds;
	
public static DateTime FromSeconds(double seconds) =>
	DateTime.MinValue.AddSeconds(seconds);
```

The following is what a query involving a date range might look like, again using the sample "movies" index...

```csharp
// Movies released in 1982.
var startDate = new DateTime(1982, 1, 1);
var endDate = startDate.AddYears(1);

var query = RediSearchQuery
    .On("movies")
    .UsingQuery($"@Released:[{ToSeconds(startDate)} {ToSeconds(endDate)}]")
    .Build();

var result = _db.Search(query);

var movies = result.Select(x => 
    new {
        Key = x.DocumentKey,
        Title = (string)x["Title"],
        Released = FromSeconds((double)x["Released"])
    });
```

### Executing a Query

Searching an index is done by using the `RediSearchQuery` builder to create a RediSearch query and then executing the query using the `Search` or `SearchAsync` extension method. 

```csharp
var queryDefinition = RediSearchQuery
        .On("zipcodes")
        .UsingQuery("@State:FL")
        .Build()
    );

var result = await _db.SearchAsync(queryDefinition);
```

#### Handling the Result
 
The result from the above query against the sample `zipcodes` index will yield an instance of the `SearchResult` class. The `SearchResult` class is an implementation of `IEnumerable<SearchResultItem>`. Each instance of `SearchResultItem` represents a "row" in the result set, and gives you access to the Redis key as well as a dictionary of the stored fields for the search result. 

The following example demonstrates handling the `result` from the "Executing a Query" sample above and projecting it into an anonymous type:

```csharp
var result = await _db.SearchAsync(queryDefinition);

var floridaZipcodes = result.Select(x =>
{
    // This index defines "Coordinates" as "Geo" however when they are
    // returned they come back as a string. So we have to do a little
    // bit of post processing here. 
    var coordinates = ((string)x["Coordinates"]).Split(",");

    return new
    {
        x.DocumentKey,
        ZipCode = (string)x["ZipCode"],
        City = (string)x["City"],
        State = (string)x["State"],
        Latitude = double.Parse(coordinates[1]),
        Longitude = double.Parse(coordinates[0]),
        TimeZoneOffset = (int)x["TimeZoneOffset"],
        DaylightSavingsFlag = (bool)x["DaylightSavingsFlag"]
    };
});
```

### Executing an Aggregation

Executing an aggregation is a lot like doing a standard search, except we'll be using the `RediSearchAggregateQuery` builder in order to create the aggregation query and then using the `Aggregate` or `AggregateAsync` extension methods to invoke the actual aggregation.

The following example demonstrates 

```csharp
var zipCodeCountByProximity = RediSearchAggregateQuery.On("zips")
    .Query("@Coordinates:[-87.320330 30.423090 10 mi]")
    .GroupBy(gb =>
    {
        gb.Fields("@City");
        gb.Reduce(Reducer.Count).As("Count");
    })
    .Build();

var result = await _db.AggregateAsync(zipCodeCountByProximity);
```

#### Handling the Result

The result from the above aggregation query of the zip code data in the sample `zipcodes` index will yield an instance of the `AggregateResult` class. The `AggregateResult` class is an implementation of the `IEnumerable<AggregateResultCollection>`. The `AggregateResultCollection` is an abstraction on top of a collection of `KeyValuePair<string, RedisResult>` which gives us the ability to easily access the fields for each "row" in the result set. 

The following example demonstrates handling the `result` from the "Executing an Aggregation" sample above and projecting it into an anonymous type:

```csharp
var result = await _db.AggregateAsync(zipCodeCountByProximity);

var cityZipCodes = result.Select(x => new
{
    City = (string)x["City"],
    Count = (int)x["Count"]
});
```

### Auto-Complete Suggestions

RediSearch includes the ability to maintain a special index or dictionary for the purposes of creating suggestions based on input.

The sample data project included in this repository will populate an auto-complete suggestion dictionary with city names as the string to index and the coordinates in longitude,latitude format as the payload.

#### Populating an Auto-Complete Suggestion Dictionary

In order to populate an auto-complete suggestion dictionary you'll use the `AddSuggestion` or `AddSuggestionAsync` extension methods.

The following is an example of adding an entries to the `cities` auto-complete suggestion dictionary:

```csharp
await db.AddSuggestionAsync("cities", "Pensacola", 1, payload: "-87.275772,30.61428");
```

In the above example notice that I'm providing the optional `payload` parameter. The auto-complete suggestion dictionary gives us the opportunity to store a little bit of additional data, so in this case I'm storing the coordinates of the city. 

#### Querying an Auto-Complete Suggestion Dictionary

Querying an auto-complete suggestion dictionary is pretty straight forward as it only requires calling the `GetSuggestions` or `GetSuggestionsAsync` method with the name of the dictionary and the partial term that we're searching for...

```csharp
var suggestions = await _db.GetSuggestionsAsync("cities", "pensac");
```

By default, we will get a maximum of five results back. In order to expand that, we can provide the optional `max` parameter with the number of results that we want...

```csharp
var suggestions = await _db.GetSuggestionsAsync("cities", "pensac", max: 15);
```

We can further augment our query by providing the following optional parameters:

* `fuzzy`: Will perform a "fuzzy" search for terms within a Levenshtein distance of 1.
* `withScores`: Will populate the score of a match in the result of the query. 
* `withPayload`: Will populate the payload of a match in the result of the query.

#### Removing Entries from an Auto-Complete Suggestion Dictionary

Should it become necessary, we also have the ability to remove terms from an auto-complete suggestion dictionary. 

The following is an example of removing an entry...

```csharp
await _db.DeleteSuggestionAsync("cities", "Destin");
```