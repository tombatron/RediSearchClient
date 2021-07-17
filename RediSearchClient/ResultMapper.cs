using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace RediSearchClient
{
    public static class ResultMapper
    {
        public sealed class MapperDefinition
        {
            public string SourceField {get;}

            public string DestinationPropertyName {get;}

            public Func<RedisResult, object> Converter {get;}

            public MapperDefinition(string sourceField, string destinationPropertyName, Func<RedisResult, object> converter)
            {
                SourceField = sourceField;
                DestinationPropertyName = destinationPropertyName;
                Converter = converter;
            }

            public static implicit operator MapperDefinition((string sourceField, string destinationPropertyName, Func<RedisResult, object> converter) source)
            {
                return new MapperDefinition(source.sourceField, source.destinationPropertyName, source.converter);
            }
        }

        internal class MapperDefinitionContainer
        {
            private readonly MapperDefinition[] _mappers;

            public MapperDefinitionContainer(MapperDefinition[] mappers) =>
                _mappers = mappers;

            public TTarget Apply<TTarget>(SearchResultItem searchResultItem)
            {
                throw new NotImplementedException();
            }
        }

        private static ConcurrentDictionary<Type, MapperDefinitionContainer> _mapperDefinitions = 
            new ConcurrentDictionary<Type, MapperDefinitionContainer>();

        public static void CreateMapFor<TTarget>(params MapperDefinition[] mappers)
        {
            if(!_mapperDefinitions.ContainsKey(typeof(TTarget)))
            {
                _mapperDefinitions.TryAdd(typeof(TTarget), new MapperDefinitionContainer(mappers));
            }
        }

        internal static void SynthesizeMapFor<TTarget>()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TTarget> MapTo<TTarget>(SearchResult searchResult)
        {
            if (!_mapperDefinitions.ContainsKey(typeof(TTarget)))
            {
                SynthesizeMapFor<TTarget>();
            }

            if(_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
            {
                foreach(var sr in searchResult)
                {
                    yield return mapper.Apply<TTarget>(sr);
                }
            }

            yield break;
        }
    }
}