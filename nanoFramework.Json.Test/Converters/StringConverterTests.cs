//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class StringConverterTests
    {
        [TestMethod]
        [DataRow("\"TestJson\"", "\"TestJson\"")]
        [DataRow("TestJson1", "TestJson1")]
        public void StringConverter_ToType_ShouldReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        public void StringConverter_ToType_ShouldReturnStringEmptyForNull()
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(null);

            Assert.Equal(string.Empty, convertedValue);
        }

        [TestMethod]
        [DataRow("TestJson2", "\"TestJson2\"")]
        public void StringConverter_ToJson_Should_ReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
