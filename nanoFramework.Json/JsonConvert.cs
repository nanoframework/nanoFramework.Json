//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json
{
    /// <summary>
    /// Provides methods for converting between .NET types and JSON types.
    /// </summary>
    public static class JsonConvert
    {
        private enum TokenType
        {
            LBrace, RBrace, LArray, RArray, Colon, Comma, String, Number, Date, Error,
            True, False, Null, End
        }

        private struct LexToken
        {
            public TokenType TType;
            public string TValue;
        }

        internal class SerializationCtx
        {
            public int Indent;
        }

        internal static SerializationCtx SerializationContext = null;
        internal static object SyncObj = new();

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="oSource">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, DictionaryEntry, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only public properties with getters are converted.</remarks>
        public static string SerializeObject(object oSource)
        {
            var type = oSource.GetType();

            if (type.IsArray)
            {
                JsonToken retToken = JsonArray.Serialize(type, oSource);

                return retToken.ToString();
            }
            else
            {
                JsonToken retToken;

                if (type.FullName == "System.Collections.ArrayList")
                {
                    retToken = JsonObject.Serialize((ArrayList)oSource);
                }
                else if (type.BaseType.FullName == "System.ValueType"
                         || type.FullName == "System.String")
                {
                    JsonToken[] jsonValue = new JsonToken[1];
                    jsonValue[0] = JsonValue.Serialize(type, oSource);
                    JsonArray jsonArray = new JsonArray(jsonValue);

                    return jsonArray.ToString();
                }
                else
                {
                    retToken = JsonObject.Serialize(type, oSource);
                }

                return retToken.ToString();
            }
        }

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="type">The object type to convert to</param>
        /// <returns></returns>
        public static object DeserializeObject(string sourceString, Type type)
        {
            var dserResult = Deserialize(sourceString);
            return PopulateObject((JsonToken)dserResult, type, "/");
        }

#if NANOFRAMEWORK_1_0

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type">The object type to convert to</param>
        /// <returns></returns>
        public static object DeserializeObject(Stream stream, Type type)
        {
            var dserResult = Deserialize(stream);
            return PopulateObject((JsonToken)dserResult, type, "/");
        }

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="type">The object type to convert to</param>
        /// <returns></returns>
        public static object DeserializeObject(StreamReader dr, Type type)
        {
            var dserResult = Deserialize(dr);

            return PopulateObject((JsonToken)dserResult, type, "/");
        }

        private static object Deserialize(StreamReader dr)
        {
            // Read the DataReader into jsonBytes[]
            jsonBytes = new byte[dr.BaseStream.Length];
            jsonPos = 0;

            while (!dr.EndOfStream)
            {
                jsonBytes[jsonPos++] = (byte)dr.Read();
            }

            jsonPos = 0;

            return Deserialize();
        }

#endif

        private static object PopulateObject(JsonToken rootToken, Type rootType, string rootPath)
        {
            if (
                (rootToken == null)
                || (rootType == null)
                || (rootPath == null))
            {
                // All parameters must be non-null
                throw new DeserializationException();
            }

            try
            {
                Type rootElementType = rootType.GetElementType();

                if (rootToken is JsonObject rootObject)
                {
                    bool isHashtable = false;
                    bool isArrayList = false;

                    if (rootElementType == null
                        && rootType.FullName == "System.Collections.Hashtable")
                    {
                        isHashtable = true;

                        rootElementType = rootType;
                    }

                    if (rootElementType == null
                       && rootType.FullName == "System.Collections.ArrayList")
                    {
                        isArrayList = true;

                        rootElementType = rootType;
                    }

                    if (isHashtable)
                    {
                        Hashtable rootInstance = new();

                        foreach (var m in rootObject.Members)
                        {
                            var memberProperty = (JsonProperty)m;

                            if (memberProperty.Value is JsonValue jsonValue)
                            {
                                rootInstance.Add(memberProperty.Name, jsonValue.Value);
                            }
                            else if (memberProperty.Value is JsonArray jsonArray)
                            {
                                rootInstance.Add(memberProperty.Name, PopulateArrayList(jsonArray));
                            }
                            else if (memberProperty.Value is JsonToken jsonToken)
                            {
                                rootInstance.Add(memberProperty.Name, PopulateHashtable(jsonToken));
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }

                        return rootInstance;
                    }
                    else if (isArrayList)
                    {
                        ArrayList rootArrayList = new();

                        // In case we have elements to put there.
                        var result = new Hashtable();
                        foreach (var m in rootObject.Members)
                        {
                            var memberProperty = (JsonProperty)m;

                            if (m is JsonValue value)
                            {
                                rootArrayList.Add(value.Value);
                            }
                            else if (m is JsonArray jsonArray)
                            {
                                rootArrayList.Add(PopulateArrayList(jsonArray));
                            }
                            else if (m is JsonToken jsonToken)
                            {
                                result.Add(memberProperty.Name, PopulateObject(memberProperty.Value));
                            }
                            else
                            {
                                throw new DeserializationException();
                            }
                        }

                        if (result.Count > 0)
                        {
                            rootArrayList.Add(result);
                        }
                        return rootArrayList;
                    }
                    else
                    {
                        // This is the object that gets populated and returned
                        // Create rootInstance from the rootType's constructor
                        object rootInstance = null;

                        // Empty array of Types - GetConstructor didn't work unless given an empty array of Type[]
                        Type[] types = { };

                        ConstructorInfo ci = rootType.GetConstructor(types);

                        if (ci == null)
                        {
                            // failed to create target instance
                            throw new DeserializationException();
                        }

                        rootInstance = ci.Invoke(null);

                        // If we haven't successfully created rootInstance, bail out
                        if (rootInstance == null)
                        {
                            // failed to create target instance from rootType
                            throw new DeserializationException();
                        }

                        if ((rootObject == null) || (rootObject.Members == null))
                        {
                            // failed to create target instance from rootType
                            throw new DeserializationException();
                        }

                        // Process all members for this rootObject
                        foreach (var m in rootObject.Members)
                        {
                            var memberProperty = (JsonProperty)m;

                            string memberPropertyName = memberProperty.Name;

                            // workaround for for property names that start with '$' like Azure Twins
                            if (memberPropertyName[0] == '$')
                            {
                                memberPropertyName = "_" + memberProperty.Name.Substring(1);
                            }

                            // Figure out if we're dealing with a Field or a Property and handle accordingly
                            Type memberType = null;
                            FieldInfo memberFieldInfo = null;
                            MethodInfo memberPropSetMethod = null;
                            MethodInfo memberPropGetMethod = null;
                            bool memberIsProperty = false;

                            memberFieldInfo = rootType.GetField(memberPropertyName);

                            if (memberFieldInfo != null)
                            {
                                memberType = memberFieldInfo.FieldType;
                                memberIsProperty = false;
                            }
                            else
                            {
                                memberPropGetMethod = rootType.GetMethod("get_" + memberPropertyName);

                                if (memberPropGetMethod == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    memberType = memberPropGetMethod.ReturnType;
                                    memberPropSetMethod = rootType.GetMethod("set_" + memberPropertyName);

                                    if (memberType == null)
                                    {
                                        // failed to get setter of memberType {rootType.Name}. Possibly this property doesn't have a setter.
                                        throw new DeserializationException();
                                    }

                                    memberIsProperty = true;
                                }
                            }
                            // Process the member based on JObject, JValue, or JArray
                            if (memberProperty.Value is JsonObject @object)
                            {
                                // Call PopulateObject() for this member - i.e. recursion
                                var memberPath = rootPath;

                                if (memberPath[memberPath.Length - 1] == '/')
                                {
                                    // Don't need to add a slash before appending rootElementType
                                    memberPath += memberPropertyName;
                                }
                                else
                                {
                                    // Need to add a slash before appending rootElementType
                                    memberPath = memberPath + '/' + memberPropertyName;
                                }

                                object memberObject = null;

                                // check if property type it's HashTable
                                if (memberType.FullName == "System.Collections.Hashtable")
                                {
                                    Hashtable table = new();

                                    foreach (JsonProperty v in @object.Members)
                                    {
                                        if (v.Value is JsonValue jsonValue)
                                        {
                                            table.Add(v.Name, (jsonValue).Value);
                                        }
                                        else if (v.Value is JsonObject JsonObject)
                                        {
                                            table.Add(v.Name, PopulateHashtable(JsonObject));
                                        }
                                        else if (v.Value is JsonArray jsonArrayAttribute)
                                        {
                                            throw new NotImplementedException();
                                        }
                                    }

                                    memberObject = table;
                                }
                                else
                                {
                                    memberObject = PopulateObject(memberProperty.Value, memberType, memberPath);
                                }

                                if (memberIsProperty)
                                {
                                    memberPropSetMethod.Invoke(rootInstance, new object[] { memberObject });
                                }
                                else
                                {
                                    memberFieldInfo.SetValue(rootInstance, memberObject);
                                }
                            }
                            else if (memberProperty.Value is JsonValue memberPropertyValue)
                            {
                                if (memberType != typeof(DateTime))
                                {
                                    if (memberPropertyValue.Value == null)
                                    {
                                        if (memberIsProperty)
                                        {
                                            if (!memberPropGetMethod.ReturnType.IsValueType)
                                            {
                                                memberPropSetMethod.Invoke(rootInstance, new object[] { null });
                                            }
                                            else
                                            {
                                                switch (memberPropGetMethod.ReturnType.Name)
                                                {
                                                    case "Single":
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { float.NaN });
                                                        break;

                                                    case "Double":
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { double.NaN });
                                                        break;

                                                    default:
                                                        break;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            object obj = null;
                                            memberFieldInfo.SetValue(rootInstance, obj);
                                        }
                                    }
                                    else
                                    {
                                        if (memberIsProperty)
                                        {
                                            JsonValue val = memberPropertyValue;

                                            if (val.Value.GetType() != memberType)
                                            {
                                                // Note: keeping the full names Int16, UInt16 for readability
                                                switch (memberType.Name)
                                                {
                                                    case nameof(Int16):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToInt16(val.Value.ToString()) });
                                                        break;

                                                    case nameof(UInt16):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToUInt16(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Int32):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToInt32(val.Value.ToString()) });
                                                        break;

                                                    case nameof(UInt32):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToUInt32(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Int64):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToInt64(val.Value.ToString()) });
                                                        break;

                                                    case nameof(UInt64):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToUInt64(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Byte):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToByte(val.Value.ToString()) });
                                                        break;

                                                    case nameof(SByte):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToSByte(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Single):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToSingle(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Double):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToDouble(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Boolean):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToBoolean(Convert.ToByte(val.Value.ToString())) });
                                                        break;

                                                    case nameof(TimeSpan):
                                                        if (TimeSpanExtensions.TryConvertFromString(val.Value.ToString(), out TimeSpan value))
                                                        {
                                                            memberPropSetMethod.Invoke(rootInstance, new object[] { value });
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            return null;
                                                        }

                                                    default:
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { memberPropertyValue.Value });
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                memberPropSetMethod.Invoke(rootInstance, new object[] { memberPropertyValue.Value });
                                            }
                                        }
                                        else
                                        {
                                            memberFieldInfo.SetValue(rootInstance, memberPropertyValue.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (memberIsProperty)
                                    {
                                        memberPropSetMethod.Invoke(rootInstance, new object[] { memberPropertyValue.Value });
                                    }
                                    else
                                    {
                                        memberFieldInfo.SetValue(rootInstance, memberPropertyValue.Value);
                                    }
                                }
                            }
                            else if (memberProperty.Value is JsonArray array)
                            {
                                // Need this type when we try to populate the array elements
                                Type memberElementType = memberType.GetElementType();
                                isArrayList = false;

                                if (memberElementType == null && memberType.FullName == "System.Collections.ArrayList")
                                {
                                    memberElementType = memberType;

                                    isArrayList = true;
                                }

                                // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                                var memberValueArray = array;

                                // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                                var memberValueArrayList = new ArrayList();

                                // Create a JToken[] array for Items associated for this memberProperty.Value
                                JsonToken[] memberItems = memberValueArray.Items;

                                foreach (JsonToken item in memberItems)
                                {
                                    if (item is JsonValue value)
                                    {
                                        if (memberPropGetMethod == null)
                                        {
                                            // {rootType.Name} must have a valid Property Get Method
                                            throw new DeserializationException();
                                        }

                                        if (value.Value.GetType() != memberPropGetMethod.ReturnType)
                                        {
                                            if (memberPropGetMethod.ReturnType.Name.Contains("UInt16"))
                                            {
                                                memberValueArrayList.Add(Convert.ToUInt16(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Int16"))
                                            {
                                                memberValueArrayList.Add(Convert.ToInt16(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("UInt32"))
                                            {
                                                memberValueArrayList.Add(Convert.ToUInt32(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Int32"))
                                            {
                                                memberValueArrayList.Add(Convert.ToInt32(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("UInt64"))
                                            {
                                                memberValueArrayList.Add(Convert.ToUInt64(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Int64"))
                                            {
                                                memberValueArrayList.Add(Convert.ToInt64(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("SByte"))
                                            {
                                                memberValueArrayList.Add(Convert.ToSByte(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Byte"))
                                            {
                                                memberValueArrayList.Add(Convert.ToByte(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Single"))
                                            {
                                                memberValueArrayList.Add(Convert.ToSingle(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Double"))
                                            {
                                                memberValueArrayList.Add(Convert.ToDouble(value.Value.ToString()));
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Boolean"))
                                            {
                                                memberValueArrayList.Add(Convert.ToBoolean(Convert.ToByte(value.Value.ToString())));
                                            }
                                            else
                                            {
                                                memberValueArrayList.Add(value.Value);
                                            }
                                        }

                                        else
                                        {
                                            memberValueArrayList.Add(value.Value);
                                        }
                                    }
                                    else if (item != null)
                                    {
                                        // sanity check for null memberElementType
                                        if (memberElementType == null)
                                        {
                                            // {memberType.Name} is null and this can't happen.
                                            throw new DeserializationException();
                                        }

                                        string memberElementPath = $"{rootPath}/{memberProperty.Name}/{memberElementType.Name}";

                                        var itemObj = PopulateObject(item, memberElementType, memberElementPath);

                                        memberValueArrayList.Add(itemObj);
                                    }
                                    else
                                    {
                                        // item is not a JToken or a JValue - this case is not handled
                                    }
                                }

                                // Fill targetArray with the memberValueArrayList
                                if (isArrayList)
                                {
                                    ArrayList targetArray = new();

                                    for (int i = 0; i < memberValueArrayList.Count; i++)
                                    {
                                        // Test if we have only 1 element and that the element is a Hashtable.
                                        // In this case, we'll make it more efficient and add it as an Hashtable.
                                        if ((memberValueArrayList[i].GetType() == typeof(ArrayList)) &&
                                            (((ArrayList)memberValueArrayList[i]).Count == 1) &&
                                            ((ArrayList)memberValueArrayList[i])[0].GetType() == typeof(Hashtable))
                                        {
                                            targetArray.Add(((ArrayList)memberValueArrayList[i])[0]);
                                        }
                                        else
                                        {
                                            targetArray.Add(memberValueArrayList[i]);
                                        }
                                    }

                                    // Populate rootInstance
                                    if (memberIsProperty)
                                    {
                                        memberPropSetMethod.Invoke(rootInstance, new object[] { targetArray });
                                    }
                                    else
                                    {
                                        memberFieldInfo.SetValue(rootInstance, targetArray);
                                    }
                                }
                                else
                                {
                                    // Create targetArray - an Array of memberElementType objects - targetArray will be copied to rootInstance - then rootInstance will be returned

                                    Array targetArray = Array.CreateInstance(memberElementType, memberValueArray.Length);

                                    if (targetArray == null)
                                    {
                                        // failed to create Array of type: {memberElementType}[]
                                        throw new DeserializationException();
                                    }

                                    memberValueArrayList.CopyTo(targetArray);

                                    // Populate rootInstance
                                    if (memberIsProperty)
                                    {
                                        memberPropSetMethod.Invoke(rootInstance, new object[] { targetArray });
                                    }
                                    else
                                    {
                                        memberFieldInfo.SetValue(rootInstance, targetArray);
                                    }
                                }
                            }
                        }

                        return rootInstance;
                    }
                }
                else if (rootToken is JsonArray rootArray)
                {
                    bool isArrayList = false;

                    if (rootElementType == null)
                    {
                        // check if this is an ArrayList
                        if (rootType.FullName == "System.Collections.ArrayList"
                            || rootType.BaseType.FullName == "System.ValueType"
                            || rootType.FullName == "System.String")
                        {
                            isArrayList = true;

                            rootElementType = rootType;
                        }
                        else
                        {
                            // For arrays, type: {rootType.Name} must have a valid element type
                            throw new DeserializationException();
                        }
                    }

                    // Create & populate rootArrayList with the items in rootToken - call PopulateObject if the item is more complicated than a JValue 

                    ArrayList rootArrayList = new();

                    if (isArrayList)
                    {
                        foreach (var item in rootArray.Items)
                        {
                            if (item is JsonValue value)
                            {
                                rootArrayList.Add(value.Value);
                            }
                            else if (item != null)
                            {
                                rootArrayList = PopulateArrayList(item);
                            }
                            else
                            {
                                throw new DeserializationException();
                            }
                        }

                        if ((rootType.BaseType.FullName == "System.ValueType"
                             || rootType.FullName == "System.String")
                            && rootArrayList.Count == 1)
                        {
                            // this is a case of deserialing a array with a single element,
                            // so just return the element
                            return rootArrayList[0];
                        }

                        return rootArrayList;
                    }
                    else
                    {
                        foreach (var item in rootArray.Items)
                        {
                            if (item is JsonValue value)
                            {
                                if (value.Value.GetType() != rootType.GetElementType())
                                {
                                    switch (rootType.GetElementType().Name)
                                    {
                                        case nameof(Int16):
                                            rootArrayList.Add(Convert.ToInt16(value.Value.ToString()));
                                            break;

                                        case nameof(UInt16):
                                            rootArrayList.Add(Convert.ToUInt16(value.Value.ToString()));
                                            break;

                                        case nameof(Int32):
                                            rootArrayList.Add(Convert.ToInt32(value.Value.ToString()));
                                            break;

                                        case nameof(UInt32):
                                            rootArrayList.Add(Convert.ToUInt32(value.Value.ToString()));
                                            break;

                                        case nameof(Int64):
                                            rootArrayList.Add(Convert.ToInt64(value.Value.ToString()));
                                            break;

                                        case nameof(UInt64):
                                            rootArrayList.Add(Convert.ToUInt64(value.Value.ToString()));
                                            break;

                                        case nameof(Byte):
                                            rootArrayList.Add(Convert.ToByte(value.Value.ToString()));
                                            break;

                                        case nameof(SByte):
                                            rootArrayList.Add(Convert.ToSByte(value.Value.ToString()));
                                            break;

                                        case nameof(Single):
                                            rootArrayList.Add(Convert.ToSingle(value.Value.ToString()));
                                            break;

                                        case nameof(Double):
                                            rootArrayList.Add(Convert.ToDouble(value.Value.ToString()));
                                            break;

                                        case nameof(Boolean):
                                            rootArrayList.Add(Convert.ToBoolean(Convert.ToByte(value.Value.ToString())));
                                            break;

                                        default:
                                            rootArrayList.Add(value.Value);
                                            break;
                                    }
                                }
                                else
                                {
                                    rootArrayList.Add(value.Value);
                                }
                            }
                            else
                            {
                                if (isArrayList)
                                {
                                    rootArrayList = PopulateArrayList(item);
                                }
                                else
                                {
                                    // Pass rootElementType and rootPath with rootElementType appended to PopulateObject for this item 
                                    string itemPath = rootPath;

                                    if (itemPath[itemPath.Length - 1] == '/')
                                    {
                                        // Don't need to add a slash before appending rootElementType
                                        itemPath += rootElementType.Name;
                                    }
                                    else
                                    {
                                        // Need to add a slash before appending rootElementType
                                        itemPath = itemPath + '/' + rootElementType.Name;
                                    }

                                    var itemObj = PopulateObject(item, rootElementType, itemPath);

                                    rootArrayList.Add(itemObj);
                                }
                            }
                        }

                        Array targetArray = Array.CreateInstance(rootType.GetElementType(), rootArray.Length);

                        if (targetArray == null)
                        {
                            //  CreateInstance() failed for type: {rootElementType.Name}    length: {rootArray.Length}
                            throw new DeserializationException();
                        }

                        rootArrayList.CopyTo(targetArray);

                        return targetArray;
                    }

                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static object PopulateObject(JsonToken rootToken)
        {
            if (rootToken == null)
            {
                // can't be null
                throw new DeserializationException();
            }

            try
            {
                if (rootToken is JsonObject rootObject)
                {
                    Hashtable rootInstance = new();

                    foreach (var m in rootObject.Members)
                    {
                        var memberProperty = (JsonProperty)m;

                        if (memberProperty.Value is JsonValue jsonValue)
                        {
                            rootInstance.Add(memberProperty.Name, jsonValue.Value);
                        }
                        else if (memberProperty.Value is JsonArray jsonArray)
                        {
                            rootInstance.Add(memberProperty.Name, PopulateArrayList(jsonArray));
                        }
                        else if (memberProperty.Value is JsonToken jsonToken)
                        {
                            rootInstance.Add(memberProperty.Name, PopulateHashtable(jsonToken));
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                    return rootInstance;
                }
                else if (rootToken is JsonValue rootValue)
                {
                    return rootValue.Value;
                }
                else
                {
                    // not implemented
                    throw new DeserializationException();
                }
            }
            catch
            {
                return null;
            }
        }

        private static ArrayList PopulateArrayList(JsonToken rootToken)
        {
            var result = new ArrayList();

            // Process all members for this rootObject
            if (rootToken is JsonObject rootObject)
            {
                Hashtable mainTable = new();

                foreach (var m in rootObject.Members)
                {
                    var memberProperty = (JsonProperty)m;

                    if (memberProperty == null)
                    {
                        throw new NotSupportedException();
                    }

                    // Process the member based on JObject, JValue, or JArray
                    if (memberProperty.Value is JsonObject)
                    {
                        throw new DeserializationException();
                    }
                    else if (memberProperty.Value is JsonValue value)
                    {
                        mainTable.Add(memberProperty.Name, value.Value);
                    }
                    else if (memberProperty.Value is JsonArray jsonArrayAttribute)
                    {
                        // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                        var memberValueArray = jsonArrayAttribute;

                        // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                        var memberValueArrayList = new ArrayList();

                        // Create a JToken[] array for Items associated for this memberProperty.Value
                        JsonToken[] memberItems = memberValueArray.Items;

                        foreach (JsonToken item in memberItems)
                        {
                            if (item is JsonValue jsonValue)
                            {
                                memberValueArrayList.Add((jsonValue).Value);
                            }
                            else if (item is JsonToken jsonToken)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                // item is not a JToken or a JValue - this case is not handled
                            }
                        }

                        // add to main table
                        mainTable.Add(memberProperty.Name, memberValueArrayList);
                    }
                }

                // add to result
                result.Add(mainTable);
            }
            else if (rootToken is JsonArray array)
            {
                // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                var memberValueArrayList = new ArrayList();

                // Create a JToken[] array for Items associated for this memberProperty.Value
                JsonToken[] memberItems = array.Items;

                foreach (JsonToken item in memberItems)
                {
                    if (item is JsonValue jsonValue)
                    {
                        memberValueArrayList.Add((jsonValue).Value);
                    }
                    else if (item is JsonToken jsonToken)
                    {
                        memberValueArrayList.Add(PopulateObject(jsonToken));
                    }
                    else
                    {
                        // item is not a JToken or a JValue - this case is not handled
                    }
                }

                result = memberValueArrayList;
            }
            else
            {
                throw new NotImplementedException();
            }

            return result;
        }

        private static Hashtable PopulateHashtable(JsonToken rootToken)
        {
            var result = new Hashtable();

            // Process all members for this rootObject

            if (rootToken is JsonObject rootTokenObjectAttribute)
            {
                foreach (var m in rootTokenObjectAttribute.Members)
                {
                    var memberProperty = (JsonProperty)m;

                    if (memberProperty == null)
                    {
                        throw new NotSupportedException();
                    }

                    // Process the member based on JObject, JValue, or JArray
                    if (memberProperty.Value is JsonObject memberPropertyValue)
                    {
                        // Call PopulateObject() for this member - i.e. recursion
                        result.Add(memberProperty.Name, PopulateHashtable(memberPropertyValue));
                    }
                    else if (memberProperty.Value is JsonValue memberPropertyJsonValue)
                    {
                        if (memberPropertyJsonValue is JsonValue jsonValue)
                        {
                            result.Add(memberProperty.Name, jsonValue.Value);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else if (memberProperty.Value is JsonArray jsonArrayAttribute)
                    {
                        // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                        var memberValueArray = jsonArrayAttribute;

                        // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                        var memberValueArrayList = new ArrayList();

                        // Create a JToken[] array for Items associated for this memberProperty.Value
                        JsonToken[] memberItems = memberValueArray.Items;

                        foreach (JsonToken item in memberItems)
                        {
                            if (item is JsonValue jsonValue)
                            {
                                memberValueArrayList.Add(jsonValue.Value);
                            }
                            else if (item is JsonToken jsonToken)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                // item is not a JToken or a JValue - this case is not handled
                            }
                        }

                        // add to main table
                        result.Add(memberProperty.Name, memberValueArrayList);
                    }
                }
            }
            else if (rootToken is JsonArray)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }

            return result;
        }


        // Trying to deserialize a stream in nanoFramework is problematic.
        // as Stream.Peek() has not been implemented in nanoFramework
        // Therefore, read all input into the static jsonBytes[] and use jsonPos to keep track of where we are when parsing the input
        private static byte[] jsonBytes;     // Do all deserialization using this byte[]
        private static int jsonPos;      // Current position in jsonBytes[]

        private static object Deserialize(string sourceString)
        {
            jsonBytes = new byte[sourceString.Length];
            jsonBytes = Encoding.UTF8.GetBytes(sourceString);
            jsonPos = 0;
            return Deserialize();
        }

        private static object Deserialize(Stream sourceStream)
        {
            // Read the sourcestream into jsonBytes[]
            jsonBytes = new byte[sourceStream.Length];
            sourceStream.Read(jsonBytes, 0, (int)sourceStream.Length);
            jsonPos = 0;
            return Deserialize();
        }

        // Deserialize() now assumes that the input has been copied int jsonBytes[]
        // Keep track of position with jsonPos
        private static JsonToken Deserialize()
        {
            LexToken token;
            token = GetNextToken();

            // Deserialize the json input data in jsonBytes[]
            JsonToken result;

            switch (token.TType)
            {
                case TokenType.LBrace:
                    result = ParseObject(ref token);

                    if (token.TType == TokenType.RBrace)
                    {
                        token = GetNextToken();
                    }
                    else if (token.TType != TokenType.End && token.TType != TokenType.Error)
                    {
                        // unexpected content after end of object
                        throw new DeserializationException();
                    }
                    break;

                case TokenType.LArray:
                    result = ParseArray(ref token);

                    if (token.TType == TokenType.RArray)
                    {
                        token = GetNextToken();
                    }
                    else if (token.TType != TokenType.End && token.TType != TokenType.Error)
                    {
                        // unexpected content after end of array
                        throw new DeserializationException();
                    }

                    break;

                default:
                    // unexpected initial token in json parse
                    throw new DeserializationException();
            }

            if (token.TType != TokenType.End)
            {
                // unexpected end token in json parse
                throw new DeserializationException();
            }
            else if (token.TType == TokenType.Error)
            {
                // unexpected lexical token during json parse
                throw new DeserializationException();
            }

            return result;
        }

        private static JsonObject ParseObject(ref LexToken token)
        {
            var result = new JsonObject();

            token = GetNextToken();

            while (token.TType is not TokenType.End and not TokenType.Error and not TokenType.RBrace)
            {
                // Get the name from the name:value pair
                if (token.TType != TokenType.String)
                {
                    // expected label
                    throw new DeserializationException();
                }

                var propName = token.TValue;
                // Look for the :
                token = GetNextToken();

                if (token.TType != TokenType.Colon)
                {
                    // expected colon
                    throw new DeserializationException();
                }

                // Get the value from the name:value pair
                var value = ParseValue(ref token);
                result.Add(propName, value);

                // Look for additional name:value pairs (i.e. separated by a comma)
                token = GetNextToken();

                if (token.TType == TokenType.Comma)
                {
                    token = GetNextToken();
                }

            }

            if (token.TType == TokenType.Error)
            {
                // unexpected token in json object
                throw new DeserializationException();
            }
            else if (token.TType != TokenType.RBrace)
            {
                // unterminated json object
                throw new DeserializationException();
            }

            return result;
        }

        private static JsonArray ParseArray(ref LexToken token)
        {
            ArrayList list = new();

            while (token.TType is not TokenType.End and not TokenType.Error and not TokenType.RArray)
            {
                var value = ParseValue(ref token);

                if (value != null)
                {
                    list.Add(value);

                    token = GetNextToken();

                    if (token.TType != TokenType.Comma && token.TType != TokenType.RArray)
                    {
                        // badly formed array
                        throw new DeserializationException();
                    }
                }
            }

            if (token.TType == TokenType.Error)
            {
                // unexpected token in array
                throw new DeserializationException();
            }
            else if (token.TType != TokenType.RArray)
            {
                // unterminated json array
                throw new DeserializationException();
            }

            var result = new JsonArray((JsonToken[])list.ToArray(typeof(JsonToken)));

            return result;
        }

        private static JsonToken ParseValue(ref LexToken token)
        {
            token = GetNextToken();

            if (token.TType == TokenType.RArray)
            {
                // we were expecting a value in an array, and came across the end-of-array marker,
                //  so this is an empty array
                return null;
            }
            else if (token.TType == TokenType.String)
            {
                return new JsonValue(token.TValue);
            }
            else if (token.TType == TokenType.Number)
            {
                if (token.TValue.IndexOfAny(new char[] { '.', 'f', 'F', 'd', 'D', 'e', 'E' }) != -1)
                {
                    return new JsonValue(double.Parse(token.TValue));
                }
                else
                {
                    // int.MaxValue:    2,147,483,647
                    // int.MinValue:   -2,147,483,648
                    // uint.MaxValue:   4,294,967,295
                    // long.MaxValue:   9,223,372,036,854,775,807
                    // long.MinValue:  -9,223,372,036,854,775,808
                    // If we are sure, don't go to the try catch
                    if (token.TValue.Length < 9)
                    {
                        return new JsonValue(int.Parse(token.TValue));
                    }
                    else if ((token.TValue.Length >= 12) && (token.TValue.Length < 20))
                    {
                        return new JsonValue(long.Parse(token.TValue));
                    }

                    try
                    {
                        return new JsonValue(int.Parse(token.TValue));
                    }
                    catch
                    {
                        try
                        {
                            return new JsonValue(long.Parse(token.TValue));
                        }
                        catch
                        {
                            return new JsonValue(ulong.Parse(token.TValue));
                        }
                    }
                }
            }
            else if (token.TType == TokenType.True)
            {
                return new JsonValue(true);
            }
            else if (token.TType == TokenType.False)
            {
                return new JsonValue(false);
            }
            else if (token.TType == TokenType.Null)
            {
                return new JsonValue(null);
            }
            else if (token.TType == TokenType.Date)
            {
                return new JsonValue(token.TValue, true);
            }
            else if (token.TType == TokenType.LBrace)
            {
                return ParseObject(ref token);
            }
            else if (token.TType == TokenType.LArray)
            {
                return ParseArray(ref token);
            }

            // invalid value found during json parse
            throw new DeserializationException();
        }

        private static LexToken GetNextToken()
        {
            var result = GetNextTokenInternal();

            return result;
        }

        private static LexToken GetNextTokenInternal()
        {
            StringBuilder sb = null;

            char openQuote = '\0';
            char ch;

            while (true)
            {
                if (jsonPos >= jsonBytes.Length)
                {
                    return EndToken(sb);
                }

                ch = (char)jsonBytes[jsonPos++];

                // Handle json escapes
                bool escaped = false;
                bool unicodeEncoded = false;

                if (ch == '\\')
                {
                    escaped = true;
                    ch = (char)jsonBytes[jsonPos++];

                    if (ch == (char)0xffff)
                    {
                        return EndToken(sb);
                    }

                    //TODO: replace with a mapping array? This switch is really incomplete.
                    switch (ch)
                    {
                        case 't':
                            ch = '\t';
                            break;

                        case 'r':
                            ch = '\r';
                            break;

                        case 'n':
                            ch = '\n';
                            break;

                        case 'u':
                            unicodeEncoded = true;
                            break;

                        default:
                            throw new DeserializationException();
                    }
                }

                if ((sb != null) && ((ch != openQuote) || (escaped)))
                {
                    if (unicodeEncoded)
                    {
                        int numberCounter = 0;

                        // next 4 chars have to be numeric
                        StringBuilder encodedValue = new();

                        // advance position to next char
                        jsonPos++;
                        ch = (char)jsonBytes[jsonPos];

                        for (int i = 0; i < 4; i++)
                        {
                            if (IsNumberChar(ch))
                            {
                                numberCounter++;

                                encodedValue.Append(ch);

                                ch = (char)jsonBytes[jsonPos];

                                if (IsNumberChar(ch))
                                {
                                    // We're still working on the number - advance jsonPos
                                    jsonPos++;
                                }
                            }
                        }

                        if (numberCounter == 4)
                        {
                            // we're good with the encoded data
                            // try parse number as an UTF-8 char
                            try
                            {
                                // NOTE: the encoded value has hexadecimal format
                                int unicodeChar = Convert.ToInt16(encodedValue.ToString(), 16);

                                _ = sb.Append((char)unicodeChar);
                            }
                            catch
                            {
                                // couldn't parse this number as a valid Unicode value
                                throw new DeserializationException();
                            }
                        }
                        else
                        {
                            // anything else, we can't parse it properly
                            // throw exception
                            throw new DeserializationException();
                        }
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                else if (IsNumberIntroChar(ch))
                {
                    sb = new StringBuilder();

                    while (IsNumberChar(ch))
                    {
                        sb.Append(ch);

                        // nanoFramework doesn't support Peek() for Streams or DataReaders
                        // This is why we converted everything to a byte[] instead of trying to work directly from a Stream or a DataReader
                        // Look at the next byte but don't advance jsonPos unless we're still working on the number
                        // i.e. 'peek' to see if we're at the end of the number
                        ch = (char)jsonBytes[jsonPos];

                        if (IsNumberChar(ch))
                        {
                            jsonPos++;                      // We're still working on the number - advance jsonPos
                        }

                        if (ch == (char)0xffff)
                        {
                            return EndToken(sb);
                        }
                    }

                    // Note that we don't claim that this is a well-formed number
                    return new LexToken() { TType = TokenType.Number, TValue = sb.ToString() };
                }
                else
                {
                    switch (ch)
                    {
                        case '{':
                            return new LexToken() { TType = TokenType.LBrace, TValue = null };

                        case '}':
                            return new LexToken() { TType = TokenType.RBrace, TValue = null };

                        case '[':
                            return new LexToken() { TType = TokenType.LArray, TValue = null };

                        case ']':
                            return new LexToken() { TType = TokenType.RArray, TValue = null };

                        case ':':
                            return new LexToken() { TType = TokenType.Colon, TValue = null };

                        case ',':
                            return new LexToken() { TType = TokenType.Comma, TValue = null };

                        case '"':
                        case '\'':
                            if (sb == null)
                            {
                                openQuote = ch;
                                sb = new StringBuilder();
                            }
                            else
                            {
                                // We're building a string and we hit a quote character.
                                // The ch must match openQuote, or otherwise we should have eaten it above as string content

                                var stringValue = sb.ToString();

                                if (DateTimeExtensions.ConvertFromString(stringValue, out _))
                                {
                                    return new LexToken() { TType = TokenType.Date, TValue = stringValue };
                                }

                                return new LexToken() { TType = TokenType.String, TValue = stringValue };
                            }
                            break;

                        case ' ':
                        case '\t':
                        case '\r':
                        case '\n':
                            break; // whitespace - go around again

                        case (char)0xffff:
                            return EndToken(sb);

                        default:
                            // try to collect a token
                            switch (ch.ToLower())
                            {
                                case 't':
                                    Expect('r');
                                    Expect('u');
                                    Expect('e');
                                    return new LexToken() { TType = TokenType.True, TValue = null };

                                case 'f':
                                    Expect('a');
                                    Expect('l');
                                    Expect('s');
                                    Expect('e');
                                    return new LexToken() { TType = TokenType.False, TValue = null };

                                case 'n':
                                    Expect('u');
                                    Expect('l');
                                    Expect('l');
                                    return new LexToken() { TType = TokenType.Null, TValue = null };

                                default:
                                    // unexpected character during json lexical parse
                                    throw new DeserializationException();
                            }
                    }
                }
            }
        }

        private static void Expect(char expected)
        {
            char ch = (char)jsonBytes[jsonPos++];

            if (ch.ToLower() != expected)
            {
                // unexpected character during json lexical parse
                throw new DeserializationException();
            }
        }

        private static LexToken EndToken(StringBuilder sb)
        {
            if (sb != null)
            {
                return new LexToken() { TType = TokenType.Error, TValue = null };
            }
            else
            {
                return new LexToken() { TType = TokenType.End, TValue = null };
            }
        }

        // Legal first characters for numbers
        private static bool IsNumberIntroChar(char ch) => (ch == '-') || (ch == '+') || (ch == '.') || (ch >= '0' && ch <= '9');

        // Legal chars for 2..n'th position of a number
        private static bool IsNumberChar(char ch) => (ch == '-') || (ch == '+') || (ch == '.') || (ch == 'e') || (ch == 'E') || (ch >= '0' && ch <= '9');
    }
}
