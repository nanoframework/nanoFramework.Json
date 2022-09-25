using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class DoubleConverterTests
    {
        [TestMethod]
        [DataRow("120.0", 120.0)]
        [DataRow("42.5", 42.5)]
        public void ToType_ShouldReturnValidData(string value, double expectedValue)
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = (double)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow(120.6, "120.6")]
        [DataRow(42.5, "42.5")]
        [DataRow(double.NaN, "null")]
        public void ToJson_Should_ReturnValidData(double value, string expectedValue)
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}