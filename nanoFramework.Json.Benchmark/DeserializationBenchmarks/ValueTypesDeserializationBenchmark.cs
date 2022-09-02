using nanoFramework.Benchmark;
using nanoFramework.Json.Benchmark.TestClasses;
using System;
using System.Collections;

namespace nanoFramework.Json.Benchmark.DeserializationBenchmarks
{
    public class ValueTypesDeserializationBenchmark
    {
        const string ShortJson = "[9134]";
        const string TimeSpanJson = "[\"14.04:16:48.8920000\"]";
        const string FloatJson = "[0.04608]";
        const string DoubleJson = "[0.82747237]";
        const string DateTimeJson = "[\"2059-11-01T00:00:00.0000000Z\"]";
        const string LongJson = "[284268484]";
        const string EnumJson = "0";
        const string BoxedEnumJson = "{\"gender\":0}";

        [Benchmark]
        public void Short()
        {
            // TODO: Return value should be of type short
            var value = (int)JsonConvert.DeserializeObject(ShortJson, typeof(short));
        }

        [Benchmark]
        public void TimeSpanT()
        {
            // TODO: Return value should be of type timespan
            var value = JsonConvert.DeserializeObject(TimeSpanJson, typeof(TimeSpan));
        }

        [Benchmark]
        public void Float()
        {
            // TODO: Return value should be of type float
            var value = (double)JsonConvert.DeserializeObject(FloatJson, typeof(float));
        }

        [Benchmark]
        public void Double()
        {
            var value = (double)JsonConvert.DeserializeObject(DoubleJson, typeof(double));
        }

        [Benchmark]
        public void DateTimeT()
        {
            var value = (DateTime)JsonConvert.DeserializeObject(DateTimeJson, typeof(DateTime));
        }

        [Benchmark]
        public void Long()
        {
            // TODO: Return value should be of type long
            var value = (int)JsonConvert.DeserializeObject(LongJson, typeof(long));
        }

        //[Benchmark]
        //public void Enum()
        //{
            // TODO: Throws exception
            //var value = (Gender)JsonConvert.DeserializeObject(EnumJson, typeof(Gender));
        //}

        [Benchmark]
        public void EnumBoxed()
        {
            var value = (Hashtable)JsonConvert.DeserializeObject(BoxedEnumJson, typeof(Hashtable));
        }
    }
}