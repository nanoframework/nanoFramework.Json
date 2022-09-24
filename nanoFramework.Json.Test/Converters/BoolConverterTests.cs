using nanoFramework.TestFramework;
using System;
using System.Text;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class BoolConverterTests
    {
        [TestMethod]
        [DataRow("true", true, typeof(bool))]
        [DataRow("false", false, typeof(bool))]
        public void ToType_ShouldReturnValidData(string value, bool expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.BoolConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void ToJson_Should_ReturnValidData(bool value, string expectedValue)
        {
            var converter = new Json.Converters.BoolConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}