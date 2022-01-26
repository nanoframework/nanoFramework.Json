//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json
{
    internal class JsonObject : JsonToken
    {
        private readonly Hashtable _members = new();

        public JsonProperty this[string name]
        {
            get { return (JsonProperty)_members[name.ToLower()]; }
            set
            {
                if (name.ToLower() != value.Name.ToLower())
                {
                    throw new ArgumentException("index value must match property name");
                }
                _members.Add(value.Name.ToLower(), value);
            }
        }

        public bool Contains(string name) => this._members.Contains(name.ToLower());

        public ICollection Members => _members.Values;

        public static object JsonObjectAttribute { get; private set; }

        public void Add(string name, JsonToken value)
        {
            _members.Add(name.ToLower(), new JsonProperty(name, value));
        }

        public static JsonObject Serialize(Type type, object oSource)
        {
            var result = new JsonObject();
            MethodInfo[] methods;

            if (type.FullName == "System.Collections.Hashtable")
            {
                return Serialize((Hashtable)oSource);
            }

            if (type.FullName == "System.Collections.ArrayList")
            {
                var arraySer = Serialize((ArrayList)oSource);
                result._members.Add(string.Empty, arraySer);
                return result;
            }

            // Loop through all of this type's methods - find a get_ method that can be used to serialize oSource
            methods = type.GetMethods();

            foreach (var m in methods)
            {
                // don't care about:
                // - non public methods
                // - use abstract methods
                if (!m.IsPublic
                    || m.IsAbstract)
                {
                    continue;
                }

                // Modified AS TINY CLR May Have issue with Getter for Chars & Length from String (see post forum)
                // Discard methods that start with 'get_'
                if (m.Name.IndexOf("get_") != 0)
                {
                    continue;
                }

                if (
                    m.Name == "get_Chars"
                    || m.Name == "get_Length"
                    || m.Name == "Empty"
                    || m.Name == "get_IsReadOnly"
                    || m.Name == "get_IsFixedSize"
                    || m.Name == "get_IsSynchronized"
                    || m.Name == "get_Item"
                    || m.Name == "get_Keys"
                    || m.Name == "get_Values"
                    || m.Name == "get_SyncRoot"
                    || m.Name == "get_Count"
                    || m.Name == "get_Capacity"
                    )
                {
                    continue;   // Not all 'get_' methods have what we're looking for
                }

                // take out the 'get_'
                var name = m.Name.Substring(4);

                var methodResult = m.Invoke(oSource, null);

                if (methodResult == null)
                {
                    result._members.Add(name, new JsonProperty(name, JsonValue.Serialize(m.ReturnType, null)));
                }
                else if (
                    m.ReturnType.IsValueType
                    || m.ReturnType == typeof(string))
                {
                    result._members.Add(name, new JsonProperty(name, JsonValue.Serialize(m.ReturnType, methodResult)));
                }
                else if (m.ReturnType.IsArray)
                {
                    result._members.Add(name, new JsonProperty(name, JsonArray.Serialize(m.ReturnType, methodResult)));
                }
                else if (m.ReturnType.FullName == "System.Collections.ArrayList")
                {
                    result._members.Add(name, new JsonProperty(name, Serialize((ArrayList)methodResult)));
                }
                else
                {
                    result._members.Add(name, new JsonProperty(name, Serialize(m.ReturnType, methodResult)));
                }
            }

            var fields = type.GetFields();
            foreach (var f in fields)
            {
                if (f.FieldType.IsNotPublic)
                {
                    continue;
                }

                switch (f.MemberType)
                {
                    case MemberTypes.Field:
                    case MemberTypes.Property:
                        var value = f.GetValue(oSource);

                        if (value == null)
                        {
                            result._members.Add(f.Name, new JsonProperty(f.Name, JsonValue.Serialize(f.FieldType, null)));
                        }
                        else if (f.FieldType.IsValueType || f.FieldType == typeof(string))
                        {
                            result._members.Add(f.Name.ToLower(), new JsonProperty(f.Name, JsonValue.Serialize(f.FieldType, value)));
                        }
                        else
                        {
                            if (f.FieldType.IsArray)
                            {
                                result._members.Add(f.Name.ToLower(), new JsonProperty(f.Name, JsonArray.Serialize(f.FieldType, value)));
                            }
                            else
                            {
                                result._members.Add(f.Name.ToLower(), new JsonProperty(f.Name, Serialize(f.FieldType, value)));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        private static JsonObject Serialize(Hashtable source)
        {
            JsonObject result = new();

            // index for items
            int index = 0;

            foreach (var key in source.Keys)
            {
                var value = source[key];

                if (value == null)
                {
                    result._members.Add(key.ToString(), new JsonProperty(key.ToString(), new JsonValue(null)));
                }
                else
                {
                    var valueType = value.GetType();

                    if (valueType == null)
                    {
                        //TODO: handle nulls
                        throw new DeserializationException();
                    }

                    if ((valueType.IsValueType) || (valueType == typeof(string)))
                    {
                        result._members.Add(key.ToString(), new JsonProperty(key.ToString(), JsonValue.Serialize(valueType, value)));
                    }
                    else if (valueType.IsArray)
                    {
                        result._members.Add(key.ToString(), JsonArray.Serialize(valueType, value));
                    }
                    else if (valueType.FullName == "System.Collections.ArrayList")
                    {
                        result._members.Add(key.ToString(), new JsonProperty(key.ToString(), JsonArray.Serialize(valueType, (ArrayList)value)));
                    }
                    else
                    {
                        result._members.Add(key.ToString(), Serialize(valueType, value));
                    }
                }

                index++;
            }

            return result;
        }

        public static JsonArray Serialize(ArrayList source)
        {
            JsonToken[] result = new JsonToken[source.Count];

            // index for items
            int index = 0;

            foreach (var item in source)
            {
                if (item == null)
                {
                    result[index] = new JsonValue(null);
                }
                else
                {
                    var valueType = item.GetType();

                    if (valueType == null)
                    {
                        //TODO: handle nulls
                        throw new DeserializationException();
                    }

                    if (valueType.IsValueType || (valueType == typeof(string)))
                    {
                        result[index] = JsonValue.Serialize(valueType, item);
                    }
                    else if (valueType.IsArray)
                    {
                        result[index] = JsonArray.Serialize(valueType, item);
                    }
                    else if (valueType.FullName == "System.Collections.ArrayList")
                    {
                        result[index] = JsonArray.Serialize(valueType, (ArrayList)item);
                    }
                    else
                    {
                        result[index] = Serialize(valueType, item);
                    }
                }

                index++;
            }

            return new JsonArray(result);
        }


        //Use minimalist JSON, pretty can be handled on the client!
        public override string ToString()
        {
            // set up a SerializationContext object and Lock it (via Monitor)
            EnterSerialization();

            try
            {
                StringBuilder sb = new();

                sb.Append("{");

                bool first = true;
                Type type;

                foreach (var key in _members.Keys)
                {
                    var member = _members[key];
                    if (!first)
                    {
                        sb.Append(",");
                    }

                    first = false;

                    type = member.GetType();
                    if (type == typeof(JsonProperty))
                    {
                        sb.Append(((JsonProperty)member).ToString());
                    }
                    else if (type == typeof(JsonObject))
                    {
                        sb.Append($"\"{key}\":{(JsonObject)member}");
                    }
                    else if (type == typeof(JsonArray))
                    {
                        sb.Append(((JsonArray)member).ToString());
                    }
                }

                sb.Append("}");

                return sb.ToString();
            }
            finally
            {
                ExitSerialization();    // Unlocks the SerializationContext object
            }
        }
    }
}
