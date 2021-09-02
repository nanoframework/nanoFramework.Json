using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace nanoFramework.Json.Test
{

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
        public int                    CompanyID            { get; set; }
        public string                 CompanyName          { get; set; }
    }
    public class JsonTestEmployee
    {
        public int                     EmployeeID          { get; set; }
        public string                  EmployeeName        { get; set; }
        public JsonTestCompany         CurrentEmployer     { get; set; }
        public JsonTestCompany[]       FormerEmployers     { get; set; }
    }
    public class JsonTestTown
    {
        public int                     TownID              { get; set; }
        public string                  TownName            { get; set; }
        public JsonTestCompany[]       CompaniesInThisTown { get; set; }
        public JsonTestEmployee[]      EmployeesInThisTown { get; set; }
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

            var result = JsonConvert.SerializeObject(myTown);
            Debug.WriteLine($"Serialized Array: {result}");

            JsonTestTown dserResult = (JsonTestTown)JsonConvert.DeserializeObject(result, typeof(JsonTestTown));
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

            var result = JsonConvert.SerializeObject(intArray);
            Debug.WriteLine($"Serialized Array: {result}");

            var dserResult = (int[])JsonConvert.DeserializeObject(result, typeof(int[]));
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

            var serialized = JsonConvert.SerializeObject(source);
            Debug.WriteLine($"Serialized Object: {serialized}");

            var dserResult = (JsonTestClassChild)JsonConvert.DeserializeObject(serialized, typeof(JsonTestClassChild));

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
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassComplex)JsonConvert.DeserializeObject(result, typeof(JsonTestClassComplex));
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
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassFloat)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloat));
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
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassFloat)JsonConvert.DeserializeObject(result, typeof(JsonTestClassFloat));
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
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");

            var dserResult = (JsonTestClassDouble)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDouble));
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
            var result = JsonConvert.SerializeObject(test);
            Debug.WriteLine($"Serialized Object: {result}");


            var dserResult = (JsonTestClassDouble)JsonConvert.DeserializeObject(result, typeof(JsonTestClassDouble));
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

            string json = JsonConvert.SerializeObject(array);
            
            string correctValue = "[{\"collection\":[1,null,2,\"blah\",false],\"nulltest\":null,\"stringtest\":\"hello world\"}]";

            Assert.Equal(json, correctValue, "Values did not match");

            Debug.WriteLine("");
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

            string jsonString = JsonConvert.SerializeObject(collection);

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

            string json = JsonConvert.SerializeObject(person);
            
            string correctValue = "{\"Address\":null,\"ArrayProperty\":[\"hello\",\"world\"],\"ID\":27,\"Birthday\":\"1988-04-23T00:00:00.000Z\",\"LastName\":\"Doe\",\"Friend\""
                + ":{\"Address\":\"123 Some St\",\"ArrayProperty\":[\"hi\",\"planet\"],\"ID\":2,\"Birthday\":\"1983-07-03T00:00:00.000Z\",\"LastName\":\"Smith\",\"Friend\":null,\"FirstName\":\"Bob\"}"
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
            
            json = JsonConvert.SerializeObject(b);
            
            correctValue = "{\"Test2\":\"test2\",\"ID\":12,\"Test\":\"test\"}";

            Assert.Equal(json, correctValue, "Values for RealClass did not match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_01()
        {
            var testString = "{\"desired\":{\"TimeToSleep\":5,\"$version\":2},\"reported\":{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}}";

            var twinPayload = (TwinProperties)JsonConvert.DeserializeObject(testString, typeof(TwinProperties));

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
            TwinPayload twinPayload = (TwinPayload)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayload));

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
            TwinPayloadProperties twinPayload = (TwinPayloadProperties)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayloadProperties));

            Assert.NotNull(twinPayload, "Deserialization returned a null object");

            Assert.Equal(twinPayload.properties.desired.TimeToSleep, 30, "properties.desired.TimeToSleep doesn't match");
            Assert.Equal(twinPayload.properties.reported._metadata.Count, 3, "properties.reported._metadata collection count doesn't match");
            Assert.Equal(twinPayload.properties.desired._metadata.Count, 3, "properties.desired._metadata collection count doesn't match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeAzureTwinProperties_04()
        {
            Hashtable twinPayload = (Hashtable)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(Hashtable));

            Assert.NotNull(twinPayload, "Deserialization returned a null object");

            Assert.Equal((string)twinPayload["authenticationType"], "sas", "authenticationType doesn't match");
            Assert.Equal(((DateTime)twinPayload["statusUpdateTime"]).Ticks, DateTime.MinValue.Ticks, "statusUpdateTime doesn't match");
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

            // this requires an aproximation on the milleseconds
            // becasue .NET DateTime can't Parse milleseconds value bigger than 999
            DateTime desiredLastUpdated = new(2021, 06, 03, 05, 37, 11, 999);
            DateTime reportedLastUpdated = new(2021, 06, 03, 05, 52, 41, 999);

            Assert.Equal((DateTime)reportedMetadata["$lastUpdated"], reportedLastUpdated, "properties.reported.$metadata.$lastUpdated doesn't match");
            Assert.Equal((DateTime)desiredMetadata["$lastUpdated"], desiredLastUpdated, "properties.reported.$metadata.$lastUpdated doesn't match");

            Hashtable desiredTimeToSleep = (Hashtable)desiredMetadata["TimeToSleep"];

            // this requires an aproximation on the milleseconds
            // becasue .NET DateTime can't Parse milleseconds value bigger than 999
            DateTime desiredTimeToSleepUpdated = new(2021, 06, 03, 05, 37, 11, 999);

            Assert.Equal((DateTime)desiredTimeToSleep["$lastUpdated"], desiredTimeToSleepUpdated, "properties.reported.$metadata.TimeToSleep.$lastUpdated doesn't match");
            Assert.Equal((int)desiredTimeToSleep["$lastUpdatedVersion"], 7, "properties.reported.$metadata.TimeToSleep.$lastUpdatedVersion doesn't match");

            Debug.WriteLine("");
        }

        [TestMethod]
        public void CanDeserializeInvocationReceiveMessage_01()
        {

            var testString = "{\"type\":6}";

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testString, typeof(InvocationReceiveMessage));

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

            var dserResult = (InvocationReceiveMessage)JsonConvert.DeserializeObject(testString, typeof(InvocationReceiveMessage));

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
        public void DeserializeArrayList()
        {
            string correctValue = "{\"desired\":{\"Url\":\"https://ellerbachiotstorage.blob.core.windows.net/nano-containers\"," +
                "\"Authorization\":\"sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D\"," +
                "\"Files\":[\"Iot.Device.Bmxx80.pe\"]}}";
            
            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(correctValue, typeof(Hashtable));
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
           
            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(correctValue, typeof(Hashtable));
            
            Assert.IsType(typeof(string), hash["Authorization"], "Authorization is not a string and it should be.");
            Assert.Equal("sp=r&st=2021-06-12T09:11:53Z&se=2021-06-14T17:11:53Z&spr=https&sv=2020-02-10&sr=c&sig=rn125LiO55RSCoEs4IEaCgg%2BuXKETdEZQPygxVjCHiY%3D", (string)hash["Authorization"], "Authorization string doesn't match original value.");
            
            ArrayList files = (ArrayList)hash["Files"];
            
            Assert.IsType(typeof(string), files[0]);
            Assert.Equal("Iot.Device.Bmxx80.pe", (string)files[0]);
        }

        #region Test classes

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

        #endregion
    }
}
