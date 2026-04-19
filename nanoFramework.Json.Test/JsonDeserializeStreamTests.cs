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
    public class JsonDeserializeStreamTests
    {
        private class DeserializeTarget
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public bool IsActive { get; set; }

            public int[] Scores { get; set; }
        }

        [TestMethod]
        [DataRow(false)] // Deserialize from string
        [DataRow(true)] // Deserialize from stream
        public void Can_deserialize_the_same_payload_from_string_and_stream(bool useStream)
        {
            var source = new DeserializeTarget
            {
                Id = 42,
                Name = "Nano Framework",
                IsActive = true,
                Scores = new[] { 1, 3, 5, 7, 9 }
            };

            var json = JsonConvert.SerializeObject(source);
            var deserialized = (DeserializeTarget)Deserialize(useStream, json, typeof(DeserializeTarget));

            Assert.AreEqual(source.Id, deserialized.Id);
            Assert.AreEqual(source.Name, deserialized.Name);
            Assert.AreEqual(source.IsActive, deserialized.IsActive);
            Assert.IsNotNull(deserialized.Scores);
            CollectionAssert.AreEqual(source.Scores, deserialized.Scores);
        }

        private static object Deserialize(bool useStream, string json, System.Type type)
        {
            if (useStream)
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                using (var stream = new MemoryStream(bytes))
                {
                    return JsonConvert.DeserializeObject(stream, type);
                }
            }

            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
