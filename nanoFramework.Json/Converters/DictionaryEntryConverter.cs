//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;

namespace nanoFramework.Json.Converters
{
    internal sealed class DictionaryEntryConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            Hashtable hashtable = new();

            var entry = (DictionaryEntry)value;
            hashtable.Add(entry.Key, entry.Value);

            return JsonSerializer.SerializeDictionary(hashtable);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            throw new NotImplementedException();
        }
    }
}
