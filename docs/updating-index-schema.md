### Updating Index Schema

Depending on how your index is defined (JSON or Hash) you can use the `AlterHashSchema(Async)` or `AlterJsonSchema(Async)` methods to expand the index's schema definition.

Previously we had the method `AlterSchema(Async` but I decided to deprecate that in favor of the more specific methods. The reason for separate methods is each type of index uses a different kind of field builder. 

Let's take a look at an example where I'm adding a TEXT field and a GEO field to an existing hash based index.

```csharp
await _db.AlterHashSchemaAsync("existing_index", 
    fb => fb.Text("NewTextField", noindex: true), 
    fb => fb.Numeric("NewNumericField", sortable: true)
);
```

The following is an example of using `AlterJsonSchemaAsync`...

```csharp
await _db.AlterJsonSchemaAsync("existing_json_index",
  fb => fb.Text("$.newTextField, "NewTextField", noindex: true),
  fb => fb.Numeric("$.newNumericField", sortable: true)
);
```