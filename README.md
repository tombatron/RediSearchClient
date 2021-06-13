# RediSearchClient
[![Build Status](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml)

## Overview

What you have here is a set of extensions for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) Redis client that allows for interacting with version 2.x of the RediSearch Redis module. 

Support for RedisJson "JSON" datatypes is probably the most exciting forthcoming feature of RediSearch, and once that support goes mainstream and is documented, expect this package to support that as well. 

### What about NRediSearch?

NRediSearch is woefully behind in terms of support for the latest functionality for the RediSearch module. That's not meant as a slight against the StackExchange team that maintains that package, it's simply not a priority for them. I thought, why not break out a brand new package that isn't coupled to the main StackExchange.Redis project. 

So here we are. 

## Installation

```
PM> Install-Package RediSearchClient
```

## Sample Data

Just need some sample data to play with? Hey, we've all been there. 

Included in this repository is a small console application that will load a local Redis instance with two sample datasets and create two sample indexes for you to mess around with. 

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
 
The result from the above query against the sample `zipcodes` index will yield an instance of the `SearchResult` class. The `SearchQuery` class is an implementation of `IEnumerable<SearchResultItem>`. Each instance of `SearchResultItem` represents a "row" in the result set, and gives you access to the Redis key as well as a dictionary of the stored fields for the search result. 

The following example demonstrates handling the `result` from the "Executing a Query" sample above and projecting it into an anonymous type:

```csharp
var result = await _db.SearchAsync(queryDefinition);

var floridaZipcodes = result.Select(x =>
{
    // This index defines "Coordinates" as "Geo" however when they are
    // returned they come back as a string. So we have to do a little
    // bit of post processing here. 
    var coordinates = ((string)x.Fields["Coordinates"]).Split(",");

    return new
    {
        x.DocumentKey,
        ZipCode = (string)x.Fields["ZipCode"],
        City = (string)x.Fields["City"],
        State = (string)x.Fields["State"],
        Latitude = double.Parse(coordinates[1]),
        Longitude = double.Parse(coordinates[0]),
        TimeZoneOffset = (int)x.Fields["TimeZoneOffset"],
        DaylightSavingsFlag = (bool)x.Fields["DaylightSavingsFlag"]
    };
});
```