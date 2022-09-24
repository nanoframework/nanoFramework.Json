using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class ShortConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            return value.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            return Convert.ToInt16(value.ToString());
        }
    }
}