using System;
using System.ComponentModel;

namespace nanoFramework.Json.Converters
{
    internal sealed class DoubleConverter : IConverter
    {
        public string ToJson(object value)
        {
            if (double.IsNaN((double)value))
            {
                return "null";
            }

            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToDouble(value.ToString());
        }
    }
}