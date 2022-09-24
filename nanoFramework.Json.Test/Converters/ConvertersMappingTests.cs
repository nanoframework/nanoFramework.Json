using nanoFramework.Json.Converters;
using nanoFramework.TestFramework;
using System;
using System.Text;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ConvertersMappingTests
    {
        class TestConverter : IConverter
        {
            public string ToJson(object value)
            {
                throw new NotImplementedException();
            }

            public object ToType(object value)
            {
                throw new NotImplementedException();
            }
        }

        class TestConverter2 : IConverter
        {
            public string ToJson(object value)
            {
                throw new NotImplementedException();
            }

            public object ToType(object value)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void Add_Should_AddTypeMapping()
        {
            ConvertersMapping.Add(typeof(IConverter), new TestConverter());

            var converter = ConvertersMapping.ConversionTable[typeof(IConverter)];
            Assert.NotNull(converter);

            if (converter.GetType() != typeof(TestConverter))
            {
                throw new InvalidOperationException("Invalid type returned.");
            }
        }

        [TestMethod]
        public void Remove_Should_RemoveTypeMapping()
        {
            ConvertersMapping.Add(typeof(IConverter), new TestConverter());
            ConvertersMapping.Remove(typeof(IConverter));

            var converterKeys = ConvertersMapping.ConversionTable.Keys;
            foreach (var item in converterKeys)
            {
                var type = (Type)item;
                if (type == typeof(IConverter))
                {
                    throw new InvalidOperationException("After removing short type, it should not be in collection.");
                }
            }
        }

        [TestMethod]
        public void Replace_ShouldReplaceMapping()
        {
            ConvertersMapping.Add(typeof(IConverter), new TestConverter());
            ConvertersMapping.Replace(typeof(IConverter), new TestConverter2());

            var converter = ConvertersMapping.ConversionTable[typeof(short)];
            Assert.NotNull(converter);

            if (converter.GetType() != typeof(TestConverter2))
            {
                throw new InvalidOperationException("Invalid type returned.");
            }
        }
    }
}
