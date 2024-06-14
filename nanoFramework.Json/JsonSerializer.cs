//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright Mike Jones, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using System;
using System.Collections;
using System.Reflection;

namespace nanoFramework.Json
{
    internal static class JsonSerializer
    {
        private static string SerializeClass(object value, Type type)
        {
            Hashtable hashtable = new();

            // Iterate through all the methods, looking for internal GET properties
            var methods = type.GetMethods();

            foreach (var method in methods)
            {
                if (!ShouldSerializeMethod(method))
                {
                    continue;
                }

                var invokedValue = method.Invoke(value, null);
                hashtable.Add(method.Name.Substring(4), invokedValue);
            }

            return SerializeDictionary(hashtable);
        }

        /// <summary>
        /// Convert an <see cref="IDictionary"/> to a JSON string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        internal static string SerializeDictionary(IDictionary value)
        {
            // TODO: Come back later to investigate using a StringBuilder
            var result = "{";

            foreach (DictionaryEntry entry in value)
            {
                if (result.Length > 1)
                {
                    result += ",";
                }

                result += $"\"{entry.Key}\":";
                result += SerializeObject(entry.Value, false);
            }

            result += "}";

            return result;
        }

        /// <summary>
        /// Convert an <see cref="IEnumerable"/> to a JSON string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        internal static string SerializeEnumerable(IEnumerable value)
        {
            // TODO: Come back later to investigate using a StringBuilder
            var result = "[";

            foreach (var current in value)
            {
                if (result.Length > 1)
                {
                    result += ",";
                }

                result += SerializeObject(current, false);
            }

            result += "]";

            return result;
        }

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="value">The value to convert. Supported types are: <see cref="bool"/>, <see cref="string"/>, <see cref="byte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,  <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="Array"/>, <see cref="IDictionary"/>, <see cref="IEnumerable"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="TimeSpan"/>, <see cref="DictionaryEntry"/>, <see cref="object"/> and <see langword="null"/>.</param>
        /// <param name="topObject">Is the object top in hierarchy. Default true.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only internal properties with getters are converted.</remarks>
        public static string SerializeObject(object value, bool topObject = true)
        {
            if (value is null)
            {
                return "null";
            }

            var type = value.GetType();

            if (topObject && !type.IsArray && type.BaseType?.FullName == "System.ValueType")
            {
                return $"[{SerializeObject(value, false)}]";
            }

            var converter = ConvertersMapping.GetConverter(type);
            if (converter is not null)
            {
                return converter.ToJson(value);
            }

            if (type.IsEnum)
            {
                return value.ToString();
            }

            switch (value)
            {
                case IDictionary dictionary when !type.IsArray:
                    return SerializeDictionary(dictionary);
                case IEnumerable enumerable:
                    return SerializeEnumerable(enumerable);
            }

            return type.IsClass ? SerializeClass(value, type) : null;
        }

        private static bool ShouldSerializeMethod(MethodInfo method)
        {
            // We care only about property getters when serializing
            if (!method.Name.StartsWith("get_"))
            {
                return false;
            }

            // Ignore abstract and virtual objects
            if (method.IsAbstract)
            {
                return false;
            }

            // Ignore static methods
            if (method.IsStatic)
            {
                return false;
            }

            // Ignore Delegate, MethodInfo, and MulticastDelegate
            if (method.ReturnType == typeof(Delegate) || method.ReturnType == typeof(MethodInfo) || method.ReturnType == typeof(MulticastDelegate))
            {
                return false;
            }

            // Ignore Delegate and MulticastDelegate
            if (method.DeclaringType == typeof(Delegate) || method.DeclaringType == typeof(MulticastDelegate))
            {
                return false;
            }

            return true;
        }
    }
}
