﻿//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class ByteConverterTests
    {
        [TestMethod]
        [DataRow("120", (byte)120)]
        [DataRow("42", (byte)42)]
        public void ByteConverter_ToType_ShouldReturnValidData(string value, byte expectedValue)
        {
            var converter = new Json.Converters.ByteConverter();
            var convertedValue = (byte)converter.ToType(value);

            Assert.Equal(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow((byte)120, "120")]
        [DataRow((byte)42, "42")]
        public void ByteConverter_ToJson_Should_ReturnValidData(byte value, string expectedValue)
        {
            var converter = new Json.Converters.ByteConverter();
            var convertedValue = converter.ToJson(value);

            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
