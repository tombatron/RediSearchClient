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
