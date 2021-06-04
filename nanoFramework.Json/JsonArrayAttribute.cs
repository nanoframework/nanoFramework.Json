//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace nanoFramework.Json
{
	internal class JsonArrayAttribute : JsonToken
	{
		public JsonArrayAttribute()
		{
		}

		public JsonArrayAttribute(JsonToken[] values)
		{
			Items = values;
		}

		// Made a lot of changes here to get Array serialization working
		private JsonArrayAttribute(Array source)
		{
			Debug.WriteLine($"JArray(Array source) - Start - source type: {source.GetType().Name}  length: {source.Length}  value: {source.GetValue(0)}");
			
			Items = new JsonToken[source.Length];
			
			for (int i = 0; i < source.Length; ++i)
			{
				Debug.WriteLine($"JArray(Array source) - _contents loop - i: {i}");
				
				var value = source.GetValue(i);
				
				if (value == null)
				{
					// TODO: handle nulls
					throw new Exception($"JArray(Array source) - source.GetValue() returned null");
				}

				var valueType = value.GetType();

				if (valueType == null)
				{
					//TODO: handle nulls
					throw new Exception($"JArray(Array source) - value.GetType() returned null");
				}

				Debug.WriteLine($"JArray(Array source) - valueType: {valueType.Name} ");
				
				if ((valueType.IsValueType) || (valueType == typeof(string)))
				{
					Debug.WriteLine($"JArray(Array source) - valueType is ValueType or string - calling JValue.Serialize(valueType, value)");
					
					Items[i] = JsonValue.Serialize(valueType, value);
				}
				else if (valueType.IsArray)
				{
					Debug.WriteLine($"JArray(Array source) - valueType is Array - calling JArray.Serialize(valueType, value)");
					
					Items[i] = JsonArrayAttribute.Serialize(valueType, value);
				}
                else if (valueType.FullName == "System.Collections.Hashtable")
                {
                    Debug.WriteLine($"JArray(Array source) - valueType is Hashtable - calling JArray.Serialize(valueType, value)");

                    Items[i] = JsonObjectAttribute.Serialize(valueType, (Hashtable)value);
                }
                else
				{
					Debug.WriteLine($"JArray(Array source) - valueType is not Array and not ValueType or string - calling JObject.Serialize(valueType, value)");
					
					Items[i] = JsonObjectAttribute.Serialize(valueType, value); ;
				}
			}

			Debug.WriteLine($"JArray(Array source) - Finished");
		}

		private JsonArrayAttribute(ArrayList source)
		{
			Items = new JsonToken[source.Count];

			// index for items
			int index = 0;

			foreach (var item in source)
			{
				if (item == null)
				{
					Debug.WriteLine($"JArray(ArrayList source) - value is null");

					Items[index++] = new JsonValue(null);

					continue;
				}
	
				var valueType = item.GetType();

                if (valueType == null)
                {
                    //TODO: handle nulls
                    throw new Exception($"JArray(ArrayList source) - value.GetType() returned null");
                }

                Debug.WriteLine($"JArray(ArrayList source) - valueType: {valueType.Name} ");

                if ((valueType.IsValueType) || (valueType == typeof(string)))
                {
                    Debug.WriteLine($"JArray(ArrayList source) - valueType is ValueType or string - calling JValue.Serialize(valueType, value)");

                    Items[index++] = JsonValue.Serialize(valueType, item);
                }
                else if (valueType.IsArray)
                {
                    Debug.WriteLine($"JArray(ArrayList source) - valueType is Array - calling JArray.Serialize(valueType, value)");

                    Items[index++] = JsonArrayAttribute.Serialize(valueType, item);
                }
				else if (valueType.FullName == "System.Collections.ArrayList")
				{
					Debug.WriteLine($"JArray(ArrayList source) - valueType is ArrayList - calling JsonArrayListAttribute.Serialize(valueType, value)");

					Items[index++] = JsonArrayAttribute.Serialize(valueType, (ArrayList)item);
				}
				else
                {
                    Debug.WriteLine($"JArray(ArrayList source) - valueType is not Array and not ValueType or string - calling JObject.Serialize(valueType, value)");

                    Items[index++] = JsonObjectAttribute.Serialize(valueType, item); ;
                }
            }

			Debug.WriteLine($"JArray(ArrayList source) - Finished");
		}

		public int Length
		{
			get { return Items.Length; }
		}

		public JsonToken[] Items { get; }

		public static JsonArrayAttribute Serialize(Type type, object oSource)
		{
			return new JsonArrayAttribute((Array)oSource);
		}

		public static JsonArrayAttribute Serialize(Type type, ArrayList oSource)
		{
			return new JsonArrayAttribute(oSource);
		}

		public JsonToken this[int i]
		{
			get { return Items[i]; }
		}

		public override string ToString()
		{
			// set up a SerializationContext object and Lock it (via Monitor)
			EnterSerialization();

			try
			{
				StringBuilder sb = new StringBuilder();

				sb.Append('[');

				int prefaceLength = 0;
				bool first = true;
				
				foreach (var item in Items)
				{
					if (!first)
					{
						if (sb.Length - prefaceLength > 72)
						{
							// Use minimalist JSON, pretty can be handled by the client!
							sb.Append(",");

							prefaceLength = sb.Length;
						}
						else
						{
							sb.Append(',');
						}
					}

					first = false;

					sb.Append(item);
				}

				sb.Append(']');

				return sb.ToString();
			}
			finally
			{
				ExitSerialization();    // Unlocks the SerializationContext object
			}
		}
	}
}
