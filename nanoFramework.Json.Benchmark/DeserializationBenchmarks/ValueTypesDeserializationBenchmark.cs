using nanoFramework.Benchmark;
using System;

namespace nanoFramework.Json.Benchmark.DeserializationBenchmarks
{
    public class ValueTypesDeserializationBenchmark
    {
        const string ShortJson = "[9134]";

        [Benchmark]
        public void Short()
        {
            // TODO: Bug return type is int
            var value = (int)JsonConvert.DeserializeObject(ShortJson, typeof(short));
        }
    }
}
