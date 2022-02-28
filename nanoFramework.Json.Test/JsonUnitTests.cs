//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace nanoFramework.Json.Test
{
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
        public void Can_serialize_and_deserialize_arrays_of_class_objects()
        {
            Debug.WriteLine("Can_serialize_and_deserialize_arrays_of_class_objects() - Starting test...");

            JsonTestTown myTown = new JsonTestTown
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

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(myTown);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Array: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            JsonTestTown dserResult = (JsonTestTown)JsonConvert.DeserializeObject(result, typeof(JsonTestTown));

            endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After deserialization - type: JsonTestTown");

            Assert.Equal(
                myTown.TownID,
                dserResult.TownID,
                $"Validation: myTown.TownID: {myTown.TownID}");

            Assert.Equal(
                myTown.TownName,
                dserResult.TownName,
                $"Validation: myTown.TownName: {myTown.TownName}");

            for (int i = 0; i < myTown.CompaniesInThisTown.Length; i++)
            {
                Assert.Equal(
                    myTown.CompaniesInThisTown[i].CompanyID,
                    dserResult.CompaniesInThisTown[i].CompanyID,
                    $"Validation: myTown.CompaniesInThisTown[{i}].CompanyID: {myTown.CompaniesInThisTown[i].CompanyID}");

                Assert.Equal(
                    myTown.CompaniesInThisTown[i].CompanyName,
                    dserResult.CompaniesInThisTown[i].CompanyName,
                    $"Validation: myTown.CompaniesInThisTown[{i}].CompanyName: {myTown.CompaniesInThisTown[i].CompanyName}");
            }

            for (int i = 0; i < myTown.EmployeesInThisTown.Length; i++)
            {
                Assert.Equal(
                    myTown.EmployeesInThisTown[i].EmployeeID,
                    dserResult.EmployeesInThisTown[i].EmployeeID,
                    $"Validation: myTown.EmployeesInThisTown[{i}].EmployeeID: {myTown.EmployeesInThisTown[i].EmployeeID} ");

                Assert.Equal(
                    myTown.EmployeesInThisTown[i].EmployeeName,
                    dserResult.EmployeesInThisTown[i].EmployeeName,
                    $"Validation: myTown.EmployeesInThisTown[{i}].EmployeeName: {myTown.EmployeesInThisTown[i].EmployeeName} ");

                Assert.Equal(
                    myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyID,
                    dserResult.EmployeesInThisTown[i].CurrentEmployer.CompanyID,
                    $"Validation: myTown.EmployeesInThisTown[{i}].CurrentEmployer.CompanyID: {myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyID} ");

                Assert.Equal(
                    myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyName,
                    dserResult.EmployeesInThisTown[i].CurrentEmployer.CompanyName,
                    $"Validation: myTown.EmployeesInThisTown[{i}].CurrentEmployer.CompanyName: {myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyName} ");

                for (int j = 0; j < myTown.EmployeesInThisTown[i].FormerEmployers.Length; j++)
                {
                    Assert.Equal(
                        myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyID,
                        dserResult.EmployeesInThisTown[i].FormerEmployers[j].CompanyID,
                        $"Validation: myTown.EmployeesInThisTown[{i}].FormerEmployers[{j}].CompanyID: {myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyID} ");

                    Assert.Equal(
                        myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyName,
                        dserResult.EmployeesInThisTown[i].FormerEmployers[j].CompanyName,
                        $"Validation: myTown.EmployeesInThisTown[{i}].FormerEmployers[{j}].CompanyName: {myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyName} ");
                }
            }

            Debug.WriteLine("Can_serialize_and_deserialize_arrays_of_class_objects() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_int_array()
        {
            Debug.WriteLine("Can_serialize_int_array() - Starting test...");
            int[] intArray = new[] { 1, 3, 5, 7, 9 };

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(intArray);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Array: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (int[])JsonConvert.DeserializeObject(result, typeof(int[]));

            endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

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

            Debug.WriteLine($"fixed timestamp used for test = {timestampTests.FixedTimestamp}");
            Debug.WriteLine($"variable timestamp used for test = {timestampTests.Timestamp}");

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(timestampTests);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Array: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassTimestamp)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimestamp));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(timestampTests.FixedTimestamp.ToString(), dserResult.FixedTimestamp.ToString()); //cannot handle DateTime, so use ToString()
            Assert.Equal(timestampTests.Timestamp.ToString(), dserResult.Timestamp.ToString()); //cannot handle DateTime, so use ToString()

            Debug.WriteLine("Can_serialize_deserialize_timestamp() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timespan_00()
        {
            Debug.WriteLine("Can_serialize_deserialize_timespan_00() - Starting test...");

            var randomValue = new Random();

            var timeSpanTests = new JsonTestClassTimeSpan()
            {
                Duration2 = TimeSpan.FromSeconds(randomValue.Next(59)),
                Duration3 = TimeSpan.FromMilliseconds(randomValue.Next(999)),
                Duration4 = TimeSpan.FromHours(randomValue.Next(99)),
                Duration5 = TimeSpan.FromDays(randomValue.Next(999)),
            };

            Debug.WriteLine($"Fixed timespan used for test = {timeSpanTests.Duration1}");
            Debug.WriteLine($"variable timespan 2 used for test = {timeSpanTests.Duration2}");
            Debug.WriteLine($"variable timespan 3 used for test = {timeSpanTests.Duration3}");
            Debug.WriteLine($"variable timespan 4 used for test = {timeSpanTests.Duration4}");
            Debug.WriteLine($"variable timespan 5 used for test = {timeSpanTests.Duration5}");

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(timeSpanTests);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized class: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassTimeSpan)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimeSpan));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(timeSpanTests.Duration1.ToString(), dserResult.Duration1.ToString(), $"wrong value for Duration1, expected 1:09:00, got {dserResult.Duration1.ToString()}");
            Assert.Equal(timeSpanTests.Duration2.Ticks.ToString(), dserResult.Duration2.Ticks.ToString(), $"wrong value for Duration2, expected {timeSpanTests.Duration2}, got {dserResult.Duration2}");
            Assert.Equal(timeSpanTests.Duration3.Ticks.ToString(), dserResult.Duration3.Ticks.ToString(), $"wrong value for Duration3, expected {timeSpanTests.Duration3}, got {dserResult.Duration3}");
            Assert.Equal(timeSpanTests.Duration4.Ticks.ToString(), dserResult.Duration4.Ticks.ToString(), $"wrong value for Duration4, expected {timeSpanTests.Duration4}, got {dserResult.Duration4}");
            Assert.Equal(timeSpanTests.Duration5.Ticks.ToString(), dserResult.Duration5.Ticks.ToString(), $"wrong value for Duration5, expected {timeSpanTests.Duration5}, got {dserResult.Duration5}");

            Debug.WriteLine("Can_serialize_deserialize_timespan_00() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timespan_01()
        {
            Debug.WriteLine("Can_serialize_deserialize_timespan_01() - Starting test...");

            JsonTestClassTimeSpan[] _timeSpans = new JsonTestClassTimeSpan[] {
                new JsonTestClassTimeSpan()
                {
                    Duration2 = TimeSpan.Zero,
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(12, 00, 00),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(10, 20, 00),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(00, 10, 00),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(10, 00, 00),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(00, 00, 59),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(00, 59, 00),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(23, 00, 00),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(23, 59, 59, 99),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(99, 23, 59, 59, 9999999),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(10, 20, 30, 40, 50),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(1, 2, 3),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = TimeSpan.FromDays(14),
                },
                new JsonTestClassTimeSpan()
                {
                    Duration2 = new TimeSpan(10, 12, 00, 00)
                }
            };

            long startTimestamp;
            long endTimestamp;

            for (int i = 0; i < _timeSpans.Length; i++)
            {
                startTimestamp = Environment.TickCount64;

                var result = JsonConvert.SerializeObject(_timeSpans[i]);

                endTimestamp = Environment.TickCount64;

                Debug.WriteLine($"Serialized class: {result} took {endTimestamp - startTimestamp}ms");

                startTimestamp = Environment.TickCount64;

                var dserResult = (JsonTestClassTimeSpan)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimeSpan));

                endTimestamp = Environment.TickCount64;
                Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

                Debug.WriteLine($"After Type deserialization: {dserResult}");

                // can't compare TimeSpans directly, using ticks
                Assert.Equal(_timeSpans[i].Duration2.Ticks, dserResult.Duration2.Ticks, $"wrong value, expected {_timeSpans[i].Duration2}, got {dserResult.Duration2}");
            }

            Debug.WriteLine("Can_serialize_deserialize_timespan_01() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timespan_02()
        {
            Debug.WriteLine("Can_serialize_deserialize_timespan_02() - Starting test...");

            string[] strArr = new string[] {
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\"24:0:0\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\"0:0:60\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\"10:\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\":10\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\"10:20:\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\".123\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\"10.\"}",
                "{\"Duration5\":\"00:00:00\",\"Duration1\":\"01:09:00\",\"DummyValue2\":777,\"Duration2\":\"00:00:00\",\"DummyValue1\":-999,\"Duration3\":\"00:00:00\",\"Duration4\":\"10.12\"}",
            };

            long startTimestamp;
            long endTimestamp;

            for (int i = 0; i < strArr.Length; i++)
            {
                startTimestamp = Environment.TickCount64;

                var dserResult = (JsonTestClassTimeSpan)JsonConvert.DeserializeObject(strArr[i], typeof(JsonTestClassTimeSpan));

                endTimestamp = Environment.TickCount64;
                Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

                Assert.Null(dserResult, $"Deserialization should have failed for strArr[{i}]");
            }

            Debug.WriteLine("Can_serialize_deserialize_timespan_02() - Finished - test succeeded.");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_short_array()
        {
            Debug.WriteLine("Can_serialize_short_array() - Starting test...");
            short[] shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 };

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(shortArray);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Array: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (short[])JsonConvert.DeserializeObject(result, typeof(short[]));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

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

            var startTimestamp = Environment.TickCount64;

            var serialized = JsonConvert.SerializeObject(source);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Object: {serialized} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassChild)JsonConvert.DeserializeObject(serialized, typeof(JsonTestClassChild));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Debug.WriteLine("Can_serialize_and_deserialize_simple_object() - Finished - test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_complex_object()
        {
            Debug.WriteLine("Can_serialize_and_deserialize_complex_object() - Starting test...");
            var test = new JsonTestClassComplex()
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

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(test);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Object: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassComplex)JsonConvert.DeserializeObject(result, typeof(JsonTestClassComplex));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization:");
            Debug.WriteLine($"   aString:   {dserResult.aString} ");
            Debug.WriteLine($"   aInteger:  {dserResult.aInteger} ");
            Debug.WriteLine($"   aByte:     {dserResult.aByte} ");
            Debug.WriteLine($"   Timestamp: {dserResult.Timestamp} ");
            Debug.WriteLine($"   FixedTimestamp: {dserResult.FixedTimestamp} ");
            Debug.Write($"   intArray: ");

            foreach (int i in dserResult.intArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   stringArray: ");
            foreach (string i in dserResult.stringArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   shortArray: ");
            foreach (short i in dserResult.shortArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   byteArray: ");
            foreach (byte i in dserResult.byteArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   floatArray: ");
            foreach (float i in dserResult.floatArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i}, ");
            }
            Debug.WriteLine("");

            Debug.WriteLine($"   child1: {dserResult.child1} ");
            Debug.WriteLine($"   Child: {dserResult.Child} ");

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
            Debug.WriteLine($"   aFloat: {dserResult.aFloat} ");
            Debug.WriteLine($"   aDouble: {dserResult.aDouble} ");
            Debug.WriteLine($"   aBoolean: {dserResult.aBoolean} ");

            Debug.WriteLine("Can_serialize_and_deserialize_complex_object() - Finished - test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_float()
        {
            Debug.WriteLine("Starting float Object Test...");
            var test = new JsonTestClassFloat()
            {
                aFloat = 2567.454f, //TODO Deserialized float fails when number is greater than 3-4 DP with an extra `.` at the end.
            };

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(test);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Object: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassFloat)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloat));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(result, "{\"aFloat\":" + test.aFloat + "}", "Serialized float result is equal"); //TODO: better str handling!
            Assert.Equal(test.aFloat, dserResult.aFloat, "Deserialized float Result is Equal");

            Debug.WriteLine("float Object Test Test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_nan_float()
        {
            Debug.WriteLine("Starting float NaN Object Test...");
            var test = new JsonTestClassFloat()
            {
                aFloat = float.NaN,
            };

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(test);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Object: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassFloat)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloat));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(result, "{\"aFloat\":null}", "Serialized float result is null");
            Assert.True(float.IsNaN(dserResult.aFloat), "Deserialized float Result is NaN");

            Debug.WriteLine("float Object Test Test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_double()
        {
            Debug.WriteLine("Starting double Object Test...");
            var test = new JsonTestClassDouble()
            {
                aDouble = 123.4567,
            };

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(test);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Object: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassDouble)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDouble));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(result, "{\"aDouble\":123.45669999}", "Serialized double result is a double"); //TODO: possible conversion issue (but can happen with conversions)

            Debug.WriteLine("double Object Test Test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_nan_double()
        {
            Debug.WriteLine("Starting double NaN Object Test...");
            var test = new JsonTestClassDouble()
            {
                aDouble = double.NaN,
            };

            var startTimestamp = Environment.TickCount64;

            var result = JsonConvert.SerializeObject(test);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialized Object: {result} took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var dserResult = (JsonTestClassDouble)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDouble));

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Debug.WriteLine($"After Type deserialization: {dserResult}");

            Assert.Equal(result, "{\"aDouble\":null}", "Serialized double result is null");
            Assert.Equal(true, double.IsNaN(dserResult.aDouble), "Deserialized double Result is NaN");

            Debug.WriteLine("double NaN Object Test Test succeeded");
            Debug.WriteLine("");
        }

        [TestMethod]
        public void BasicSerializationTest()
        {
            ICollection collection = new ArrayList() { 1, null, 2, "blah", false };

            Hashtable hashtable = new();
            hashtable.Add("collection", collection);
            hashtable.Add("nulltest", null);
            hashtable.Add("stringtest", "hello world");

            object[] array = new object[] { hashtable };

            var startTimestamp = Environment.TickCount64;

            string json = JsonConvert.SerializeObject(array);

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            string correctValue = "[{\"collection\":[1,null,2,\"blah\",false],\"nulltest\":null,\"stringtest\":\"hello world\"}]";

            Assert.Equal(json, correctValue, "Values did not match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void BasicDeserializationTest()
        {
            string json = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[-1,null,24.565657576,\"blah\",false]}]";

            var startTimestamp = Environment.TickCount64;

            ArrayList arrayList = (ArrayList)JsonConvert.DeserializeObject(json, typeof(ArrayList));

            var endTimestamp = Environment.TickCount64;

            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Hashtable hashtable = arrayList[0] as Hashtable;
            string stringtest = hashtable["stringtest"].ToString();
            object nulltest = hashtable["nulltest"];

            ArrayList collection = hashtable["collection"] as ArrayList;
            int a = (int)collection[0];
            object b = collection[1];
            double c = (double)collection[2];
            string d = collection[3].ToString();
            bool e = (bool)collection[4];

            Assert.Equal(arrayList.Count, 1, "arrayList count did not match");

            Assert.Equal(hashtable.Count, 3, "hashtable count did not match");

            Assert.Equal(stringtest, "hello world", "stringtest did not match");

            Assert.Null(nulltest, "nulltest did not match");

            Assert.Equal(collection.Count, 5, "collection count did not match");

            Assert.Equal(a, -1, "a value did not match");

            Assert.Null(b, "b value did not match");

            Assert.Equal(c, 24.565657576, "c value did not match");

            Assert.Equal(d, "blah", "d value did not match");

            Assert.False(e, "e value did not match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void SerializeDeserializeDateTest()
        {
            DateTime testTime = new(2015, 04, 22, 11, 56, 39, 456);

            ICollection collection = new ArrayList() { testTime };

            var startTimestamp = Environment.TickCount64;

            string jsonString = JsonConvert.SerializeObject(collection);

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            OutputHelper.WriteLine($"Json payload: {jsonString}");

            ArrayList convertTime = (ArrayList)JsonConvert.DeserializeObject(jsonString, typeof(ArrayList));

            Assert.Equal(testTime.Ticks, ((DateTime)convertTime[0]).Ticks, "Values did not match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void SerializeSimpleClassTest()
        {
            Person friend = new()
            {
                FirstName = "Bob",
                LastName = "Smith",
                Birthday = new DateTime(1983, 7, 3),
                ID = 2,
                Address = "123 Some St",
                ArrayProperty = new string[] { "hi", "planet" },
            };

            Person person = new()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthday = new DateTime(1988, 4, 23),
                ID = 27,
                Address = null,
                ArrayProperty = new string[] { "hello", "world" },
                Friend = friend
            };

            var startTimestamp = Environment.TickCount64;

            string json = JsonConvert.SerializeObject(person);

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            string correctValue = "{\"Address\":null,\"ArrayProperty\":[\"hello\",\"world\"],\"ID\":27,\"Birthday\":\"1988-04-23T00:00:00.0000000Z\",\"LastName\":\"Doe\",\"Friend\""
                + ":{\"Address\":\"123 Some St\",\"ArrayProperty\":[\"hi\",\"planet\"],\"ID\":2,\"Birthday\":\"1983-07-03T00:00:00.0000000Z\",\"LastName\":\"Smith\",\"Friend\":null,\"FirstName\":\"Bob\"}"
                + ",\"FirstName\":\"John\"}";

            Assert.Equal(json, correctValue, "Values did not match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void SerializeAbstractClassTest()
        {
            AbstractClass a = new RealClass() { ID = 12 };
            string json = JsonConvert.SerializeObject(a);

            string correctValue = "{\"Test2\":\"test2\",\"ID\":12,\"Test\":\"test\"}";

            Assert.Equal(json, correctValue, "Value for AbstractClass did not match");

            RealClass b = new() { ID = 12 };

            var startTimestamp = Environment.TickCount64;

            json = JsonConvert.SerializeObject(b);

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            correctValue = "{\"Test2\":\"test2\",\"ID\":12,\"Test\":\"test\"}";

            Assert.Equal(json, correctValue, "Values for RealClass did not match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_01()
        {
            var testString = "{\"desired\":{\"TimeToSleep\":5,\"$version\":2},\"reported\":{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}}";

            var startTimestamp = Environment.TickCount64;

            var twinPayload = (TwinProperties)JsonConvert.DeserializeObject(testString, typeof(TwinProperties));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(twinPayload, "Deserialization returned a null object");

            Assert.Equal(twinPayload.desired.TimeToSleep, 5, "desired.TimeToSleep doesn't match");
            Assert.Null(twinPayload.desired._metadata, "desired._metadata doesn't match");

            Assert.Equal(twinPayload.reported.Firmware, "nanoFramework", "reported.Firmware doesn't match");
            Assert.Equal(twinPayload.reported.TimeToSleep, 2, "reported.TimeToSleep doesn't match");
            Assert.Null(twinPayload.reported._metadata, "reported._metadata doesn't match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_02()
        {
            var startTimestamp = Environment.TickCount64;

            TwinPayload twinPayload = (TwinPayload)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayload));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(twinPayload, "Deserialization returned a null object");

            Assert.Equal(twinPayload.authenticationType, "sas", "authenticationType doesn't match");
            Assert.Equal(twinPayload.statusUpdateTime.Ticks, DateTime.MinValue.Ticks, "statusUpdateTime doesn't match");
            Assert.Equal(twinPayload.cloudToDeviceMessageCount, 0, "cloudToDeviceMessageCount doesn't match");
            Assert.Equal(twinPayload.x509Thumbprint.Count, 2, "x509Thumbprint collection count doesn't match");
            Assert.Equal(twinPayload.version, 381, "version doesn't match");
            Assert.Equal(twinPayload.properties.desired.TimeToSleep, 30, "properties.desired.TimeToSleep doesn't match");
            Assert.Equal(twinPayload.properties.reported._metadata.Count, 3, "properties.reported._metadata collection count doesn't match");
            Assert.Equal(twinPayload.properties.desired._metadata.Count, 3, "properties.desired._metadata collection count doesn't match");

            Debug.WriteLine("");
        }


        [TestMethod]
        public void CanDeserializeAzureTwinProperties_03()
        {
            var startTimestamp = Environment.TickCount64;

            TwinPayloadProperties twinPayload = (TwinPayloadProperties)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayloadProperties));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(twinPayload, "Deserialization returned a null object");

            Assert.Equal(twinPayload.properties.desired.TimeToSleep, 30, "properties.desired.TimeToSleep doesn't match");
            Assert.Equal(twinPayload.properties.reported._metadata.Count, 3, "properties.reported._metadata collection count doesn't match");
            Assert.Equal(twinPayload.properties.desired._metadata.Count, 3, "properties.desired._metadata collection count doesn't match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_04()
        {
            var startTimestamp = Environment.TickCount64;

            Hashtable twinPayload = (Hashtable)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(Hashtable));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(twinPayload, "Deserialization returned a null object");

            Assert.Equal((string)twinPayload["authenticationType"], "sas", "authenticationType doesn't match");
            //Assert.Equal(((DateTime)twinPayload["statusUpdateTime"]).Ticks, DateTime.MinValue.Ticks, "statusUpdateTime doesn't match");
            Assert.Equal((int)twinPayload["cloudToDeviceMessageCount"], 0, "cloudToDeviceMessageCount doesn't match");
            Assert.Equal(((Hashtable)twinPayload["x509Thumbprint"]).Count, 2, "x509Thumbprint collection count doesn't match");
            Assert.Equal((int)twinPayload["version"], 381, "version doesn't match");

            // get properties
            Hashtable properties = (Hashtable)twinPayload["properties"];

            // get hastables with properties
            Hashtable reported = (Hashtable)properties["reported"];
            Hashtable desired = (Hashtable)properties["desired"];

            Assert.Equal((int)desired["TimeToSleep"], 30, "properties.desired.TimeToSleep doesn't match");
            Assert.Equal((int)desired["$version"], 7, "properties.desired.$version doesn't match");
            Assert.Equal((int)reported["TimeToSleep"], 30, "properties.reported.TimeToSleep doesn't match");
            Assert.Equal((int)reported["$version"], 374, "properties.reported.$version doesn't match");
            Assert.Equal((string)reported["Firmware"], "nanoFramework", "properties.reported.Firmware doesn't match");

            Hashtable reportedMetadata = (Hashtable)reported["$metadata"];
            Hashtable desiredMetadata = (Hashtable)desired["$metadata"];

            Assert.Equal(reportedMetadata.Count, 3, "properties.reported collection count doesn't match");
            Assert.Equal(desiredMetadata.Count, 3, "properties.desired collection count doesn't match");

            DateTime desiredLastUpdated = new(637582954318120413);
            DateTime reportedLastUpdated = new(637582963611232797);

            Assert.Equal((DateTime)reportedMetadata["$lastUpdated"], reportedLastUpdated, $"Expecting {reportedLastUpdated.ToString("o")} for properties.reported.$metadata.$lastUpdated, got {((DateTime)reportedMetadata["$lastUpdated"]).ToString("o")}");
            Assert.Equal((DateTime)desiredMetadata["$lastUpdated"], desiredLastUpdated, $"Expecting {desiredLastUpdated.ToString("o")} properties.desired.$metadata.$lastUpdated, got {((DateTime)desiredMetadata["$lastUpdated"]).ToString("o")}");

            Hashtable desiredTimeToSleep = (Hashtable)desiredMetadata["TimeToSleep"];

            DateTime desiredTimeToSleepUpdated = new(637582954318120413);

            Assert.Equal((DateTime)desiredTimeToSleep["$lastUpdated"], desiredTimeToSleepUpdated, $"Expecting {desiredTimeToSleepUpdated.ToString("o")} properties.reported.$metadata.TimeToSleep.$lastUpdated, got {((DateTime)desiredTimeToSleep["$lastUpdated"]).ToString("o")}");
            Assert.Equal((int)desiredTimeToSleep["$lastUpdatedVersion"], 7, "properties.reported.$metadata.TimeToSleep.$lastUpdatedVersion doesn't match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_01()
        {

            var testString = "{\"type\":6}";

            var startTimestamp = Environment.TickCount64;

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testString, typeof(InvocationReceiveMessage));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(dserResult, "Deserialization returned a null object");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_02()
        {
            var testString = @"{
    ""type"": 1,
    ""headers"": {
        ""Foo"": ""Bar""
    },
    ""invocationId"": ""123"",
    ""target"": ""Send""
    ""arguments"": [
        42,
        ""Test Message"",
    ]
    }";

            var startTimestamp = Environment.TickCount64;

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testString, typeof(InvocationReceiveMessage));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(dserResult, "Deserialization returned a null object");

            Assert.Equal(dserResult.type, 1, "type value is not correct");
            Assert.Equal(dserResult.invocationId, "123", "invocationId value is not correct");
            Assert.Equal(dserResult.target, "Send", "target value is not correct");

            Assert.Equal((int)dserResult.arguments[0], 42, "arguments[0] value is not correct");
            Assert.Equal((string)dserResult.arguments[1], "Test Message", "arguments[1] value is not correct");

            Assert.Equal(dserResult.headers.Count, 1, "headers count is not correct");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_03()
        {
            var startTimestamp = Environment.TickCount64;

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testInvocationReceiveMessage, typeof(InvocationReceiveMessage));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(dserResult, "Deserialization returned a null object");

            Assert.Equal(dserResult.type, 1, "type value is not correct");
            Assert.Equal(dserResult.target, "ReceiveAdvancedMessage", "target value is not correct");

            Assert.Equal((int)dserResult.arguments[2], 3, "arguments[2] value is not correct");
            Assert.IsType(typeof(ArrayList), dserResult.arguments, "arguments type it's wrong after deserialization");
            Assert.Equal(dserResult.arguments.Count, 3, $"number of arguments is different than expected: {dserResult.arguments.Count}");

            Hashtable arg0 = (Hashtable)dserResult.arguments[0];
            Assert.NotNull(arg0, "Deserializing arg 0 returned a null object");

            Hashtable car0 = (Hashtable)arg0["car"];
            Assert.NotNull(car0, "Deserializing car from arg 0 returned a null object");

            Assert.Equal(arg0["name"] as string, "Monica", $"arg0.name has unexpected value: {arg0["name"] as string}");
            Assert.Equal((int)arg0["age"], 22, $"arg0.age has unexpected value: {(int)arg0["age"]}");
            Assert.Equal((int)arg0["gender"], 1, $"arg0.gender has unexpected value: {(int)arg0["gender"]}");

            Assert.Equal((int)car0["age"], 5, $"car0.age has unexpected value: {(int)car0["age"]}");
            Assert.Equal(car0["model"] as string, "Tesla", $"car0.model has unexpected value: {car0["model"] as string}");

            Hashtable arg1 = (Hashtable)dserResult.arguments[1];
            Assert.NotNull(arg1, "Deserializing arg 1 returned a null object");

            Hashtable car1 = (Hashtable)arg1["car"];
            Assert.NotNull(car1, "Deserializing car from arg 1 returned a null object");

            Assert.Equal(arg1["name"] as string, "Grandpa", $"arg1.name has unexpected value: {arg1["name"] as string}");
            Assert.Equal((int)arg1["age"], 88, $"arg1.age has unexpected value: {(int)arg1["age"]}");
            Assert.Equal((int)arg1["gender"], 0, $"arg1.gender has unexpected value: {(int)arg1["gender"]}");

            Assert.Equal((int)car1["age"], 35, $"car1.age has unexpected value: {(int)car1["age"]}");
            Assert.Equal(car1["model"] as string, "Buick", $"car1.model has unexpected value: {car1["model"] as string}");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_04()
        {
            var startTimestamp = Environment.TickCount64;

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testInvocationReceiveMessage, typeof(InvocationReceiveMessage));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(dserResult, "Deserialization returned a null object");

            Assert.Equal(dserResult.type, 1, "type value is not correct");
            Assert.Equal(dserResult.target, "ReceiveAdvancedMessage", "target value is not correct");

            Assert.Equal((int)dserResult.arguments[2], 3, "arguments[2] value is not correct");
            Assert.IsType(typeof(ArrayList), dserResult.arguments, "arguments type it's wrong after deserialization");
            Assert.Equal(dserResult.arguments.Count, 3, $"number of arguments is different than expected: {dserResult.arguments.Count}");

            Person2 person1 = (Person2)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[0]), typeof(Person2));
            Assert.NotNull(person1, "Deserializing person1 returned a null object");

            Person2 person2 = (Person2)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[1]), typeof(Person2));
            Assert.NotNull(person2, "Deserializing person2 returned a null object");

            Assert.Equal(person1.name, "Monica", $"person1.name has unexpected value: {person1.name}");
            Assert.Equal(person1.age, 22, $"person1.age has unexpected value: {person1.age}");
            Assert.Equal((int)person1.gender, (int)Gender.Female, $"person1.gender has unexpected value: {person1.gender}");

            Assert.Equal(person1.car.age, 5, $"person1.car.age has unexpected value: {person1.car.age}");
            Assert.Equal(person1.car.model, "Tesla", $"person1.car.model has unexpected value: {person1.car.model}");


            Assert.Equal(person2.name, "Grandpa", $"person2.name has unexpected value: {person2.name}");
            Assert.Equal(person2.age, 88, $"person2.age has unexpected value: {person2.age}");
            Assert.Equal((int)person2.gender, (int)Gender.Male, $"person2.gender has unexpected value: {person2.gender}");

            Assert.Equal(person2.car.age, 35, $"person2.car.age has unexpected value: {person2.car.age}");
            Assert.Equal(person2.car.model, "Buick", $"person2.car.model has unexpected value: {person2.car.model}");

            int argsCount = (int)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[2]), typeof(int));

            Assert.Equal(argsCount, 3, $"argsCount has unexpected value: {argsCount}");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_05()
        {
            var startTimestamp = Environment.TickCount64;

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(@"{""type"":1,""target"":""ReceiveMessage"",""arguments"":[""I_am_a_string"",""I_am_another_string""]}", typeof(InvocationReceiveMessage));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(dserResult, "Deserialization returned a null object");

            Assert.Equal(dserResult.type, 1, "type value is not correct");
            Assert.Equal(dserResult.target, "ReceiveMessage", "target value is not correct");

            Assert.IsType(typeof(ArrayList), dserResult.arguments, "arguments type it's wrong after deserialization");
            Assert.Equal(dserResult.arguments.Count, 2, $"number of arguments is different than expected: {dserResult.arguments.Count}");

            string arg0 = (string)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[0]), typeof(string));
            string arg1 = (string)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[1]), typeof(string));

            Assert.Equal(arg0, "I_am_a_string", $"arg0 has unexpected value: {arg0}");
            Assert.Equal(arg1, "I_am_another_string", $"arg1 has unexpected value: {arg1}");
        }

        [TestMethod]
        public void CanDeserializeUnicodeData_01()
        {
            var startTimestamp = Environment.TickCount64;

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(@"{ ""type"":3,""invocationId"":""1"",""error"":""Failed to invoke \u0027SendMessage\u0027 due to an error on the server. HubException: Method does not exist.""}", typeof(InvocationReceiveMessage));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.NotNull(dserResult, "Deserialization returned a null object");

            Assert.Equal(dserResult.type, 3, "type value is not correct");
            Assert.Equal(dserResult.invocationId, "1", "invocationId value is not correct");
            Assert.Equal(dserResult.error, "Failed to invoke \u0027SendMessage\u0027 due to an error on the server. HubException: Method does not exist.", "error value is not correct");
        }

        [TestMethod]
        public void SerializeArrayListInAnObject()
        {
            var invocMessage = new InvocationSendMessage
            {
                type = 1,
                invocationId = "0",
                arguments = new ArrayList() { 1, 2 },
                target = "Add"
            };

            var startTimestamp = Environment.TickCount64;

            var sentMessage = JsonConvert.SerializeObject(invocMessage);

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            Assert.Equal(@"{""type"":1,""invocationId"":""0"",""arguments"":[1,2],""target"":""Add""}", sentMessage);
        }

        [TestMethod]
        public void DeserializeArrayList()
        {
            string correctValue = "{\"desired\":{\"Url\":\"https://ellerbachiotstorage.blob.core.windows.net/nano-containers\"," +
                "\"Authorization\":\"sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D\"," +
                "\"Files\":[\"Iot.Device.Bmxx80.pe\"]}}";

            var startTimestamp = Environment.TickCount64;

            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(correctValue, typeof(Hashtable));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Hashtable desired = (Hashtable)hash["desired"];

            Assert.IsType(typeof(string), desired["Authorization"], "Authorization is not a string and it should be.");

            Assert.Equal("sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D", (string)desired["Authorization"], "Authorization string doesn't match original value.");

            ArrayList files = (ArrayList)desired["Files"];

            Assert.IsType(typeof(string), files[0]);
            Assert.Equal("Iot.Device.Bmxx80.pe", (string)files[0]);
        }

        [TestMethod]
        public void DeserializeArrayListElements()
        {
            string correctValue = "{\"Url\":\"https://ellerbachiotstorage.blob.core.windows.net/nano-containers\"," +
                "\"Authorization\":\"sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D\"," +
                "\"Files\":[\"Iot.Device.Bmxx80.pe\"]}";

            var startTimestamp = Environment.TickCount64;

            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(correctValue, typeof(Hashtable));

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.IsType(typeof(string), hash["Authorization"], "Authorization is not a string and it should be.");
            Assert.Equal("sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D", (string)hash["Authorization"], "Authorization string doesn't match original value.");

            ArrayList files = (ArrayList)hash["Files"];

            Assert.IsType(typeof(string), files[0]);
            Assert.Equal("Iot.Device.Bmxx80.pe", (string)files[0]);
        }

        [TestMethod]
        public void SerializeObjectAsAProperty()
        {
            var correctValue = "{\"Led\":{\"nodeID\":\"14\",\"value\":\"On\"}}";
            JsonSerializeObjectAsProperty ledProp = new() { value = "On", nodeID = "14" };

            Hashtable twin = new();
            twin.Add("Led", ledProp);

            var startTimestamp = Environment.TickCount64;

            string json = JsonConvert.SerializeObject(twin);

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            Assert.Equal(correctValue, json, "Serialize object as property fails");
        }

        [TestMethod]
        public void DeserializeSingleTypesClassDeserialization()
        {
            var json = "{\"OneByte\":42,\"OneSByte\":-42,\"OneInt16\":1234,\"OneUInt16\":5678,\"OneInt32\":-789012,\"OneUInt32\":78912,\"OneInt64\":-1234567,\"OneUInt64\":1234567,\"OneSingle\":34.45,\"OneDouble\":45678.23,\"OneBoolean\":true,\"TwoBoolean\":false}";

            var startTimestamp = Environment.TickCount64;

            var deser = JsonConvert.DeserializeObject(json, typeof(SingleTypesClassDeserialization)) as SingleTypesClassDeserialization;

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.Equal((byte)42, deser.OneByte, "Byte");
            Assert.Equal((sbyte)-42, deser.OneSByte, "SByte");
            Assert.Equal((short)1234, deser.OneInt16, "Int16");
            Assert.Equal((ushort)5678, deser.OneUInt16, "UInt16");
            Assert.Equal(-789012, deser.OneInt32, "Int32");
            Assert.Equal((uint)78912, deser.OneUInt32, "UInt32");
            Assert.Equal((long)-1234567, deser.OneInt64, "Int64");
            Assert.Equal((ulong)1234567, deser.OneUInt64, "UInt64");
            Assert.Equal((float)34.45, deser.OneSingle, "Single");
            Assert.Equal((double)45678.23, deser.OneDouble, "Double");
            Assert.True(deser.OneBoolean, "Boolean true");
            Assert.False(deser.TwoBoolean, "Boolean false");
        }

        [TestMethod]
        public void DeserializeArrayToDeserialize()
        {
            ArrayToDeserialize obj0 = new() { Prop1 = 1, Prop2 = "prop2", Prop3 = true, Prop4 = 67890123 };
            ArrayToDeserialize obj1 = new() { Prop1 = -42, Prop2 = "second2", Prop3 = false, Prop4 = 123456 };
            ArrayToDeserialize[] array = new[] { obj0, obj1 };

            var startTimestamp = Environment.TickCount64;

            var json = JsonConvert.SerializeObject(array);

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Serialization took {endTimestamp - startTimestamp}ms");

            startTimestamp = Environment.TickCount64;

            var deser = JsonConvert.DeserializeObject(json, typeof(ArrayToDeserialize[])) as ArrayToDeserialize[];

            endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");

            Assert.Equal(deser.Length, array.Length, "Array length");
            Assert.Equal(deser[0].Prop1, obj0.Prop1);
            Assert.Equal(deser[0].Prop2, obj0.Prop2);
            Assert.Equal(deser[0].Prop3, obj0.Prop3);
            Assert.Equal(deser[0].Prop4, obj0.Prop4);
            Assert.Equal(deser[1].Prop1, obj1.Prop1);
            Assert.Equal(deser[1].Prop2, obj1.Prop2);
            Assert.Equal(deser[1].Prop3, obj1.Prop3);
            Assert.Equal(deser[1].Prop4, obj1.Prop4);
        }

        [TestMethod]
        public void LongMaxValue()
        {
            SingleTypesClassDeserialization singleUInt64 = new() { OneUInt64 = ulong.MaxValue };
            SingleTypesClassDeserialization singleInt64 = new() { OneInt64 = long.MaxValue };
            SingleTypesClassDeserialization singleUInt32 = new() { OneUInt32 = uint.MaxValue };
            SingleTypesClassDeserialization singleInt32 = new() { OneInt32 = int.MaxValue };
            var serUInt64 = JsonConvert.SerializeObject(singleUInt64);
            var serInt64 = JsonConvert.SerializeObject(singleInt64);
            var serUInt32 = JsonConvert.SerializeObject(singleUInt32);
            var serInt32 = JsonConvert.SerializeObject(singleInt32);

            var deserUInt64 = JsonConvert.DeserializeObject(serUInt64, typeof(SingleTypesClassDeserialization)) as SingleTypesClassDeserialization;
            var deserInt64 = JsonConvert.DeserializeObject(serInt64, typeof(SingleTypesClassDeserialization)) as SingleTypesClassDeserialization;
            var deserUInt32 = JsonConvert.DeserializeObject(serUInt32, typeof(SingleTypesClassDeserialization)) as SingleTypesClassDeserialization;
            var deserInt32 = JsonConvert.DeserializeObject(serInt32, typeof(SingleTypesClassDeserialization)) as SingleTypesClassDeserialization;

            Assert.Equal(deserUInt64.OneUInt64, singleUInt64.OneUInt64);
            Assert.Equal(deserUInt64.OneInt64, singleUInt64.OneInt64);
            Assert.Equal(deserUInt64.OneUInt32, singleUInt64.OneUInt32);
            Assert.Equal(deserUInt64.OneInt32, singleUInt64.OneInt32);
        }

        [TestMethod]
        public void CompleHashtableArraysList()
        {
            string json = @"{""desired"":{""TimeToSleep"":2,""Files"":[{""FileName"":""https://ellerbachiotstorage.blob.core.windows.net/nano-containers/CountMeasurement.pe"",""Signature"":""4E-1E-12-45-C5-EB-EC-E3-86-D3-09-39-AE-E9-E8-81-97-A9-0E-DF-EE-D0-71-27-A7-3F-26-D0-4B-4E-CF-23""}],""Token"":""sp=r&st=2022-02-12T12:32:10Z&se=2023-11-01T20:32:10Z&spr=https&sv=2020-08-04&sr=c&sig=O32denO9Hw8mZ2OlNSBS%2FULuRn9RcArGDZ5%2BGvKgolM%3D"",""CodeVersion"":12,""UpdateTime"":120000,""$version"":43},""reported"":{""Firmware"":""nanoFramework"",""Sdk"":0.2,""TimeToSleep"":2,""CodeUpdated"":true,""CodeRunning"":true,""$version"":4353}}";

            var startTimestamp = Environment.TickCount64;

            Hashtable deser = (Hashtable)JsonConvert.DeserializeObject(json, typeof(Hashtable));


            Assert.NotNull(deser, "Deserialization returned a null object");

            var endTimestamp = Environment.TickCount64;
            Debug.WriteLine($"Deserialization took {endTimestamp - startTimestamp}ms");
        }

        private static string testInvocationReceiveMessage = @"{
        ""type"":1,
        ""target"":""ReceiveAdvancedMessage"",
        ""arguments"": [
            {
                ""age"":22,
                ""name"":""Monica"",
                ""gender"":1,
                ""car"":{
                    ""age"":5,
                    ""model"":""Tesla""
                }
            },
            {
                ""age"":88,
                ""name"":""Grandpa"",
                ""gender"":0,
                ""car"":{
                    ""age"":35,
                    ""model"":""Buick""
                }
            },
            3
        ]}";

        private static string s_AzureTwinsJsonTestPayload = @"{
    ""deviceId"": ""nanoDeepSleep"",
    ""etag"": ""AAAAAAAAAAc="",
    ""deviceEtag"": ""Njc2MzYzMTQ5"",
    ""status"": ""enabled"",
    ""statusUpdateTime"": ""0001-01-01T00:00:00Z"",
    ""connectionState"": ""Disconnected"",
    ""lastActivityTime"": ""2021-06-03T05:52:41.4683112Z"",
    ""cloudToDeviceMessageCount"": 0,
    ""authenticationType"": ""sas"",
    ""x509Thumbprint"": {
                ""primaryThumbprint"": null,
        ""secondaryThumbprint"": null
    },
    ""modelId"": """",
    ""version"": 381,
    ""properties"": {
                ""desired"": {
                    ""TimeToSleep"": 30,
            ""$metadata"": {
                        ""$lastUpdated"": ""2021-06-03T05:37:11.8120413Z"",
                ""$lastUpdatedVersion"": 7,
                ""TimeToSleep"": {
                            ""$lastUpdated"": ""2021-06-03T05:37:11.8120413Z"",
                    ""$lastUpdatedVersion"": 7
                }
                    },
            ""$version"": 7
                },
        ""reported"": {
                    ""Firmware"": ""nanoFramework"",
            ""TimeToSleep"": 30,
            ""$metadata"": {
                        ""$lastUpdated"": ""2021-06-03T05:52:41.1232797Z"",
                ""Firmware"": {
                            ""$lastUpdated"": ""2021-06-03T05:52:41.1232797Z""
                },
                ""TimeToSleep"": {
                            ""$lastUpdated"": ""2021-06-03T05:52:41.1232797Z""
                }
                    },
            ""$version"": 374
        }
            },
    ""capabilities"": {
                ""iotEdge"": false
    }
        }";

    }

    #region Test classes

    public class TwinPayload
    {
        public string deviceId { get; set; }
        public string etag { get; set; }
        public string status { get; set; }
        public DateTime statusUpdateTime { get; set; }
        public string connectionState { get; set; }
        public DateTime lastActivityTime { get; set; }
        public int cloudToDeviceMessageCount { get; set; }
        public string authenticationType { get; set; }
        public Hashtable x509Thumbprint { get; set; }
        public string modelId { get; set; }
        public int version { get; set; }
        public TwinProperties properties { get; set; }
    }

    public class TwinPayloadProperties
    {
        public TwinProperties properties { get; set; }
    }

    public class TwinProperties
    {
        public Desired desired { get; set; }
        public Reported reported { get; set; }
    }
    public class Desired
    {
        public int TimeToSleep { get; set; }
        public Hashtable _metadata { get; set; }
    }

    public class Reported
    {
        public string Firmware { get; set; }

        public int TimeToSleep { get; set; }

        public Hashtable _metadata { get; set; }

        public int _version { get; set; }
    }

    public class InvocationReceiveMessage
    {
        public int type { get; set; }
        public Hashtable headers { get; set; }
        public string invocationId { get; set; }
        public string target { get; set; }
        public ArrayList arguments { get; set; }
        public string[] streamIds { get; set; }
        public string error { get; set; }
        public bool allowReconnect { get; set; }
        public object result { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public int ID { get; set; }
        public string[] ArrayProperty { get; set; }
        public Person Friend { get; set; }
    }

    public abstract class AbstractClass
    {
        public int ID { get; set; }
        public abstract string Test { get; }
        public virtual string Test2 => "test2";
    }

    public class RealClass : AbstractClass
    {
        public override string Test => "test";
    }

    public class SingleTypesClassDeserialization
    {
        public byte OneByte { get; set; }
        public sbyte OneSByte { get; set; }
        public short OneInt16 { get; set; }
        public ushort OneUInt16 { get; set; }
        public int OneInt32 { get; set; }
        public uint OneUInt32 { get; set; }
        public long OneInt64 { get; set; }
        public ulong OneUInt64 { get; set; }
        public bool OneBoolean { get; set; }
        public float OneSingle { get; set; }
        public double OneDouble { get; set; }
        public bool TwoBoolean { get; set; }
    }

    public class ArrayToDeserialize
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public bool Prop3 { get; set; }
        public long Prop4 { get; set; }
    }

    public class Person2
    {
        public int age { get; set; }
        public string name { get; set; }
        public Gender gender { get; set; }
        public Car car { get; set; }
    }

    public class Car
    {
        public int age { get; set; }
        public string model { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class JsonTestClassChild
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
#pragma warning disable S1104 // Fields should not have public accessibility
        public int four; //not a property on purpose!
#pragma warning restore S1104 // Fields should not have public accessibility

        public override string ToString()
        {
            return $"ChildClass: one={one}, two={two}, three={three}, four={four}";
        }
    }

    public class JsonTestClassFloat
    {
        public float aFloat { get; set; }
    }

    public class JsonTestClassDouble
    {
        public double aDouble { get; set; }
    }

    public class JsonTestClassTimestamp
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public DateTime FixedTimestamp { get; set; }
    }

    public class JsonTestClassTimeSpan
    {
        public TimeSpan Duration1 { get; set; } = TimeSpan.FromMinutes(69);
        public TimeSpan Duration2 { get; set; }
        public TimeSpan Duration3 { get; set; }
        public TimeSpan Duration4 { get; set; }
        public TimeSpan Duration5 { get; set; }
        public int DummyValue1 { get; set; } = -999;
        public uint DummyValue2 { get; set; } = 777;
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

    // Classes to more thoroughly test array serialization/deserialization
    public class JsonTestCompany
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
    public class JsonTestEmployee
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public JsonTestCompany CurrentEmployer { get; set; }
        public JsonTestCompany[] FormerEmployers { get; set; }
    }
    public class JsonTestTown
    {
        public int TownID { get; set; }
        public string TownName { get; set; }
        public JsonTestCompany[] CompaniesInThisTown { get; set; }
        public JsonTestEmployee[] EmployeesInThisTown { get; set; }
    }

    public class JsonSerializeObjectAsProperty
    {
        public string value { get; set; }
        public string nodeID { get; set; }
    }

    public class InvocationSendMessage
    {
        public int type { get; set; }
        public string invocationId { get; set; }
        public string target { get; set; }
        public ArrayList arguments { get; set; }
    }

    #endregion
}
