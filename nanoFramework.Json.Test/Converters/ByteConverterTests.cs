using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ByteConverterTests
    {
        [TestMethod]
        [DataRow("120", (byte)120, typeof(byte))]
        [DataRow("42", (byte)42, typeof(byte))]
        public void ToType_ShouldReturnValidData(string value, bool expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.ByteConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((byte)120, "120")]
        [DataRow((byte)42, "42")]
        public void ToJson_Should_ReturnValidData(bool value, string expectedValue)
        {
            var converter = new Json.Converters.ByteConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}