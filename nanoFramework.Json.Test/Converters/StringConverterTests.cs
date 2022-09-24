using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class StringConverterTests
    {
        [TestMethod]
        [DataRow("\"TestJson\"", "TestJson")]
        public void ToType_ShouldReturnValidData(string value, string expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow("TestJson", "\"TestJson\"")]
        public void ToJson_Should_ReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}