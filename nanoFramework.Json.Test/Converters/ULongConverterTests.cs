using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ULongConverterTests
    {
        [TestMethod]
        [DataRow("120", 120, typeof(ulong))]
        [DataRow("45", 45, typeof(ulong))]
        public void ToType_ShouldReturnValidData(string value, ulong expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.ULongConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((ulong)120, "120")]
        [DataRow((ulong)45, "45")]
        public void ToJson_Should_ReturnValidData(ulong value, string expectedValue)
        {
            var converter = new Json.Converters.ULongConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}