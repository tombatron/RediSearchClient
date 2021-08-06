# RediSearchClient
[![Build Status](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml)

Special thanks to [JetBrains](https://jb.gg/OpenSource) for providing the project with an open source license for all of the JetBrains products!

![](jetbrains.svg)

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
* [Updating Index Schema](#updating-index-schema)
* [Deleting and Index](#deleting-an-index)
* [Index Aliases](#index-aliases)
* [Executing a Query](#executing-a-query)
* [Executing an Aggregation](#executing-an-aggregation)
* [Auto-Complete Suggestions](#auto-complete-suggestions)
* [Spell Check](#spell-check)
    - [Custom Dictionaries](#custom-dictionaries)
* [Tag Values](#tag-values)
* [Synonyms](#synonyms)

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

### Updating Index Schema

You can utilize the `AlterSchema` or `AlterSchemaAsync` to expand the schema of an existing index. The method takes an index name and leverages the same schema builder that is used when creating an index. 

Let's take a look at an example where I'm adding a TEXT field and a GEO field to an existing index.

```csharp
_db.AlterSchemaAsync("existing_index", 
    fb => fb.Text("NewTextField", noindex: true), 
    fb => fb.Numeric("NewNumericField", sortable: true)
);
```

### Deleting an Index

Deleting an index is done by using the `DropIndex` or `DropIndexAsync` extension method. 

```csharp
await _db.DropIndexAsync("name_of_the_index");
```

If you attempt to delete an index that doesn't exist a `RedisServerException` will be thrown.

Should you want to delete all of the associated hash documents (the things that RediSearch indexed) from Redis along with the search index, you can provide a value to the optional parameter `dropDocumentHashes`...

```csharp
await _db.DropIndexAsync("name_of_the_index", deleteDocumentHashes: true);
```

### Index Aliases

Index aliases can be very useful. A common scenario would be where you have a search index whos documents are updated on a schedule (let's say twice a day). If our program leveraged the non-aliased index name, it's possible that twice a day it would be searching partially updated data. However, if the program treated the indexes as "transient" and created a new one for each refresh, we could leverage aliases to ensure your application is always querying the latest, fully populated index.

Keep in mind, an index can have multiple aliases, but you *cannot* create an alias that refers to another alias. 

Adding an alias (`AddAlias` or `AddAliasAsync`)...

```csharp
await _db.AddAliasAsync("alias_of_the_index", "name_of_index_being_aliased");
```

When you're ready to reassign an alias to a new index (`UpdateAlias` or `UpdateAliasAsync`)...

```csharp
await _db.UpdateAliasAsync("alias_of_the_index", "name_of_a_new_index");
```

And finally, if you want to delete an alias (`DeleteAlias` or `DeleteAliasAsync`)...

```csharp
await _db.DeleteAliasAsync("alias_to_delete");
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

#### Mapping the Result

See [this](https://github.com/tombatron/RediSearchClient/wiki/ResultMapping) documentation for defining a type mapping.

With the appropriate mapping defined you can now map the result like...

```csharp
var result = await _db.SearchAsync(queryDefinition);

var mappedResult = result.As<ZipCode>();
```

The variable `mappedResult` will be of type `IEnumerable<ZipCode>`.

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
    .SortBy(sb => 
    {
        sb.Field("@City", Direction.Ascending);
        sb.Field("@Count", Direction.Descending);
        sb.Max(100);
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

#### Mapping the Result

See [this](https://github.com/tombatron/RediSearchClient/wiki/ResultMapping) documentation for defining a type mapping.

With the appropriate mapping defined you can now map the result like...

```csharp
var result = await _db.AggregateAsync(zipCodeCountByProximity);

var mappedResult = result.As<CityCount>();
```

The variable `mappedResult` will be of type `IEnumerable<CityCount>`.

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

### Spell Check

RediSearch gives us the ability to "spell check" our queries.  This is useful when a query is being executed based on user input.  If the initial query yields no results we can use the spell check functionality to get suggestions to either return to the user or to use when re-executing the query. 

For more information about the spelling correction functionality see the official documentation [here](https://oss.redislabs.com/redisearch/Spellcheck/).

#### Invoking "SpellCheck" for Suggestions

The `SpellCheck` and `SpellCheckAsync` methods can accept a query definition or a plain string-based query. 

Consider the following examples:

```csharp
// Passing in the query directly... 
var result = await _db.SpellCheckAsync("simple_movie_index", "@Title:ghustbosters @Genre:{Fantasy}", 4);
```

```csharp
// Passing in the query definition from the query builder...
var query = RediSearchQuery
    .On("simple_movie_index")
    .UsingQuery("@Title:ghustboster @Genre:{Fantasy}")
    .Build();

var result = await _db.SpellCheckAsync(query, 4);
```

#### Handling "SpellCheck" Results

The results come back in the form of array of `SpellCheckResult` objects. 

Each `SpellCheckResult` contains a property labeled `Term` which describes the word being spell checked. The next property is `Suggestions` which is an array of `Suggestion` classes containing a suggestion and a score for the suggestion. 

That being said, if we wanted to load the first suggestion for a specific term, we could do something like the following:

```csharp
var result = await _db.SpellCheckAsync(query, distance: 4);

var titleSuggestion = result?["ghustbosters"]?
    .Suggestions?.FirstOrDefault()?.Value;
```

In the above example, I'm using null propagation because if there are no suggestions for a given query we wouldn't want a runtime exception would we?

#### A "Practical" Usage

The following method will return a value tuple with the `movieName` and the `released` year. If the `movieName` provided isn't found the method will invoke the `SpellCheck` command, grab the first suggestion and recursively call itself with the suggested `@Title`, returning once a match is found or spell check yields no results.

```csharp
public (string movieName, long released) FindReleaseYear(string movieName)
{
	var query = RediSearchQuery
		.On("movies")
		.UsingQuery($"@Title:{movieName}")
		.Return("Title", "Released")
		.Build();

	var result = _db.Search(query);

	if (result.Any())
	{
		var match = result.Select(x => new
		{
			Title = (String)x["Title"],
			Released = (long)x["Released"]
		}).First();

		return (match.Title, match.Released);
	}
	else
	{
		var spellCheckResult = _db.SpellCheck(query, 2);

		var suggestion = spellCheckResult?[movieName]?
			.Suggestions?
			.FirstOrDefault()?
			.Value;
			
		if (suggestion == null)
		{
			return ("No Matches", 0);
		}
		else
		{
			return FindReleaseYear(suggestion);
		}
	}
}
```

#### Custom Dictionaries

Dictionaries can be used to include and exlude terms from potential spell check results.

For more information about this functionality, see the [official documentation](https://oss.redislabs.com/redisearch/Spellcheck/#custom_dictionaries).

##### Adding to a Dictionary

To create a new dictionary or add to an existing dictionary you'll use the `AddToDictionary` or `AddToDictionaryAsync` methods. 

```csharp
var result = await _db.AddToDictionaryAsync("test_dictionary", "super", "cool", "example", "term");
```

The `result` in the above example will be equal to the number of terms that were added to the dictionary. ie If the dictionary didn't contain the words "super", "cool", "example", and "term" then the result would be `4`.

##### Removing from a Dictionary

In order to remove terms from a dictionary you'll use the `DeleteFromDictionary` or `DeleteFromDictionaryAsync` methods.

```csharp
var result = await _db.DeleteFromDictionaryAsync("test_dictionary", "example");
```

Just like adding to a dictionary, the result here will be an integer representing the number of terms affected (or in this case deleted).

##### Dumping a Dictionary

If you want to examine the contents of an **existing** dictionary you'll use the `DumpDictionary` or `DumpDictionaryAsync` methods.

```csharp
var result = await _db.DumpDictionaryAsync("test_dictionary");
```

The result will be an array of strings representing all of the terms that exist in the dictionary. If you provide the name of a dictionary that doesn't exist you'll encounter a `RedisServerException` with a message of "could not open dict key".

##### Using a Dictionary

In order leverage a custom dictionary with your call to spell check you'll create an instance of the `SpellCheckTerm` class to specify the name of the dictionary and whether or not it should be included or excluded from analysis.

You can pass one or more dictionaries into the `SpellCheck` or `SpellCheckAsync` function(s). 

```csharp
var testDictionary = new SpellCheckTerm 
{ 
    DictionaryName = "test_dictionary", 
    Treatment = TermTreatment.Include 
};

var result = await _db.SpellCheckAsync(query, testDictionary);
```

The above call to `SpellCheckAsync` will include terms from the "test_dictionary" in its analysis. 

For information on handling the result from `SpellCheckAsync` check [this](#handling-spellcheck-results) out. 

### Tag Values

When you are defining a schema, one of the field types available to you is the `TAG` field type. This field type allows for exact-match queries against values like categories or primary keys. 

RediSearch gives you access to enumerate all of the tag values by index and field by using the `FT.TAGVALS` command.

The following example demonstrates how to leverage the `FT.TAGVALS` command with RediSearchClient by issuing the command against the [sample](#sample-data) "movies" index and the "Genre" field.

```csharp
var tagValues = await _db.TagValuesAsync("movies", "Genre");
```

#### Handling the Result

The result of the `FT.TAGVALS` command is a simple, unpaged, unordered, lower-cased, stripped of whitespaces array of strings representing all of the tags applied to the specified field on the specified index.

### Synonyms

RediSearch allows for defining synonym groups which can make your full-text queries a bit more useful. For example, if you defined a synonym group for the words "boy", "girl", "child", a search for the world "child" would also yield results for "boy" and "girl".

#### Updating a Synonym Group

Defining or updating a synonym group are both done with the `FT.SYNUPDATE` command. 

For the RediSearchClient you'll use the `UpdateSynonyms` or `UpdateSynonymsAsync` command.

For example...

```csharp
await _db.UpdateSynonymsAsync("sample_index", "example_group_id", "boy", "girl", "child");
```

#### Dumping a Synonym Group

In order to retrieve previously defined synonyms you'd execute the `FT.SYNDUMP` command with the name of the index that the synonymn groups were defined on. 

```csharp
var result = await _db.DumpSynonymsAsync("sample_index");
```

The result is an array of `SynonymGroupElement` objects which define the synonym and the synonym groups that the synonym belongs to.