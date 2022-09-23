using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class UIntConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToUInt32(value.ToString());
        }
    }
}