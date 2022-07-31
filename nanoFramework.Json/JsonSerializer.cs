//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright Mike Jones, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
    /// </summary>
    public class JsonSerializer
    {
        internal static Hashtable EscapableCharactersMapping = new Hashtable()
        {
            {'\n', 'n'},
            {'\r', 'r'},
            {'\"', '"' }
        };

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="o">The value to convert. Supported types are: <see cref="bool"/>, <see cref="string"/>, <see cref="byte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,  <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="Array"/>, <see cref="IDictionary"/>, <see cref="IEnumerable"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="TimeSpan"/>, <see cref="DictionaryEntry"/>, <see cref="object"/> and <see langword="null"/>.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only internal properties with getters are converted.</remarks>
        internal static string SerializeObject(object o, bool topObject = true)
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

            switch (type.Name)
            {
                case "Boolean":
                    return (bool)o ? "true" : "false";

                case "TimeSpan":
                case "Char":
                case "Guid":
                    return "\"" + o.ToString() + "\"";
                case "String":
                    return "\"" + SerializeString((string)o) + "\"";

                case "Single":
                    if (float.IsNaN((float)o))
                    {
                        return "null";
                    }
                    return o.ToString();

                case "Double":
                    if (double.IsNaN((double)o))
                    {
                        return "null";
                    }
                    return o.ToString();

                case "Decimal":
                case "Float":
                case "Byte":
                case "SByte":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                    return o.ToString();

                case "DateTime":
                    return "\"" + nanoFramework.Json.DateTimeExtensions.ToIso8601((DateTime)o) + "\"";
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

            if (type == typeof(DictionaryEntry))
            {
                Hashtable hashtable = new();

                if (o is DictionaryEntry)
                {
                    DictionaryEntry dic = (DictionaryEntry)o;
                    DictionaryEntry entry = dic;
                    hashtable.Add(entry.Key, entry.Value);
                }

                return SerializeIDictionary(hashtable);
            }

            if (type.IsClass)
            {
                Hashtable hashtable = new Hashtable();

                // Iterate through all of the methods, looking for internal GET properties
                MethodInfo[] methods = type.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    // We care only about property getters when serializing
                    if (method.Name.StartsWith("get_"))
                    {
                        // Ignore abstract and virtual objects
                        if (method.IsAbstract)
                        {
                            continue;
                        }

                        // Ignore delegates and MethodInfos
                        if ((method.ReturnType == typeof(System.Delegate)) ||
                            (method.ReturnType == typeof(System.MulticastDelegate)) ||
                            (method.ReturnType == typeof(System.Reflection.MethodInfo)))
                        {
                            continue;
                        }

                        // Ditto for DeclaringType
                        if ((method.DeclaringType == typeof(System.Delegate)) ||
                            (method.DeclaringType == typeof(System.MulticastDelegate)))
                        {
                            continue;
                        }

                        object returnObject = method.Invoke(o, null);
                        hashtable.Add(method.Name.Substring(4), returnObject);
                    }
                }

                return SerializeIDictionary(hashtable);
            }

            return null;
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

        internal static bool StringContainsCharactersToEscape(string str, bool deserializing)
        {
            foreach (var item in EscapableCharactersMapping.Keys)
            {
                var charToCheck = deserializing ? $"\\{EscapableCharactersMapping[item]}" : item.ToString();
                if (str.IndexOf(charToCheck) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool CheckIfCharIsRequiresEscape(char chr)
        {
            foreach (var item in EscapableCharactersMapping.Keys)
            {
                if ((char)item == chr)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Safely serialize a String into a JSON string value, escaping all backslash and quote characters.
        /// </summary>
        /// <param name="str">The string to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        protected static string SerializeString(string str)
        {
            // If the string is just fine (most are) then make a quick exit for improved performance
            if (!StringContainsCharactersToEscape(str, false))
            {
                return str;
            }

            // Build a new string
            // we know there is at least 1 char to escape
            StringBuilder result = new(str.Length + 1);

            foreach (char ch in str)
            {
                var charToAppend = ch;
                if (CheckIfCharIsRequiresEscape(charToAppend))
                {
                    result.Append('\\');
                    charToAppend = (char)EscapableCharactersMapping[charToAppend];
                }

                result.Append(charToAppend);
            }

            return result.ToString();
        }
    }
}
