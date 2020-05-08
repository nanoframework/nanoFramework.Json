//
// Copyright (c) 2020 The nanoFramework project contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using Windows.Storage.Streams;

namespace nanoFramework.Json
{

	public static class JsonConvert
	{
		private enum TokenType
		{
			LBrace, RBrace, LArray, RArray, Colon, Comma, String, Number, Date, Error,
			True, False, Null, End
		}

		private struct LexToken
		{
			public TokenType TType;
			public string TValue;
		}

		internal class SerializationCtx
		{
			public int Indent;
		}

		internal static SerializationCtx SerializationContext = null;
		internal static object SyncObj = new object();


		/// <summary>
		/// Convert an object to a JSON string.
		/// </summary>
		/// <param name="oSource">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, Array, IDictionary, IEnumerable, Guid, Datetime, DictionaryEntry, Object and null.</param>
		/// <returns>The JSON object as a string or null when the value type is not supported.</returns>
		/// <remarks>For objects, only public properties with getters are converted.</remarks>
		public static string SerializeObject(object oSource)
		{
			DebugHelper.DisplayDebug($"Serialize(object oSource) - oSource.GetType(): {oSource.GetType().Name}  oSource.ToString(): {oSource.ToString()}");
			var type = oSource.GetType();
			if (type.IsArray)
			{
				JsonToken retToken = JsonArrayAttribute.Serialize(type, oSource);
				DebugHelper.DisplayDebug($"Serialize(object oSource) - finished after calling JArray.Serialize() ");
				return retToken.ToString();
			}
			else
			{
				JsonToken retToken = JsonObjectAttribute.Serialize(type, oSource);
				DebugHelper.DisplayDebug($"Serialize(object oSource) - finished after calling JObject.Serialize() ");
				return retToken.ToString();
			}
		}

