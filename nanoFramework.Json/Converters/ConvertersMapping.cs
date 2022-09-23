using System;
using System.Collections;
using System.Text;

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

        // TODO: Add method: Add, Replace, Remove
    }
}
