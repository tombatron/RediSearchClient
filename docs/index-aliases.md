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