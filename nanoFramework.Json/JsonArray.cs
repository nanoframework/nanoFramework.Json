//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json
{
    /// <summary>
    /// Represents array of objects from JSON string.
    /// </summary>
    public sealed class JsonArray : JsonToken
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray" /> class.
        /// </summary>
        public JsonArray()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray" /> class.
        /// </summary>
        /// <param name="values">Initial <see cref="JsonToken"/> array.</param>
        public JsonArray(JsonToken[] values)
        {
            Items = values;
        }
        
        /// <summary>
        /// Gets the lenght of <see cref="Items"/> array.
        /// </summary>
        public int Length => Items.Length;

        /// <summary>
        /// Gets collection of <see cref="JsonToken"/>.
        /// </summary>
        public JsonToken[] Items { get; }
    }
}
