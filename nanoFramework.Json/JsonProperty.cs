//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json
{
    internal class JsonProperty : JsonToken
    {
        public JsonProperty()
        {
        }

        public JsonProperty(string name, JsonToken value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public JsonToken Value { get; set; }
    }
}
