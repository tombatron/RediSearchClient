# RediSearchClient
[![Build Status](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml)

## Overview

What you have here is a set of extensions for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) Redis client that allows for interacting with version 2.x of the RediSearch Redis module. 

Support for RedisJson "JSON" datatypes is probably the most exciting forthcoming feature of RediSearch, and once that support goes mainstream and is documented, expect this package to support that as well. 

### What about NRediSearch?

NRediSearch is woefully behind in terms of support for the latest functionality for the RediSearch module. That's not meant as a slight against the StackExchange team that maintains that package, it's simply not a priority for them. I thought, why not break out a brand new package that isn't coupled to the main StackExchange.Redis project. 

So here we are. 

## Installation

```
PM> Install-Package RediSearchClient
```

## Usage

I intend to build out the Wiki with a cookbook of sorts for how you would interact with RediSearch using this package, but for now here are a few examples from the integration tests. 

### Creating an Index.

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

await _db.CreateIndexAsync("the-index-name", indexDefinition);
```

### Executing a Query

```csharp
var result = await _db.SearchAsync(
    RediSearchQuery
        .On("the-index-name")
        .UsingQuery("@State:FL")
        .Build()
    );
```
