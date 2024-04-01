### Spell Check

RediSearch gives us the ability to "spell check" our queries.  This is useful when a query is being executed based on user input.  If the initial query yields no results we can use the spell check functionality to get suggestions to either return to the user or to use when re-executing the query. 

For more information about the spelling correction functionality see the official documentation [here](https://oss.redislabs.com/redisearch/Spellcheck/).

#### Invoking "SpellCheck" for Suggestions

The `SpellCheck` and `SpellCheckAsync` methods can accept a query definition or a plain string-based query. 

Consider the following examples:

```csharp
// Passing in the query directly... 
var result = await _db.SpellCheckAsync("simple_movie_index", "@Title:ghustbosters @Genre:{Fantasy}", 4);
```

```csharp
// Passing in the query definition from the query builder...
var query = RediSearchQuery
    .On("simple_movie_index")
    .UsingQuery("@Title:ghustboster @Genre:{Fantasy}")
    .Build();

var result = await _db.SpellCheckAsync(query, 4);
```

#### Handling "SpellCheck" Results

The results come back in the form of array of `SpellCheckResult` objects. 

Each `SpellCheckResult` contains a property labeled `Term` which describes the word being spell checked. The next property is `Suggestions` which is an array of `Suggestion` classes containing a suggestion and a score for the suggestion. 

That being said, if we wanted to load the first suggestion for a specific term, we could do something like the following:

```csharp
var result = await _db.SpellCheckAsync(query, distance: 4);

var titleSuggestion = result?["ghustbosters"]?
    .Suggestions?.FirstOrDefault()?.Value;
```

In the above example, I'm using null propagation because if there are no suggestions for a given query we wouldn't want a runtime exception would we?

#### A "Practical" Usage

The following method will return a value tuple with the `movieName` and the `released` year. If the `movieName` provided isn't found the method will invoke the `SpellCheck` command, grab the first suggestion and recursively call itself with the suggested `@Title`, returning once a match is found or spell check yields no results.

```csharp
public (string movieName, long released) FindReleaseYear(string movieName)
{
	var query = RediSearchQuery
		.On("movies")
		.UsingQuery($"@Title:{movieName}")
		.Return("Title", "Released")
		.Build();

	var result = _db.Search(query);

	if (result.Any())
	{
		var match = result.Select(x => new
		{
			Title = (String)x["Title"],
			Released = (long)x["Released"]
		}).First();

		return (match.Title, match.Released);
	}
	else
	{
		var spellCheckResult = _db.SpellCheck(query, 2);

		var suggestion = spellCheckResult?[movieName]?
			.Suggestions?
			.FirstOrDefault()?
			.Value;
			
		if (suggestion == null)
		{
			return ("No Matches", 0);
		}
		else
		{
			return FindReleaseYear(suggestion);
		}
	}
}
```

#### Custom Dictionaries

Dictionaries can be used to include and exlude terms from potential spell check results.

For more information about this functionality, see the [official documentation](https://oss.redislabs.com/redisearch/Spellcheck/#custom_dictionaries).

##### Adding to a Dictionary

To create a new dictionary or add to an existing dictionary you'll use the `AddToDictionary` or `AddToDictionaryAsync` methods. 

```csharp
var result = await _db.AddToDictionaryAsync("test_dictionary", "super", "cool", "example", "term");
```

The `result` in the above example will be equal to the number of terms that were added to the dictionary. ie If the dictionary didn't contain the words "super", "cool", "example", and "term" then the result would be `4`.

##### Removing from a Dictionary

In order to remove terms from a dictionary you'll use the `DeleteFromDictionary` or `DeleteFromDictionaryAsync` methods.

```csharp
var result = await _db.DeleteFromDictionaryAsync("test_dictionary", "example");
```

Just like adding to a dictionary, the result here will be an integer representing the number of terms affected (or in this case deleted).

##### Dumping a Dictionary

If you want to examine the contents of an **existing** dictionary you'll use the `DumpDictionary` or `DumpDictionaryAsync` methods.

```csharp
var result = await _db.DumpDictionaryAsync("test_dictionary");
```

The result will be an array of strings representing all of the terms that exist in the dictionary. If you provide the name of a dictionary that doesn't exist you'll encounter a `RedisServerException` with a message of "could not open dict key".

##### Using a Dictionary

In order leverage a custom dictionary with your call to spell check you'll create an instance of the `SpellCheckTerm` class to specify the name of the dictionary and whether or not it should be included or excluded from analysis.

You can pass one or more dictionaries into the `SpellCheck` or `SpellCheckAsync` function(s). 

```csharp
var testDictionary = new SpellCheckTerm 
{ 
    DictionaryName = "test_dictionary", 
    Treatment = TermTreatment.Include 
};

var result = await _db.SpellCheckAsync(query, testDictionary);
```

The above call to `SpellCheckAsync` will include terms from the "test_dictionary" in its analysis. 

For information on handling the result from `SpellCheckAsync` check [this](#handling-spellcheck-results) out. 
