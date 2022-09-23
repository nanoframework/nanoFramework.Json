using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class UShortConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToUInt16(value.ToString());
        }
    }
}