using nanoFramework.Json.Converters;
using nanoFramework.TestFramework;
using System;
using System.Text;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonCustomTypeTests
    {
        private class TestObject
        {
            public int Value { get; set; }
        }


        private class CustomConverter : IConverter
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
        public void Setup()
        {
            ConvertersMapping.Add(typeof(TestObject), new CustomConverter());
        }

        [Cleanup]
        public void CleanUp()
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
