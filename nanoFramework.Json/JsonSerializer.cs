// Source code is modified from Mike Jones's JSON Serialization and Deserialization library (https://www.ghielectronics.com/community/codeshare/entry/357)

using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace nanoFramework.json
{
    /// <summary>
    /// Adapted from JSON.NetMF - JSON Serialization and Deserialization library for .NET Micro Framework
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="o">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, TimeSpan, DictionaryEntry, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only internal properties with getters are converted.</remarks>
        internal static string SerializeObject(object o, bool topObject = true)
        {
            if (o == null)
                return "null";

            Type type = o.GetType();

            if (topObject 
                && !type.IsArray
                && (type.BaseType.FullName == "System.ValueType"
                || type.FullName == "System.String"))
            {
                return $"[{SerializeObject(o, false)}]";
            }

            switch (type.Name)
            {
                case "Boolean":
                        return (bool)o ? "true" : "false";
                case "TimeSpan":
                case "String":
                case "Char":
                case "Guid":
                        return "\"" + o.ToString() + "\"";
                case "Single":
                        if (float.IsNaN((Single)o))
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

            if (type == typeof(System.Collections.DictionaryEntry))
            {
                Hashtable hashtable = new Hashtable();
                if (o is DictionaryEntry)
                {
                    var dic = (DictionaryEntry)o;
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

                result += "\"" + entry.Key + "\":";
                result += SerializeObject(entry.Value, false);
            }
            result += "}";
 
            return result;
        }

        /// <summary>
        /// Safely serialize a String into a JSON string value, escaping all backslash and quote characters.
        /// </summary>
        /// <param name="str">The string to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        protected static string SerializeString(String str)
        {
            // If the string is just fine (most are) then make a quick exit for improved performance
            if (str.IndexOf('\\') < 0 && str.IndexOf('\"') < 0)
            {
                return str;
            }

            // Build a new string
            StringBuilder result = new StringBuilder(str.Length + 1); // we know there is at least 1 char to escape
 
            foreach (char ch in str.ToCharArray())
            {
                if (ch == '\\' || ch == '\"')
                    result.Append('\\');
                result.Append(ch);
            }
            
            return result.ToString();
        }
    }
}
