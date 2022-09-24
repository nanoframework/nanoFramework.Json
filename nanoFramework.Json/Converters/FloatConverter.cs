using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class FloatConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            if (float.IsNaN((float)value))
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
            return Convert.ToSingle(value.ToString());
        }
    }
}