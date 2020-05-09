//
// Copyright (c) 2020 The nanoFramework project contributors
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

		public JsonValue(object value)
		{
			Value = value;
		}

		public object Value { get; set; }

		public static JsonValue Serialize(Type type, object oValue)
		{
			if (type.Name == "Single")
			{
				if (float.IsNaN((float)oValue))
				{
					return new JsonValue() { Value = null }; //the other option would be to return a string of "NaN"
				}
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
				if (type == typeof(string) || type == typeof(char))
				{
					return "\"" + this.Value.ToString() + "\"";
				}
				else if (type == typeof(DateTime))
				{
					return "\"" + DateTimeExtensions.ToIso8601(((DateTime)Value)) + "\"";
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
