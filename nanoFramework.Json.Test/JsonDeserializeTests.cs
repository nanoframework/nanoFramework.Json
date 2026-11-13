//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System.IO;
using System.Text;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonDeserializeTests
    {
        private const string LongUtf8Text =
            "NanoFramework UTF-8 payload: äöü ÄÖÜ éèê ñ ç 漢字 ąćęłńóśźż ❤ " +
            "repeated to force the stream buffer to refill while decoding multi-byte characters. ";

        public enum DeserializeSource
        {
            String,
            Stream,
            StreamReader
        }

        private class DeserializeTarget
        {
            public const string JsonPayload = "{\"Id\":42,\"Name\":\"Nano Framework\",\"IsActive\":true,\"Scores\":[1,3,5,7,9]}";

            public int Id { get; set; }

            public string Name { get; set; }

            public bool IsActive { get; set; }

            public int[] Scores { get; set; }
        }

        private class LargeDeserializeTarget
        {
            public const string JsonPayload =
                "{\"Id\":99,\"Description\":\"" +
                LongUtf8Text +
                LongUtf8Text +
                LongUtf8Text +
                LongUtf8Text +
                LongUtf8Text +
                LongUtf8Text +
                LongUtf8Text +
                LongUtf8Text +
                "\"}";

            public int Id { get; set; }

            public string Description { get; set; }
        }

        private class RpcResponse
        {
            public string CorrelationId { get; set; }

            public int Status { get; set; }

            public string Result { get; set; }
        }

        private class Product
        {
            public int ProductId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public int Price { get; set; }

            public int LiterCount { get; set; }
        }

        private class StringTarget
        {
            public string Value { get; set; }
        }

        [TestMethod]
        [DataRow(DeserializeSource.String)]
        [DataRow(DeserializeSource.Stream)]
        [DataRow(DeserializeSource.StreamReader)]
        public void DeserializeObject_Should_ReturnValidReferenceObject(DeserializeSource source)
        {
            var deserialized = (DeserializeTarget)Deserialize(source, DeserializeTarget.JsonPayload, typeof(DeserializeTarget));

            Assert.AreEqual(42, deserialized.Id);
            Assert.AreEqual("Nano Framework", deserialized.Name);
            Assert.AreEqual(true, deserialized.IsActive);
            Assert.IsNotNull(deserialized.Scores);
            CollectionAssert.AreEqual(new[] { 1, 3, 5, 7, 9 }, deserialized.Scores);
        }

        [TestMethod]
        [DataRow(DeserializeSource.String)]
        [DataRow(DeserializeSource.Stream)]
        [DataRow(DeserializeSource.StreamReader)]
        public void DeserializeObject_Should_HandleLargeUtf8PayloadAndBufferRefill(DeserializeSource source)
        {
            var deserialized = (LargeDeserializeTarget)Deserialize(source, LargeDeserializeTarget.JsonPayload, typeof(LargeDeserializeTarget));

            Assert.AreEqual(99, deserialized.Id);
            Assert.AreEqual(
                LongUtf8Text + LongUtf8Text + LongUtf8Text + LongUtf8Text + LongUtf8Text + LongUtf8Text + LongUtf8Text + LongUtf8Text,
                deserialized.Description);
        }

        [TestMethod]
        public void DeserializeObject_Should_PreserveDigitFollowingUnicodeEscape()
        {
            const string message =
                "{\"CorrelationId\":\"48c972d2-787c-4151-96ab-70eb26cd21b8\",\"Status\":1," +
                "\"Result\":\"{\\u0022ProductId\\u0022:1,\\u0022Name\\u0022:\\u0022A 5-liter bottle\\u0022," +
                "\\u0022Description\\u0022:\\u00225 liters\\u0022,\\u0022Price\\u0022:100,\\u0022LiterCount\\u0022:5}\"}";
            const string expectedResult =
                "{\"ProductId\":1,\"Name\":\"A 5-liter bottle\",\"Description\":\"5 liters\",\"Price\":100,\"LiterCount\":5}";

            foreach (DeserializeSource source in new[] { DeserializeSource.String, DeserializeSource.Stream, DeserializeSource.StreamReader })
            {
                var response = (RpcResponse)Deserialize(source, message, typeof(RpcResponse));

                Assert.AreEqual(expectedResult, response.Result);

                var product = (Product)JsonConvert.DeserializeObject(response.Result, typeof(Product));
                Assert.AreEqual("5 liters", product.Description);
                Assert.AreEqual("A 5-liter bottle", product.Name);
            }
        }

        [TestMethod]
        public void DeserializeObject_Should_DecodeUnicodeEscapesWithoutConsumingAdjacentCharacters()
        {
            string[] jsonValues = { "\\u00355", "a\\u0041F", "\\u00e9", "\\u0041\\u0035" };
            string[] expectedValues = { "55", "aAF", "\u00e9", "A5" };

            foreach (DeserializeSource source in new[] { DeserializeSource.String, DeserializeSource.Stream, DeserializeSource.StreamReader })
            {
                for (int i = 0; i < jsonValues.Length; i++)
                {
                    string json = "{\"Value\":\"" + jsonValues[i] + "\"}";
                    var result = (StringTarget)Deserialize(source, json, typeof(StringTarget));

                    Assert.AreEqual(expectedValues[i], result.Value);
                }
            }
        }

        private static object Deserialize(DeserializeSource source, string json, System.Type type)
        {
            if (source == DeserializeSource.String)
            {
                return JsonConvert.DeserializeObject(json, type);
            }

            var bytes = Encoding.UTF8.GetBytes(json);
            if (source == DeserializeSource.Stream)
            {
                using var stream = new MemoryStream(bytes);
                return JsonConvert.DeserializeObject(stream, type);
            }

            if (source == DeserializeSource.StreamReader)
            {
                using var stream = new MemoryStream(bytes);
                using var reader = new StreamReader(stream);
                return JsonConvert.DeserializeObject(reader, type);
            }

            throw new System.InvalidOperationException();
        }
    }
}
