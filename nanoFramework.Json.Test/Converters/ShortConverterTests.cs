using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ShortConverterTests
    {
        [TestMethod]
        [DataRow("120", (short)120, typeof(short))]
        [DataRow("45", (short)45, typeof(short))]
        public void ToType_ShouldReturnValidData(string value, short expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.ShortConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((short)120, "120")]
        [DataRow((short)45, "45")]
        public void ToJson_Should_ReturnValidData(short value, string expectedValue)
        {
            var converter = new Json.Converters.ShortConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}