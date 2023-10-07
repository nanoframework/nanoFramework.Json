//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright Mike Jones, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using nanoFramework.Json.Converters;
using System;
using System.Collections;
using System.Reflection;

namespace nanoFramework.Json
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
    /// </summary>
    public class JsonSerializer
    {
        JsonSerializer()
        {
        }

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="o">The value to convert. Supported types are: <see cref="bool"/>, <see cref="string"/>, <see cref="byte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,  <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="Array"/>, <see cref="IDictionary"/>, <see cref="IEnumerable"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="TimeSpan"/>, <see cref="DictionaryEntry"/>, <see cref="object"/> and <see langword="null"/>.</param>
        /// <param name="topObject">Is the object top in hierarchy. Default true.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only internal properties with getters are converted.</remarks>
        public static string SerializeObject(object o, bool topObject = true)
        {
            if (o == null)
            {
                return "null";
            }

            Type type = o.GetType();

            if (topObject
                && !type.IsArray
                && type.BaseType.FullName == "System.ValueType")
            {
                return $"[{SerializeObject(o, false)}]";
            }

            var converter = ConvertersMapping.GetConverter(type);
            if (converter != null)
            {
                return converter.ToJson(o);
            }

            if (type.IsEnum)
            {
                return o.ToString();
            }

            if (o is IDictionary && !type.IsArray)
            {
                IDictionary dictionary = o as IDictionary;
                return SerializeIDictionary(dictionary);
            }

            if (o is IEnumerable)
            {
                IEnumerable enumerable = o as IEnumerable;
                return SerializeIEnumerable(enumerable);
            }

            if (type.IsClass)
            {
                return SerializeClass(o, type);
            }

            return null;
        }

        private static string SerializeClass(object o, Type type)
        {
            Hashtable hashtable = new();

            // Iterate through all of the methods, looking for internal GET properties
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (!ShouldSerializeMethod(method))
                {
                    continue;
                }

                object returnObject = method.Invoke(o, null);
                hashtable.Add(method.Name.Substring(4), returnObject);
            }

            return SerializeIDictionary(hashtable);
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

            // Ignore delegates and MethodInfos
            if ((method.ReturnType == typeof(Delegate)) ||
                (method.ReturnType == typeof(MulticastDelegate)) ||
                (method.ReturnType == typeof(MethodInfo)))
            {
                return false;
            }

            // Ditto for DeclaringType
            if ((method.DeclaringType == typeof(Delegate)) ||
                (method.DeclaringType == typeof(MulticastDelegate)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Convert an IEnumerable to a JSON string.
        /// </summary>
        /// <param name="enumerable">The value to convert.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        internal static string SerializeIEnumerable(IEnumerable enumerable)
        {
            string result = "[";

            foreach (object current in enumerable)
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
        /// Convert an IDictionary to a JSON string.
        /// </summary>
        /// <param name="dictionary">The value to convert.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        internal static string SerializeIDictionary(IDictionary dictionary)
        {
            string result = "{";

            foreach (DictionaryEntry entry in dictionary)
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
    }
}
