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
using System.Diagnostics;
#if NANOFRAMEWORK_1_0
using Windows.Storage.Streams;
#endif

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
        internal static object SyncObj = new object();


        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="oSource">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, DictionaryEntry, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only public properties with getters are converted.</remarks>
        public static string SerializeObject(object oSource)
        {
            Debug.WriteLine($"Serialize(object oSource) - oSource.GetType(): {oSource.GetType().Name}  oSource.ToString(): {oSource.ToString()}");
            
            var type = oSource.GetType();
            
            if (type.IsArray)
            {
                JsonToken retToken = JsonArrayAttribute.Serialize(type, oSource);
                
                Debug.WriteLine($"Serialize(object oSource) - finished after calling JArray.Serialize() ");
                
                return retToken.ToString();
            }
            else
            {
                JsonToken retToken;

                if (type.FullName == "System.Collections.ArrayList")
                {
                    retToken = JsonObjectAttribute.Serialize((ArrayList)oSource);
                }
                else
                {
                    retToken = JsonObjectAttribute.Serialize(type, oSource);
                }

                Debug.WriteLine($"Serialize(object oSource) - finished after calling JObject.Serialize() ");

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
        public static object DeserializeObject(DataReader dr, Type type)
        {
            var dserResult = Deserialize(dr);
            return PopulateObject((JsonToken)dserResult, type, "/");
        }

        private static object Deserialize(DataReader dr)
        {
            // Read the DataReader into jsonBytes[]
            jsonBytes = new byte[dr.UnconsumedBufferLength];
            jsonPos = 0;
            while (dr.UnconsumedBufferLength > 0)
            {
                jsonBytes[jsonPos++] = dr.ReadByte();
            }
            jsonPos = 0;
            return Deserialize();
        }

#endif

        private static string debugIndent = $"PopulateObject() - ";      // PopulateObject() goes recursive - this cleans up the debug output
        private static int debugOutdent = 10;

        private static object PopulateObject(JsonToken rootToken, Type rootType, string rootPath)
        {
            debugIndent = "          " + debugIndent;

            // Simple message to make sure we get to this point - be careful with this - displaying null values causes the device to hang & makes debugging problematic
            Debug.WriteLine($"{debugIndent} Start - ");        

            if (
                (rootToken == null) 
                || (rootType == null) 
                || (rootPath == null))
            {
                throw new Exception($"PopulateObject() - All parameters must be non-null.  rootToken: {(rootToken != null ? rootToken.GetType().Name : "null")}   rootType: {(rootType != null ? rootType.Name : "null")}   rootPath: {(rootPath != null ? rootPath : "null")}");
            }

            try
            {
                // Leave the debug here in case future testing reveals trouble - maybe get rid of it at some point
                Debug.WriteLine($"{debugIndent} rootToken is a {rootToken.GetType().Name}  rootType: {rootType.Name}   rootPath: {rootPath}");
               
                Type rootElementType = rootType.GetElementType();

                if (rootToken is JsonObjectAttribute rootObject)
                {
                    bool isHashtable = false;

                    if (rootElementType == null)
                    {
                        // check if this is an Hashtable
                        if (rootType.FullName == "System.Collections.Hashtable")
                        {
                            isHashtable = true;

                            rootElementType = rootType;

                            Debug.WriteLine($"{debugIndent} rootType: {rootType.Name}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"{debugIndent} rootType: {rootType.Name}  rootType.GetElementType(): {rootType.GetElementType().Name}");
                    }

                    if (isHashtable)
                    {
                        Hashtable rootInstance = new Hashtable();

                        foreach (var m in rootObject.Members)
                        {
                            var memberProperty = (JsonPropertyAttribute)m;

                            if (memberProperty.Value is JsonValue jsonValue)
                            {
                                rootInstance.Add(memberProperty.Name, jsonValue.Value);
                            }
                            else if (memberProperty.Value is JsonArrayAttribute jsonArray)
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

                        Debug.WriteLine($"{debugIndent} created Hashtable by calling CreateInstance() with rootType.GetElementType()");

                        // 'Outdent' before returning
                        debugIndent = debugIndent.Substring(debugOutdent);

                        return rootInstance;
                    }
                    else
                    {
                        // This is the object that gets populated and returned
                        // Create rootInstance from the rootType's constructor
                        object rootInstance = null;

                        Debug.WriteLine($"{debugIndent} create rootInstance from rootType: {rootType.Name} using GetConstructor() & Invoke()");

                        // Empty array of Types - GetConstructor didn't work unless given an empty array of Type[]
                        Type[] types = { };

                        ConstructorInfo ci = rootType.GetConstructor(types);

                        if (ci == null)
                        {
                            throw new Exception($"PopulateObject() - failed to create target instance.   rootType: {rootType.Name} ");
                        }

                        rootInstance = ci.Invoke(null);

                        Debug.WriteLine($"{debugIndent} rootInstance created.  rootInstance.GetType(): {(rootInstance?.GetType()?.Name != null ? rootInstance.GetType().Name : "null")}");

                        // If we haven't successfully created rootInstance, bail out
                        if (rootInstance == null)
                        {
                            throw new Exception($"PopulateObject() - failed to create target instance from rootType: {rootType.Name} ");
                        }

                        if ((rootObject == null) || (rootObject.Members == null))
                        {
                            throw new Exception($"PopulateObject() - failed to create target instance from rootType: {rootType.Name} ");
                        }

                        // Process all members for this rootObject
                        Debug.WriteLine($"{debugIndent} Entering rootObject.Members loop ");

                        foreach (var m in rootObject.Members)
                        {
                            Debug.WriteLine($"{debugIndent} Process rootObject.Member");

                            var memberProperty = (JsonPropertyAttribute)m;

                            Debug.WriteLine($"{debugIndent}     memberProperty.Name:  {memberProperty.Name ?? "null"} ");

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
                                    Debug.WriteLine($"PopulateObject() - failed to create memberType.  {rootType.Name}.GetMethod() is null. Possibly this property doesn't exist.");

                                    continue;
                                }
                                else
                                {
                                    memberType = memberPropGetMethod.ReturnType;
                                    memberPropSetMethod = rootType.GetMethod("set_" + memberPropertyName);

                                    if (memberType == null)
                                    {
                                        throw new Exception($"PopulateObject() - failed to get setter of memberType {rootType.Name}. Possibly this property doesn't have a setter.");
                                    }

                                    memberIsProperty = true;

                                    Debug.WriteLine($"{debugIndent}     memberType:  {memberType.Name} ");
                                    Debug.WriteLine($"{debugIndent}     memberPropGetMethod.Name:  {memberPropertyName}  memberPropGetMethod.ReturnType:  {memberPropGetMethod.ReturnType.Name}");
                                }
                            }
                            // Process the member based on JObject, JValue, or JArray
                            if (memberProperty.Value is JsonObjectAttribute)
                            {
                                // Call PopulateObject() for this member - i.e. recursion
                                Debug.WriteLine($"{debugIndent}     memberProperty.Value is JObject");

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
                                    Hashtable table = new Hashtable();

                                    foreach (JsonPropertyAttribute v in ((JsonObjectAttribute)memberProperty.Value).Members)
                                    {
                                        if (v.Value is JsonValue jsonValue)
                                        {
                                            table.Add(v.Name, (jsonValue).Value);
                                        }
                                        else if (v.Value is JsonObjectAttribute jsonObjectAttribute)
                                        {
                                            table.Add(v.Name, PopulateHashtable(jsonObjectAttribute));
                                        }
                                        else if (v.Value is JsonArrayAttribute jsonArrayAttribute)
                                        {
                                            throw new NotImplementedException();
                                        }
                                    }

                                    memberObject = table;

                                    Debug.WriteLine($"{debugIndent}     populated the {memberPropertyName} Hashtable");
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

                                Debug.WriteLine($"{debugIndent}     successfully initialized member {memberPropertyName} to memberObject");
                            }
                            else if (memberProperty.Value is JsonValue)
                            {
                                // Don't need any more info - populate the member using memberSetMethod.Invoke()
                                Debug.WriteLine($"{debugIndent}     memberProperty.Value is JValue");

                                if (memberType != typeof(DateTime))
                                {
                                    Debug.WriteLine($"{debugIndent}     attempting to set rootInstance by invoking this member's set method for properties  or  SetValue() for fields");

                                    if (((JsonValue)memberProperty.Value).Value == null)
                                    {
                                        Debug.WriteLine($"{debugIndent}     memberProperty.Value is null");

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
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Single.NaN });
                                                        break;

                                                    case "Double":
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Double.NaN });
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
                                        Debug.WriteLine($"{debugIndent}     successfully initialized member {memberPropertyName}  to  null");
                                    }
                                    else
                                    {
                                        if (memberIsProperty)
                                        {
                                            JsonValue val = (JsonValue)memberProperty.Value;

                                            Debug.WriteLine($"{debugIndent}     setting value with memberPropSetMethod: {memberPropSetMethod.Name}   Declaring Type: {memberPropSetMethod.DeclaringType}  Value: {((JsonValue)memberProperty.Value).Value}");

                                            Debug.WriteLine($"{debugIndent}     memberProperty.Value.Value.Type: {val.Value.GetType().Name}  memberProperty.Value.Value: {val.Value}");

                                            if (val.Value.GetType() != memberType)
                                            {
                                                Debug.WriteLine($"{debugIndent}     need to change memberProperty.Value.Value.Type to {memberType} to match memberPropGetMethod.ReturnType - why are these are different?!?");

                                                switch (memberType.Name)
                                                {
                                                    case nameof(Int16):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToInt16(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Byte):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToByte(val.Value.ToString()) });
                                                        break;

                                                    case nameof(Single):
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { Convert.ToSingle(val.Value.ToString()) });
                                                        break;

                                                    default:
                                                        memberPropSetMethod.Invoke(rootInstance, new object[] { ((JsonValue)memberProperty.Value).Value });
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                memberPropSetMethod.Invoke(rootInstance, new object[] { ((JsonValue)memberProperty.Value).Value });
                                            }
                                        }
                                        else
                                        {
                                            memberFieldInfo.SetValue(rootInstance, ((JsonValue)memberProperty.Value).Value);
                                        }

                                        Debug.WriteLine($"{debugIndent}     successfully initialized member {memberPropertyName}  to  {((JsonValue)memberProperty.Value).Value} ");
                                    }
                                }
                                else
                                {
                                    if (memberIsProperty)
                                    {
                                        memberPropSetMethod.Invoke(rootInstance, new object[] { ((JsonValue)memberProperty.Value).Value });
                                    }
                                    else
                                    {
                                        memberFieldInfo.SetValue(rootInstance, ((JsonValue)memberProperty.Value).Value);
                                    }

                                    Debug.WriteLine($"{debugIndent}     successfully initialized member {memberPropertyName}  to  {(JsonValue)memberProperty.Value} ");
                                }
                            }
                            else if (memberProperty.Value is JsonArrayAttribute)
                            {
                                Debug.WriteLine($"{debugIndent}     memberProperty.Value is a JArray");

                                // Need this type when we try to populate the array elements
                                Type memberElementType = memberType.GetElementType();
                                bool isArrayList = false;

                                if (memberElementType == null && memberType.FullName == "System.Collections.ArrayList")
                                {
                                    memberElementType = memberType;

                                    isArrayList = true;
                                }

                                // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                                var memberValueArray = (JsonArrayAttribute)memberProperty.Value;

                                // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                                var memberValueArrayList = new ArrayList();

                                // Create a JToken[] array for Items associated for this memberProperty.Value
                                JsonToken[] memberItems = memberValueArray.Items;

                                Debug.WriteLine($"{debugIndent}       copy {memberItems.Length} memberItems from memberValueArray into memberValueArrayList - call PopulateObject() for items that aren't JValue");

                                foreach (JsonToken item in memberItems)
                                {
                                    if (item is JsonValue)
                                    {
                                        if (memberPropGetMethod == null)
                                        {
                                            Debug.WriteLine($"{debugIndent}         memberPropGetMethod is null - item is a JsonValue: {((JsonValue)item).Value}  type: {((JsonValue)item).Value.GetType().Name} - THIS SHOULD NEVER HAPPEN ********************");
                                            throw new NotSupportedException($"PopulateObject() - {rootType.Name} must have a valid Property Get Method");
                                        }
                                        Debug.WriteLine($"{debugIndent}         item is a JsonValue: {((JsonValue)item).Value}  type: {((JsonValue)item).Value.GetType().Name}- add it to memberValueArrayList");
                                        if (((JsonValue)item).Value.GetType() != memberPropGetMethod.ReturnType)
                                        {
                                            Debug.WriteLine($"{debugIndent}         need to change item.Value.Type to {memberPropGetMethod.ReturnType} to match memberPropGetMethod.ReturnType - why are these are different?!?");
                                            if (memberPropGetMethod.ReturnType.Name.Contains("Int16"))
                                            {
                                                memberValueArrayList.Add(Convert.ToInt16(((JsonValue)item).Value.ToString()));
                                                Debug.WriteLine($"{debugIndent}         item is a JsonValue - converted to Int16 & added to memberValueArrayList");
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Byte"))
                                            {
                                                memberValueArrayList.Add(Convert.ToByte(((JsonValue)item).Value.ToString()));
                                                Debug.WriteLine($"{debugIndent}         item is a JsonValue - converted to byte & added to memberValueArrayList");
                                            }
                                            else if (memberPropGetMethod.ReturnType.Name.Contains("Single"))
                                            {
                                                memberValueArrayList.Add(Convert.ToSingle(((JsonValue)item).Value.ToString()));
                                                Debug.WriteLine($"{debugIndent}         item is a JsonValue - converted to byte & added to memberValueArrayList");
                                            }
                                            else
                                            {
                                                memberValueArrayList.Add(((JsonValue)item).Value);
                                                Debug.WriteLine($"{debugIndent}         item is a JsonValue - added to memberValueArrayList");
                                            }
                                        }
                                        else
                                        {
                                            memberValueArrayList.Add(((JsonValue)item).Value);
                                            Debug.WriteLine($"{debugIndent}         item is a JsonValue - added to memberValueArrayList");
                                        }
                                    }
                                    else if (item is JsonToken)
                                    {
                                        // sanity check for null memberElementType
                                        if (memberElementType == null)
                                        {
                                            Debug.WriteLine($"{debugIndent}         memberElementType is null - THIS SHOULD NEVER HAPPEN ********************");
                                            throw new NotSupportedException($"PopulateObject() - memberElementType for memberType: {memberType.Name} is null and this can't happen.");
                                        }

                                        // Since memberProperty.Value is a JsonnArray:
                                        // 		memberType        is the array   type (i.e. foobar[])
                                        // 		memberElementType is the element type (i.e. foobar)		- use this to call PopulateObject()

                                        string memberElementPath = rootPath + "/" + memberProperty.Name + "/" + memberElementType.Name;

                                        Debug.WriteLine($"{debugIndent}         memberType: {memberType.Name}   memberElementType: {memberElementType.Name} ");
                                        Debug.WriteLine($"{debugIndent}         calling PopulateObject(JsonToken item, {memberElementType.Name}, {memberElementPath}) ");

                                        var itemObj = PopulateObject(item, memberElementType, memberElementPath);

                                        Debug.WriteLine($"{debugIndent}         item is a JsonToken - add it to memberValueArrayList");

                                        memberValueArrayList.Add(itemObj);

                                        Debug.WriteLine($"{debugIndent}         item is a JsonToken - added to memberValueArrayList");
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"{debugIndent}         item is not a JToken or a JValue - this case is not handled");
                                    }
                                }

                                Debug.WriteLine($"{debugIndent}       {memberItems.Length} memberValueArray.Items copied into memberValueArrayList - i.e. contents of memberProperty.Value");

                                // Fill targetArray with the memberValueArrayList
                                if (isArrayList)
                                {
                                    ArrayList targetArray = new ArrayList();

                                    for (int i = 0; i < memberValueArrayList.Count; i++)
                                    {
                                        targetArray.Add(memberValueArrayList[i]);
                                    }

                                    Debug.WriteLine($"{debugIndent}       copied memberValueArrayList into the targetArray");

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
                                    Debug.WriteLine($"{debugIndent}       create targetArray - an Array of memberElementType: {memberElementType} objects - use Array.CreateInstance({memberElementType}, {memberValueArray.Length}");

                                    Array targetArray = Array.CreateInstance(memberElementType, memberValueArray.Length);

                                    if (targetArray == null)
                                    {
                                        throw new Exception("PopulateObject() - failed to create Array of type: {memberElementType}[]");
                                    }

                                    Debug.WriteLine($"{debugIndent}       targetArray created using CreateInstance().  targetArray.GetType().Name: {(targetArray?.GetType()?.Name != null ? targetArray.GetType().Name : "null")}");

                                    memberValueArrayList.CopyTo(targetArray);

                                    Debug.WriteLine($"{debugIndent}       copied memberValueArrayList into the targetArray");

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

                                Debug.WriteLine($"{debugIndent}       populated the rootInstance object with the contents of targetArray");
                            }
                        }

                        debugIndent = debugIndent.Substring(debugOutdent);     // 'Outdent' before returning

                        Debug.WriteLine($"{debugIndent} Returning rootInstance");

                        return rootInstance;
                    }
                }
                else if (rootToken is JsonArrayAttribute rootArray)
                {
                    bool isArrayList = false;

                    if (rootElementType == null)
                    {
                        // check if this is an ArrayList
                        if (rootType.FullName == "System.Collections.ArrayList")
                        {
                            isArrayList = true;

                            rootElementType = rootType;

                            Debug.WriteLine($"{debugIndent} rootType: {rootType.Name}");
                        }
                        else
                        {
                            throw new NotSupportedException($"PopulateObject() - For arrays, type: {rootType.Name} must have a valid element type");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"{debugIndent} rootType: {rootType.Name}  rootType.GetElementType(): {rootType.GetElementType().Name}");
                    }
                   
                    // Create & populate rootArrayList with the items in rootToken - call PopulateObject if the item is more complicated than a JValue 
                    Debug.WriteLine($"{debugIndent} Create and populate rootArrayList with the items in rootToken - call PopulateObject if the item is more complicated than a JValue");
                    
                    ArrayList rootArrayList = new ArrayList();
                    
                    if (isArrayList)
                    {
                        foreach (var item in rootArray.Items)
                        {
                            if (item is JsonValue)
                            {
                                rootArrayList.Add(((JsonValue)item).Value);
                            }
                            else if (item is JsonToken)
                            {
                                rootArrayList = PopulateArrayList(item);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                           
                        Debug.WriteLine($"{debugIndent} created Array targetArray by calling CreateInstance() with rootType.GetElementType()");

                        // 'Outdent' before returning
                        debugIndent = debugIndent.Substring(debugOutdent);

                        return rootArrayList;
                    }
                    else
                    {
                        foreach (var item in rootArray.Items)
                        {
                            if (item is JsonValue)
                            {
                                Debug.WriteLine($"{debugIndent} item.Type is JsonValue  -  item.Value type: {((JsonValue)item).Value.GetType().Name}.   Adding it to rootArrayList");

                                if (((JsonValue)item).Value.GetType() != rootType.GetElementType())
                                {
                                    Debug.WriteLine($"{debugIndent}     need to change item.Value.Type to {rootType.GetElementType()} to match rootType.GetElementType() - why are these are different?!?");

                                    switch (rootType.GetElementType().Name)
                                    {
                                        case nameof(Int16):
                                            rootArrayList.Add(Convert.ToInt16(((JsonValue)item).Value.ToString()));

                                            Debug.WriteLine($"{debugIndent}         item.Type is a JsonValue - Converted to Int16 & added to rootArrayList");

                                            break;

                                        case nameof(Byte):
                                            rootArrayList.Add(Convert.ToByte(((JsonValue)item).Value.ToString()));

                                            Debug.WriteLine($"{debugIndent}         item.Type is a JsonValue - Converted to Byte & added to rootArrayList");

                                            break;

                                        default:
                                            rootArrayList.Add(((JsonValue)item).Value);

                                            Debug.WriteLine($"{debugIndent}         item.Type is a JsonValue - added to rootArrayList");

                                            break;
                                    }
                                }
                                else
                                {
                                    rootArrayList.Add(((JsonValue)item).Value);

                                    Debug.WriteLine($"{debugIndent}         item.Type is a JsonValue - added to rootArrayList");
                                }
                            }
                            else
                            {
                                if (isArrayList)
                                {
                                    Debug.WriteLine($"{debugIndent} Call PopulateObject() for ArrayList");

                                    rootArrayList = PopulateArrayList(item);

                                    Debug.WriteLine($"{debugIndent} added object returned from PopulateArrayList() to rootArrayList");
                                }
                                else
                                {
                                    Debug.WriteLine($"{debugIndent} item.Type is {item.GetType().Name} - use rootElementType to call PopulateObject()");

                                    // Pass rootElementType and rootPath with rootElementType appended to PopulateObject for this item 
                                    string itemPath = rootPath;

                                    if (itemPath[itemPath.Length - 1] == '/')
                                    {
                                        itemPath += rootElementType.Name;                   // Don't need to add a slash before appending rootElementType
                                    }
                                    else
                                    {
                                        itemPath = itemPath + '/' + rootElementType.Name;   // Need to add a slash before appending rootElementType
                                    }

                                    var itemObj = PopulateObject(item, rootElementType, itemPath);

                                    rootArrayList.Add(itemObj);

                                    Debug.WriteLine($"{debugIndent} added object of type: {itemObj.GetType().Name} (returned from PopulateObject()) to rootArrayList");
                                }
                            }
                        }

                        Debug.WriteLine($"{debugIndent} finished creating rootArrayList - copy rootArrayList to targetArray");

                        Array targetArray = Array.CreateInstance(rootType.GetElementType(), rootArray.Length);

                        if (targetArray == null)
                        {
                            throw new Exception($"PopulateObject() - CreateInstance() failed for type: {rootElementType.Name}    length: {rootArray.Length}");
                        }

                        Debug.WriteLine($"{debugIndent} created Array targetArray by calling CreateInstance() with rootType.GetElementType()");
                        Debug.WriteLine($"{debugIndent} copying rootArrayList to targetArray.   rootArrayList.Type: {rootArrayList.GetType().Name}  targetArray.Type {targetArray.GetType().Name}");

                        rootArrayList.CopyTo(targetArray);

                        Debug.WriteLine($"{debugIndent} populated targetArray with the contents of rootArrayList");

                        // 'Outdent' before returning
                        debugIndent = debugIndent.Substring(debugOutdent);

                        return targetArray;
                    }

                }   // end of  (if rootToken is JArray)

                // 'Outdent' before returning
                debugIndent = debugIndent.Substring(debugOutdent);

                return null;
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"{debugIndent} Exception: {ex.Message}");

                // 'Outdent' before returning
                debugIndent = debugIndent.Substring(debugOutdent);
                
                return null;
            }
        }

        private static ArrayList PopulateArrayList(JsonToken rootToken)
        {
            var result = new ArrayList();

            // Process all members for this rootObject
            Debug.WriteLine($"{debugIndent} Entering rootObject.Members loop ");

            if (rootToken is JsonObjectAttribute)
            {
                var rootObject = (JsonObjectAttribute)rootToken;

                var mainTable = new Hashtable();

                foreach (var m in rootObject.Members)
                {
                    Debug.WriteLine($"{debugIndent} Process rootObject.Member");

                    var memberProperty = (JsonPropertyAttribute)m;

                    if(memberProperty == null)
                    {
                        Debug.WriteLine($"memberProperty is null and can't be");

                        throw new NotSupportedException();
                    }

                    Debug.WriteLine($"{debugIndent}     memberProperty.Name:  {memberProperty?.Name ?? "null"} ");

                    // Process the member based on JObject, JValue, or JArray
                    if (memberProperty.Value is JsonObjectAttribute)
                    {
                        // Call PopulateObject() for this member - i.e. recursion
                        Debug.WriteLine($"{debugIndent}     memberProperty.Value is JObject");

                        throw new NotImplementedException();
                    }
                    else if (memberProperty.Value is JsonValue)
                    {
                        // Don't need any more info 
                        Debug.WriteLine($"{debugIndent}     memberProperty.Value is JValue");

                        mainTable.Add(memberProperty.Name, ((JsonValue)memberProperty.Value).Value);
                    }
                    else if (memberProperty.Value is JsonArrayAttribute jsonArrayAttribute)
                    {
                        Debug.WriteLine($"{debugIndent}     memberProperty.Value is a JArray");

                        // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                        var memberValueArray = jsonArrayAttribute;

                        // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                        var memberValueArrayList = new ArrayList();

                        // Create a JToken[] array for Items associated for this memberProperty.Value
                        JsonToken[] memberItems = memberValueArray.Items;

                        //Debug.WriteLine($"{debugIndent}       copy {memberItems.Length} memberItems from memberValueArray into memberValueArrayList - call PopulateObject() for items that aren't JValue");

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
                                Debug.WriteLine($"{debugIndent}         item is not a JToken or a JValue - this case is not handled");
                            }
                        }

                        Debug.WriteLine($"{debugIndent}       {memberItems.Length} memberValueArray.Items copied into memberValueArrayList - i.e. contents of memberProperty.Value");

                        // add to main table
                        mainTable.Add(memberProperty.Name, memberValueArrayList);

                        Debug.WriteLine($"{debugIndent}       populated the rootInstance object with the contents of targetArray");
                    }
                }

                // add to result
                result.Add(mainTable);
            }
            else if (rootToken is JsonArrayAttribute)
            {
                Debug.WriteLine($"{debugIndent}     memberProperty.Value is a JArray");

                // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                var memberValueArray = (JsonArrayAttribute)rootToken;

                // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                var memberValueArrayList = new ArrayList();

                // Create a JToken[] array for Items associated for this memberProperty.Value
                JsonToken[] memberItems = memberValueArray.Items;

                //Debug.WriteLine($"{debugIndent}       copy {memberItems.Length} memberItems from memberValueArray into memberValueArrayList - call PopulateObject() for items that aren't JValue");

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
                        Debug.WriteLine($"{debugIndent}         item is not a JToken or a JValue - this case is not handled");
                    }
                }

                Debug.WriteLine($"{debugIndent}       {memberItems.Length} memberValueArray.Items copied into memberValueArrayList - i.e. contents of memberProperty.Value");

                result = memberValueArrayList;

                Debug.WriteLine($"{debugIndent}       populated the rootInstance object with the contents of targetArray");
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
            Debug.WriteLine($"{debugIndent} Entering rootObject.Members loop ");

            if (rootToken is JsonObjectAttribute rootTokenObjectAttribute)
            {
                foreach (var m in rootTokenObjectAttribute.Members)
                {
                    Debug.WriteLine($"{debugIndent} Process rootObject.Member");

                    var memberProperty = (JsonPropertyAttribute)m;

                    if (memberProperty == null)
                    {
                        Debug.WriteLine($"memberProperty is null and can't be");

                        throw new NotSupportedException();
                    }

                    Debug.WriteLine($"{debugIndent}     memberProperty.Name:  {memberProperty?.Name ?? "null"} ");

                    // Process the member based on JObject, JValue, or JArray
                    if (memberProperty.Value is JsonObjectAttribute memberPropertyValue)
                    {
                        // Call PopulateObject() for this member - i.e. recursion
                        Debug.WriteLine($"{debugIndent}     memberProperty.Value is JObject");

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
                    else if (memberProperty.Value is JsonArrayAttribute jsonArrayAttribute)
                    {
                        Debug.WriteLine($"{debugIndent}     memberProperty.Value is a JArray");

                        // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
                        var memberValueArray = jsonArrayAttribute;

                        // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
                        var memberValueArrayList = new ArrayList();

                        // Create a JToken[] array for Items associated for this memberProperty.Value
                        JsonToken[] memberItems = memberValueArray.Items;

                        //Debug.WriteLine($"{debugIndent}       copy {memberItems.Length} memberItems from memberValueArray into memberValueArrayList - call PopulateObject() for items that aren't JValue");

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
                                Debug.WriteLine($"{debugIndent}         item is not a JToken or a JValue - this case is not handled");
                            }
                        }

                        Debug.WriteLine($"{debugIndent}       {memberItems.Length} memberValueArray.Items copied into memberValueArrayList - i.e. contents of memberProperty.Value");

                        // add to main table
                        result.Add(memberProperty.Name, memberValueArrayList);

                        Debug.WriteLine($"{debugIndent}       populated the rootInstance object with the contents of targetArray");
                    }
                }
            }
            else if (rootToken is JsonArrayAttribute)
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
            //DisplayDebug($"Deserialize() - jsonPos: {jsonPos}   jsonBytes.Length: {jsonBytes.Length}");
            JsonToken result;
            //DisplayDebug($"Deserialize() - JsonConverter - Deserialize() - GetNextToken() returned:  token.TType: {token.TType.TokenTypeToString()}  token.TValue: {token.TValue}");

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
                        throw new Exception("unexpected content after end of object");
                    }
                    break;
                case TokenType.LArray:
                    result = ParseArray(ref token);

                    if (token.TType == TokenType.RArray)
                    {
                        token = GetNextToken();
                    }
                    else if (token.TType != TokenType.End && token.TType != TokenType.Error)
                    {        // MORT clean this up
                        throw new Exception("unexpected content after end of array");
                    }

                    break;

                default:
                    throw new Exception("unexpected initial token in json parse");
            }
            if (token.TType != TokenType.End)
            {
                throw new Exception("unexpected end token in json parse");
            }
            else if (token.TType == TokenType.Error)
            {
                throw new Exception("unexpected lexical token during json parse");
            }
            return result;
        }

        private static JsonObjectAttribute ParseObject(ref LexToken token)
        {
            var result = new JsonObjectAttribute();

            token = GetNextToken();

            while (token.TType != TokenType.End && token.TType != TokenType.Error && token.TType != TokenType.RBrace)
            {
                // Get the name from the name:value pair
                if (token.TType != TokenType.String)
                {
                    throw new Exception("expected label");
                }

                var propName = token.TValue;
                // Look for the :
                token = GetNextToken();

                if (token.TType != TokenType.Colon)
                {
                    throw new Exception("expected colon");
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

            } //TODO!!! Comma removed?

            if (token.TType == TokenType.Error)
            {
                throw new Exception("unexpected token in json object");
            }
            else if (token.TType != TokenType.RBrace)
            {
                throw new Exception("unterminated json object");
            }
            return result;
        }

        private static JsonArrayAttribute ParseArray(ref LexToken token)
        {
            ArrayList list = new ArrayList();
            
            while (token.TType != TokenType.End && token.TType != TokenType.Error && token.TType != TokenType.RArray)
            {
                var value = ParseValue(ref token);

                if (value != null)
                {
                    list.Add(value);

                    token = GetNextToken();

                    if (token.TType != TokenType.Comma && token.TType != TokenType.RArray)
                    {
                        throw new Exception("badly formed array");
                    }
                }
            } //TODO!!! comma removed?

            if (token.TType == TokenType.Error)
            {
                throw new Exception("unexpected token in array");
            }
            else if (token.TType != TokenType.RArray)
            {
                throw new Exception("unterminated json array");
            }

            var result = new JsonArrayAttribute((JsonToken[])list.ToArray(typeof(JsonToken)));

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
                    return new JsonValue(int.Parse(token.TValue));
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

            throw new Exception("invalid value found during json parse");
        }

        private static LexToken GetNextToken()
        {
            var result = GetNextTokenInternal();

            return result;
        }


        private static LexToken GetNextTokenInternal()
        {
            try
            {
                StringBuilder sb = null;

                char openQuote = '\0';
                char ch = ' ';

                while (true)
                {
                    if (jsonPos >= jsonBytes.Length)
                    {
                        Debug.WriteLine($"GetNextTokenInternal() - no more data - call EndToken()");
                        return EndToken(sb);
                    }

                    ch = (char)jsonBytes[jsonPos++];

                    // Handle json escapes
                    bool escaped = false;
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
                            case '\'':
                                ch = '\'';
                                break;
                            case '"':
                                ch = '"';
                                break;
                            case 't':
                                ch = '\t';
                                break;
                            case 'r':
                                ch = '\r';
                                break;
                            case 'n':
                                ch = '\n';
                                break;
                            default:
                                throw new Exception("unsupported escape");
                        }
                    }

                    if ((sb != null) && ((ch != openQuote) || (escaped)))
                    {
                        sb.Append(ch);
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
                                    //Debug.Assert(ch == openQuote);

                                    var stringValue = sb.ToString();
                                    DateTime dtValue = DateTime.MaxValue;

                                    // check if this could be a DateTime value
                                    // min lenght is 18 for Java format: "Date(628318530718)": 18

                                    if (stringValue.Length >= 18)
                                    {
                                        // check for special case of "null" date
                                        if(stringValue == "0001-01-01T00:00:00Z")
                                        {
                                            dtValue = DateTime.MinValue;
                                        }

                                        if (dtValue == DateTime.MaxValue)
                                        {
                                            try
                                            {
                                                dtValue = DateTimeExtensions.FromIso8601(stringValue);
                                            }
                                            catch
                                            {
                                                // intended, to catch failed conversion attempt
                                            }
                                        }

                                        if (dtValue == DateTime.MaxValue)
                                        {
                                            try
                                            {
                                                dtValue = DateTimeExtensions.FromASPNetAjax(stringValue);
                                            }
                                            catch
                                            {
                                                // intended, to catch failed conversion attempt
                                            }
                                        }

                                        if (dtValue == DateTime.MaxValue)
                                        {
                                            try
                                            {
                                                dtValue = DateTimeExtensions.FromiCalendar(stringValue);
                                            }
                                            catch
                                            {
                                                // intended, to catch failed conversion attempt
                                            }
                                        }

                                        if (dtValue != DateTime.MaxValue)
                                        {
                                            return new LexToken() { TType = TokenType.Date, TValue = stringValue };
                                        }
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
                                        throw new Exception("unexpected character during json lexical parse");
                                }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"GetNextTokenInternal() - Exception caught");
                Debug.WriteLine($"GetNextTokenInternal() - Exception: {e.Message}");
                Debug.WriteLine($"GetNextTokenInternal() - StackTrace: {e.StackTrace.ToString()}");
                throw new Exception("something bad happened");
            }
        }

        private static void Expect(char expected)
        {
            char ch = (char)jsonBytes[jsonPos++];
            if (ch.ToLower() != expected)
            {
                throw new Exception("unexpected character during json lexical parse");
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
        private static bool IsNumberIntroChar(char ch)
        {
            return (ch == '-') || (ch == '+') || (ch == '.') || (ch >= '0' & ch <= '9');
        }

        // Legal chars for 2..n'th position of a number
        private static bool IsNumberChar(char ch)
        {
            return (ch == '-') || (ch == '+') || (ch == '.') || (ch == 'e') || (ch == 'E') || (ch >= '0' & ch <= '9');
        }
    }
}
