using RediSearchClient.Exceptions;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RediSearchClient
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use the generic ResultMapper<TTarget> instead.")]
    public static class ResultMapper
    {
        /// <summary>
        /// [Obsolete] Use `CreateMap` from `ResultMapper&lt;TTarget&gt;` instead.
        /// 
        /// This method is for providing the type mappings ahead of time.
        /// </summary>
        /// <param name="mappers"></param>
        /// <typeparam name="TTarget"></typeparam>
        [Obsolete("Use `CreateMap` from `ResultMapper<TTarget>` instead.")]
        public static void CreateMapFor<TTarget>(params ResultMapper<TTarget>.MapperDefinition[] mappers) where TTarget : new() =>
            ResultMapper<TTarget>.CreateMap(mappers);
    }

    /// <summary>
    /// Utility class for mapping a search result to a custom type.
    /// </summary>
    public static class ResultMapper<TTarget> where TTarget : new()
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
            /// The property in the custom type to map the value to.
            /// </summary>
            /// <value></value>
            public PropertyInfo DestinationProperty { get; }

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
                Converter = converter;

                var props = typeof(TTarget).GetProperties();
                var prop = props.FirstOrDefault(x => x.Name == destinationPropertyName);

                if (prop == default)
                {
                    throw new Exception($"Couldn't find a property called: {destinationPropertyName}");
                }

                DestinationProperty = prop;
            }

            /// <summary>
            /// Initializes a MapperDefinition.
            /// </summary>
            /// <param name="sourceField"></param>
            /// <param name="destinationProperty"></param>
            /// <param name="converter"></param>
            public MapperDefinition(string sourceField, PropertyInfo destinationProperty, Func<RedisResult, object> converter)
            {
                SourceField = sourceField;
                DestinationProperty = destinationProperty;
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
            public List<MapperDefinition> Mappers { get; }

            public MapperDefinitionContainer(MapperDefinition[] mappers)
            {
                Mappers = new List<MapperDefinition>(mappers);
            }

            public TTarget Apply(SearchResultItem searchResultItem)
            {
                var result = new TTarget();

                if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
                {
                    foreach (var m in mapper.Mappers)
                    {
                        m.DestinationProperty.SetValue(result, m.Converter(searchResultItem[m.SourceField]));
                    }
                }
                else
                {
                    // TODO: Throw an exception here if the mapper isn't found. It should have been created by now though...
                }

                return result;
            }

            public TTarget Apply(AggregateResultCollection aggregateCollection)
            {
                var result = new TTarget();

                if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
                {
                    foreach (var m in mapper.Mappers)
                    {
                        m.DestinationProperty.SetValue(result, m.Converter(aggregateCollection[m.SourceField]));
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
        public static void CreateMap(params MapperDefinition[] mappers)
        {
            if (!_mapperDefinitions.ContainsKey(typeof(TTarget)))
            {
                _mapperDefinitions.TryAdd(typeof(TTarget), new MapperDefinitionContainer(mappers));
            }
        }

        internal static void AppendMap(string sourceField, PropertyInfo destinationProperty, Func<RedisResult, object> converter)
        {
            var mapperDefinition = new MapperDefinition(sourceField, destinationProperty, converter);

            if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapperDefinitionContainer))
            {
                mapperDefinitionContainer.Mappers.Add(mapperDefinition);
            }
            else
            {
                CreateMap(mapperDefinition);
            }
        }

        /// <summary>
        /// Builder class for configuring mappings. 
        /// </summary>
        public class MapperBuilder
        {
            internal MapperBuilder() { }

            /// <summary>
            /// Designate a property from the custom type to be mapped. 
            /// 
            /// This overload will assume that the key of the result and property name are exactly the same, and will attempt
            /// to convert the value of the result to the same type as the property.
            /// </summary>
            /// <param name="destinationPropertyExpression"></param>
            /// <typeparam name="_"></typeparam>
            /// <returns></returns>
            public MapperBuilder ForMember<_>(Expression<Func<TTarget, _>> destinationPropertyExpression)
            {
                var destinationPropertyInfo = GetPropertyInfoByExpression(destinationPropertyExpression);
                var converter = GetConverterByPropertyInfo(destinationPropertyInfo);

                AppendMap(destinationPropertyInfo.Name, destinationPropertyInfo, converter);

                return this;
            }

            /// <summary>
            /// Designate a property from the custom type to be mapped, along with a delegate that will convert the record value
            /// to the appropriate type for the property being mapped to.
            /// </summary>
            /// <param name="destinationProperty"></param>
            /// <param name="converter"></param>
            /// <returns></returns>
            public MapperBuilder ForMember(Expression<Func<TTarget, object>> destinationProperty, Func<RedisResult, object> converter)
            {
                var destinationPropertyInfo = GetPropertyInfoByExpression(destinationProperty);
                var sourceField = destinationPropertyInfo.Name;

                AppendMap(sourceField, destinationPropertyInfo, converter);

                return this;
            }

            /// <summary>
            /// Setup an explicit mapping between a result key/value and a destination property, the value from the result and we'll attempt
            /// to automatically convert the value to the type of the destination property.
            /// </summary>
            /// <param name="sourceField"></param>
            /// <param name="destinationProperty"></param>
            /// <returns></returns>
            public MapperBuilder ForMember(string sourceField, Expression<Func<TTarget, object>> destinationProperty)
            {
                var destinationPropertyInfo = GetPropertyInfoByExpression(destinationProperty);
                var converter = GetConverterByPropertyInfo(destinationPropertyInfo);

                AppendMap(sourceField, destinationPropertyInfo, converter);

                return this;
            }

            /// <summary>
            /// Setup an explict mapping between a result key/value and a destination property, with a converter.
            /// </summary>
            /// <param name="sourceField">Key name of the result entry.</param>
            /// <param name="destinationProperty">The property being mapped to.</param>
            /// <param name="converter">A delegate that will convert the result value to the appropriate type.</param>
            /// <returns></returns>
            public MapperBuilder ForMember(string sourceField, Expression<Func<TTarget, object>> destinationProperty, Func<RedisResult, object> converter)
            {
                var propertyInfo = GetPropertyInfoByExpression(destinationProperty);

                AppendMap(sourceField, propertyInfo, converter);

                return this;
            }

            private static PropertyInfo GetPropertyInfoByExpression<TPropertyType>(Expression<Func<TTarget, TPropertyType>> destinationPropertyExpression)
            {
                if (destinationPropertyExpression == default)
                {
                    throw new ArgumentNullException(nameof(destinationPropertyExpression));
                }

                var property = default(MemberExpression);

                if (destinationPropertyExpression.Body.NodeType == ExpressionType.Convert)
                {
                    var convert = destinationPropertyExpression.Body as UnaryExpression;

                    if (convert != default)
                    {
                        property = convert.Operand as MemberExpression;
                    }
                }

                if (property == default)
                {
                    property = destinationPropertyExpression.Body as MemberExpression;
                }

                if (property == default)
                {
                    throw new ArgumentException("The expression cannot be null and should be passed in the format of: x => x.PropertyName");
                }

                var propertyInfo = property.Member as PropertyInfo;

                return propertyInfo;
            }

            private static Func<RedisResult, object> GetConverterByPropertyInfo(PropertyInfo propertyInfo)
            {
                var type = propertyInfo.PropertyType;

                if (type == typeof(bool))
                {
                    return ConvertRedisResultToBoolean;
                }
                else if (type == typeof(bool[]))
                {
                    return ConvertRedisResultToBooleanArray;
                }
                else if (type == typeof(byte[]))
                {
                    return ConvertRedisResultToByteArray;
                }
                else if (type == typeof(byte[][]))
                {
                    return ConvertRedisResultToByteArrayArray;
                }
                else if (type == typeof(double))
                {
                    return ConvertRedisResultToDouble;
                }
                else if (type == typeof(double[]))
                {
                    return ConvertRedisResultToDoubleArray;
                }
                else if (type == typeof(int))
                {
                    return ConvertRedisResultToInteger;
                }
                else if (type == typeof(int[]))
                {
                    return ConvertRedisResultToIntegerArray;
                }
                else if (type == typeof(long))
                {
                    return ConvertRedisResultToLongInteger;
                }
                else if (type == typeof(long[]))
                {
                    return ConvertRedisResultToLongIntegerArray;
                }
                else if (type == typeof(ulong))
                {
                    return ConvertRedisResultToUnsignedLongInteger;
                }
                else if (type == typeof(ulong[]))
                {
                    return ConvertRedisResultToUnsignedLongIntegerArray;
                }
                else if (type == typeof(bool?))
                {
                    return ConvertRedisResultToNullableBoolean;
                }
                else if (type == typeof(double?))
                {
                    return ConvertRedisResultToNullableDouble;
                }
                else if (type == typeof(int?))
                {
                    return ConvertRedisResultToNullableInteger;
                }
                else if (type == typeof(long?))
                {
                    return ConvertRedisResultToNullableLongInteger;
                }
                else if (type == typeof(ulong?))
                {
                    return ConvertRedisResultToNullableUnsignedLongInteger;
                }
                else if (type == typeof(string))
                {
                    return ConvertRedisResultToString;
                }
                else if (type == typeof(string[]))
                {
                    return ConvertRedisResultToStringArray;
                }
                else
                {
                    throw new ResultMapperConfigurationException("Couldn't resolve a converter by type, you should try to create the mapping using `.ForMember(x => x.PropertyName, r => ConversionCode(r))`.");
                }
            }

            private static object ConvertRedisResultToBoolean(RedisResult result) => (bool)result;

            private static object ConvertRedisResultToBooleanArray(RedisResult result) => (bool[])result;

            private static object ConvertRedisResultToByteArray(RedisResult result) => (byte[])result;

            private static object ConvertRedisResultToByteArrayArray(RedisResult result) => (byte[][])result;

            private static object ConvertRedisResultToString(RedisResult result) => (string)result;

            private static object ConvertRedisResultToStringArray(RedisResult result) => (string[])result;

            private static object ConvertRedisResultToInteger(RedisResult result) => (int)result;

            private static object ConvertRedisResultToIntegerArray(RedisResult result) => (int[])result;

            private static object ConvertRedisResultToLongInteger(RedisResult result) => (long)result;

            private static object ConvertRedisResultToLongIntegerArray(RedisResult result) => (long[])result;

            private static object ConvertRedisResultToUnsignedLongInteger(RedisResult result) => (ulong)result;

            private static object ConvertRedisResultToUnsignedLongIntegerArray(RedisResult result) => (ulong[])result;

            private static object ConvertRedisResultToDouble(RedisResult result) => (double)result;

            private static object ConvertRedisResultToDoubleArray(RedisResult result) => (double[])result;

            private static object ConvertRedisResultToNullableBoolean(RedisResult result) => (bool?)result;

            private static object ConvertRedisResultToNullableDouble(RedisResult result) => (double?)result;

            private static object ConvertRedisResultToNullableInteger(RedisResult result) => (int?)result;

            private static object ConvertRedisResultToNullableLongInteger(RedisResult result) => (long?)result;

            private static object ConvertRedisResultToNullableUnsignedLongInteger(RedisResult result) => (ulong?)result;
        }

        /// <summary>
        /// Static factory method for creating a mapper builder. 
        /// </summary>
        /// <returns></returns>
        public static MapperBuilder CreateMap()
        {
            return new MapperBuilder();
        }

        /// <summary>
        /// Creates a type mapping for types that... don't have type mappings.
        /// </summary>
        internal static void SynthesizeMapFor()
        {
            var mappers = new List<MapperDefinition>();

            foreach (var p in typeof(TTarget).GetProperties())
            {
                mappers.Add((p.Name, p.Name, CreateConverter(p)));
            }

            CreateMap(mappers.ToArray());
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

        private static void RegisterMapper(Action<MapperBuilder> inlineBuilder)
        {
            if (!_mapperDefinitions.ContainsKey(typeof(TTarget)))
            {
                if (inlineBuilder == default)
                {
                    SynthesizeMapFor();
                }
                else
                {
                    inlineBuilder(CreateMap());
                }

            }
        }

        /// <summary>
        /// Maps a SearchResult collection to a collection of... whatever you want.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <param name="inlineBuilder">Optional builder for declaring a type mapping inline.</param>
        /// <returns></returns>
        public static IEnumerable<TTarget> MapTo(SearchResult searchResult, Action<MapperBuilder> inlineBuilder = default)
        {
            RegisterMapper(inlineBuilder);

            if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
            {
                foreach (var sr in searchResult)
                {
                    yield return mapper.Apply(sr);
                }
            }
        }

        /// <summary>
        /// Maps an AggregateResult collection to a collection of custom types. 
        /// </summary>
        /// <param name="aggregateResult">The aggregate result.</param>
        /// <param name="inlineBuilder">Optional builder for declaring a type mapping inline.</param>
        /// <returns></returns>
        public static IEnumerable<TTarget> MapTo(AggregateResult aggregateResult, Action<MapperBuilder> inlineBuilder = default)
        {
            RegisterMapper(inlineBuilder);
            
            if (_mapperDefinitions.TryGetValue(typeof(TTarget), out var mapper))
            {
                foreach (var ar in aggregateResult)
                {
                    yield return mapper.Apply(ar);
                }
            }
        }
    }
}