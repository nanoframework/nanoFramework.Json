using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ULongConverterTests
    {
        [TestMethod]
        [DataRow("120", 120)]
        [DataRow("45", 45)]
        public void ToType_ShouldReturnValidData(string value, ulong expectedValue)
        {
            var converter = new Json.Converters.ULongConverter();
            var convertedValue = (ulong)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((ulong)120, "120")]
        [DataRow((ulong)45, "45")]
        public void ToJson_Should_ReturnValidData(ulong value, string expectedValue)
        {
            var converter = new Json.Converters.ULongConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}