//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Test.Shared;
using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonSerializerOptionsTests
    {
        [Setup]
        public void Initialize()
        {
            OutputHelper.WriteLine("Json Library tests initialized.");
        }

        [Cleanup]
        public void CleanUp()
        {
            OutputHelper.WriteLine("Cleaning up after Json Library tests.");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_object_with_self_referencing_static_field()
        {
            OutputHelper.WriteLine("Can_serialize_and_deserialize_object_with_self_referencing_static_field() - Starting test...");

            var serialized = JsonConvert.SerializeObject(JsonTestStaticProperty.StaticProperty);
            var deserialized = (JsonTestStaticProperty)JsonConvert.DeserializeObject(serialized, typeof(JsonTestStaticProperty));

            Assert.AreEqual(
                JsonTestStaticProperty.StaticProperty.InstanceProperty,
                deserialized.InstanceProperty,
                $"Validation: JsonTestStaticProperty.StaticProperty.InstanceProperty: {JsonTestStaticProperty.StaticProperty.InstanceProperty}");

            Assert.DoesNotContains(
                "StaticProperty",
                serialized,
                $"Validation: JsonTestStaticProperty.StaticProperty.InstanceProperty: {JsonTestStaticProperty.StaticProperty.InstanceProperty}");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_arrays_of_class_objects()
        {
            OutputHelper.WriteLine("Can_serialize_and_deserialize_arrays_of_class_objects() - Starting test...");

            JsonTestTown myTown = JsonTestTown.CreateTestClass();

            var result = JsonConvert.SerializeObject(myTown);
            JsonTestTown dserResult = (JsonTestTown)JsonConvert.DeserializeObject(result, typeof(JsonTestTown));
            OutputHelper.WriteLine($"After deserialization - type: JsonTestTown");

            Assert.AreEqual(
                myTown.TownID,
                dserResult.TownID,
                $"Validation: myTown.TownID: {myTown.TownID}");

            Assert.AreEqual(
                myTown.TownName,
                dserResult.TownName,
                $"Validation: myTown.TownName: {myTown.TownName}");

            for (int i = 0; i < myTown.CompaniesInThisTown.Length; i++)
            {
                Assert.AreEqual(
                    myTown.CompaniesInThisTown[i].CompanyID,
                    dserResult.CompaniesInThisTown[i].CompanyID,
                    $"Validation: myTown.CompaniesInThisTown[{i}].CompanyID: {myTown.CompaniesInThisTown[i].CompanyID}");

                Assert.AreEqual(
                    myTown.CompaniesInThisTown[i].CompanyName,
                    dserResult.CompaniesInThisTown[i].CompanyName,
                    $"Validation: myTown.CompaniesInThisTown[{i}].CompanyName: {myTown.CompaniesInThisTown[i].CompanyName}");
            }

            for (int i = 0; i < myTown.EmployeesInThisTown.Length; i++)
            {
                Assert.AreEqual(
                    myTown.EmployeesInThisTown[i].EmployeeID,
                    dserResult.EmployeesInThisTown[i].EmployeeID,
                    $"Validation: myTown.EmployeesInThisTown[{i}].EmployeeID: {myTown.EmployeesInThisTown[i].EmployeeID} ");

                Assert.AreEqual(
                    myTown.EmployeesInThisTown[i].EmployeeName,
                    dserResult.EmployeesInThisTown[i].EmployeeName,
                    $"Validation: myTown.EmployeesInThisTown[{i}].EmployeeName: {myTown.EmployeesInThisTown[i].EmployeeName} ");

                Assert.AreEqual(
                    myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyID,
                    dserResult.EmployeesInThisTown[i].CurrentEmployer.CompanyID,
                    $"Validation: myTown.EmployeesInThisTown[{i}].CurrentEmployer.CompanyID: {myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyID} ");

                Assert.AreEqual(
                    myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyName,
                    dserResult.EmployeesInThisTown[i].CurrentEmployer.CompanyName,
                    $"Validation: myTown.EmployeesInThisTown[{i}].CurrentEmployer.CompanyName: {myTown.EmployeesInThisTown[i].CurrentEmployer.CompanyName} ");

                for (int j = 0; j < myTown.EmployeesInThisTown[i].FormerEmployers.Length; j++)
                {
                    Assert.AreEqual(
                        myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyID,
                        dserResult.EmployeesInThisTown[i].FormerEmployers[j].CompanyID,
                        $"Validation: myTown.EmployeesInThisTown[{i}].FormerEmployers[{j}].CompanyID: {myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyID} ");

                    Assert.AreEqual(
                        myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyName,
                        dserResult.EmployeesInThisTown[i].FormerEmployers[j].CompanyName,
                        $"Validation: myTown.EmployeesInThisTown[{i}].FormerEmployers[{j}].CompanyName: {myTown.EmployeesInThisTown[i].FormerEmployers[j].CompanyName} ");
                }
            }

            OutputHelper.WriteLine("Can_serialize_and_deserialize_arrays_of_class_objects() - Finished - test succeeded.");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_arrays_of_class_objects_when_array_items_may_be_null()
        {
            OutputHelper.WriteLine("Starting test...");

            Console.WriteLine($"{TimeSpan.FromHours(1).TotalMilliseconds}");

            JsonTestCompany first = new JsonTestCompany
            {
                CompanyID = 1,
                CompanyName = "foo"
            };

            JsonTestCompany second = null;

            JsonTestCompany third = new JsonTestCompany
            {
                CompanyID = 3,
                CompanyName = "foo3"
            };

            JsonTestCompany[] test = new JsonTestCompany[]
                    {
                    first,
                    second,
                    third,
                    null,
                    null,
                    };

            string result = JsonConvert.SerializeObject(test);

            JsonTestCompany[] deserializedResult = (JsonTestCompany[])JsonConvert.DeserializeObject(result, typeof(JsonTestCompany[]));

            Console.WriteLine($"dserResult== null: {deserializedResult == null}");

            Assert.IsNotNull(deserializedResult);

            Assert.AreEqual(deserializedResult[0].CompanyID, first.CompanyID);
            Assert.AreEqual(deserializedResult[0].CompanyName, first.CompanyName);

            Assert.IsNull(deserializedResult[1]);

            Assert.AreEqual(deserializedResult[2].CompanyID, third.CompanyID);
            Assert.AreEqual(deserializedResult[2].CompanyName, third.CompanyName);

            Assert.IsNull(deserializedResult[3]);
            Assert.IsNull(deserializedResult[4]);

            Assert.AreEqual(deserializedResult.Length, 5);

            OutputHelper.WriteLine("Finished test...");
        }

        [TestMethod]
        public void Can_serialize_int_array()
        {
            OutputHelper.WriteLine("Can_serialize_int_array() - Starting test...");
            int[] intArray = new[] { 1, 3, 5, 7, 9 };

            var result = JsonConvert.SerializeObject(intArray);
            var dserResult = (int[])JsonConvert.DeserializeObject(result, typeof(int[]));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            CollectionAssert.AreEqual(intArray, dserResult);

            OutputHelper.WriteLine("Can_serialize_int_array() - Finished - test succeeded.");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timestamp()
        {
            OutputHelper.WriteLine("Can_serialize_deserialize_timestamp() - Starting test...");

            var timestampTests = new JsonTestClassTimestamp()
            {
                Timestamp = DateTime.UtcNow,
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00)
            };

            OutputHelper.WriteLine($"fixed timestamp used for test = {timestampTests.FixedTimestamp}");
            OutputHelper.WriteLine($"variable timestamp used for test = {timestampTests.Timestamp}");

            var result = JsonConvert.SerializeObject(timestampTests);
            var dserResult = (JsonTestClassTimestamp)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimestamp));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            Assert.AreEqual(timestampTests.FixedTimestamp.ToString(), dserResult.FixedTimestamp.ToString()); //cannot handle DateTime, so use ToString()
            Assert.AreEqual(timestampTests.Timestamp.ToString(), dserResult.Timestamp.ToString()); //cannot handle DateTime, so use ToString()

            OutputHelper.WriteLine("Can_serialize_deserialize_timestamp() - Finished - test succeeded.");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timespan_00()
        {
            OutputHelper.WriteLine("Can_serialize_deserialize_timespan_00() - Starting test...");

            var randomValue = new Random();

            var timeSpanTests = new JsonTestClassTimeSpan()
            {
                Duration2 = TimeSpan.FromSeconds(randomValue.Next(59)),
                Duration3 = TimeSpan.FromMilliseconds(randomValue.Next(999)),
                Duration4 = TimeSpan.FromHours(randomValue.Next(99)),
                Duration5 = TimeSpan.FromDays(randomValue.Next(999)),
            };

            OutputHelper.WriteLine($"Fixed timespan used for test = {timeSpanTests.Duration1}");
            OutputHelper.WriteLine($"variable timespan 2 used for test = {timeSpanTests.Duration2}");
            OutputHelper.WriteLine($"variable timespan 3 used for test = {timeSpanTests.Duration3}");
            OutputHelper.WriteLine($"variable timespan 4 used for test = {timeSpanTests.Duration4}");
            OutputHelper.WriteLine($"variable timespan 5 used for test = {timeSpanTests.Duration5}");

            var result = JsonConvert.SerializeObject(timeSpanTests);
            var dserResult = (JsonTestClassTimeSpan)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimeSpan));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            Assert.AreEqual(timeSpanTests.Duration1.ToString(), dserResult.Duration1.ToString(), $"wrong value for Duration1, expected 1:09:00, got {dserResult.Duration1}");
            Assert.AreEqual(timeSpanTests.Duration2.Ticks.ToString(), dserResult.Duration2.Ticks.ToString(), $"wrong value for Duration2, expected {timeSpanTests.Duration2}, got {dserResult.Duration2}");
            Assert.AreEqual(timeSpanTests.Duration3.Ticks.ToString(), dserResult.Duration3.Ticks.ToString(), $"wrong value for Duration3, expected {timeSpanTests.Duration3}, got {dserResult.Duration3}");
            Assert.AreEqual(timeSpanTests.Duration4.Ticks.ToString(), dserResult.Duration4.Ticks.ToString(), $"wrong value for Duration4, expected {timeSpanTests.Duration4}, got {dserResult.Duration4}");
            Assert.AreEqual(timeSpanTests.Duration5.Ticks.ToString(), dserResult.Duration5.Ticks.ToString(), $"wrong value for Duration5, expected {timeSpanTests.Duration5}, got {dserResult.Duration5}");

            OutputHelper.WriteLine("Can_serialize_deserialize_timespan_00() - Finished - test succeeded.");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_deserialize_timespan_01()
        {
            OutputHelper.WriteLine("Can_serialize_deserialize_timespan_01() - Starting test...");

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

            for (int i = 0; i < _timeSpans.Length; i++)
            {
                var result = JsonConvert.SerializeObject(_timeSpans[i]);
                var dserResult = (JsonTestClassTimeSpan)JsonConvert.DeserializeObject(result, typeof(JsonTestClassTimeSpan));

                OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

                // can't compare TimeSpans directly, using ticks
                Assert.AreEqual(_timeSpans[i].Duration2.Ticks, dserResult.Duration2.Ticks, $"wrong value, expected {_timeSpans[i].Duration2}, got {dserResult.Duration2}");
            }

            OutputHelper.WriteLine("Can_serialize_deserialize_timespan_01() - Finished - test succeeded.");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_short_array()
        {
            OutputHelper.WriteLine("Can_serialize_short_array() - Starting test...");
            short[] shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 };

            var result = JsonConvert.SerializeObject(shortArray);
            var dserResult = (short[])JsonConvert.DeserializeObject(result, typeof(short[]));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            CollectionAssert.AreEqual(shortArray, dserResult);

            OutputHelper.WriteLine("Can_serialize_short_array() - Finished - test succeeded.");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_simple_object()
        {
            OutputHelper.WriteLine("Can_serialize_and_deserialize_simple_object() - Starting test...");
            var source = new JsonTestClassChild()
            {
                one = 1,
                two = 2,
                three = 3,
                four = 4,
            };


            var serialized = JsonConvert.SerializeObject(source);
            var dserResult = (JsonTestClassChild)JsonConvert.DeserializeObject(serialized, typeof(JsonTestClassChild));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");
            OutputHelper.WriteLine("Can_serialize_and_deserialize_simple_object() - Finished - test succeeded");
            OutputHelper.WriteLine("");
        }

        public class ThingWithString
        {
            public string Value { get; set; }
        }

        [TestMethod]
        [DataRow("a")]  // Single character
        [DataRow("1")]  // Single numeric character
        [DataRow("\t")]  // Single Tab character
        [DataRow("Testing / solidus")] // Forward slash in string
        [DataRow("Testing  solidus")] // Double space in string
        [DataRow("Some string with \" that needs escaping")] // String containing a quote
        [DataRow("Quotes in a \"string\".")] // String with escaped quotes
        [DataRow("Escaped last character \n")] // Newline as the last character
        [DataRow("I:\\Nano\\rApp\\app.pe")] // Backslash in string
        [DataRow("Tab \t in a string \t")] // Tab character in multiple places
        [DataRow("Newline \n in a string \n")] // Newline character in multiple places
        [DataRow("LineFeed \f in a string \f")] // Line feed character in multiple places
        [DataRow("CarriageReturn \r in a string \r")] // Carriage return character in multiple places
        [DataRow("Backspace \b in a string \b")] // Backspace character in multiple places
        [DataRow("TestString")] // Simple string with no special characters
        [DataRow("\"TestString\"")] // String wrapped in quotes
        [DataRow("\u0041")] // Unicode character (A)
        [DataRow("\u2764")] // Unicode character (❤)
        [DataRow("\x1B")] // Escape character (ASCII 27)
        [DataRow("\x7F")] // Delete character (ASCII 127)
        [DataRow("\0")] // Null character
        [DataRow("")] // Empty string
        [DataRow("Line 1\nLine 2\nLine 3")] // Multi-line string
        [DataRow("Curly braces: { }")] // JSON-like curly braces
        [DataRow("Square brackets: [ ]")] // JSON-like square brackets
        [DataRow("Colon and comma: : ,")] // Colon and comma
        [DataRow("Special symbols: @#$%^&*()_+~")] // Special symbols
        [DataRow("English 中文 Español العربية हिंदी")] // Mixed language text
        [DataRow("{\"key\": \"value\"}")] // JSON-like string
        [DataRow("\"[{\"inner\":\"value\"}]\"")] // Serialized JSON-like string
        [DataRow("{\"name\":\"John\",\"age\":30}")] // Serialized JSON
        [DataRow("Invalid escape: \\q")] // Invalid escape sequence
        [DataRow("https://example.com/api?query=escaped%20characters")] // URL
        [DataRow("Unicode \u2764, Newline \n, Tab \t, Backslash \\")] // Combination of cases
        public void Can_serialize_and_deserialize_object_containing_string_with_escaped_characters(string testValue)
        {
            var thing = new ThingWithString
            {
                Value = testValue
            };

            Console.WriteLine("Original: " + testValue);

            var serialized = JsonConvert.SerializeObject(thing);
            Console.WriteLine("Serialized: " + serialized);

            var deserialized = (ThingWithString)JsonConvert.DeserializeObject(serialized, typeof(ThingWithString));
            Console.WriteLine("Deserialized: " + deserialized.Value);

            Assert.AreEqual(thing.Value, deserialized.Value);
        }

        [TestMethod]
        [DataRow("a")] // Single character
        [DataRow("\t")] // Tab character
        [DataRow("Testing / solidus")] // Forward slash
        [DataRow("Testing  solidus")] // Double space
        [DataRow("Quotes in a \"string\".")] // String with escaped quotes
        [DataRow("Escaped last character \n")] // Newline at the end
        [DataRow("I:\\Nano\\rApp\\app.pe")] // Backslash in string
        [DataRow("Tab \t in a string \ta")] // Tab character in multiple places
        [DataRow("Newline \n in a string \na")] // Newline character in multiple places
        [DataRow("LineFeed \f in a string \fa")] // Line feed character
        [DataRow("CarriageReturn \r in a string \ra")] // Carriage return character
        [DataRow("Backspace \b in a string \ba")] // Backspace character
        [DataRow("TestString")] // Simple string
        [DataRow("\"TestString\"")] // String wrapped in quotes
        [DataRow("\u0041")] // Unicode character (A)
        [DataRow("\u2764")] // Unicode character (❤)
        [DataRow("\x1B")] // Escape character (ASCII 27)
        [DataRow("\x7F")] // Delete character (ASCII 127)
        [DataRow("\0")] // Null character
        [DataRow("")] // Empty string
        [DataRow("Line 1\nLine 2\nLine 3")] // Multi-line string
        [DataRow("Curly braces: { }")] // JSON-like curly braces
        [DataRow("Square brackets: [ ]")] // JSON-like square brackets
        [DataRow("Colon and comma: : ,")] // Colon and comma
        [DataRow("Special symbols: @#$%^&*()_+~")] // Special symbols
        [DataRow("English 中文 Español العربية हिंदी")] // Mixed language text
        [DataRow("{\"key\": \"value\"}")] // JSON-like string
        [DataRow("\"[{\"inner\":\"value\"}]\"")] // Serialized JSON-like string
        [DataRow("{\"name\":\"John\",\"age\":30}")] // Serialized JSON
        [DataRow("Invalid escape: \\q")] // Invalid escape sequence
        [DataRow("https://example.com/api?query=escaped%20characters")] // URL
        [DataRow("Unicode \u2764, Newline \n, Tab \t, Backslash \\")] // Combination of cases
        [DataRow("\"\\\"TestJson\\\"\"")] // Double escaped string
        public void Can_serialize_and_deserialize_string_with_escaped_characters(string testValue)
        {
            Console.WriteLine("Original: " + testValue);

            var serialized = JsonConvert.SerializeObject(testValue);
            Console.WriteLine("Serialized: " + serialized);

            var deserialized = (string)JsonConvert.DeserializeObject(serialized, typeof(string));
            Console.WriteLine("Deserialized: " + deserialized);

            Assert.AreEqual(testValue, deserialized);
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_complex_object()
        {
            OutputHelper.WriteLine("Can_serialize_and_deserialize_complex_object() - Starting test...");
            var test = JsonTestClassComplex.CreateTestClass();

            var result = JsonConvert.SerializeObject(test);
            var dserResult = (JsonTestClassComplex)JsonConvert.DeserializeObject(result, typeof(JsonTestClassComplex));

            OutputHelper.WriteLine($"After Type deserialization:");
            OutputHelper.WriteLine($"   aString:   {dserResult.aString} ");
            OutputHelper.WriteLine($"   aInteger:  {dserResult.aInteger} ");
            OutputHelper.WriteLine($"   aByte:     {dserResult.aByte} ");
            OutputHelper.WriteLine($"   Timestamp: {dserResult.Timestamp} ");
            OutputHelper.WriteLine($"   FixedTimestamp: {dserResult.FixedTimestamp} ");
            Debug.Write($"   intArray: ");

            foreach (int i in dserResult.intArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   stringArray: ");
            foreach (string i in dserResult.stringArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   shortArray: ");
            foreach (short i in dserResult.shortArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   byteArray: ");
            foreach (byte i in dserResult.byteArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   floatArray: ");
            foreach (float i in dserResult.floatArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            Debug.Write($"   doubleArray: ");
            foreach (double i in dserResult.doubleArray)
            {
                Debug.Write($"{i}, ");
            }
            OutputHelper.WriteLine("");

            OutputHelper.WriteLine($"   child1: {dserResult.child1} ");
            OutputHelper.WriteLine($"   Child: {dserResult.Child} ");

            if (dserResult.nullObject == null)
            {
                OutputHelper.WriteLine($"   nullObject is null");
            }
            else
            {
                OutputHelper.WriteLine($"   nullObject: {dserResult.nullObject}");
            }
            OutputHelper.WriteLine($"   nanFloat: {dserResult.nanFloat} ");
            OutputHelper.WriteLine($"   nanDouble: {dserResult.nanDouble} ");
            OutputHelper.WriteLine($"   aFloat: {dserResult.aFloat} ");
            OutputHelper.WriteLine($"   aDouble: {dserResult.aDouble} ");
            OutputHelper.WriteLine($"   aBoolean: {dserResult.aBoolean} ");

            OutputHelper.WriteLine("Can_serialize_and_deserialize_complex_object() - Finished - test succeeded");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_float()
        {
            OutputHelper.WriteLine("Starting float Object Test...");
            var test = new JsonTestClassFloat()
            {
                aFloat = 2567.454f, //BUG: Deserialized float fails when number is greater than 3-4 DP with an extra `.` at the end.
            };

            var result = JsonConvert.SerializeObject(test);
            var dserResult = (JsonTestClassFloat)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloat));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            Assert.AreEqual(result, "{\"aFloat\":" + test.aFloat + "}", "Serialized float result is equal");
            Assert.AreEqual(test.aFloat, dserResult.aFloat, "Deserialized float Result is Equal");

            OutputHelper.WriteLine("float Object Test Test succeeded");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_nan_float()
        {
            OutputHelper.WriteLine("Starting float NaN Object Test...");
            var test = new JsonTestClassFloat()
            {
                aFloat = float.NaN,
            };

            var result = JsonConvert.SerializeObject(test);
            var dserResult = (JsonTestClassFloat)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloat));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            Assert.AreEqual(result, "{\"aFloat\":null}", "Serialized float result is null");
            Assert.IsTrue(float.IsNaN(dserResult.aFloat), "Deserialized float Result is NaN");

            OutputHelper.WriteLine("float Object Test Test succeeded");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_double()
        {
            OutputHelper.WriteLine("Starting double Object Test...");
            var test = new JsonTestClassDouble()
            {
                aDouble = 123.4567,
            };

            var result = JsonConvert.SerializeObject(test);
            var dserResult = (JsonTestClassDouble)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDouble));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            Assert.AreEqual(result, "{\"aDouble\":123.4567}", "Serialized double result is a double");

            OutputHelper.WriteLine("double Object Test Test succeeded");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_nan_double()
        {
            OutputHelper.WriteLine("Starting double NaN Object Test...");
            var test = new JsonTestClassDouble()
            {
                aDouble = double.NaN,
            };

            var result = JsonConvert.SerializeObject(test);
            var dserResult = (JsonTestClassDouble)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDouble));

            OutputHelper.WriteLine($"After Type deserialization: {dserResult}");

            Assert.AreEqual(result, "{\"aDouble\":null}", "Serialized double result is null");
            Assert.AreEqual(true, double.IsNaN(dserResult.aDouble), "Deserialized double Result is NaN");

            OutputHelper.WriteLine("double NaN Object Test Test succeeded");
            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void BasicSerializationTest()
        {
            ICollection collection = new ArrayList() { 1, null, 2, "blah", false };

            Hashtable hashtable = new()
            {
                { "collection", collection },
                { "nulltest", null },
                { "stringtest", "hello world" }
            };

            object[] array = new object[] { hashtable };

            string json = JsonConvert.SerializeObject(array);
            string expectedValue = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[1,null,2,\"blah\",false]}]";

            Assert.AreEqual(json, expectedValue, "Values did not match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void BasicDeserializationTest()
        {
            string json = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[-1,null,24.565657576,\"blah\",false]}]";

            ArrayList arrayList = (ArrayList)JsonConvert.DeserializeObject(json, typeof(ArrayList));

            Hashtable hashtable = arrayList[0] as Hashtable;
            string stringtest = hashtable["stringtest"].ToString();
            object nulltest = hashtable["nulltest"];

            ArrayList collection = hashtable["collection"] as ArrayList;
            int a = (int)collection[0];
            object b = collection[1];
            double c = (double)collection[2];
            string d = collection[3].ToString();
            bool e = (bool)collection[4];

            Assert.AreEqual(arrayList.Count, 1, "arrayList count did not match");

            Assert.AreEqual(hashtable.Count, 3, "hashtable count did not match");

            Assert.AreEqual(stringtest, "hello world", "stringtest did not match");

            Assert.IsNull(nulltest, "nulltest did not match");

            Assert.AreEqual(collection.Count, 5, "collection count did not match");

            Assert.AreEqual(a, -1, "a value did not match");

            Assert.IsNull(b, "b value did not match");

            Assert.AreEqual(c, 24.565657576, "c value did not match");

            Assert.AreEqual(d, "blah", "d value did not match");

            Assert.IsFalse(e, "e value did not match");

            OutputHelper.WriteLine("");
        }

        private class TestStringNumbers
        {
            public string SmallStringNumber { get; set; }
            public string BigStringNumber { get; set; }
            public string BiggerStringNumber { get; set; }
        }

        [TestMethod]
        public void BasicDeserializationTestWithStringNumbers()
        {
            string json = "{\"SmallStringNumber\":\"1234567890\",\"BigStringNumber\":\"63805508613140626\",\"BiggerStringNumber\":\"638055086131406269\"}";

            TestStringNumbers oTestStringNumbers = (TestStringNumbers)JsonConvert.DeserializeObject(json, typeof(TestStringNumbers));

            Assert.AreEqual(oTestStringNumbers.SmallStringNumber, "1234567890", "oTestStringNumbers.SmallStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BigStringNumber, "63805508613140626", "oTestStringNumbers.BigStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BiggerStringNumber, "638055086131406269", "oTestStringNumbers.BiggerStringNumber value did not match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void BasicDeserializationTestWithStringNumbersSmall()
        {
            string json = "{\"SmallStringNumber\":\"1234567890\",\"BigStringNumber\":\"\",\"BiggerStringNumber\":\"\"}";

            TestStringNumbers oTestStringNumbers = (TestStringNumbers)JsonConvert.DeserializeObject(json, typeof(TestStringNumbers));

            Assert.AreEqual(oTestStringNumbers.SmallStringNumber, "1234567890", "oTestStringNumbers.SmallStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BigStringNumber, "", "oTestStringNumbers.BigStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BiggerStringNumber, "", "oTestStringNumbers.BiggerStringNumber value did not match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void BasicDeserializationTestWithStringNumbersBig()
        {
            string json = "{\"SmallStringNumber\":\"\",\"BigStringNumber\":\"63805508613140626\",\"BiggerStringNumber\":\"\"}";

            TestStringNumbers oTestStringNumbers = (TestStringNumbers)JsonConvert.DeserializeObject(json, typeof(TestStringNumbers));

            Assert.AreEqual(oTestStringNumbers.SmallStringNumber, "", "oTestStringNumbers.SmallStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BigStringNumber, "63805508613140626", "oTestStringNumbers.BigStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BiggerStringNumber, "", "oTestStringNumbers.BiggerStringNumber value did not match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void BasicDeserializationTestWithStringNumbersBigger()
        {
            string json = "{\"SmallStringNumber\":\"\",\"BigStringNumber\":\"\",\"BiggerStringNumber\":\"638055086131406269\"}";

            TestStringNumbers oTestStringNumbers = (TestStringNumbers)JsonConvert.DeserializeObject(json, typeof(TestStringNumbers));

            Assert.AreEqual(oTestStringNumbers.SmallStringNumber, "", "oTestStringNumbers.SmallStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BigStringNumber, "", "oTestStringNumbers.BigStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BiggerStringNumber, "638055086131406269", "oTestStringNumbers.BiggerStringNumber value did not match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void BasicDeserializationTestWithStringNumbersBiggerExtra()
        {
            string json = "{\"SmallStringNumber\":\"\",\"BigStringNumber\":\"\",\"BiggerStringNumber\":\"T638055086131406269\"}";

            TestStringNumbers oTestStringNumbers = (TestStringNumbers)JsonConvert.DeserializeObject(json, typeof(TestStringNumbers));

            Assert.AreEqual(oTestStringNumbers.SmallStringNumber, "", "oTestStringNumbers.SmallStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BigStringNumber, "", "oTestStringNumbers.BigStringNumber value did not match");
            Assert.AreEqual(oTestStringNumbers.BiggerStringNumber, "T638055086131406269", "oTestStringNumbers.BiggerStringNumber value did not match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void SerializeDeserializeDateTest()
        {
            DateTime testTime = new(2015, 04, 22, 11, 56, 39, 456);

            ICollection collection = new ArrayList() { testTime };

            string jsonString = JsonConvert.SerializeObject(collection);

            OutputHelper.WriteLine($"Json payload: {jsonString}");

            ArrayList convertTime = (ArrayList)JsonConvert.DeserializeObject(jsonString, typeof(ArrayList));

            Assert.AreEqual(testTime.Ticks, ((DateTime)convertTime[0]).Ticks, "Values did not match");

            OutputHelper.WriteLine("");
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

            string json = JsonConvert.SerializeObject(person);
            string correctValue = "{\"FirstName\":\"John\",\"LastName\":\"Doe\",\"ArrayProperty\":[\"hello\",\"world\"],\"Address\":null,\"Birthday\":\"1988-04-23T00:00:00.0000000Z\",\"ID\":27,\"Friend\":{\"FirstName\":\"Bob\",\"LastName\":\"Smith\",\"ArrayProperty\":[\"hi\",\"planet\"],\"Address\":\"123 Some St\",\"Birthday\":\"1983-07-03T00:00:00.0000000Z\",\"ID\":2,\"Friend\":null}}";

            Assert.AreEqual(json, correctValue, $"Values did not match. Expecting >>{json}<<");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void SerializeAbstractClassTest()
        {
            AbstractClass a = new RealClass() { ID = 12 };
            string json = JsonConvert.SerializeObject(a);

            string correctValue = "{\"Test2\":\"test2\",\"Test\":\"test\",\"ID\":12}";

            Assert.AreEqual(json, correctValue, $"Value for AbstractClass did not match. Got >>{json}<<.");

            RealClass b = new() { ID = 12 };

            json = JsonConvert.SerializeObject(b);

            correctValue = "{\"Test2\":\"test2\",\"Test\":\"test\",\"ID\":12}";

            Assert.AreEqual(json, correctValue, $"Values for RealClass did not match. Got >>{json}<<.");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_01()
        {
            var testString = "{\"desired\":{\"TimeToSleep\":5,\"$version\":2},\"reported\":{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}}";


            var twinPayload = (TwinProperties)JsonConvert.DeserializeObject(testString, typeof(TwinProperties));

            Assert.IsNotNull(twinPayload, "Deserialization returned a null object");

            Assert.AreEqual(twinPayload.desired.TimeToSleep, 5, "desired.TimeToSleep doesn't match");
            Assert.IsNull(twinPayload.desired._metadata, "desired._metadata doesn't match");

            Assert.AreEqual(twinPayload.reported.Firmware, "nanoFramework", "reported.Firmware doesn't match");
            Assert.AreEqual(twinPayload.reported.TimeToSleep, 2, "reported.TimeToSleep doesn't match");
            Assert.IsNull(twinPayload.reported._metadata, "reported._metadata doesn't match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_02()
        {
            TwinPayloadTestClass twinPayload = (TwinPayloadTestClass)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayloadTestClass));

            Assert.IsNotNull(twinPayload, "Deserialization returned a null object");

            Assert.AreEqual(twinPayload.authenticationType, "sas", "authenticationType doesn't match");
            Assert.AreEqual(twinPayload.statusUpdateTime.Ticks, DateTime.MinValue.Ticks, "statusUpdateTime doesn't match");
            Assert.AreEqual(twinPayload.cloudToDeviceMessageCount, 0, "cloudToDeviceMessageCount doesn't match");
            Assert.AreEqual(twinPayload.x509Thumbprint.Count, 2, "x509Thumbprint collection count doesn't match");
            Assert.AreEqual(twinPayload.version, 381, "version doesn't match");
            Assert.AreEqual(twinPayload.properties.desired.TimeToSleep, 30, "properties.desired.TimeToSleep doesn't match");
            Assert.AreEqual(twinPayload.properties.reported._metadata.Count, 3, "properties.reported._metadata collection count doesn't match");
            Assert.AreEqual(twinPayload.properties.desired._metadata.Count, 3, "properties.desired._metadata collection count doesn't match");

            OutputHelper.WriteLine("");
        }


        [TestMethod]
        public void CanDeserializeAzureTwinProperties_03()
        {
            TwinPayloadProperties twinPayload = (TwinPayloadProperties)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayloadProperties));

            Assert.IsNotNull(twinPayload, "Deserialization returned a null object");

            Assert.AreEqual(twinPayload.properties.desired.TimeToSleep, 30, "properties.desired.TimeToSleep doesn't match");
            Assert.AreEqual(twinPayload.properties.reported._metadata.Count, 3, "properties.reported._metadata collection count doesn't match");
            Assert.AreEqual(twinPayload.properties.desired._metadata.Count, 3, "properties.desired._metadata collection count doesn't match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_04()
        {
            Hashtable twinPayload = (Hashtable)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(Hashtable));

            Assert.IsNotNull(twinPayload, "Deserialization returned a null object");

            Assert.AreEqual((string)twinPayload["authenticationType"], "sas", "authenticationType doesn't match");
            Assert.AreEqual((int)twinPayload["cloudToDeviceMessageCount"], 0, "cloudToDeviceMessageCount doesn't match");
            Assert.AreEqual(((Hashtable)twinPayload["x509Thumbprint"]).Count, 2, "x509Thumbprint collection count doesn't match");
            Assert.AreEqual((int)twinPayload["version"], 381, "version doesn't match");

            // get properties
            Hashtable properties = (Hashtable)twinPayload["properties"];

            // get hastables with properties
            Hashtable reported = (Hashtable)properties["reported"];
            Hashtable desired = (Hashtable)properties["desired"];

            Assert.AreEqual((int)desired["TimeToSleep"], 30, "properties.desired.TimeToSleep doesn't match");
            Assert.AreEqual((int)desired["$version"], 7, "properties.desired.$version doesn't match");
            Assert.AreEqual((int)reported["TimeToSleep"], 30, "properties.reported.TimeToSleep doesn't match");
            Assert.AreEqual((int)reported["$version"], 374, "properties.reported.$version doesn't match");
            Assert.AreEqual((string)reported["Firmware"], "nanoFramework", "properties.reported.Firmware doesn't match");

            Hashtable reportedMetadata = (Hashtable)reported["$metadata"];
            Hashtable desiredMetadata = (Hashtable)desired["$metadata"];

            Assert.AreEqual(reportedMetadata.Count, 3, "properties.reported collection count doesn't match");
            Assert.AreEqual(desiredMetadata.Count, 3, "properties.desired collection count doesn't match");

            DateTime desiredLastUpdated = new(637582954318120413);
            DateTime reportedLastUpdated = new(637582963611232797);

            Assert.AreEqual((DateTime)reportedMetadata["$lastUpdated"], reportedLastUpdated, $"Expecting {reportedLastUpdated.ToString("o")} for properties.reported.$metadata.$lastUpdated, got {((DateTime)reportedMetadata["$lastUpdated"]).ToString("o")}");
            Assert.AreEqual((DateTime)desiredMetadata["$lastUpdated"], desiredLastUpdated, $"Expecting {desiredLastUpdated.ToString("o")} properties.desired.$metadata.$lastUpdated, got {((DateTime)desiredMetadata["$lastUpdated"]).ToString("o")}");

            Hashtable desiredTimeToSleep = (Hashtable)desiredMetadata["TimeToSleep"];

            DateTime desiredTimeToSleepUpdated = new(637582954318120413);

            Assert.AreEqual((DateTime)desiredTimeToSleep["$lastUpdated"], desiredTimeToSleepUpdated, $"Expecting {desiredTimeToSleepUpdated.ToString("o")} properties.reported.$metadata.TimeToSleep.$lastUpdated, got {((DateTime)desiredTimeToSleep["$lastUpdated"]).ToString("o")}");
            Assert.AreEqual((int)desiredTimeToSleep["$lastUpdatedVersion"], 7, "properties.reported.$metadata.TimeToSleep.$lastUpdatedVersion doesn't match");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_01()
        {

            var testString = "{\"type\":6}";
            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testString, typeof(InvocationReceiveMessage));

            Assert.IsNotNull(dserResult, "Deserialization returned a null object");

            OutputHelper.WriteLine("");
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

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testString, typeof(InvocationReceiveMessage));

            Assert.IsNotNull(dserResult, "Deserialization returned a null object");

            Assert.AreEqual(dserResult.type, 1, "type value is not correct");
            Assert.AreEqual(dserResult.invocationId, "123", "invocationId value is not correct");
            Assert.AreEqual(dserResult.target, "Send", "target value is not correct");

            Assert.AreEqual((int)dserResult.arguments[0], 42, "arguments[0] value is not correct");
            Assert.AreEqual((string)dserResult.arguments[1], "Test Message", "arguments[1] value is not correct");

            Assert.AreEqual(dserResult.headers.Count, 1, "headers count is not correct");

            OutputHelper.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_03()
        {
            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testInvocationReceiveMessage, typeof(InvocationReceiveMessage));

            Assert.IsNotNull(dserResult, "Deserialization returned a null object");

            Assert.AreEqual(dserResult.type, 1, "type value is not correct");
            Assert.AreEqual(dserResult.target, "ReceiveAdvancedMessage", "target value is not correct");

            Assert.AreEqual((int)dserResult.arguments[2], 3, "arguments[2] value is not correct");
            Assert.IsInstanceOfType(dserResult.arguments, typeof(ArrayList), "arguments type it's wrong after deserialization");
            Assert.AreEqual(dserResult.arguments.Count, 3, $"number of arguments is different than expected: {dserResult.arguments.Count}");

            Hashtable arg0 = (Hashtable)dserResult.arguments[0];
            Assert.IsNotNull(arg0, "Deserializing arg 0 returned a null object");

            Hashtable car0 = (Hashtable)arg0["car"];
            Assert.IsNotNull(car0, "Deserializing car from arg 0 returned a null object");

            Assert.AreEqual(arg0["name"] as string, "Monica", $"arg0.name has unexpected value: {arg0["name"] as string}");
            Assert.AreEqual((int)arg0["age"], 22, $"arg0.age has unexpected value: {(int)arg0["age"]}");
            Assert.AreEqual((int)arg0["gender"], 1, $"arg0.gender has unexpected value: {(int)arg0["gender"]}");

            Assert.AreEqual((int)car0["age"], 5, $"car0.age has unexpected value: {(int)car0["age"]}");
            Assert.AreEqual(car0["model"] as string, "Tesla", $"car0.model has unexpected value: {car0["model"] as string}");

            Hashtable arg1 = (Hashtable)dserResult.arguments[1];
            Assert.IsNotNull(arg1, "Deserializing arg 1 returned a null object");

            Hashtable car1 = (Hashtable)arg1["car"];
            Assert.IsNotNull(car1, "Deserializing car from arg 1 returned a null object");

            Assert.AreEqual(arg1["name"] as string, "Grandpa", $"arg1.name has unexpected value: {arg1["name"] as string}");
            Assert.AreEqual((int)arg1["age"], 88, $"arg1.age has unexpected value: {(int)arg1["age"]}");
            Assert.AreEqual((int)arg1["gender"], 0, $"arg1.gender has unexpected value: {(int)arg1["gender"]}");

            Assert.AreEqual((int)car1["age"], 35, $"car1.age has unexpected value: {(int)car1["age"]}");
            Assert.AreEqual(car1["model"] as string, "Buick", $"car1.model has unexpected value: {car1["model"] as string}");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_04()
        {
            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testInvocationReceiveMessage, typeof(InvocationReceiveMessage));

            Assert.IsNotNull(dserResult, "Deserialization returned a null object");

            Assert.AreEqual(dserResult.type, 1, "type value is not correct");
            Assert.AreEqual(dserResult.target, "ReceiveAdvancedMessage", "target value is not correct");

            Assert.AreEqual((int)dserResult.arguments[2], 3, "arguments[2] value is not correct");
            Assert.IsInstanceOfType(dserResult.arguments, typeof(ArrayList), "arguments type it's wrong after deserialization");
            Assert.AreEqual(dserResult.arguments.Count, 3, $"number of arguments is different than expected: {dserResult.arguments.Count}");

            OutputHelper.WriteLine("Serializing dserResult.arguments[0]");

            Person2 person1 = (Person2)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[0]), typeof(Person2));
            Assert.IsNotNull(person1, "Deserializing person1 returned a null object");

            OutputHelper.WriteLine("Serializing dserResult.arguments[1]");

            Person2 person2 = (Person2)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[1]), typeof(Person2));
            Assert.IsNotNull(person2, "Deserializing person2 returned a null object");

            Assert.AreEqual(person1.name, "Monica", $"person1.name has unexpected value: {person1.name}");
            Assert.AreEqual(person1.age, 22, $"person1.age has unexpected value: {person1.age}");
            Assert.AreEqual((int)person1.gender, (int)Gender.Female, $"person1.gender has unexpected value: {person1.gender}");

            Assert.AreEqual(person1.car.age, 5, $"person1.car.age has unexpected value: {person1.car.age}");
            Assert.AreEqual(person1.car.model, "Tesla", $"person1.car.model has unexpected value: {person1.car.model}");


            Assert.AreEqual(person2.name, "Grandpa", $"person2.name has unexpected value: {person2.name}");
            Assert.AreEqual(person2.age, 88, $"person2.age has unexpected value: {person2.age}");
            Assert.AreEqual((int)person2.gender, (int)Gender.Male, $"person2.gender has unexpected value: {person2.gender}");

            Assert.AreEqual(person2.car.age, 35, $"person2.car.age has unexpected value: {person2.car.age}");
            Assert.AreEqual(person2.car.model, "Buick", $"person2.car.model has unexpected value: {person2.car.model}");

            OutputHelper.WriteLine($"Serializing dserResult.arguments[2]:{dserResult.arguments[2]}");

            var serializedObject = JsonConvert.SerializeObject(dserResult.arguments[2]);

            OutputHelper.WriteLine($"Serialized object is:>>{serializedObject}<<");

            int argsCount = (int)JsonConvert.DeserializeObject(serializedObject, typeof(int));

            Assert.AreEqual(argsCount, 3, $"argsCount has unexpected value: {argsCount}");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_05()
        {
            OutputHelper.WriteLine($"Starting CanDeserializeInvocationReceiveMessage_05");

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(@"{""type"":1,""target"":""ReceiveMessage"",""arguments"":[""I_am_a_string"",""I_am_another_string""]}", typeof(InvocationReceiveMessage));

            Assert.IsNotNull(dserResult, "Deserialization returned a null object");

            Assert.AreEqual(dserResult.type, 1, "type value is not correct");
            Assert.AreEqual(dserResult.target, "ReceiveMessage", "target value is not correct");

            Assert.IsInstanceOfType(dserResult.arguments, typeof(ArrayList), "arguments type it's wrong after deserialization");
            Assert.AreEqual(dserResult.arguments.Count, 2, $"number of arguments is different than expected: {dserResult.arguments.Count}");

            OutputHelper.WriteLine($"SerializingdserResult.arguments[0]:{dserResult.arguments[0]}");
            var serializedObject = JsonConvert.SerializeObject(dserResult.arguments[0]);
            OutputHelper.WriteLine($"Serialized object is:>>{serializedObject}<<");

            string arg0 = (string)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[0]), typeof(string));

            OutputHelper.WriteLine($"SerializingdserResult.arguments[0]:{dserResult.arguments[1]}");
            serializedObject = JsonConvert.SerializeObject(dserResult.arguments[1]);
            OutputHelper.WriteLine($"Serialized object is:>>{serializedObject}<<");

            string arg1 = (string)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dserResult.arguments[1]), typeof(string));

            Assert.AreEqual(arg0, "I_am_a_string", $"arg0 has unexpected value: {arg0}");
            Assert.AreEqual(arg1, "I_am_another_string", $"arg1 has unexpected value: {arg1}");
        }

        [TestMethod]
        public void CanDeserializeUnicodeData_01()
        {
            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(@"{ ""type"":3,""invocationId"":""1"",""error"":""Failed to invoke \u0027SendMessage\u0027 due to an error on the server. HubException: Method does not exist.""}", typeof(InvocationReceiveMessage));

            Assert.IsNotNull(dserResult, "Deserialization returned a null object");

            Assert.AreEqual(dserResult.type, 3, "type value is not correct");
            Assert.AreEqual(dserResult.invocationId, "1", "invocationId value is not correct");
            Assert.AreEqual(dserResult.error, "Failed to invoke \u0027SendMessage\u0027 due to an error on the server. HubException: Method does not exist.", "error value is not correct");
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

            var sentMessage = JsonConvert.SerializeObject(invocMessage);

            Assert.AreEqual(@"{""type"":1,""invocationId"":""0"",""target"":""Add"",""arguments"":[1,2]}", sentMessage, $"Sent message was >>{sentMessage}<<");
        }

        [TestMethod]
        public void DeserializeArrayList()
        {
            string correctValue = "{\"desired\":{\"Url\":\"https://ellerbachiotstorage.blob.core.windows.net/nano-containers\"," +
                "\"Authorization\":\"sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D\"," +
                "\"Files\":[\"Iot.Device.Bmxx80.pe\"]}}";


            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(correctValue, typeof(Hashtable));

            Hashtable desired = (Hashtable)hash["desired"];

            Assert.IsInstanceOfType(desired["Authorization"], typeof(string), "Authorization is not a string and it should be.");

            Assert.AreEqual("sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D", (string)desired["Authorization"], "Authorization string doesn't match original value.");

            ArrayList files = (ArrayList)desired["Files"];

            Assert.IsInstanceOfType(files[0], typeof(string));
            Assert.AreEqual("Iot.Device.Bmxx80.pe", (string)files[0]);
        }

        [TestMethod]
        public void DeserializeArrayListElements()
        {
            string correctValue = "{\"Url\":\"https://ellerbachiotstorage.blob.core.windows.net/nano-containers\"," +
                "\"Authorization\":\"sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D\"," +
                "\"Files\":[\"Iot.Device.Bmxx80.pe\"]}";

            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(correctValue, typeof(Hashtable));

            Assert.IsInstanceOfType(hash["Authorization"], typeof(string), "Authorization is not a string and it should be.");
            Assert.AreEqual("sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D", (string)hash["Authorization"], "Authorization string doesn't match original value.");

            ArrayList files = (ArrayList)hash["Files"];

            Assert.IsInstanceOfType(files[0], typeof(string));
            Assert.AreEqual("Iot.Device.Bmxx80.pe", (string)files[0]);
        }

        [TestMethod]
        public void SerializeObjectAsAProperty()
        {
            var correctValue = "{\"Led\":{\"value\":\"On\",\"nodeID\":\"14\"}}";
            JsonSerializeObjectAsProperty ledProp = new() { value = "On", nodeID = "14" };

            Hashtable twin = new()
            {
                { "Led", ledProp }
            };

            string json = JsonConvert.SerializeObject(twin);

            Assert.AreEqual(correctValue, json, "Serialize object as property fails");
        }

        [TestMethod]
        public void DeserializeSingleTypesClassDeserialization()
        {
            var json = "{\"OneByte\":42,\"OneSByte\":-42,\"OneInt16\":1234,\"OneUInt16\":5678,\"OneInt32\":-789012,\"OneUInt32\":78912,\"OneInt64\":-1234567,\"OneUInt64\":1234567,\"OneSingle\":34.45,\"OneDouble\":45678.23,\"OneBoolean\":true,\"TwoBoolean\":false}";

            var deser = JsonConvert.DeserializeObject(json, typeof(SingleTypesClassDeserialization)) as SingleTypesClassDeserialization;

            Assert.AreEqual((byte)42, deser.OneByte, "Byte");
            Assert.AreEqual((sbyte)-42, deser.OneSByte, "SByte");
            Assert.AreEqual((short)1234, deser.OneInt16, "Int16");
            Assert.AreEqual((ushort)5678, deser.OneUInt16, "UInt16");
            Assert.AreEqual(-789012, deser.OneInt32, "Int32");
            Assert.AreEqual(78912, deser.OneUInt32, "UInt32");
            Assert.AreEqual(-1234567, deser.OneInt64, "Int64");
            Assert.AreEqual(1234567, deser.OneUInt64, "UInt64");
            Assert.AreEqual((float)34.45, deser.OneSingle, "Single");
            Assert.AreEqual(45678.23, deser.OneDouble, "Double");
            Assert.IsTrue(deser.OneBoolean, "Boolean true");
            Assert.IsFalse(deser.TwoBoolean, "Boolean false");
        }

        [TestMethod]
        public void DeserializeArrayToDeserialize()
        {
            ArrayToDeserialize obj0 = new() { Prop1 = 1, Prop2 = "prop2", Prop3 = true, Prop4 = 67890123 };
            ArrayToDeserialize obj1 = new() { Prop1 = -42, Prop2 = "second2", Prop3 = false, Prop4 = 123456 };
            ArrayToDeserialize[] array = new[] { obj0, obj1 };

            var json = JsonConvert.SerializeObject(array);

            var deser = JsonConvert.DeserializeObject(json, typeof(ArrayToDeserialize[])) as ArrayToDeserialize[];

            Assert.AreEqual(deser.Length, array.Length, "Array length");
            Assert.AreEqual(deser[0].Prop1, obj0.Prop1);
            Assert.AreEqual(deser[0].Prop2, obj0.Prop2);
            Assert.AreEqual(deser[0].Prop3, obj0.Prop3);
            Assert.AreEqual(deser[0].Prop4, obj0.Prop4);
            Assert.AreEqual(deser[1].Prop1, obj1.Prop1);
            Assert.AreEqual(deser[1].Prop2, obj1.Prop2);
            Assert.AreEqual(deser[1].Prop3, obj1.Prop3);
            Assert.AreEqual(deser[1].Prop4, obj1.Prop4);
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
            JsonConvert.DeserializeObject(serInt64, typeof(SingleTypesClassDeserialization));
            JsonConvert.DeserializeObject(serUInt32, typeof(SingleTypesClassDeserialization));
            JsonConvert.DeserializeObject(serInt32, typeof(SingleTypesClassDeserialization));

            Assert.AreEqual(deserUInt64.OneUInt64, singleUInt64.OneUInt64);
            Assert.AreEqual(deserUInt64.OneInt64, singleUInt64.OneInt64);
            Assert.AreEqual(deserUInt64.OneUInt32, singleUInt64.OneUInt32);
            Assert.AreEqual(deserUInt64.OneInt32, singleUInt64.OneInt32);
        }

        [TestMethod]
        public void CompleHashtableArraysList()
        {
            string json = @"{""desired"":{""TimeToSleep"":2,""Files"":[{""FileName"":""https://ellerbachiotstorage.blob.core.windows.net/nano-containers/CountMeasurement.pe"",""Signature"":""4E-1E-12-45-C5-EB-EC-E3-86-D3-09-39-AE-E9-E8-81-97-A9-0E-DF-EE-D0-71-27-A7-3F-26-D0-4B-4E-CF-23""}],""Token"":""sp=r&st=2022-02-12T12:32:10Z&se=2023-11-01T20:32:10Z&spr=https&sv=2020-08-04&sr=c&sig=O32denO9Hw8mZ2OlNSBS%2FULuRn9RcArGDZ5%2BGvKgolM%3D"",""CodeVersion"":12,""UpdateTime"":120000,""$version"":43},""reported"":{""Firmware"":""nanoFramework"",""Sdk"":0.2,""TimeToSleep"":2,""CodeUpdated"":true,""CodeRunning"":true,""$version"":4353}}";

            Hashtable deser = (Hashtable)JsonConvert.DeserializeObject(json, typeof(Hashtable));

            Assert.IsNotNull(deser, "Deserialization returned a null object");
        }

        [TestMethod]
        public void SerializeCosmosDbObject_01()
        {
            var valueAsJsonString = @"{""_count"":1,""Databases"":[{""_users"":""users/"",""_ts"":1644173816,""id"":""HomeAutomation"",""_rid"":""MfAzAA=="",""_colls"":""colls/"",""_etag"":""\""000020002-0000-0a00-0000-620019f80000\"""",""_self"":""dbs/MFzAA==/""}],""_rid"":null}";

            CosmosDbDatabaseList dbObject = new CosmosDbDatabaseList
            {
                _count = 1,

                Databases = new CosmosDbDatabaseList.Database[1]
            };

            dbObject.Databases[0] = new CosmosDbDatabaseList.Database
            {
                id = "HomeAutomation",
                _rid = "MfAzAA==",
                _self = "dbs/MFzAA==/",
                _etag = "\"000020002-0000-0a00-0000-620019f80000\"",
                _colls = "colls/",
                _users = "users/",
                _ts = 1644173816
            };

            var serializedObject = JsonConvert.SerializeObject(dbObject);

            Assert.AreEqual(serializedObject, valueAsJsonString, $"Got >>{serializedObject}<<");
        }


        [TestMethod]
        public void SerializeCosmosDbObject_02()
        {
            var valueAsJsonString = @"{
  ""_rid"": """",
  ""Databases"": [
    {
            ""id"": ""HomeAutomation"",
      ""_rid"": ""MfAzAA=="",
      ""_self"": ""dbs/MFzAA==/"",
      ""_etag"": ""\""000020002-0000-0a00-0000-620019f80000\"""",
      ""_colls"": ""colls/"",
      ""_users"": ""users/"",
      ""_ts"": 1644173816
    }
  ],
  ""_count"": 1
}";

            var result = (CosmosDbDatabaseList)JsonConvert.DeserializeObject(valueAsJsonString, typeof(CosmosDbDatabaseList));

            Assert.AreEqual(result._rid, "", "result._rid has wrong value");
            Assert.AreEqual(result._count, 1, "result._count has wrong value");

            Assert.AreEqual(result.Databases.Length, 1, "Databases.Length count is wrong");

            Assert.AreEqual(result.Databases[0].id, "HomeAutomation", $"Database.id is wrong, got {result.Databases[0].id}");
            Assert.AreEqual(result.Databases[0]._rid, "MfAzAA==", $"Database._rid is wrong, got {result.Databases[0]._rid}");
            Assert.AreEqual(result.Databases[0]._self, "dbs/MFzAA==/", $"Database._self is wrong, got {result.Databases[0]._self}");
            Assert.AreEqual(result.Databases[0]._etag, "\"000020002-0000-0a00-0000-620019f80000\"", $"Database._etag is wrong, got {result.Databases[0]._etag}");
            Assert.AreEqual(result.Databases[0]._colls, "colls/", $"Database._colls is wrong, got {result.Databases[0]._colls}");
            Assert.AreEqual(result.Databases[0]._users, "users/", $"Database._colls is wrong, got {result.Databases[0]._users}");
            Assert.AreEqual(result.Databases[0]._ts, 1644173816, $"Database._ts is wrong, got {result.Databases[0]._ts}");
        }

        [TestMethod]
        public void SerializeStringWithNewLine_Should_ReturnWithoutNewLine()
        {
            var inputData = "multiline\nstring";
            var expectedJson = "\"multiline\\nstring\"";

            var json = JsonConvert.SerializeObject(inputData);

            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void SerializeStringWithReturn_Should_ReturnWithoutNewLine()
        {
            var inputData = "multiline\rstring";
            var expectedJson = "\"multiline\\rstring\"";

            var json = JsonConvert.SerializeObject(inputData);

            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void DeserializeStringWithNewLine_Should_ReturnWithoutNewLine()
        {
            var inputData = "\"multiline\\nstring\"";
            var expectedValue = "multiline\nstring";

            var result = (string)JsonConvert.DeserializeObject(inputData, typeof(string));

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void DeserializeStringWithReturn_Should_ReturnWithoutNewLine()
        {
            var inputData = "\"multiline\\rstring\"";
            var expectedValue = "multiline\rstring";

            var result = (string)JsonConvert.DeserializeObject(inputData, typeof(string));

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void CanSerializeBoxedEnum()
        {
            Hashtable values = new Hashtable
            {
                { "gender", Gender.Male }
            };

            Assert.AreEqual(JsonConvert.SerializeObject(values), "{\"gender\":0}");
        }

        private static readonly string testInvocationReceiveMessage = @"{
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

        private static readonly string s_AzureTwinsJsonTestPayload = @"{
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

        [TestMethod]
        public void DeserializeObjectWithStringContainingNonAsciiChars()
        {
            var input = new ThingWithString { Value = "zażółć gęślą jaźń \u0165 \u0f65 \uff11" };
            var str = JsonConvert.SerializeObject(input);
            var result = (ThingWithString)JsonConvert.DeserializeObject(str, typeof(ThingWithString));
            Assert.AreEqual(input.Value, result.Value);
        }

    }

    #region Test classes

    public class TwinPayloadTestClass : TwinPayload
    {
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

    // Classes to more thoroughly test array serialization/deserialization
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

    public class CosmosDbDatabaseList
    {
        public string _rid { get; set; }
        public Database[] Databases { get; set; }
        public int _count { get; set; }

        public class Database
        {
            public string id { get; set; }
            public string _rid { get; set; }
            public string _self { get; set; }
            public string _etag { get; set; }
            public string _colls { get; set; }
            public string _users { get; set; }
            public int _ts { get; set; }
        }
    }

    #endregion
}
