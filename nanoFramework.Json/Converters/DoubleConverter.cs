using System;
using System.ComponentModel;

namespace nanoFramework.Json.Converters
{
    internal sealed class DoubleConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            if (double.IsNaN((double)value))
            {
                return "null";
            }

            return value.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            return Convert.ToDouble(value.ToString());
        }
    }
}