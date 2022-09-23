using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class ULongConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToUInt64(value.ToString());
        }
    }
}