using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class BoolConverterTests
    {
        /*[TestMethod]
        [DataRow("true", true)]
        [DataRow("false", false)]
        public void ToType_ShouldReturnValidData(string value, bool expectedValue)
        {
            var converter = new Json.Converters.BoolConverter();
            var convertedValue = (bool)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }*/

        [TestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void ToJson_Should_ReturnValidData(bool value, string expectedValue)
        {
            var converter = new Json.Converters.BoolConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}