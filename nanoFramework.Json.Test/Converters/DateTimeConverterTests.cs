//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class DateTimeConverterTests
    {
        [TestMethod]
        [DataRow(0, "\"1970-01-01T00:00:00.0000000Z\"")]
        public void DateTimeConverter_BoolConverter_ToJson_Should_ReturnValidData(int value, string expectedValue)
        {
            var converter = new Json.Converters.DateTimeConverter();
            var convertedValue = converter.ToJson(DateTime.FromUnixTimeSeconds(value));

            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}
