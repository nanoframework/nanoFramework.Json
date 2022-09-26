using System;
using System.Text;

namespace nanoFramework.Json.Converters
{
    internal sealed class DateTimeConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            return "\"" + DateTimeExtensions.ToIso8601((DateTime)value) + "\"";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            // Not sure hwo this one should work
            throw new NotImplementedException();
        }
    }
}
