//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace nanoFramework.Json
{
    internal class JsonArray : JsonToken
    {
        public JsonArray()
        {
        }

        public JsonArray(JsonToken[] values)
        {
            Items = values;
        }

        // Made a lot of changes here to get Array serialization working
        private JsonArray(Array source)
        {
            Items = new JsonToken[source.Length];

            for (int i = 0; i < source.Length; ++i)
            {
                var value = source.GetValue(i);

                if (value == null)
                {
                    // TODO: handle nulls
                    throw new DeserializationException();
                }

                var valueType = value.GetType();

                if (valueType == null)
                {
                    //TODO: handle nulls
                    throw new DeserializationException();
                }

                if ((valueType.IsValueType) || (valueType == typeof(string)))
                {
                    Items[i] = JsonValue.Serialize(valueType, value);
                }
                else if (valueType.IsArray)
                {
                    Items[i] = Serialize(valueType, value);
                }
                else if (valueType.FullName == "System.Collections.Hashtable")
                {
                    Items[i] = JsonObject.Serialize(valueType, (Hashtable)value);
                }
                else
                {
                    Items[i] = JsonObject.Serialize(valueType, value);
                }
            }
        }

        private JsonArray(ArrayList source)
        {
            Items = new JsonToken[source.Count];

            // index for items
            int index = 0;

            foreach (var item in source)
            {
                if (item == null)
                {
                    Items[index++] = new JsonValue(null);

                    continue;
                }

                var valueType = item.GetType();

                if (valueType == null)
                {
                    //TODO: handle nulls
                    throw new DeserializationException();
                }

                if ((valueType.IsValueType) || (valueType == typeof(string)))
                {
                    Items[index++] = JsonValue.Serialize(valueType, item);
                }
                else if (valueType.IsArray)
                {
                    Items[index++] = Serialize(valueType, item);
                }
                else if (valueType.FullName == "System.Collections.ArrayList")
                {
                    Items[index++] = Serialize(valueType, (ArrayList)item);
                }
                else
                {
                    Items[index++] = JsonObject.Serialize(valueType, item);
                }
            }
        }

        public int Length => Items.Length;

        public JsonToken[] Items { get; }

        public static JsonArray Serialize(Type type, object oSource) => new((Array)oSource);

        public static JsonArray Serialize(Type type, ArrayList oSource) => new(oSource);

        public JsonToken this[int i] => Items[i];

        public override string ToString()
        {
            // set up a SerializationContext object and Lock it (via Monitor)
            EnterSerialization();

            try
            {
                StringBuilder sb = new();

                sb.Append('[');

                int prefaceLength = 0;
                bool first = true;

                foreach (var item in Items)
                {
                    if (!first)
                    {
                        if (sb.Length - prefaceLength > 72)
                        {
                            // Use minimalist JSON, pretty can be handled by the client!
                            sb.Append(",");

                            prefaceLength = sb.Length;
                        }
                        else
                        {
                            sb.Append(',');
                        }
                    }

                    first = false;

                    sb.Append(item);
                }

                sb.Append(']');

                return sb.ToString();
            }
            finally
            {
                ExitSerialization();    // Unlocks the SerializationContext object
            }
        }
    }
}
