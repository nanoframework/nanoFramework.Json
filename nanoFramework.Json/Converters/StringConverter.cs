using System.Collections;
using System.Text;

namespace nanoFramework.Json.Converters
{
    internal sealed class StringConverter : IConverter
    {
        internal static Hashtable EscapableCharactersMapping = new Hashtable()
        {
            {'\n', 'n'},
            {'\r', 'r'},
            {'\"', '"' }
        };

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
                if (str.IndexOf(charToCheck) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public object ToType(object value)
        {
            return value.ToString();
        }
    }
}