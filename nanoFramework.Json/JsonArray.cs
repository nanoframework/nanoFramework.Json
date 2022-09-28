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
    }
}
