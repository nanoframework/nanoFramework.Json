//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json
{
    /// <summary>
    /// Represents single JSON key value pair.
    /// </summary>
    public sealed class JsonProperty : JsonToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonProperty" /> class.
        /// </summary>
        public JsonProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonProperty" /> class.
        /// </summary>
        /// <param name="name">JSON property key.</param>
        /// <param name="value">JSON property value.</param>
        public JsonProperty(string name, JsonToken value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets key of JSON property.
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// Gets or sets value of Json property.
        /// </summary>
        public JsonToken Value { get; set; }
    }
}
