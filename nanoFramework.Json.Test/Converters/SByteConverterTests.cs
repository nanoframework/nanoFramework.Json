//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class SByteConverterTests
    {
        [TestMethod]
        [DataRow("120", (sbyte)120)]
        [DataRow("42", (sbyte)42)]
        public void SByteConverter_ToType_ShouldReturnValidData(string value, sbyte expectedValue)
        {
            var converter = new Json.Converters.SByteConverter();
            var convertedValue = (sbyte)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((sbyte)120, "120")]
        [DataRow((sbyte)42, "42")]
        public void SByteConverter_ToJson_Should_ReturnValidData(sbyte value, string expectedValue)
        {
            var converter = new Json.Converters.SByteConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
