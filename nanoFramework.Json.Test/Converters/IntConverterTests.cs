using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class IntConverterTests
    {
        [TestMethod]
        [DataRow("120", 120)]
        [DataRow("45", 45)]
        public void IntConverter_ToType_ShouldReturnValidData(string value, int expectedValue)
        {
            var converter = new Json.Converters.IntConverter();
            var convertedValue = (int)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow(120, "120")]
        [DataRow(45, "45")]
        public void IntConverter_ToJson_Should_ReturnValidData(int value, string expectedValue)
        {
            var converter = new Json.Converters.IntConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
