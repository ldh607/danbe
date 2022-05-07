using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Collections.Generic;


namespace CellBig.Common
{
	public static class JsonSerializer
	{
		enum TokenType
		{
			None,
			CurlyOpen,
			CurlyClose,
			SquaredOpen,
			SquaredClose,
			Colon,
			Comma,
			String,
			Number,
			True,
			False,
			Null
		}

		static StringBuilder _builder = new StringBuilder(512);
		static StringBuilder _parserBuilder = new StringBuilder(64);


		public static object Decode(byte[] json)
		{
			var enc = new UTF8Encoding();
			return Decode(enc.GetString(json, 0, json.Length));
		}

		public static object Decode(string json)
		{
			bool success = true;
			return Decode(json, ref success);
		}

		public static void Decode(object instance, string json)
		{
			var obj = Decode(json);
			PopulateObject(instance.GetType(), obj, instance);
		}

		public static object Decode(string json, ref bool success)
		{
			success = true;
			if (json != null)
			{
				char[] charArray = json.ToCharArray();
				int index = 0;
				object value = ParseValue(charArray, ref index, ref success);
				return value;
			}
			else
			{
				return null;
			}
		}

		public static string Encode(object json)
		{
			_builder.Length = 0;

			var success = SerializeValue(json, _builder);
			return (success ? _builder.ToString() : null);
		}

		public static T Decode<T>(byte[] json) where T : class, new()
		{
			var enc = new UTF8Encoding();
			return Decode<T>(enc.GetString(json, 0, json.Length));
		}

		public static T Decode<T>(string json) where T : class, new()
		{
			var success = true;
			var obj = Decode(json, ref success);
			return PopulateObject(typeof(T), obj) as T;
		}

		public static object Decode(Type t, string json)
		{
			var success = true;
			var obj = Decode(json, ref success);
			return PopulateObject(t, obj);
		}

		static object PopulateObject(Type T, object obj)
		{
			return PopulateObject(T, obj, null);
		}

		static object PopulateObject(Type T, object obj, object instance)
		{
			if (obj == null)
				return null;

			if (T.IsInstanceOfType(obj))
			{
				instance = obj;
			}
			else if (T.IsPrimitive && obj.GetType().IsPrimitive)
			{
				instance = obj;
			}
			else if (obj is Hashtable)
			{
				instance = instance ?? Activator.CreateInstance(T);

				var h = (Hashtable)obj;

				//var fields = T.GetFields();
				//FieldInfo fi;
				//for (int i = 0; i < fields.Length; ++i)
				foreach (var fi in T.GetFields())
				{
					//fi = fields[i];
					if (h.ContainsKey(fi.Name))
					{
						fi.SetValue(instance, PopulateObject(fi.FieldType, h[fi.Name]));
					}
				}

				//var properties = T.GetProperties();
				//PropertyInfo pi;
				//for (int i = 0; i < properties.Length; ++i)
				foreach (var pi in T.GetProperties())
				{
					//pi = properties[i];
					if (h.ContainsKey(pi.Name))
					{
						pi.SetValue(instance, PopulateObject(pi.PropertyType, h[pi.Name]), null);
					}
				}
			}
			else if (obj is IEnumerable)
			{
				instance = instance ?? Activator.CreateInstance(T);

				var list = instance as IList;
				if (list != null)
				{
					Type containerType = typeof(object);
					var IT = instance.GetType();
					if (IT.IsGenericType)
					{
						var args = IT.GetGenericArguments();
						if (args.Length != 1)
							return null;

						containerType = args[0];
					}

					foreach (var i in (IEnumerable)obj)
					{
						var v = PopulateObject(containerType, i);
						if (containerType == v.GetType())
						{
							list.Add(v);
						}
						else
						{
							var v2 = Convert.ChangeType(v, containerType);
							list.Add(v2);
						}
					}
				}
			}
			return instance;
		}

		static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			var table = new Hashtable();
			TokenType token;

			// {
			NextToken(json, ref index);

			while (true)
			{
				token = LookAhead(json, index);
				if (token == TokenType.None)
				{
					success = false;
					return null;
				}
				else if (token == TokenType.Comma)
				{
					NextToken(json, ref index);
				}
				else if (token == TokenType.CurlyClose)
				{
					NextToken(json, ref index);
					break;
				}
				else
				{
					// name
					var name = ParseString(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}

					// :
					token = NextToken(json, ref index);
					if (token != TokenType.Colon)
					{
						success = false;
						return null;
					}

					// value
					var value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}

					table[name] = value;
				}
			}

