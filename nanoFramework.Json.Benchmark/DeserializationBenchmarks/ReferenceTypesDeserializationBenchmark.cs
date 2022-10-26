using nanoFramework.Benchmark;
using System;
using System.Collections;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json.Benchmark.Base;
using nanoFramework.Json.Test.Shared;

namespace nanoFramework.Json.Benchmark.DeserializationBenchmarks
{
    [IterationCount(5)]
    public class ReferenceTypesDeserializationBenchmark : BaseIterationBenchmark
    {
        const string IntArrayJson = "[405421362,1082483948,1131707654,345242860,1111968802]";
        const string ShortArrayJson = "[12345,25463,22546,18879,12453]";
        const string StringJson = "some string";
        const string ArrayListJson = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[-1,null,24.565657576,\"blah\",false]}]";
        const string s_AzureTwinsJsonTestPayload = @"{
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

        const string NestedClassJson = "{\"FirstName\":\"John\",\"LastName\":\"Doe\",\"ArrayProperty\":[\"hello\",\"world\"],\"Address\":null,\"Birthday\":\"1988-04-23T00:00:00.0000000Z\",\"ID\":27,\"Friend\":{\"FirstName\":\"Bob\",\"LastName\":\"Smith\",\"ArrayProperty\":[\"hi\",\"planet\"],\"Address\":\"123 Some St\",\"Birthday\":\"1983-07-03T00:00:00.0000000Z\",\"ID\":2,\"Friend\":null}}";
        const string ComplexArrayJson = "{\"TownID\":1,\"EmployeesInThisTown\":[{\"CurrentEmployer\":{\"CompanyID\":3,\"CompanyName\":\"CCC Amalgamated Industries\"},\"EmployeeID\":1,\"FormerEmployers\":[{\"CompanyID\":2,\"CompanyName\":\"BBB Amalgamated Industries\"},{\"CompanyID\":5,\"CompanyName\":\"EEE Amalgamated Industries\"}],\"EmployeeName\":\"John Smith\"},{\"CurrentEmployer\":{\"CompanyID\":7,\"CompanyName\":\"GGG Amalgamated Industries\"},\"EmployeeID\":1,\"FormerEmployers\":[{\"CompanyID\":4,\"CompanyName\":\"DDD Amalgamated Industries\"},{\"CompanyID\":1,\"CompanyName\":\"AAA Amalgamated Industries\"},{\"CompanyID\":6,\"CompanyName\":\"FFF Amalgamated Industries\"}],\"EmployeeName\":\"Jim Smith\"}],\"TownName\":\"myTown\",\"CompaniesInThisTown\":[{\"CompanyID\":1,\"CompanyName\":\"AAA Amalgamated Industries\"},{\"CompanyID\":2,\"CompanyName\":\"BBB Amalgamated Industries\"},{\"CompanyID\":3,\"CompanyName\":\"CCC Amalgamated Industries\"},{\"CompanyID\":4,\"CompanyName\":\"DDD Amalgamated Industries\"},{\"CompanyID\":5,\"CompanyName\":\"EEE Amalgamated Industries\"},{\"CompanyID\":6,\"CompanyName\":\"FFF Amalgamated Industries\"},{\"CompanyID\":7,\"CompanyName\":\"GGG Amalgamated Industries\"},{\"CompanyID\":8,\"CompanyName\":\"HHH Amalgamated Industries\"}]}";

        protected override int IterationCount => 20;

        // [Benchmark]
        // public void IntArray()
        // {
        //     RunInIteration(() =>
        //     {
        //         JsonConvert.DeserializeObject(IntArrayJson, typeof(int[]));
        //     });
        // }

        [Benchmark]
        public void ArrayList()
        {
            RunInIteration(() =>
            {
                JsonConvert.DeserializeObject(ArrayListJson, typeof(ArrayList));
            });
        }

        [Benchmark]
        public void ComplexObjectAzureTwinPayload()
        {
            JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayload));
        }

        // [Benchmark]
        // public void ShortArray()
        // {
        //     RunInIteration(() =>
        //     {
        //         JsonConvert.DeserializeObject(ShortArrayJson, typeof(short[]));
        //     });
        // }

        [Benchmark]
        public void String()
        {
            RunInIteration(() =>
            {
                JsonConvert.DeserializeObject(StringJson, typeof(string));
            });
        }

        [Benchmark]
        public void NestedClass()
        {
            JsonConvert.DeserializeObject(NestedClassJson, typeof(Person));
        }

        [Benchmark]
        public void ComplexArrayObject()
        {
            JsonConvert.DeserializeObject(ComplexArrayJson, typeof(JsonTestTown));
        }
    }
}
