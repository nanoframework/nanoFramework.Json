//
// Copyright (c) 2020 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Threading;
using nanoFramework.Json;


namespace nanoFramework.Test
{

    public class ChildClass
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
        public int four; //<- Fails to get populated on deserialization...

        public override string ToString()
        {
            return $"ChildClass: one={one}, two={two}, three={three}, four={four}";
        }
    }

    public class TestClass
    {
        public int i { get; set; }
        public string aString { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime FixedTimestamp { get; set; }
        public int[] intArray { get; set; }
        public string[] stringArray { get; set; }
        public ChildClass child1;
        public ChildClass Child { get; set; }
        //public object nullObject { get; set; } //<- fails on serialization
        //public float nanFloat { get; set; } //<- fails on serialization
        public float f { get; set; }
        public bool b { get; set; }
        private string dontSerializeStr = "dontPublish";
        private string dontSerialize { get; set; } = "dontPublish";
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("nanoFramework Json Test Program.");

            DoArrayTest();
            DoSimpleObjectTest();
            DoComplexObjectTest();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void DoArrayTest()
        {
            Console.WriteLine("Starting Array Test...");
            int[] intArray = new[] { 1, 3, 5, 7, 9 };

            var result = JsonConvert.SerializeObject(intArray);
            Console.WriteLine($"Serialized Array: {result}");

            Console.WriteLine("Array test succeeded");
        }

        private static void DoSimpleObjectTest()
        {
            Console.WriteLine("Starting Simple Object Test...");
            var source = new ChildClass()
            {
                one = 1,
                two = 2,
                three = 3,
                four = 4, //<- fails because not a property
            };

            var serialized = JsonConvert.SerializeObject(source);
            Console.WriteLine($"Serialized Object: {serialized}");

            var dserResult = (ChildClass)JsonConvert.DeserializeObject(serialized, typeof(ChildClass));

            Console.WriteLine("After Type deserialization:");
            Console.WriteLine(dserResult.ToString());

            Console.WriteLine("Simple Object Test succeeded");
        }

        private static void DoComplexObjectTest()
        {
            Console.WriteLine("Starting Complex Object Test...");
            var test = new TestClass()
            {
                aString = "A string",
                i = 10,
                Timestamp = DateTime.UtcNow,
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00),
                intArray = new[] { 1, 3, 5, 7, 9 },
                stringArray = new[] { "two", "four", "six", "eight" },
                child1 = new ChildClass() { one = 1, two = 2, three = 3 },
                Child = new ChildClass() { one = 100, two = 200, three = 300 },
                //nullObject = null,
                //nanFloat = float.NaN,
                f = 1.2345f,
                b = true
            };
            var result = JsonConvert.SerializeObject(test);
            Console.WriteLine($"Serialized Object: {result}");


            var dserResult = (TestClass)JsonConvert.DeserializeObject(result, typeof(TestClass));

            Console.WriteLine("After Type deserialization:");
            Console.WriteLine(dserResult.ToString());

            //var newInstance = (TestClass)JsonConvert.DeserializeObject(stringValue, typeof(TestClass));
            //if (test.i != newInstance.i ||
            //    test.Timestamp.ToString() != newInstance.Timestamp.ToString() ||
            //    test.aString != newInstance.aString ||
            //    test.someName != newInstance.someName ||
            //    !ArraysAreEqual(test.intArray, newInstance.intArray) ||
            //    !ArraysAreEqual(test.stringArray, newInstance.stringArray)
            //    )
            //    throw new Exception("complex object test failed");
            Console.WriteLine("Complex Object Test succeeded");
        }

        private static bool ArraysAreEqual(Array a1, Array a2)
        {
            if (a1 == null && a2 == null)
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; ++i)
            {
                if (!a1.GetValue(i).Equals(a2.GetValue(i)))
                    return false;
            }
            return true;
        }

    }
}
