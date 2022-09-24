using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class TimeSpanConverterTests
    {
        [TestMethod]
        [DataRow("10.00:00:00.0000000", 10, typeof(TimeSpan))]
        public void ToType_ShouldReturnValidData(string value, int expectedValueHours, Type expectedType)
        {
            var converter = new Json.Converters.TimeSpanConverter();
            var convertedValue = converter.ToType(value);

            var expectedTimeSpanValue = TimeSpan.FromHours(expectedValueHours);
            Assert.Equals(expectedTimeSpanValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow(10, "10.00:00:00.0000000")]
        public void ToJson_Should_ReturnValidData(int valueHours, string expectedValue)
        {
            var converter = new Json.Converters.TimeSpanConverter();
            var convertedValue = converter.ToJson(TimeSpan.FromHours(valueHours));

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}