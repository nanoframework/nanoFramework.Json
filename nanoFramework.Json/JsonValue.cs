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
				DateTime dtValue = DateTime.MaxValue;
				
				// check for special case of "null" date
				if ((string)value == "0001-01-01T00:00:00Z")
				{
					dtValue = DateTime.MinValue;
				}

				if (dtValue == DateTime.MaxValue)
				{
					try
					{
						dtValue = DateTimeExtensions.FromIso8601((string)value);
					}
					catch
					{
						// intended, to catch failed conversion attempt
					}
				}

				if (dtValue == DateTime.MaxValue)
				{
					try
					{
						dtValue = DateTimeExtensions.FromASPNetAjax((string)value);
					}
					catch
					{
						// intended, to catch failed conversion attempt
					}
				}

				if (dtValue == DateTime.MaxValue)
				{
					try
					{
						dtValue = DateTimeExtensions.FromiCalendar((string)value);
					}
					catch
					{
						// intended, to catch failed conversion attempt
					}
				}

				if (dtValue != DateTime.MaxValue)
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

				if (type == typeof(string) || type == typeof(char))
				{
					return "\"" + this.Value.ToString() + "\"";
				}
				else if (type == typeof(DateTime))
				{
					return "\"" + DateTimeExtensions.ToIso8601(((DateTime)Value)) + "\"";
				}
				else if(type == typeof(Boolean))
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
