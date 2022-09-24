using System;
using System.ComponentModel;

namespace nanoFramework.Json.Converters
{
    internal sealed class ByteConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToByte(value.ToString());
        }
    }
}