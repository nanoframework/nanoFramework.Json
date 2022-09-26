using System;
using System.Text;

namespace nanoFramework.Json.Converters
{
    /// <summary>
    /// Interface for all JSON converters.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Converts JSON string to object.
        /// </summary>
        /// <param name="value">JSON value to convert from.</param>
        /// <returns>Object converted to type.</returns>
        object ToType(object value);

        /// <summary>
        /// Converts object into JSON string.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <returns>String with JSON value.</returns>
        string ToJson(object value);
    }
}
