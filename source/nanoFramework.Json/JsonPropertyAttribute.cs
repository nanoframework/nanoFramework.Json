//
// Copyright (c) 2020 The nanoFramework project contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System.Text;

namespace nanoFramework.Json
{
	internal class JsonPropertyAttribute : JsonToken
	{
		public JsonPropertyAttribute()
		{
		}

		public JsonPropertyAttribute(string name, JsonToken value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; set; }
		public JsonToken Value { get; set; }

		public override string ToString()
		{
			EnterSerialization();
			StringBuilder sb = new StringBuilder(); //TODO: why move out of the try?
			try
			{
				sb.Append('"');
				sb.Append(Name);
				sb.Append("\":"); //Use minimalist JSON, pretty can be handled on the client!



				//// skigrinder - 
				//// Checked with Jose' - He said to stick with "True" and "False"
				//// Decided to convert to lower case anyway
				////	JSON strings with upper case fail when sent to a browser
				////	JSON strings with upper case also fail when sent to https://json2csharp.com or http://json.parser.online.fr/ 
				////
				//// Boolean types get serialized to "True" and "False".  
				//// But the convention for https://json2csharp.com is lower case (i.e. "true" and "false")
				//// The goal here is to create a JSON string that can be entered into https://json2csharp.com without errors
				// Convert Boolean values here to lower case when appending to the JSON string
				// Have to dig into this.Value to figure out if it's a Boolean
				JsonToken token = Value;
				if (token is JsonValue j)
				{      // Not all tokens are JValue - some are JObject or JArray
					if (j.Value != null)
					{
						if (j.Value.GetType().Name == "Boolean") //TODO: can this be done on convert rather than here??
						{
							sb.Append(Value.ToString().ToLower());
							return sb.ToString();
						}
					}
				}
				sb.Append(Value.ToString());
				return sb.ToString();
			}
			finally
			{
				ExitSerialization();
			}
		}
	}
}
