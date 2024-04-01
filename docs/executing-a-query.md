### Executing a Query

Searching an index is done by using the `RediSearchQuery` builder to create a RediSearch query and then executing the query using the `Search` or `SearchAsync` extension method. 

```csharp
var queryDefinition = RediSearchQuery
        .On("zipcodes")
        .UsingQuery("@State:FL")
        .Build();

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