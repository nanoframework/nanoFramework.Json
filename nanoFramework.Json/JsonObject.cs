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
