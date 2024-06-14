//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json.Benchmark.Base;
using nanoFramework.Json.Test.Shared;
using System;
using System.Collections;

namespace nanoFramework.Json.Benchmark.SerializationBenchmarks
{
    [IterationCount(5)]
    public class ReferenceTypesSerializationBenchmark : BaseIterationBenchmark
    {
        const string testString = "TestStringToSerialize";
        const short arrayElementCount = 5;
        readonly int[] intArray = new int[arrayElementCount];
        readonly short[] shortArray = new short[arrayElementCount];
        private Person nestedTestClass;
        private JsonTestClassComplex complexClass;
        private JsonTestTown myTown;
        private ArrayList arrayList;
        private JsonIgnoreTestClass ignoreTest;
        private JsonIgnoreTestClassNoAttr ignoreTestNoAttr;

        [Setup]
        public void Setup()
        {
            var random = new Random();
            for (int i = 0; i < arrayElementCount; i++)
            {
                intArray[i] = random.Next();
                shortArray[i] = (short)random.Next(short.MaxValue);
            }

            nestedTestClass = new Person()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthday = new DateTime(1988, 4, 23),
                ID = 27,
                Address = null,
                ArrayProperty = new string[] { "hello", "world" },
                Friend = new Person()
                {
                    FirstName = "Bob",
                    LastName = "Smith",
                    Birthday = new DateTime(1983, 7, 3),
                    ID = 2,
                    Address = "123 Some St",
                    ArrayProperty = new string[] { "hi", "planet" },
                }
            };

            complexClass = JsonTestClassComplex.CreateTestClass();

            myTown = JsonTestTown.CreateTestClass();

            arrayList = new ArrayList()
            {
                { "testString" },
                { 42 },
                { null },
                { DateTime.UtcNow },
                { TimeSpan.FromSeconds(100) }
            };
            ignoreTest = JsonIgnoreTestClass.CreateTestClass();
            ignoreTestNoAttr = JsonIgnoreTestClassNoAttr.CreateTestClass();
        }

        [Benchmark]
        public void IntArray()
        {
            RunInIteration(() =>
            {
                JsonConvert.SerializeObject(intArray);
            });
        }

        [Benchmark]
        public void ShortArray()
        {
            RunInIteration(() =>
            {
                JsonConvert.SerializeObject(shortArray);
            });
        }

        [Benchmark]
        public void String()
        {
            RunInIteration(() =>
            {
                JsonConvert.SerializeObject(testString);
            });
        }

        [Benchmark]
        public void NestedClass()
        {
            JsonConvert.SerializeObject(nestedTestClass);
        }

        [Benchmark]
        public void ComplexObject()
        {
            JsonConvert.SerializeObject(complexClass);
        }

        [Benchmark]
        public void ComplexArrayObject()
        {
            JsonConvert.SerializeObject(myTown);
        }

        [Benchmark]
        public void ArrayList()
        {
            RunInIteration(() =>
            {
                JsonConvert.SerializeObject(arrayList);
            });
        }

        [Benchmark]
        public void ClassWithAttributeIgnoreEnabled()
        {
            RunInIteration(() =>
            {
                // Turn ON the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = true;
                JsonConvert.SerializeObject(ignoreTest);
                // Turn OFF the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = false;
            });
        }
        [Benchmark]
        public void ClassWithAttributeIgnoreDisabled()
        {
            RunInIteration(() =>
            {
                // Turn OFF the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = false;
                JsonConvert.SerializeObject(ignoreTest);
            });
        }
        [Benchmark]
        public void ClassNoAttributeIgnoreEnabled()
        {
            RunInIteration(() =>
            {
                // Turn ON the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = true;
                JsonConvert.SerializeObject(ignoreTestNoAttr);
                // Turn OFF the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = false;
            });
        }
        [Benchmark]
        public void ClassNoAttributeIgnoreDisabled()
        {
            RunInIteration(() =>
            {
                // Turn OFF the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = false;
                JsonConvert.SerializeObject(ignoreTestNoAttr);
            });
        }
    }
}
