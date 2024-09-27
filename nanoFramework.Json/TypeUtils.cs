#nullable enable
using System;
using System.Collections;

namespace nanoFramework.Json
{
    internal static class TypeUtils
    {
        // A very small optimization occurs by caching these types
        public static readonly Type ArrayListType = typeof(ArrayList);
        public static readonly Type HashTableType = typeof(Hashtable);
        public static readonly Type StringType = typeof(string);
        public static readonly Type ValueTypeType = typeof(ValueType);

        public static bool IsArrayList(Type? type) => ArrayListType == type;

        public static bool IsHashTable(Type? type) => HashTableType == type;

        public static bool IsString(Type? type) => StringType == type;

        public static bool IsValueType(Type? type) => ValueTypeType == type?.BaseType;
    }
}
