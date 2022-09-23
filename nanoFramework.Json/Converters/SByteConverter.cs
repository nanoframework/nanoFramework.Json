using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class SByteConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToSByte(value.ToString());
        }
    }
}