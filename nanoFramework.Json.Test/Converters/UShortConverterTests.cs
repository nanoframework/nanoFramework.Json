//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class UShortConverterTests
    {
        [TestMethod]
        [DataRow("120", (ushort)120)]
        [DataRow("45", (ushort)45)]
        public void UShortConverter_ToType_ShouldReturnValidData(string value, ushort expectedValue)
        {
            var converter = new Json.Converters.UShortConverter();
            var convertedValue = (ushort)converter.ToType(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((ushort)120, "120")]
        [DataRow((ushort)45, "45")]
        public void UShortConverter_ToJson_Should_ReturnValidData(ushort value, string expectedValue)
        {
            var converter = new Json.Converters.UShortConverter();
            var convertedValue = converter.ToJson(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}
