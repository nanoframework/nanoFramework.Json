using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class StringConverterTests
    {
        [TestMethod]
        [DataRow("\"TestJson\"", "TestJson", typeof(string))]
        public void ToType_ShouldReturnValidData(string value, string expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow("TestJson", "\"TestJson\"")]
        public void ToJson_Should_ReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}