using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class SByteConverterTests
    {
        [TestMethod]
        [DataRow("120", (byte)120, typeof(sbyte))]
        [DataRow("42", (byte)42, typeof(sbyte))]
        public void ToType_ShouldReturnValidData(string value, sbyte expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.SByteConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((sbyte)120, "120")]
        [DataRow((sbyte)42, "42")]
        public void ToJson_Should_ReturnValidData(sbyte value, string expectedValue)
        {
            var converter = new Json.Converters.SByteConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}