//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

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
            if (value == null)
            {
                return double.NaN;
            }

            return Convert.ToDouble(value.ToString());
        }
    }
}
