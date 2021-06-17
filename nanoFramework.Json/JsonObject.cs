//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json
{
    internal class JsonObject : JsonToken
	{
		private readonly Hashtable _members = new();

		public JsonProperty this[string name]
		{
			get { return (JsonProperty)_members[name.ToLower()]; }
			set
			{
				if (name.ToLower() != value.Name.ToLower())
				{
					throw new ArgumentException("index value must match property name");
				}
				_members.Add(value.Name.ToLower(), value);
			}
		}

		public bool Contains(string name) => this._members.Contains(name.ToLower());

        public ICollection Members => _members.Values;

        public static object JsonObjectAttribute { get; private set; }

        public void Add(string name, JsonToken value)
		{
			_members.Add(name.ToLower(), new JsonProperty(name, value));
		}


		private static string indent = "";

		public static JsonObject Serialize(Type type, object oSource)
		{
			indent += "      ";         // Indent the debug output - this helps to show recursion
		
			Debug.WriteLine($"JObject.Serialize() - Start - type: {type.Name}    oSource.GetType(): {oSource.GetType().Name}");
			
			var result = new JsonObject();
			MethodInfo[] methods;
			Type elementType;

			if (type.FullName == "System.Collections.Hashtable")
			{
				Debug.WriteLine($"JObject.Serialize() - type is Hashtable");

				return Serialize((Hashtable)oSource);
			}

            if (type.IsArray)
			{
				elementType = type.GetElementType();
			
				Debug.WriteLine($"JObject.Serialize() - type is Array - elementType: {elementType?.Name ?? "null"} ");
			}
			
			// Loop through all of this type's methods - find a get_ method that can be used to serialize oSource
			methods = type.GetMethods();
			
			foreach (var m in methods)
			{
				// don't care about:
				// - non public methods
				// - use abstract methods
				if (!m.IsPublic
					|| m.IsAbstract)
				{
					continue;
				}

				// Modified AS TINY CLR May Have issue with Getter for Chars & Length from String (see post forum)
				// Discard methods that start with 'get_'
				if (m.Name.IndexOf("get_") != 0)
				{
					continue;   
				}

				if (
					m.Name == "get_Chars"
					|| m.Name == "get_Length" 
					|| m.Name == "Empty" 
					|| m.Name == "get_IsReadOnly"
					|| m.Name == "get_IsFixedSize"
					|| m.Name == "get_IsSynchronized"
					|| m.Name == "get_Item"
					|| m.Name == "get_Keys"
					|| m.Name == "get_Values"
					|| m.Name == "get_SyncRoot"
					|| m.Name == "get_Count"
					|| m.Name == "get_Capacity"
					)
				{
					continue;   // Not all 'get_' methods have what we're looking for
				}

				// take out the 'get_'
				var name = m.Name.Substring(4);

				var methodResult = m.Invoke(oSource, null);

				// It was pretty tricky getting things to work - tried lots of different combinations - needed lots of debug - keep it in case future testing reveals trouble
				// Code would be pretty simple without all this debug - maybe get rid of it at some point after things have been well proven

				//TODO: debug helper does not handle null objects. Commented out for the time being!
				//Debug.WriteLine($"JObject.Serialize() - methods loop - method: {m.Name}   methodResult.GetType(): {methodResult.GetType().Name}  methodResult: {methodResult.ToString()}  m.DeclaringType: {m.DeclaringType.Name}");
				if (methodResult == null)
				{
					Debug.WriteLine($"JObject.Serialize() - methods loop - methodResult is null.  Calling JValue.Serialize({m.ReturnType.Name}, null) ");
					result._members.Add(name, new JsonProperty(name, JsonValue.Serialize(m.ReturnType, null)));
					Debug.WriteLine($"JObject.Serialize() - methods loop - added JProperty({name}, JValue.Serialize(...)) results to result._members[]");
				}
				else if (
					m.ReturnType.IsValueType 
					|| m.ReturnType == typeof(string))
				{
					Debug.WriteLine($"JObject.Serialize() - methods loop - m.ReturnType is ValueType or string. Calling JValue.Serialize({m.ReturnType.Name}, {methodResult}) ");
					
					result._members.Add(name, new JsonProperty(name, JsonValue.Serialize(m.ReturnType, methodResult)));
					
					Debug.WriteLine($"JObject.Serialize() - methods loop - added JProperty({name}, JValue.Serialize(...)) results to result._members[]");
				}
				else if (m.ReturnType.IsArray)
				{	
					// Original code checked m.DeclaringType - this didn't work very well - checking m.ReturnType made all the difference
					elementType = methodResult.GetType().GetElementType();
					
					// Tried lots of combinations to get this to work - used 'json2csharp.com' to verify the serialized result string - leave this debug here in case future testing reveals trouble  
					Debug.WriteLine($"JObject.Serialize() - methods loop - m.ReturnType is ValueType.  Calling JArray.Serialize({m.ReturnType.Name}, {methodResult}) ");
					
					result._members.Add(name, new JsonProperty(name, JsonArray.Serialize(m.ReturnType, methodResult)));
					
					Debug.WriteLine($"JObject.Serialize() - methods loop - added JProperty({elementType.Name}, JArray.Serialize(...)) results to result._members[]");
				}
				else
				{
					Debug.WriteLine($"JObject.Serialize() - methods loop - calling JObject.Serialize({m.ReturnType.Name}, {methodResult}) ");
					
					result._members.Add(name, new JsonProperty(name, Serialize(m.ReturnType, methodResult)));
					
					Debug.WriteLine($"JObject.Serialize() - methods loop - added JProperty({name}, JObject.Serialize(...)) results to result._members[]");
				}
			}

			Debug.WriteLine($"JObject.Serialize() - methods loop finished - start fields loop");

			var fields = type.GetFields();
			foreach (var f in fields)
			{
				if (f.FieldType.IsNotPublic)
				{
					continue;
				}

				switch (f.MemberType)
				{
					case MemberTypes.Field:
					case MemberTypes.Property:
						var value = f.GetValue(oSource);

						if (value == null)
						{
							result._members.Add(f.Name, new JsonProperty(f.Name, JsonValue.Serialize(f.FieldType, null)));
						}
						else if (f.FieldType.IsValueType || f.FieldType == typeof(string))
						{
							result._members.Add(f.Name.ToLower(), new JsonProperty(f.Name, JsonValue.Serialize(f.FieldType, value)));
						}
						else
						{
							if (f.FieldType.IsArray)
							{
								result._members.Add(f.Name.ToLower(), new JsonProperty(f.Name, JsonArray.Serialize(f.FieldType, value)));
							}
							else
							{
								result._members.Add(f.Name.ToLower(), new JsonProperty(f.Name, Serialize(f.FieldType, value)));
							}
						}
						break;

					default:
						break;
				}
			}
			Debug.WriteLine($"JObject.Serialize() - fields loop finished");
			Debug.WriteLine($"JObject.Serialize() - Finished - type: {type.Name}");

			indent = indent.Substring(6);     // 'Outdent' before returning

			return result;
		}

        private static JsonObject Serialize(Hashtable source)
        {
            Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - Start - length: {source.Keys.Count}");

            JsonObject result = new();

			// index for items
			int index = 0;

            foreach (var key in source.Keys)
            {
                Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - keys loop - processing key: {key}");

                var value = source[key];

                if (value == null)
                {
                    Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - value is null");

					result._members.Add(key.ToString(), new JsonProperty(key.ToString(), new JsonValue(null)));
                }
                else
                {
                    var valueType = value.GetType();

                    if (valueType == null)
                    {
                        //TODO: handle nulls
                        throw new Exception($"JsonObjectAttribute(Hashtable source) - value.GetType() returned null");
                    }

                    Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - valueType: {valueType.Name} ");

                    if ((valueType.IsValueType) || (valueType == typeof(string)))
                    {
                        Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - valueType is ValueType or string - calling JValue.Serialize(valueType, value)");

						result._members.Add(key.ToString(), new JsonProperty(key.ToString(), JsonValue.Serialize(valueType, value)));
                    }
                    else if (valueType.IsArray)
                    {
                        Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - valueType is Array - calling JsonObjectAttribute.Serialize(valueType, value)");

						result._members.Add(key.ToString(), JsonArray.Serialize(valueType, value));
                    }
                    else if (valueType.FullName == "System.Collections.ArrayList")
                    {
                        Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - valueType is ArrayList - calling JsonArrayAttribute.Serialize(valueType, value)");

                        result._members.Add(key.ToString(), new JsonProperty(key.ToString(), JsonArray.Serialize(valueType, (ArrayList)value)));
                    }
                    else
                    {
                        Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - valueType is not Array and not ValueType or string - calling JObject.Serialize(valueType, value)");

						result._members.Add(key.ToString(), Serialize(valueType, value));
                    }
                }
				
				index++;
            }

            Debug.WriteLine($"JsonObjectAttribute(Hashtable source) - Finished");

			return result;
        }

		public static JsonArray Serialize(ArrayList source)
		{
			Debug.WriteLine($"JArray(ArrayList source) - Start - source type: {source.GetType().Name}  length: {source.Count}");

            JsonToken[] result = new JsonToken[source.Count];

			// index for items
			int index = 0;

			foreach (var item in source)
			{
				Debug.WriteLine($"JArray(ArrayList source) - loop - processing item: {item}");

				if (item == null)
				{
					Debug.WriteLine($"JArray(ArrayList source) - value is null");

					result[index] = new JsonValue(null);
				}
				else
				{
					var valueType = item.GetType();

					if (valueType == null)
					{
						//TODO: handle nulls
						throw new Exception($"JArray(ArrayList source) - value.GetType() returned null");
					}

					Debug.WriteLine($"JArray(ArrayList source) - valueType: {valueType.Name} ");

					if (valueType.IsValueType || (valueType == typeof(string)))
					{
						Debug.WriteLine($"JArray(ArrayList source) - valueType is ValueType or string - calling JValue.Serialize(valueType, value)");

						result[index] = JsonValue.Serialize(valueType, item);
					}
					else if (valueType.IsArray)
					{
						Debug.WriteLine($"JArray(ArrayList source) - valueType is Array - calling JArray.Serialize(valueType, value)");

						result[index] = JsonArray.Serialize(valueType, item);
					}
					else if (valueType.FullName == "System.Collections.ArrayList")
					{
						Debug.WriteLine($"JArray(ArrayList source) - valueType is ArrayList - calling JsonArrayListAttribute.Serialize(valueType, value)");

						result[index] = JsonArray.Serialize(valueType, (ArrayList)item);
					}
					else
					{
						Debug.WriteLine($"JArray(ArrayList source) - valueType is not Array and not ValueType or string - calling JObject.Serialize(valueType, value)");

						result[index] = Serialize(valueType, item);
					}
				}

				index++;
			}

			Debug.WriteLine($"JArray(ArrayList source) - Finished");

			return new JsonArray(result);
		}


		//Use minimalist JSON, pretty can be handled on the client!
		public override string ToString()
		{
			// set up a SerializationContext object and Lock it (via Monitor)
			EnterSerialization();

			try
			{
				StringBuilder sb = new();

				sb.Append("{"); 

				bool first = true;
				
				foreach (var member in _members.Values)
				{
					if (!first)
					{
						sb.Append(",");
					}

					first = false;

					sb.Append(((JsonProperty)member).ToString());
				}

				sb.Append("}");

				return sb.ToString();
			}
			finally
			{
				ExitSerialization();    // Unlocks the SerializationContext object
			}
		}
	}
}
