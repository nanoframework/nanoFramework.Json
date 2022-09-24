using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class UShortConverterTests
    {
        [TestMethod]
        [DataRow("120", (ushort)120, typeof(ushort))]
        [DataRow("45", (ushort)45, typeof(ushort))]
        public void ToType_ShouldReturnValidData(string value, ushort expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.UShortConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((ushort)120, "120")]
        [DataRow((ushort)45, "45")]
        public void ToJson_Should_ReturnValidData(ushort value, string expectedValue)
        {
            var converter = new Json.Converters.UShortConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}