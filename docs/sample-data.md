## Sample Data

Just need some sample data to play with? Hey, we've all been there. 

Included in this repository is a small console application that will load a local Redis instance with two sample datasets and create two samples indexes and an auto-suggest dictionary for you to mess around with. 

You can load the sample data by following these steps:

1. Ensure that you have a Redis instance running (with the RediSearch 2.x module loaded). See the [RediSearch Quick Start](https://oss.redislabs.com/redisearch/Quick_Start/) if you don't already have that. 

2. Clone this repository. 

3. Execute the following command from within the root of the cloned repository: `dotnet run --project .\RediSearchClient.SampleData\RediSearchClient.SampleData.csproj`

And that's pretty much it!

I suggest before writing any code you play around with query language using the sample data/indexes that we just loaded using a tool like [RedisInsight](https://redislabs.com/redis-enterprise/redis-insight/) which has support for the RediSearch (and a few others) module.
