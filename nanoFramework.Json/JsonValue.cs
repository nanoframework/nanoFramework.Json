//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json
{
    internal class JsonValue : JsonToken
    {
        public JsonValue()
        {
        }

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

        public object Value { get; set; }
    }
}
