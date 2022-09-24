using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class FloatConverterTests
    {
        [TestMethod]
        [DataRow("120.0", 120.0f, typeof(float))]
        [DataRow("42.5", 42.5f, typeof(float))]
        public void ToType_ShouldReturnValidData(string value, float expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.FloatConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow(120.0f, "120.5")]
        [DataRow(42.5f, "42.5")]
        [DataRow(float.NaN, "null")]
        public void ToJson_Should_ReturnValidData(float value, string expectedValue)
        {
            var converter = new Json.Converters.FloatConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}