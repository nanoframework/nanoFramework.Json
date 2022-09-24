using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class LongConverterTests
    {
        [TestMethod]
        [DataRow("120", (long)120, typeof(long))]
        [DataRow("45", (long)45, typeof(long))]
        public void ToType_ShouldReturnValidData(string value, long expectedValue, Type expectedType)
        {
            var converter = new Json.Converters.LongConverter();
            var convertedValue = converter.ToType(value);

            Assert.Equals(expectedValue, convertedValue);
            Assert.Equals(convertedValue.GetType(), expectedType);
        }

        [TestMethod]
        [DataRow((long)120, "120")]
        [DataRow((long)45, "45")]
        public void ToJson_Should_ReturnValidData(long value, string expectedValue)
        {
            var converter = new Json.Converters.LongConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equals(expectedValue, convertedValue);
        }
    }
}