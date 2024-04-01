# Vector Search Support (Work in Progress)

## Introduction

Starting with package version 2.1 (beta) it is possible to utilize the vector support present in RediSearch. 

## Defining an Index

**RediSearchClient** supports the creation of indexes by extending the [existing index creation](https://github.com/tombatron/RediSearchClient?tab=readme-ov-file#creating-an-index) builder classes by introducing the `Vector` method. This new extension method is available on both JSON and HASH based indexes and supports **HNSW** and **FLAT** vector indexes. 

The following is an example of how to define an HNSW vector index on a JSON based search index.

```csharp
var jsonIndex = RediSearchIndex
    .OnJson()
    .ForKeysWithPrefix("test_hash_vector:")
    .WithSchema(
        s => s.Text("$.name", "name"),
        s => s.Vector("$.feature_embeddings", "feature_embeddings",
            VectorIndexAlgorithm.HNSW(
                type: VectorType.FLOAT32,
                dimensions: 512,
                distanceMetric: DistanceMetric.COSINE
            )
        )
    ).Build();
```

In the above example, the vector index is defined using only the required parameters. All of the optional index parameters are settable as optional method arguments.

Defining a **FLAT** index can be done by calling the `FLAT` builder method on the `VectorIndexAlgorithm` class when defining the vector field.

All of the relevant methods and parameters should have fully populated XMLDOC documentation.

## Persisting a Vector

The following examples were pulled from the integration test suite. 

### HASH

```csharp
_db.HashSet($"test_hash_vector:{vec.Name}", new[]
{
    new HashEntry("name", vec.Name),
    new HashEntry("feature_embeddings", vec.FileBytes)
});
```

### JSON

```csharp
_db.JsonSet($"test_json_vector:{vec.Name}", new
{
    name = vec.Name,
    feature_embeddings = vec.FileFloats
});
```

## Executing a Query

I'm assuming that you already have a vector to query with as it's currently beyond the scope of this document to demonstrate the creation of feature embeddings. 