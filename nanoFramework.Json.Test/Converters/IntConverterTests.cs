using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class IntConverterTests
    {
        [TestMethod]
        [DataRow("120", 120, typeof(int))]
        [DataRow("45", 45, typeof(int))]
        public void ToType_ShouldReturnValidData(string value, int expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.IntConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow(120, "120")]
        [DataRow(45, "45")]
        public void ToJson_Should_ReturnValidData(int value, string expectedValue)
        {
            var converter = new Json.Converters.IntConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}