using System;
using System.Text;

namespace nanoFramework.Json.Converters
{
    internal sealed class GuidConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            return "\"" + value.ToString() + "\"";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            return new Guid(value.ToString());
        }
    }
}
