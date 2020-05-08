//
// Copyright (c) 2020 The nanoFramework project contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Reflection;
using System.Text;


namespace nanoFramework.Json
{
	internal class JsonObjectAttribute : JsonToken
	{
		private readonly Hashtable _members = new Hashtable();

		public JsonPropertyAttribute this[string name]
		{
			get { return (JsonPropertyAttribute)_members[name.ToLower()]; }
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

		public ICollection Members
		{
			get { return _members.Values; }
		}

		public void Add(string name, JsonToken value)
		{
			_members.Add(name.ToLower(), new JsonPropertyAttribute(name, value));
		}


		private static string indent = "";

		public static JsonObjectAttribute Serialize(Type type, object oSource)
		{
			indent += "      ";         // Indent the debug output - this helps to show recursion
			DebugHelper.DisplayDebug($"JObject.Serialize() - Start - type: {type.Name}    oSource.GetType(): {oSource.GetType().Name}");
			var result = new JsonObjectAttribute();
			MethodInfo[] methods;
			Type elementType;
			if (type.IsArray)
			{
				elementType = type.GetElementType();
				DebugHelper.DisplayDebug($"JObject.Serialize() - type is Array - elementType: {elementType?.Name ?? "null"} ");
			}
			// Loop through all of this type's methods - find a get_ method that can be used to serialize oSource
			methods = type.GetMethods();
			foreach (var m in methods)
			{
				if (!m.IsPublic)
				{
					continue;               // Only look at public methods
				}
				// Modified AS TINY CLR May Have issue with Getter for Chars & Length from String (see post forum)
				if (m.Name.IndexOf("get_") != 0)
				{
					continue;   // Only look at methods that start with 'get_'
				}
				if ((m.Name == "get_Chars") || (m.Name == "get_Length" || (m.Name == "Empty") || (m.Name == "get_IsReadOnly") || (m.Name == "get_IsFixedSize") || (m.Name == "get_IsSynchronized")))
				{
					continue;   // Not all 'get_' methods have what we're looking for
				}
				var name = m.Name.Substring(4);     // take out the 'get_'
				var methodResult = m.Invoke(oSource, null);
				// It was pretty tricky getting things to work - tried lots of different combinations - needed lots of debug - keep it in case future testing reveals trouble
				// Code would be pretty simple without all this debug - maybe get rid of it at some point after things have been well proven
				DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - method: {m.Name}   methodResult.GetType(): {methodResult.GetType().Name}  methodResult: {methodResult.ToString()}  m.DeclaringType: {m.DeclaringType.Name}");
				if (methodResult == null)
				{
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - methodResult is null.  Calling JValue.Serialize({m.ReturnType.Name}, null) ");
					result._members.Add(name, new JsonPropertyAttribute(name, JsonValue.Serialize(m.ReturnType, null)));
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - added JProperty({name}, JValue.Serialize(...)) results to result._members[]");
				}
				else if (m.ReturnType.IsValueType || m.ReturnType == typeof(string))
				{
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - m.ReturnType is ValueType or string. Calling JValue.Serialize({m.ReturnType.Name}, {methodResult.ToString()}) ");
					result._members.Add(name, new JsonPropertyAttribute(name, JsonValue.Serialize(m.ReturnType, methodResult)));
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - added JProperty({name}, JValue.Serialize(...)) results to result._members[]");
				}
				else if (m.ReturnType.IsArray)
				{          // Original code checked m.DeclaringType - this didn't work very well - checking m.ReturnType made all the difference
					elementType = methodResult.GetType().GetElementType();
					// Tried lots of combinations to get this to work - used 'json2csharp.com' to verify the serialized result string - leave this debug here in case future testing reveals trouble  
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - m.ReturnType is ValueType.  Calling JArray.Serialize({m.ReturnType.Name}, {methodResult.ToString()}) ");
					result._members.Add(name, new JsonPropertyAttribute(name, JsonArrayAttribute.Serialize(m.ReturnType, methodResult)));
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - added JProperty({elementType.Name}, JArray.Serialize(...)) results to result._members[]");
				}
				else
				{
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - calling JObject.Serialize({m.ReturnType.Name}, {methodResult.ToString()}) ");
					result._members.Add(name, new JsonPropertyAttribute(name, JsonObjectAttribute.Serialize(m.ReturnType, methodResult)));
					DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop - added JProperty({name}, JObject.Serialize(...)) results to result._members[]");
				}
			}   // end of method loop
			DebugHelper.DisplayDebug($"JObject.Serialize() - methods loop finished - start fields loop");

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
							result._members.Add(f.Name, new JsonPropertyAttribute(f.Name, JsonValue.Serialize(f.FieldType, null)));
						}
						else if (f.FieldType == typeof(System.Single)) //catch nan values TODO: how should this be handled??? It doesnt hit it anyway!!! (value == NaN)
						{
							if ((float)value == Single.NaN)
							{
								result._members.Add(f.Name.ToLower(), new JsonPropertyAttribute(f.Name, JsonValue.Serialize(f.FieldType, value)));
							}
							else
							{
								result._members.Add(f.Name.ToLower(), new JsonPropertyAttribute(f.Name, JsonValue.Serialize(f.FieldType, value)));
							}
						}
						else if (f.FieldType.IsValueType || f.FieldType == typeof(string))
						{
							result._members.Add(f.Name.ToLower(), new JsonPropertyAttribute(f.Name, JsonValue.Serialize(f.FieldType, value)));
						}
						else
						{
							if (f.FieldType.IsArray)
							{
								result._members.Add(f.Name.ToLower(), new JsonPropertyAttribute(f.Name, JsonArrayAttribute.Serialize(f.FieldType, value)));
							}
							else
							{
								result._members.Add(f.Name.ToLower(), new JsonPropertyAttribute(f.Name, JsonObjectAttribute.Serialize(f.FieldType, value)));
							}
						}
						break;
					default:
						break;
				}
			}
			DebugHelper.DisplayDebug($"JObject.Serialize() - fields loop finished");
			DebugHelper.DisplayDebug($"JObject.Serialize() - Finished - type: {type.Name}");
			indent = indent.Substring(6);     // 'Outdent' before returning
			return result;
		}

		//Use minimalist JSON, pretty can be handled on the client!
		public override string ToString()
		{
			EnterSerialization();           // set up a SerializationContext object and Lock it (via Monitor)
			try
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("{"); 
				bool first = true;
				foreach (var member in _members.Values)
				{
					if (!first)
					{
						sb.Append(",");
					}
					first = false;
					sb.Append(((JsonPropertyAttribute)member).ToString());
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
