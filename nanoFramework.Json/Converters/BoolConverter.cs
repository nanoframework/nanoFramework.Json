using System;
using System.ComponentModel;

namespace nanoFramework.Json.Converters
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class BoolConverter : IConverter
    {
        public string ToJson(object value)
        {
            return (bool)value ? "true" : "false";
        }

        public object ToType(object value)
        {
            return Convert.ToBoolean(Convert.ToByte(value.ToString()));
        }
    }
}