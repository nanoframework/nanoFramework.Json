//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Converters;
using System;
using System.Collections;

namespace nanoFramework.Json.Configuration
{
    /// <summary>
    /// Contains all converters for JSON.
    /// </summary>
    /// 
    public static class ConvertersMapping
    {
        private static readonly Hashtable ConversionTable = new()
        {
            { typeof(short).FullName, new ShortConverter() },
            { typeof(ushort).FullName, new UShortConverter() },
            { typeof(int).FullName, new IntConverter() },
            { typeof(uint).FullName, new UIntConverter() },
            { typeof(long).FullName, new LongConverter() },
            { typeof(ulong).FullName, new ULongConverter() },
            { typeof(byte).FullName, new ByteConverter() },
            { typeof(sbyte).FullName, new SByteConverter() },
            { typeof(float).FullName, new FloatConverter()},
            { typeof(double).FullName, new DoubleConverter() },
            { typeof(bool).FullName, new BoolConverter() },
            { typeof(string).FullName, new StringConverter() },
            { typeof(TimeSpan).FullName, new TimeSpanConverter() },
            { typeof(DateTime).FullName, new DateTimeConverter() },
            { typeof(char).FullName, new CharConverter() },
            { typeof(Guid).FullName, new GuidConverter() },
            { typeof(DictionaryEntry).FullName, new DictionaryEntryConverter() }
        };

        /// <summary>
        /// Adds new converter to collection to support more types.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/></param>
        public static void Add(Type type, IConverter converter)
        {
            ConversionTable.Add(type.FullName, converter);
        }

        /// <summary>
        /// Remove existing type converter.
        /// </summary>
        /// <param name="type">Type of object.</param>
        public static void Remove(Type type)
        {
            ConversionTable.Remove(type.FullName);
        }

        /// <summary>
        /// Remove and then adds converter for given type.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/></param>
        public static void Replace(Type type, IConverter converter)
        {
            Remove(type);
            Add(type, converter);
        }

        internal static IConverter GetConverter(Type type)
        {
            if (ConversionTable.Contains(type.FullName))
            {
                return (IConverter)ConversionTable[type.FullName];
            }

            return null;
        }
    }
}
