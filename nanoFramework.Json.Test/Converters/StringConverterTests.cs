﻿using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class StringConverterTests
    {
        [TestMethod]
        [DataRow("\"TestJson\"", "TestJson")]
        public void StringConverter_ToType_ShouldReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow("TestJson", "\"TestJson\"")]
        public void StringConverter_ToJson_Should_ReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
