using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class TimeSpanConverterTests
    {
        [TestMethod]
        [DataRow("10:00:00", 10)]
        [DataRow("24:00:00", 24)]
        public void TimeSpanConverter_ToType_ShouldReturnValidData(string value, int expectedValueHours)
        {
            var converter = new Json.Converters.TimeSpanConverter();
            var convertedValue = (TimeSpan)converter.ToType(value);

            var expectedTimeSpanValue = TimeSpan.FromHours(expectedValueHours);
            Assert.Equal(expectedTimeSpanValue.Ticks, convertedValue.Ticks);
        }

        [TestMethod]
        [DataRow(10, "\"10:00:00\"")]
        [DataRow(24, "\"24:00:00\"")]
        public void TimeSpanConverter_ToJson_Should_ReturnValidData(int valueHours, string expectedValue)
        {
            var converter = new Json.Converters.TimeSpanConverter();
            var convertedValue = converter.ToJson(TimeSpan.FromHours(valueHours));

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
