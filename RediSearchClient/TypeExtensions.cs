using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RediSearchClient
{
    internal static class TypeExtensions
    {
        private static readonly IEnumerable<Type> NumericTypes = new List<Type>
        {
            typeof(byte),
            typeof(byte?),
            typeof(sbyte),
            typeof(sbyte?),
            typeof(short),
            typeof(short?),
            typeof(ushort),
            typeof(ushort?),
            typeof(int),
            typeof(int?),
            typeof(uint),
            typeof(uint?),
            typeof(long),
            typeof(long?),
            typeof(ulong),
            typeof(ulong?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?)
        };

        internal static bool IsNullableBooleanType(this Type t) => t.FullName == typeof(bool?).FullName;

        internal static bool IsNullableCharType(this Type t) => t.FullName == typeof(char?).FullName;

        internal static bool IsStringType(this Type t) => t.FullName == typeof(string).FullName;

        internal static bool IsObjectType(this Type t) => t.FullName == typeof(object).FullName;

        internal static bool IsNumericType(this Type t) => NumericTypes.Select(x => x.FullName).Contains(t.FullName);

        internal static bool IsBooleanType(this Type t) => t.FullName == typeof(bool).FullName || t.IsNullableBooleanType();

        internal static bool IsDateTimeType(this Type t) => t.FullName == typeof(DateTime).FullName || t.FullName == typeof(DateTime?).FullName;

        internal static bool IsBuiltInType(this Type t) => t.IsPrimitive || t.IsNullableBooleanType() || t.IsNullableCharType() || t.IsNumericType() || t.IsStringType() || t.IsDateTimeType() || t.IsObjectType();

        internal static bool IsCollectionType(this Type t) => t.GetInterfaces().Any(x => x.Name == typeof(IEnumerable<>).Name || x.Name == typeof(IEnumerable).Name);
    }
}
