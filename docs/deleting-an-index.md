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
