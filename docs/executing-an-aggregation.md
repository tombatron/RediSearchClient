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
