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

        public int Length => Items.Length;

        public JsonToken[] Items { get; }

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
