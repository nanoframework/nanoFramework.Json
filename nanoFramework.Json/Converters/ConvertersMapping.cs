using System;
using System.Collections;

namespace nanoFramework.Json.Converters
{
    /// <summary>
    /// Contains all converters for JSON.
    /// </summary>
    public static class ConvertersMapping
    {
        internal static readonly Hashtable ConversionTable = new Hashtable()
        {
            { typeof(short), new ShortConverter() },
            { typeof(ushort), new UShortConverter() },
            { typeof(int), new IntConverter() },
            { typeof(uint), new UIntConverter() },
            { typeof(long), new LongConverter() },
            { typeof(ulong), new ULongConverter() },
            { typeof(byte), new ByteConverter() },
            { typeof(sbyte), new SByteConverter() },
            { typeof(float), new FloatConverter()},
            { typeof(double), new DoubleConverter() },
            { typeof(bool), new BoolConverter() },
            { typeof(string), new StringConverter() },
            { typeof(TimeSpan), new TimeSpanConverter() },
            { typeof(DateTime), new DateTimeConverter() },
            { typeof(char), new CharConverter() },
            { typeof(Guid), new GuidConverter() },
            { typeof(DictionaryEntry), new DictionaryEntryConverter() }
        };

        /// <summary>
        /// Adds new converter to collection to support more types.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/></param>
        public static void Add(Type type, IConverter converter)
        {
            ConversionTable.Add(type, converter);
        }

        /// <summary>
        /// Remove existing type converter.
        /// </summary>
        /// <param name="type">Type of object.</param>
        public static void Remove(Type type)
        {
            ConversionTable.Remove(type);
        }

        /// <summary>
        /// Remove and then adds converter for given type.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="converter">Converter instance which will be used to convert <paramref name="type"/></param>
        public static void Replace(Type type, IConverter converter)
        {
            ConversionTable.Remove(type);
            ConversionTable.Add(type, converter);
        }

        // TODO: Tests if inject new type (class), we will be able to serizalize from other type
    }
}
