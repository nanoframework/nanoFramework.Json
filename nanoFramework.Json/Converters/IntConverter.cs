using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class IntConverter : IConverter
    {
        public string ToJson(object value)
        {
            return value.ToString();
        }

        public object ToType(object value)
        {
            return Convert.ToInt32(value.ToString());
        }
    }
}