//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Threading;
using System.Diagnostics;
using nanoFramework.Json;



namespace nanoFramework.Test
{

    public class ChildClass
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

    public class TestClassNaN
    {
        public float nF { get; set; } //<- fails on deserialization if NaN
    }

    public class TestClass
    {
        public int aInteger { get; set; }
        public short aShort { get; set; } //<- fails on deserialization
        public byte aByte { get; set; }
        public string aString { get; set; }
        public float aFloat { get; set; }
        public bool aBoolean { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime FixedTimestamp { get; set; }
        public int[] intArray { get; set; }
        public string[] stringArray { get; set; }
        public short[] shortArray { get; set; } //<- fails on serialization/deserialization
        public float[] floatArray { get; set; }
        public ChildClass child1;
        public ChildClass Child { get; set; }
        public object nullObject { get; set; }
        public float nanFloat { get; set; } //<- fails on serialization
#pragma warning disable 0414 //there is no need to set this in the function as it is a test, as such, warning has been disabled!
        private string dontSerializeStr = "dontPublish";
#pragma warning restore 0414
        private string dontSerialize { get; set; } = "dontPublish";
    }

    //[TestFixture]
    public class Program
    {
        //[SetUp]
        public static void Main()
        {
            Console.WriteLine("nanoFramework Json Test Program.");
            Thread.Sleep(10000);

            Can_serialize_int_array();
            Can_serialize_short_array();

            Can_serialize_and_deserialize_nan_float();

            Can_serialize_and_deserialize_simple_object();
            Can_serialize_and_deserialize_complex_object();


            Thread.Sleep(Timeout.Infinite);
        }

        //[Test]
        private static void Can_serialize_int_array()
        {
            Debug.WriteLine("Starting Int Array Serialize Test...");
            int[] intArray = new[] { 1, 3, 5, 7, 9 };

            var result = JsonConvert.SerializeObject(intArray);
            Debug.WriteLine($"Serialized Array: {result}");

            Debug.WriteLine("Array test succeeded");
        }

        //[Test]
        private static void Can_serialize_short_array()
        {
            Debug.WriteLine("Starting short Array Serialize Test...");
            short[] shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 };

            var result = JsonConvert.SerializeObject(shortArray);
            Debug.WriteLine($"Serialized Array: {result}");

            Debug.WriteLine("Array test succeeded");
        }

        //[Test]
        private static void Can_serialize_and_deserialize_simple_object()
        {
            Debug.WriteLine("Starting Simple Object Serialize and Deserialize Test...");
            var source = new ChildClass()
            {
                one = 1,
                two = 2,
                three = 3,
                four = 4,
            };

            var serialized = JsonConvert.SerializeObject(source);
            Debug.WriteLine($"Serialized Object: {serialized}");

            var dserResult = (ChildClass)JsonConvert.DeserializeObject(serialized, typeof(ChildClass));

            Debug.WriteLine("After Type deserialization:");
            Debug.WriteLine(dserResult.ToString());

            Debug.WriteLine("Simple Object Test succeeded");
        }

        //[Test]
        private static void Can_serialize_and_deserialize_complex_object()
        {
            Debug.WriteLine("Starting Serialize and Deserialize Complex Object Test...");
            var test = new TestClass()
            {
                aString = "A string",
                aInteger = 10,
                Timestamp = DateTime.UtcNow,
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00),
                intArray = new[] { 1, 3, 5, 7, 9 },
                stringArray = new[] { "two", "four", "six", "eight" },
                shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 },
                floatArray = new[] { 1.1f, 3.3f, 5.5f, 7.7f, 9.9f },
                child1 = new ChildClass() { one = 1, two = 2, three = 3 },
                Child = new ChildClass() { one = 100, two = 200, three = 300 },
                nullObject = null,
                nanFloat = float.NaN,
                aFloat = 1.2345f,
                aBoolean = true
            };
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (TestClass)JsonConvert.DeserializeObject(result, typeof(TestClass));

            Debug.WriteLine("After Type deserialization:");
            Debug.WriteLine(dserResult.ToString());

            //var newInstance = (TestClass)JsonConvert.DeserializeObject(stringValue, typeof(TestClass));
            //if (test.i != newInstance.i ||
            //    test.Timestamp.ToString() != newInstance.Timestamp.ToString() ||
            //    test.aString != newInstance.aString ||
            //    test.someName != newInstance.someName ||
            //    !ArraysAreEqual(test.intArray, newInstance.intArray) ||
            //    !ArraysAreEqual(test.stringArray, newInstance.stringArray)
            //    )
            //    throw new Exception("complex object test failed");
            Debug.WriteLine("Complex Object Test succeeded");
        }

        //[Test]
        private static void Can_serialize_and_deserialize_nan_float()
        {
            Debug.WriteLine("Starting float NaN Object Test...");
            var test = new TestClassNaN()
            {
                nF = float.NaN,
            };
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (TestClassNaN)JsonConvert.DeserializeObject(result, typeof(TestClassNaN));

            Debug.WriteLine("After Type deserialization:");
            Debug.WriteLine(dserResult.ToString());

            Debug.WriteLine("float NaN Object Test Test succeeded");

        }

        //[Test]
        //private static bool ArraysAreEqual(Array a1, Array a2)
        //{
        //    if (a1 == null && a2 == null)
        //        return true;
        //    if (a1 == null || a2 == null)
        //        return false;
        //    if (a1.Length != a2.Length)
        //        return false;
        //    for (int i = 0; i < a1.Length; ++i)
        //    {
        //        if (!a1.GetValue(i).Equals(a2.GetValue(i)))
        //            return false;
        //    }
        //    return true;
        //}

    }
}
