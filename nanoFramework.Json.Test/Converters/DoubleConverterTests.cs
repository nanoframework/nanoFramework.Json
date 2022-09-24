using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class DoubleConverterTests
    {
        [TestMethod]
        [DataRow("120.0", 120.0, typeof(double))]
        [DataRow("42.5", 42.5, typeof(double))]
        public void ToType_ShouldReturnValidData(string value, double expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow(120.0, "120.0")]
        [DataRow(42.5, "42.5")]
        [DataRow(double.NaN, "null")]
        public void ToJson_Should_ReturnValidData(double value, string expectedValue)
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}