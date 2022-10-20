using nanoFramework.TestFramework;
using System;
using System.Text;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonDeserializationArraysTests
    {
        const string IntArrayJson = "[405421362,1082483948,1131707654,345242860,1111968802]";
        const string ShortArrayJson = "[12345,25463,22546,18879,12453]";

        [TestMethod]
        public void CanDeserializeIntArray()
        {
            var result = (int[])JsonConvert.DeserializeObject(IntArrayJson, typeof(int[]));

            Assert.Equal(result[0], 405421362);
            Assert.Equal(result[1], 1082483948);
            Assert.Equal(result[2], 1131707654);
            Assert.Equal(result[3], 345242860);
            Assert.Equal(result[4], 1111968802);
        }

        [TestMethod]
        public void CanDeserializeShortArray()
        {
            var result = (short[])JsonConvert.DeserializeObject(ShortArrayJson, typeof(short[]));

            Assert.Equal(result[0], (short)12345);
            Assert.Equal(result[1], (short)25463);
            Assert.Equal(result[2], (short)22546);
            Assert.Equal(result[3], (short)18879);
            Assert.Equal(result[4], (short)12453);
        }
    }
}
