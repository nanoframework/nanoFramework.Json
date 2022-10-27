//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json.Converters
{
    internal sealed class BoolConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            return (bool)value ? "true" : "false";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            return (bool)value;
        }
    }
}
