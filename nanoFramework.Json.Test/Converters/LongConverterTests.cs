//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class LongConverterTests
    {
        [TestMethod]
        [DataRow("120", (long)120)]
        [DataRow("45", (long)45)]
        public void LongConverter_ToType_ShouldReturnValidData(string value, long expectedValue)
        {
            var converter = new Json.Converters.LongConverter();
            var convertedValue = (long)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((long)120, "120")]
        [DataRow((long)45, "45")]
        public void LongConverter_ToJson_Should_ReturnValidData(long value, string expectedValue)
        {
            var converter = new Json.Converters.LongConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
