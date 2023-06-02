//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ShortConverterTests
    {
        [TestMethod]
        [DataRow("120", (short)120)]
        [DataRow("45", (short)45)]
        public void ShortConverter_ToType_ShouldReturnValidData(string value, short expectedValue)
        {
            var converter = new Json.Converters.ShortConverter();
            var convertedValue = (short)converter.ToType(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((short)120, "120")]
        [DataRow((short)45, "45")]
        public void ShortConverter_ToJson_Should_ReturnValidData(short value, string expectedValue)
        {
            var converter = new Json.Converters.ShortConverter();
            var convertedValue = converter.ToJson(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}
