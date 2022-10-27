//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ULongConverterTests
    {
        [TestMethod]
        [DataRow("120", (ulong)120)]
        [DataRow("45", (ulong)45)]
        public void ULongConverter_ToType_ShouldReturnValidData(string value, ulong expectedValue)
        {
            var converter = new Json.Converters.ULongConverter();
            var convertedValue = (ulong)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((ulong)120, "120")]
        [DataRow((ulong)45, "45")]
        public void ULongConverter_ToJson_Should_ReturnValidData(ulong value, string expectedValue)
        {
            var converter = new Json.Converters.ULongConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
