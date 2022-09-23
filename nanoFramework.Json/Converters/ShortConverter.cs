using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class ShortConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToInt16(value.ToString());
        }
    }
}