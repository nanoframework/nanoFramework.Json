//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
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
        private const string RootPath = "/";

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

        /// <summary>
        /// Convert an object to a JSON string.
        /// </summary>
        /// <param name="value">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, DictionaryEntry, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only public properties with getters are converted.</remarks>
        public static string SerializeObject(object value)
        {
            return JsonSerializer.SerializeObject(value);
        }

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">The object type to convert to</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(string value, Type type) =>
            DeserializeObject(value, type, JsonSerializerOptions.Default);

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">The object type to convert to</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to be used during deserialization.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(string value, Type type, JsonSerializerOptions options)
        {
            if (type == typeof(string))
            {
                var converter = ConvertersMapping.GetConverter(type);
                return converter.ToType(value);
            }

            var token = Deserialize(value);
            return PopulateObject((JsonToken)token, type, "/", options);
        }

// TODO: Is this still required?
#if NANOFRAMEWORK_1_0

        /// <summary>
        /// Deserializes a JSON <see cref="Stream"/> into an object.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type">The object type to convert to</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(Stream stream, Type type) =>
            DeserializeObject(stream, type, JsonSerializerOptions.Default);

        /// <summary>
        /// Deserializes a JSON <see cref="Stream"/> into an object.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type">The object type to convert to</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to be used during deserialization.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(Stream stream, Type type, JsonSerializerOptions options)
        {
            var token = Deserialize(stream);
            return PopulateObject((JsonToken)token, type, "/", options);
        }

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="streamReader"></param>
        /// <param name="type">The object type to convert to</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(StreamReader streamReader, Type type) =>
            DeserializeObject(streamReader, type, JsonSerializerOptions.Default);

        /// <summary>
        /// Deserializes a Json string into an object.
        /// </summary>
        /// <param name="streamReader"></param>
        /// <param name="type">The object type to convert to</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to be used during deserialization.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(StreamReader streamReader, Type type, JsonSerializerOptions options)
        {
            var token = Deserialize(streamReader);
            return PopulateObject((JsonToken)token, type, "/", options);
        }

