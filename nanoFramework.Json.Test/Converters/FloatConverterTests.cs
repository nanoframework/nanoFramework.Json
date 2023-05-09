//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class FloatConverterTests
    {
        [TestMethod]
        [DataRow("120.0", 120.0f)]
        [DataRow("42.5", 42.5f)]
        public void FloatConverter_ToType_ShouldReturnValidData(string value, float expectedValue)
        {
            var converter = new Json.Converters.FloatConverter();
            var convertedValue = (float)converter.ToType(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void FloatConverter_ToType_NullShouldReturnNaN()
        {
            var converter = new Json.Converters.FloatConverter();
            var convertedValue = (float)converter.ToType(null);

            Assert.AreEqual(float.NaN.ToString(), convertedValue.ToString());
        }

        [TestMethod]
        [DataRow(120.5f, "120.5")]
        [DataRow(42.5f, "42.5")]
        [DataRow(float.NaN, "null")]
        public void FloatConverter_ToJson_Should_ReturnValidData(float value, string expectedValue)
        {
            var converter = new Json.Converters.FloatConverter();
            var convertedValue = converter.ToJson(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}
