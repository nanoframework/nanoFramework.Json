using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class FloatConverter : IConverter
    {
        public string ToJson(object value)
        {
            if (float.IsNaN((float)value))
            {
                return "null";
            }

            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToSingle(value.ToString());
        }
    }
}