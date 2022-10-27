//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using nanoFramework.Json.Converters;
using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Configuration
{
    [TestClass]
    public class JsonCustomTypeTests
    {
        private sealed class TestObject
        {
            public int Value { get; set; }
        }


        private sealed class CustomConverter : IConverter
        {
            public string ToJson(object value)
            {
                return "123";
            }

            public object ToType(object value)
            {
                return new TestObject() { Value = 321 };
            }
        }

        [Setup]
        public void JsonCustomTypeTests_Setup()
        {
            ConvertersMapping.Add(typeof(TestObject), new CustomConverter());
        }

        [Cleanup]
        public void JsonCustomTypeTests_CleanUp()
        {
            ConvertersMapping.Remove(typeof(TestObject));
        }

        [TestMethod]
        public void CustomMapping_Should_SerializeToGivenValue()
        {
            var obj = new TestObject() { Value = 5 };

            var value = JsonConvert.SerializeObject(obj);

            Assert.Equal(value, "123");
        }

        [TestMethod]
        public void CustomMapping_Should_DeserializeToGivenValue()
        {
            var obj = (TestObject)JsonConvert.DeserializeObject("{\"TestObject\" : \"whatever\"}", typeof(TestObject));

            Assert.Equal(obj.Value, 321);
        }
    }
}
