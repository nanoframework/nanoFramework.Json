using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class UIntConverterTests
    {
        [TestMethod]
        [DataRow("120", 120)]
        [DataRow("45", 45)]
        public void ToType_ShouldReturnValidData(string value, uint expectedValue)
        {
            var converter = new Json.Converters.UIntConverter();
            var convertedValue = (uint)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((uint)120, "120")]
        [DataRow((uint)45, "45")]
        public void ToJson_Should_ReturnValidData(uint value, string expectedValue)
        {
            var converter = new Json.Converters.UIntConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}