			return table;
		}

		static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			var array = new ArrayList();

			// [
			NextToken(json, ref index);

			while (true)
			{
				var token = LookAhead(json, index);
				if (token == TokenType.None)
				{
					success = false;
					return null;
				}
				else if (token == TokenType.Comma)
				{
					NextToken(json, ref index);
				}
				else if (token == TokenType.SquaredClose)
				{
					NextToken(json, ref index);
					break;
				}
				else
				{
					var value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}
					array.Add(value);
				}
			}

			return array;
		}

		static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index))
			{
			case TokenType.String:
				return ParseString(json, ref index, ref success);
			case TokenType.Number:
				return ParseNumber(json, ref index, ref success);
			case TokenType.CurlyOpen:
				return ParseObject(json, ref index, ref success);
			case TokenType.SquaredOpen:
				return ParseArray(json, ref index, ref success);
			case TokenType.True:
				NextToken(json, ref index);
				return true;
			case TokenType.False:
				NextToken(json, ref index);
				return false;
			case TokenType.Null:
				NextToken(json, ref index);
				return null;
			case TokenType.None:
				break;
			}

			success = false;
			return null;
		}

		static string ParseString(char[] json, ref int index, ref bool success)
		{
			_parserBuilder.Length = 0;

			char c;

			EatWhitespace(json, ref index);

			// Skip "(double quote) char
			index++;

			var complete = false;
			while (!complete)
			{
				if (index == json.Length)
				{
					break;
				}

				c = json[index++];
				if (c == '"')
				{
					complete = true;
					break;
				}
				else if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}
					c = json[index++];
					if (c == '"')
					{
						_parserBuilder.Append('"');
					}
					else if (c == '\\')
					{
						_parserBuilder.Append('\\');
					}
					else if (c == '/')
					{
						_parserBuilder.Append('/');
					}
					else if (c == 'b')
					{
						_parserBuilder.Append('\b');
					}
					else if (c == 'f')
					{
						_parserBuilder.Append('\f');
					}
					else if (c == 'n')
					{
						_parserBuilder.Append('\n');
					}
					else if (c == 'r')
					{
						_parserBuilder.Append('\r');
					}
					else if (c == 't')
					{
						_parserBuilder.Append('\t');
					}
					else if (c == 'u')
					{
						var remainingLength = json.Length - index;
						if (remainingLength >= 4)
						{
							// parse the 32 bit hex into an integer codepoint
							uint codePoint;
							if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
							{
								return "";
							}
							// convert the integer codepoint to a unicode char and add to string
							_parserBuilder.Append(Char.ConvertFromUtf32((int)codePoint));
							// skip 4 chars
							index += 4;
						}
						else
						{
							break;
						}
					}
				}
				else
				{
					_parserBuilder.Append(c);
				}
			}

			if (!complete)
			{
				success = false;
				return null;
			}

			return _parserBuilder.ToString();
		}

		static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			EatWhitespace(json, ref index);

			var lastIndex = GetLastIndexOfNumber(json, index);
			var charLength = (lastIndex - index) + 1;


			var token = new string(json, index, charLength);
			index = lastIndex + 1;
			if (token.Contains("."))
			{
				float number;
				if (float.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
				{
					return number;
				}
				else
				{
					return token;
				}
			}
			else
			{
				byte byteNumber;
				if (byte.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out byteNumber))
				{
					return byteNumber;
				}
				else
				{
					short shortNumber;
					if (short.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out shortNumber))
					{
						return shortNumber;
					}
					else
					{
						int intNumber;
						if (int.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out intNumber))
						{
							return intNumber;
						}
						else
						{
							long number;
							if (long.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
							{
								return number;
							}
							else
							{
								return token;
							}
						}
					}
				}
			}
		}

		static int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++)
			{
				if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
				{
					break;
				}
			}
			return lastIndex - 1;
		}

		static void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++)
			{
				if (" \t\n\r".IndexOf(json[index]) == -1)
				{
					break;
				}
			}
		}

		static TokenType LookAhead(char[] json, int index)
		{
			var saveIndex = index;
			return NextToken(json, ref saveIndex);
		}

		static TokenType NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);

			if (index == json.Length)
			{
				return TokenType.None;
			}

			var c = json[index++];
			switch (c)
			{
			case '{':
				return TokenType.CurlyOpen;
			case '}':
				return TokenType.CurlyClose;
			case '[':
				return TokenType.SquaredOpen;
			case ']':
				return TokenType.SquaredClose;
			case ',':
				return TokenType.Comma;
			case '"':
				return TokenType.String;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case '-':
				return TokenType.Number;
			case ':':
				return TokenType.Colon;
			}
			index--;

			var remainingLength = json.Length - index;

			// false
			if (remainingLength >= 5)
			{
				if (json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
				{
					index += 5;
					return TokenType.False;
				}
			}

			// true
			if (remainingLength >= 4)
			{
				if (json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
				{
					index += 4;
					return TokenType.True;
				}
			}

			// null
			if (remainingLength >= 4)
			{
				if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
				{
					index += 4;
					return TokenType.Null;
				}
			}

			return TokenType.None;
		}

		/// <summary>
		///   Returns all the fields of a type, working around the fact that reflection
		///   does not return private fields in any other part of the hierarchy than
		///   the exact class GetFields() is called on.
		/// </summary>
		/// <param name="type">Type whose fields will be returned</param>
		/// <param name="bindingFlags">Binding flags to use when querying the fields</param>
		/// <returns>All of the type's fields, including its base types</returns>
		public static FieldInfo[] GetFieldInfosIncludingBaseClasses(this Type type, BindingFlags bindingFlags)
		{
			FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

			// If this class doesn't have a base, don't waste any time
			if (type.BaseType == typeof(object))
			{
				return fieldInfos;
			}
			else
			{ // Otherwise, collect all types up to the furthest base class
				var fieldInfoList = new List<FieldInfo>(fieldInfos);
				while (type.BaseType != typeof(object))
				{
					type = type.BaseType;
					fieldInfos = type.GetFields(bindingFlags);

					// Look for fields we do not have listed yet and merge them into the main list
					for (int index = 0; index < fieldInfos.Length; ++index)
					{
						bool found = false;

						for (int searchIndex = 0; searchIndex < fieldInfoList.Count; ++searchIndex)
						{
							bool match =
								(fieldInfoList[searchIndex].DeclaringType == fieldInfos[index].DeclaringType) &&
								(fieldInfoList[searchIndex].Name == fieldInfos[index].Name);

							if (match)
							{
								found = true;
								break;
							}
						}

						if (!found)
						{
							fieldInfoList.Add(fieldInfos[index]);
						}
					}
				}

				return fieldInfoList.ToArray();
			}
		}

		public static PropertyInfo[] GetPropertyInfosIncludingBaseClasses(this Type type, BindingFlags bindingFlags)
		{
			PropertyInfo[] propertyInfos = type.GetProperties(bindingFlags);

			// If this class doesn't have a base, don't waste any time
			if (type.BaseType == typeof(object))
			{
				return propertyInfos;
			}
			else
			{ // Otherwise, collect all types up to the furthest base class
				var propertyInfoList = new List<PropertyInfo>(propertyInfos);
				while (type.BaseType != typeof(object))
				{
					type = type.BaseType;
					propertyInfos = type.GetProperties(bindingFlags);

					// Look for propertys we do not have listed yet and merge them into the main list
					for (int index = 0; index < propertyInfos.Length; ++index)
					{
						bool found = false;

						for (int searchIndex = 0; searchIndex < propertyInfoList.Count; ++searchIndex)
						{
							bool match =
								(propertyInfoList[searchIndex].DeclaringType == propertyInfos[index].DeclaringType) &&
								(propertyInfoList[searchIndex].Name == propertyInfos[index].Name);

							if (match)
							{
								found = true;
								break;
							}
						}

						if (!found)
						{
							propertyInfoList.Add(propertyInfos[index]);
						}
					}
				}

				return propertyInfoList.ToArray();
			}
		}

		static bool SerializeValue(object value, StringBuilder builder)
		{
			var success = true;

			if (value is string)
			{
				success = SerializeString((string)value, builder);
			}
			else if (value is Hashtable)
			{
				success = SerializeObject((Hashtable)value, builder);
			}
			else if (value is IEnumerable)
			{
				success = SerializeArray((IEnumerable)value, builder);
			}
			else if (value is float)
			{
				success = SerializeNumber(Convert.ToSingle(value), builder);
			}
			else if (value is char || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong)
			{
				success = SerializeNumber(Convert.ToInt64(value), builder);
			}
			else if (value is double)
			{
				success = SerializeNumber(Convert.ToDouble(value), builder);
			}
			else if ((value is Boolean) && ((Boolean)value == true))
			{
				builder.Append("true");
			}
			else if ((value is Boolean) && ((Boolean)value == false))
			{
				builder.Append("false");
			}
			else if (value == null)
			{
				builder.Append("null");
			}
			else if (value is DateTime)
			{
				builder.Append(((DateTime)value).ToString("o"));
			}
			else
			{
				var h = new Hashtable();

				//var fields = value.GetType().GetFields();
				//FieldInfo fi;
				//for (int i = 0; i < fields.Length; ++i)
				foreach (var fi in value.GetType().GetFieldInfosIncludingBaseClasses(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
				{
					//fi = fields[i];
					if (fi.IsNotSerialized)
						continue;
					
					h[fi.Name] = fi.GetValue(value);
				}

				//var properties = value.GetType().GetProperties();
				//PropertyInfo pi;
				//for (int i = 0; i < properties.Length; ++i)
				foreach (var pi in value.GetType().GetPropertyInfosIncludingBaseClasses(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
				{
					if (!pi.CanRead || !pi.CanWrite)
						continue;
					
					//pi = properties[i];
					h[pi.Name] = pi.GetValue(value, null);
				}

				SerializeObject(h, builder);
			}
			return success;
		}

		static bool SerializeObject(Hashtable anObject, StringBuilder builder)
		{
			builder.Append("{");
			IDictionaryEnumerator e = anObject.GetEnumerator();
			var first = true;
			while (e.MoveNext())
			{
				var key = e.Key.ToString();
				var value = e.Value;

				if (value is UnityEngine.Vector2 || value is UnityEngine.Vector3 || value is UnityEngine.Vector4 ||
				    value is UnityEngine.Color || value is UnityEngine.Color32 || value is UnityEngine.Quaternion)
				{
					/* DO NOTHING */
					continue;
				}

				if (!first)
				{
					builder.Append(", ");
				}
				SerializeString(key, builder);
				builder.Append(":");
				if (!SerializeValue(value, builder))
				{
					return false;
				}
				first = false;
			}
			builder.Append("}");
			return true;
		}

		static bool SerializeArray(IEnumerable anArray, StringBuilder builder)
		{
			builder.Append("[");
			var first = true;
			foreach (var value in anArray)
			{
				if (!first)
				{
					builder.Append(", ");
				}
				if (!SerializeValue(value, builder))
				{
					return false;
				}
				first = false;
			}
			builder.Append("]");
			return true;
		}

		static bool SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");

			var charArray = aString.ToCharArray();
			for (var i = 0; i < charArray.Length; i++)
			{
				var c = charArray[i];
				if (c == '"')
				{
					builder.Append("\\\"");
				}
				else if (c == '\\')
				{
					builder.Append("\\\\");
				}
				else if (c == '\b')
				{
					builder.Append("\\b");
				}
				else if (c == '\f')
				{
					builder.Append("\\f");
				}
				else if (c == '\n')
				{
					builder.Append("\\n");
				}
				else if (c == '\r')
				{
					builder.Append("\\r");
				}
				else if (c == '\t')
				{
					builder.Append("\\t");
				}
				else
				{
					int codepoint = Convert.ToInt32(c);
					if ((codepoint >= 32) && (codepoint <= 126))
					{
						builder.Append(c);
					}
					else
					{
						builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
					}
				}
			}

			builder.Append("\"");
			return true;
		}

		static bool SerializeNumber(int number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}

		static bool SerializeNumber(float number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}

		static bool SerializeNumber(long number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}

		static bool SerializeNumber(double number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}

		/// <summary>
		/// Determines if a given object is numeric in any way
		/// (can be integer, double, null, etc). 
		/// 
		/// Thanks to mtighe for pointing out Double.TryParse to me.
		/// </summary>
		static bool IsNumeric(object o)
		{
			float result;

			return (o != null) && float.TryParse(o.ToString(), out result);
		}
	}
}