		/// <summary>
		/// Deserializes a Json string into an object.
		/// </summary>
		/// <param name="sourceString"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object DeserializeObject(string sourceString, Type type)
		{
			var dserResult = DeserializeObject(sourceString);
			return PopulateObject((JsonToken)dserResult, type, "/");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object DeserializeObject(Stream stream, Type type)
		{
			var dserResult = DeserializeObject(stream);
			return PopulateObject((JsonToken)dserResult, type, "/");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object DeserializeObject(DataReader dr, Type type)
		{
			var dserResult = DeserializeObject(dr);
			return PopulateObject((JsonToken)dserResult, type, "/");
		}

		// Trying to deserialize a stream in nanoFramework is problematic.
		// as Stream.Peek() has not been implemented in nanoFramework
		// Therefore, read all input into the static jsonBytes[] and use jsonPos to keep track of where we are when parsing the input
		private static byte[] jsonBytes;     // Do all deserialization using this byte[]
		private static int jsonPos;      // Current position in jsonBytes[]

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceString"></param>
		/// <returns></returns>
		public static object DeserializeObject(string sourceString)
		{
			jsonBytes = new byte[sourceString.Length];
			jsonBytes = Encoding.UTF8.GetBytes(sourceString);
			jsonPos = 0;
			return Deserialize();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceStream"></param>
		/// <returns></returns>
		public static object DeserializeObject(Stream sourceStream)
		{
			// Read the sourcestream into jsonBytes[]
			jsonBytes = new byte[sourceStream.Length];
			sourceStream.Read(jsonBytes, 0, (int)sourceStream.Length);
			jsonPos = 0;
			return Deserialize();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dr"></param>
		/// <returns></returns>
		public static object DeserializeObject(DataReader dr)
		{
			// Read the DataReader into jsonBytes[]
			jsonBytes = new byte[dr.UnconsumedBufferLength];
			jsonPos = 0;
			while (dr.UnconsumedBufferLength > 0)
			{
				jsonBytes[jsonPos++] = dr.ReadByte();
			}
			jsonPos = 0;
			return Deserialize();
		}


		private static string debugIndent = $"PopulateObject() - ";      // PopulateObject() goes recursive - this cleans up the debug output
		private static int debugOutdent = 10;

		private static object PopulateObject(JsonToken rootToken, Type rootType, string rootPath)
		{
			if ((rootToken == null) || (rootType == null) || (rootPath == null))
			{
				throw new Exception($"PopulateObject() - All parameters must be non-null.  rootToken: {(rootToken != null ? rootToken.GetType().Name : "null")}   rootType: {(rootType != null ? rootType.Name : "null")}   rootPath: {(rootPath != null ? rootPath : "null")}");
			}
			try
			{
				// Leave the debug here in case future testing reveals trouble - maybe get rid of it at some point
				debugIndent = "          " + debugIndent;
				DebugHelper.DisplayDebug($"{debugIndent} Start - ");        // Simple message to make sure we get to this point - displaying null values causes the device to hang & makes debugging problematic
				DebugHelper.DisplayDebug($"{debugIndent} rootToken is a {rootToken.GetType().Name}  rootType: {rootType.Name}   rootPath: {rootPath}");
				if (rootToken is JsonObjectAttribute)
				{
					object rootInstance = null;         // This is the object that gets populated and returned
														// Create rootInstance from the rootType's constructor
					DebugHelper.DisplayDebug($"{debugIndent} create rootInstance from rootType: {rootType.Name} using GetConstructor() & Invoke()");
					Type[] types = { };        // Empty array of Types - GetConstructor didn't work unless given an empty array of Type[]
					ConstructorInfo ci = rootType.GetConstructor(types);
					if (ci == null)
					{
						throw new Exception($"PopulateObject() - failed to create target instance.   rootType: {rootType.Name} ");
					}
					rootInstance = ci.Invoke(null);
					DebugHelper.DisplayDebug($"{debugIndent} rootInstance created.  rootInstance.GetType(): {(rootInstance?.GetType()?.Name != null ? rootInstance.GetType().Name : "null")}");
					// If we haven't successfully created rootInstance, bail out
					if (rootInstance == null)
					{
						throw new Exception($"PopulateObject() - failed to create target instance from rootType: {rootType.Name} ");
					}

					// Everything looks good - process the rootToken as a JObject
					var rootObject = (JsonObjectAttribute)rootToken;
					if ((rootObject == null) || (rootObject.Members == null))
					{
						throw new Exception($"PopulateObject() - failed to create target instance from rootType: {rootType.Name} ");
					}

					// Process all members for this rootObject
					DebugHelper.DisplayDebug($"{debugIndent} Entering rootObject.Members loop ");
					foreach (var m in rootObject.Members)
					{
						DebugHelper.DisplayDebug($"{debugIndent} Process rootObject.Member");
						var memberProperty = (JsonPropertyAttribute)m;
						DebugHelper.DisplayDebug($"{debugIndent}     memberProperty.Name:  {memberProperty?.Name ?? "null"} ");
						MethodInfo memberGetMethod = rootType.GetMethod("get_" + memberProperty.Name);
						Type memberType = memberGetMethod.ReturnType;
						MethodInfo memberSetMethod = rootType.GetMethod("set_" + memberProperty.Name);
						if (memberType == null)
						{
							throw new Exception($"PopulateObject() - failed to create memberType from {rootType.Name}.GetMethod ");
						}
						// Process the member based on JObject, JValue, or JArray
						if (memberProperty.Value is JsonObjectAttribute)
						{
							// Call PopulateObject() for this member - i.e. recursion
							DebugHelper.DisplayDebug($"{debugIndent}     memberProperty.Value is JObject");
							var memberPath = rootPath;
							if (memberPath[memberPath.Length - 1] == '/')
							{
								memberPath += memberProperty.Name;                      // Don't need to add a slash before appending rootElementType
							}
							else
							{
								memberPath = memberPath + '/' + memberProperty.Name;    // Need to add a slash before appending rootElementType
							}
							var memberObject = PopulateObject(memberProperty.Value, memberType, memberPath);
							memberSetMethod.Invoke(rootInstance, new object[] { memberObject });
						}
						else if (memberProperty.Value is JsonValue)
						{
							// Don't need any more info - populate the member using memberSetMethod.Invoke()
							DebugHelper.DisplayDebug($"{debugIndent}     memberProperty.Value is JValue");
							if (memberType != typeof(DateTime))
							{
								memberSetMethod.Invoke(rootInstance, new object[] { ((JsonValue)memberProperty.Value).Value });
							}
							else
							{
								DateTime dt;
								var sdt = ((JsonValue)memberProperty.Value).Value.ToString();
								if (sdt.Contains("Date("))
								{
									dt = DateTimeExtensions.FromASPNetAjax(sdt);
								}
								else
								{
									dt = DateTimeExtensions.FromIso8601(sdt);
								}
								memberSetMethod.Invoke(rootInstance, new object[] { dt });
							}
						}
						else if (memberProperty.Value is JsonArrayAttribute)
						{
							DebugHelper.DisplayDebug($"{debugIndent}     memberProperty.Value is a JArray");
							Type memberElementType = memberType.GetElementType();    // Need this type when we try to populate the array elements
							var memberValueArray = (JsonArrayAttribute)memberProperty.Value;   // Create a JArray (memberValueArray) to hold the contents of memberProperty.Value 
							var memberValueArrayList = new ArrayList();             // Create a temporary ArrayList memberValueArrayList - populate this as the memberItems are parsed
							JsonToken[] memberItems = memberValueArray.Items;          // Create a JToken[] array for Items associated for this memberProperty.Value
							DebugHelper.DisplayDebug($"{debugIndent}       copy {memberItems.Length} memberItems from memberValueArray into memberValueArrayList - call PopulateObject() for items that aren't JValue");
							foreach (JsonToken item in memberItems)
							{
								if (item is JsonValue)
								{
									DebugHelper.DisplayDebug($"{debugIndent}         item is a JsonValue: {((JsonValue)item).Value} - Add it to memberValueArrayList");
									memberValueArrayList.Add(((JsonValue)item).Value);
									DebugHelper.DisplayDebug($"{debugIndent}         item JValue added to memberValueArrayList");
								}
								else if (item is JsonToken)
								{
									// Since memberProperty.Value is a JsonnArray:
									// 		memberType        is the array   type (i.e. foobar[])
									// 		memberElementType is the element type (i.e. foobar)		- use this to call PopulateObject()
									string memberElementPath = rootPath + "/" + memberProperty.Name + "/" + memberElementType.Name;
									DebugHelper.DisplayDebug($"{debugIndent}         memberType: {memberType.Name}   memberElementType: {memberElementType.Name} ");
									DebugHelper.DisplayDebug($"{debugIndent}         calling PopulateObject(JsonToken item, {memberElementType.Name}, {memberElementPath}) ");
									var itemObj = PopulateObject(item, memberElementType, memberElementPath);
									memberValueArrayList.Add(itemObj);
									DebugHelper.DisplayDebug($"{debugIndent}         item added to memberValueArrayList");
								}
								else
								{
									DebugHelper.DisplayDebug($"{debugIndent}         item is not a JToken or a JValue - this case is not handled");
								}
							}
							DebugHelper.DisplayDebug($"{debugIndent}       {memberItems.Length} memberValueArray.Items copied into memberValueArrayList - i.e. contents of memberProperty.Value");

							// Create targetArray - an Array of memberElementType objects - targetArray will be copied to rootInstance - then rootInstance will be returned
							DebugHelper.DisplayDebug($"{debugIndent}       create targetArray - an Array of memberElementType: {memberElementType} objects - use Array.CreateInstance({memberElementType}, {memberValueArray.Length}");
							Array targetArray = Array.CreateInstance(memberElementType, memberValueArray.Length);
							if (targetArray == null)
							{
								throw new Exception("PopulateObject() - failed to create Array of type: {memberElementType}[]");
							}
							DebugHelper.DisplayDebug($"{debugIndent}       targetArray created using CreateInstance().  targetArray.GetType().Name: {(targetArray?.GetType()?.Name != null ? targetArray.GetType().Name : "null")}");

							memberValueArrayList.CopyTo(targetArray);
							DebugHelper.DisplayDebug($"{debugIndent}       copied memberValueArrayList into the targetArray");
							memberSetMethod.Invoke(rootInstance, new object[] { targetArray });
							DebugHelper.DisplayDebug($"{debugIndent}       populated the rootInstance object with the contents of targetArray -  via {memberSetMethod.Name}()");
						}
					}
					debugIndent = debugIndent.Substring(debugOutdent);     // 'Outdent' before returning
					DebugHelper.DisplayDebug($"{debugIndent} Returning rootInstance");
					return rootInstance;
				}
				else if (rootToken is JsonArrayAttribute)
				{
					Type rootElementType = rootType.GetElementType();
					if (rootElementType == null)
					{
						throw new NotSupportedException($"PopulateObject() - For arrays, type: {rootType.Name} must have a valid element type");
					}
					DebugHelper.DisplayDebug($"{debugIndent} rootType: {rootType.Name}  rootType.GetElementType(): {rootType.GetElementType().Name}");

					// Create & populate rootArrayList with the items in rootToken - call PopulateObject if the item is more complicated than a JValue 
					DebugHelper.DisplayDebug($"{debugIndent} Create and populate rootArrayList with the items in rootToken - call PopulateObject if the item is more complicated than a JValue");
					ArrayList rootArrayList = new ArrayList();
					JsonArrayAttribute rootArray = (JsonArrayAttribute)rootToken;
					foreach (var item in rootArray.Items)
					{
						if (item is JsonValue)
						{
							DebugHelper.DisplayDebug($"{debugIndent} item type: {item.GetType().Name}.   Adding it to rootArrayList");
							rootArrayList.Add(((JsonValue)item).Value);
						}
						else
						{
							DebugHelper.DisplayDebug($"{debugIndent} item type: {item.GetType().Name} - use rootElementType to call PopulateObject()");
							// Pass rootElementType and rootPath with rootElementType appended to PopulateObject for this item 
							string itemPath = rootPath;
							if (itemPath[itemPath.Length - 1] == '/')
							{
								itemPath += rootElementType.Name;                   // Don't need to add a slash before appending rootElementType
							}
							else
							{
								itemPath = itemPath + '/' + rootElementType.Name;   // Need to add a slash before appending rootElementType
							}
							var itemObj = PopulateObject(item, rootElementType, itemPath);
							rootArrayList.Add(itemObj);
							DebugHelper.DisplayDebug($"{debugIndent} added object of type: {itemObj.GetType().Name} (returned from PopulateObject()) to rootArrayList");
						}
					}
					DebugHelper.DisplayDebug($"{debugIndent} finished creating rootArrayList - copy rootArrayList to targetArray");
					Array targetArray = Array.CreateInstance(rootType.GetElementType(), rootArray.Length);
					if (targetArray == null)
					{
						throw new Exception($"PopulateObject() - CreateInstance() failed for type: {rootElementType.Name}    length: {rootArray.Length}");
					}
					DebugHelper.DisplayDebug($"{debugIndent} created Array targetArray by calling CreateInstance() with rootType.GetElementType()");
					rootArrayList.CopyTo(targetArray);
					DebugHelper.DisplayDebug($"{debugIndent} populated targetArray with the contents of rootArrayList");
					debugIndent = debugIndent.Substring(debugOutdent);     // 'Outdent' before returning
					return targetArray;
				}   // end of  (if rootToken is JArray)
				debugIndent = debugIndent.Substring(debugOutdent);          // 'Outdent' before returning
				return null;
			}
			catch (Exception ex)
			{
				DebugHelper.DisplayDebug($"{debugIndent} Exception: {ex.Message}");
				debugIndent = debugIndent.Substring(debugOutdent);          // 'Outdent' before returning
				return null;
			}
		}   // end of PopulateObject


		// Deserialize() now assumes that the input has been copied int jsonBytes[]
		// Keep track of position with jsonPos
		private static JsonToken Deserialize()
		{
			LexToken token;
			token = GetNextToken();
			// Deserialize the json input data in jsonBytes[]
			//DisplayDebug($"Deserialize() - jsonPos: {jsonPos}   jsonBytes.Length: {jsonBytes.Length}");
			JsonToken result;
			//DisplayDebug($"Deserialize() - JsonConverter - Deserialize() - GetNextToken() returned:  token.TType: {token.TType.TokenTypeToString()}  token.TValue: {token.TValue}");

			switch (token.TType)
			{
				case TokenType.LBrace:
					result = ParseObject(ref token);
					if (token.TType == TokenType.RBrace)
					{
						token = GetNextToken();
					}
					else if (token.TType != TokenType.End && token.TType != TokenType.Error)
					{        // MORT clean this up
						throw new Exception("unexpected content after end of object");
					}
					break;
				case TokenType.LArray:
					result = ParseArray(ref token);
					if (token.TType == TokenType.RArray)
					{
						token = GetNextToken();
					}
					else if (token.TType != TokenType.End && token.TType != TokenType.Error)
					{        // MORT clean this up
						throw new Exception("unexpected content after end of array");
					}
					break;
				default:
					throw new Exception("unexpected initial token in json parse");
			}
			if (token.TType != TokenType.End)
			{
				throw new Exception("unexpected end token in json parse");
			}
			else if (token.TType == TokenType.Error)
			{
				throw new Exception("unexpected lexical token during json parse");
			}
			return result;
		}

		private static JsonObjectAttribute ParseObject(ref LexToken token)
		{
			var result = new JsonObjectAttribute();
			token = GetNextToken();
			while (token.TType != TokenType.End && token.TType != TokenType.Error && token.TType != TokenType.RBrace)
			{         // MORT clean this up
					  // Get the name from the name:value pair
				if (token.TType != TokenType.String)
				{
					throw new Exception("expected label");
				}
				var propName = token.TValue;
				// Look for the :
				token = GetNextToken();
				if (token.TType != TokenType.Colon)
				{
					throw new Exception("expected colon");
				}
				// Get the value from the name:value pair
				var value = ParseValue(ref token);
				result.Add(propName, value);
				// Look for additional name:value pairs (i.e. separated by a comma)
				token = GetNextToken();
				if (token.TType == TokenType.Comma)
				{
					token = GetNextToken();
				}
			} //TODO!!! Comma removed?
			if (token.TType == TokenType.Error)
			{
				throw new Exception("unexpected token in json object");
			}
			else if (token.TType != TokenType.RBrace)
			{
				throw new Exception("unterminated json object");
			}
			return result;
		}

		private static JsonArrayAttribute ParseArray(ref LexToken token)
		{
			ArrayList list = new ArrayList();
			while (token.TType != TokenType.End && token.TType != TokenType.Error && token.TType != TokenType.RArray)
			{         // MORT clean this up
				var value = ParseValue(ref token);
				if (value != null)
				{
					list.Add(value);
					token = GetNextToken();
					if (token.TType != TokenType.Comma && token.TType != TokenType.RArray)
					{
						throw new Exception("badly formed array");
					}
				}
			} //TODO!!! comma removed?
			if (token.TType == TokenType.Error)
			{
				throw new Exception("unexpected token in array");
			}
			else if (token.TType != TokenType.RArray)
			{
				throw new Exception("unterminated json array");
			}
			var result = new JsonArrayAttribute((JsonToken[])list.ToArray(typeof(JsonToken)));
			return result;
		}

		private static JsonToken ParseValue(ref LexToken token)
		{
			token = GetNextToken();
			if (token.TType == TokenType.RArray)
			{
				// we were expecting a value in an array, and came across the end-of-array marker,
				//  so this is an empty array
				return null;
			}
			else if (token.TType == TokenType.String)
			{
				return new JsonValue(token.TValue);
			}
			else if (token.TType == TokenType.Number)
			{
				if (token.TValue.IndexOfAny(new char[] { '.', 'e', 'E' }) != -1)
				{
					return new JsonValue(double.Parse(token.TValue));
				}
				else
				{
					return new JsonValue(int.Parse(token.TValue));
				}
			}
			else if (token.TType == TokenType.True)
			{
				return new JsonValue(true);
			}
			else if (token.TType == TokenType.False)
			{
				return new JsonValue(false);
			}
			else if (token.TType == TokenType.Null)
			{
				return new JsonValue(null);
			}
			else if (token.TType == TokenType.Date)
			{
				throw new NotSupportedException("datetime parsing not supported");
			}
			else if (token.TType == TokenType.LBrace)
			{
				return ParseObject(ref token);
			}
			else if (token.TType == TokenType.LArray)
			{
				return ParseArray(ref token);
			}

			throw new Exception("invalid value found during json parse");
		}

		private static LexToken GetNextToken()
		{
			var result = GetNextTokenInternal();
			return result;
		}


		private static LexToken GetNextTokenInternal()
		{
			try
			{
				StringBuilder sb = null;
				char openQuote = '\0';
				char ch = ' ';
				while (true)
				{
					if (jsonPos >= jsonBytes.Length)
					{
						DebugHelper.DisplayDebug($"GetNextTokenInternal() - JsonConverter GetNextToken() - no more data - call EndToken()");
						return EndToken(sb);
					}
					ch = (char)jsonBytes[jsonPos++];

					// Handle json escapes
					bool escaped = false;
					if (ch == '\\')
					{
						escaped = true;
						ch = (char)jsonBytes[jsonPos++];
						if (ch == (char)0xffff)
						{
							return EndToken(sb);
						}
						//TODO: replace with a mapping array? This switch is really incomplete.
						switch (ch)
						{
							case '\'':
								ch = '\'';
								break;
							case '"':
								ch = '"';
								break;
							case 't':
								ch = '\t';
								break;
							case 'r':
								ch = '\r';
								break;
							case 'n':
								ch = '\n';
								break;
							default:
								throw new Exception("unsupported escape");
						}
					}

					if ((sb != null) && ((ch != openQuote) || (escaped)))
					{
						sb.Append(ch);
					}
					else if (IsNumberIntroChar(ch))
					{
						sb = new StringBuilder();
						while (IsNumberChar(ch))
						{
							sb.Append(ch);
							// nanoFramework doesn't support Peek() for Streams or DataReaders
							// This is why we converted everything to a byte[] instead of trying to work directly from a Stream or a DataReader
							// Look at the next byte but don't advance jsonPos unless we're still working on the number
							// i.e. Peek() to see if we're at the end of the number
							ch = (char)jsonBytes[jsonPos];
							if (IsNumberChar(ch))
							{
								jsonPos++;                      // We're still working on the number - advance jsonPos
							}

							if (ch == (char)0xffff)
							{
								return EndToken(sb);
							}
						}
						// Note that we don't claim that this is a well-formed number
						return new LexToken() { TType = TokenType.Number, TValue = sb.ToString() };
					}
					else
					{
						switch (ch)
						{
							case '{':
								return new LexToken() { TType = TokenType.LBrace, TValue = null };
							case '}':
								return new LexToken() { TType = TokenType.RBrace, TValue = null };
							case '[':
								return new LexToken() { TType = TokenType.LArray, TValue = null };
							case ']':
								return new LexToken() { TType = TokenType.RArray, TValue = null };
							case ':':
								return new LexToken() { TType = TokenType.Colon, TValue = null };
							case ',':
								return new LexToken() { TType = TokenType.Comma, TValue = null };
							case '"':
							case '\'':
								if (sb == null)
								{
									openQuote = ch;
									sb = new StringBuilder();
								}
								else
								{
									// We're building a string and we hit a quote character.
									// The ch must match openQuote, or otherwise we should have eaten it above as string content
									//Debug.Assert(ch == openQuote);
									return new LexToken() { TType = TokenType.String, TValue = sb.ToString() };
								}
								break;
							case ' ':
							case '\t':
							case '\r':
							case '\n':
								break; // whitespace - go around again
							case (char)0xffff:
								return EndToken(sb);
							default:
								// try to collect a token
								switch (ch.ToLower())
								{
									case 't':
										Expect('r');
										Expect('u');
										Expect('e');
										return new LexToken() { TType = TokenType.True, TValue = null };
									case 'f':
										Expect('a');
										Expect('l');
										Expect('s');
										Expect('e');
										return new LexToken() { TType = TokenType.False, TValue = null };
									case 'n':
										Expect('u');
										Expect('l');
										Expect('l');
										return new LexToken() { TType = TokenType.Null, TValue = null };
									default:
										throw new Exception("unexpected character during json lexical parse");
								}
						}
					}
				}
			}
			catch (Exception e)
			{
				// MORT - eventually get rid of this try/catch
				DebugHelper.DisplayDebug($"GetNextTokenInternal() - Exception caught");
				DebugHelper.DisplayDebug($"GetNextTokenInternal() - Exception: {e.Message}");
				DebugHelper.DisplayDebug($"GetNextTokenInternal() - StackTrace: {e.StackTrace.ToString()}");
				throw new Exception("something bad happened");
			}
		}

		private static void Expect(char expected)
		{
			char ch = (char)jsonBytes[jsonPos++];
			if (ch.ToLower() != expected)
			{
				throw new Exception("unexpected character during json lexical parse");
			}
		}

		private static LexToken EndToken(StringBuilder sb)
		{
			if (sb != null)
			{
				return new LexToken() { TType = TokenType.Error, TValue = null };
			}
			else
			{
				return new LexToken() { TType = TokenType.End, TValue = null };
			}
		}

		// Legal first characters for numbers
		private static bool IsNumberIntroChar(char ch)
		{
			return (ch == '-') || (ch == '+') || (ch == '.') || (ch >= '0' & ch <= '9');
		}

		// Legal chars for 2..n'th position of a number
		private static bool IsNumberChar(char ch)
		{
			return (ch == '-') || (ch == '+') || (ch == '.') || (ch == 'e') || (ch == 'E') || (ch >= '0' & ch <= '9');
		}
	}
}
