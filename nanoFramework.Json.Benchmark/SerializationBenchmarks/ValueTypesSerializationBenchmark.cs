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
    public class ValueTypesSerializationBenchmark : BaseIterationBenchmark
    {
        short shortTestValue;
        TimeSpan timeSpanTestValue;
        float floatTestValue;
        double doubleTestValue;
        DateTime dateTimeTestValue;
        long longTestValue;
        Gender enumValue;
        Hashtable boxedEnum;

        protected override int IterationCount => 100;

        [Setup]
        public void Setup()
        {
            var random = new Random();
            shortTestValue = (short)random.Next(short.MaxValue);
            timeSpanTestValue = TimeSpan.FromMilliseconds(random.Next());
            floatTestValue = (float)random.NextDouble();
            doubleTestValue = random.NextDouble();
            dateTimeTestValue = new DateTime(random.Next(1000) + 1601, random.Next(11) + 1, random.Next(20) + 1);
            longTestValue = random.Next();
            enumValue = Gender.Male;
            boxedEnum= new Hashtable
            {
                { "gender", Gender.Male }
            };
        }

        [Benchmark]
        public void Short()
        {
            RunInIteration(() =>
            {
                JsonConvert.SerializeObject(shortTestValue);
            });
        }

        [Benchmark]
        public void TimeSpanT()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(timeSpanTestValue);
            });
        }

        [Benchmark]
        public void Float()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(floatTestValue);
            });
        }

        [Benchmark]
        public void Double()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(doubleTestValue);
            });
        }

        [Benchmark]
        public void DateTimeT()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(dateTimeTestValue);
            });
        }

        [Benchmark]
        public void Long()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(longTestValue);
            });
        }

        [Benchmark]
        public void Enum()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(enumValue);
            });
        }

        [Benchmark]
        public void EnumBoxed()
        {
            RunInIteration(() =>
            {
                _ = JsonConvert.SerializeObject(boxedEnum);
            });
        }
    }
}
