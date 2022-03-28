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

        public static JsonValue Serialize(Type type, object oValue)
        {
            if (type.Name == "Single" && float.IsNaN((float)oValue))
            {
                //Unfortunately JSON does not understand "float.NaN". This is the next best option!
                return new JsonValue() { Value = null };
            }
            else if (type.Name == "Double" && double.IsNaN((double)oValue))
            {
                //Unfortunately JSON does not understand "float.NaN". This is the next best option!
                return new JsonValue() { Value = null };
            }

            return new JsonValue() { Value = oValue };
        }

        public override string ToString()
        {
            EnterSerialization();

            try
            {
                if (Value == null)
                {
                    return "null";
                }

                var type = Value.GetType();

                if (type == typeof(string)
                    || type == typeof(char)
                    || type == typeof(TimeSpan))
                {
                    return "\"" + Value.ToString() + "\"";
                }
                else if (type == typeof(DateTime))
                {
                    return "\"" + DateTimeExtensions.ToIso8601(((DateTime)Value)) + "\"";
                }
                else if (type == typeof(bool))
                {
                    // need to convert Boolean values to lower case 
                    return Value.ToString().ToLower();
                }
                else
                {
                    return Value.ToString();
                }
            }
            finally
            {
                ExitSerialization();
            }
        }
    }
}
