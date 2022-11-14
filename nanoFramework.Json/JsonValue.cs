//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json
{
    /// <summary>
    /// Represents single object from JSON string.
    /// </summary>
    public sealed class JsonValue : JsonToken
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonValue" /> class.
        /// </summary>
        /// <param name="value">Value of current JSON object.</param>
        /// <param name="isDateTime">Is the type of value DateTime.</param>
        public JsonValue(object value, bool isDateTime = false)
        {
            if (isDateTime)
            {
                if (DateTimeExtensions.ConvertFromString((string)value, out DateTime dtValue))
                {
                    Value = dtValue;
                }
            }
            else
            {
                Value = value;
            }
        }

        /// <summary>
        /// Gets or sets object value.
        /// </summary>
        public object Value { get; }
    }
}
