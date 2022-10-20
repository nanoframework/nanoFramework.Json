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
        public void DoubleConverter_ToType_ShouldReturnValidData(string value, double expectedValue)
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = (double)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        public void DoubleConverter_ToType_NullShouldReturnNaN()
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = (double)converter.ToType(null);

            Assert.Equal(double.NaN.ToString(), convertedValue.ToString());
        }

        [TestMethod]
        [DataRow(120.5, "120.5")]
        [DataRow(42.5, "42.5")]
        [DataRow(double.NaN, "null")]
        public void DoubleConverter_ToJson_Should_ReturnValidData(double value, string expectedValue)
        {
            var converter = new Json.Converters.DoubleConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
