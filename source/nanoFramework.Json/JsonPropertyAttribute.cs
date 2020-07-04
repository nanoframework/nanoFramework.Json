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

		public string	 Name	{ get; set; }
		public JsonToken Value	{ get; set; }

		public override string ToString()
		{
			EnterSerialization();
			StringBuilder sb = new StringBuilder(); //TODO: why move out of the try?
			try
			{
				sb.Append('"');
				sb.Append(Name);
				sb.Append("\":"); //Use minimalist JSON, pretty can be handled on the client!
				JsonToken token = Value;
				if (token is JsonValue j)
				{	// Not all tokens are JValue - some are JObject or JArray
					if (j.Value != null)
					{
						if (j.Value.GetType().Name == "Boolean") //TODO: can this be done on convert rather than here??
						{
							// Convert Boolean values to lower case when appending to the JSON string
							// Lower case JSON convention is described here:  https://www.json.org/json-en.html
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
