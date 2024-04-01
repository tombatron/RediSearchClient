# RediSearchClient
[![Build Status](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/RedisSearchClient/actions/workflows/dotnet.yml)

## Overview

What you have here is a set of extensions for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) Redis client that allows for interacting with version 2.x of the RediSearch Redis module. 

Support for the RedisJson "JSON" data type is in place for those of you running RediSearch 2.2+ with RedisJson 2.0+.

### What about NRediSearch?

NRediSearch is woefully behind in terms of support for the latest functionality for the RediSearch module. That's not meant as a slight against the StackExchange team that maintains that package, it's simply not a priority for them. I thought, why not break out a brand new package that isn't coupled to the main StackExchange.Redis project. 

So here we are. 

## Table of Contents

* [Installation](docs/installation.md)
* [Sample Data](docs/sample-data.md)
* [Creating an Index](docs/creating-an-index.md)
* [Updating Index Schema](docs/updating-index-schema.md)
* [Deleting and Index](docs/deleting-an-index.md)
* [Index Aliases](docs/index-aliases.md)
* [Executing a Query](docs/executing-a-query.md)
* [Executing an Aggregation](docs/executing-an-aggregation.md)
* [Auto-Complete Suggestions](docs/auto-complete-suggestions.md)
* [Spell Check](docs/spell-check.md)
    - [Custom Dictionaries](docs/spell-check.md#custom-dictionaries)
* [Tag Values](docs/tag-values.md)
* [Synonyms](docs/synonyms.md)
* [Vector Support **\*NEW\***](docs/vector-search-support.md)



## Sample Data

Just need some sample data to play with? Hey, we've all been there. 

Included in this repository is a small console application that will load a local Redis instance with two sample datasets and create two samples indexes and an auto-suggest dictionary for you to mess around with. 

You can load the sample data by following these steps:

1. Ensure that you have a Redis instance running (with the RediSearch 2.x module loaded). See the [RediSearch Quick Start](https://oss.redislabs.com/redisearch/Quick_Start/) if you don't already have that. 

2. Clone this repository. 

3. Execute the following command from within the root of the cloned repository: `dotnet run --project .\RediSearchClient.SampleData\RediSearchClient.SampleData.csproj`

And that's pretty much it!

I suggest before writing any code you play around with query language using the sample data/indexes that we just loaded using a tool like [RedisInsight](https://redislabs.com/redis-enterprise/redis-insight/) which has support for the RediSearch (and a few others) module.
