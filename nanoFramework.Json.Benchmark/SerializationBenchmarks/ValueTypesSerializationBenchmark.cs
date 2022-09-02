using nanoFramework.Benchmark;
using nanoFramework.Json.Benchmark.TestClasses;
using System;
using System.Collections;
using System.Text;

namespace nanoFramework.Json.Benchmark.SerializationBenchmarks
{
    public class ValueTypesSerializationBenchmark
    {
        short shortTestValue;
        TimeSpan timeSpanTestValue;
        float floatTestValue;
        double doubleTestValue;
        DateTime dateTimeTestValue;
        long longTestValue;
        Gender enumValue;
        Hashtable boxedEnum;

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
            JsonConvert.SerializeObject(shortTestValue);
        }

        [Benchmark]
        public void TimeSpanT()
        {
            var value = JsonConvert.SerializeObject(timeSpanTestValue);
        }

        [Benchmark]
        public void Float()
        {
            var value = JsonConvert.SerializeObject(floatTestValue);
        }

        [Benchmark]
        public void Double()
        {
            var value = JsonConvert.SerializeObject(doubleTestValue);
        }

        [Benchmark]
        public void DateTimeT()
        {
            var value = JsonConvert.SerializeObject(dateTimeTestValue);
        }

        [Benchmark]
        public void Long()
        {
            var value = JsonConvert.SerializeObject(longTestValue);
        }

        [Benchmark]
        public void Enum()
        {
            var value = JsonConvert.SerializeObject(enumValue);
        }

        [Benchmark]
        public void EnumBoxed()
        {
            var value = JsonConvert.SerializeObject(boxedEnum);
        }
    }
}