#endif

        private static bool ShouldSkipConvert(Type sourceType, Type targetType, bool forceConversion)
        {
            if (forceConversion)
            {
                return false;
            }

            return sourceType == targetType;
        }

        internal static object ConvertToType(Type sourceType, Type targetType, object value, bool forceConversion = false)
        {
            if (ShouldSkipConvert(sourceType, targetType, forceConversion))
            {
                return value;
            }

            if (targetType.IsArray)
            {
                return ConvertToType(sourceType, targetType.GetElementType(), value, forceConversion);
            }

            var converter = ConvertersMapping.GetConverter(targetType);
            return converter is not null ? converter.ToType(value) : value;
        }

        private static object PopulateObject(JsonArray jsonArray, Type type, string path, JsonSerializerOptions options)
        {
            if (jsonArray is null || type is null || path is null)
            {
                // All parameters must be non-null
                throw new DeserializationException();
            }

            var isArrayList = false;
            var elementType = type.GetElementType();

            if (elementType is null)
            {
                // check if this is an ArrayList
                if (type.FullName == "System.Collections.ArrayList" || type.BaseType?.FullName == "System.ValueType" || type.FullName == "System.String")
                {
                    isArrayList = true;
                    elementType = type;
                }
                else
                {
                    // Arrays must have a valid element type
                    throw new DeserializationException();
                }
            }

            var result = new ArrayList();

            if (isArrayList)
            {
                result = PopulateArrayList(jsonArray);

                if ((type.BaseType?.FullName == "System.ValueType" || type.FullName == "System.String") && result.Count == 1)
                {
                    // This is a case of deserializing an array with a single element. Just return the element.
                    return result[0];
                }

                return result;
            }

            foreach (var item in jsonArray.Items)
            {
                if (item is JsonValue jsonValue)
                {
                    if (jsonValue.Value is null)
                    {
                        result.Add(null);
                        continue;
                    }

                    var value = jsonValue.Value;
                    result.Add(ConvertToType(value.GetType(), elementType, value));
                }
                else
                {

                    // Pass rootElementType and path with rootElementType appended to PopulateObject for this item 
                    var itemPath = path;

                    if (itemPath[itemPath.Length - 1] == '/')
                    {
                        // Don't need to add a slash before appending rootElementType
                        itemPath += elementType.Name;
                    }
                    else
                    {
                        // Need to add a slash before appending rootElementType
                        itemPath = itemPath + '/' + elementType.Name;
                    }

                    result.Add(PopulateObject(item, elementType, itemPath, options));
                }
            }

            var targetArray = Array.CreateInstance(elementType, jsonArray.Length);

            if (targetArray is null)
            {
                throw new DeserializationException();
            }

            result.CopyTo(targetArray);

            return targetArray;
        }

        private static object PopulateObject(JsonObject rootObject, Type type, string path, JsonSerializerOptions options)
        {
            if (rootObject is null || type is null || path is null)
            {
                // All parameters must be non-null
                throw new DeserializationException();
            }

            var elementType = type.GetElementType();

            switch (elementType)
            {
                case null when type.FullName == "System.Collections.ArrayList":
                    return PopulateArrayList(rootObject);
                case null when type.FullName == "System.Collections.Hashtable":
                    return PopulateHashtable(rootObject);
            }

            var converter = ConvertersMapping.GetConverter(type);
            if (converter is not null)
            {
                return converter.ToType(rootObject);
            }

            // Empty array of Types - GetConstructor didn't work unless given an empty array of Type[]
            Type[] types = { };

            ConstructorInfo ci = type.GetConstructor(types);

            if (ci == null)
            {
                // failed to create target instance
                throw new DeserializationException();
            }

            // This is the object that gets populated and returned
            // Create rootInstance from the type's constructor
            var rootInstance = ci.Invoke(null);

            // If we haven't successfully created rootInstance, bail out
            if (rootInstance == null)
            {
                // failed to create target instance from type
                throw new DeserializationException();
            }

            if ((rootObject == null) || (rootObject.Members == null))
            {
                // failed to create target instance from type
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

                // Call current resolver to get info how to deal with data
                var memberResolver = options.Resolver.Get(memberPropertyName, type, options);
                if (memberResolver.Skip)
                {
                    continue;
                }

                // Process the member based on JObject, JValue, or JArray
                if (memberProperty.Value is JsonObject @object)
                {
                    // Call PopulateObject() for this member - i.e. recursion
                    var memberPath = path;

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
                    // whole if can be replaced with memberObject = PopulateObject(memberProperty.Value, memberType, memberPath);??
                    if (memberResolver.ObjectType.FullName == "System.Collections.Hashtable")
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
                        memberObject = PopulateObject(memberProperty.Value, memberResolver.ObjectType, memberPath, options);
                    }

                    memberResolver.SetValue(rootInstance, memberObject);
                }
                else if (memberProperty.Value is JsonValue memberPropertyValue)
                {
                    var returnType = memberPropertyValue.Value != null ? memberPropertyValue.Value.GetType() : memberResolver.ObjectType;
                    var convertedValueAsObject = ConvertToType(returnType, memberResolver.ObjectType, memberPropertyValue.Value, true);
                    memberResolver.SetValue(rootInstance, convertedValueAsObject);
                }
                else if (memberProperty.Value is JsonArray array)
                {
                    // Need this type when we try to populate the array elements
                    Type memberElementType = memberResolver.ObjectType.GetElementType();
                    bool isArrayList = false;

                    if (memberElementType == null && memberResolver.ObjectType.FullName == "System.Collections.ArrayList")
                    {
                        memberElementType = memberResolver.ObjectType;
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
                            var valueToAddAsObject = ConvertToType(value.Value.GetType(), memberResolver.ObjectType, value.Value);
                            memberValueArrayList.Add(valueToAddAsObject);
                        }
                        else if (item != null)
                        {
                            // sanity check for null memberElementType
                            if (memberElementType == null)
                            {
                                // {memberType.Name} is null and this can't happen.
                                throw new DeserializationException();
                            }

                            string memberElementPath = $"{path}/{memberProperty.Name}/{memberElementType.Name}";

                            var itemObj = PopulateObject(item, memberElementType, memberElementPath, options);

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
                        memberResolver.SetValue(rootInstance, targetArray);
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
                        memberResolver.SetValue(rootInstance, targetArray);
                    }
                }
            }

            return rootInstance;

        }

        private static object PopulateObject(JsonToken jsonToken, Type type, string path, JsonSerializerOptions options)
        {
            if (jsonToken is null || type is null || path is null)
            {
                // All parameters must be non-null
                throw new DeserializationException();
            }

            // TODO: Is there a reason JsonValue isn't handled here?
            return jsonToken switch
            {
                JsonArray jsonArray => PopulateObject(jsonArray, type, path, options),
                JsonObject jsonObject => PopulateObject(jsonObject, type, path, options),
                _ => null
            };
        }

        // TODO: Rename to GetTokenValue to be more clear? (PopulateArrayList and PopulateDictionary would be similar)
        private static object PopulateObject(JsonToken jsonToken)
        {
            return jsonToken switch
            {
                JsonArray rootArray => PopulateArrayList(rootArray),
                JsonObject rootObject => PopulateHashtable(rootObject),
                JsonValue rootValue => rootValue.Value,
                _ => throw new DeserializationException()
            };
        }

        private static ArrayList PopulateArrayList(JsonArray jsonArray)
        {
            var result = new ArrayList();
            var jsonTokens = jsonArray.Items;

            foreach (var jsonToken in jsonTokens)
            {
                result.Add(PopulateObject(jsonToken));
            }

            return result;
        }

        private static ArrayList PopulateArrayList(JsonToken rootToken)
        {
            switch (rootToken)
            {
                case JsonArray jsonArray:
                    return PopulateArrayList(jsonArray);
                case JsonObject jsonObject:
                    return new ArrayList { PopulateHashtable(jsonObject) };
                default:
                    throw new NotImplementedException();
            }
        }

        private static Hashtable PopulateHashtable(JsonObject jsonObject)
        {
            var result = new Hashtable();
            var members = jsonObject.Members;

            foreach (var member in members)
            {
                if (member is not JsonProperty jsonProperty)
                {
                    throw new DeserializationException();
                }

                result.Add(jsonProperty.Name, PopulateObject(jsonProperty.Value));
            }

            return result;
        }


        // Trying to deserialize a stream in nanoFramework is problematic.
        // as Stream.Peek() has not been implemented in nanoFramework
        // Therefore, read all input into the static jsonBytes[] and use jsonPos to keep track of where we are when parsing the input
        private static object Deserialize(string sourceString)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(sourceString);
            var jsonPos = 0;
            return Deserialize(ref jsonPos, ref jsonBytes);
        }

        private static object Deserialize(Stream sourceStream)
        {
            // Read the source stream into jsonBytes[]
            var jsonBytes = new byte[sourceStream.Length];
            sourceStream.Read(jsonBytes, 0, (int)sourceStream.Length);
            var jsonPos = 0;
            return Deserialize(ref jsonPos, ref jsonBytes);
        }

        // Deserialize() now assumes that the input has been copied into jsonBytes[]
        // Keep track of position with jsonPos
        private static JsonToken Deserialize(ref int jsonPos, ref byte[] jsonBytes)
        {
            LexToken token = GetNextToken(ref jsonPos, ref jsonBytes);

            // Deserialize the json input data in jsonBytes[]
            JsonToken result;

            switch (token.TType)
            {
                case TokenType.LBrace:
                    result = ParseObject(ref jsonPos, ref jsonBytes, ref token);

                    if (token.TType == TokenType.RBrace)
                    {
                        token = GetNextToken(ref jsonPos, ref jsonBytes);
                    }
                    else if (token.TType != TokenType.End && token.TType != TokenType.Error)
                    {
                        // unexpected content after end of object
                        throw new DeserializationException();
                    }
                    break;

                case TokenType.LArray:
                    result = ParseArray(ref jsonPos, ref jsonBytes, ref token);

                    if (token.TType == TokenType.RArray)
                    {
                        token = GetNextToken(ref jsonPos, ref jsonBytes);
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

            if (token.TType == TokenType.Error)
            {
                // unexpected lexical token during json parse
                throw new DeserializationException();
            }

            return result;
        }

        private static object Deserialize(StreamReader dr)
        {
            // Read the DataReader into jsonBytes[]
            var jsonBytes = new byte[dr.BaseStream.Length];
            var jsonPos = 0;

            while (!dr.EndOfStream)
            {
                jsonBytes[jsonPos++] = (byte)dr.Read();
            }

            jsonPos = 0;

            return Deserialize(ref jsonPos, ref jsonBytes);
        }

        // ReSharper disable once RedundantAssignment
        private static JsonObject ParseObject(ref int jsonPos, ref byte[] jsonBytes, ref LexToken token)
        {
            var result = new JsonObject();

            token = GetNextToken(ref jsonPos, ref jsonBytes);

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
                token = GetNextToken(ref jsonPos, ref jsonBytes);

                if (token.TType != TokenType.Colon)
                {
                    // expected colon
                    throw new DeserializationException();
                }

                // Get the value from the name:value pair
                var value = ParseValue(ref jsonPos, ref jsonBytes, ref token);
                result.Add(propName, value);

                // Look for additional name:value pairs (i.e. separated by a comma)
                token = GetNextToken(ref jsonPos, ref jsonBytes);

                if (token.TType == TokenType.Comma)
                {
                    token = GetNextToken(ref jsonPos, ref jsonBytes);
                }

            }

            if (token.TType == TokenType.Error)
            {
                // unexpected token in json object
                throw new DeserializationException();
            }

            if (token.TType != TokenType.RBrace)
            {
                // unterminated json object
                throw new DeserializationException();
            }

            return result;
        }

        private static JsonArray ParseArray(ref int jsonPos, ref byte[] jsonBytes, ref LexToken token)
        {
            ArrayList list = new();

            while (token.TType is not TokenType.End and not TokenType.Error and not TokenType.RArray)
            {
                var value = ParseValue(ref jsonPos, ref jsonBytes, ref token);

                if (value == null)
                {
                    continue;
                }

                list.Add(value);

                token = GetNextToken(ref jsonPos, ref jsonBytes);

                if (token.TType != TokenType.Comma && token.TType != TokenType.RArray)
                {
                    // badly formed array
                    throw new DeserializationException();
                }
            }

            if (token.TType == TokenType.Error)
            {
                // unexpected token in array
                throw new DeserializationException();
            }

            if (token.TType != TokenType.RArray)
            {
                // unterminated json array
                throw new DeserializationException();
            }

            return new JsonArray((JsonToken[])list.ToArray(typeof(JsonToken)));
        }

        private static JsonToken ParseValue(ref int jsonPos, ref byte[] jsonBytes, ref LexToken token)
        {
            token = GetNextToken(ref jsonPos, ref jsonBytes);

            if (token.TType == TokenType.RArray)
            {
                // we were expecting a value in an array, and came across the end-of-array marker,
                //  so this is an empty array
                return null;
            }

            if (token.TType == TokenType.String)
            {
                return new JsonValue(token.TValue);
            }

            if (token.TType == TokenType.Number)
            {
                if (token.TValue.IndexOfAny(new char[] { '.', 'f', 'F', 'd', 'D', 'e', 'E' }) != -1)
                {
                    return new JsonValue(double.Parse(token.TValue));
                }

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

                if ((token.TValue.Length >= 12) && (token.TValue.Length < 20))
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

            if (token.TType == TokenType.True)
            {
                return new JsonValue(true);
            }

            if (token.TType == TokenType.False)
            {
                return new JsonValue(false);
            }

            if (token.TType == TokenType.Null)
            {
                return new JsonValue(null);
            }

            if (token.TType == TokenType.Date)
            {
                return new JsonValue(token.TValue, true);
            }

            if (token.TType == TokenType.LBrace)
            {
                return ParseObject(ref jsonPos, ref jsonBytes, ref token);
            }

            if (token.TType == TokenType.LArray)
            {
                return ParseArray(ref jsonPos, ref jsonBytes, ref token);
            }

            // invalid value found during json parse
            throw new DeserializationException();
        }

        private static LexToken GetNextToken(ref int jsonPos, ref byte[] jsonBytes)
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

#pragma warning disable S1121
#pragma warning disable S3358
                ch = (jsonBytes[jsonPos] & 0x80) == 0 ? (char)jsonBytes[jsonPos++]
                    : (jsonBytes[jsonPos] & 0x20) == 0 ? Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 2) - 2, 2)[0]
                    : (jsonBytes[jsonPos] & 0x10) == 0 ? Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 3) - 3, 3)[0]
                    : Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 4) - 4, 4)[0];
