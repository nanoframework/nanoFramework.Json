using System;
using System.Collections;

namespace nanoFramework.Json.Converters
{
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
            { typeof(TimeSpan), new TimeSpanConverter() }
        };

        public static void Add(Type type, IConverter converter)
        {
            ConversionTable.Add(type, converter);
        }

        public static void Remove(Type type)
        {
            ConversionTable.Remove(type);
        }

        public static void Replace(Type type, IConverter converter)
        {
            ConversionTable.Remove(type);
            ConversionTable.Add(type, converter);
        }

        // TODO: Tests for this class
        // TODO: Tests if inject new type (class), we will be able to serizalize from other type
    }
}
