using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace RediSearchClient
{
    /// <summary>
    /// Utility class for mapping a search result to a custom type.
    /// </summary>
    public static class ResultMapper
    {
        /// <summary>
        /// Defines a mapping.
        /// </summary>
        public sealed class MapperDefinition
        {
            /// <summary>
            /// The name of the field in the search result.
            /// </summary>
            /// <value></value>
            public string SourceField { get; }

            /// <summary>
            /// The name of the property in the custom type to map the value to.
            /// </summary>
            /// <value></value>
            public string DestinationPropertyName { get; }

            /// <summary>
            /// Converter function used to convert the RedisResult to whatever type is needed.
            /// </summary>
            /// <value></value>
            public Func<RedisResult, object> Converter { get; }

            /// <summary>
            /// Initializes a MapperDefinition.
            /// </summary>
            /// <param name="sourceField"></param>
            /// <param name="destinationPropertyName"></param>
            /// <param name="converter"></param>
            public MapperDefinition(string sourceField, string destinationPropertyName, Func<RedisResult, object> converter)
            {
                SourceField = sourceField;
                DestinationPropertyName = destinationPropertyName;
                Converter = converter;
            }

            /// <summary>
            /// Allows for the implict conversion between a (string, string, Func&lt;RedisResult, object&gt;) tuple to a "MapperDefinition".
            /// </summary>
            /// <param name="source"></param>
            public static implicit operator MapperDefinition((string sourceField, string destinationPropertyName, Func<RedisResult, object> converter) source)
            {
                return new MapperDefinition(source.sourceField, source.destinationPropertyName, source.converter);
            }
        }

        internal class MapperDefinitionContainer
        {
            public MapperDefinition[] Mappers { get; }

            public MapperDefinitionContainer(MapperDefinition[] mappers) =>
                Mappers = mappers;

            public TTarget Apply<TTarget>(SearchResultItem searchResultItem) where TTarget : new()
            {
                var result = new TTarget();

                if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
                {
                    foreach (var m in mapper.Mappers)
                    {
                        var prop = typeof(TTarget).GetProperty(m.DestinationPropertyName);

                        prop.SetValue(result, m.Converter(searchResultItem[m.SourceField]));
                    }
                }
                else
                {
                    // TODO: Throw an exception here if the mapper isn't found. It should have been created by now though...
                }

                return result;
            }
        }

        private static ConcurrentDictionary<Type, MapperDefinitionContainer> _mapperDefinitions =
            new ConcurrentDictionary<Type, MapperDefinitionContainer>();

        /// <summary>
        /// This method is for providing the type mappings ahead of time.
        /// </summary>
        /// <param name="mappers"></param>
        /// <typeparam name="TTarget"></typeparam>
        public static void CreateMapFor<TTarget>(params MapperDefinition[] mappers) where TTarget : new()
        {
            if (!_mapperDefinitions.ContainsKey(typeof(TTarget)))
            {
                _mapperDefinitions.TryAdd(typeof(TTarget), new MapperDefinitionContainer(mappers));
            }
        }

        /// <summary>
        /// Creates a type mapping for types that... don't have type mappings.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        internal static void SynthesizeMapFor<TTarget>() where TTarget : new()
        {
            var mappers = new List<MapperDefinition>();

            foreach (var p in typeof(TTarget).GetProperties())
            {
                mappers.Add((p.Name, p.Name, CreateConverter(p)));
            }

            CreateMapFor<TTarget>(mappers.ToArray());
        }

        /// <summary>
        /// Creates a converter delegate. 
        /// 
        /// !!! If you want to add more supported types to the mapper then this is where you would do that. !!!
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static Func<RedisResult, object> CreateConverter(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;

            if (type == typeof(int))
            {
                return (r) => (int)r;
            }
            else if (type == typeof(long))
            {
                return (r) => (long)r;
            }
            else if (type == typeof(double))
            {
                return (r) => (double)r;
            }
            else
            {
                return (r) => r.ToString();
            }
        }

        /// <summary>
        /// Maps a SearchResult collection to a collection of... whatever you want.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <typeparam name="TTarget">The type to map to.</typeparam>
        /// <returns></returns>
        public static IEnumerable<TTarget> MapTo<TTarget>(SearchResult searchResult) where TTarget : new()
        {
            if (!_mapperDefinitions.ContainsKey(typeof(TTarget)))
            {
                SynthesizeMapFor<TTarget>();
            }

            if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
            {
                foreach (var sr in searchResult)
                {
                    yield return mapper.Apply<TTarget>(sr);
                }
            }

            yield break;
        }
    }
}