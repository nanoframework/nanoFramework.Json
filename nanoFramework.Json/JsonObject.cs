//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System.Collections;

namespace nanoFramework.Json
{
    /// <summary>
    /// Represents single JSON object that contains multiple JSON properties.
    /// </summary>
    public sealed class JsonObject : JsonToken
    {
        private readonly Hashtable _members = new();

        /// <summary>
        /// Gets the collection of values from <see cref="_members"/>.
        /// </summary>
        public ICollection Members => _members.Values;

        /// <summary>
        /// Adds new key value pair to collection.
        /// </summary>
        /// <param name="name">JSON property key.</param>
        /// <param name="value">JSON property value.</param>
        public void Add(string name, JsonToken value)
        {
            _members.Add(name, new JsonProperty(name, value));
        }

        /// <summary>
        /// Gets the value of property for given key.
        /// </summary>
        /// <param name="name">JSON property key.</param>
        /// <returns>JsonProperty object which contains key and value of object.</returns>
        public JsonProperty Get(string name)
        {
            return (JsonProperty)_members[name];
        }
    }
}
