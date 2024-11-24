using System;
using StackExchange.Redis;
using System.IO;
using System.Linq;

namespace RediSearchClient.IntegrationTests;

public static class SampleData
{
    public record VectorData(string FileName, string Name, byte[] FileBytes, float[] FileFloats);

    public static VectorData[] SampleVectorData;

    public static VectorData GetByNameOrDefault(this VectorData[] @this, string name) =>
        @this.FirstOrDefault(x => x.Name == name);

    static SampleData()
    {
        var sampleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples");
        var binFiles = Directory.GetFiles(sampleDirectory, "*.bin");
        var names = binFiles.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();

        SampleVectorData = new VectorData[names.Length];

        for (var i = 0; i < names.Length; i++)
        {
            var fileBytes = File.ReadAllBytes(binFiles[i]);
            var fileFloats = Enumerable.Range(0, fileBytes.Length / sizeof(float))
                .Select(i => BitConverter.ToSingle(fileBytes, i * sizeof(float))).ToArray();
            var name = names[i];

            SampleVectorData[i] = new VectorData(binFiles[i], name, fileBytes, fileFloats);
        }
    }

    // Source: https://github.com/sckott/elastic_data/blob/master/data/omdb.json
    public static HashEntry[][] Movies = new[]
    {
        new[]
        {
            new HashEntry("Title", "Iron Man"), new HashEntry("Year", 2008), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63345283200), new HashEntry("Runtime", 126),
            new HashEntry("Genre", "Action, Adventure, Sci-Fi"), new HashEntry("Director", "Jon Favreau"),
            new HashEntry("Writer",
                "Mark Fergus (screenplay), Hawk Ostby (screenplay), Art Marcum (screenplay), Matt Holloway (screenplay), Stan Lee (characters), Don Heck (characters), Larry Lieber (characters), Jack Kirby (characters)"),
            new HashEntry("Actors", "Robert Downey Jr., Terrence Howard, Jeff Bridges, Gwyneth Paltrow"),
            new HashEntry("Plot",
                "After being held captive in an Afghan cave, a billionaire engineer creates a unique weaponized suit of armor to fight evil."),
            new HashEntry("Language", "English, Persian, Urdu, Arabic, Hungarian"),
            new HashEntry("Awards", "Nominated for 2 Oscars. Another 19 wins & 61 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTczNTI2ODUwOF5BMl5BanBnXkFtZTcwMTU0NTIzMw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 79), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 689098), new HashEntry("imdbID", "tt0371746"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Iron Man 2"), new HashEntry("Year", 2010), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63408787200), new HashEntry("Runtime", 124),
            new HashEntry("Genre", "Action, Adventure, Sci-Fi"), new HashEntry("Director", "Jon Favreau"),
            new HashEntry("Writer",
                "Justin Theroux (screenplay), Stan Lee (Marvel comic book), Don Heck (Marvel comic book), Larry Lieber (Marvel comic book), Jack Kirby (Marvel comic book)"),
            new HashEntry("Actors", "Robert Downey Jr., Gwyneth Paltrow, Don Cheadle, Scarlett Johansson"),
            new HashEntry("Plot",
                "With the world now aware of his identity as Iron Man, Tony Stark must contend with both his declining health and a vengeful mad man with ties to his father's legacy."),
            new HashEntry("Language", "English, French, Russian"),
            new HashEntry("Awards", "Nominated for 1 Oscar. Another 7 wins & 40 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTM0MDgwNjMyMl5BMl5BanBnXkFtZTcwNTg3NzAzMw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 57), new HashEntry("imdbRating", 7), new HashEntry("imdbVotes", 515464),
            new HashEntry("imdbID", "tt1228705"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Frozen"), new HashEntry("Year", 2013), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63521107200), new HashEntry("Runtime", 102),
            new HashEntry("Genre", "Animation, Adventure, Comedy"),
            new HashEntry("Director", "Chris Buck, Jennifer Lee"),
            new HashEntry("Writer",
                "Jennifer Lee (screenplay), Hans Christian Andersen (story inspired by \"The Snow Queen\" by), Chris Buck (story by), Jennifer Lee (story by), Shane Morris (story by)"),
            new HashEntry("Actors", "Kristen Bell, Idina Menzel, Jonathan Groff, Josh Gad"),
            new HashEntry("Plot",
                "When the newly crowned Queen Elsa accidentally uses her power to turn things into ice to curse her home in infinite winter, her sister, Anna, teams up with a mountain man, his playful reindeer, and a snowman to change the weather condition."),
            new HashEntry("Language", "English, Icelandic"),
            new HashEntry("Awards", "Won 2 Oscars. Another 72 wins & 57 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQ1MjQwMTE5OF5BMl5BanBnXkFtZTgwNjk3MTcyMDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 74), new HashEntry("imdbRating", 7.5999999999999996),
            new HashEntry("imdbVotes", 415027), new HashEntry("imdbID", "tt2294629"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Ghostbusters"), new HashEntry("Year", 1984), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 62591097600), new HashEntry("Runtime", 105),
            new HashEntry("Genre", "Adventure, Comedy, Fantasy"), new HashEntry("Director", "Ivan Reitman"),
            new HashEntry("Writer", "Dan Aykroyd, Harold Ramis"),
            new HashEntry("Actors", "Bill Murray, Dan Aykroyd, Sigourney Weaver, Harold Ramis"),
            new HashEntry("Plot",
                "Three former parapsychology professors set up shop as a unique ghost removal service."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 2 Oscars. Another 6 wins & 6 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkxMjYyNzgwMl5BMl5BanBnXkFtZTgwMTE3MjYyMTE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 67), new HashEntry("imdbRating", 7.7999999999999998),
            new HashEntry("imdbVotes", 261569), new HashEntry("imdbID", "tt0087332"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Bourne Identity"), new HashEntry("Year", 2002), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63159609600), new HashEntry("Runtime", 119),
            new HashEntry("Genre", "Action, Mystery, Thriller"), new HashEntry("Director", "Doug Liman"),
            new HashEntry("Writer", "Tony Gilroy (screenplay), W. Blake Herron (screenplay), Robert Ludlum (novel)"),
            new HashEntry("Actors", "Matt Damon, Franka Potente, Chris Cooper, Clive Owen"),
            new HashEntry("Plot",
                "A man is picked up by a fishing boat, bullet-riddled and suffering from amnesia, before racing to elude assassins and regain his memory."),
            new HashEntry("Language", "English, French, German, Dutch, Italian"),
            new HashEntry("Awards", "3 wins & 5 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQ3MDA4MDIyN15BMl5BanBnXkFtZTYwOTg0Njk4._V1_SX300.jpg"),
            new HashEntry("Metascore", 68), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 399968), new HashEntry("imdbID", "tt0258463"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Game of Thrones"), new HashEntry("Year", 2011), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "TV-MA"), new HashEntry("Released", 63438595200), new HashEntry("Runtime", 56),
            new HashEntry("Genre", "Adventure, Drama, Fantasy"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "David Benioff, D.B. Weiss"),
            new HashEntry("Actors", "Peter Dinklage, Lena Headey, Emilia Clarke, Kit Harington"),
            new HashEntry("Plot",
                "While a civil war brews between several noble families in Westeros, the children of the former rulers of the land attempt to rise up to power. Meanwhile a forgotten race, bent on destruction, plans to return after thousands of years in the North."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 1 Golden Globe. Another 183 wins & 307 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjM5OTQ1MTY5Nl5BMl5BanBnXkFtZTgwMjM3NzMxODE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 9.5), new HashEntry("imdbVotes", 1010798),
            new HashEntry("imdbID", "tt0944947"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "The Hunger Games"), new HashEntry("Year", 2012), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63468057600), new HashEntry("Runtime", 142),
            new HashEntry("Genre", "Adventure, Drama, Sci-Fi"), new HashEntry("Director", "Gary Ross"),
            new HashEntry("Writer",
                "Gary Ross (screenplay), Suzanne Collins (screenplay), Billy Ray (screenplay), Suzanne Collins (novel)"),
            new HashEntry("Actors", "Stanley Tucci, Wes Bentley, Jennifer Lawrence, Willow Shields"),
            new HashEntry("Plot",
                "Katniss Everdeen voluntarily takes her younger sister's place in the Hunger Games, a televised competition in which two teenagers from each of the twelve Districts of Panem are chosen at random to fight to the death."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 Golden Globe. Another 34 wins & 42 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA4NDg3NzYxMF5BMl5BanBnXkFtZTcwNTgyNzkyNw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 68), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 695328), new HashEntry("imdbID", "tt1392170"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Guardians of the Galaxy"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63542448000), new HashEntry("Runtime", 121),
            new HashEntry("Genre", "Action, Adventure, Sci-Fi"), new HashEntry("Director", "James Gunn"),
            new HashEntry("Writer", "James Gunn, Nicole Perlman, Dan Abnett (comic book), Andy Lanning (comic book)"),
            new HashEntry("Actors", "Chris Pratt, Zoe Saldana, Dave Bautista, Vin Diesel"),
            new HashEntry("Plot",
                "A group of intergalactic criminals are forced to work together to stop a fanatical warrior from taking control of the universe."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 2 Oscars. Another 48 wins & 92 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTAwMjU5OTgxNjZeQTJeQWpwZ15BbWU4MDUxNDYxODEx._V1_SX300.jpg"),
            new HashEntry("Metascore", 76), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 669684), new HashEntry("imdbID", "tt2015381"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Hunger Games: Catching Fire"), new HashEntry("Year", 2013),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63520675200),
            new HashEntry("Runtime", 146), new HashEntry("Genre", "Adventure, Sci-Fi, Thriller"),
            new HashEntry("Director", "Francis Lawrence"),
            new HashEntry("Writer", "Simon Beaufoy (screenplay), Michael Arndt (screenplay), Suzanne Collins (novel)"),
            new HashEntry("Actors", "Jennifer Lawrence, Liam Hemsworth, Jack Quaid, Taylor St. Clair"),
            new HashEntry("Plot",
                "Katniss Everdeen and Peeta Mellark become targets of the Capitol after their victory in the 74th Hunger Games sparks a rebellion in the Districts of Panem."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 Golden Globe. Another 21 wins & 59 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTAyMjQ3OTAxMzNeQTJeQWpwZ15BbWU4MDU0NzA1MzAx._V1_SX300.jpg"),
            new HashEntry("Metascore", 76), new HashEntry("imdbRating", 7.5999999999999996),
            new HashEntry("imdbVotes", 493490), new HashEntry("imdbID", "tt1951264"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Imitation Game"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63555062400), new HashEntry("Runtime", 114),
            new HashEntry("Genre", "Biography, Drama, Thriller"), new HashEntry("Director", "Morten Tyldum"),
            new HashEntry("Writer", "Graham Moore, Andrew Hodges (book)"),
            new HashEntry("Actors", "Benedict Cumberbatch, Keira Knightley, Matthew Goode, Rory Kinnear"),
            new HashEntry("Plot",
                "During World War II, mathematician Alan Turing tries to crack the enigma code with help from fellow mathematicians."),
            new HashEntry("Language", "English, German"),
            new HashEntry("Awards", "Won 1 Oscar. Another 44 wins & 147 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNDkwNTEyMzkzNl5BMl5BanBnXkFtZTgwNTAwNzk3MjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 73), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 452678), new HashEntry("imdbID", "tt2084970"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Great Gatsby"), new HashEntry("Year", 2013), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63503740800), new HashEntry("Runtime", 143),
            new HashEntry("Genre", "Drama, Romance"), new HashEntry("Director", "Baz Luhrmann"),
            new HashEntry("Writer",
                "Baz Luhrmann (screenplay), Craig Pearce (screenplay), F. Scott Fitzgerald (based on the novel by)"),
            new HashEntry("Actors", "Lisa Adam, Frank Aldridge, Amitabh Bachchan, Steve Bisley"),
            new HashEntry("Plot",
                "A writer and wall street trader, Nick, finds himself drawn to the past and lifestyle of his millionaire neighbor, Jay Gatsby."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 2 Oscars. Another 43 wins & 73 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkxNTk1ODcxNl5BMl5BanBnXkFtZTcwMDI1OTMzOQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 55), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 359439), new HashEntry("imdbID", "tt1343092"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Sherlock Holmes: A Game of Shadows"), new HashEntry("Year", 2011),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63459590400),
            new HashEntry("Runtime", 129), new HashEntry("Genre", "Action, Adventure, Crime"),
            new HashEntry("Director", "Guy Ritchie"),
            new HashEntry("Writer",
                "Michele Mulroney, Kieran Mulroney, Arthur Conan Doyle (characters: Sherlock Holmes,  Dr. Watson)"),
            new HashEntry("Actors", "Robert Downey Jr., Jude Law, Noomi Rapace, Rachel McAdams"),
            new HashEntry("Plot",
                "Sherlock Holmes and his sidekick Dr. Watson join forces to outwit and bring down their fiercest adversary, Professor Moriarty."),
            new HashEntry("Language", "English, French, Italian, German, Romany"),
            new HashEntry("Awards", "2 wins & 10 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQwMzQ5Njk1MF5BMl5BanBnXkFtZTcwNjIxNzIxNw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 48), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 335307),
            new HashEntry("imdbID", "tt1515091"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "American Gangster"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63329558400), new HashEntry("Runtime", 157),
            new HashEntry("Genre", "Biography, Crime, Drama"), new HashEntry("Director", "Ridley Scott"),
            new HashEntry("Writer", "Steven Zaillian, Mark Jacobson (article)"),
            new HashEntry("Actors", "Denzel Washington, Russell Crowe, Chiwetel Ejiofor, Josh Brolin"),
            new HashEntry("Plot",
                "In 1970s America, a detective works to bring down the drug empire of Frank Lucas, a heroin kingpin from Manhattan, who is smuggling the drug into the country from the Far East."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 2 Oscars. Another 12 wins & 34 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkyNzY5MDA5MV5BMl5BanBnXkFtZTcwMjg4MzI3MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 76), new HashEntry("imdbRating", 7.7999999999999998),
            new HashEntry("imdbVotes", 322645), new HashEntry("imdbID", "tt0765429"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gangs of New York"), new HashEntry("Year", 2002), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63175939200), new HashEntry("Runtime", 167),
            new HashEntry("Genre", "Crime, Drama, History"), new HashEntry("Director", "Martin Scorsese"),
            new HashEntry("Writer",
                "Jay Cocks (story), Jay Cocks (screenplay), Steven Zaillian (screenplay), Kenneth Lonergan (screenplay)"),
            new HashEntry("Actors", "Leonardo DiCaprio, Daniel Day-Lewis, Cameron Diaz, Jim Broadbent"),
            new HashEntry("Plot",
                "In 1863, Amsterdam Vallon returns to the Five Points area of New York City seeking revenge against Bill the Butcher, his father's killer."),
            new HashEntry("Language", "English, Irish, Chinese, Latin"),
            new HashEntry("Awards", "Nominated for 10 Oscars. Another 47 wins & 117 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTI4NTM0Mzg2NV5BMl5BanBnXkFtZTcwNjQxMDAwMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 72), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 311111),
            new HashEntry("imdbID", "tt0217505"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Hunger Games: Mockingjay - Part 1"), new HashEntry("Year", 2014),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63552124800),
            new HashEntry("Runtime", 123), new HashEntry("Genre", "Adventure, Sci-Fi, Thriller"),
            new HashEntry("Director", "Francis Lawrence"),
            new HashEntry("Writer",
                "Peter Craig (screenplay), Danny Strong (screenplay), Suzanne Collins (adaptation), Suzanne Collins (novel)"),
            new HashEntry("Actors", "Jennifer Lawrence, Josh Hutcherson, Liam Hemsworth, Woody Harrelson"),
            new HashEntry("Plot",
                "Katniss Everdeen is in District 13 after she shatters the games forever. Under the leadership of President Coin and the advice of her trusted friends, Katniss spreads her wings as she fights to save Peeta and a nation moved by her courage."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 Golden Globe. Another 15 wins & 26 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTcxNDI2NDAzNl5BMl5BanBnXkFtZTgwODM3MTc2MjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 64), new HashEntry("imdbRating", 6.7000000000000002),
            new HashEntry("imdbVotes", 301187), new HashEntry("imdbID", "tt1951265"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Ninth Gate"), new HashEntry("Year", 1999), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63088243200), new HashEntry("Runtime", 133),
            new HashEntry("Genre", "Mystery, Thriller"), new HashEntry("Director", "Roman Polanski"),
            new HashEntry("Writer",
                "Arturo Pérez-Reverte (novel), John Brownjohn (screenplay), Enrique Urbizu (screenplay), Roman Polanski (screenplay)"),
            new HashEntry("Actors", "Johnny Depp, Frank Langella, Lena Olin, Emmanuelle Seigner"),
            new HashEntry("Plot",
                "A rare book dealer, while seeking out the last two copies of a demon text, gets drawn into a conspiracy with supernatural overtones."),
            new HashEntry("Language", "English, French, Latin, Portuguese, Spanish"),
            new HashEntry("Awards", "1 win & 4 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTk2NTI4NDYzMl5BMl5BanBnXkFtZTcwNzgxMDU0MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 44), new HashEntry("imdbRating", 6.7000000000000002),
            new HashEntry("imdbVotes", 137040), new HashEntry("imdbID", "tt0142688"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Galaxy Quest"), new HashEntry("Year", 1999), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63081676800), new HashEntry("Runtime", 102),
            new HashEntry("Genre", "Adventure, Comedy, Sci-Fi"), new HashEntry("Director", "Dean Parisot"),
            new HashEntry("Writer", "David Howard (story), David Howard (screenplay), Robert Gordon (screenplay)"),
            new HashEntry("Actors", "Tim Allen, Sigourney Weaver, Alan Rickman, Tony Shalhoub"),
            new HashEntry("Plot",
                "The alumni cast of a space opera television series have to play their roles as the real thing when an alien race needs their help."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "7 wins & 14 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA0NjM1ODkyMl5BMl5BanBnXkFtZTcwODY0NDMzMg@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 70), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 122817), new HashEntry("imdbID", "tt0177789"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Spy Game"), new HashEntry("Year", 2001), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63141897600), new HashEntry("Runtime", 126),
            new HashEntry("Genre", "Action, Crime, Thriller"), new HashEntry("Director", "Tony Scott"),
            new HashEntry("Writer",
                "Michael Frost Beckner (story), Michael Frost Beckner (screenplay), David Arata (screenplay)"),
            new HashEntry("Actors", "Robert Redford, Brad Pitt, Catherine McCormack, Stephen Dillane"),
            new HashEntry("Plot",
                "Retiring CIA agent Nathan Muir recalls his training of Tom Bishop while working against agency politics to free him from his Chinese captors."),
            new HashEntry("Language", "English, German, Arabic, French, Cantonese"),
            new HashEntry("Awards", "3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNjNhOGZkNzktMGU3NC00ODk2LWE4NjctZTliN2JjZTQxZmIxXkEyXkFqcGdeQXVyNDk3NzU2MTQ@._V1_SX300.jpg"),
            new HashEntry("Metascore", 63), new HashEntry("imdbRating", 7), new HashEntry("imdbVotes", 120408),
            new HashEntry("imdbID", "tt0266987"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica"), new HashEntry("Year", 2004), new HashEntry("YearEnd", 2009),
            new HashEntry("Rated", "TV-14"), new HashEntry("Released", 63241257600), new HashEntry("Runtime", 44),
            new HashEntry("Genre", "Action, Adventure, Drama"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Glen A. Larson, Ronald D. Moore"),
            new HashEntry("Actors", "Edward James Olmos, Mary McDonnell, Jamie Bamber, James Callis"),
            new HashEntry("Plot",
                "When an old enemy, the Cylons, resurface and obliterate the 12 colonies, the crew of the aged Galactica protect a small civilian fleet - the last of humanity - as they journey toward the fabled 13th colony, Earth."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 3 Primetime Emmys. Another 32 wins & 80 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTc1NTg1MDk3NF5BMl5BanBnXkFtZTYwNDYyMjI3._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.8000000000000007),
            new HashEntry("imdbVotes", 118636), new HashEntry("imdbID", "tt0407362"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Gamer"), new HashEntry("Year", 2009), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63387619200), new HashEntry("Runtime", 95),
            new HashEntry("Genre", "Action, Sci-Fi, Thriller"),
            new HashEntry("Director", "Mark Neveldine, Brian Taylor"),
            new HashEntry("Writer", "Mark Neveldine, Brian Taylor"),
            new HashEntry("Actors", "Gerard Butler, Amber Valletta, Michael C. Hall, Kyra Sedgwick"),
            new HashEntry("Plot",
                "In a future mind-controlling game, death row convicts are forced to battle in a 'Doom'-type environment. Convict Kable, controlled by Simon, a skilled teenage gamer, must survive thirty sessions in order to be set free. Or won't he?"),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkzMDU0NTg3MF5BMl5BanBnXkFtZTcwNzU1MjU1Mg@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 27), new HashEntry("imdbRating", 5.7999999999999998),
            new HashEntry("imdbVotes", 112941), new HashEntry("imdbID", "tt1034032"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Constant Gardener"), new HashEntry("Year", 2005), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63261043200), new HashEntry("Runtime", 129),
            new HashEntry("Genre", "Drama, Mystery, Romance"), new HashEntry("Director", "Fernando Meirelles"),
            new HashEntry("Writer", "Jeffrey Caine (screenplay), John le Carré (based on the novel by)"),
            new HashEntry("Actors", "Ralph Fiennes, Rachel Weisz, Hubert Koundé, Danny Huston"),
            new HashEntry("Plot",
                "A widower is determined to get to the bottom of a potentially explosive secret involving his wife's murder, big business, and corporate corruption."),
            new HashEntry("Language", "English, Italian, Swahili, German"),
            new HashEntry("Awards", "Won 1 Oscar. Another 33 wins & 64 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTg1MTYyMDE2NF5BMl5BanBnXkFtZTcwNTk1NTAzMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 82), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 110826),
            new HashEntry("imdbID", "tt0387131"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Life of David Gale"), new HashEntry("Year", 2003), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63181382400), new HashEntry("Runtime", 130),
            new HashEntry("Genre", "Crime, Drama, Thriller"), new HashEntry("Director", "Alan Parker"),
            new HashEntry("Writer", "Charles Randolph"),
            new HashEntry("Actors", "Kate Winslet, Cleo King, Constance Jones, Kevin Spacey"),
            new HashEntry("Plot",
                "A man against capital punishment is accused of murdering a fellow activist and is sent to death row."),
            new HashEntry("Language", "English, Spanish"), new HashEntry("Awards", "3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTAxMzU0NTgxNzZeQTJeQWpwZ15BbWU2MDQzNDkxNw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 31), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 87307),
            new HashEntry("imdbID", "tt0289992"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Patriot Games"), new HashEntry("Year", 1992), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 62843299200), new HashEntry("Runtime", 117),
            new HashEntry("Genre", "Action, Thriller"), new HashEntry("Director", "Phillip Noyce"),
            new HashEntry("Writer", "Tom Clancy (novel), W. Peter Iliff (screenplay), Donald Stewart (screenplay)"),
            new HashEntry("Actors", "Harrison Ford, Anne Archer, Patrick Bergin, Sean Bean"),
            new HashEntry("Plot",
                "When CIA Analyst Jack Ryan interferes with an IRA assassination, a renegade faction targets him and his family for revenge."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 2 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA3OTA0NjI0Nl5BMl5BanBnXkFtZTgwNjUwODQxMTE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.9000000000000004),
            new HashEntry("imdbVotes", 80741), new HashEntry("imdbID", "tt0105112"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Funny Games"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63342864000), new HashEntry("Runtime", 111),
            new HashEntry("Genre", "Crime, Drama, Horror"), new HashEntry("Director", "Michael Haneke"),
            new HashEntry("Writer", "Michael Haneke"),
            new HashEntry("Actors", "Naomi Watts, Tim Roth, Michael Pitt, Brady Corbet"),
            new HashEntry("Plot", "Two psychopathic young men take a family hostage in their cabin."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 5 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTg4OTExNTYzMV5BMl5BanBnXkFtZTcwOTg1MDU1MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 44), new HashEntry("imdbRating", 6.5), new HashEntry("imdbVotes", 68925),
            new HashEntry("imdbID", "tt0808279"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Garfield"), new HashEntry("Year", 2004), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63222508800), new HashEntry("Runtime", 80),
            new HashEntry("Genre", "Animation, Comedy, Family"), new HashEntry("Director", "Peter Hewitt"),
            new HashEntry("Writer", "Jim Davis (comic strip \"Garfield\"), Joel Cohen, Alec Sokolow"),
            new HashEntry("Actors", "Breckin Meyer, Jennifer Love Hewitt, Stephen Tobolowsky, Bill Murray"),
            new HashEntry("Plot",
                "Jon Arbuckle buys a second pet, a dog named Odie. However, Odie is then abducted and it is up to Jon's cat, Garfield, to find and rescue the canine."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTIzMTc1OTUxOV5BMl5BanBnXkFtZTYwNTMxODc3._V1_SX300.jpg"),
            new HashEntry("Metascore", 27), new HashEntry("imdbRating", 5), new HashEntry("imdbVotes", 58113),
            new HashEntry("imdbID", "tt0356634"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Gambler"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63555062400), new HashEntry("Runtime", 111),
            new HashEntry("Genre", "Crime, Drama, Thriller"), new HashEntry("Director", "Rupert Wyatt"),
            new HashEntry("Writer", "William Monahan (screenplay), James Toback"),
            new HashEntry("Actors", "Mark Wahlberg, George Kennedy, Griffin Cleveland, Jessica Lange"),
            new HashEntry("Plot",
                "Lit professor and gambler Jim Bennett's debt causes him to borrow money from his mother and a loan shark. Further complicating his situation is his relationship with one of his students. Will Bennett risk his life for a second chance?"),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 2 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA5MjIzODE3N15BMl5BanBnXkFtZTgwNzUwNzYwMzE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 55), new HashEntry("imdbRating", 6), new HashEntry("imdbVotes", 48061),
            new HashEntry("imdbID", "tt2039393"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica"), new HashEntry("Year", 2004), new HashEntry("YearEnd", 2009),
            new HashEntry("Rated", "TV-14"), new HashEntry("Released", 63241257600), new HashEntry("Runtime", 44),
            new HashEntry("Genre", "Action, Adventure, Drama"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Glen A. Larson, Ronald D. Moore"),
            new HashEntry("Actors", "Edward James Olmos, Mary McDonnell, Jamie Bamber, James Callis"),
            new HashEntry("Plot",
                "When an old enemy, the Cylons, resurface and obliterate the 12 colonies, the crew of the aged Galactica protect a small civilian fleet - the last of humanity - as they journey toward the fabled 13th colony, Earth."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 3 Primetime Emmys. Another 32 wins & 80 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTc1NTg1MDk3NF5BMl5BanBnXkFtZTYwNDYyMjI3._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.8000000000000007),
            new HashEntry("imdbVotes", 118636), new HashEntry("imdbID", "tt0407362"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Gangs of Wasseypur"), new HashEntry("Year", 2012), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63479462400), new HashEntry("Runtime", 320),
            new HashEntry("Genre", "Action, Crime, Drama"), new HashEntry("Director", "Anurag Kashyap"),
            new HashEntry("Writer", "Akhilesh Jaiswal, Anurag Kashyap, Sachin K. Ladia, Zeishan Quadri"),
            new HashEntry("Actors", "Manoj Bajpayee, Richa Chadha, Nawazuddin Siddiqui, Tigmanshu Dhulia"),
            new HashEntry("Plot",
                "A clash between Sultan (a Qureishi dacoit chief) and Shahid Khan (a Pathan who impersonates him) leads to the expulsion of Khan from Wasseypur, and ignites a deadly blood feud spanning three generations."),
            new HashEntry("Language", "Hindi"), new HashEntry("Awards", "10 wins & 32 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTc5NjY4MjUwNF5BMl5BanBnXkFtZTgwODM3NzM5MzE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 89), new HashEntry("imdbRating", 8.3000000000000007),
            new HashEntry("imdbVotes", 43736), new HashEntry("imdbID", "tt1954470"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Funny Games"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63342864000), new HashEntry("Runtime", 111),
            new HashEntry("Genre", "Crime, Drama, Horror"), new HashEntry("Director", "Michael Haneke"),
            new HashEntry("Writer", "Michael Haneke"),
            new HashEntry("Actors", "Naomi Watts, Tim Roth, Michael Pitt, Brady Corbet"),
            new HashEntry("Plot", "Two psychopathic young men take a family hostage in their cabin."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 5 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTg4OTExNTYzMV5BMl5BanBnXkFtZTcwOTg1MDU1MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 44), new HashEntry("imdbRating", 6.5), new HashEntry("imdbVotes", 68925),
            new HashEntry("imdbID", "tt0808279"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Game Plan"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63326534400), new HashEntry("Runtime", 110),
            new HashEntry("Genre", "Comedy, Family, Sport"), new HashEntry("Director", "Andy Fickman"),
            new HashEntry("Writer",
                "Nichole Millard (screenplay), Kathryn Price (screenplay), Nichole Millard (story), Kathryn Price (story), Audrey Wells (story)"),
            new HashEntry("Actors", "Dwayne Johnson, Madison Pettis, Kyra Sedgwick, Roselyn Sanchez"),
            new HashEntry("Plot",
                "An NFL quarterback living the bachelor lifestyle discovers that he has an 8-year-old daughter from a previous relationship."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTAzNDIyODYzMTJeQTJeQWpwZ15BbWU3MDA3NTA5NDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 44), new HashEntry("imdbRating", 6.2000000000000002),
            new HashEntry("imdbVotes", 43348), new HashEntry("imdbID", "tt0492956"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Crying Game"), new HashEntry("Year", 1992), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 62865676800), new HashEntry("Runtime", 112),
            new HashEntry("Genre", "Crime, Drama, Romance"), new HashEntry("Director", "Neil Jordan"),
            new HashEntry("Writer", "Neil Jordan"),
            new HashEntry("Actors", "Forest Whitaker, Miranda Richardson, Stephen Rea, Adrian Dunbar"),
            new HashEntry("Plot",
                "A British soldier is kidnapped by IRA terrorists. He befriends one of his captors, who is drawn into the soldier's world."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 1 Oscar. Another 20 wins & 36 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTI3NjE3NTAyN15BMl5BanBnXkFtZTcwMDg2MzcyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 90), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 41051), new HashEntry("imdbID", "tt0104036"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Spy Kids 3-D: Game Over"), new HashEntry("Year", 2003), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63194688000), new HashEntry("Runtime", 84),
            new HashEntry("Genre", "Action, Adventure, Comedy"), new HashEntry("Director", "Robert Rodriguez"),
            new HashEntry("Writer", "Robert Rodriguez (script)"),
            new HashEntry("Actors", "Antonio Banderas, Carla Gugino, Alexa PenaVega, Daryl Sabara"),
            new HashEntry("Plot",
                "Carmen's caught in a virtual reality game designed by the Kids' new nemesis, the Toymaker (Stallone). It's up to Juni to save his sister, and ultimately the world."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "3 wins & 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTI4MTQyNTUzMF5BMl5BanBnXkFtZTcwNzE2MDAwMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 57), new HashEntry("imdbRating", 4.0999999999999996),
            new HashEntry("imdbVotes", 39771), new HashEntry("imdbID", "tt0338459"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Fair Game"), new HashEntry("Year", 2010), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63426931200), new HashEntry("Runtime", 108),
            new HashEntry("Genre", "Biography, Drama, Thriller"), new HashEntry("Director", "Doug Liman"),
            new HashEntry("Writer",
                "Jez Butterworth (screenplay), John-Henry Butterworth (screenplay), Joseph Wilson (book), Valerie Plame Wilson (book)"),
            new HashEntry("Actors", "Naomi Watts, Sonya Davison, Vanessa Chong, Anand Tiwari"),
            new HashEntry("Plot",
                "CIA operative Valerie Plame discovers her identity is allegedly leaked by the government as payback for an op-ed article her husband wrote criticizing the Bush administration."),
            new HashEntry("Language", "English, Arabic, French"), new HashEntry("Awards", "3 wins & 9 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjIyOTg1NzU0Ml5BMl5BanBnXkFtZTcwMjA2OTY5Mw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 69), new HashEntry("imdbRating", 6.7999999999999998),
            new HashEntry("imdbVotes", 38196), new HashEntry("imdbID", "tt0977855"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Inspector Gadget"), new HashEntry("Year", 1999), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63068284800), new HashEntry("Runtime", 78),
            new HashEntry("Genre", "Action, Adventure, Comedy"), new HashEntry("Director", "David Kellogg"),
            new HashEntry("Writer",
                "Andy Heyward (characters), Jean Chalopin (characters), Bruno Bianchi (characters), Dana Olsen (story), Kerry Ehrin (story), Kerry Ehrin (screenplay), Zak Penn (screenplay)"),
            new HashEntry("Actors", "Matthew Broderick, Rupert Everett, Joely Fisher, Michelle Trachtenberg"),
            new HashEntry("Plot",
                "A security guard's dreams come true when he is selected to be transformed into a cybernetic police officer."),
            new HashEntry("Language", "English, Norwegian, French, Spanish"),
            new HashEntry("Awards", "1 win & 9 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTU5NDkwMTUxN15BMl5BanBnXkFtZTcwNzM2OTMyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 36), new HashEntry("imdbRating", 4.0999999999999996),
            new HashEntry("imdbVotes", 36211), new HashEntry("imdbID", "tt0141369"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "He Got Game"), new HashEntry("Year", 1998), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63029577600), new HashEntry("Runtime", 136),
            new HashEntry("Genre", "Drama, Sport"), new HashEntry("Director", "Spike Lee"),
            new HashEntry("Writer", "Spike Lee"),
            new HashEntry("Actors", "Denzel Washington, Ray Allen, Milla Jovovich, Rosario Dawson"),
            new HashEntry("Plot",
                "A basketball player's father must try to convince him to go to a college so he can get a shorter sentence."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "10 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTg3MTY1NjgxM15BMl5BanBnXkFtZTYwMjU2Mzc5._V1_SX300.jpg"),
            new HashEntry("Metascore", 63), new HashEntry("imdbRating", 6.9000000000000004),
            new HashEntry("imdbVotes", 35847), new HashEntry("imdbID", "tt0124718"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gridiron Gang"), new HashEntry("Year", 2006), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63293875200), new HashEntry("Runtime", 125),
            new HashEntry("Genre", "Crime, Drama, Sport"), new HashEntry("Director", "Phil Joanou"),
            new HashEntry("Writer", "Jeff Maguire, Jac Flanders (film \"Gridiron Gang\")"),
            new HashEntry("Actors", "Dwayne Johnson, Xzibit, L. Scott Caldwell, Leon Rippy"),
            new HashEntry("Plot",
                "Teenagers at a juvenile detention center, under the leadership of their counselor, gain self-esteem by playing football together."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNzk4NTAwNTAzN15BMl5BanBnXkFtZTcwNjczODYzMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 52), new HashEntry("imdbRating", 6.9000000000000004),
            new HashEntry("imdbVotes", 32589), new HashEntry("imdbID", "tt0421206"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Midnight in the Garden of Good and Evil"), new HashEntry("Year", 1997),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 63015667200),
            new HashEntry("Runtime", 155), new HashEntry("Genre", "Crime, Drama, Mystery"),
            new HashEntry("Director", "Clint Eastwood"),
            new HashEntry("Writer", "John Berendt (book), John Lee Hancock (screenplay)"),
            new HashEntry("Actors", "John Cusack, Kevin Spacey, Jack Thompson, Irma P. Hall"),
            new HashEntry("Plot",
                "A visiting city reporter's assignment suddenly revolves around the murder trial of a local millionaire, whom he befriends."),
            new HashEntry("Language", "English, French"), new HashEntry("Awards", "1 win & 3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA2ODcyODM3NF5BMl5BanBnXkFtZTcwMzg2OTk0OQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 57), new HashEntry("imdbRating", 6.5999999999999996),
            new HashEntry("imdbVotes", 31535), new HashEntry("imdbID", "tt0119668"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Reindeer Games"), new HashEntry("Year", 2000), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63087033600), new HashEntry("Runtime", 104),
            new HashEntry("Genre", "Action, Crime, Drama"), new HashEntry("Director", "John Frankenheimer"),
            new HashEntry("Writer", "Ehren Kruger"),
            new HashEntry("Actors", "Ben Affleck, James Frain, Dana Stubblefield, Mark Acheson"),
            new HashEntry("Plot",
                "After assuming his dead cellmate's identity to get with his girlfriend, an ex-con finds himself the reluctant participant in a casino heist."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BODA3NjUzMzk3MF5BMl5BanBnXkFtZTcwNjczMTIyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 37), new HashEntry("imdbRating", 5.7000000000000002),
            new HashEntry("imdbVotes", 30912), new HashEntry("imdbID", "tt0184858"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gallipoli"), new HashEntry("Year", 1981), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 62503401600), new HashEntry("Runtime", 110),
            new HashEntry("Genre", "Adventure, Drama, History"), new HashEntry("Director", "Peter Weir"),
            new HashEntry("Writer", "Peter Weir (story), David Williamson (screenplay)"),
            new HashEntry("Actors", "Mark Lee, Bill Kerr, Harold Hopkins, Charles Lathalu Yunipingu"),
            new HashEntry("Plot",
                "Two Australian sprinters face the brutal realities of war when they are sent to fight in the Gallipoli campaign in Turkey during World War I."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 Golden Globe. Another 11 wins & 5 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTc2NDgyNDEzNF5BMl5BanBnXkFtZTcwMDExNTI3MQ@@._V1._CR13,56,329,443_SY132_CR4,0,89,132_AL_.jpg_V1_SX300.jpg"),
            new HashEntry("Metascore", 65), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 29629),
            new HashEntry("imdbID", "tt0082432"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Dinner Game"), new HashEntry("Year", 1998), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63028195200), new HashEntry("Runtime", 80),
            new HashEntry("Genre", "Comedy"), new HashEntry("Director", "Francis Veber"),
            new HashEntry("Writer", "Francis Veber"),
            new HashEntry("Actors", "Thierry Lhermitte, Jacques Villeret, Francis Huster, Daniel Prévost"),
            new HashEntry("Plot",
                "To amuse themselves at a weekly dinner, a few well-heeled folk each bring a dimwit along who is to talk about his pastime. Each member seeks to introduce a champion dumbbell. Pierre, an ..."),
            new HashEntry("Language", "French"), new HashEntry("Awards", "5 wins & 4 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMmIxY2FhNGEtMWU0NS00OTI2LTllOWQtZDZmZTQ0Yzk2MWRhXkEyXkFqcGdeQXVyNDk3NzU2MTQ@._V1_SX300.jpg"),
            new HashEntry("Metascore", 73), new HashEntry("imdbRating", 7.7000000000000002),
            new HashEntry("imdbVotes", 29459), new HashEntry("imdbID", "tt0119038"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Secret Garden"), new HashEntry("Year", 1993), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "G"), new HashEntry("Released", 62880796800), new HashEntry("Runtime", 101),
            new HashEntry("Genre", "Drama, Family, Fantasy"), new HashEntry("Director", "Agnieszka Holland"),
            new HashEntry("Writer", "Frances Hodgson Burnett (book), Caroline Thompson (screenplay)"),
            new HashEntry("Actors", "Kate Maberly, Heydon Prowse, Andrew Knott, Maggie Smith"),
            new HashEntry("Plot",
                "A young British girl born and raised in India loses her neglectful parents in an earthquake. She is returned to England to live at her uncle's estate. Her uncle is very distant due to the ..."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 BAFTA Film Award. Another 3 wins & 5 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNzEyNTc0NjE0MF5BMl5BanBnXkFtZTcwNTUzNzYxMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 27899), new HashEntry("imdbID", "tt0108071"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Garfield 2"), new HashEntry("Year", 2006), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63286012800), new HashEntry("Runtime", 78),
            new HashEntry("Genre", "Animation, Comedy, Family"), new HashEntry("Director", "Tim Hill"),
            new HashEntry("Writer", "Joel Cohen, Alec Sokolow, Jim Davis (comic strip \"Garfield\")"),
            new HashEntry("Actors", "Breckin Meyer, Jennifer Love Hewitt, Billy Connolly, Bill Murray"),
            new HashEntry("Plot",
                "Jon and Garfield visit the United Kingdom, where a case of mistaken cat identity finds Garfield ruling over a castle. His reign is soon jeopardized by the nefarious Lord Dargis, who has designs on the estate."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "4 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQwNTcyOTQ5MV5BMl5BanBnXkFtZTYwMTY2NjU3._V1_SX300.jpg"),
            new HashEntry("Metascore", 37), new HashEntry("imdbRating", 5), new HashEntry("imdbVotes", 27132),
            new HashEntry("imdbID", "tt0455499"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "For Love of the Game"), new HashEntry("Year", 1999), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63073123200), new HashEntry("Runtime", 137),
            new HashEntry("Genre", "Drama, Romance, Sport"), new HashEntry("Director", "Sam Raimi"),
            new HashEntry("Writer", "Michael Shaara (novel), Dana Stevens (screenplay)"),
            new HashEntry("Actors", "Kevin Costner, Kelly Preston, John C. Reilly, Jena Malone"),
            new HashEntry("Plot", "A washed up pitcher flashes through his career."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "7 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTcyMDk1ODM5N15BMl5BanBnXkFtZTYwNjYzNzQ5._V1_SX300.jpg"),
            new HashEntry("Metascore", 43), new HashEntry("imdbRating", 6.5), new HashEntry("imdbVotes", 25858),
            new HashEntry("imdbID", "tt0126916"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Greatest Game Ever Played"), new HashEntry("Year", 2005),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "PG"), new HashEntry("Released", 63263635200),
            new HashEntry("Runtime", 120), new HashEntry("Genre", "Drama, History, Sport"),
            new HashEntry("Director", "Bill Paxton"),
            new HashEntry("Writer", "Mark Frost (book), Mark Frost (screenplay)"),
            new HashEntry("Actors", "James Paxton, Tom Rack, Armand Laroche, Peter Hurley"),
            new HashEntry("Plot",
                "In the 1913 US Open, 20-year-old Francis Ouimet played against his idol, 1900 US Open champion, Englishman Harry Vardon."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQ4NDk3MDk0NV5BMl5BanBnXkFtZTcwMzk4OTgyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 55), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 22153),
            new HashEntry("imdbID", "tt0388980"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The World According to Garp"), new HashEntry("Year", 1982),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 62533641600),
            new HashEntry("Runtime", 136), new HashEntry("Genre", "Comedy, Drama"),
            new HashEntry("Director", "George Roy Hill"),
            new HashEntry("Writer", "John Irving (novel), Steve Tesich (screenplay)"),
            new HashEntry("Actors", "Robin Williams, Mary Beth Hurt, Glenn Close, John Lithgow"),
            new HashEntry("Plot",
                "Based on the John Irving novel, this film chronicles the life of T S Garp, and his mother, Jenny. Whilst Garp sees himself as a \"serious\" writer, Jenny writes a feminist manifesto at an ..."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 2 Oscars. Another 5 wins & 4 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTk4MzQ3NTQzOV5BMl5BanBnXkFtZTgwNDE0ODg4MDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.2000000000000002),
            new HashEntry("imdbVotes", 20468), new HashEntry("imdbID", "tt0084917"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Asterix at the Olympic Games"), new HashEntry("Year", 2008),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "N/A"), new HashEntry("Released", 63337248000),
            new HashEntry("Runtime", 116), new HashEntry("Genre", "Adventure, Comedy, Family"),
            new HashEntry("Director", "Frédéric Forestier, Thomas Langmann"),
            new HashEntry("Writer",
                "René Goscinny (comic book), Albert Uderzo (comic book), Thomas Langmann (screenplay), Olivier Dazat (screenplay), Alexandre Charlot (screenplay), Franck Magnier (screenplay), Alexandre Charlot (adaptation), Franck Magnier (adaptation), Thomas Langmann (adaptation)"),
            new HashEntry("Actors", "Gérard Depardieu, Clovis Cornillac, Benoît Poelvoorde, Alain Delon"),
            new HashEntry("Plot",
                "Astérix and Obélix compete at the Olympics in order to help their friend Lovesix marry Princess Irina. Brutus also tries to win the game with his own team and get rid of his father Julius Caesar."),
            new HashEntry("Language", "French, Portuguese"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTg1MzkwMzg2Ml5BMl5BanBnXkFtZTcwNTg4MzQ4MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 5.0999999999999996),
            new HashEntry("imdbVotes", 20402), new HashEntry("imdbID", "tt0463872"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Rules of the Game"), new HashEntry("Year", 1939), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "APPROVED"), new HashEntry("Released", 61512825600), new HashEntry("Runtime", 110),
            new HashEntry("Genre", "Comedy, Drama, Romance"), new HashEntry("Director", "Jean Renoir"),
            new HashEntry("Writer", "Jean Renoir (scenario & dialogue), Carl Koch (collaborator)"),
            new HashEntry("Actors", "Nora Gregor, Paulette Dubost, Mila Parély, Odette Talazac"),
            new HashEntry("Plot",
                "A bourgeois life in France at the onset of World War II, as the rich and their poor servants meet up at a French chateau."),
            new HashEntry("Language", "French, German, English"), new HashEntry("Awards", "2 wins."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTA4MjI2MTU1OTleQTJeQWpwZ15BbWU4MDE5NTAzOTEx._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 20272), new HashEntry("imdbID", "tt0031885"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Big Game"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63570873600), new HashEntry("Runtime", 110),
            new HashEntry("Genre", "Action, Adventure"), new HashEntry("Director", "Jalmari Helander"),
            new HashEntry("Writer",
                "Jalmari Helander (screenplay), Jalmari Helander (based on the original story by), Petri Jokiranta (based on the original story by)"),
            new HashEntry("Actors", "Samuel L. Jackson, Onni Tommila, Ray Stevenson, Victor Garber"),
            new HashEntry("Plot",
                "A young teenager camping in the woods helps rescue the President of the United States when Air Force One is shot down near his campsite."),
            new HashEntry("Language", "English, Finnish"), new HashEntry("Awards", "1 win & 8 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjQxNTc1ODA1Ml5BMl5BanBnXkFtZTgwMTQ1NDM3NDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 53), new HashEntry("imdbRating", 5.5), new HashEntry("imdbVotes", 19495),
            new HashEntry("imdbID", "tt2088003"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Big Game"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63570873600), new HashEntry("Runtime", 110),
            new HashEntry("Genre", "Action, Adventure"), new HashEntry("Director", "Jalmari Helander"),
            new HashEntry("Writer",
                "Jalmari Helander (screenplay), Jalmari Helander (based on the original story by), Petri Jokiranta (based on the original story by)"),
            new HashEntry("Actors", "Samuel L. Jackson, Onni Tommila, Ray Stevenson, Victor Garber"),
            new HashEntry("Plot",
                "A young teenager camping in the woods helps rescue the President of the United States when Air Force One is shot down near his campsite."),
            new HashEntry("Language", "English, Finnish"), new HashEntry("Awards", "1 win & 8 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjQxNTc1ODA1Ml5BMl5BanBnXkFtZTgwMTQ1NDM3NDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 53), new HashEntry("imdbRating", 5.5), new HashEntry("imdbVotes", 19495),
            new HashEntry("imdbID", "tt2088003"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Great Gatsby"), new HashEntry("Year", 2013), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63503740800), new HashEntry("Runtime", 143),
            new HashEntry("Genre", "Drama, Romance"), new HashEntry("Director", "Baz Luhrmann"),
            new HashEntry("Writer",
                "Baz Luhrmann (screenplay), Craig Pearce (screenplay), F. Scott Fitzgerald (based on the novel by)"),
            new HashEntry("Actors", "Lisa Adam, Frank Aldridge, Amitabh Bachchan, Steve Bisley"),
            new HashEntry("Plot",
                "A writer and wall street trader, Nick, finds himself drawn to the past and lifestyle of his millionaire neighbor, Jay Gatsby."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 2 Oscars. Another 43 wins & 73 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkxNTk1ODcxNl5BMl5BanBnXkFtZTcwMDI1OTMzOQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 55), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 359439), new HashEntry("imdbID", "tt1343092"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gambit"), new HashEntry("Year", 2012), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63489052800), new HashEntry("Runtime", 89),
            new HashEntry("Genre", "Comedy, Crime"), new HashEntry("Director", "Michael Hoffman"),
            new HashEntry("Writer", "Joel Coen (screenplay), Ethan Coen (screenplay), Sidney Carroll (short story)"),
            new HashEntry("Actors", "Colin Firth, Tom Courtenay, Alan Rickman, Mike Noble"),
            new HashEntry("Plot",
                "An art curator decides to seek revenge on his abusive boss by conning him into buying a fake Monet, but his plan requires the help of an eccentric and unpredictable Texas rodeo queen."),
            new HashEntry("Language", "English, Japanese"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjE3NzU5MDUzMF5BMl5BanBnXkFtZTcwNjM4MDQ0OA@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 5.7000000000000002),
            new HashEntry("imdbVotes", 18422), new HashEntry("imdbID", "tt0404978"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Game Change"), new HashEntry("Year", 2012), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "TV-MA"), new HashEntry("Released", 63466934400), new HashEntry("Runtime", 118),
            new HashEntry("Genre", "Biography, Drama, History"), new HashEntry("Director", "Jay Roach"),
            new HashEntry("Writer", "Danny Strong, Mark Halperin (book), John Heilemann (book)"),
            new HashEntry("Actors", "Julianne Moore, Woody Harrelson, Ed Harris, Peter MacNicol"),
            new HashEntry("Plot",
                "This searing, behind-the-scenes look at John McCain's ill-fated 2008 presidential campaign follows the daring plan to add Sarah Palin to the ticket and the implosion that would soon follow."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 3 Golden Globes. Another 28 wins & 38 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQwNjkzNzg4NV5BMl5BanBnXkFtZTcwODIxMTM0Nw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.4000000000000004),
            new HashEntry("imdbVotes", 17138), new HashEntry("imdbID", "tt1848902"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gaslight"), new HashEntry("Year", 1944), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PASSED"), new HashEntry("Released", 61325510400), new HashEntry("Runtime", 114),
            new HashEntry("Genre", "Crime, Drama, Film-Noir"), new HashEntry("Director", "George Cukor"),
            new HashEntry("Writer",
                "John Van Druten (screenplay), Walter Reisch (screenplay), John L. Balderston (screenplay), Patrick Hamilton (play)"),
            new HashEntry("Actors", "Charles Boyer, Ingrid Bergman, Joseph Cotten, Dame May Whitty"),
            new HashEntry("Plot",
                "Years after her aunt was murdered in her home, a young woman moves back into the house with her new husband. However, he has a secret that he will do anything to protect, even if it means driving his wife insane."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 2 Oscars. Another 1 win & 7 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTUyMzg4MTYxNl5BMl5BanBnXkFtZTcwOTY0MjUyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 16579), new HashEntry("imdbID", "tt0036855"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica: The Plan"), new HashEntry("Year", 2009),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "N/A"), new HashEntry("Released", 63392198400),
            new HashEntry("Runtime", 112), new HashEntry("Genre", "Action, Adventure, Drama"),
            new HashEntry("Director", "Edward James Olmos"), new HashEntry("Writer", "Jane Espenson"),
            new HashEntry("Actors", "Edward James Olmos, Dean Stockwell, Michael Trucco, Grace Park"),
            new HashEntry("Plot",
                "When the initial Cylon attack against the Twelve Colonies fails to achieve complete extermination of human life as planned, twin Number Ones (Cavils) embedded on Galactica and Caprica must improvise to destroy the human survivors."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTgyODE2MjY2OF5BMl5BanBnXkFtZTcwMjQxMTI5Mg@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7), new HashEntry("imdbVotes", 14197),
            new HashEntry("imdbID", "tt1286130"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Anne of Green Gables"), new HashEntry("Year", 1985), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 62644579200), new HashEntry("Runtime", 199),
            new HashEntry("Genre", "Drama, Family"), new HashEntry("Director", "N/A"), new HashEntry("Writer", "N/A"),
            new HashEntry("Actors", "Megan Follows"),
            new HashEntry("Plot",
                "An orphan girl, sent to an elderly brother and sister by mistake, charms her new home and community with her fiery spirit and imagination."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 1 Primetime Emmy. Another 11 wins & 6 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNDI4MjA2MzUxOV5BMl5BanBnXkFtZTcwNjE0MjE2MQ@@._V1._CR53,69,306,401_SY132_CR6,0,89,132_AL_.jpg_V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.4000000000000004),
            new HashEntry("imdbVotes", 14044), new HashEntry("imdbID", "tt0088727"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Elevator to the Gallows"), new HashEntry("Year", 1958), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "NOT RATED"), new HashEntry("Released", 61759324800), new HashEntry("Runtime", 91),
            new HashEntry("Genre", "Crime, Drama, Thriller"), new HashEntry("Director", "Louis Malle"),
            new HashEntry("Writer",
                "Roger Nimier (adaptation), Louis Malle (adaptation), Roger Nimier (dialogue), Noël Calef (novel), Noël Calef (pre-adaptation)"),
            new HashEntry("Actors", "Jeanne Moreau, Maurice Ronet, Georges Poujouly, Yori Bertin"),
            new HashEntry("Plot",
                "A self-assured business man murders his employer, the husband of his mistress, which unintentionally provokes an ill-fated chain of events."),
            new HashEntry("Language", "French, German"), new HashEntry("Awards", "1 win."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMzk5NDA0NDYyN15BMl5BanBnXkFtZTgwMDIyMDkwMzE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 92), new HashEntry("imdbRating", 8), new HashEntry("imdbVotes", 13740),
            new HashEntry("imdbID", "tt0051378"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Assassination Games"), new HashEntry("Year", 2011), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63462528000), new HashEntry("Runtime", 101),
            new HashEntry("Genre", "Action, Crime, Drama"), new HashEntry("Director", "Ernie Barbarash"),
            new HashEntry("Writer", "Aaron Rahsaan Thomas"),
            new HashEntry("Actors", "Jean-Claude Van Damme, Scott Adkins, Ivan Kaye, Valentin Teodosiu"),
            new HashEntry("Plot",
                "Two assassins agree to work together as one tries to avenge his wife and the other collect a reward for a job."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTYwODY3MTg0MF5BMl5BanBnXkFtZTcwNDE0NjAxNg@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.2000000000000002),
            new HashEntry("imdbVotes", 13587), new HashEntry("imdbID", "tt1436568"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Bring Me the Head of Alfredo Garcia"), new HashEntry("Year", 1974),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 62299497600),
            new HashEntry("Runtime", 112), new HashEntry("Genre", "Action, Crime, Drama"),
            new HashEntry("Director", "Sam Peckinpah"),
            new HashEntry("Writer",
                "Frank Kowalski (story), Sam Peckinpah (story), Sam Peckinpah (screenplay), Gordon T. Dawson (screenplay)"),
            new HashEntry("Actors", "Warren Oates, Isela Vega, Robert Webber, Gig Young"),
            new HashEntry("Plot",
                "An American bartender and his prostitute girlfriend go on a road trip through the Mexican underworld to collect a $1 million bounty on the head of a dead gigolo."),
            new HashEntry("Language", "English, Spanish"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTIwMDM5MDQ1NF5BMl5BanBnXkFtZTcwNzE3MzgyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 13444),
            new HashEntry("imdbID", "tt0071249"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Pat Garrett & Billy the Kid"), new HashEntry("Year", 1973),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 62251200000),
            new HashEntry("Runtime", 122), new HashEntry("Genre", "Biography, Drama, Romance"),
            new HashEntry("Director", "Sam Peckinpah"), new HashEntry("Writer", "Rudy Wurlitzer"),
            new HashEntry("Actors", "James Coburn, Kris Kristofferson, Richard Jaeckel, Katy Jurado"),
            new HashEntry("Plot",
                "An aging Pat Garrett is hired as a lawman on behalf of a group of wealthy New Mexico cattle barons--his sole purpose being to bring down his old friend Billy the Kid."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 BAFTA Film Award. Another 3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTU2ODYzOTQ4NV5BMl5BanBnXkFtZTcwMDA3MDYyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.4000000000000004),
            new HashEntry("imdbVotes", 13306), new HashEntry("imdbID", "tt0070518"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica: Blood & Chrome"), new HashEntry("Year", 2012),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "UNRATED"), new HashEntry("Released", 63488016000),
            new HashEntry("Runtime", 94), new HashEntry("Genre", "Action, Sci-Fi"),
            new HashEntry("Director", "Jonas Pate"),
            new HashEntry("Writer",
                "Michael Taylor (creator), David Eick (creator), Michael Taylor (teleplay), David Eick (story), Bradley Thompson (story), David Weddle (story), Glen A. Larson (creator: \"Battlestar Galactica\")"),
            new HashEntry("Actors", "Luke Pasqualino, Ben Cotton, Lili Bordán, Jill Teed"),
            new HashEntry("Plot", "The adventures of young William Adama in the First Cylon War."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 3 Primetime Emmys. Another 3 wins & 3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BM2RiNjA4MzctMGU1MC00MThiLTk4OWUtODI4ZmM4MmU3YTllXkEyXkFqcGdeQXVyMDM0MzU2NA@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.0999999999999996),
            new HashEntry("imdbVotes", 13100), new HashEntry("imdbID", "tt1704292"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Gallows"), new HashEntry("Year", 2015), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63572083200), new HashEntry("Runtime", 81),
            new HashEntry("Genre", "Horror, Thriller"), new HashEntry("Director", "Travis Cluff, Chris Lofing"),
            new HashEntry("Writer", "Chris Lofing, Travis Cluff"),
            new HashEntry("Actors", "Reese Mishler, Pfeifer Brown, Ryan Shoos, Cassidy Gifford"),
            new HashEntry("Plot",
                "20 years after a horrific accident during a small town school play, students at the school resurrect the failed show in a misguided attempt to honor the anniversary of the tragedy - but soon discover that some things are better left alone."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTU2MTMyOTkwM15BMl5BanBnXkFtZTgwOTQzNjc3NTE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 30), new HashEntry("imdbRating", 4.2000000000000002),
            new HashEntry("imdbVotes", 13053), new HashEntry("imdbID", "tt2309260"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica: Blood & Chrome"), new HashEntry("Year", 2012),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "UNRATED"), new HashEntry("Released", 63488016000),
            new HashEntry("Runtime", 94), new HashEntry("Genre", "Action, Sci-Fi"),
            new HashEntry("Director", "Jonas Pate"),
            new HashEntry("Writer",
                "Michael Taylor (creator), David Eick (creator), Michael Taylor (teleplay), David Eick (story), Bradley Thompson (story), David Weddle (story), Glen A. Larson (creator: \"Battlestar Galactica\")"),
            new HashEntry("Actors", "Luke Pasqualino, Ben Cotton, Lili Bordán, Jill Teed"),
            new HashEntry("Plot", "The adventures of young William Adama in the First Cylon War."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 3 Primetime Emmys. Another 3 wins & 3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BM2RiNjA4MzctMGU1MC00MThiLTk4OWUtODI4ZmM4MmU3YTllXkEyXkFqcGdeQXVyMDM0MzU2NA@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.0999999999999996),
            new HashEntry("imdbVotes", 13100), new HashEntry("imdbID", "tt1704292"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "When the Game Stands Tall"), new HashEntry("Year", 2014),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "PG"), new HashEntry("Released", 63544262400),
            new HashEntry("Runtime", 115), new HashEntry("Genre", "Drama, Family, Sport"),
            new HashEntry("Director", "Thomas Carter"),
            new HashEntry("Writer",
                "Scott Marshall Smith (screenplay), Scott Marshall Smith (story), David Zelon (story), Neil Hayes (book)"),
            new HashEntry("Actors", "Jim Caviezel, Michael Chiklis, Alexander Ludwig, Clancy Brown"),
            new HashEntry("Plot",
                "The journey of legendary football coach Bob Ladouceur, who took the De La Salle High School Spartans from obscurity to a 151-game winning streak that shattered all records for any American sport."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQ5NDMzOTI3MV5BMl5BanBnXkFtZTgwMjU4MTMyMjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 41), new HashEntry("imdbRating", 6.7000000000000002),
            new HashEntry("imdbVotes", 12415), new HashEntry("imdbID", "tt2247476"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Game of Death"), new HashEntry("Year", 1978), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 62433244800), new HashEntry("Runtime", 85),
            new HashEntry("Genre", "Action, Crime, Drama"), new HashEntry("Director", "Robert Clouse, Bruce Lee"),
            new HashEntry("Writer", "Robert Clouse"),
            new HashEntry("Actors", "Bruce Lee, Colleen Camp, Dean Jagger, Gig Young"),
            new HashEntry("Plot",
                "A martial arts movie star must fake his death to find the people who are trying to kill him."),
            new HashEntry("Language", "English, Mandarin, Cantonese"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkyMjc1MzAwMl5BMl5BanBnXkFtZTYwMDMxMTQ5._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6), new HashEntry("imdbVotes", 12221),
            new HashEntry("imdbID", "tt0077594"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Inspector Gadget"), new HashEntry("Year", 1999), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 63068284800), new HashEntry("Runtime", 78),
            new HashEntry("Genre", "Action, Adventure, Comedy"), new HashEntry("Director", "David Kellogg"),
            new HashEntry("Writer",
                "Andy Heyward (characters), Jean Chalopin (characters), Bruno Bianchi (characters), Dana Olsen (story), Kerry Ehrin (story), Kerry Ehrin (screenplay), Zak Penn (screenplay)"),
            new HashEntry("Actors", "Matthew Broderick, Rupert Everett, Joely Fisher, Michelle Trachtenberg"),
            new HashEntry("Plot",
                "A security guard's dreams come true when he is selected to be transformed into a cybernetic police officer."),
            new HashEntry("Language", "English, Norwegian, French, Spanish"),
            new HashEntry("Awards", "1 win & 9 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTU5NDkwMTUxN15BMl5BanBnXkFtZTcwNzM2OTMyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 36), new HashEntry("imdbRating", 4.0999999999999996),
            new HashEntry("imdbVotes", 36211), new HashEntry("imdbID", "tt0141369"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gabriel"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63330681600), new HashEntry("Runtime", 114),
            new HashEntry("Genre", "Action, Fantasy, Horror"), new HashEntry("Director", "Shane Abbess"),
            new HashEntry("Writer",
                "Matt Hylton Todd (story), Shane Abbess (story), Matt Hylton Todd (screenplay), Shane Abbess (screenplay)"),
            new HashEntry("Actors", "Andy Whitfield, Dwaine Stevenson, Samantha Noble, Michael Piccirilli"),
            new HashEntry("Plot",
                "GABRIEL tells the story of an archangel who fights to bring light back to purgatory - a place where darkness rules - and save the souls of the city's inhabitants."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "2 wins & 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTIyNzQ4ODg5NV5BMl5BanBnXkFtZTcwNTAzMTU1MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 5.7000000000000002),
            new HashEntry("imdbVotes", 12179), new HashEntry("imdbID", "tt0857376"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Koi... Mil Gaya"), new HashEntry("Year", 2003), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63195897600), new HashEntry("Runtime", 171),
            new HashEntry("Genre", "Drama, Fantasy, Romance"), new HashEntry("Director", "Rakesh Roshan"),
            new HashEntry("Writer",
                "Rakesh Roshan (story), Rakesh Roshan (screenplay), Honey Irani (screenplay), Sachin Bhowmick (screenplay), Robin Bhatt (screenplay), Javed Siddiqui (dialogue)"),
            new HashEntry("Actors", "Rekha, Hrithik Roshan, Preity Zinta, Rakesh Roshan"),
            new HashEntry("Plot",
                "A developmentally disabled young man tries to continue the work his father did in communicating with extra-terrestrials from outer space, which leads to something miraculous and wonderful."),
            new HashEntry("Language", "Hindi, English"), new HashEntry("Awards", "25 wins & 30 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTIxNjg4NzkwOV5BMl5BanBnXkFtZTcwNzU2OTUyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.0999999999999996),
            new HashEntry("imdbVotes", 11882), new HashEntry("imdbID", "tt0254481"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Game of Death"), new HashEntry("Year", 1978), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 62433244800), new HashEntry("Runtime", 85),
            new HashEntry("Genre", "Action, Crime, Drama"), new HashEntry("Director", "Robert Clouse, Bruce Lee"),
            new HashEntry("Writer", "Robert Clouse"),
            new HashEntry("Actors", "Bruce Lee, Colleen Camp, Dean Jagger, Gig Young"),
            new HashEntry("Plot",
                "A martial arts movie star must fake his death to find the people who are trying to kill him."),
            new HashEntry("Language", "English, Mandarin, Cantonese"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTkyMjc1MzAwMl5BMl5BanBnXkFtZTYwMDMxMTQ5._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6), new HashEntry("imdbVotes", 12221),
            new HashEntry("imdbID", "tt0077594"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gangster No. 1"), new HashEntry("Year", 2000), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63096105600), new HashEntry("Runtime", 103),
            new HashEntry("Genre", "Crime, Drama, Thriller"), new HashEntry("Director", "Paul McGuigan"),
            new HashEntry("Writer",
                "Johnny Ferguson (screenplay), Louis Mellis (original screenplay), David Scinto (original screenplay)"),
            new HashEntry("Actors", "Malcolm McDowell, David Thewlis, Paul Bettany, Saffron Burrows"),
            new HashEntry("Plot",
                "Chronicles the rise and fall of a prominent, and particularly ruthless English gangster."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 9 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTY1NjYzNDA5Ml5BMl5BanBnXkFtZTcwODM1MTQyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 60), new HashEntry("imdbRating", 6.7999999999999998),
            new HashEntry("imdbVotes", 10930), new HashEntry("imdbID", "tt0210065"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Video Game High School"), new HashEntry("Year", 2012), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63472291200), new HashEntry("Runtime", 42),
            new HashEntry("Genre", "Action, Romance, Sci-Fi"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "N/A"),
            new HashEntry("Actors", "Josh Blaylock, Ellary Porterfield, Johanna Braddy, Jimmy Wong"),
            new HashEntry("Plot",
                "In a futuristic world where gaming is the top sport, a teenager attends a school which specializes in a curriculum of video games in each genre."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "4 wins & 8 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BOTgxOTMwOTAxMF5BMl5BanBnXkFtZTcwMTY3MTExOA@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.7000000000000002),
            new HashEntry("imdbVotes", 10740), new HashEntry("imdbID", "tt2170584"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Fireflies in the Garden"), new HashEntry("Year", 2008), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63351849600), new HashEntry("Runtime", 120),
            new HashEntry("Genre", "Drama"), new HashEntry("Director", "Dennis Lee"),
            new HashEntry("Writer", "Robert Frost (poem), Dennis Lee"),
            new HashEntry("Actors", "Ryan Reynolds, Willem Dafoe, Emily Watson, Carrie-Anne Moss"),
            new HashEntry("Plot",
                "The Taylor family is devastated by an accident that takes place on the day their matriarch is due to graduate from college -- decades after leaving to raise her children."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQ2NjMzMzIzN15BMl5BanBnXkFtZTcwMDM2MzU3Ng@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 34), new HashEntry("imdbRating", 6.5), new HashEntry("imdbVotes", 10580),
            new HashEntry("imdbID", "tt0961108"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gargoyles"), new HashEntry("Year", 1994), new HashEntry("YearEnd", 1996),
            new HashEntry("Rated", "TV-PG"), new HashEntry("Released", 62918553600), new HashEntry("Runtime", 30),
            new HashEntry("Genre", "Animation, Action, Adventure"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Greg Weisman"),
            new HashEntry("Actors", "Keith David, Salli Richardson-Whitfield, Jeff Bennett, Frank Welker"),
            new HashEntry("Plot",
                "A clan of heroic night creatures pledge to protect modern New York City as they did in Scotland long ago"),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 11 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNjE1MzUzMzc2NF5BMl5BanBnXkFtZTcwMTU2NjEzMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 10447), new HashEntry("imdbID", "tt0108783"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "The Gate"), new HashEntry("Year", 1987), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 62683632000), new HashEntry("Runtime", 85),
            new HashEntry("Genre", "Fantasy, Horror"), new HashEntry("Director", "Tibor Takács"),
            new HashEntry("Writer", "Michael Nankin"),
            new HashEntry("Actors", "Stephen Dorff, Christa Denton, Louis Tripp, Kelly Rowan"),
            new HashEntry("Plot",
                "Two young boys accidentally release a horde of nasty, pint-sized demons from a hole in a suburban backyard. What follows is a classic battle between good and evil as the two kids struggle ..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "2 wins & 2 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTI1NDE2MzkwOF5BMl5BanBnXkFtZTcwMDIzMTkzMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 5.9000000000000004),
            new HashEntry("imdbVotes", 10142), new HashEntry("imdbID", "tt0093075"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica"), new HashEntry("Year", 2004), new HashEntry("YearEnd", 2009),
            new HashEntry("Rated", "TV-14"), new HashEntry("Released", 63241257600), new HashEntry("Runtime", 44),
            new HashEntry("Genre", "Action, Adventure, Drama"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Glen A. Larson, Ronald D. Moore"),
            new HashEntry("Actors", "Edward James Olmos, Mary McDonnell, Jamie Bamber, James Callis"),
            new HashEntry("Plot",
                "When an old enemy, the Cylons, resurface and obliterate the 12 colonies, the crew of the aged Galactica protect a small civilian fleet - the last of humanity - as they journey toward the fabled 13th colony, Earth."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 3 Primetime Emmys. Another 32 wins & 80 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTc1NTg1MDk3NF5BMl5BanBnXkFtZTYwNDYyMjI3._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.8000000000000007),
            new HashEntry("imdbVotes", 118636), new HashEntry("imdbID", "tt0407362"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Fair Game"), new HashEntry("Year", 2010), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG-13"), new HashEntry("Released", 63426931200), new HashEntry("Runtime", 108),
            new HashEntry("Genre", "Biography, Drama, Thriller"), new HashEntry("Director", "Doug Liman"),
            new HashEntry("Writer",
                "Jez Butterworth (screenplay), John-Henry Butterworth (screenplay), Joseph Wilson (book), Valerie Plame Wilson (book)"),
            new HashEntry("Actors", "Naomi Watts, Sonya Davison, Vanessa Chong, Anand Tiwari"),
            new HashEntry("Plot",
                "CIA operative Valerie Plame discovers her identity is allegedly leaked by the government as payback for an op-ed article her husband wrote criticizing the Bush administration."),
            new HashEntry("Language", "English, Arabic, French"), new HashEntry("Awards", "3 wins & 9 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjIyOTg1NzU0Ml5BMl5BanBnXkFtZTcwMjA2OTY5Mw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 69), new HashEntry("imdbRating", 6.7999999999999998),
            new HashEntry("imdbVotes", 38196), new HashEntry("imdbID", "tt0977855"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Cats"), new HashEntry("Year", 1998), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 315537897599), new HashEntry("Runtime", 115),
            new HashEntry("Genre", "Musical"), new HashEntry("Director", "David Mallet"),
            new HashEntry("Writer", "T.S. Eliot (book)"),
            new HashEntry("Actors", "James Barron, Jo Bingham, Jacob Brent, Kaye Brown"),
            new HashEntry("Plot",
                "Andrew Lloyd Webber's CATS, the most famous musical of all time, first exploded onto the West End stage in 1981. 'Memory', one of its many classic songs, became an instant worldwide hit. ..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTYxNzQyODcxNV5BMl5BanBnXkFtZTgwNDEzNTAxMjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.4000000000000004),
            new HashEntry("imdbVotes", 872), new HashEntry("imdbID", "tt2227830"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Angry Video Game Nerd"), new HashEntry("Year", 2004),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "NOT RATED"), new HashEntry("Released", 63219484800),
            new HashEntry("Runtime", 0), new HashEntry("Genre", "Comedy"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "James Rolfe"), new HashEntry("Actors", "James Rolfe"),
            new HashEntry("Plot", "A foul-mouthed nerd reviews bad video games."),
            new HashEntry("Language", "English, Spanish"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjEzMTkzNzUzMl5BMl5BanBnXkFtZTgwMDUzNjM1MjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.5999999999999996),
            new HashEntry("imdbVotes", 9819), new HashEntry("imdbID", "tt1230180"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Hunting and Gathering"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63310032000), new HashEntry("Runtime", 97),
            new HashEntry("Genre", "Drama, Romance"), new HashEntry("Director", "Claude Berri"),
            new HashEntry("Writer", "Claude Berri, Anna Gavalda (novel)"),
            new HashEntry("Actors", "Audrey Tautou, Guillaume Canet, Laurent Stocker, Françoise Bertin"),
            new HashEntry("Plot",
                "When Camille (Audrey Tautou) falls ill, she is forced to live with Philibert and Franck (Guillaume Canet). A moving trio story."),
            new HashEntry("Language", "French"), new HashEntry("Awards", "3 wins & 3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTUyMjYyNTEyMl5BMl5BanBnXkFtZTcwMzg5MjQ4MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.7999999999999998),
            new HashEntry("imdbVotes", 9678), new HashEntry("imdbID", "tt0792965"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Beyond the Gates"), new HashEntry("Year", 2005), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63279360000), new HashEntry("Runtime", 115),
            new HashEntry("Genre", "Drama, History, War"), new HashEntry("Director", "Michael Caton-Jones"),
            new HashEntry("Writer", "David Wolstencroft (screenplay), Richard Alwyn (story), David Belton (story)"),
            new HashEntry("Actors", "John Hurt, Hugh Dancy, Dominique Horwitz, Louis Mahoney"),
            new HashEntry("Plot",
                "In April 1994, after the airplane of the Hutu President of Rwanda is shot down, the Hutu militias slaughter the Tutsi population. In the Ecole Technique Officielle, the Catholic priest ..."),
            new HashEntry("Language", "French, English"), new HashEntry("Awards", "1 win & 3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTM4NjA2MTAwNl5BMl5BanBnXkFtZTcwMzc0NzI0MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 71), new HashEntry("imdbRating", 7.7000000000000002),
            new HashEntry("imdbVotes", 9590), new HashEntry("imdbID", "tt0420901"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Heavens Gate with Goggles"), new HashEntry("Year", 2001),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "N/A"), new HashEntry("Released", 315537897599),
            new HashEntry("Runtime", 25), new HashEntry("Genre", "Short"), new HashEntry("Director", "M. dot Strange"),
            new HashEntry("Writer", "M. dot Strange"),
            new HashEntry("Actors", "Damon Barry, Jen Kahler, Maureen Melillo, Stephen Pitkin"),
            new HashEntry("Plot", "N/A"), new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster", "N/A"), new HashEntry("Metascore", 0), new HashEntry("imdbRating", 0),
            new HashEntry("imdbVotes", 0), new HashEntry("imdbID", "tt2004243"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Galavant"), new HashEntry("Year", 2015), new HashEntry("YearEnd", 2016),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63555926400), new HashEntry("Runtime", 22),
            new HashEntry("Genre", "Comedy, Musical"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Dan Fogelman"),
            new HashEntry("Actors", "Joshua Sasse, Timothy Omundson, Vinnie Jones, Mallory Jansen"),
            new HashEntry("Plot",
                "The adventures of Galavant, a dashing hero who is determined to reclaim his reputation and his \"Happily Ever After\" by going after the evil King Richard, who ruined it the moment he stole the love of Galavant's life, Madalena."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "3 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjIwNDE3MjU3N15BMl5BanBnXkFtZTgwOTgyODE4NzE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8), new HashEntry("imdbVotes", 9499),
            new HashEntry("imdbID", "tt3305096"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Gavin"), new HashEntry("Year", 2012), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 315537897599), new HashEntry("Runtime", 10),
            new HashEntry("Genre", "Short, Comedy"), new HashEntry("Director", "Alexandra Edmondson"),
            new HashEntry("Writer", "Alexandra Edmondson, Claire Phillips"),
            new HashEntry("Actors", "Jacob Asquith, Franck Bossi, Harley Connor, Laurence Coy"),
            new HashEntry("Plot",
                "Local hero Gavin put the tiny town of Dundoon on the map. Town mayor Ken Slater wants to plaster Gavin's remains to the walls of the Golden Palace restaurant as a tourist attraction. Enter ..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"), new HashEntry("Poster", "N/A"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.4000000000000004),
            new HashEntry("imdbVotes", 5), new HashEntry("imdbID", "tt2246707"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "GasLand"), new HashEntry("Year", 2010), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63430819200), new HashEntry("Runtime", 107),
            new HashEntry("Genre", "Documentary"), new HashEntry("Director", "Josh Fox"),
            new HashEntry("Writer", "Josh Fox"),
            new HashEntry("Actors", "Josh Fox, Dick Cheney, Pete Seeger, Richard Nixon"),
            new HashEntry("Plot",
                "An exploration of the fracking petroleum extraction industry and the serious environmental consequences involved."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Nominated for 1 Oscar. Another 7 wins & 6 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNDA5NDc4NTUzMl5BMl5BanBnXkFtZTcwMjcxNDg1Mw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.7000000000000002),
            new HashEntry("imdbVotes", 9314), new HashEntry("imdbID", "tt1558250"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Over the Garden Wall"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "TV-PG"), new HashEntry("Released", 63550569600), new HashEntry("Runtime", 110),
            new HashEntry("Genre", "Animation, Adventure, Comedy"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "N/A"), new HashEntry("Actors", "Elijah Wood, Collin Dean, Melanie Lynskey"),
            new HashEntry("Plot",
                "Two brothers find themselves lost in a mysterious land and try to find their way home."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 2 Primetime Emmys. Another 2 wins & 7 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTA0NTYwMjg1NzJeQTJeQWpwZ15BbWU4MDA1MTAwNTMx._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 9.0999999999999996),
            new HashEntry("imdbVotes", 9006), new HashEntry("imdbID", "tt3718778"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "I Am a Fugitive from a Chain Gang"), new HashEntry("Year", 1932),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "NOT RATED"), new HashEntry("Released", 60964272000),
            new HashEntry("Runtime", 92), new HashEntry("Genre", "Crime, Drama, Film-Noir"),
            new HashEntry("Director", "Mervyn LeRoy"),
            new HashEntry("Writer", "Robert E. Burns (by), Howard J. Green (screen play), Brown Holmes (screen play)"),
            new HashEntry("Actors", "Paul Muni, Glenda Farrell, Helen Vinson, Noel Francis"),
            new HashEntry("Plot",
                "Wrongly convicted James Allen serves in the intolerable conditions of a southern chain gang, which later comes back to haunt him."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "Nominated for 3 Oscars. Another 3 wins."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQxODQ5MDMwMl5BMl5BanBnXkFtZTgwOTQ0MTM5MjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 8928), new HashEntry("imdbID", "tt0023042"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Balls Out: Gary the Tennis Coach"), new HashEntry("Year", 2009),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 63374832000),
            new HashEntry("Runtime", 92), new HashEntry("Genre", "Comedy, Sport"),
            new HashEntry("Director", "Danny Leiner"), new HashEntry("Writer", "Andy Stock, Rick Stempson"),
            new HashEntry("Actors", "Seann William Scott, Randy Quaid, Brando Eaton, Emilee Wallace"),
            new HashEntry("Plot",
                "A high school janitor has not recovered from his failed career as a tennis pro. He begins coaching his beloved sport to a group of misfits and leads them to the Nebraska State Championships."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA5OTMzNDY3MV5BMl5BanBnXkFtZTcwMjE2MTQyMg@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 5.5), new HashEntry("imdbVotes", 8554),
            new HashEntry("imdbID", "tt0787470"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Garam Masala"), new HashEntry("Year", 2005), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63266572800), new HashEntry("Runtime", 146),
            new HashEntry("Genre", "Comedy"), new HashEntry("Director", "Priyadarshan"),
            new HashEntry("Writer", "Priyadarshan (screenplay), Neeraj Vora (dialogue), Priyadarshan (story)"),
            new HashEntry("Actors", "Akshay Kumar, John Abraham, Paresh Rawal, Rimi Sen"),
            new HashEntry("Plot",
                "Shyam and Makarand work as photographers in a commercial advertising agency. Both of them like to fool around with women, even though Makarand is engaged to be married to a doctor named ..."),
            new HashEntry("Language", "Hindi"), new HashEntry("Awards", "1 win & 2 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BOTc4MDUxMjMwM15BMl5BanBnXkFtZTgwMjE1MDA2MDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.7000000000000002),
            new HashEntry("imdbVotes", 8425), new HashEntry("imdbID", "tt0453671"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Forbidden Games"), new HashEntry("Year", 1952), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "NOT RATED"), new HashEntry("Released", 61597065600), new HashEntry("Runtime", 86),
            new HashEntry("Genre", "Drama, War"), new HashEntry("Director", "René Clément"),
            new HashEntry("Writer",
                "Jean Aurenche (dialogue), Jean Aurenche (screenplay), Pierre Bost (dialogue), Pierre Bost (screenplay), François Boyer (dialogue), François Boyer (novel), François Boyer (screenplay), René Clément"),
            new HashEntry("Actors", "Georges Poujouly, Brigitte Fossey, Amédée, Laurence Badie"),
            new HashEntry("Plot",
                "A young French girl orphaned in a Nazi air attack is befriended by the son of a poor farmer, and together they try to come to terms with the realities of death."),
            new HashEntry("Language", "French"), new HashEntry("Awards", "Nominated for 1 Oscar. Another 8 wins."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMzE4NTc0MDc1NV5BMl5BanBnXkFtZTgwODAxMTEyNTE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 8416), new HashEntry("imdbID", "tt0043686"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Surviving the Game"), new HashEntry("Year", 1994), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 62901964800), new HashEntry("Runtime", 96),
            new HashEntry("Genre", "Action, Adventure, Crime"), new HashEntry("Director", "Ernest R. Dickerson"),
            new HashEntry("Writer", "Eric Bernt"),
            new HashEntry("Actors", "Ice-T, Rutger Hauer, Charles S. Dutton, Gary Busey"),
            new HashEntry("Plot",
                "A homeless man is hired as a survival guide for a group of wealthy businessmen on a hunting trip in the mountains, unaware that they are killers who hunt humans for sport, and that he is their new prey."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTIxMzY0MDU0OV5BMl5BanBnXkFtZTcwNjIxNTYyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.0999999999999996),
            new HashEntry("imdbVotes", 8232), new HashEntry("imdbID", "tt0111323"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica: The Resistance"), new HashEntry("Year", 2006),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "N/A"), new HashEntry("Released", 63293011200),
            new HashEntry("Runtime", 26), new HashEntry("Genre", "Drama, Sci-Fi"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "N/A"),
            new HashEntry("Actors", "Dominic Zamprogna, Aaron Douglas, Michael Hogan, Christian Tessier"),
            new HashEntry("Plot",
                "Battlestar Galactica: The Resistance is an online series that aims to fill in the gaps between seasons two and three of the Re-imagined Series. The webisodes can be viewed through the ..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BM2NiN2M3MmUtOWFiZC00M2ZjLWI0MjEtNWU4NmQwNGM5NGQyXkEyXkFqcGdeQXVyMDM0MzU2NA@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 8092), new HashEntry("imdbID", "tt0840800"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "The Garden of Words"), new HashEntry("Year", 2013), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63505555200), new HashEntry("Runtime", 46),
            new HashEntry("Genre", "Animation, Drama, Romance"), new HashEntry("Director", "Makoto Shinkai"),
            new HashEntry("Writer", "Makoto Shinkai"),
            new HashEntry("Actors", "Miyu Irino, Kana Hanazawa, Fumi Hirano, Gou Maeda"),
            new HashEntry("Plot",
                "A 15-year-old boy and 27-year-old woman find an unlikely friendship one rainy day in the Shinjuku Gyoen National Garden. These two broken pieces come together and heal one another as they learn what it is to walk."),
            new HashEntry("Language", "Japanese, English"), new HashEntry("Awards", "2 wins & 2 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNDEyYmI1MjQtNDNhYi00MmUxLTgyZmItZTU3MTJiMWIwYmFjXkEyXkFqcGdeQXVyNjQ2NjMyNzA@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.5999999999999996),
            new HashEntry("imdbVotes", 8057), new HashEntry("imdbID", "tt2591814"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Grey Gardens"), new HashEntry("Year", 1975), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 62316604800), new HashEntry("Runtime", 94),
            new HashEntry("Genre", "Documentary, Comedy, Drama"),
            new HashEntry("Director", "Ellen Hovde, Albert Maysles, David Maysles, Muffie Meyer"),
            new HashEntry("Writer", "N/A"),
            new HashEntry("Actors",
                "Edith 'Little Edie' Bouvier Beale, Edith Bouvier Beale, Brooks Hyers, Norman Vincent Peale"),
            new HashEntry("Plot",
                "An old mother and her middle-aged daughter, the aunt and cousin of Jacqueline Kennedy Onassis, live their eccentric lives in a filthy, decaying mansion in East Hampton."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "5 wins & 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BODQ2MzEwNDA0OF5BMl5BanBnXkFtZTgwNTYzMjM2NDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.7000000000000002),
            new HashEntry("imdbVotes", 8052), new HashEntry("imdbID", "tt0073076"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Hitch Hikers Guide to the Galaxy"), new HashEntry("Year", 1981),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "N/A"), new HashEntry("Released", 62540380800),
            new HashEntry("Runtime", 152), new HashEntry("Genre", "Comedy, Sci-Fi, Adventure"),
            new HashEntry("Director", "N/A"), new HashEntry("Writer", "N/A"),
            new HashEntry("Actors", "Peter Jones, Simon Jones, David Dixon, Sandra Dickinson"),
            new HashEntry("Plot",
                "An Earth man and his alien friend escape Earth's destruction and go on a truly strange adventure as space hitchhikers."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "5 wins."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTI2OTMwNDU1NF5BMl5BanBnXkFtZTcwOTIyNzAzMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 8048), new HashEntry("imdbID", "tt0081874"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "The Hungover Games"), new HashEntry("Year", 2014), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63528278400), new HashEntry("Runtime", 85),
            new HashEntry("Genre", "Comedy"), new HashEntry("Director", "Josh Stolberg"),
            new HashEntry("Writer",
                "Kyle Barnett Anderson (screenplay), Kyle Barnett Anderson (story), David Bernstein (screenplay), David Bernstein (story), Jamie Kennedy (story)"),
            new HashEntry("Actors", "Ross Nathan, Sam Pancake, Ben Begley, Herbert Russell"),
            new HashEntry("Plot",
                "After celebrating Doug's upcoming wedding in a cut rate hotel in Laughlin, NV, hungover guys Bradley, Ed and Zach wake up in a futuristic dystopia, having lost their pal, Doug. With the ..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTk0NjM3MDk3NV5BMl5BanBnXkFtZTgwNTEzNTY3MDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 3.7000000000000002),
            new HashEntry("imdbVotes", 8013), new HashEntry("imdbID", "tt3138104"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gainsbourg: A Heroic Life"), new HashEntry("Year", 2010),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "N/A"), new HashEntry("Released", 63399542400),
            new HashEntry("Runtime", 135), new HashEntry("Genre", "Biography, Drama, Music"),
            new HashEntry("Director", "Joann Sfar"),
            new HashEntry("Writer", "Joann Sfar (graphic novel), Joann Sfar (screenplay), Declan May (dialogue)"),
            new HashEntry("Actors", "Eric Elmosnino, Lucy Gordon, Laetitia Casta, Doug Jones"),
            new HashEntry("Plot",
                "A glimpse at the life of French singer Serge Gainsbourg, from growing up in 1940s Nazi-occupied Paris through his successful song-writing years in the 1960s to his death in 1991 at the age of 62."),
            new HashEntry("Language", "English, Russian, French"), new HashEntry("Awards", "6 wins & 7 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTUyMjMzMjU2NF5BMl5BanBnXkFtZTcwOTM2MjgxNg@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 58), new HashEntry("imdbRating", 6.9000000000000004),
            new HashEntry("imdbVotes", 8000), new HashEntry("imdbID", "tt1329457"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Elmer Gantry"), new HashEntry("Year", 1960), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "APPROVED"), new HashEntry("Released", 61835702400), new HashEntry("Runtime", 146),
            new HashEntry("Genre", "Drama"), new HashEntry("Director", "Richard Brooks"),
            new HashEntry("Writer", "Richard Brooks (screenplay), Sinclair Lewis (from the novel by)"),
            new HashEntry("Actors", "Burt Lancaster, Jean Simmons, Arthur Kennedy, Dean Jagger"),
            new HashEntry("Plot",
                "A fast-talking traveling salesman with a charming, loquacious manner convinces a sincere evangelist that he can be an effective preacher for her cause."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 3 Oscars. Another 8 wins & 12 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjI3NjIzMDA4NV5BMl5BanBnXkFtZTcwMzg4NTczNA@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 7897), new HashEntry("imdbID", "tt0053793"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gangaajal"), new HashEntry("Year", 2003), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63197712000), new HashEntry("Runtime", 157),
            new HashEntry("Genre", "Crime, Drama"), new HashEntry("Director", "Prakash Jha"),
            new HashEntry("Writer", "Prakash Jha (screenplay)"),
            new HashEntry("Actors", "Ajay Devgn, Gracy Singh, Mohan Joshi, Yashpal Sharma"),
            new HashEntry("Plot",
                "An IPS officer motivates and leads a dysfunctional, corrupt police force of Tezpur to fight against the corrupt politician."),
            new HashEntry("Language", "Hindi"), new HashEntry("Awards", "3 wins & 26 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BYjRjOWViZTgtYjA4Ny00MWJiLTkxYzktMzdlOGRmMWYwOTdjXkEyXkFqcGdeQXVyNDUzOTQ5MjY@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.9000000000000004),
            new HashEntry("imdbVotes", 7825), new HashEntry("imdbID", "tt0373856"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Another Gay Movie"), new HashEntry("Year", 2006), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "NOT RATED"), new HashEntry("Released", 63313747200), new HashEntry("Runtime", 92),
            new HashEntry("Genre", "Comedy, Romance"), new HashEntry("Director", "Todd Stephens"),
            new HashEntry("Writer", "Tim Kaltenecker (story), Todd Stephens (screenplay), Todd Stephens (story)"),
            new HashEntry("Actors", "Michael Carbonaro, Jonah Blechman, Jonathan Chase, Mitch Morris"),
            new HashEntry("Plot",
                "Four gay high school friends make a pact to lose their virginity before they go to college."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "2 wins & 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTYzMTIzNDI3MV5BMl5BanBnXkFtZTgwNDQwMjMxMTE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 37), new HashEntry("imdbRating", 5), new HashEntry("imdbVotes", 7769),
            new HashEntry("imdbID", "tt0443431"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Garfield and Friends"), new HashEntry("Year", 1988), new HashEntry("YearEnd", 1995),
            new HashEntry("Rated", "TV-Y"), new HashEntry("Released", 62726054400), new HashEntry("Runtime", 30),
            new HashEntry("Genre", "Animation, Comedy"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Jim Davis"),
            new HashEntry("Actors", "Lorenzo Music, Thom Huge, Gregg Berger, Howard Morris"),
            new HashEntry("Plot",
                "Stories about Garfield the cat, Odie the dog, their owner Jon and the trouble they get into, and Orson the Pig and his adventures on a farm with fellow farm animals."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 4 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjE1NDQwODgxMF5BMl5BanBnXkFtZTcwNzEzMjYyMQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 7680), new HashEntry("imdbID", "tt0094469"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "Grey Gardens"), new HashEntry("Year", 1975), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "PG"), new HashEntry("Released", 62316604800), new HashEntry("Runtime", 94),
            new HashEntry("Genre", "Documentary, Comedy, Drama"),
            new HashEntry("Director", "Ellen Hovde, Albert Maysles, David Maysles, Muffie Meyer"),
            new HashEntry("Writer", "N/A"),
            new HashEntry("Actors",
                "Edith 'Little Edie' Bouvier Beale, Edith Bouvier Beale, Brooks Hyers, Norman Vincent Peale"),
            new HashEntry("Plot",
                "An old mother and her middle-aged daughter, the aunt and cousin of Jacqueline Kennedy Onassis, live their eccentric lives in a filthy, decaying mansion in East Hampton."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "5 wins & 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BODQ2MzEwNDA0OF5BMl5BanBnXkFtZTgwNTYzMjM2NDE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.7000000000000002),
            new HashEntry("imdbVotes", 8052), new HashEntry("imdbID", "tt0073076"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Real Gangsters"), new HashEntry("Year", 2013), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63505641600), new HashEntry("Runtime", 88),
            new HashEntry("Genre", "Crime, Drama, Mystery"), new HashEntry("Director", "Frank D'Angelo"),
            new HashEntry("Writer", "Frank D'Angelo (screenplay), Frank D'Angelo (story), Philip Morton (screenplay)"),
            new HashEntry("Actors", "Paul Amato, Steven Bauer, Jason Blicker, Frank A. Caruso"),
            new HashEntry("Plot",
                "Real Gangsters tells the story of the Lo Giacamo family, one of the most successful crime syndicates in New York City. Run by cousins Vincent Lo Giacamo (Nick Mancuso) and Jack Lo Giacamo (..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjIwNjQ3NTc2NF5BMl5BanBnXkFtZTcwNzc0MjM4OQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.5), new HashEntry("imdbVotes", 6803),
            new HashEntry("imdbID", "tt2865074"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Most Dangerous Game"), new HashEntry("Year", 1932), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "NOT RATED"), new HashEntry("Released", 60958742400), new HashEntry("Runtime", 63),
            new HashEntry("Genre", "Adventure, Horror, Mystery"),
            new HashEntry("Director", "Irving Pichel, Ernest B. Schoedsack"),
            new HashEntry("Writer",
                "James Ashmore Creelman (screen play), Richard Connell (from the O'Henry prize winning story by)"),
            new HashEntry("Actors", "Joel McCrea, Fay Wray, Robert Armstrong, Leslie Banks"),
            new HashEntry("Plot",
                "An insane hunter arranges for a ship to be wrecked on an island where he can indulge in some sort of hunting and killing of the passengers."),
            new HashEntry("Language", "English, Russian"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNzU4MjYxMjI1N15BMl5BanBnXkFtZTgwNjk1NTAwMjE@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.2999999999999998),
            new HashEntry("imdbVotes", 6799), new HashEntry("imdbID", "tt0023238"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Babylon 5: The Gathering"), new HashEntry("Year", 1993),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "UNRATED"), new HashEntry("Released", 62865936000),
            new HashEntry("Runtime", 89), new HashEntry("Genre", "Action, Adventure, Sci-Fi"),
            new HashEntry("Director", "Richard Compton"), new HashEntry("Writer", "J. Michael Straczynski"),
            new HashEntry("Actors", "Michael O'Hare, Tamlyn Tomita, Jerry Doyle, Mira Furlan"),
            new HashEntry("Plot",
                "The opening of a crucial space station is put in jeopardy when the commanding officer is accused of the attempted murder of a diplomat."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 1 Primetime Emmy. Another 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTMzMTUzMDA5OV5BMl5BanBnXkFtZTYwNTA3NDc5._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.5999999999999996),
            new HashEntry("imdbVotes", 6742), new HashEntry("imdbID", "tt0106336"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Johnny Gaddaar"), new HashEntry("Year", 2007), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63326534400), new HashEntry("Runtime", 135),
            new HashEntry("Genre", "Crime, Drama, Thriller"), new HashEntry("Director", "Sriram Raghavan"),
            new HashEntry("Writer", "Vinay Choudary (additional screenplay), Sriram Raghavan"),
            new HashEntry("Actors", "Dharmendra, Rimi Sen, Ashwini Khalsekar, Neil Nitin Mukesh"),
            new HashEntry("Plot",
                "Police officers recount a tale of missing money which results in lies, deceit, betrayal and death."),
            new HashEntry("Language", "Hindi"), new HashEntry("Awards", "3 wins & 1 nomination."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BNDE4MmU1YTktMzY1OS00NWQ4LWExZmYtZjI3MzJjNGIxZTcxXkEyXkFqcGdeQXVyNDUzOTQ5MjY@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8), new HashEntry("imdbVotes", 6732),
            new HashEntry("imdbID", "tt1077248"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "The Gates"), new HashEntry("Year", 2010), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "TV-14"), new HashEntry("Released", 63412588800), new HashEntry("Runtime", 60),
            new HashEntry("Genre", "Crime, Drama, Fantasy"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Grant Scharbo, Richard Hatem"),
            new HashEntry("Actors", "Rhona Mitra, Frank Grillo, Marisol Nichols, Luke Mably"),
            new HashEntry("Plot",
                "A metropolitan police officer becomes chief of police in a gated suburban neighborhood where vampires, werewolves, witches and other supernatural entities reside."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "N/A"),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTMwMDM2MDg4NF5BMl5BanBnXkFtZTcwMjkzODc1Mw@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 7.2000000000000002),
            new HashEntry("imdbVotes", 6722), new HashEntry("imdbID", "tt1599357"), new HashEntry("Type", "series")
        },

        new[]
        {
            new HashEntry("Title", "The Gathering"), new HashEntry("Year", 2003), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "R"), new HashEntry("Released", 63188553600), new HashEntry("Runtime", 92),
            new HashEntry("Genre", "Horror, Thriller"), new HashEntry("Director", "Brian Gilbert"),
            new HashEntry("Writer", "Anthony Horowitz"),
            new HashEntry("Actors", "Christina Ricci, Ioan Gruffudd, Stephen Dillane, Kerry Fox"),
            new HashEntry("Plot",
                "While going to the town of Ashby Wake, the drifter Cassie is hit by a car driven by Marion Kirkman and loses her memory. Marion invites Cassie to stay in her huge old house with her family,..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win & 2 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTI4Mjc2Mjg1Nl5BMl5BanBnXkFtZTYwMzA3ODk5._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 5.7000000000000002),
            new HashEntry("imdbVotes", 6675), new HashEntry("imdbID", "tt0294594"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Asterix the Gaul"), new HashEntry("Year", 1967), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 62090755200), new HashEntry("Runtime", 68),
            new HashEntry("Genre", "Animation, Action, Adventure"), new HashEntry("Director", "Ray Goossens"),
            new HashEntry("Writer",
                "René Goscinny (comic), Albert Uderzo (comic), Willy Lateste, Jos Marissen, László Molnár"),
            new HashEntry("Actors", "Roger Carel, Jacques Morel, Pierre Tornade, Jacques Jouanneau"),
            new HashEntry("Plot", "Gaul invaded! Asterix must save the Druid Getafix from the Romans."),
            new HashEntry("Language", "French"), new HashEntry("Awards", "1 win."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTQyNDIzODY5MV5BMl5BanBnXkFtZTYwMTUxMjE5._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 6.5999999999999996),
            new HashEntry("imdbVotes", 6516), new HashEntry("imdbID", "tt0061369"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Flying Swords of Dragon Gate"), new HashEntry("Year", 2011),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 63459504000),
            new HashEntry("Runtime", 122), new HashEntry("Genre", "Action, Adventure"),
            new HashEntry("Director", "Hark Tsui"), new HashEntry("Writer", "Hark Tsui (screenplay)"),
            new HashEntry("Actors", "Jet Li, Xun Zhou, Kun Chen, Lun Mei Gwei"),
            new HashEntry("Plot",
                "Set three years after Dragon Inn, innkeeper Jade has disappeared and a new inn has risen from the ashes - one that's staffed by marauders masquerading as law-abiding citizens, who hope to unearth the fabled lost city buried in the desert."),
            new HashEntry("Language", "Mandarin"), new HashEntry("Awards", "11 wins & 29 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTAxMDk2ODE0NjdeQTJeQWpwZ15BbWU3MDkxMDQwMzg@._V1_SX300.jpg"),
            new HashEntry("Metascore", 57), new HashEntry("imdbRating", 6), new HashEntry("imdbVotes", 6289),
            new HashEntry("imdbID", "tt1686784"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Gamers"), new HashEntry("Year", 2006), new HashEntry("YearEnd", 0),
            new HashEntry("Rated", "N/A"), new HashEntry("Released", 63278496000), new HashEntry("Runtime", 84),
            new HashEntry("Genre", "Comedy"), new HashEntry("Director", "Christopher Folino"),
            new HashEntry("Writer", "Christopher Folino"),
            new HashEntry("Actors", "Kevin Sherwood, Kevin Kirkpatrick, Scott Rinker, Dave Hanson"),
            new HashEntry("Plot",
                "Gamers is a comedy about the lives of four slacker friends (and one obsessive interloper) living at home, -\"with my parents... it's just temporary... 'til they die\"- working in the real ..."),
            new HashEntry("Language", "English"), new HashEntry("Awards", "1 win."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMjA5MjE0NTQ5N15BMl5BanBnXkFtZTcwOTUzNTM2MQ@@._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.0999999999999996),
            new HashEntry("imdbVotes", 6252), new HashEntry("imdbID", "tt0485909"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Flying Swords of Dragon Gate"), new HashEntry("Year", 2011),
            new HashEntry("YearEnd", 0), new HashEntry("Rated", "R"), new HashEntry("Released", 63459504000),
            new HashEntry("Runtime", 122), new HashEntry("Genre", "Action, Adventure"),
            new HashEntry("Director", "Hark Tsui"), new HashEntry("Writer", "Hark Tsui (screenplay)"),
            new HashEntry("Actors", "Jet Li, Xun Zhou, Kun Chen, Lun Mei Gwei"),
            new HashEntry("Plot",
                "Set three years after Dragon Inn, innkeeper Jade has disappeared and a new inn has risen from the ashes - one that's staffed by marauders masquerading as law-abiding citizens, who hope to unearth the fabled lost city buried in the desert."),
            new HashEntry("Language", "Mandarin"), new HashEntry("Awards", "11 wins & 29 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTAxMDk2ODE0NjdeQTJeQWpwZ15BbWU3MDkxMDQwMzg@._V1_SX300.jpg"),
            new HashEntry("Metascore", 57), new HashEntry("imdbRating", 6), new HashEntry("imdbVotes", 6289),
            new HashEntry("imdbID", "tt1686784"), new HashEntry("Type", "movie")
        },

        new[]
        {
            new HashEntry("Title", "Battlestar Galactica"), new HashEntry("Year", 2004), new HashEntry("YearEnd", 2009),
            new HashEntry("Rated", "TV-14"), new HashEntry("Released", 63241257600), new HashEntry("Runtime", 44),
            new HashEntry("Genre", "Action, Adventure, Drama"), new HashEntry("Director", "N/A"),
            new HashEntry("Writer", "Glen A. Larson, Ronald D. Moore"),
            new HashEntry("Actors", "Edward James Olmos, Mary McDonnell, Jamie Bamber, James Callis"),
            new HashEntry("Plot",
                "When an old enemy, the Cylons, resurface and obliterate the 12 colonies, the crew of the aged Galactica protect a small civilian fleet - the last of humanity - as they journey toward the fabled 13th colony, Earth."),
            new HashEntry("Language", "English"),
            new HashEntry("Awards", "Won 3 Primetime Emmys. Another 32 wins & 80 nominations."),
            new HashEntry("Poster",
                "http://ia.media-imdb.com/images/M/MV5BMTc1NTg1MDk3NF5BMl5BanBnXkFtZTYwNDYyMjI3._V1_SX300.jpg"),
            new HashEntry("Metascore", 0), new HashEntry("imdbRating", 8.8000000000000007),
            new HashEntry("imdbVotes", 118636), new HashEntry("imdbID", "tt0407362"), new HashEntry("Type", "series")
        },
    };
}