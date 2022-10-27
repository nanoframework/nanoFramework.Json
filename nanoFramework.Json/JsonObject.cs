//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System.Collections;

namespace nanoFramework.Json
{
    internal class JsonObject : JsonToken
    {
        private readonly Hashtable _members = new();

        public ICollection Members => _members.Values;

        public void Add(string name, JsonToken value)
        {
            _members.Add(name.ToLower(), new JsonProperty(name, value));
        }
    }
}