#pragma warning restore S1121
#pragma warning restore S3358

                // Handle json escapes
                bool escaped = false;
                bool unicodeEncoded = false;

                if (ch == '\\')
                {
                    escaped = true;
#pragma warning disable S1121
#pragma warning disable S3358
                    ch = (jsonBytes[jsonPos] & 0x80) == 0 ? (char)jsonBytes[jsonPos++]
                        : (jsonBytes[jsonPos] & 0x20) == 0 ? Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 2) - 2, 2)[0]
                        : (jsonBytes[jsonPos] & 0x10) == 0 ? Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 3) - 3, 3)[0]
                        : Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 4) - 4, 4)[0];
#pragma warning restore S1121
#pragma warning restore S3358
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

                        case '"':
                            ch = '"';
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
                                    Expect('r', ref jsonPos, ref jsonBytes);
                                    Expect('u', ref jsonPos, ref jsonBytes);
                                    Expect('e', ref jsonPos, ref jsonBytes);
                                    return new LexToken() { TType = TokenType.True, TValue = null };

                                case 'f':
                                    Expect('a', ref jsonPos, ref jsonBytes);
                                    Expect('l', ref jsonPos, ref jsonBytes);
                                    Expect('s', ref jsonPos, ref jsonBytes);
                                    Expect('e', ref jsonPos, ref jsonBytes);
                                    return new LexToken() { TType = TokenType.False, TValue = null };

                                case 'n':
                                    Expect('u', ref jsonPos, ref jsonBytes);
                                    Expect('l', ref jsonPos, ref jsonBytes);
                                    Expect('l', ref jsonPos, ref jsonBytes);
                                    return new LexToken() { TType = TokenType.Null, TValue = null };

                                default:
                                    // unexpected character during json lexical parse
                                    throw new DeserializationException();
                            }
                    }
                }
            }
        }

        private static void Expect(char expected, ref int jsonPos, ref byte[] jsonBytes)
        {
#pragma warning disable S1121
#pragma warning disable S3358
            char ch = (jsonBytes[jsonPos] & 0x80) == 0 ? (char)jsonBytes[jsonPos++]
                : (jsonBytes[jsonPos] & 0x20) == 0 ? Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 2) - 2, 2)[0]
                : (jsonBytes[jsonPos] & 0x10) == 0 ? Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 3) - 3, 3)[0]
                : Encoding.UTF8.GetChars(jsonBytes, (jsonPos += 4) - 4, 4)[0];
#pragma warning restore S1121
#pragma warning restore S3358
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

            return new LexToken() { TType = TokenType.End, TValue = null };
        }

        // Legal chars for 2…nth position of a number
        private static bool IsNumberChar(char ch) => ch is '-' or '+' or '.' or 'e' or 'E' or >= '0' and <= '9';

        // Legal first characters for numbers
        private static bool IsNumberIntroChar(char ch) => ch is '-' or '+' or '.' or >= '0' and <= '9';
    }
}
