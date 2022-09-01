using nanoFramework.Benchmark;
using nanoFramework.Json.Benchmark.TestClasses;
using System;
using System.Collections;

namespace nanoFramework.Json.Benchmark.SerializationBenchmarks
{
    public class ReferenceTypesSerializationBenchmark
    {
        const string IntArrayJson = "[405421362,1082483948,1131707654,345242860,1111968802]";
        const string ArrayListJson = "[{\"stringtest\":\"hello world\",\"nulltest\":null,\"collection\":[-1,null,24.565657576,\"blah\",false]}]";
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

        [Benchmark]
        public void IntArray()
        {
            var dserResult = (int[])JsonConvert.DeserializeObject(IntArrayJson, typeof(int[]));
        }

        [Benchmark]
        public void ArrayList()
        {
            ArrayList arrayList = (ArrayList)JsonConvert.DeserializeObject(ArrayListJson, typeof(ArrayList));
        }

        [Benchmark]
        public void ComplexObjectAzureTwinPayload()
        {
            TwinPayload twinPayload = (TwinPayload)JsonConvert.DeserializeObject(s_AzureTwinsJsonTestPayload, typeof(TwinPayload));
        }
    }
}
