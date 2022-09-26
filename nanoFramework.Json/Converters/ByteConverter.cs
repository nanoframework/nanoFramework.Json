using System;
using System.ComponentModel;

namespace nanoFramework.Json.Converters
{
    internal sealed class ByteConverter : IConverter
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
            return Convert.ToByte(value.ToString());
        }
    }
}