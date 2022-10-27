//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json.Converters
{
    internal sealed class CharConverter : IConverter
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
            return value.ToString()[0];
        }
    }
}
