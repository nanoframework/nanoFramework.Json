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
        [DataRow("\"a\"", "a")]
        [DataRow("\"TestJson\"", "TestJson")]
        [DataRow("TestJson1", "TestJson1")]
        [DataRow("\"Test / solidus\"", "Test / solidus")]
        public void StringConverter_ToType_ShouldReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void StringConverter_ToType_ShouldReturnStringEmptyForNull()
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(null);

            Assert.AreEqual(string.Empty, convertedValue);
        }

        [TestMethod]
        [DataRow("a", "\"a\"")]
        [DataRow("\"TestJson\"", "\"\\\"TestJson\\\"\"")]
        [DataRow("TestJson2", "\"TestJson2\"")]
        [DataRow("Test / solidus", "\"Test / solidus\"")]
        public void StringConverter_ToJson_Should_ReturnValidData(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToJson(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow("Text\\1", "Text\\1")]  // Backslash
        [DataRow("Text\b1", "Text\b1")]  // Backspace
        [DataRow("Text\f1", "Text\f1")]  // FormFeed
        [DataRow("Text\r1", "Text\r1")]  // CarriageReturn
        [DataRow("Text\"1", "Text\"1")]  // DoubleQuote
        [DataRow("Text\n1", "Text\n1")]  // Newline
        [DataRow("Text\t1", "Text\t1")]  // Tab
        [DataRow("['Text3', 1]", "['Text3', 1]")] // Array
        [DataRow("{\"Text1\" : \"/Text1/\"}", "{\"Text1\" : \"/Text1/\"}")] // Json
        [DataRow("ä", "ä")]  // Unicode
        [DataRow("\"I:\\\\nano\\\\rpath\\\\to\"", "I:\\nano\\rpath\\to")]
        public void StringConverter_ToType_Should_HandleSpecialCharacters(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = (string)converter.ToType(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        [DataRow("Text\\1", "\"Text\\\\1\"")]  // Backslash
        [DataRow("Text\b1", "\"Text\\b1\"")]  // Backspace
        [DataRow("Text\f1", "\"Text\\f1\"")]  // FormFeed
        [DataRow("Text\r1", "\"Text\\r1\"")]  // CarriageReturn
        [DataRow("Text\"1", "\"Text\\\"1\"")]  // DoubleQuote
        [DataRow("Text\n1", "\"Text\\n1\"")]  // Newline
        [DataRow("Text\t1", "\"Text\\t1\"")]  // Tab
        [DataRow("ä", "\"ä\"")]  // Unicode
        [DataRow("I:\\nano\\rpath\\to", "\"I:\\\\nano\\\\rpath\\\\to\"")]
        public void StringConverter_ToJson_Should_HandleSpecialCharacters(string value, string expectedValue)
        {
            var converter = new Json.Converters.StringConverter();
            var convertedValue = converter.ToJson(value);

            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}
