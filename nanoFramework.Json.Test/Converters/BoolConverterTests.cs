//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class BoolConverterTests
    {
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void BoolConverter_ToType_ShouldReturnValidData(object value, bool expectedValue)
        {
            var converter = new Json.Converters.BoolConverter();
            var convertedValue = (bool)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void BoolConverter_ToJson_Should_ReturnValidData(bool value, string expectedValue)
        {
            var converter = new Json.Converters.BoolConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
