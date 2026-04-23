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
