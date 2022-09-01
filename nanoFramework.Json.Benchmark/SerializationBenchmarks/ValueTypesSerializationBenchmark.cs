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
        }

        [Benchmark]
        public void Short()
        {
            JsonConvert.SerializeObject(shortTestValue);
        }

        [Benchmark]
        public void TimeSpanT()
        {
            JsonConvert.SerializeObject(timeSpanTestValue);
        }

        [Benchmark]
        public void Float()
        {
            JsonConvert.SerializeObject(floatTestValue);
        }

        [Benchmark]
        public void Double()
        {
            JsonConvert.SerializeObject(doubleTestValue);
        }

        [Benchmark]
        public void DateTimeT()
        {
            JsonConvert.SerializeObject(dateTimeTestValue);
        }

        [Benchmark]
        public void Long()
        {
            JsonConvert.SerializeObject(longTestValue);
        }

        [Benchmark]
        public void EnumBoxed()
        {
            Hashtable values = new Hashtable();
            values.Add("gender", Gender.Male);
            JsonConvert.SerializeObject(values);
        }
    }
}
