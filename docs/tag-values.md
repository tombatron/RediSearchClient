### Tag Values

When you are defining a schema, one of the field types available to you is the `TAG` field type. This field type allows for exact-match queries against values like categories or primary keys. 

RediSearch gives you access to enumerate all of the tag values by index and field by using the `FT.TAGVALS` command.

The following example demonstrates how to leverage the `FT.TAGVALS` command with RediSearchClient by issuing the command against the [sample](#sample-data) "movies" index and the "Genre" field.

```csharp
var tagValues = await _db.TagValuesAsync("movies", "Genre");
```

#### Handling the Result

The result of the `FT.TAGVALS` command is a simple, unpaged, unordered, lower-cased, stripped of whitespaces array of strings representing all of the tags applied to the specified field on the specified index.
