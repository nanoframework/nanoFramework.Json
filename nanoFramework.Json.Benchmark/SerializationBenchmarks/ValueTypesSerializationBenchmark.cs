using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json.Benchmark.Base;
using nanoFramework.Json.Benchmark.TestClasses;
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
                var value = JsonConvert.SerializeObject(timeSpanTestValue);
            });
        }

        [Benchmark]
        public void Float()
        {
            RunInIteration(() =>
            {
                var value = JsonConvert.SerializeObject(floatTestValue);
            });
        }

        [Benchmark]
        public void Double()
        {
            RunInIteration(() =>
            {
                var value = JsonConvert.SerializeObject(doubleTestValue);
            });
        }

        [Benchmark]
        public void DateTimeT()
        {
            RunInIteration(() =>
            {
                var value = JsonConvert.SerializeObject(dateTimeTestValue);
            });
        }

        [Benchmark]
        public void Long()
        {
            RunInIteration(() =>
            {
                var value = JsonConvert.SerializeObject(longTestValue);
            });
        }

        [Benchmark]
        public void Enum()
        {
            RunInIteration(() =>
            {
                var value = JsonConvert.SerializeObject(enumValue);
            });
        }

        [Benchmark]
        public void EnumBoxed()
        {
            RunInIteration(() =>
            {
                var value = JsonConvert.SerializeObject(boxedEnum);
            });
        }
    }
}
