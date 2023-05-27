//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using nanoFramework.Json.Converters;
using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Configuration
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
        public void ConvertersMappingT_Add_Should_AddTypeMapping()
        {
            ConvertersMapping.Add(typeof(IConverter), new TestConverter());

            var converter = ConvertersMapping.GetConverter(typeof(IConverter));
            Assert.IsNotNull(converter);

            if (converter.GetType() != typeof(TestConverter))
            {
                throw new InvalidOperationException("Invalid type returned.");
            }
        }

        [TestMethod]
        public void ConvertersMappingT_Remove_Should_RemoveTypeMapping()
        {
            ConvertersMapping.Add(typeof(TestConverter), new TestConverter());
            ConvertersMapping.Remove(typeof(TestConverter));

            var converter = ConvertersMapping.GetConverter(typeof(TestConverter));
            Assert.IsNull(converter);
        }

        [TestMethod]
        public void ConvertersMappingT_Replace_ShouldReplaceMapping()
        {
            ConvertersMapping.Add(typeof(TestConverter2), new TestConverter());
            ConvertersMapping.Replace(typeof(TestConverter2), new TestConverter2());

            var converter = ConvertersMapping.GetConverter(typeof(TestConverter2));
            Assert.IsNotNull(converter);

            if (converter.GetType() != typeof(TestConverter2))
            {
                throw new InvalidOperationException("Invalid type returned.");
            }
        }

        [Cleanup]
        public void Cleanup()
        {
            ConvertersMapping.Remove(typeof(IConverter));
            ConvertersMapping.Remove(typeof(TestConverter2));
        }
    }
}
