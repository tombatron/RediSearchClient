### Creating an Index.

Creating an index is done by using the `RediSearchIndex` builder to create an index definitions and then invoking the `CreateIndex` or `CreateIndexAsync` extension method. 

> Note: Where appropriate the following examples will use the sample data and indexes that are provided by the `RediSearchClient.SampleData` application.

The following is an example of creating a search index based on the "hash" data type. 

```csharp
var indexDefinition = RediSearchIndex
    .OnHash()
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
If your deployment of RediSearch supports JSON based indexes you can define one by specifying the `.OnJson` builder method when initiating the remainder of the builder. The following example is pulled from the sample data project and demonstrates how to define a JSON based index.

```csharp
var indexDefintion = RediSearchIndex
      .OnJson()
      .ForKeysWithPrefix("laureate::")
      .WithSchema(
          x => x.Text("$.Id", "Id"),
          x => x.Text("$.FirstName", "FirstName", sortable: true),
          x => x.Text("$.Surname", "LastName", sortable: true),
          x => x.Numeric("$.BornSeconds", "Born", sortable: true),
          x => x.Numeric("$.DiedSeconds", "Died", sortable: true),
          x => x.Text("$.BornCountry", "BornCountry"),
          x => x.Text("$.BornCountyCode", "BornCountryCode"),
          x => x.Text("$.DiedCountry", "DiedCountry"),
          x => x.Text("$.DiedCountryCode", "DiedCountryCode"),
          x => x.Text("$.DiedCity", "DiedCity"),
          x => x.Text("$.Gender", "Gender"),
          x => x.Numeric("$.Prizes[*].YearInt", "YearAwarded"),
          x => x.Text("$.Prizes[*].Category", "AwardCategory"),
          x => x.Text("$.Prizes[*].Share", "AwardSharedWith"),
          x => x.Text("$.Prizes[*].Motivation", "Motivation"),
          x => x.Text("$.Prizes[*].affiliations[*].name", "InstitutionName"),
          x => x.Text("$.Prizes[*].affiliations[*].city", "InstitutionCity"),
          x => x.Text("$.Prizes[*].affiliations[*].country", "InstitutionCountry")
      )
      .Build();
```

If you're using RediSearch as a vector database, you can now define a `VECTOR` field on hash-based and JSON-based indexes. 

The following is an example of defining a `VECTOR` field on a hash-based index:

```csharp
var indexDefinition = RediSearchIndex
    .OnHash()
    .WithSchema(
        x=> x.Text("Name"),
        x=> x.Vector("Embedding", 
            VectorIndexAlgorithm.FLAT(
                type: VectorType.FLOAT32, 
                dimensions: 32, 
                distanceMetric: DistanceMetric.L2, 
                initialCap: 30, 
                blockSize: 20))
    )
    .Build();
```

Next is an example of defining a `VECTOR` field on a JSON-based index:

```csharp
var indexDefinition = RediSearchIndex
    .OnJson()
    .WithSchema(
        x=> x.Text("$.Id", "Id"),
        x=> x.Vector("$.Embedding", alias: "Embedded",
            VectorIndexAlgorithm.FLAT(
                type: VectorType.FLOAT64,
                dimensions: 33,
                distanceMetric: DistanceMetric.L2,
                initialCap: 23,
                blockSize: 22))
    )
    .Build();
```

Note, that while the above examples specify a "FLAT" vector index, the `VectorIndexAlgorithm` factory class will also allow you to define an HNSW index as well. 

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