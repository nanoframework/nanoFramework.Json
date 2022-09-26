using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class ULongConverter : IConverter
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
            return Convert.ToUInt64(value.ToString());
        }
    }
}