//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json.Benchmark.Base;
using System;
using System.Collections;

namespace nanoFramework.Json.Benchmark.DeserializationBenchmarks
{
    [IterationCount(5)]
    public class ValueTypesDeserializationBenchmark : BaseIterationBenchmark
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
            RunInIteration(() =>
            {
                // TODO: Return value should be of type short
                JsonConvert.DeserializeObject(ShortJson, typeof(short));
            });
        }

        [Benchmark]
        public void TimeSpanT()
        {
            RunInIteration(() =>
            {
                // TODO: Return value should be of type timespan
                JsonConvert.DeserializeObject(TimeSpanJson, typeof(TimeSpan));
            });
        }

        [Benchmark]
        public void Float()
        {
            RunInIteration(() =>
            {
                // TODO: Return value should be of type float
                JsonConvert.DeserializeObject(FloatJson, typeof(float));
            });
        }

        [Benchmark]
        public void Double()
        {
            RunInIteration(() =>
            {
                JsonConvert.DeserializeObject(DoubleJson, typeof(double));
            });
        }

        [Benchmark]
        public void DateTimeT()
        {
            RunInIteration(() =>
            {
                JsonConvert.DeserializeObject(DateTimeJson, typeof(DateTime));
            });
        }

        [Benchmark]
        public void Long()
        {
            RunInIteration(() =>
            {
                // TODO: Return value should be of type long
                JsonConvert.DeserializeObject(LongJson, typeof(long));
            });
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
            RunInIteration(() =>
            {
                JsonConvert.DeserializeObject(BoxedEnumJson, typeof(Hashtable));
            });
        }
    }
}
