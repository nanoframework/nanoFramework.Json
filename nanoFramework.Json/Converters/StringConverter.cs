using System;
using System.Collections;
using System.Text;

namespace nanoFramework.Json.Converters
{
    internal sealed class StringConverter : IConverter
    {
        internal static readonly Hashtable EscapableCharactersMapping = new Hashtable()
        {
            {'\n', 'n'},
            {'\r', 'r'},
            {'\"', '"' }
        };

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            return "\"" + SerializeString((string)value) + "\"";
        }

        internal static string SerializeString(string str)
        {
            // If the string is just fine (most are) then make a quick exit for improved performance
            if (!StringContainsCharactersToEscape(str, false))
            {
                return str;
            }

            // Build a new string
            // we know there is at least 1 char to escape
            StringBuilder result = new(str.Length + 1);

            foreach (char ch in str)
            {
                var charToAppend = ch;
                if (CheckIfCharIsRequiresEscape(charToAppend))
                {
                    result.Append('\\');
                    charToAppend = (char)EscapableCharactersMapping[charToAppend];
                }

                result.Append(charToAppend);
            }

            return result.ToString();
        }

        internal static bool CheckIfCharIsRequiresEscape(char chr)
        {
            foreach (var item in EscapableCharactersMapping.Keys)
            {
                if ((char)item == chr)
                {
                    return true;
                }
            }

            return false;
        }


        internal static bool StringContainsCharactersToEscape(string str, bool deserializing)
        {
            foreach (var item in EscapableCharactersMapping.Keys)
            {
                var charToCheck = deserializing ? $"\\{EscapableCharactersMapping[item]}" : item.ToString();
                if (str.IndexOf(charToCheck) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            var sourceString = value.ToString();
            //String by default has escaped \" at beggining and end, just remove them
            var resultString = sourceString.Substring(1, sourceString.Length - 2);
            if (StringContainsCharactersToEscape(resultString, true))
            {
                var newString = string.Empty;
                //Last character can not be escaped, because it's last one
                for (int i = 0; i < resultString.Length - 1; i++)
                {
                    var curChar = resultString[i];
                    var nextChar = resultString[i + 1];

                    if (curChar == '\\')
                    {
                        var charToAppend = GetEscapableCharKeyBasedOnValue(nextChar);
                        newString += charToAppend;
                        i++;
                        continue;
                    }
                    newString += curChar;
                }
                //Append last character skkiped by loop
                newString += resultString[resultString.Length - 1];
                return newString.ToString();
            }
            return resultString;
        }

        private static char GetEscapableCharKeyBasedOnValue(char inputChar)
        {
            foreach (var item in EscapableCharactersMapping.Keys)
            {
                var value = (char)EscapableCharactersMapping[item];
                if (value == inputChar)
                {
                    return (char)item;
                }
            }
            // in case inputChar is not supported
            throw new InvalidOperationException();
        }
    }
}