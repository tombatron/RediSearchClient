### Auto-Complete Suggestions

RediSearch includes the ability to maintain a special index or dictionary for the purposes of creating suggestions based on input.

The sample data project included in this repository will populate an auto-complete suggestion dictionary with city names as the string to index and the coordinates in longitude,latitude format as the payload.

#### Populating an Auto-Complete Suggestion Dictionary

In order to populate an auto-complete suggestion dictionary you'll use the `AddSuggestion` or `AddSuggestionAsync` extension methods.

The following is an example of adding an entries to the `cities` auto-complete suggestion dictionary:

```csharp
await db.AddSuggestionAsync("cities", "Pensacola", 1, payload: "-87.275772,30.61428");
```

In the above example notice that I'm providing the optional `payload` parameter. The auto-complete suggestion dictionary gives us the opportunity to store a little bit of additional data, so in this case I'm storing the coordinates of the city. 

#### Querying an Auto-Complete Suggestion Dictionary

Querying an auto-complete suggestion dictionary is pretty straight forward as it only requires calling the `GetSuggestions` or `GetSuggestionsAsync` method with the name of the dictionary and the partial term that we're searching for...

```csharp
var suggestions = await _db.GetSuggestionsAsync("cities", "pensac");
```

By default, we will get a maximum of five results back. In order to expand that, we can provide the optional `max` parameter with the number of results that we want...

```csharp
var suggestions = await _db.GetSuggestionsAsync("cities", "pensac", max: 15);
```

We can further augment our query by providing the following optional parameters:

* `fuzzy`: Will perform a "fuzzy" search for terms within a Levenshtein distance of 1.
* `withScores`: Will populate the score of a match in the result of the query. 
* `withPayload`: Will populate the payload of a match in the result of the query.

#### Removing Entries from an Auto-Complete Suggestion Dictionary

Should it become necessary, we also have the ability to remove terms from an auto-complete suggestion dictionary. 

The following is an example of removing an entry...

```csharp
await _db.DeleteSuggestionAsync("cities", "Destin");
```
