using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class UIntConverterTests
    {
        [TestMethod]
        [DataRow("120", 120, typeof(uint))]
        [DataRow("45", 45, typeof(uint))]
        public void ToType_ShouldReturnValidData(string value, uint expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.UIntConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((uint)120, "120")]
        [DataRow((uint)45, "45")]
        public void ToJson_Should_ReturnValidData(uint value, string expectedValue)
        {
            var converter = new Json.Converters.UIntConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}