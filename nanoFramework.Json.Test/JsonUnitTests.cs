using nanoFramework.TestFramework;
using System;
using System.Diagnostics;

namespace nanoFramework.Json.Test
{

    public class JsonTestClassChild
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
        public int four;

        public override string ToString()
        {
            return $"ChildClass: one={one}, two={two}, three={three}, four={four}";
        }
    }

    public class JsonTestClassFloatNaN
    {
        public float nF { get; set; }
    }

        public class JsonTestClassDoubleNaN
    {
        public double nF { get; set; }
    }

    public class JsonTestClassTimestamp
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public DateTime FixedTimestamp { get; set; }
    }

    public class JsonTestClassComplex
    {
        public int aInteger { get; set; }
        public short aShort { get; set; }
        public byte aByte { get; set; }
        public string aString { get; set; }
        public float aFloat { get; set; }
        public double aDouble { get; set; }
        public bool aBoolean { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime FixedTimestamp { get; set; }
        public int[] intArray { get; set; }
        public short[] shortArray { get; set; }
        public byte[] byteArray { get; set; }
        public string[] stringArray { get; set; }
        public float[] floatArray { get; set; }
        public double[] doubleArray { get; set; }
        public JsonTestClassChild child1;
        public JsonTestClassChild Child { get; set; }
        public object nullObject { get; set; }
        public float nanFloat { get; set; }
        public double nanDouble { get; set; }
#pragma warning disable 0414 //there is no need to set this in the function as it is a test, as such, warning has been disabled!
        private string dontSerializeStr = "dontPublish";
#pragma warning restore 0414
        private string dontSerialize { get; set; } = "dontPublish";
    }

    [TestClass]
    public class JsonUnitTests
    {
        [Setup]
        public void Initialize()
        {
            Debug.WriteLine("Json Library tests initialized.");
        }

        [Cleanup]
        public void CleanUp()
        {
            Debug.WriteLine("Cleaning up after Json Library tests.");
        }

        [TestMethod]
        public void Can_serialize_int_array()
        {
            Debug.WriteLine("Can_serialize_int_array() - Starting test...");
            int[] intArray = new[] { 1, 3, 5, 7, 9 };

            var result = JsonConvert.SerializeObject(intArray);
            Debug.WriteLine($"Serialized Array: {result}");

            var dserResult = (int[])JsonConvert.DeserializeObject(result, typeof(int[]));
            Debug.WriteLine($"After Type deserialization: {dserResult.ToString()}");

            Assert.Equal(intArray, dserResult);

            Debug.WriteLine("Can_serialize_int_array() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timestamp()
        {
            Debug.WriteLine("Can_serialize_deserialize_timestamp() - Starting test...");
            var timestampTests = new JsonTestClassTimestamp()
            {
                Timestamp = DateTime.UtcNow,
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00)
            };

            Debug.WriteLine($"fixed timestamp used for test = {timestampTests.FixedTimestamp.ToString()}");
            Debug.WriteLine($"variable timestamp used for test = {timestampTests.Timestamp.ToString()}");

            var result = JsonConvert.SerializeObject(timestampTests);
            Debug.WriteLine($"Serialized Array: {result}");

            var dserResult = (JsonTestClassTimestamp)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimestamp));
            Debug.WriteLine($"After Type deserialization: {dserResult}");


            Assert.Equal(timestampTests.FixedTimestamp.ToString(), dserResult.FixedTimestamp.ToString()); //cannot handle DateTime, so use ToString()
            Assert.Equal(timestampTests.Timestamp.ToString(), dserResult.Timestamp.ToString()); //cannot handle DateTime, so use ToString()

            Debug.WriteLine("Can_serialize_deserialize_timestamp() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_short_array()
        {
            Debug.WriteLine("Can_serialize_short_array() - Starting test...");
            short[] shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 };

            var result = JsonConvert.SerializeObject(shortArray);
            Debug.WriteLine($"Serialized Array: {result}");

            var dserResult = (short[])JsonConvert.DeserializeObject(result, typeof(short[]));
            Debug.WriteLine($"After Type deserialization: {dserResult.ToString()}");

            Assert.Equal(shortArray, dserResult);

            Debug.WriteLine("Can_serialize_short_array() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_simple_object()
        {
            Debug.WriteLine("Can_serialize_and_deserialize_simple_object() - Starting test...");
            var source = new JsonTestClassChild()
            {
                one = 1,
                two = 2,
                three = 3,
                four = 4,
            };

            var serialized = JsonConvert.SerializeObject(source);
            Debug.WriteLine($"Serialized Object: {serialized}");

            var dserResult = (JsonTestClassChild)JsonConvert.DeserializeObject(serialized, typeof(JsonTestClassChild));

            Debug.WriteLine($"After Type deserialization: {dserResult.ToString()}");

            //Assert.Same(source, dserResult);

            Debug.WriteLine("Can_serialize_and_deserialize_simple_object() - Finished - test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_complex_object()
        {
            Debug.WriteLine("Can_serialize_and_deserialize_complex_object() - Starting test...");
            var test = new JsonTestClassComplex()
            {
                aString = "A string",
                aInteger = 10,
                aByte = 0x05,
                Timestamp = DateTime.UtcNow,
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00),
                intArray = new[] { 1, 3, 5, 7, 9 },
                stringArray = new[] { "two", "four", "six", "eight" },
                shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 },
                byteArray = new[] { (byte)0x22, (byte)0x23, (byte)0x24, (byte)0x25, (byte)0x26 },
                floatArray = new[] { 1.1f, 3.3f, 5.5f, 7.7f, 9.9f },
                doubleArray = new[] { 1.1, 3.3, 5.5, 7.7, 9.9 },
                child1 = new JsonTestClassChild() { one = 1, two = 2, three = 3 },
                Child = new JsonTestClassChild() { one = 100, two = 200, three = 300 },
                nullObject = null,
                nanFloat = float.NaN,
                nanDouble = double.NaN,
                aFloat = 1.2345f,
                aDouble = 1.2345,
                aBoolean = true
            };
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassComplex)JsonConvert.DeserializeObject(result, typeof(JsonTestClassComplex));
            Debug.WriteLine($"After Type deserialization:");
            Debug.WriteLine($"   aString:   {dserResult.aString} ");
            Debug.WriteLine($"   aInteger:  {dserResult.aInteger} ");
            Debug.WriteLine($"   aByte:     {dserResult.aByte} ");
            Debug.WriteLine($"   Timestamp: {dserResult.Timestamp.ToString()} ");
            Debug.WriteLine($"   FixedTimestamp: {dserResult.FixedTimestamp.ToString()} ");
            Debug.Write($"   intArray: ");
            foreach (int i in dserResult.intArray)
            {
                Debug.Write($"{i.ToString()}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   stringArray: ");
            foreach (string i in dserResult.stringArray)
            {
                Debug.Write($"{i.ToString()}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   shortArray: ");
            foreach (short i in dserResult.shortArray)
            {
                Debug.Write($"{i.ToString()}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   byteArray: ");
            foreach (byte i in dserResult.byteArray)
            {
                Debug.Write($"{i.ToString()}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   floatArray: ");
            foreach (float i in dserResult.floatArray)
            {
                Debug.Write($"{i.ToString()}, ");
            }
            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i.ToString()}, ");
            }
            Debug.WriteLine("");

            Debug.WriteLine($"   child1: {dserResult.child1.ToString()} ");
            Debug.WriteLine($"   Child: {dserResult.Child.ToString()} ");

            if (dserResult.nullObject == null)
            {
                Debug.WriteLine($"   nullObject is null");
            }
            else
            {
                Debug.WriteLine($"   nullObject: {dserResult.nullObject}");
            }
            Debug.WriteLine($"   nanFloat: {dserResult.nanFloat} ");
            Debug.WriteLine($"   nanDouble: {dserResult.nanDouble} ");
            Debug.WriteLine($"   aFloat: {dserResult.aFloat.ToString()} ");
            Debug.WriteLine($"   aDouble: {dserResult.aDouble.ToString()} ");
            Debug.WriteLine($"   aBoolean: {dserResult.aBoolean.ToString()} ");

            //Assert.Same(test, result);

            Debug.WriteLine("Can_serialize_and_deserialize_complex_object() - Finished - test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_nan_float()
        {
            Debug.WriteLine("Starting float NaN Object Test...");
            var test = new JsonTestClassFloatNaN()
            {
                nF = float.NaN,
            };
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassFloatNaN)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloatNaN));
            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(result, "{\"nF\":null}", "Serialized result is null");
            Assert.Equal(true, float.IsNaN(dserResult.nF), "Deserialized Result is NaN");

            Debug.WriteLine("float NaN Object Test Test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_nan_double()
        {
            Debug.WriteLine("Starting double NaN Object Test...");
            var test = new JsonTestClassDoubleNaN()
            {
                nF = double.NaN,
            };
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassDoubleNaN)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDoubleNaN));
            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(result, "{\"nF\":null}", "Serialized result is null");
            Assert.Equal(true, double.IsNaN(dserResult.nF), "Deserialized Result is NaN");

            Debug.WriteLine("double NaN Object Test Test succeeded");
            Debug.WriteLine("");
        }
    }
}
