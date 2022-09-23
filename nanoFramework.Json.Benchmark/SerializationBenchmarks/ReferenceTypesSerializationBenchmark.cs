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

            complexClass = new JsonTestClassComplex()
            {
                aInteger = 10,
                aShort = 254,
                aByte = 0x05,
                aString = "A string",
                aFloat = 1.2345f,
                aDouble = 1.2345,
                aBoolean = true,
                Timestamp = DateTime.UtcNow,
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00),
                intArray = new[] { 1, 3, 5, 7, 9 },
                shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 },
                byteArray = new[] { (byte)0x22, (byte)0x23, (byte)0x24, (byte)0x25, (byte)0x26 },
                stringArray = new[] { "two", "four", "six", "eight" },
                floatArray = new[] { 1.1f, 3.3f, 5.5f, 7.7f, 9.9f },
                doubleArray = new[] { 1.12345, 3.3456, 5.56789, 7.78910, 9.910111213 },
                child1 = new JsonTestClassChild() { one = 1, two = 2, three = 3 },
                Child = new JsonTestClassChild() { one = 100, two = 200, three = 300 },
                nullObject = null,
                nanFloat = float.NaN,
                nanDouble = double.NaN,
            };

            myTown = new JsonTestTown
            {
                TownID = 1,
                TownName = "myTown",
                CompaniesInThisTown = new JsonTestCompany[]
                {
                    new JsonTestCompany { CompanyID = 1, CompanyName = "AAA Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 2, CompanyName = "BBB Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 3, CompanyName = "CCC Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 4, CompanyName = "DDD Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 5, CompanyName = "EEE Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 6, CompanyName = "FFF Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 7, CompanyName = "GGG Amalgamated Industries" },
                    new JsonTestCompany { CompanyID = 8, CompanyName = "HHH Amalgamated Industries" }
                },
                EmployeesInThisTown = new JsonTestEmployee[]
                {
                    new JsonTestEmployee
                    {
                        EmployeeID = 1,
                        EmployeeName = "John Smith",
                        CurrentEmployer = new JsonTestCompany { CompanyID = 3, CompanyName = "CCC Amalgamated Industries" },
                        FormerEmployers = new JsonTestCompany[]
                        {
                            new JsonTestCompany { CompanyID = 2, CompanyName = "BBB Amalgamated Industries" },
                            new JsonTestCompany { CompanyID = 5, CompanyName = "EEE Amalgamated Industries" },
                        }
                    },
                    new JsonTestEmployee
                    {
                        EmployeeID = 1,
                        EmployeeName = "Jim Smith",
                        CurrentEmployer = new JsonTestCompany { CompanyID = 7, CompanyName = "GGG Amalgamated Industries" },
                        FormerEmployers = new JsonTestCompany[]
                        {
                            new JsonTestCompany { CompanyID = 4, CompanyName = "DDD Amalgamated Industries" },
                            new JsonTestCompany { CompanyID = 1, CompanyName = "AAA Amalgamated Industries" },
                            new JsonTestCompany { CompanyID = 6, CompanyName = "FFF Amalgamated Industries" },
                        }
                    }
                }
            };

            arrayList = new ArrayList()
            {
                { "testString" },
                { 42 },
                { null },
                { DateTime.UtcNow },
                { TimeSpan.FromSeconds(100) }
            };
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
    }
}