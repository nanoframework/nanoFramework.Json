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
        private static bool _useIgnoreAttribute = false;
        /// <summary>
        /// If true, will check for JsonIgnoreAttribute upon serialization. Has a performance cost.
        /// </summary>
        public static bool UseIgnoreAttribute
        {
            get => _useIgnoreAttribute;
            set => _useIgnoreAttribute = value;
        }

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

            if (ConvertersMapping.ConversionTable.Contains(type))
            {
                var converter = (IConverter)ConvertersMapping.ConversionTable[type];
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
            // Cache the type's class-level attributes only if UseIgnoreAttribute setting is enabled.
            object[] classAttributes = null;
            if (UseIgnoreAttribute)
            {
                classAttributes = type.GetCustomAttributes(false);
            }

            Hashtable hashtable = new();

            // Iterate through all of the methods, looking for internal GET properties
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (!ShouldSerializeMethod(method, classAttributes))
                {
                    continue;
                }
                
                object returnObject = method.Invoke(o, null);
                hashtable.Add(ExtractGetterName(method), returnObject);
            }

            return SerializeIDictionary(hashtable);
        }

        /// <summary>
        /// Checks whether a property (MethodInfo) should be serialized.
        /// </summary>
        /// <param name="method">The MethodInfo to check.</param>
        /// <param name="classAttributes">The cached class-level attributes. Only used if UseIgnoreAttribute is true.</param>
        private static bool ShouldSerializeMethod(MethodInfo method, object[] classAttributes)
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

            // Ignore property getters with at least one parameter (index properties / this[index] operator overloads)
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                return false;
            }

            // Only check for attribute if the setting is on
            if (UseIgnoreAttribute)
            {
                // Ignore properties listed in [JsonIgnore()] attribute
                if (ShouldIgnorePropertyFromClassAttribute(method, classAttributes))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks for JsonIgnore attribute on a method's declaring class. (helper method for SerializeClass)
        /// </summary>
        /// <param name="method">The MethodInfo of a property getter to check.</param>
        /// <param name="classAttributes">The cached class-level attributes. Only used if UseIgnoreAttribute is true.</param>
        /// <returns></returns>
        private static bool ShouldIgnorePropertyFromClassAttribute(MethodInfo method, object[] classAttributes)
        {
            string[] gettersToIgnore = null;
            
            foreach (object attribute in classAttributes)
            {
                if (attribute is JsonIgnoreAttribute ignoreAttribute)
                {
                    gettersToIgnore = ignoreAttribute.PropertyNames;
                    break;
                }
            }

            if (gettersToIgnore == null)
            {
                return false;
            }

            foreach (string propertyName in gettersToIgnore)
            {
                if (propertyName.Equals(ExtractGetterName(method)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Extracts "get_" from MethodInfo.Name to retrieve the name of a getter property.
        /// Assumes the MethodInfo is for a getter, checked elsewhere.
        /// </summary>
        /// <param name="getterMethodInfo">The MethodInfo of the getter property.</param>
        /// <returns>The property name as it appears in written code.</returns>
        private static string ExtractGetterName(MethodInfo getterMethodInfo)
        {
            // Substring(4) is to extract the "get_"
            return getterMethodInfo.Name.Substring(4);
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
