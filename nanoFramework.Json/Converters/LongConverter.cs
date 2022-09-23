using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class LongConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToInt64(value.ToString());
        }
    }
}